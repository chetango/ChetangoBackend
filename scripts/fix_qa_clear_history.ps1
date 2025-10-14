param(
  [string]$ServerInstance = 'U-89P5FG3\\SQLEXPRESS'
)

$ErrorActionPreference = 'Stop'

# Paths
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path | Join-Path -ChildPath '..' | Resolve-Path
Push-Location $repoRoot
$infraProj = '.\\Chetango.Infrastructure\\Chetango.Infrastructure.csproj'
$apiProj   = '.\\Chetango.Api\\Chetango.Api.csproj'
$logDir = Join-Path $repoRoot 'LogdeCambios'
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }
$logFile = Join-Path $logDir 'ef_fix_QA_clear_history.log'

# Build QA connection string
$conn = "Server=$ServerInstance;Database=ChetangoDB_QA;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"

Add-Type -AssemblyName System.Data

function Get-QaCounts {
  $r = @{ Tables = 0; History = 0 }
  $c = New-Object System.Data.SqlClient.SqlConnection $conn
  try {
    $c.Open()
    $cmd = $c.CreateCommand()
    $cmd.CommandText = 'SELECT COUNT(*) FROM sys.tables'; $r.Tables = [int]$cmd.ExecuteScalar()
    try { $cmd.CommandText = 'SELECT COUNT(*) FROM __EFMigrationsHistory'; $r.History = [int]$cmd.ExecuteScalar() } catch { $r.History = 0 }
  } catch { "Connection error: $($_.Exception.Message)" | Tee-Object -FilePath $logFile -Append | Out-Host }
  finally { $c.Close() }
  return $r
}

function Drop-HistoryIfExists {
  $c = New-Object System.Data.SqlClient.SqlConnection $conn
  try {
    $c.Open()
    $cmd = $c.CreateCommand()
    $cmd.CommandText = "IF OBJECT_ID(N'__EFMigrationsHistory','U') IS NOT NULL DROP TABLE [__EFMigrationsHistory];"
    [void]$cmd.ExecuteNonQuery()
    "Dropped __EFMigrationsHistory (if existed) on ChetangoDB_QA" | Tee-Object -FilePath $logFile -Append | Out-Host
  } catch { "Error dropping __EFMigrationsHistory: $($_.Exception.Message)" | Tee-Object -FilePath $logFile -Append | Out-Host }
  finally { $c.Close() }
}

function Apply-MigrationsTarget {
  $prevEnv = $env:ASPNETCORE_ENVIRONMENT; $env:ASPNETCORE_ENVIRONMENT = 'QA'
  try {
    $args = @('ef','database','update','20250917211543_AlignSnapshot_SeedOID','--project',$infraProj,'--startup-project',$apiProj,'--context','Chetango.Infrastructure.Persistence.ChetangoDbContext','--no-build','--connection',$conn)
    "Applying EF migrations to QA up to 20250917211543_AlignSnapshot_SeedOID (ASPNETCORE_ENVIRONMENT=QA)..." | Tee-Object -FilePath $logFile -Append | Out-Host
    & dotnet @args 2>&1 | Tee-Object -FilePath $logFile -Append | Out-Host
  } finally { $env:ASPNETCORE_ENVIRONMENT = $prevEnv }
}

function Ensure-IdempotentScript {
  $scriptPath = Join-Path $logDir 'ef_script_QA_Prod.sql'
  if (-not (Test-Path $scriptPath)) {
    $prevEnv = $env:ASPNETCORE_ENVIRONMENT; $env:ASPNETCORE_ENVIRONMENT = 'QA'
    try {
      "Generating idempotent script ef_script_QA_Prod.sql (ASPNETCORE_ENVIRONMENT=QA)..." | Tee-Object -FilePath $logFile -Append | Out-Host
      & dotnet ef migrations script --idempotent --project $infraProj --startup-project $apiProj --context Chetango.Infrastructure.Persistence.ChetangoDbContext --from 0 --to 20250917211543_AlignSnapshot_SeedOID --no-build --output $scriptPath 2>&1 | Tee-Object -FilePath $logFile -Append | Out-Host
    } finally { $env:ASPNETCORE_ENVIRONMENT = $prevEnv }
  }
  return $scriptPath
}

function Invoke-SqlScriptBatches([string]$scriptPath) {
  $sql = Get-Content -Raw -Path $scriptPath
  # Split on GO separators (line with only GO)
  $batches = [System.Text.RegularExpressions.Regex]::Split($sql, "(?im)^\s*GO\s*;?\s*$")
  $c = New-Object System.Data.SqlClient.SqlConnection $conn
  $c.Open()
  try {
    $i = 0
    foreach ($b in $batches) {
      $batch = $b.Trim()
      if ([string]::IsNullOrWhiteSpace($batch)) { continue }
      $i++
      $cmd = $c.CreateCommand()
      $cmd.CommandTimeout = 0
      $cmd.CommandText = $batch
      try {
        [void]$cmd.ExecuteNonQuery()
      } catch {
        "Error executing batch #${i}: $($_.Exception.Message)" | Tee-Object -FilePath $logFile -Append | Out-Host
        throw
      }
    }
    "Executed $i SQL batches from $scriptPath" | Tee-Object -FilePath $logFile -Append | Out-Host
  } finally { $c.Close() }
}

# Ensure DB exists
try {
  $master = New-Object System.Data.SqlClient.SqlConnection ("Server=$ServerInstance;Database=master;Integrated Security=True;TrustServerCertificate=True;")
  $master.Open()
  $cmd = $master.CreateCommand(); $cmd.CommandText = "IF DB_ID('ChetangoDB_QA') IS NULL CREATE DATABASE [ChetangoDB_QA];"; [void]$cmd.ExecuteNonQuery(); $master.Close()
  "Ensured ChetangoDB_QA exists" | Tee-Object -FilePath $logFile -Append | Out-Host
} catch { "Error ensuring DB: $($_.Exception.Message)" | Tee-Object -FilePath $logFile -Append | Out-Host }

# Diagnose
$before = Get-QaCounts
"Before -> Tables=$($before.Tables) History=$($before.History)" | Tee-Object -FilePath $logFile -Append | Out-Host

# If we have history but no tables -> clear history
if ($before.Tables -eq 0 -and $before.History -gt 0) {
  Drop-HistoryIfExists
}

# Apply target migrations
Apply-MigrationsTarget

# Re-check
$mid = Get-QaCounts
"After EF -> Tables=$($mid.Tables) History=$($mid.History)" | Tee-Object -FilePath $logFile -Append | Out-Host

# If still no tables, execute idempotent script directly
if ($mid.Tables -eq 0) {
  "EF migration did not create tables; executing idempotent SQL script..." | Tee-Object -FilePath $logFile -Append | Out-Host
  $script = Ensure-IdempotentScript
  Invoke-SqlScriptBatches -scriptPath $script
}

# Final check
$final = Get-QaCounts
"Final -> Tables=$($final.Tables) History=$($final.History)" | Tee-Object -FilePath $logFile -Append | Out-Host

# Verify via helper
"Verifying QA DB after fix..." | Tee-Object -FilePath $logFile -Append | Out-Host
& .\scripts\check_db_details.ps1 -ServerInstance $ServerInstance | Out-Host

Pop-Location
"Done." | Tee-Object -FilePath $logFile -Append | Out-Host

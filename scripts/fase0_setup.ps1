param(
  [string]$ServerInstance = 'U-89P5FG3\\SQLEXPRESS'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $root '..')
Push-Location $repoRoot

function Ensure-Db($name) {
  $tsql = "IF DB_ID('$name') IS NULL CREATE DATABASE [$name];"
  if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
    sqlcmd -S $ServerInstance -E -b -Q $tsql | Out-Null
    Write-Host "Ensured database $name"
  } else {
    try {
      Add-Type -AssemblyName 'Microsoft.SqlServer.Smo' -ErrorAction SilentlyContinue | Out-Null
      $srv = New-Object Microsoft.SqlServer.Management.Smo.Server $ServerInstance
      if (-not $srv.Databases[$name]) { (New-Object Microsoft.SqlServer.Management.Smo.Database ($srv, $name)).Create(); Write-Host "Created database $name" }
      else { Write-Host "Database $name already exists" }
    } catch {
      Write-Warning "Could not ensure DB $name automatically. Please create it manually. $_"
    }
  }
}

# Ensure local DBs
Ensure-Db 'ChetangoDB_Dev'
Ensure-Db 'ChetangoDB_QA'
Ensure-Db 'ChetangoDB_Prod'

# Apply migrations to Dev
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet restore | Out-Host
if ($LASTEXITCODE) { throw 'dotnet restore failed' }

dotnet build --nologo | Out-Host
if ($LASTEXITCODE) { throw 'dotnet build failed' }

$infraProj = '.\\Chetango.Infrastructure\\Chetango.Infrastructure.csproj'
$apiProj = '.\\Chetango.Api\\Chetango.Api.csproj'

Write-Host 'Applying EF Core migrations to ChetangoDB_Dev...'
dotnet ef database update --project $infraProj --startup-project $apiProj --context Chetango.Infrastructure.Persistence.ChetangoDbContext --no-build | Out-Host
if ($LASTEXITCODE) { throw 'dotnet ef database update failed' }

# Generate idempotent scripts
Write-Host 'Generating idempotent SQL scripts...'
$logDir = '.\\LogdeCambios'
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }

# Excluding Dev seed (to use on QA/Prod)
dotnet ef migrations script --idempotent --project $infraProj --startup-project $apiProj --context Chetango.Infrastructure.Persistence.ChetangoDbContext --from 0 --to 20250917211543_AlignSnapshot_SeedOID --no-build --output (Join-Path $logDir 'ef_script_QA_Prod.sql') | Out-Host

# Full including Dev seed (for Dev refresh)
dotnet ef migrations script --idempotent --project $infraProj --startup-project $apiProj --context Chetango.Infrastructure.Persistence.ChetangoDbContext --no-build --output (Join-Path $logDir 'ef_script_AllIncludingDevSeed.sql') | Out-Host

Pop-Location
Write-Host 'Fase 0 setup completed.'

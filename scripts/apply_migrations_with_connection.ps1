param(
  [string]$ServerInstance = 'U-89P5FG3\\SQLEXPRESS'
)

$ErrorActionPreference = 'Stop'
$infra = ".\\Chetango.Infrastructure\\Chetango.Infrastructure.csproj"
$api = ".\\Chetango.Api\\Chetango.Api.csproj"
$logDir = Join-Path $PSScriptRoot '..\\LogdeCambios'
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }

function Update-Db {
  param(
    [string]$dbName,
    [string]$migration = '',
    [string]$envName = ''
  )
  $conn = if ($ServerInstance -eq '(localdb)\\MSSQLLocalDB') {
    "Server=(localdb)\\MSSQLLocalDB;Database=$dbName;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  } else {
    "Server=$ServerInstance;Database=$dbName;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
  $args = @('ef','database','update','--project',$infra,'--startup-project',$api,'--context','Chetango.Infrastructure.Persistence.ChetangoDbContext','--no-build','--connection',$conn)
  if ($migration) { $args = @('ef','database','update',$migration,'--project',$infra,'--startup-project',$api,'--context','Chetango.Infrastructure.Persistence.ChetangoDbContext','--no-build','--connection',$conn) }
  $log = Join-Path $logDir ("ef_update_" + $dbName + ".log")
  $prevEnv = $env:ASPNETCORE_ENVIRONMENT
  if ($envName) { $env:ASPNETCORE_ENVIRONMENT = $envName }
  try {
    Write-Host "Applying migrations to $dbName ($migration) with ASPNETCORE_ENVIRONMENT=$envName ..."
    dotnet @args 2>&1 | Tee-Object -FilePath $log | Out-Host
  }
  finally {
    $env:ASPNETCORE_ENVIRONMENT = $prevEnv
  }
}

# Dev: todas las migraciones
Update-Db -dbName 'ChetangoDB_Dev' -envName 'Development'

# QA/Prod: hasta AlignSnapshot_SeedOID (sin semillas dev)
Update-Db -dbName 'ChetangoDB_QA' -migration '20250917211543_AlignSnapshot_SeedOID' -envName 'QA'
Update-Db -dbName 'ChetangoDB_Prod' -migration '20250917211543_AlignSnapshot_SeedOID' -envName 'Production'

Write-Host 'Done.'

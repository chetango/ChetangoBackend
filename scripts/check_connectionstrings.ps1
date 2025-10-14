$ErrorActionPreference = 'Stop'

function Get-Conn([string]$path){
  $json = Get-Content -Raw -Path $path | ConvertFrom-Json
  return $json.ConnectionStrings.ChetangoConnection
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$api = Join-Path $root '..\Chetango.Api'

$devPath = Join-Path $api 'appsettings.Development.json'
$qaPath  = Join-Path $api 'appsettings.QA.json'
$prodPath= Join-Path $api 'appsettings.json'

$devConn = Get-Conn $devPath
$qaConn  = Get-Conn $qaPath
$prodConn= Get-Conn $prodPath

"DEV:  $devConn"
"QA:   $qaConn"
"PROD: $prodConn"

# Test each connection by querying DB_NAME()
Add-Type -AssemblyName System.Data
function Test-Conn([string]$conn){
  $c = New-Object System.Data.SqlClient.SqlConnection $conn
  try { $c.Open(); $cmd=$c.CreateCommand(); $cmd.CommandText='SELECT DB_NAME()'; $db=$cmd.ExecuteScalar(); return "OK -> $db" }
  catch { return "ERROR -> $($_.Exception.Message)" }
  finally { $c.Close() }
}

"DEV Test:  $(Test-Conn $devConn)"
"QA Test:   $(Test-Conn $qaConn)"
"PROD Test: $(Test-Conn $prodConn)"

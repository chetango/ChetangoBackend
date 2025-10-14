param(
  [string]$ServerInstance = 'U-89P5FG3\\SQLEXPRESS'
)

$ErrorActionPreference = 'Stop'
$names = @('ChetangoDB_Dev','ChetangoDB_QA','ChetangoDB_Prod')

$outDir = Join-Path $PSScriptRoot '..\LogdeCambios'
if (-not (Test-Path $outDir)) { New-Item -ItemType Directory -Path $outDir | Out-Null }
$outFile = Join-Path $outDir 'db_check.txt'

$result = @{}

try {
  if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
    $inList = ($names | ForEach-Object { "'$_'" }) -join ','
    $rows = sqlcmd -S $ServerInstance -E -h -1 -W -Q "SET NOCOUNT ON; SELECT name FROM sys.databases WHERE name IN ($inList);"
    $present = @{}
    foreach ($n in $rows) { if ($n) { $present[$n.Trim()] = $true } }
    foreach ($n in $names) { $result[$n] = if ($present.ContainsKey($n)) { 'Exists' } else { 'Missing' } }
  } else {
    try { Add-Type -AssemblyName 'Microsoft.SqlServer.Smo' -ErrorAction SilentlyContinue | Out-Null } catch {}
    $srv = New-Object Microsoft.SqlServer.Management.Smo.Server $ServerInstance
    foreach ($n in $names) { $result[$n] = if ($srv.Databases[$n]) { 'Exists' } else { 'Missing' } }
  }
}
catch {
  $result['Error'] = $_.Exception.Message
}

$out = @()
$out += "Server: $ServerInstance"
foreach ($n in $names) { $out += "$($n): $($result[$n])" }
if ($result.ContainsKey('Error')) { $out += "Error: $($result['Error'])" }
$out | Out-File -Encoding UTF8 -FilePath $outFile -Force
Write-Host "Wrote $outFile"

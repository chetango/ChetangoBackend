param(
  [string]$ServerInstance = 'U-89P5FG3\\SQLEXPRESS'
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data

function Exec-TSql($sql) {
  $connString = "Server=$ServerInstance;Database=master;Integrated Security=True;TrustServerCertificate=True;"
  $conn = New-Object System.Data.SqlClient.SqlConnection $connString
  try {
    $conn.Open()
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $cmd.ExecuteNonQuery() | Out-Null
  } finally { $conn.Close() }
}

$tsql = @"
IF DB_ID('ChetangoDB_Dev') IS NULL CREATE DATABASE [ChetangoDB_Dev];
IF DB_ID('ChetangoDB_QA') IS NULL CREATE DATABASE [ChetangoDB_QA];
IF DB_ID('ChetangoDB_Prod') IS NULL CREATE DATABASE [ChetangoDB_Prod];
"@

Exec-TSql $tsql
Write-Host 'Ensured ChetangoDB_Dev, ChetangoDB_QA, ChetangoDB_Prod via SqlClient.'

param(
  [string]$ServerInstance = 'U-89P5FG3\\SQLEXPRESS'
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data

function Query-Db($dbName) {
  $connString = "Server=$ServerInstance;Database=$dbName;Integrated Security=True;TrustServerCertificate=True;"
  $conn = New-Object System.Data.SqlClient.SqlConnection $connString
  $result = [ordered]@{ Database=$dbName; Tables=0; Migrations=0; DevSeed='N/A' }
  try {
    $conn.Open()
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT COUNT(*) FROM sys.tables"; $result.Tables = [int]$cmd.ExecuteScalar()
    try { $cmd.CommandText = "SELECT COUNT(*) FROM __EFMigrationsHistory"; $result.Migrations = [int]$cmd.ExecuteScalar() } catch { $result.Migrations = 0 }

    if ($dbName -eq 'ChetangoDB_Dev') {
      # Check dev seed known GUIDs
      $usuarioId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
      $alumnoId =  "bbbbbbbb-cccc-dddd-eeee-ffffffffffff"
      $profesorId ="cccccccc-dddd-eeee-ffff-000000000000"
      $claseId =   "dddddddd-eeee-ffff-0000-111111111111"
      $paqueteId = "f0f0f0f0-0000-0000-0000-000000000000"
      $asistenciaId="eeeeeeee-ffff-0000-1111-222222222222"
      try {
        $cmd.CommandText = "SELECT COUNT(*) FROM sys.objects WHERE name='Usuarios' AND type='U'"; $hasUsuarios = [int]$cmd.ExecuteScalar()
        if ($hasUsuarios -gt 0) {
          $cmd.CommandText = "SELECT COUNT(*) FROM Usuarios WHERE IdUsuario = @id"
          $cmd.Parameters.Clear(); $p=$cmd.Parameters.Add('@id',[System.Data.SqlDbType]::UniqueIdentifier); $p.Value=[Guid]$usuarioId
          $u = [int]$cmd.ExecuteScalar()
          $cmd.Parameters.Clear()
          $cmd.CommandText = "SELECT COUNT(*) FROM Alumnos WHERE IdAlumno = @id"; $p=$cmd.Parameters.Add('@id',[System.Data.SqlDbType]::UniqueIdentifier); $p.Value=[Guid]$alumnoId; $a=[int]$cmd.ExecuteScalar(); $cmd.Parameters.Clear()
          $cmd.CommandText = "SELECT COUNT(*) FROM Profesores WHERE IdProfesor = @id"; $p=$cmd.Parameters.Add('@id',[System.Data.SqlDbType]::UniqueIdentifier); $p.Value=[Guid]$profesorId; $pr=[int]$cmd.ExecuteScalar(); $cmd.Parameters.Clear()
          $cmd.CommandText = "SELECT COUNT(*) FROM Clases WHERE IdClase = @id"; $p=$cmd.Parameters.Add('@id',[System.Data.SqlDbType]::UniqueIdentifier); $p.Value=[Guid]$claseId; $cl=[int]$cmd.ExecuteScalar(); $cmd.Parameters.Clear()
          $cmd.CommandText = "SELECT COUNT(*) FROM Paquetes WHERE IdPaquete = @id"; $p=$cmd.Parameters.Add('@id',[System.Data.SqlDbType]::UniqueIdentifier); $p.Value=[Guid]$paqueteId; $pq=[int]$cmd.ExecuteScalar(); $cmd.Parameters.Clear()
          $cmd.CommandText = "SELECT COUNT(*) FROM Asistencias WHERE IdAsistencia = @id"; $p=$cmd.Parameters.Add('@id',[System.Data.SqlDbType]::UniqueIdentifier); $p.Value=[Guid]$asistenciaId; $as=[int]$cmd.ExecuteScalar(); $cmd.Parameters.Clear()
          $result.DevSeed = if (($u+$a+$pr+$cl+$pq+$as) -gt 0) { 'Present' } else { 'Absent' }
        } else { $result.DevSeed = 'Absent' }
      } catch { $result.DevSeed = 'Error' }
    }
  }
  catch { $result.Error = $_.Exception.Message }
  finally { $conn.Close() }
  return [pscustomobject]$result
}

$items = @()
foreach ($db in 'ChetangoDB','ChetangoDB_Dev','ChetangoDB_QA','ChetangoDB_Prod') { $items += Query-Db $db }

$outDir = Join-Path $PSScriptRoot '..\\LogdeCambios'
if (-not (Test-Path $outDir)) { New-Item -ItemType Directory -Path $outDir | Out-Null }
$outFile = Join-Path $outDir 'db_details.txt'
$items | Format-Table -AutoSize | Out-String | Out-File -Encoding UTF8 -FilePath $outFile -Force
Write-Host "Wrote $outFile"

# ============================================
# SCRIPT PARA SINCRONIZAR USUARIOS DE ENTRA ID CON BASE DE DATOS LOCAL
# Fecha: 30 Enero 2026
# ============================================

Write-Host "🔄 SINCRONIZACIÓN DE USUARIOS ENTRA ID → BASE DE DATOS" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Verificar que exista el archivo de credenciales
$credencialesFile = ".\credenciales-usuarios-qa.csv"

if (-not (Test-Path $credencialesFile)) {
    Write-Host "❌ No se encontró el archivo de credenciales: $credencialesFile" -ForegroundColor Red
    Write-Host "   Primero ejecute: .\crear-usuarios-entra-id.ps1" -ForegroundColor Yellow
    exit
}

# Importar credenciales
Write-Host "📂 Leyendo credenciales de: $credencialesFile" -ForegroundColor Cyan
$usuarios = Import-Csv -Path $credencialesFile

Write-Host "✅ $($usuarios.Count) usuarios encontrados" -ForegroundColor Green
Write-Host ""

# Configuración de base de datos
$serverName = "localhost"
$databaseName = "ChetangoDB_Dev"

Write-Host "🔍 Conectando a SQL Server..." -ForegroundColor Cyan
Write-Host "   Server: $serverName" -ForegroundColor White
Write-Host "   Database: $databaseName" -ForegroundColor White
Write-Host ""

# Obtener IDs de estados y tipos
Write-Host "📋 Obteniendo catálogos de la base de datos..." -ForegroundColor Cyan

$queryEstados = @"
SELECT 
    (SELECT Id FROM TiposDocumento WHERE Nombre = 'Cédula de Ciudadanía') AS IdTipoDocumento,
    (SELECT Id FROM EstadosUsuario WHERE Nombre = 'Activo') AS IdEstadoUsuario,
    (SELECT Id FROM EstadosAlumno WHERE Nombre = 'Activo') AS IdEstadoAlumno,
    (SELECT Id FROM TiposProfesor WHERE Nombre = 'Principal') AS IdTipoProfesor;
"@

try {
    $catalogos = Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Query $queryEstados -ErrorAction Stop
    
    $idTipoDocumento = $catalogos.IdTipoDocumento
    $idEstadoUsuario = $catalogos.IdEstadoUsuario
    $idEstadoAlumno = $catalogos.IdEstadoAlumno
    $idTipoProfesor = $catalogos.IdTipoProfesor
    
    Write-Host "   ✅ IdTipoDocumento (CC): $idTipoDocumento" -ForegroundColor Green
    Write-Host "   ✅ IdEstadoUsuario (Activo): $idEstadoUsuario" -ForegroundColor Green
    Write-Host "   ✅ IdEstadoAlumno (Activo): $idEstadoAlumno" -ForegroundColor Green
    Write-Host "   ✅ IdTipoProfesor (Principal): $idTipoProfesor" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host "❌ Error al obtener catálogos: $($_.Exception.Message)" -ForegroundColor Red
    exit
}

# Función para generar número de documento ficticio
function Get-NumeroDocumento {
    param($index)
    return (1000000000 + $index).ToString()
}

# Función para generar teléfono ficticio
function Get-Telefono {
    param($index)
    return "300" + (1000000 + $index).ToString()
}

# Procesar cada usuario
Write-Host "👥 Procesando usuarios..." -ForegroundColor Cyan
Write-Host ""

$contadorCreados = 0
$contadorExistentes = 0
$contadorErrores = 0

for ($i = 0; $i -lt $usuarios.Count; $i++) {
    $usuario = $usuarios[$i]
    $numeroDocumento = Get-NumeroDocumento -index ($i + 1)
    $telefono = Get-Telefono -index ($i + 1)
    
    try {
        Write-Host "[$($i + 1)/$($usuarios.Count)] Procesando: $($usuario.Nombre) ($($usuario.Rol))" -ForegroundColor Yellow
        
        # Verificar si el usuario ya existe
        $checkQuery = "SELECT IdUsuario FROM Usuarios WHERE IdUsuario = '$($usuario.UserId)'"
        $existingUser = Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Query $checkQuery
        
        if ($existingUser) {
            Write-Host "   ℹ️  Usuario ya existe en BD" -ForegroundColor Yellow
            $contadorExistentes++
        }
        else {
            # Insertar en tabla Usuarios
            $insertUsuario = @"
INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
VALUES (
    '$($usuario.UserId)',
    '$($usuario.Nombre)',
    '$idTipoDocumento',
    '$numeroDocumento',
    '$($usuario.Email)',
    '$telefono',
    $idEstadoUsuario,
    GETDATE()
);
"@
            
            Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Query $insertUsuario
            Write-Host "   ✅ Usuario creado en tabla Usuarios" -ForegroundColor Green
            $contadorCreados++
        }
        
        # Si es profesor, crear registro en tabla Profesores
        if ($usuario.Rol -eq "profesor") {
            $checkProfesor = "SELECT IdProfesor FROM Profesores WHERE IdUsuario = '$($usuario.UserId)'"
            $existingProfesor = Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Query $checkProfesor
            
            if (-not $existingProfesor) {
                $newProfesorId = [guid]::NewGuid().ToString().ToUpper()
                $insertProfesor = @"
INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
VALUES (
    '$newProfesorId',
    '$($usuario.UserId)',
    '$idTipoProfesor',
    1,
    1,
    1
);
"@
                
                Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Query $insertProfesor
                Write-Host "   ✅ Profesor creado (Tipo: Principal)" -ForegroundColor Green
            }
            else {
                Write-Host "   ℹ️  Registro de profesor ya existe" -ForegroundColor Yellow
            }
        }
        
        # Si es alumno, crear registro en tabla Alumnos
        if ($usuario.Rol -eq "alumno") {
            $checkAlumno = "SELECT IdAlumno FROM Alumnos WHERE IdUsuario = '$($usuario.UserId)'"
            $existingAlumno = Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Query $checkAlumno
            
            if (-not $existingAlumno) {
                $newAlumnoId = [guid]::NewGuid().ToString().ToUpper()
                $insertAlumno = @"
INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado, NotificacionesEmail, RecordatoriosClase, AlertasPaquete)
VALUES (
    '$newAlumnoId',
    '$($usuario.UserId)',
    GETDATE(),
    $idEstadoAlumno,
    1,
    1,
    1
);
"@
                
                Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Query $insertAlumno
                Write-Host "   ✅ Alumno creado" -ForegroundColor Green
            }
            else {
                Write-Host "   ℹ️  Registro de alumno ya existe" -ForegroundColor Yellow
            }
        }
        
        Write-Host ""
    }
    catch {
        Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        $contadorErrores++
    }
}

Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host "✅ SINCRONIZACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host ""
Write-Host "📊 RESUMEN:" -ForegroundColor Cyan
Write-Host "   Total procesados: $($usuarios.Count)" -ForegroundColor White
Write-Host "   ✅ Creados: $contadorCreados" -ForegroundColor Green
Write-Host "   ℹ️  Ya existían: $contadorExistentes" -ForegroundColor Yellow
Write-Host "   ❌ Errores: $contadorErrores" -ForegroundColor Red
Write-Host ""

# Verificar conteos finales
Write-Host "🔍 Verificando estado de la base de datos..." -ForegroundColor Cyan

$verifyQuery = @"
SELECT 'Usuarios' AS Tabla, COUNT(*) AS Cantidad FROM Usuarios
UNION ALL
SELECT 'Profesores', COUNT(*) FROM Profesores
UNION ALL
SELECT 'Alumnos', COUNT(*) FROM Alumnos;
"@

$conteos = Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Query $verifyQuery

Write-Host ""
foreach ($conteo in $conteos) {
    Write-Host "   $($conteo.Tabla): $($conteo.Cantidad)" -ForegroundColor White
}

Write-Host ""
Write-Host "🎯 SIGUIENTE PASO: Probar login con los usuarios en la aplicación" -ForegroundColor Yellow
Write-Host ""
Write-Host "✅ Script completado" -ForegroundColor Green

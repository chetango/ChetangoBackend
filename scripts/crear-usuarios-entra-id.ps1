# ============================================
# SCRIPT PARA CREAR USUARIOS EN ENTRA ID - CHETANGO
# Fecha: 30 Enero 2026
# ============================================

# Verificar e instalar Microsoft.Graph si es necesario
if (-not (Get-Module -ListAvailable -Name Microsoft.Graph)) {
    Write-Host "Instalando modulo Microsoft.Graph..." -ForegroundColor Yellow
    Install-Module Microsoft.Graph -Scope CurrentUser -Force
}

# Importar modulos necesarios
Import-Module Microsoft.Graph.Users
Import-Module Microsoft.Graph.Applications

# Conectar a Microsoft Graph
Write-Host "Conectando a Microsoft Graph..." -ForegroundColor Cyan
Connect-MgGraph -Scopes "User.ReadWrite.All", "Application.ReadWrite.All", "AppRoleAssignment.ReadWrite.All"

# Configuracion
$tenantDomain = "chetangoprueba.onmicrosoft.com"
$appDisplayName = "Chetango Backend"
$passwordDefault = "Chetango2026!"  # Contraseña temporal que deben cambiar

# Obtener la aplicacion Chetango Backend
Write-Host "Buscando aplicacion '$appDisplayName'..." -ForegroundColor Cyan
$app = Get-MgApplication -Filter "displayName eq '$appDisplayName'"

if (-not $app) {
    Write-Host "ERROR: No se encontro la aplicacion '$appDisplayName'" -ForegroundColor Red
    exit
}

$appId = $app.AppId
Write-Host "OK: Aplicacion encontrada: $appId" -ForegroundColor Green

# Obtener el Service Principal de la aplicacion
$servicePrincipal = Get-MgServicePrincipal -Filter "appId eq '$appId'"

if (-not $servicePrincipal) {
    Write-Host "ERROR: No se encontro el Service Principal" -ForegroundColor Red
    exit
}

$servicePrincipalId = $servicePrincipal.Id
Write-Host "OK: Service Principal ID: $servicePrincipalId" -ForegroundColor Green

# Obtener los roles de la aplicacion
$appRoles = $servicePrincipal.AppRoles
$roleAdmin = ($appRoles | Where-Object { $_.Value -eq "admin" }).Id
$roleProfesor = ($appRoles | Where-Object { $_.Value -eq "profesor" }).Id
$roleAlumno = ($appRoles | Where-Object { $_.Value -eq "alumno" }).Id

Write-Host "Roles encontrados:" -ForegroundColor Cyan
Write-Host "   Admin: $roleAdmin" -ForegroundColor White
Write-Host "   Profesor: $roleProfesor" -ForegroundColor White
Write-Host "   Alumno: $roleAlumno" -ForegroundColor White
Write-Host ""

# Definir usuarios a crear
$usuarios = @(
    # PROFESORES
    @{ Nombre = "Santiago"; Apellido = "Salazar"; Rol = "profesor"; RoleId = $roleProfesor },
    @{ Nombre = "Maria Alejandra"; Apellido = "Rodriguez"; Rol = "profesor"; RoleId = $roleProfesor },
    @{ Nombre = "Ana Zoraida"; Apellido = "Gomez"; Rol = "profesor"; RoleId = $roleProfesor },
    @{ Nombre = "Laura"; Apellido = "Machado"; Rol = "profesor"; RoleId = $roleProfesor },
    @{ Nombre = "Susana"; Apellido = "Alzate"; Rol = "profesor"; RoleId = $roleProfesor },
    @{ Nombre = "Jhonathan"; Apellido = "Pachon"; Rol = "profesor"; RoleId = $roleProfesor },
    @{ Nombre = "Suly"; Apellido = "Pachon"; Rol = "profesor"; RoleId = $roleProfesor },
    
    # ALUMNOS
    @{ Nombre = "Juan Pablo"; Apellido = "Gomez"; Rol = "alumno"; RoleId = $roleAlumno },
    @{ Nombre = "Diana"; Apellido = "Diaz"; Rol = "alumno"; RoleId = $roleAlumno },
    @{ Nombre = "Humberto"; Apellido = "Giraldo"; Rol = "alumno"; RoleId = $roleAlumno },
    @{ Nombre = "Manuela"; Apellido = "Gonzales"; Rol = "alumno"; RoleId = $roleAlumno },
    @{ Nombre = "Andrea"; Apellido = "Solorzano"; Rol = "alumno"; RoleId = $roleAlumno },
    @{ Nombre = "Pablo"; Apellido = "Murillo"; Rol = "alumno"; RoleId = $roleAlumno },
    @{ Nombre = "Catalina"; Apellido = "Sanchez"; Rol = "alumno"; RoleId = $roleAlumno },
    @{ Nombre = "Camilo"; Apellido = "Tobon"; Rol = "alumno"; RoleId = $roleAlumno },
    
    # ADMINISTRADOR
    @{ Nombre = "Yeny"; Apellido = "Padilla"; Rol = "admin"; RoleId = $roleAdmin }
)

# Array para guardar credenciales
$credenciales = @()

# Funcion para generar nombre de usuario
function Get-UserPrincipalName {
    param($nombre, $apellido)
    
    # Remover espacios y caracteres especiales
    $nombreClean = $nombre -replace '\s+', ''
    $apellidoClean = $apellido -replace '\s+', ''
    
    # Remover acentos
    $nombreClean = $nombreClean -replace 'á', 'a' -replace 'é', 'e' -replace 'í', 'i' -replace 'ó', 'o' -replace 'ú', 'u'
    $apellidoClean = $apellidoClean -replace 'á', 'a' -replace 'é', 'e' -replace 'í', 'i' -replace 'ó', 'o' -replace 'ú', 'u'
    
    $upn = "$nombreClean$apellidoClean@$tenantDomain"
    return $upn.ToLower()
}

# Crear usuarios
Write-Host "Creando usuarios..." -ForegroundColor Cyan
Write-Host ""

$contador = 0
foreach ($usuario in $usuarios) {
    try {
        $contador++
        $upn = Get-UserPrincipalName -nombre $usuario.Nombre -apellido $usuario.Apellido
        $displayName = "$($usuario.Nombre) $($usuario.Apellido)"
        $mailNickname = ($upn -split '@')[0]
        
        Write-Host "[$contador/$($usuarios.Count)] Creando: $displayName ($upn)" -ForegroundColor Yellow
        
        # Verificar si el usuario ya existe
        $existingUser = Get-MgUser -Filter "userPrincipalName eq '$upn'" -ErrorAction SilentlyContinue
        
        if ($existingUser) {
            Write-Host "   ADVERTENCIA: Usuario ya existe: $upn" -ForegroundColor Yellow
            $userId = $existingUser.Id
        }
        else {
            # Crear el usuario
            $passwordProfile = @{
                Password = $passwordDefault
                ForceChangePasswordNextSignIn = $true
            }
            
            $newUser = New-MgUser -DisplayName $displayName `
                                  -UserPrincipalName $upn `
                                  -MailNickname $mailNickname `
                                  -AccountEnabled `
                                  -PasswordProfile $passwordProfile `
                                  -GivenName $usuario.Nombre `
                                  -Surname $usuario.Apellido
            
            $userId = $newUser.Id
            Write-Host "   OK: Usuario creado: $upn" -ForegroundColor Green
        }
        
        # Asignar rol de aplicacion
        Write-Host "   Asignando rol: $($usuario.Rol)" -ForegroundColor Cyan
        
        # Verificar si ya tiene el rol asignado
        $existingAssignment = Get-MgUserAppRoleAssignment -UserId $userId | Where-Object { 
            $_.AppRoleId -eq $usuario.RoleId -and $_.ResourceId -eq $servicePrincipalId 
        }
        
        if ($existingAssignment) {
            Write-Host "   INFO: Rol ya asignado" -ForegroundColor Yellow
        }
        else {
            $appRoleAssignment = @{
                PrincipalId = $userId
                ResourceId = $servicePrincipalId
                AppRoleId = $usuario.RoleId
            }
            
            New-MgUserAppRoleAssignment -UserId $userId -BodyParameter $appRoleAssignment | Out-Null
            Write-Host "   OK: Rol asignado correctamente" -ForegroundColor Green
        }
        
        # Guardar credenciales
        $credenciales += [PSCustomObject]@{
            Nombre = $displayName
            Rol = $usuario.Rol
            Email = $upn
            Password = $passwordDefault
            UserId = $userId
        }
        
        Write-Host ""
        Start-Sleep -Milliseconds 500
    }
    catch {
        Write-Host "   ERROR: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
    }
}

Write-Host "=============================================================" -ForegroundColor Green
Write-Host "USUARIOS CREADOS EXITOSAMENTE" -ForegroundColor Green
Write-Host "=============================================================" -ForegroundColor Green
Write-Host ""

# Guardar credenciales en archivo CSV
$credencialesFile = ".\credenciales-usuarios-qa.csv"
$credenciales | Export-Csv -Path $credencialesFile -NoTypeInformation -Encoding UTF8
Write-Host "Credenciales guardadas en: $credencialesFile" -ForegroundColor Cyan

# Guardar credenciales en formato legible
$credencialesReadableFile = ".\credenciales-usuarios-qa.txt"
$content = @"
=============================================================
CREDENCIALES DE USUARIOS PARA PRUEBAS QA - CHETANGO
Fecha: $(Get-Date -Format "dd/MM/yyyy HH:mm:ss")
=============================================================

IMPORTANTE: Estos usuarios deben cambiar su contraseña en el primer inicio de sesión.
Contraseña temporal para todos: $passwordDefault

=============================================================
PROFESORES (7)
=============================================================

"@

$profesores = $credenciales | Where-Object { $_.Rol -eq "profesor" }
foreach ($prof in $profesores) {
    $content += @"
$($prof.Nombre)
   Email: $($prof.Email)
   Password: $($prof.Password)
   User ID: $($prof.UserId)

"@
}

$content += @"

=============================================================
ALUMNOS (8)
=============================================================

"@

$alumnos = $credenciales | Where-Object { $_.Rol -eq "alumno" }
foreach ($alumno in $alumnos) {
    $content += @"
$($alumno.Nombre)
   Email: $($alumno.Email)
   Password: $($alumno.Password)
   User ID: $($alumno.UserId)

"@
}

$content += @"

=============================================================
ADMINISTRADORES (1)
=============================================================

"@

$admins = $credenciales | Where-Object { $_.Rol -eq "admin" }
foreach ($admin in $admins) {
    $content += @"
$($admin.Nombre)
   Email: $($admin.Email)
   Password: $($admin.Password)
   User ID: $($admin.UserId)

"@
}

$content += @"

=============================================================
RESUMEN
=============================================================

Total de usuarios creados: $($credenciales.Count)
- Profesores: $($profesores.Count)
- Alumnos: $($alumnos.Count)
- Administradores: $($admins.Count)

=============================================================
SIGUIENTE PASO
=============================================================

1. Ejecutar el script de sincronización de usuarios con la BD local:
   .\sincronizar-usuarios-bd.ps1

2. Esto creará los registros correspondientes en las tablas:
   - Usuarios
   - Profesores (para los 7 profes)
   - Alumnos (para los 8 alumnos)

3. Iniciar sesión con cada usuario para cambiar contraseña temporal

=============================================================
"@

$content | Out-File -FilePath $credencialesReadableFile -Encoding UTF8
Write-Host "Credenciales legibles guardadas en: $credencialesReadableFile" -ForegroundColor Cyan
Write-Host ""

# Mostrar resumen
Write-Host "RESUMEN:" -ForegroundColor Cyan
Write-Host "   Total usuarios: $($credenciales.Count)" -ForegroundColor White
Write-Host "   Profesores: $($profesores.Count)" -ForegroundColor White
Write-Host "   Alumnos: $($alumnos.Count)" -ForegroundColor White
Write-Host "   Administradores: $($admins.Count)" -ForegroundColor White
Write-Host ""
Write-Host "SIGUIENTE PASO: Ejecutar .\sincronizar-usuarios-bd.ps1" -ForegroundColor Yellow
Write-Host ""

# Desconectar
Disconnect-MgGraph | Out-Null
Write-Host "Script completado" -ForegroundColor Green

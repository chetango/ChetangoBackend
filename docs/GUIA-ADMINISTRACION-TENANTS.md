# GUÍA DE ADMINISTRACIÓN DE TENANTS (SUPER ADMIN)

## Descripción General

Hemos implementado una arquitectura profesional de multi-tenancy con las siguientes características:

✅ **Subdomain-based tenant resolution** - El tenant se determina desde el subdomain (academia1.aphelion.com)
✅ **Row-Level Security (RLS)** - Protección a nivel de base de datos contra bugs y SQL injection
✅ **Scoped TenantProvider** - Cada request HTTP tiene su propio TenantId
✅ **SESSION_CONTEXT** - El TenantId se establece en cada conexión SQL
✅ **TenantUsers** - Tabla many-to-many para usuarios que trabajan en múltiples academias

## Flujo de Onboarding (Como Super Admin)

### FLUJO RECOMENDADO: TODO EN UNO ⭐

#### 1. Registrar Usuario Administrador en Azure AD (PRIMERO)

**IMPORTANTE:** El usuario DEBE existir en Azure Entra ID antes de crearlo en la app.

**Pasos:**
1. Ir a Azure Portal → Entra ID → Usuarios
2. Crear nuevo usuario: `admin.salsalatina@salsalatina.aphelion.com`
3. Asignar rol de aplicación: **Admin**
4. Guardar contraseña temporal

#### 2. Crear Academia con Administrador (TODO EN UNO)

**Endpoint:** `POST /api/super-admin/academias`

**Cuerpo de la solicitud:**
```json
{
  "nombreTenant": "Salsa Latina",
  "subdomain": "salsalatina",
  "dominioCompleto": "salsalatina.aphelion.com",
  "plan": "Basic",
  "maxSedes": 2,
  "maxAlumnos": 100,
  "maxProfesores": 20,
  "maxStorageMB": 2000,
  "nombreUsuario": "Administrador Salsa Latina",
  "correoAdmin": "admin.salsalatina@salsalatina.aphelion.com",
  "idTipoDocumento": "<GUID del tipo CC>",
  "numeroDocumento": "123456789",
  "telefono": "3001234567"
}
```

**Respuesta:**
```json
{
  "success": true,
  "tenantId": "12345678-1234-1234-1234-123456789012",
  "idUsuario": "87654321-4321-4321-4321-210987654321",
  "message": "Academia 'Salsa Latina' creada exitosamente con administrador 'Administrador Salsa Latina'. URL: https://salsalatina.aphellion.com"
}
```

**Este comando hace TODO automáticamente:**
- ✅ Crea el Tenant con TenantId
- ✅ Crea el Usuario en DB (si no existe)
- ✅ Asigna el usuario al tenant (TenantUsers)
- ✅ Listo para que el admin se loguee

#### 3. Entregar Acceso

**Le envías a la administradora:**
- 📧 **URL**: `https://salsalatina.aphellion.com`
- 👤 **Usuario**: `admin.salsalatina@salsalatina.aphelion.com`
- 🔑 **Contraseña**: (la que definiste en Azure)

---

### FLUJO ALTERNATIVO: Paso por Paso

Si prefieres crear tenant y usuario por separado:

#### 1. Crear Solo el Tenant

**Endpoint:** `POST /api/super-admin/tenants`

```json
{
  "nombre": "Salsa Latina",
  "subdomain": "salsalatina",
  "dominio": "salsalatina.aphelion.com",
  "plan": "Basic",
  "maxSedes": 2,
  "maxAlumnos": 100,
  "maxProfesores": 20,
  "maxStorageMB": 2000,
  "emailContacto": "contacto@salsalatina.aphelion.com"
}
```

#### 2. Registrar Usuario en Azure AD

(Mismo proceso que el flujo recomendado)

#### 3. Asignar Usuario al Tenant

**Endpoint:** `POST /api/super-admin/tenants/assign-user`

```json
{
  "tenantId": "12345678-1234-1234-1234-123456789012",
  "idUsuario": "<GUID del usuario en tabla Usuarios>"
}
```

## Arquitectura de Seguridad

### Flujo de Autenticación y Autorización

1. **Usuario navega a:** `https://salsalatina.aphelion.com`
2. **Frontend redirige a Azure AD** para login
3. **Azure AD valida credenciales** y genera JWT con roles
4. **Frontend envía request** con JWT en header `Authorization: Bearer <token>`
5. **TenantResolutionMiddleware:**
   - Extrae subdomain del host (`salsalatina`)
   - Busca TenantId en base de datos
   - Guarda TenantId en `ITenantProvider` (scoped)
6. **DbConnectionInterceptor:**
   - Al abrir cada conexión SQL
   - Ejecuta `sp_set_session_context @key = 'TenantId', @value = <TenantId>`
7. **Row-Level Security:**
   - Filtra automáticamente WHERE TenantId = SESSION_CONTEXT('TenantId')
   - Protege contra bugs en código y SQL injection

### Capas de Seguridad

1. **Azure AD Authentication** - Solo usuarios registrados pueden obtener JWT
2. **JWT Validation** - El token se valida en cada request
3. **Role-based Authorization** - Políticas Admin/Profesor/Alumno
4. **Tenant Resolution** - Subdomain determina tenant automáticamente
5. **EF Core Query Filters** - Filtran queries en código C#
6. **Row-Level Security** - Filtra queries a nivel de SQL Server

### Protección Contra Ataques

**Escenario 1: Hacker roba JWT de Chetango**
- Intenta usarlo en `salsalatina.aphelion.com`
- Middleware resuelve TenantId = Salsa Latina
- RLS filtra datos de Salsa Latina
- Hacker NO ve datos de Chetango ✅

**Escenario 2: Bug en código omite Query Filters**
- Código hace `.IgnoreQueryFilters()` por error
- EF Core genera query SIN filtro TenantId
- Row-Level Security INTERCEPTA query en SQL Server
- SQL Server agrega WHERE TenantId = ... automáticamente
- Solo retorna datos del tenant correcto ✅

**Escenario 3: SQL Injection**
- Atacante inyecta: `'; DROP TABLE Usuarios; --`
- SQL Server ejecuta en contexto de SESSION_CONTEXT
- RLS previene acceso a datos de otros tenants
- Daño limitado a un solo tenant ✅

## Usuarios Multi-Tenant

Un usuario puede trabajar en múltiples academias:

**Ejemplo: Carlos es profesor en Chetango Y Salsa Latina**

1. Registro en Azure AD: `carlos.profesor@chetango.aphelion.com` (rol: Profesor)
2. Crear usuario en DB: IdUsuario = `AAAA-BBBB-CCCC-DDDD`
3. Asignar a Chetango:
   ```json
   {
     "tenantId": "A1B2C3D4-E5F6-7890-ABCD-EF1234567890",
     "idUsuario": "AAAA-BBBB-CCCC-DDDD"
   }
   ```
4. Asignar a Salsa Latina:
   ```json
   {
     "tenantId": "12345678-1234-1234-1234-123456789012",
     "idUsuario": "AAAA-BBBB-CCCC-DDDD"
   }
   ```

**Funcionamiento:**
- Carlos navega a `chetango.aphelion.com` → Ve solo datos de Chetango
- Carlos navega a `salsalatina.aphelion.com` → Ve solo datos de Salsa Latina
- El subdomain determina qué tenant es el activo

## Tabla TenantUsers

```sql
SELECT 
    t.Nombre AS Academia,
    u.NombreUsuario,
    u.Correo,
    tu.FechaAsignacion,
    tu.Activo
FROM TenantUsers tu
JOIN Tenants t ON tu.TenantId = t.Id
JOIN Usuarios u ON tu.IdUsuario = u.IdUsuario
WHERE tu.Activo = 1
ORDER BY t.Nombre, u.NombreUsuario;
```

## Verificación de Row-Level Security

### Ver políticas activas:
```sql
SELECT 
    t.name AS TableName,
    sp.name AS SecurityPolicyName,
    spp.predicate_type_desc AS PredicateType
FROM sys.security_policies sp
INNER JOIN sys.security_predicates spp ON sp.object_id = spp.object_id
INNER JOIN sys.tables t ON spp.target_object_id = t.object_id
WHERE sp.name LIKE 'TenantSecurityPolicy%';
```

### Probar RLS manualmente:
```sql
-- Establecer TenantId
EXEC sp_set_session_context @key = N'TenantId', @value = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890';

-- Esta query solo retorna clases de Chetango
SELECT * FROM Clases;

-- Limpiar contexto
EXEC sp_set_session_context @key = N'TenantId', @value = NULL;
```

## Troubleshooting

### Problema: Usuario ve datos de otro tenant

**Diagnóstico:**
```csharp
// Agregar logs en TenantResolutionMiddleware
_logger.LogInformation("Host: {Host}, Subdomain: {Subdomain}, TenantId: {TenantId}", 
    host, subdomain, tenantId);
```

**Verificar:**
1. ¿El subdomain se extrae correctamente?
2. ¿El tenant existe en la tabla Tenants con ese subdomain?
3. ¿El TenantProvider se inyecta como Scoped?
4. ¿El DbConnectionInterceptor se registró correctamente?

### Problema: Error al crear clase/pago (TenantId required)

**Causa:** El código de creación debe establecer el TenantId.

**Solución:**
```csharp
// En CreateClaseCommandHandler:
var clase = new Clase
{
    // ... otros campos
    TenantId = _tenantProvider.GetCurrentTenantId() // Agregar esta línea
};
```

### Problema: No se aplica RLS

**Verificar:**
```sql
-- Ver si las políticas están activas
SELECT name, is_enabled FROM sys.security_policies;

-- Activar política si está deshabilitada
ALTER SECURITY POLICY dbo.TenantSecurityPolicy_Clases WITH (STATE = ON);
```

## Desarrollo Local

En `localhost` no hay subdomain, por lo que TenantId será NULL y verás TODOS los datos (modo Super Admin).

**Para probar multi-tenancy en desarrollo:**

### Opción 1: Modificar archivo hosts
```
# C:\Windows\System32\drivers\etc\hosts
127.0.0.1 chetango.localhost
127.0.0.1 salsalatina.localhost
```

Navegar a: `http://chetango.localhost:5000`

### Opción 2: Header personalizado
Modificar `TenantResolutionMiddleware` para leer header:
```csharp
if (host.StartsWith("localhost"))
{
    return context.Request.Headers["X-Tenant-Subdomain"].FirstOrDefault();
}
```

Enviar header: `X-Tenant-Subdomain: chetango`

## Comandos Útiles

### Ver todos los tenants:
```sql
SELECT Id, Nombre, Subdomain, Dominio, Estado, MaxAlumnos 
FROM Tenants 
ORDER BY Nombre;
```

### Ver usuarios de un tenant:
```sql
SELECT 
    u.NombreUsuario,
    u.Correo,
    tu.FechaAsignacion,
    tu.Activo
FROM TenantUsers tu
JOIN Usuarios u ON tu.IdUsuario = u.IdUsuario
WHERE tu.TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890'
AND tu.Activo = 1;
```

### Ver tenants de un usuario:
```sql
SELECT 
    t.Nombre,
    t.Subdomain,
    tu.FechaAsignacion
FROM TenantUsers tu
JOIN Tenants t ON tu.TenantId = t.Id
WHERE tu.IdUsuario = '<GUID del usuario>'
AND tu.Activo = 1;
```

## Próximos Pasos

1. **Crear política SuperAdminOnly** en Program.cs para endpoints de administración
2. **Validar JWT tenant claim** contra subdomain (doble validación)
3. **Agregar TenantId automático** en command handlers de creación
4. **Panel de administración** para gestionar tenants visualmente
5. **Auditoría** de cambios de tenants y asignaciones

---

**Autor:** AI Assistant  
**Fecha:** 2026-03-03  
**Versión:** 1.0

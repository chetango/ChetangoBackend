# GUÍA DE ADMINISTRACIÓN DE TENANTS (SUPER ADMIN)

## Descripción General

Arquitectura multi-tenancy donde **una sola infraestructura soporta N academias**, cada una con sus datos completamente aislados.

✅ **Email-based tenant resolution** - El tenant se determina desde el dominio del email (`@salsalatina.aphellion.com`)
✅ **Subdomain fallback** - Si hay subdominio en el host de la API, también se usa para resolver el tenant
✅ **Row-Level Security (RLS)** - Protección a nivel de base de datos contra bugs y SQL injection
✅ **Scoped TenantProvider** - Cada request HTTP tiene su propio TenantId
✅ **SESSION_CONTEXT** - El TenantId se establece en cada conexión SQL
✅ **TenantUsers** - Tabla many-to-many que vincula usuarios a academias

---

## Infraestructura Actual (Producción)

| Recurso | Valor |
|---|---|
| Frontend (Static Web App) | `delightful-plant-02670d70f.1.azurestaticapps.net` |
| URL Chetango | `corporacionchetango.aphellion.com` / `app.corporacionchetango.com` |
| URL genérica (todas las academias) | `delightful-plant-02670d70f.1.azurestaticapps.net` |
| Backend API | `chetango-api-prod.azurewebsites.net` |
| DNS Zone | `aphellion.com` en Azure (`rg-chetango-prod`) |
| Azure Entra ID (CIAM) | `chetangoprueba.onmicrosoft.com` |

---

## Cómo Funciona el Aislamiento de Datos

El backend resuelve el `TenantId` con esta cascada de prioridades:

```
Request llega al API
        ↓
1. TenantResolutionMiddleware:
   ¿Hay subdominio en el host? (ej: salsalatina.chetango-api.net)
   → SÍ: SELECT Id FROM Tenants WHERE Subdomain = 'salsalatina'
   → NO: continuar al siguiente paso
        ↓
2. TenantService (email del JWT):
   email del usuario = juan@salsalatina.aphellion.com
   → dominio = salsalatina.aphellion.com
   → SELECT Id FROM Tenants WHERE Dominio = 'salsalatina.aphellion.com'
        ↓
3. Fallback TenantUsers:
   Si los pasos anteriores no resuelven:
   → SELECT TenantId FROM TenantUsers WHERE IdUsuario = <guid> AND Activo = 1
        ↓
4. TenantDbConnectionInterceptor:
   En CADA conexión SQL:
   → EXEC sp_set_session_context @key='TenantId', @value=<TenantId>
        ↓
5. Row-Level Security (SQL Server):
   Filtra automáticamente TODAS las queries:
   → WHERE TenantId = SESSION_CONTEXT('TenantId')
```

**Conclusión:** El email `@salsalatina.aphellion.com` es suficiente para aislar datos. El subdominio en el frontend es opcional (solo branding).

---

## Proceso de Registro de Nueva Academia

### Paso 1 — Registrar el Dominio en Azure DNS Zone

En Azure Portal → **Zonas DNS** → `aphellion.com` → **+ Agregar conjunto de registros**:

```
Nombre:  salsalatina
Tipo:    TXT
Valor:   MS-ms<código que te da Azure Entra ID>
TTL:     3600
```

> ⚠️ Este registro TXT es para verificar que eres dueño del dominio. Azure Entra ID te dará el valor exacto en el siguiente paso.

---

### Paso 2 — Agregar y Verificar el Dominio en Azure Entra ID

1. Azure Portal → **Microsoft Entra ID** → **Nombres de dominio personalizados** → **+ Agregar dominio personalizado**
2. Ingresa: `salsalatina.aphellion.com`
3. Azure te muestra el registro TXT a agregar (ya lo hiciste en el Paso 1)
4. Clic en **Verificar** → Estado: ✅ Verificado

> Una vez verificado, `salsalatina.aphellion.com` aparece en el dropdown al crear usuarios.

---

### Paso 3 — Crear Usuarios en Azure Entra ID

Azure Portal → **Entra ID** → **Usuarios** → **+ Nuevo usuario** → **Crear usuario interno**:

```
Nombre principal: admin@salsalatina.aphellion.com
Nombre mostrar:   Administrador Salsa Latina
Contraseña:       (generar automáticamente o definir)
```

Repetir para cada usuario de la academia (profesores, alumnos, etc.):
```
profesor1@salsalatina.aphellion.com
alumno1@salsalatina.aphellion.com
```

> **Importante:** El dominio DEBE ser `salsalatina.aphellion.com` para que el backend identifique automáticamente a qué academia pertenece el usuario.

---

### Paso 4 — Crear la Academia en el Backend (Super Admin)

**Endpoint:** `POST /api/super-admin/academias`

```json
{
  "nombreTenant": "Salsa Latina",
  "subdomain": "salsalatina",
  "dominioCompleto": "salsalatina.aphellion.com",
  "plan": "Basic",
  "maxSedes": 2,
  "maxAlumnos": 100,
  "maxProfesores": 20,
  "maxStorageMB": 2000,
  "nombreUsuario": "Administrador Salsa Latina",
  "correoAdmin": "admin@salsalatina.aphellion.com",
  "idTipoDocumento": "<GUID del tipo CC>",
  "numeroDocumento": "123456789",
  "telefono": "3001234567"
}
```

**Respuesta exitosa:**
```json
{
  "success": true,
  "tenantId": "12345678-1234-1234-1234-123456789012",
  "idUsuario": "87654321-4321-4321-4321-210987654321",
  "message": "Academia 'Salsa Latina' creada exitosamente con administrador 'Administrador Salsa Latina'."
}
```

**Este comando hace TODO automáticamente:**
- ✅ Crea el registro `Tenant` con TenantId único
- ✅ Crea el `Usuario` en la tabla Usuarios (si no existe)
- ✅ Crea el vínculo en `TenantUsers`
- ✅ El admin ya puede loguearse

---

### Paso 5 — Entregar Acceso al Administrador

Enviarle al administrador de la academia:

```
🌐 URL de acceso:  https://delightful-plant-02670d70f.1.azurestaticapps.net/login
👤 Usuario:        admin@salsalatina.aphellion.com
🔑 Contraseña:     (la definida en Azure Entra ID)
```

> El administrador podrá crear el resto de usuarios (profesores, alumnos) desde el panel de administración de la app una vez dentro.

---

### Resumen del Proceso por Academia

| Paso | Dónde | Qué hacer |
|---|---|---|
| 1 | Azure DNS Zone `aphellion.com` | Agregar registro TXT de verificación |
| 2 | Azure Entra ID → Dominios | Verificar `salsalatina.aphellion.com` |
| 3 | Azure Entra ID → Usuarios | Crear `admin@salsalatina.aphellion.com` |
| 4 | API Backend (Super Admin) | `POST /api/super-admin/academias` |
| 5 | Entregar acceso | URL genérica + credenciales |

> ❌ **NO es necesario** agregar dominio personalizado al Static Web App (límite 2 en plan gratuito). La URL genérica funciona para todas las academias.

---

### Ejemplo Real: Academias Actuales

| Academia | Dominio Email | TenantId en DB |
|---|---|---|
| Corporación Chetango | `@corporacionchetango.com` | (existente en prod) |
| Salsa Latina (ejemplo) | `@salsalatina.aphellion.com` | (crear con endpoint) |
| Nueva Academia (futuro) | `@nuevaacademia.aphellion.com` | (crear con endpoint) |

---

## Flujo Alternativo: Paso por Paso

Si prefieres crear tenant y usuario por separado:

#### 1. Crear Solo el Tenant

**Endpoint:** `POST /api/super-admin/tenants`

```json
{
  "nombre": "Salsa Latina",
  "subdomain": "salsalatina",
  "dominio": "salsalatina.aphellion.com",
  "plan": "Basic",
  "maxSedes": 2,
  "maxAlumnos": 100,
  "maxProfesores": 20,
  "maxStorageMB": 2000,
  "emailContacto": "contacto@salsalatina.aphellion.com"
}
```

#### 2. Registrar Usuario en Azure AD

(Mismo proceso que Paso 3 del flujo recomendado)

#### 3. Asignar Usuario al Tenant

**Endpoint:** `POST /api/super-admin/tenants/assign-user`

```json
{
  "tenantId": "12345678-1234-1234-1234-123456789012",
  "idUsuario": "<GUID del usuario en tabla Usuarios>"
}
```

---

## Arquitectura de Seguridad

### Capas de Seguridad

1. **Azure Entra ID Authentication** — Solo usuarios registrados en Entra ID pueden obtener JWT
2. **JWT Validation** — El token se valida en cada request
3. **Role-based Authorization** — Políticas Admin/Profesor/Alumno
4. **Email-domain Tenant Resolution** — El dominio del email determina la academia
5. **EF Core Query Filters** — Filtran queries en código C#
6. **Row-Level Security** — Filtra queries a nivel de SQL Server (última barrera)

### Protección Contra Ataques

**Escenario 1: Usuario de Salsa Latina intenta ver datos de Chetango**
- Su email es `@salsalatina.aphellion.com`
- Backend resuelve TenantId = Salsa Latina
- RLS filtra: solo ve datos de Salsa Latina ✅

**Escenario 2: Bug en código omite Query Filters**
- Código hace `.IgnoreQueryFilters()` por error
- Row-Level Security INTERCEPTA en SQL Server
- SQL Server agrega `WHERE TenantId = ...` automáticamente ✅

**Escenario 3: SQL Injection**
- RLS previene acceso a datos de otros tenants
- Daño limitado al tenant del atacante ✅

---

## Usuarios Multi-Academia

Un usuario puede trabajar en múltiples academias asignándolo a varios tenants:

**Ejemplo: Carlos es profesor en Chetango Y Salsa Latina**

1. Crear en Azure Entra ID: `carlos@corporacionchetango.com`
2. Asignar a Chetango (automático por dominio de email)
3. Asignar manualmente a Salsa Latina:
   ```
   POST /api/super-admin/tenants/assign-user
   { "tenantId": "<guid Salsa Latina>", "idUsuario": "<guid Carlos>" }
   ```

El sistema usa el `TenantUsers` con `OrderByDescending(FechaAsignacion)` para determinar cuál es su academia principal cuando no hay subdominio en el host.

## Consultas SQL Útiles

### Ver todos los tenants registrados:
```sql
SELECT Id, Nombre, Subdomain, Dominio, Plan, Estado, MaxAlumnos 
FROM Tenants 
ORDER BY Nombre;
```

### Ver usuarios por academia:
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

### Ver academias de un usuario específico:
```sql
SELECT 
    t.Nombre,
    t.Subdomain,
    t.Dominio,
    tu.FechaAsignacion
FROM TenantUsers tu
JOIN Tenants t ON tu.TenantId = t.Id
WHERE tu.IdUsuario = '<GUID del usuario>'
AND tu.Activo = 1;
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
**Fecha:** 2026-03-06  
**Versión:** 2.0 — Actualizado con flujo real de producción (aphellion.com, email-based resolution, URL genérica)

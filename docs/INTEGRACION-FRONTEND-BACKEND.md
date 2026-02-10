# 🔗 Integración Frontend-Backend - Guía de Patrones y Convenciones

## 📋 Tabla de Contenidos
- [Resumen](#resumen)
- [Autenticación y Perfil de Usuario](#autenticación-y-perfil-de-usuario)
- [Patrones de Endpoints](#patrones-de-endpoints)
- [DTOs y Contratos de Datos](#dtos-y-contratos-de-datos)
- [Validación de Ownership](#validación-de-ownership)
- [Paginación](#paginación)
- [Casos Resueltos](#casos-resueltos)

---

## Resumen

Este documento establece los patrones y convenciones para la integración entre el frontend React/TypeScript y el backend .NET 9, basados en casos reales resueltos durante el desarrollo del sistema Chetango.

**Última actualización:** 27 Enero 2026

---

## Autenticación y Perfil de Usuario

### ✅ Patrón Correcto: Endpoint `/api/auth/me`

El endpoint `/api/auth/me` es la fuente única de verdad para obtener el perfil del usuario autenticado.

#### Backend (`Program.cs`)
```csharp
app.MapGet("/api/auth/me", async (
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    var email = user.FindFirst(ClaimTypes.Email)?.Value
             ?? user.FindFirst("preferred_username")?.Value
             ?? user.FindFirst("upn")?.Value
             ?? user.FindFirst("emails")?.Value;
    
    if (string.IsNullOrWhiteSpace(email))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Correo == email);

    if (usuario == null)
        return Results.NotFound(new { message = "Usuario no existe en BD" });

    var roles = user.FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();

    // ⚠️ IMPORTANTE: Incluir idProfesor e idAlumno si existen
    var idProfesor = await db.Profesores
        .Where(p => p.IdUsuario == usuario.IdUsuario)
        .Select(p => p.IdProfesor)
        .FirstOrDefaultAsync();
    
    var idAlumno = await db.Alumnos
        .Where(a => a.IdUsuario == usuario.IdUsuario)
        .Select(a => a.IdAlumno)
        .FirstOrDefaultAsync();

    return Results.Ok(new
    {
        idUsuario = usuario.IdUsuario,
        nombre = usuario.NombreUsuario,
        correo = usuario.Correo,
        telefono = usuario.Telefono,
        roles = roles,
        idProfesor = idProfesor == Guid.Empty ? (Guid?)null : idProfesor,
        idAlumno = idAlumno == Guid.Empty ? (Guid?)null : idAlumno
    });
}).RequireAuthorization("ApiScope");
```

#### Frontend (`profileQueries.ts`)
```typescript
export interface UserProfileResponse {
  idUsuario: string
  nombre: string
  correo: string
  telefono: string
  roles: string[]
  idProfesor?: string | null  // ✅ REQUERIDO para endpoints de profesor
  idAlumno?: string | null    // ✅ REQUERIDO para endpoints de alumno
}
```

### ❌ Error Común
No incluir `idProfesor` o `idAlumno` en la respuesta de `/api/auth/me`, causando que el frontend no pueda construir URLs de endpoints específicos como `/api/profesores/{idProfesor}/clases`.

---

## Patrones de Endpoints

### Patrón 1: Endpoints por Rol con ID Específico

**Usar cuando:** El recurso pertenece específicamente a un profesor o alumno

```
GET /api/profesores/{idProfesor}/clases
GET /api/alumnos/{idAlumno}/paquetes
```

**Validación de Ownership en el Endpoint:**
```csharp
app.MapGet("/api/profesores/{idProfesor:guid}/clases", async (
    Guid idProfesor,
    IMediator mediator,
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    // 1. Validar que el profesor existe
    var profesor = await db.Profesores
        .Include(p => p.Usuario)
        .FirstOrDefaultAsync(p => p.IdProfesor == idProfesor);
    
    if (profesor is null)
        return Results.NotFound(new { error = "Profesor no encontrado" });

    // 2. Validar ownership por correo
    var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value
        ?? user.FindFirst("preferred_username")?.Value;
    
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase));

    // 3. Si no es admin, verificar que el correo coincida
    if (!esAdmin)
    {
        if (string.IsNullOrWhiteSpace(emailClaim) || 
            !string.Equals(profesor.Usuario.Correo, emailClaim, StringComparison.OrdinalIgnoreCase))
            return Results.Forbid();
    }

    // 4. Ejecutar query
    var query = new GetClasesDeProfesorQuery(idProfesor, ...);
    var result = await mediator.Send(query);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("AdminOrProfesor");
```

**✅ Ventajas:**
- Validación de ownership en el endpoint (más simple)
- El handler solo necesita ejecutar la lógica de negocio
- Fácil de probar y depurar

### Patrón 2: Endpoints "Mis Recursos"

**Usar cuando:** El usuario solo accede a sus propios recursos

```
GET /api/mis-paquetes
GET /api/mis-pagos
GET /api/reportes/mi-reporte
```

**Validación por Correo:**
```csharp
app.MapGet("/api/mis-paquetes", async (
    IMediator mediator,
    ClaimsPrincipal user) =>
{
    var correo = user.FindFirst(ClaimTypes.Email)?.Value
                 ?? user.FindFirst("preferred_username")?.Value;
    
    if (string.IsNullOrWhiteSpace(correo))
        return Results.Unauthorized();

    var query = new GetMisPaquetesQuery(CorreoUsuario: correo, ...);
    var result = await mediator.Send(query);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("ApiScope");
```

### Patrón 3: Dashboards por Rol

**Usar cuando:** El dashboard muestra datos específicos del rol del usuario autenticado

```
GET /api/reportes/dashboard/profesor
GET /api/reportes/mi-reporte (alumno)
```

**Validación solo por Correo (sin pasar IDs):**
```csharp
app.MapGet("/api/reportes/dashboard/profesor", async (
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var query = new GetDashboardProfesorQuery
    {
        EmailUsuario = emailClaim  // ✅ Solo correo, el handler busca el profesor
    };

    var result = await mediator.Send(query);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("ApiScope");
```

**Handler busca por correo:**
```csharp
public async Task<Result<DashboardProfesorDTO>> Handle(GetDashboardProfesorQuery request, ...)
{
    var profesor = await _db.Profesores
        .Include(p => p.Usuario)
        .FirstOrDefaultAsync(p => p.Usuario.Correo == request.EmailUsuario);
    
    if (profesor == null)
        return Result<DashboardProfesorDTO>.Failure("Profesor no encontrado");

    // Continuar con la lógica usando profesor.IdProfesor
}
```

---

## DTOs y Contratos de Datos

### ✅ Sincronización Backend-Frontend

**Regla de Oro:** El DTO del backend DEBE coincidir exactamente con el tipo TypeScript del frontend.

#### Ejemplo: Lista de Clases

**Backend (`ClaseDTO.cs`)**
```csharp
public record ClaseDTO(
    Guid IdClase,
    DateTime Fecha,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    string TipoClase,
    Guid IdProfesorPrincipal,
    string NombreProfesor,
    int CupoMaximo,        // ✅ DEBE estar presente
    int TotalAsistencias
);
```

**Frontend (`classTypes.ts`)**
```typescript
export interface ClaseListItemDTO {
  idClase: string              // Guid → string
  fecha: string                // DateTime → ISO 8601 string
  horaInicio: string           // TimeSpan → "HH:mm:ss"
  horaFin: string              // TimeSpan → "HH:mm:ss"
  tipoClase: string
  idProfesorPrincipal: string  // ✅ DEBE estar presente
  nombreProfesor: string       // ✅ DEBE estar presente
  cupoMaximo: number           // ✅ DEBE estar presente
  totalAsistencias: number
}
```

### ❌ Errores Comunes

1. **Falta una propiedad en el DTO del backend:**
   ```csharp
   // ❌ MAL - Falta CupoMaximo
   public record ClaseDTO(
       Guid IdClase,
       DateTime Fecha,
       string TipoClase,
       int TotalAsistencias  // Frontend espera cupoMaximo
   );
   ```

2. **El tipo TypeScript no coincide con el backend:**
   ```typescript
   // ❌ MAL - Faltan idProfesorPrincipal y nombreProfesor
   export interface ClaseListItemDTO {
     idClase: string
     fecha: string
     tipoClase: string
     cupoMaximo: number
     totalAsistencias: number
   }
   ```

---

## Validación de Ownership

### ❌ Patrón Incorrecto: Validar por OID

```csharp
// ❌ NO USAR - El OID de Azure no coincide con IdUsuario de la BD
var oidClaim = user.FindFirst("oid")?.Value;
if (profesor.IdUsuario.ToString() != oidClaim)
    return Results.Forbid();
```

### ✅ Patrón Correcto: Validar por Correo

```csharp
// ✅ USAR - El correo es la clave de identificación
var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value;
if (!string.Equals(profesor.Usuario.Correo, emailClaim, StringComparison.OrdinalIgnoreCase))
    return Results.Forbid();
```

**Razón:** Los usuarios se crean en Azure Entra CIAM con un OID diferente al `IdUsuario` generado en la base de datos. El correo es el único campo que coincide entre ambos sistemas.

---

## Paginación

### ✅ Nombres Estándar

**Backend espera:**
- `pageNumber` (int, base 1)
- `pageSize` (int)

**Frontend debe enviar:**
```typescript
queryParams.append('pageNumber', pagina.toString())
queryParams.append('pageSize', tamanoPagina.toString())
```

### ❌ Error Común

```typescript
// ❌ MAL - Backend no reconoce estos parámetros
queryParams.append('pagina', pagina.toString())
queryParams.append('tamanoPagina', tamanoPagina.toString())
```

### Implementación Correcta

```typescript
export function useClasesByProfesorQuery(idProfesor: string, params: ClasesQueryParams) {
  return useQuery({
    queryKey: classKeys.clasesByProfesor(idProfesor, params),
    queryFn: async () => {
      const queryParams = new URLSearchParams()
      if (params.fechaDesde) queryParams.append('fechaDesde', params.fechaDesde)
      if (params.fechaHasta) queryParams.append('fechaHasta', params.fechaHasta)
      if (params.pagina) queryParams.append('pageNumber', params.pagina.toString()) // ✅
      if (params.tamanoPagina) queryParams.append('pageSize', params.tamanoPagina.toString()) // ✅

      const url = `/api/profesores/${idProfesor}/clases?${queryParams}`
      const response = await httpClient.get<PaginatedResponse<ClaseListItemDTO>>(url)
      return response.data
    },
    enabled: !!idProfesor,
  })
}
```

---

## Casos Resueltos

### Caso 1: Clases del Profesor No Se Mostraban

**Problema:**
- El dashboard del profesor mostraba clases correctamente
- La página "Mis Clases" no mostraba nada

**Causa Raíz:**
1. `/api/auth/me` no devolvía `idProfesor`
2. El frontend no podía construir la URL `/api/profesores/{idProfesor}/clases`
3. El DTO `ClaseDTO` no incluía `CupoMaximo`
4. Los parámetros de query no coincidían (`pagina` vs `pageNumber`)

**Solución:**
1. ✅ Agregar `idProfesor` e `idAlumno` a `/api/auth/me`
2. ✅ Agregar `CupoMaximo` al DTO `ClaseDTO`
3. ✅ Actualizar el tipo TypeScript `ClaseListItemDTO` con todas las propiedades
4. ✅ Cambiar parámetros de query a `pageNumber` y `pageSize`
5. ✅ Validar ownership por correo en el endpoint (no en el handler)

**Archivos Modificados:**
- `Chetango.Api/Program.cs` (línea 362-395) - Endpoint `/api/auth/me`
- `Chetango.Api/Program.cs` (línea 774-810) - Endpoint `/api/profesores/{id}/clases`
- `Chetango.Application/Clases/DTOs/ClaseDTO.cs` - Agregar `CupoMaximo`
- `Chetango.Application/Clases/Queries/GetClasesDeProfesorQueryHandler.cs` - Simplificar validación
- `chetango-frontend/src/features/classes/api/classQueries.ts` - Corregir parámetros
- `chetango-frontend/src/features/classes/types/classTypes.ts` - Actualizar tipo
- `chetango-frontend/src/features/classes/hooks/useProfesorClasses.ts` - Usar profile para idProfesor

---

## Checklist de Integración

Al crear un nuevo endpoint que requiere datos de usuario:

- [ ] ¿El endpoint `/api/auth/me` devuelve todos los IDs necesarios (`idProfesor`, `idAlumno`)?
- [ ] ¿El DTO del backend incluye TODAS las propiedades que el frontend necesita?
- [ ] ¿El tipo TypeScript coincide exactamente con el DTO del backend?
- [ ] ¿Los parámetros de query usan nombres estándar (`pageNumber`, `pageSize`, `fechaDesde`, `fechaHasta`)?
- [ ] ¿La validación de ownership se hace por correo, no por OID?
- [ ] ¿El endpoint valida ownership en el endpoint mismo, no en el handler?
- [ ] ¿El frontend obtiene los IDs del usuario desde el profile query, no del token?

---

## Referencias

- [API Contract - Autenticación](./FRONTEND-AUTH-SETUP.md)
- [API Contract - Asistencias](./API-CONTRACT-ASISTENCIAS.md)
- [API Contract - Clases](./API-CONTRACT-CLASES.md)
- [API Contract - Paquetes](./API-CONTRACT-PAQUETES.md)
- [Módulos del Sistema](./MODULOS-SISTEMA.md)

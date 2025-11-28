using Microsoft.EntityFrameworkCore;
using Chetango.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using System.Security.Claims;
using MediatR;
using Chetango.Application.Clases.Queries.GetClasesDeAlumno;
using Chetango.Application.Asistencias.Commands.RegistrarAsistencia;
using Chetango.Application.Asistencias.Commands.ActualizarEstadoAsistencia;
using Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase;
using Chetango.Application.Asistencias.Queries.GetAsistenciasPorAlumno;
using Chetango.Application.Asistencias.Admin.DTOs;
using Chetango.Application.Asistencias.Admin.Queries.GetDiasConClasesAdmin;
using Chetango.Application.Asistencias.Admin.Queries.GetClasesDelDiaAdmin;
using Chetango.Application.Asistencias.Admin.Queries.GetResumenAsistenciasClaseAdmin;
using Chetango.Domain.Entities; // Added for Usuario
using Chetango.Domain.Entities.Estados; // Added for TipoDocumento
using Chetango.Application.Common; // registrar IAppDbContext
using Chetango.Api.Infrastructure; // MigrationRunner
using Microsoft.OpenApi.Models; // Swagger security
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Configuración de EF Core: registra el DbContext con la cadena de conexión de SQL Server
builder.Services.AddDbContext<ChetangoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChetangoConnection")));
// Mapear interfaz de Application al DbContext concreto
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<ChetangoDbContext>());

// CORS por entorno
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// Swagger/OpenAPI con Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chetango API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer. Use: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Autenticación JWT contra Azure Entra ID: valida tokens emitidos para la API
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.Events = new JwtBearerEvents
        {
            // Evento donde aprovisionamos / sincronizamos usuario interno usando claim oid
            OnTokenValidated = async ctx =>
            {
                var sp = ctx.HttpContext.RequestServices;
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Auth");
                try
                {
                    var principal = ctx.Principal;
                    var oid = principal?.FindFirstValue("oid") ?? principal?.FindFirstValue("sub");
                    var email = principal?.FindFirstValue(ClaimTypes.Email)
                               ?? principal?.FindFirst("preferred_username")?.Value
                               ?? principal?.FindFirst("upn")?.Value
                               ?? principal?.FindFirst("emails")?.Value;
                    var name = principal?.FindFirstValue("name") ?? email ?? oid ?? "usuario";
                    var phone = principal?.FindFirstValue(ClaimTypes.MobilePhone)
                              ?? principal?.FindFirstValue(ClaimTypes.HomePhone)
                              ?? principal?.FindFirstValue(ClaimTypes.OtherPhone);

                    if (string.IsNullOrWhiteSpace(oid) && string.IsNullOrWhiteSpace(email)) return; // nada útil

                    var db = sp.GetRequiredService<ChetangoDbContext>();

                    // Buscar por correo único si existe, si no por documento (usaremos oid)
                    var usuario = !string.IsNullOrWhiteSpace(email)
                        ? await db.Usuarios.FirstOrDefaultAsync(u => u.Correo == email)
                        : null;

                    if (usuario == null)
                    {
                        // Resolver TipoDocumento "OID" (si no existe, tomar cualquiera)
                        var tipoOid = await db.Set<TipoDocumento>().AsNoTracking().FirstOrDefaultAsync(t => t.Nombre == "OID");
                        var tipoDocId = tipoOid?.Id
                                       ?? await db.Set<TipoDocumento>().AsNoTracking().Select(t => t.Id).FirstAsync();

                        var numeroDoc = (oid ?? email ?? Guid.NewGuid().ToString());
                        if (numeroDoc.Length > 50) numeroDoc = numeroDoc[..50];

                        usuario = new Usuario
                        {
                            IdUsuario = Guid.NewGuid(),
                            NombreUsuario = (name.Length > 100 ? name[..100] : name),
                            IdTipoDocumento = tipoDocId,
                            NumeroDocumento = numeroDoc,
                            Correo = email ?? ($"{oid}@placeholder.local"),
                            Telefono = phone ?? string.Empty,
                            IdEstadoUsuario = 1 // Activo
                        };
                        db.Usuarios.Add(usuario);
                        await db.SaveChangesAsync();
                        logger.LogInformation("Usuario aprovisionado: {Correo}", usuario.Correo);
                    }
                    else
                    {
                        // Sync mínimo de perfil
                        var changed = false;
                        var newNombre = (name.Length > 100 ? name[..100] : name);
                        if (usuario.NombreUsuario != newNombre) { usuario.NombreUsuario = newNombre; changed = true; }
                        var newPhone = phone ?? string.Empty;
                        if (usuario.Telefono != newPhone) { usuario.Telefono = newPhone; changed = true; }
                        if (changed) { await db.SaveChangesAsync(); logger.LogInformation("Usuario sincronizado: {Correo}", usuario.Correo); }
                    }

                    // NUEVO: Cargar roles del usuario y agregarlos como Claims
                    var roles = await db.Set<UsuarioRol>()
                        .Where(ur => ur.IdUsuario == usuario.IdUsuario)
                        .Include(ur => ur.Rol)
                        .Select(ur => ur.Rol.Nombre)
                        .ToListAsync();

                    if (roles.Any())
                    {
                        var identity = principal.Identity as ClaimsIdentity;
                        foreach (var rol in roles)
                        {
                            identity?.AddClaim(new Claim(ClaimTypes.Role, rol));
                        }
                        logger.LogInformation("Roles asignados a {Email}: {Roles}", 
                            usuario.Correo, string.Join(", ", roles));
                    }
                }
                catch (Exception ex)
                {
                    // No bloquear autenticación
                    logger.LogError(ex, "Error aprovisionando usuario interno");
                }
            }
        };
    }, options => { builder.Configuration.Bind("AzureAd", options); });

// Autorización con políticas (scopes/roles desde configuración)
var requiredScopes = builder.Configuration.GetSection("Auth:RequiredScopes").Get<string[]>() ?? Array.Empty<string>();
var requiredRoles = builder.Configuration.GetSection("Auth:RequiredRoles").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        if (requiredScopes.Length > 0) policy.RequireScope(requiredScopes);
        else policy.RequireAuthenticatedUser();
    });
    if (requiredRoles.Length > 0)
    {
        options.AddPolicy("ApiRole", policy => policy.RequireRole(requiredRoles));
    }
});

var adminAuthPolicies = requiredRoles.Length > 0
    ? new[] { "ApiScope", "ApiRole" }
    : new[] { "ApiScope" };

// Registro de MediatR compatible con v11
builder.Services.AddMediatR(typeof(GetClasesDeAlumnoQuery).Assembly);

var app = builder.Build();

// Apply EF migrations automatically per environment
MigrationRunner.Apply(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chetango API v1");
        c.DocumentTitle = "Chetango API - Swagger";
    });

    // Simple ReDoc page via CDN (no extra package)
    app.MapGet("/redoc", () =>
    {
        var html = """
<!DOCTYPE html>
<html>
  <head>
    <title>Chetango API - ReDoc</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="https://fonts.googleapis.com/css?family=Montserrat:300,400,700|Roboto:300,400,700" rel="stylesheet">
    <style> body { margin: 0; padding: 0; } </style>
  </head>
  <body>
    <redoc spec-url='/swagger/v1/swagger.json'></redoc>
    <script src="https://cdn.redoc.ly/redoc/latest/bundles/redoc.standalone.js"></script>
  </body>
</html>
""";
        return Results.Content(html, "text/html");
    }).AllowAnonymous();
}

app.UseHttpsRedirection();
app.UseCors("DefaultCors");
app.UseAuthentication(); // Debe ir antes de Authorization
app.UseAuthorization();

// ====== ENDPOINTS ADMINISTRADOR - ASISTENCIAS ======
var adminAsistencias = app.MapGroup("/api/admin/asistencias")
    .WithTags("Admin - Asistencias")
    .RequireAuthorization(adminAuthPolicies);

adminAsistencias.MapGet("/dias-con-clases", async (
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(new GetDiasConClasesAdminQuery(), ct);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
})
.WithName("GetDiasConClasesAdmin")
.WithSummary("Obtiene el rango de fecha útil para el calendario del administrador.")
.WithDescription("Devuelve el día actual, el rango de los últimos 7 días y qué días tienen clases programadas.")
.Produces<DiasConClasesAdminDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithOpenApi();

adminAsistencias.MapGet("/clases-del-dia", async (
    DateOnly fecha,
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(new GetClasesDelDiaAdminQuery(fecha), ct);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
})
.WithName("GetClasesDelDiaAdmin")
.WithSummary("Lista las clases disponibles para la fecha seleccionada.")
.WithDescription("La respuesta alimenta el combo \"Clase del Día\" con nombre, horario y profesor principal.")
.Produces<ClasesDelDiaAdminDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithOpenApi(op =>
{
    if (op.Parameters.Count > 0)
    {
        op.Parameters[0].Description = "Fecha a consultar en formato YYYY-MM-DD";
    }
    return op;
});

adminAsistencias.MapGet("/clase/{idClase:guid}/resumen", async (
    Guid idClase,
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(new GetResumenAsistenciasClaseAdminQuery(idClase), ct);
    if (!result.Succeeded)
    {
        var message = result.Error ?? "Error al obtener el resumen";
        return string.Equals(message, "Clase no encontrada", StringComparison.OrdinalIgnoreCase)
            ? Results.NotFound(message)
            : Results.BadRequest(message);
    }

    return Results.Ok(result.Value);
})
.WithName("GetResumenAsistenciasClaseAdmin")
.WithSummary("Obtiene el estado completo de asistencias para una clase.")
.WithDescription("Incluye alumnos, estado de paquete, estado de asistencia y contadores para la tarjeta del administrador.")
.Produces<ResumenAsistenciasClaseAdminDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

// Endpoint protegido de prueba: permite verificar que el token es válido
app.MapGet("/auth/ping", (ClaimsPrincipal user) =>
{
    var oid = user.FindFirst("oid")?.Value ?? user.FindFirst("sub")?.Value;
    return Results.Ok(new { message = "pong", oid });
}).RequireAuthorization("ApiScope");

// GET /api/auth/me - Obtener perfil y roles del usuario autenticado
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
        return Results.NotFound();

    var roles = await db.Set<UsuarioRol>()
        .Where(ur => ur.IdUsuario == usuario.IdUsuario)
        .Include(ur => ur.Rol)
        .Select(ur => ur.Rol.Nombre)
        .ToListAsync();

    return Results.Ok(new
    {
        idUsuario = usuario.IdUsuario,
        nombre = usuario.NombreUsuario,
        correo = usuario.Correo,
        telefono = usuario.Telefono,
        roles = roles
    });
}).RequireAuthorization("ApiScope");

#if DEBUG
// ==========================================
// 🔧 ENDPOINTS DE DESARROLLO - SOLO DEBUG
// ==========================================
// Estos endpoints NO existen en builds Release (producción)
// Permiten simular autenticación sin Azure AD para pruebas con Postman/Swagger

// GET /api/dev/usuarios - Listar usuarios disponibles para testing
app.MapGet("/api/dev/usuarios", async (ChetangoDbContext db) =>
{
    var usuarios = await db.Usuarios
        .AsNoTracking()
        .Select(u => new
        {
            u.Correo,
            u.NombreUsuario,
            roles = db.Set<UsuarioRol>()
                .Where(ur => ur.IdUsuario == u.IdUsuario)
                .Include(ur => ur.Rol)
                .Select(ur => ur.Rol.Nombre)
                .ToList()
        })
        .ToListAsync();

    return Results.Ok(new
    {
        message = "⚠️ DESARROLLO: Lista de usuarios para testing",
        usuarios = usuarios
    });
})
.WithTags("🔧 Development Only")
.WithOpenApi()
.AllowAnonymous();

// GET /api/dev/me/{email} - Simular GET /api/auth/me sin autenticación
app.MapGet("/api/dev/me/{email}", async (string email, ChetangoDbContext db) =>
{
    var usuario = await db.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Correo == email);

    if (usuario == null)
        return Results.NotFound(new { message = $"Usuario con email '{email}' no encontrado" });

    var roles = await db.Set<UsuarioRol>()
        .Where(ur => ur.IdUsuario == usuario.IdUsuario)
        .Include(ur => ur.Rol)
        .Select(ur => ur.Rol.Nombre)
        .ToListAsync();

    return Results.Ok(new
    {
        message = "⚠️ DESARROLLO: Este endpoint simula /api/auth/me sin Azure AD",
        idUsuario = usuario.IdUsuario,
        nombre = usuario.NombreUsuario,
        correo = usuario.Correo,
        telefono = usuario.Telefono,
        roles = roles
    });
})
.WithTags("🔧 Development Only")
.WithOpenApi()
.AllowAnonymous();

// GET /api/dev/clases/{idClase}/asistencias - Simular endpoint con usuario específico
app.MapGet("/api/dev/clases/{idClase:guid}/asistencias", async (
    Guid idClase,
    string email, // Query param: ?email=jorge.padilla@chetango.com
    ChetangoDbContext db,
    IMediator mediator) =>
{
    // Verificar que el usuario existe
    var usuario = await db.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Correo == email);

    if (usuario == null)
        return Results.BadRequest(new { message = $"Usuario '{email}' no encontrado. Usa ?email=correo@ejemplo.com" });

    // Obtener roles del usuario
    var roles = await db.Set<UsuarioRol>()
        .Where(ur => ur.IdUsuario == usuario.IdUsuario)
        .Include(ur => ur.Rol)
        .Select(ur => ur.Rol.Nombre)
        .ToListAsync();

    // Verificar clase existe
    var claseExiste = await db.Clases.AnyAsync(c => c.IdClase == idClase);
    if (!claseExiste)
        return Results.NotFound(new { message = $"Clase con ID '{idClase}' no encontrada" });

    // Aplicar lógica de autorización
    if (roles.Contains("Administrador"))
    {
        // Admin: acceso total
        var query = new GetAsistenciasPorClaseQuery(idClase);
        var result = await mediator.Send(query);

        return Results.Ok(new
        {
            message = "⚠️ DESARROLLO: Usuario es Administrador - Acceso total",
            usuario = email,
            roles = roles,
            asistencias = result.Value
        });
    }

    if (roles.Contains("Profesor"))
    {
        // Profesor: solo sus clases
        var esProfesorDeClase = await db.Clases
            .Where(c => c.IdClase == idClase && c.ProfesorPrincipal.Usuario.Correo == email)
            .AnyAsync();

        if (!esProfesorDeClase)
        {
            return Results.Json(
                new
                {
                    message = "⚠️ DESARROLLO: Usuario es Profesor pero NO es dueño de esta clase - 403 Forbidden",
                    usuario = email,
                    roles = roles,
                    error = "No tienes permiso para ver las asistencias de esta clase"
                },
                statusCode: 403);
        }

        var query = new GetAsistenciasPorClaseQuery(idClase);
        var result = await mediator.Send(query);

        return Results.Ok(new
        {
            message = "⚠️ DESARROLLO: Usuario es Profesor dueño de esta clase - Acceso permitido",
            usuario = email,
            roles = roles,
            asistencias = result.Value
        });
    }

    // Sin roles válidos
    return Results.Json(
        new
        {
            message = "⚠️ DESARROLLO: Usuario no tiene rol Administrador ni Profesor - 403 Forbidden",
            usuario = email,
            roles = roles,
            error = "No tienes permiso para ver asistencias"
        },
        statusCode: 403);
})
.WithTags("🔧 Development Only")
.WithOpenApi()
.AllowAnonymous();

#endif
// ==========================================
// FIN ENDPOINTS DE DESARROLLO
// ==========================================

// Endpoint de consulta de clases por alumno (ejemplo CQRS)
app.MapGet("/api/alumnos/{idAlumno:guid}/clases", async (
    Guid idAlumno,
    DateTime? desde,
    DateTime? hasta,
    IMediator mediator,
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    // Autorización: validar que el usuario autenticado sea dueño (por correo)
    var email = user.FindFirst(ClaimTypes.Email)?.Value
             ?? user.FindFirst("preferred_username")?.Value
             ?? user.FindFirst("upn")?.Value
             ?? user.FindFirst("emails")?.Value;

    var ownerCorreo = await db.Alumnos
        .AsNoTracking()
        .Where(a => a.IdAlumno == idAlumno)
        .Select(a => a.Usuario.Correo)
        .SingleOrDefaultAsync();

    if (ownerCorreo is null) return Results.NotFound();
    if (!string.Equals(ownerCorreo, email, StringComparison.OrdinalIgnoreCase)) return Results.Forbid();

    var result = await mediator.Send(new GetClasesDeAlumnoQuery(idAlumno, desde, hasta));
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("ApiScope");

// Endpoints de desarrollo SIN autenticación para pruebas locales (solo en Development)
if (app.Environment.IsDevelopment())
{
    // Dev ping sin auth
    app.MapGet("/dev/ping", () => Results.Ok(new { message = "pong-dev" }))
       .AllowAnonymous();

    // Consulta de clases por alumno usando el CQRS, sin validación de propietario ni token
    app.MapGet("/api/dev/alumnos/{idAlumno:guid}/clases", async (
        Guid idAlumno,
        DateTime? desde,
        DateTime? hasta,
        IMediator mediator) =>
    {
        var result = await mediator.Send(new GetClasesDeAlumnoQuery(idAlumno, desde, hasta));
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    })
    .AllowAnonymous();

    // Atajo: clases del alumno sembrado por defecto
    app.MapGet("/api/dev/alumnos/seeded/clases", async (IMediator mediator) =>
    {
        var idAlumno = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff");
        var result = await mediator.Send(new GetClasesDeAlumnoQuery(idAlumno, null, null));
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    })
    .AllowAnonymous();

    // Estado de semillas mínimas en BD
    app.MapGet("/api/dev/seed/status", async (ChetangoDbContext db) =>
    {
        var usuarioId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var alumnoId = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff");
        var profesorId = Guid.Parse("cccccccc-dddd-eeee-ffff-000000000000");
        var claseId = Guid.Parse("dddddddd-eeee-ffff-0000-111111111111");
        var paqueteId = Guid.Parse("f0f0f0f0-0000-0000-0000-000000000000");
        var asistenciaId = Guid.Parse("eeeeeeee-ffff-0000-1111-222222222222");

        var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.IdUsuario == usuarioId);
        var alumno = await db.Alumnos.AsNoTracking().FirstOrDefaultAsync(x => x.IdAlumno == alumnoId);
        var profesor = await db.Profesores.AsNoTracking().FirstOrDefaultAsync(x => x.IdProfesor == profesorId);
        var clase = await db.Clases.AsNoTracking().FirstOrDefaultAsync(x => x.IdClase == claseId);
        var paquete = await db.Paquetes.AsNoTracking().FirstOrDefaultAsync(x => x.IdPaquete == paqueteId);
        var asistencia = await db.Asistencias.AsNoTracking().FirstOrDefaultAsync(x => x.IdAsistencia == asistenciaId);

        return Results.Ok(new
        {
            usuario = usuario is null ? null : new { usuario.IdUsuario, usuario.Correo, usuario.NombreUsuario },
            alumno = alumno is null ? null : new { alumno.IdAlumno, alumno.IdUsuario },
            profesor = profesor is null ? null : new { profesor.IdProfesor },
            clase = clase is null ? null : new { clase.IdClase, clase.Fecha, clase.HoraInicio, clase.HoraFin },
            paquete = paquete is null ? null : new { paquete.IdPaquete, paquete.IdTipoPaquete, paquete.ClasesDisponibles, paquete.ClasesUsadas, paquete.FechaVencimiento },
            asistencia = asistencia is null ? null : new { asistencia.IdAsistencia, asistencia.IdClase, asistencia.IdAlumno, IdPaqueteUsado = asistencia.IdPaqueteUsado }
        });
    })
    .AllowAnonymous();
}

// ====== ENDPOINTS DE ASISTENCIAS ======

// POST /api/asistencias - Registrar asistencia (protegido)
app.MapPost("/api/asistencias", async (
    Chetango.Application.Asistencias.Commands.RegistrarAsistencia.RegistrarAsistenciaCommand command,
    IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return result.Succeeded ? Results.Created($"/api/asistencias/{result.Value}", result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("ApiScope");

// PUT /api/asistencias/{id}/estado - Actualizar estado de asistencia (protegido)
app.MapPut("/api/asistencias/{id:guid}/estado", async (
    Guid id,
    Chetango.Application.Asistencias.Commands.ActualizarEstadoAsistencia.ActualizarEstadoAsistenciaCommand command,
    IMediator mediator) =>
{
    if (id != command.IdAsistencia) return Results.BadRequest("ID en ruta no coincide con el comando");
    var result = await mediator.Send(command);
    return result.Succeeded ? Results.NoContent() : Results.BadRequest(result.Error);
}).RequireAuthorization("ApiScope");

// GET /api/clases/{idClase}/asistencias - Obtener asistencias de una clase (protegido, profesor/admin)
app.MapGet("/api/clases/{idClase:guid}/asistencias", async (
    Guid idClase,
    IMediator mediator,
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var email = user.FindFirst(ClaimTypes.Email)?.Value
             ?? user.FindFirst("preferred_username")?.Value
             ?? user.FindFirst("upn")?.Value
             ?? user.FindFirst("emails")?.Value;

    // Administrador: acceso total a cualquier clase
    if (roles.Contains("Administrador"))
    {
        var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase.GetAsistenciasPorClaseQuery(idClase);
        var result = await mediator.Send(query);
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }

    // Profesor: solo puede ver asistencias de SUS clases
    if (roles.Contains("Profesor"))
    {
        if (string.IsNullOrWhiteSpace(email))
            return Results.Unauthorized();

        // Verificar que la clase pertenece al profesor autenticado
        var esProfesorDeClase = await db.Clases
            .AsNoTracking()
            .Include(c => c.ProfesorPrincipal)
            .ThenInclude(p => p.Usuario)
            .Where(c => c.IdClase == idClase && c.ProfesorPrincipal.Usuario.Correo == email)
            .AnyAsync();

        if (!esProfesorDeClase)
            return Results.Forbid(); // 403: El profesor no imparte esta clase

        var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase.GetAsistenciasPorClaseQuery(idClase);
        var result = await mediator.Send(query);
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }

    // Si no es Admin ni Profesor, denegar acceso
    return Results.Forbid();
}).RequireAuthorization("ApiScope");

// GET /api/alumnos/{idAlumno}/asistencias - Obtener asistencias de un alumno (protegido, owner/admin)
app.MapGet("/api/alumnos/{idAlumno:guid}/asistencias", async (
    Guid idAlumno,
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    IMediator mediator,
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    // Autorización: validar que el usuario autenticado sea dueño del alumno (por correo)
    var email = user.FindFirst(ClaimTypes.Email)?.Value
             ?? user.FindFirst("preferred_username")?.Value
             ?? user.FindFirst("upn")?.Value
             ?? user.FindFirst("emails")?.Value;

    var ownerCorreo = await db.Alumnos
        .AsNoTracking()
        .Where(a => a.IdAlumno == idAlumno)
        .Select(a => a.Usuario.Correo)
        .SingleOrDefaultAsync();

    if (ownerCorreo is null) return Results.NotFound();
    if (!string.Equals(ownerCorreo, email, StringComparison.OrdinalIgnoreCase)) return Results.Forbid();

    var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorAlumno.GetAsistenciasPorAlumnoQuery(idAlumno, fechaDesde, fechaHasta);
    var result = await mediator.Send(query);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("ApiScope");

// ====== ENDPOINTS DE DESARROLLO PARA ASISTENCIAS (solo Development, sin auth) ======
if (app.Environment.IsDevelopment())
{
    // POST /api/dev/asistencias - Registrar asistencia sin autenticación (testing)
    app.MapPost("/api/dev/asistencias", async (
        Chetango.Application.Asistencias.Commands.RegistrarAsistencia.RegistrarAsistenciaCommand command,
        IMediator mediator) =>
    {
        var result = await mediator.Send(command);
        return result.Succeeded ? Results.Created($"/api/dev/asistencias/{result.Value}", result.Value) : Results.BadRequest(result.Error);
    }).AllowAnonymous();

    // PUT /api/dev/asistencias/{id}/estado - Actualizar estado sin autenticación (testing)
    app.MapPut("/api/dev/asistencias/{id:guid}/estado", async (
        Guid id,
        Chetango.Application.Asistencias.Commands.ActualizarEstadoAsistencia.ActualizarEstadoAsistenciaCommand command,
        IMediator mediator) =>
    {
        if (id != command.IdAsistencia) return Results.BadRequest("ID en ruta no coincide con el comando");
        var result = await mediator.Send(command);
        return result.Succeeded ? Results.NoContent() : Results.BadRequest(result.Error);
    }).AllowAnonymous();

    // GET /api/dev/alumnos/{idAlumno}/asistencias - Obtener asistencias de alumno sin autenticación
    app.MapGet("/api/dev/alumnos/{idAlumno:guid}/asistencias", async (
        Guid idAlumno,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        IMediator mediator) =>
    {
        var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorAlumno.GetAsistenciasPorAlumnoQuery(idAlumno, fechaDesde, fechaHasta);
        var result = await mediator.Send(query);
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }).AllowAnonymous();

    // Atajo: asistencias del alumno sembrado por defecto
    app.MapGet("/api/dev/alumnos/seeded/asistencias", async (IMediator mediator) =>
    {
        var idAlumno = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff");
        var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorAlumno.GetAsistenciasPorAlumnoQuery(idAlumno, null, null);
        var result = await mediator.Send(query);
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }).AllowAnonymous();

    // Atajo: asistencias de la clase sembrada por defecto
    app.MapGet("/api/dev/clases/seeded/asistencias", async (IMediator mediator) =>
    {
        var idClase = Guid.Parse("dddddddd-eeee-ffff-0000-111111111111");
        var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase.GetAsistenciasPorClaseQuery(idClase);
        var result = await mediator.Send(query);
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }).AllowAnonymous();
}

// Endpoint temporal generado por plantilla (se eliminará al avanzar con CQRS / Controllers)
var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

app.Run();
using Microsoft.EntityFrameworkCore;
using Chetango.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Chetango.Application.Clases.Queries.GetClasesDeAlumno;
using Chetango.Application.Clases.Commands.CrearClase;
using Chetango.Application.Clases.Commands.EditarClase;
using Chetango.Application.Clases.Commands.CancelarClase;
using Chetango.Application.Clases.Queries.GetClaseById;
using Chetango.Application.Clases.Queries.GetClasesDeProfesor;
using Chetango.Application.Clases.Queries.GetTiposClase;
using Chetango.Application.Clases.Queries.GetProfesores;
using Chetango.Application.Clases.Queries.GetAlumnos;
using Chetango.Application.Clases.DTOs;
using Chetango.Application.Paquetes.Commands.CrearPaquete;
using Chetango.Application.Paquetes.Commands.EditarPaquete;
using Chetango.Application.Paquetes.Commands.CongelarPaquete;
using Chetango.Application.Paquetes.Commands.DescongelarPaquete;
using Chetango.Application.Paquetes.Queries.GetPaqueteById;
using Chetango.Application.Paquetes.Queries.GetPaquetesDeAlumno;
using Chetango.Application.Paquetes.Queries.GetPaquetes;
using Chetango.Application.Paquetes.Queries.GetEstadisticasPaquetes;
using Chetango.Application.Paquetes.Queries.GetTiposPaquete;
using Chetango.Application.Paquetes.Queries.GetMisPaquetes;
using Chetango.Application.Paquetes.DTOs;
using Chetango.Application.Asistencias.Commands.RegistrarAsistencia;
using Chetango.Application.Asistencias.Commands.ActualizarEstadoAsistencia;
using Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase;
using Chetango.Application.Asistencias.Queries.GetAsistenciasPorAlumno;
using Chetango.Application.Asistencias.Admin.DTOs;
using Chetango.Application.Asistencias.Admin.Queries.GetDiasConClasesAdmin;
using Chetango.Application.Asistencias.Admin.Queries.GetClasesDelDiaAdmin;
using Chetango.Application.Asistencias.Admin.Queries.GetResumenAsistenciasClaseAdmin;
using Chetango.Application.Pagos.Commands;
using Chetango.Application.Pagos.Queries;
using Chetango.Application.Pagos.DTOs;
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

// Autenticación JWT contra Entra External ID (CIAM): valida tokens emitidos para la API
var configuredAudience = builder.Configuration["AzureAd:Audience"];
var configuredClientId = builder.Configuration["AzureAd:ClientId"];
var validAudiences = new List<string>();
if (!string.IsNullOrWhiteSpace(configuredAudience)) validAudiences.Add(configuredAudience);
if (!string.IsNullOrWhiteSpace(configuredClientId)) validAudiences.Add(configuredClientId);
if (!string.IsNullOrWhiteSpace(configuredAudience) && configuredAudience.StartsWith("api://", StringComparison.OrdinalIgnoreCase))
{
    var stripped = configuredAudience.Substring("api://".Length);
    if (!string.IsNullOrWhiteSpace(stripped)) validAudiences.Add(stripped);
}
validAudiences = validAudiences.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

async Task ProvisionUsuarioAsync(TokenValidatedContext ctx)
{
    // Evento donde aprovisionamos / sincronizamos usuario interno usando claim oid
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

        if (string.IsNullOrWhiteSpace(oid) && string.IsNullOrWhiteSpace(email)) return; // nada útil

        // Roles desde App Roles (claim "roles") - SIEMPRE copiar para que policies funcionen
        var identity = principal?.Identity as ClaimsIdentity;
        if (identity != null)
        {
            var roles = principal!.FindAll("roles").Select(c => c.Value).Distinct(StringComparer.OrdinalIgnoreCase);
            foreach (var rol in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, rol));
            }

            var roleList = roles.ToList();
            if (roleList.Count > 0)
                logger.LogInformation("Roles desde token para {Email}: {Roles}", email ?? "unknown", string.Join(", ", roleList));
        }

        // Verificar usuario en BD (solo para endpoints que requieren ownership)
        var db = sp.GetRequiredService<ChetangoDbContext>();
        var usuario = !string.IsNullOrWhiteSpace(email)
            ? await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Correo == email)
            : null;

        if (usuario != null)
        {
            logger.LogInformation("Usuario encontrado en BD: {Email}", usuario.Correo);
        }
        else if (!string.IsNullOrWhiteSpace(email))
        {
            logger.LogWarning("Usuario autenticado pero no existe en BD: {Email}", email);
        }
    }
    catch (Exception ex)
    {
        // No bloquear autenticación
        logger.LogError(ex, "Error aprovisionando usuario interno");
    }
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(jwtOptions =>
    {
        builder.Configuration.Bind("AzureAd", jwtOptions);
        if (validAudiences.Count > 0)
        {
            jwtOptions.TokenValidationParameters.ValidAudience = null;
            jwtOptions.TokenValidationParameters.ValidAudiences = validAudiences;
        }

        jwtOptions.Events = new JwtBearerEvents { OnTokenValidated = ProvisionUsuarioAsync };
    }, msOptions => builder.Configuration.Bind("AzureAd", msOptions));

// Autorización con políticas (scopes/roles desde configuración)
var requiredScopes = builder.Configuration.GetSection("Auth:RequiredScopes").Get<string[]>() ?? Array.Empty<string>();
var requiredRoles = builder.Configuration.GetSection("Auth:RequiredRoles").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        // Simplificado para QA/Pruebas: solo requiere usuario autenticado
        policy.RequireAuthenticatedUser();
    });

    static bool HasAnyRole(ClaimsPrincipal principal, params string[] roles)
    {
        if (principal.Identity?.IsAuthenticated != true) return false;
        var roleClaims = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
        return roleClaims.Any(rc => roles.Any(r => string.Equals(rc, r, StringComparison.OrdinalIgnoreCase)));
    }

    // Roles aceptados (Entra App Roles y/o roles históricos internos)
    const string RoleAdminEntra = "admin";
    const string RoleAdminLegacy = "Administrador";
    const string RoleProfesorEntra = "profesor";
    const string RoleProfesorLegacy = "Profesor";

    options.AddPolicy("AdminOrProfesor", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(ctx => HasAnyRole(ctx.User, RoleAdminEntra, RoleAdminLegacy, RoleProfesorEntra, RoleProfesorLegacy));
    });

    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(ctx => HasAnyRole(ctx.User, RoleAdminEntra, RoleAdminLegacy));
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
    .RequireAuthorization("AdminOnly"); // Usar política explícita AdminOnly en lugar de adminAuthPolicies

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
    var oid = user.FindFirst("oid")?.Value
              ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
              ?? user.FindFirst("sub")?.Value
              ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
    email = string.IsNullOrWhiteSpace(email) ? email : email.Trim();

    if (string.IsNullOrWhiteSpace(email))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Correo == email);

    if (usuario == null)
        return Results.NotFound(new { message = "Usuario no existe en BD. Debe ser creado por seed/onboarding." });

    // Roles provienen del token de Entra ID (claim "roles"), copiados a ClaimTypes.Role en OnTokenValidated.
    var roles = user.FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .Where(r => !string.IsNullOrWhiteSpace(r))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();

    return Results.Ok(new
    {
        idUsuario = usuario.IdUsuario,
        nombre = usuario.NombreUsuario,
        correo = usuario.Correo,
        telefono = usuario.Telefono,
        roles = roles
    });
}).RequireAuthorization("ApiScope");

// Endpoints /dev/ eliminados por seguridad y clean code
// Se utilizan endpoints protegidos con OAuth 2.0 + Azure Entra CIAM

// Endpoint de consulta de clases por alumno (ejemplo CQRS)
app.MapGet("/api/alumnos/{idAlumno:guid}/clases", async (
    Guid idAlumno,
    DateTime? desde,
    DateTime? hasta,
    IMediator mediator,
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    // Autorización: dueño (por correo) o Administrador
    var email = user.FindFirst(ClaimTypes.Email)?.Value
             ?? user.FindFirst("preferred_username")?.Value
             ?? user.FindFirst("upn")?.Value
             ?? user.FindFirst("emails")?.Value;

    var isAdmin = user.FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
               || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var ownerCorreo = await db.Alumnos
        .AsNoTracking()
        .Where(a => a.IdAlumno == idAlumno)
        .Select(a => a.Usuario.Correo)
        .SingleOrDefaultAsync();

    if (ownerCorreo is null) return Results.NotFound();
    if (!isAdmin)
    {
        if (string.IsNullOrWhiteSpace(email)) return Results.Unauthorized();
        if (!string.Equals(ownerCorreo, email, StringComparison.OrdinalIgnoreCase)) return Results.Forbid();
    }

    var result = await mediator.Send(new GetClasesDeAlumnoQuery(idAlumno, desde, hasta));
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("ApiScope");

// Endpoints /dev/ de clases eliminados - usar endpoints protegidos con autenticación

// ====== ENDPOINTS DE CATÁLOGOS/LOOKUPS ======

// GET /api/tipos-clase - Obtener todos los tipos de clase disponibles
app.MapGet("/api/tipos-clase", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetTiposClaseQuery());
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/tipos-paquete - Obtener todos los tipos de paquete disponibles
app.MapGet("/api/tipos-paquete", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetTiposPaqueteQuery());
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/profesores - Obtener todos los profesores (solo Admin)
app.MapGet("/api/profesores", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetProfesoresQuery());
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/alumnos - Obtener todos los alumnos (Admin y Profesores)
app.MapGet("/api/alumnos", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetAlumnosQuery());
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// ====== ENDPOINTS DE PAQUETES ======

// GET /api/paquetes/estadisticas - Obtener estadísticas de paquetes (Admin)
app.MapGet("/api/paquetes/estadisticas", async (IMediator mediator) =>
{
    var query = new GetEstadisticasPaquetesQuery();
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/paquetes - Listar TODOS los paquetes con filtros (Admin)
app.MapGet("/api/paquetes", async (
    string? busquedaAlumno = null,
    int? estado = null,
    Guid? idTipoPaquete = null,
    DateTime? fechaVencimientoDesde = null,
    DateTime? fechaVencimientoHasta = null,
    int pageNumber = 1,
    int pageSize = 10,
    IMediator mediator = null!) =>
{
    var query = new GetPaquetesQuery(
        BusquedaAlumno: busquedaAlumno,
        Estado: estado,
        IdTipoPaquete: idTipoPaquete,
        FechaVencimientoDesde: fechaVencimientoDesde,
        FechaVencimientoHasta: fechaVencimientoHasta,
        PageNumber: pageNumber,
        PageSize: pageSize
    );

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/mis-paquetes - Obtener paquetes del alumno autenticado (con historial completo)
app.MapGet("/api/mis-paquetes", async (
    int? estado = null,
    Guid? idTipoPaquete = null,
    IMediator mediator = null!,
    ClaimsPrincipal user = null!) =>
{
    var correo = user.FindFirst(ClaimTypes.Email)?.Value
                 ?? user.FindFirst("preferred_username")?.Value 
                 ?? string.Empty;

    var query = new GetMisPaquetesQuery(
        CorreoUsuario: correo,
        Estado: estado,
        IdTipoPaquete: idTipoPaquete
    );

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// POST /api/paquetes - Crear un nuevo paquete (solo Admin)
app.MapPost("/api/paquetes", async (
    CrearPaqueteDTO dto,
    IMediator mediator) =>
{
    var command = new CrearPaqueteCommand(
        IdAlumno: dto.IdAlumno,
        IdTipoPaquete: dto.IdTipoPaquete,
        ClasesDisponibles: dto.ClasesDisponibles,
        ValorPaquete: dto.ValorPaquete,
        DiasVigencia: dto.DiasVigencia,
        IdPago: dto.IdPago
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Created($"/api/paquetes/{result.Value}", new { idPaquete = result.Value }) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/paquetes/{id} - Obtener detalle de un paquete (ApiScope con ownership validation)
app.MapGet("/api/paquetes/{id:guid}", async (
    Guid id,
    IMediator mediator,
    ClaimsPrincipal user) =>
{
    var correo = user.FindFirst(ClaimTypes.Email)?.Value
                 ?? user.FindFirst("preferred_username")?.Value
                 ?? string.Empty;
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var query = new GetPaqueteByIdQuery(
        IdPaquete: id,
        CorreoUsuarioActual: correo,
        EsAdmin: esAdmin
    );

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// PUT /api/paquetes/{id} - Editar un paquete (solo Admin)
app.MapPut("/api/paquetes/{id:guid}", async (
    Guid id,
    EditarPaqueteDTO dto,
    IMediator mediator) =>
{
    if (id != dto.IdPaquete) 
        return Results.BadRequest(new { error = "El ID en la ruta no coincide con el ID del DTO" });

    var command = new EditarPaqueteCommand(
        IdPaquete: dto.IdPaquete,
        ClasesDisponibles: dto.ClasesDisponibles,
        FechaVencimiento: dto.FechaVencimiento
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.NoContent() 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// POST /api/paquetes/{id}/congelar - Congelar un paquete (solo Admin)
app.MapPost("/api/paquetes/{id:guid}/congelar", async (
    Guid id,
    CongelarPaqueteDTO dto,
    IMediator mediator) =>
{
    if (id != dto.IdPaquete) 
        return Results.BadRequest(new { error = "El ID en la ruta no coincide con el ID del DTO" });

    var command = new CongelarPaqueteCommand(
        IdPaquete: dto.IdPaquete,
        FechaInicio: dto.FechaInicio,
        FechaFin: dto.FechaFin,
        Motivo: dto.Motivo
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(new { mensaje = "Paquete congelado exitosamente" }) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// POST /api/paquetes/{id}/descongelar - Descongelar un paquete (solo Admin)
app.MapPost("/api/paquetes/{id:guid}/descongelar", async (
    Guid id,
    [FromQuery] Guid idCongelacion,
    IMediator mediator) =>
{
    var command = new DescongelarPaqueteCommand(
        IdPaquete: id,
        IdCongelacion: idCongelacion
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(new { mensaje = "Paquete descongelado exitosamente" }) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/alumnos/{idAlumno}/paquetes - Listar paquetes de un alumno (ApiScope con ownership)
app.MapGet("/api/alumnos/{idAlumno:guid}/paquetes", async (
    Guid idAlumno,
    bool soloActivos = true,
    int? estado = null,
    Guid? idTipoPaquete = null,
    DateTime? fechaVencimientoDesde = null,
    DateTime? fechaVencimientoHasta = null,
    int pageNumber = 1,
    int pageSize = 10,
    IMediator mediator = null!,
    ClaimsPrincipal user = null!) =>
{
    var correo = user.FindFirst(ClaimTypes.Email)?.Value
                 ?? user.FindFirst("preferred_username")?.Value
                 ?? string.Empty;
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var query = new GetPaquetesDeAlumnoQuery(
        IdAlumno: idAlumno,
        SoloActivos: soloActivos,
        Estado: estado,
        IdTipoPaquete: idTipoPaquete,
        FechaVencimientoDesde: fechaVencimientoDesde,
        FechaVencimientoHasta: fechaVencimientoHasta,
        PageNumber: pageNumber,
        PageSize: pageSize,
        CorreoUsuarioActual: correo,
        EsAdmin: esAdmin
    );

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// ====== ENDPOINTS DE CLASES ======

// POST /api/clases - Crear una nueva clase (Admin o Profesor para sí mismo)
app.MapPost("/api/clases", async (
    CrearClaseDTO dto,
    IMediator mediator,
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    // Extraer información del usuario
    var oidClaim = user.FindFirst("oid")?.Value ?? user.FindFirst("sub")?.Value;
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var command = new CrearClaseCommand(
        IdProfesorPrincipal: dto.IdProfesorPrincipal,
        IdTipoClase: dto.IdTipoClase,
        Fecha: dto.Fecha,
        HoraInicio: dto.HoraInicio,
        HoraFin: dto.HoraFin,
        CupoMaximo: dto.CupoMaximo,
        Observaciones: dto.Observaciones,
        IdUsuarioActual: oidClaim,
        EsAdmin: esAdmin
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Created($"/api/clases/{result.Value}", new { idClase = result.Value }) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// PUT /api/clases/{id} - Editar una clase existente (Admin o Profesor dueño)
app.MapPut("/api/clases/{id:guid}", async (
    Guid id,
    EditarClaseDTO dto,
    IMediator mediator,
    ClaimsPrincipal user) =>
{
    var oidClaim = user.FindFirst("oid")?.Value ?? user.FindFirst("sub")?.Value;
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var command = new EditarClaseCommand(
        IdClase: id,
        IdTipoClase: dto.IdTipoClase,
        IdProfesor: dto.IdProfesor,
        FechaHoraInicio: dto.FechaHoraInicio,
        DuracionMinutos: dto.DuracionMinutos,
        CupoMaximo: dto.CupoMaximo,
        Observaciones: dto.Observaciones,
        IdUsuarioActual: oidClaim,
        EsAdmin: esAdmin
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.NoContent() 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// DELETE /api/clases/{id} - Cancelar una clase (Admin o Profesor dueño)
app.MapDelete("/api/clases/{id:guid}", async (
    Guid id,
    IMediator mediator,
    ClaimsPrincipal user) =>
{
    var oidClaim = user.FindFirst("oid")?.Value ?? user.FindFirst("sub")?.Value;
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var command = new CancelarClaseCommand(
        IdClase: id,
        IdUsuarioActual: oidClaim,
        EsAdmin: esAdmin
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.NoContent() 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// GET /api/clases/{id} - Obtener detalle de una clase (Admin, Profesor dueño o monitor)
app.MapGet("/api/clases/{id:guid}", async (
    Guid id,
    IMediator mediator,
    ClaimsPrincipal user) =>
{
    var oidClaim = user.FindFirst("oid")?.Value ?? user.FindFirst("sub")?.Value;
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var query = new GetClaseByIdQuery(
        IdClase: id,
        IdUsuarioActual: oidClaim,
        EsAdmin: esAdmin
    );

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// GET /api/profesores/{idProfesor}/clases - Listar clases de un profesor (Admin o Profesor dueño)
app.MapGet("/api/profesores/{idProfesor:guid}/clases", async (
    Guid idProfesor,
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    int pageNumber = 1,
    int pageSize = 10,
    IMediator mediator = null!,
    ClaimsPrincipal user = null!) =>
{
    var oidClaim = user.FindFirst("oid")?.Value ?? user.FindFirst("sub")?.Value;
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var query = new GetClasesDeProfesorQuery(
        IdProfesor: idProfesor,
        FechaDesde: fechaDesde,
        FechaHasta: fechaHasta,
        PageNumber: pageNumber,
        PageSize: pageSize,
        IdUsuarioActual: oidClaim,
        EsAdmin: esAdmin
    );

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// ====== ENDPOINTS DE ASISTENCIAS ======

// POST /api/asistencias - Registrar asistencia (protegido)
app.MapPost("/api/asistencias", async (
    Chetango.Application.Asistencias.Commands.RegistrarAsistencia.RegistrarAsistenciaCommand command,
    IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return result.Succeeded ? Results.Created($"/api/asistencias/{result.Value}", result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("AdminOrProfesor");

// PUT /api/asistencias/{id}/estado - Actualizar estado de asistencia (protegido)
app.MapPut("/api/asistencias/{id:guid}/estado", async (
    Guid id,
    Chetango.Application.Asistencias.Commands.ActualizarEstadoAsistencia.ActualizarEstadoAsistenciaCommand command,
    IMediator mediator) =>
{
    if (id != command.IdAsistencia) return Results.BadRequest("ID en ruta no coincide con el comando");
    var result = await mediator.Send(command);
    return result.Succeeded ? Results.NoContent() : Results.BadRequest(result.Error);
}).RequireAuthorization("AdminOrProfesor");

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
    if (roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase)))
    {
        var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase.GetAsistenciasPorClaseQuery(idClase);
        var result = await mediator.Send(query);
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }

    // Profesor: solo puede ver asistencias de SUS clases
    if (roles.Any(r => string.Equals(r, "profesor", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(r, "Profesor", StringComparison.OrdinalIgnoreCase)))
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
    // Autorización: dueño (por correo) o Administrador
    var email = user.FindFirst(ClaimTypes.Email)?.Value
             ?? user.FindFirst("preferred_username")?.Value
             ?? user.FindFirst("upn")?.Value
             ?? user.FindFirst("emails")?.Value;

    var isAdmin = user.FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
               || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var ownerCorreo = await db.Alumnos
        .AsNoTracking()
        .Where(a => a.IdAlumno == idAlumno)
        .Select(a => a.Usuario.Correo)
        .SingleOrDefaultAsync();

    if (ownerCorreo is null) return Results.NotFound();
    if (!isAdmin)
    {
        if (string.IsNullOrWhiteSpace(email)) return Results.Unauthorized();
        if (!string.Equals(ownerCorreo, email, StringComparison.OrdinalIgnoreCase)) return Results.Forbid();
    }

    var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorAlumno.GetAsistenciasPorAlumnoQuery(idAlumno, fechaDesde, fechaHasta);
    var result = await mediator.Send(query);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("ApiScope");

// ====== ENDPOINTS DE PAGOS ======

// GET /api/pagos/metodos-pago - Obtener catálogo de métodos de pago (ApiScope)
app.MapGet("/api/pagos/metodos-pago", async (IMediator mediator) =>
{
    var query = new GetMetodosPagoQuery();
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/pagos/estadisticas - Estadísticas de pagos (AdminOnly)
app.MapGet("/api/pagos/estadisticas", async (
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    IMediator mediator) =>
{
    var query = new GetEstadisticasPagosQuery(fechaDesde, fechaHasta);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/pagos/{id} - Detalle de pago (ApiScope + Ownership)
app.MapGet("/api/pagos/{id:guid}", async (
    Guid id,
    IMediator mediator,
    ClaimsPrincipal user) =>
{
    var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("preferred_username")?.Value;
    if (string.IsNullOrEmpty(emailClaim))
        return Results.Unauthorized();

    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var query = new GetPagoByIdQuery(id, emailClaim, esAdmin);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/mis-pagos - Mis pagos (alumno autenticado) (ApiScope)
app.MapGet("/api/mis-pagos", async (
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    Guid? idMetodoPago,
    int pageNumber = 1,
    int pageSize = 10,
    IMediator mediator = null!,
    ClaimsPrincipal user = null!) =>
{
    var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("preferred_username")?.Value;
    if (string.IsNullOrEmpty(emailClaim))
        return Results.Unauthorized();

    var query = new GetMisPagosQuery(
        emailClaim,
        fechaDesde,
        fechaHasta,
        idMetodoPago,
        pageNumber,
        pageSize
    );

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/alumnos/{idAlumno}/pagos - Pagos de alumno (ApiScope + Ownership)
app.MapGet("/api/alumnos/{idAlumno:guid}/pagos", async (
    Guid idAlumno,
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    Guid? idMetodoPago,
    int pageNumber = 1,
    int pageSize = 10,
    IMediator mediator = null!,
    ClaimsPrincipal user = null!) =>
{
    var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("preferred_username")?.Value;
    if (string.IsNullOrEmpty(emailClaim))
        return Results.Unauthorized();

    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    var query = new GetPagosDeAlumnoQuery(
        idAlumno,
        emailClaim,
        esAdmin,
        fechaDesde,
        fechaHasta,
        idMetodoPago,
        pageNumber,
        pageSize
    );

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// POST /api/pagos - Registrar pago (AdminOnly)
app.MapPost("/api/pagos", async (
    RegistrarPagoDTO dto,
    IMediator mediator) =>
{
    var command = new RegistrarPagoCommand(
        dto.IdAlumno,
        dto.FechaPago,
        dto.MontoTotal,
        dto.IdMetodoPago,
        dto.Nota,
        dto.Paquetes
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// PUT /api/pagos/{id} - Editar pago (AdminOnly)
app.MapPut("/api/pagos/{id:guid}", async (
    Guid id,
    EditarPagoDTO dto,
    IMediator mediator) =>
{
    var command = new EditarPagoCommand(
        id,
        dto.MontoTotal,
        dto.IdMetodoPago,
        dto.Nota
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.NoContent() 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

app.Run();
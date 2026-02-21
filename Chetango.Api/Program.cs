using Microsoft.EntityFrameworkCore;
using Chetango.Infrastructure.Persistence;
using Chetango.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Chetango.Application.Clases.Queries.GetClasesDeAlumno;
using Chetango.Application.Clases.Commands;
using Chetango.Application.Clases.Commands.CrearClase;
using Chetango.Application.Clases.Commands.EditarClase;
using Chetango.Application.Clases.Commands.CancelarClase;
using Chetango.Application.Clases.Queries.GetClaseById;
using Chetango.Application.Clases.Queries.GetClasesDeProfesor;
using Chetango.Application.Clases.Queries.GetTiposClase;
using Chetango.Application.Clases.Queries.GetProfesores;
using Chetango.Application.Clases.Queries.GetAlumnos;
using Chetango.Application.Clases.DTOs;
using Chetango.Application.Alumnos;
using Chetango.Application.Profesores;
using Chetango.Application.Paquetes.Commands.CrearPaquete;
using Chetango.Application.Paquetes.Commands.CrearTipoPaquete;
using Chetango.Application.Paquetes.Commands.ActualizarTipoPaquete;
using Chetango.Application.Paquetes.Commands.ToggleTipoPaqueteActivo;
using Chetango.Application.Paquetes.Commands.EditarPaquete;
using Chetango.Application.Paquetes.Commands.CongelarPaquete;
using Chetango.Application.Paquetes.Commands.DescongelarPaquete;
using Chetango.Application.Paquetes.Queries.GetPaqueteById;
using Chetango.Application.Paquetes.Queries.GetPaquetesDeAlumno;
using Chetango.Application.Paquetes.Queries.GetPaquetes;
using Chetango.Application.Paquetes.Queries.GetEstadisticasPaquetes;
using Chetango.Application.Paquetes.Queries.GetTiposPaquete;
using Chetango.Application.Paquetes.Queries.GetMisPaquetes;
using Chetango.Application.Paquetes.Queries.GetPaquetesSinPago;
using Chetango.Application.Paquetes.DTOs;
using Chetango.Application.Asistencias.Commands.RegistrarAsistencia;
using Chetango.Application.Asistencias.Commands.ActualizarEstadoAsistencia;
using Chetango.Application.Asistencias.Commands.ConfirmarAsistencia;
using Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase;
using Chetango.Application.Asistencias.Queries.GetAsistenciasClaseConAlumnos;
using Chetango.Application.Asistencias.Queries.GetAsistenciasPorAlumno;
using Chetango.Application.Asistencias.Queries.GetAsistenciasPendientesConfirmar;
using Chetango.Application.Asistencias.Queries.GetTiposAsistencia;
using Chetango.Application.Asistencias.Admin.DTOs;
using Chetango.Application.Asistencias.Admin.Queries.GetDiasConClasesAdmin;
using Chetango.Application.Asistencias.Admin.Queries.GetClasesDelDiaAdmin;
using Chetango.Application.Asistencias.Admin.Queries.GetResumenAsistenciasClaseAdmin;
using Chetango.Application.Pagos.Commands;
using Chetango.Application.Pagos.Queries;
using Chetango.Application.Pagos.DTOs;
using Chetango.Application.Reportes.Queries;
using Chetango.Application.Reportes.DTOs;
using Chetango.Application.Reportes.Services;
using Chetango.Application.Eventos.Commands;
using Chetango.Application.Eventos.Queries;
using Chetango.Application.Eventos.DTOs;
using Chetango.Application.Perfil.Commands;
using Chetango.Application.Perfil.Queries;
using Chetango.Application.Perfil.DTOs;
using Chetango.Application.Profesores.Commands;
using Chetango.Application.Profesores.Queries;
using Chetango.Application.Profesores.DTOs;
using Chetango.Application.Usuarios.Queries;
using Chetango.Application.Usuarios.Commands;
using Chetango.Application.Usuarios.DTOs;
using Chetango.Application.Nomina.Queries;
using Chetango.Application.Nomina.Commands;
using Chetango.Application.Nomina.DTOs;
using Chetango.Application.Solicitudes.Commands.SolicitarRenovacionPaquete;
using Chetango.Application.Solicitudes.Commands.SolicitarClasePrivada;
using Chetango.Application.Solicitudes.Queries;
using Chetango.Application.Solicitudes.DTOs;
using Chetango.Application.Referidos.Commands;
using Chetango.Application.Referidos.Queries;
using Chetango.Application.Referidos.DTOs;
using Chetango.Application.Admin.Queries;
using Chetango.Application.Admin.Commands;
using Chetango.Application.Admin.DTOs;
using Chetango.Domain.Entities; // Added for Usuario
using Chetango.Domain.Entities.Estados; // Added for TipoDocumento
using Chetango.Application.Common; // registrar IAppDbContext
using Chetango.Api.Infrastructure; // MigrationRunner
using Microsoft.OpenApi.Models; // Swagger security
using Microsoft.AspNetCore.Http;
using System.Text.Json; // For JSON serializer configuration

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization options to be case-insensitive
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Configuración de EF Core: registra el DbContext con la cadena de conexión de SQL Server
builder.Services.AddDbContext<ChetangoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChetangoConnection")));
// Mapear interfaz de Application al DbContext concreto
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<ChetangoDbContext>());

// Registrar servicio de WhatsApp
builder.Services.AddScoped<IWhatsAppService, TwilioWhatsAppService>();

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

// Registro de servicios de exportación de reportes
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddScoped<PdfExportService>();
builder.Services.AddScoped<CsvExportService>();

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
// CORS debe ir ANTES de otros middlewares
app.UseCors("DefaultCors");

// Solo redirigir a HTTPS en producción (no en QA/Development local)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Servir archivos estáticos (imágenes, comprobantes, avatares, etc.)
app.UseStaticFiles();

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

    // Obtener idProfesor e idAlumno si existen
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
    
    email = string.IsNullOrWhiteSpace(email) ? email : email.Trim();

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

// POST /api/tipos-paquete - Crear un nuevo tipo de paquete (solo Admin)
app.MapPost("/api/tipos-paquete", async (CrearTipoPaqueteCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Created($"/api/tipos-paquete/{result.Value}", new { id = result.Value })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// PUT /api/tipos-paquete/{id} - Actualizar un tipo de paquete (solo Admin)
app.MapPut("/api/tipos-paquete/{id:guid}", async (Guid id, ActualizarTipoPaqueteCommand command, IMediator mediator) =>
{
    // Crear comando con el ID de la URL si no coincide
    var commandToSend = command.IdTipoPaquete == Guid.Empty || command.IdTipoPaquete != id
        ? command with { IdTipoPaquete = id }
        : command;
    
    var result = await mediator.Send(commandToSend);
    return result.Succeeded 
        ? Results.Ok()
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// PATCH /api/tipos-paquete/{id}/toggle-active - Activar/Desactivar tipo de paquete (solo Admin)
app.MapPatch("/api/tipos-paquete/{id:guid}/toggle-active", async (Guid id, IMediator mediator) =>
{
    var result = await mediator.Send(new ToggleTipoPaqueteActivoCommand(id));
    return result.Succeeded 
        ? Results.Ok(new { activo = result.Value })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/profesores - Obtener todos los profesores (solo Admin)
app.MapGet("/api/profesores", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetProfesoresQuery());
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/profesores/{idProfesor}/quick-view - Obtener información rápida de profesor (Admin)
app.MapGet("/api/profesores/{idProfesor:guid}/quick-view", async (Guid idProfesor, IMediator mediator) =>
{
    var result = await mediator.Send(new GetProfesorQuickViewQuery(idProfesor));
    return result.Succeeded ? Results.Ok(result.Value) : Results.NotFound(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/profesores/{idProfesor}/detail - Obtener detalle completo de profesor (Admin)
app.MapGet("/api/profesores/{idProfesor:guid}/detail", async (Guid idProfesor, IMediator mediator) =>
{
    var result = await mediator.Send(new GetProfesorDetailQuery(idProfesor));
    return result.Succeeded ? Results.Ok(result.Value) : Results.NotFound(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/alumnos - Obtener todos los alumnos (Admin y Profesores)
app.MapGet("/api/alumnos", async (IMediator mediator) =>
{
    var result = await mediator.Send(new GetAlumnosQuery());
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// GET /api/alumnos/{idAlumno}/quick-view - Obtener información rápida de alumno (Admin)
app.MapGet("/api/alumnos/{idAlumno:guid}/quick-view", async (Guid idAlumno, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAlumnoQuickViewQuery(idAlumno));
    return result.Succeeded ? Results.Ok(result.Value) : Results.NotFound(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/alumnos/{idAlumno}/detail - Obtener detalle completo de alumno (Admin)
app.MapGet("/api/alumnos/{idAlumno:guid}/detail", async (Guid idAlumno, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAlumnoDetailQuery(idAlumno));
    return result.Succeeded ? Results.Ok(result.Value) : Results.NotFound(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

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

// GET /api/alumnos/{idAlumno}/paquetes-sin-pago - Obtener paquetes del alumno que no tienen pago asociado (AdminOnly)
app.MapGet("/api/alumnos/{idAlumno:guid}/paquetes-sin-pago", async (
    Guid idAlumno,
    IMediator mediator) =>
{
    var paquetes = await mediator.Send(new GetPaquetesSinPagoQuery(idAlumno));
    return paquetes.Succeeded 
        ? Results.Ok(paquetes.Value) 
        : Results.BadRequest(new { error = paquetes.Error });
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

    // Convertir ProfesorClaseRequestDTO a ProfesorClaseRequest (del command)
    var profesoresCommand = dto.Profesores?.Select(p => new ProfesorClaseRequest(p.IdProfesor, p.RolEnClase)).ToList();

    var command = new CrearClaseCommand(
        Profesores: profesoresCommand ?? new List<ProfesorClaseRequest>(),
        IdTipoClase: dto.IdTipoClase,
        Fecha: dto.Fecha,
        HoraInicio: dto.HoraInicio,
        HoraFin: dto.HoraFin,
        CupoMaximo: dto.CupoMaximo,
        Observaciones: dto.Observaciones,
        IdProfesorPrincipal: dto.IdProfesorPrincipal,
        IdsMonitores: dto.IdsMonitores,
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

// POST /api/clases/{id}/completar - Completar clase y generar pagos (Solo Admin)
app.MapPost("/api/clases/{id:guid}/completar", async (
    Guid id,
    IMediator mediator) =>
{
    var command = new CompletarClaseCommand(IdClase: id);
    var result = await mediator.Send(command);
    
    return result.Succeeded 
        ? Results.Ok(new { mensaje = "Clase completada y pagos generados exitosamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

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
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    // Obtener roles
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    // Obtener IdUsuario del profesor por correo (como en otros endpoints)
    string? idUsuarioActual = null;
    if (!esAdmin)
    {
        var email = user.FindFirst(ClaimTypes.Email)?.Value
                 ?? user.FindFirst("preferred_username")?.Value
                 ?? user.FindFirst("upn")?.Value
                 ?? user.FindFirst("emails")?.Value;

        if (!string.IsNullOrWhiteSpace(email))
        {
            var profesor = await db.Profesores
                .AsNoTracking()
                .Where(p => p.Usuario.Correo == email)
                .Select(p => p.IdUsuario)
                .FirstOrDefaultAsync();

            idUsuarioActual = profesor.ToString();
        }
    }

    var query = new GetClaseByIdQuery(
        IdClase: id,
        IdUsuarioActual: idUsuarioActual,
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
    ClaimsPrincipal user = null!,
    ChetangoDbContext db = null!) =>
{
    // Validar que el profesor existe
    var profesor = await db.Profesores
        .Include(p => p.Usuario)
        .FirstOrDefaultAsync(p => p.IdProfesor == idProfesor);
    
    if (profesor is null)
        return Results.NotFound(new { error = "Profesor no encontrado" });

    // Validar ownership por correo (igual que el dashboard)
    var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value
        ?? user.FindFirst("preferred_username")?.Value
        ?? user.FindFirst("upn")?.Value;
    
    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    var esAdmin = roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

    // Si no es admin, verificar que el correo coincida con el del profesor
    if (!esAdmin)
    {
        if (string.IsNullOrWhiteSpace(emailClaim) || 
            !string.Equals(profesor.Usuario.Correo, emailClaim, StringComparison.OrdinalIgnoreCase))
            return Results.Forbid();
    }

    var query = new GetClasesDeProfesorQuery(
        IdProfesor: idProfesor,
        FechaDesde: fechaDesde,
        FechaHasta: fechaHasta,
        PageNumber: pageNumber,
        PageSize: pageSize,
        IdUsuarioActual: null,
        EsAdmin: esAdmin
    );

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// ====== ENDPOINTS DE ASISTENCIAS ======

// GET /api/asistencias/tipos - Obtener catálogo de tipos de asistencia (protegido)
app.MapGet("/api/asistencias/tipos", async (IMediator mediator) =>
{
    var query = new GetTiposAsistenciaQuery();
    var result = await mediator.Send(query);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("AdminOrProfesor");

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
        var query = new GetAsistenciasClaseConAlumnosQuery(idClase);
        var result = await mediator.Send(query);
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }

    // Profesor: solo puede ver asistencias de SUS clases
    if (roles.Any(r => string.Equals(r, "profesor", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(r, "Profesor", StringComparison.OrdinalIgnoreCase)))
    {
        if (string.IsNullOrWhiteSpace(email))
            return Results.Unauthorized();

        // Verificar que el profesor está asignado a la clase (Principal o Monitor)
        var esProfesorDeClase = await db.ClasesProfesores
            .AsNoTracking()
            .Include(cp => cp.Profesor)
            .ThenInclude(p => p.Usuario)
            .Where(cp => cp.IdClase == idClase && cp.Profesor.Usuario.Correo == email)
            .AnyAsync();

        if (!esProfesorDeClase)
            return Results.Forbid(); // 403: El profesor no está asignado a esta clase

        var query = new GetAsistenciasClaseConAlumnosQuery(idClase);
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

// GET /api/alumnos/me/asistencias/pendientes - Obtener asistencias pendientes de confirmar (ApiScope, alumno autenticado)
app.MapGet("/api/alumnos/me/asistencias/pendientes", async (
    IMediator mediator,
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    // Obtener email del usuario autenticado
    var email = user.FindFirst(ClaimTypes.Email)?.Value
             ?? user.FindFirst("preferred_username")?.Value
             ?? user.FindFirst("upn")?.Value
             ?? user.FindFirst("emails")?.Value;

    if (string.IsNullOrWhiteSpace(email))
        return Results.Unauthorized();

    // Obtener IdAlumno por correo
    var alumno = await db.Alumnos
        .AsNoTracking()
        .Where(a => a.Usuario.Correo == email)
        .Select(a => new { a.IdAlumno })
        .FirstOrDefaultAsync();

    if (alumno == null)
        return Results.NotFound("Alumno no encontrado");

    var query = new GetAsistenciasPendientesConfirmarQuery(alumno.IdAlumno);
    var result = await mediator.Send(query);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
}).RequireAuthorization("ApiScope");

// POST /api/alumnos/asistencias/{idAsistencia}/confirmar - Confirmar asistencia (ApiScope, alumno owner)
app.MapPost("/api/alumnos/asistencias/{idAsistencia:guid}/confirmar", async (
    Guid idAsistencia,
    IMediator mediator,
    ClaimsPrincipal user,
    ChetangoDbContext db) =>
{
    // Obtener email del usuario autenticado
    var email = user.FindFirst(ClaimTypes.Email)?.Value
             ?? user.FindFirst("preferred_username")?.Value
             ?? user.FindFirst("upn")?.Value
             ?? user.FindFirst("emails")?.Value;

    if (string.IsNullOrWhiteSpace(email))
        return Results.Unauthorized();

    // Verificar que la asistencia pertenece al alumno autenticado
    var asistencia = await db.Asistencias
        .AsNoTracking()
        .Where(a => a.IdAsistencia == idAsistencia)
        .Select(a => new { a.Alumno.Usuario.Correo })
        .FirstOrDefaultAsync();

    if (asistencia == null)
        return Results.NotFound("Asistencia no encontrada");

    if (!string.Equals(asistencia.Correo, email, StringComparison.OrdinalIgnoreCase))
        return Results.Forbid();

    var command = new ConfirmarAsistenciaCommand(idAsistencia);
    var result = await mediator.Send(command);
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

// GET /api/pagos/pendientes - Pagos pendientes de verificación (AdminOnly)
app.MapGet("/api/pagos/pendientes", async (
    int pageNumber = 1,
    int pageSize = 50,
    IMediator mediator = null!) =>
{
    var query = new GetPagosPorEstadoQuery("Pendiente Verificación", null, null, pageNumber, pageSize);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value.Items) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/pagos/verificados-hoy - Pagos verificados hoy (AdminOnly)
app.MapGet("/api/pagos/verificados-hoy", async (
    int pageNumber = 1,
    int pageSize = 50,
    IMediator mediator = null!) =>
{
    var hoy = DateTime.Today;
    var query = new GetPagosPorEstadoQuery("Verificado", hoy, hoy.AddDays(1).AddTicks(-1), pageNumber, pageSize);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value.Items) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/pagos/verificados - Todos los pagos verificados (AdminOnly)
app.MapGet("/api/pagos/verificados", async (
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    int pageNumber = 1,
    int pageSize = 50,
    IMediator mediator = null!) =>
{
    var query = new GetPagosPorEstadoQuery("Verificado", fechaDesde, fechaHasta, pageNumber, pageSize);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value.Items) 
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
    ClaimsPrincipal user,
    IMediator mediator) =>
{
    var email = user.FindFirst(ClaimTypes.Email)?.Value 
             ?? user.FindFirst("preferred_username")?.Value 
             ?? user.FindFirst("email")?.Value;

    var command = new RegistrarPagoCommand(
        dto.IdAlumno,
        dto.FechaPago,
        dto.MontoTotal,
        dto.IdMetodoPago,
        dto.ReferenciaTransferencia,
        dto.UrlComprobante,
        dto.Nota,
        dto.Paquetes,
        dto.IdsPaquetesExistentes,
        null, // Sede - se heredará del usuario
        email // EmailUsuarioCreador
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

// POST /api/pagos/{id}/verificar - Verificar o rechazar pago (AdminOnly)
app.MapPost("/api/pagos/{id:guid}/verificar", async (
    Guid id,
    VerificarPagoRequestDTO dto,
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value
        ?? httpContext.User.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var aprobar = dto.Accion.ToLower() == "aprobar";
    
    var command = new VerificarPagoCommand(
        id,
        aprobar,
        dto.Nota,
        dto.NotificarAlumno,
        emailClaim
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(new { success = true }) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// DELETE /api/pagos/{id} - Eliminar pago (soft delete) (AdminOnly)
app.MapDelete("/api/pagos/{id:guid}", async (
    Guid id,
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value
        ?? httpContext.User.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var command = new EliminarPagoCommand(id, emailClaim);

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.NoContent() 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// ═══════════════════════════════════════════════════════════════════════════════════
// ENDPOINTS DE REPORTES - Módulo de solo lectura (queries únicamente)
// ═══════════════════════════════════════════════════════════════════════════════════

// GET /api/reportes/asistencias - Reporte de asistencias (AdminOrProfesor)
app.MapGet("/api/reportes/asistencias", async (
    HttpContext httpContext,
    IMediator mediator,
    DateTime fechaDesde,
    DateTime fechaHasta,
    Guid? idClase = null,
    Guid? idAlumno = null,
    Guid? idProfesor = null,
    string? estadoAsistencia = null) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value
        ?? httpContext.User.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var esAdmin = httpContext.User.HasClaim(ClaimTypes.Role, "admin");
    var esProfesor = httpContext.User.HasClaim(ClaimTypes.Role, "profesor");

    var query = new GetReporteAsistenciasQuery
    {
        FechaDesde = fechaDesde,
        FechaHasta = fechaHasta,
        IdClase = idClase,
        IdAlumno = idAlumno,
        IdProfesor = idProfesor,
        EstadoAsistencia = estadoAsistencia,
        EmailUsuario = emailClaim,
        EsAdmin = esAdmin,
        EsProfesor = esProfesor
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// GET /api/reportes/ingresos - Reporte de ingresos (AdminOnly)
app.MapGet("/api/reportes/ingresos", async (
    IMediator mediator,
    DateTime fechaDesde,
    DateTime fechaHasta,
    Guid? idMetodoPago = null,
    bool comparativa = false) =>
{
    var query = new GetReporteIngresosQuery
    {
        FechaDesde = fechaDesde,
        FechaHasta = fechaHasta,
        IdMetodoPago = idMetodoPago,
        ComparativaMesAnterior = comparativa
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/reportes/paquetes - Reporte de paquetes (AdminOnly)
app.MapGet("/api/reportes/paquetes", async (
    IMediator mediator,
    DateTime fechaDesde,
    DateTime fechaHasta,
    string? estado = null,
    Guid? idTipoPaquete = null) =>
{
    var query = new GetReportePaquetesQuery
    {
        FechaDesde = fechaDesde,
        FechaHasta = fechaHasta,
        Estado = estado,
        IdTipoPaquete = idTipoPaquete
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/reportes/clases - Reporte de clases (AdminOrProfesor)
app.MapGet("/api/reportes/clases", async (
    HttpContext httpContext,
    IMediator mediator,
    DateTime fechaDesde,
    DateTime fechaHasta,
    Guid? idTipoClase = null,
    Guid? idProfesor = null) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value
        ?? httpContext.User.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var esAdmin = httpContext.User.HasClaim(ClaimTypes.Role, "admin");
    var esProfesor = httpContext.User.HasClaim(ClaimTypes.Role, "profesor");

    var query = new GetReporteClasesQuery
    {
        FechaDesde = fechaDesde,
        FechaHasta = fechaHasta,
        IdTipoClase = idTipoClase,
        IdProfesor = idProfesor,
        EmailUsuario = emailClaim,
        EsAdmin = esAdmin,
        EsProfesor = esProfesor
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// GET /api/reportes/alumnos - Reporte de alumnos (AdminOnly)
app.MapGet("/api/reportes/alumnos", async (
    IMediator mediator,
    DateTime? fechaInscripcionDesde = null,
    DateTime? fechaInscripcionHasta = null,
    string? estado = null) =>
{
    var query = new GetReporteAlumnosQuery
    {
        FechaInscripcionDesde = fechaInscripcionDesde,
        FechaInscripcionHasta = fechaInscripcionHasta,
        Estado = estado
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/reportes/dashboard - Dashboard general (AdminOnly)
app.MapGet("/api/reportes/dashboard", async (
    IMediator mediator,
    DateTime? fechaDesde = null,
    DateTime? fechaHasta = null,
    string? periodo = null) =>
{
    var query = new GetDashboardQuery
    {
        FechaDesde = fechaDesde,
        FechaHasta = fechaHasta,
        PeriodoPreset = periodo
    };
    
    var result = await mediator.Send(query);
    
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/reportes/mi-reporte - Mi reporte (alumno autenticado) (ApiScope)
app.MapGet("/api/reportes/mi-reporte", async (
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value
        ?? httpContext.User.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var query = new GetMiReporteQuery
    {
        EmailUsuario = emailClaim
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/reportes/mis-clases - Mis clases reporte (profesor autenticado) (ApiScope)
app.MapGet("/api/reportes/mis-clases", async (
    HttpContext httpContext,
    IMediator mediator,
    DateTime fechaDesde,
    DateTime fechaHasta) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value
        ?? httpContext.User.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var query = new GetMisClasesReporteQuery
    {
        FechaDesde = fechaDesde,
        FechaHasta = fechaHasta,
        EmailUsuario = emailClaim
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/reportes/dashboard/profesor - Dashboard profesor (ApiScope)
app.MapGet("/api/reportes/dashboard/profesor", async (
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value
        ?? httpContext.User.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var query = new GetDashboardProfesorQuery
    {
        EmailUsuario = emailClaim
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/reportes/dashboard/alumno - Dashboard alumno (ApiScope)
app.MapGet("/api/reportes/dashboard/alumno", async (
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value
        ?? httpContext.User.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var query = new GetDashboardAlumnoQuery
    {
        EmailUsuario = emailClaim
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// ==========================================
// ENDPOINTS DE PERFIL PROFESOR
// ==========================================

// GET /api/profesores/me/perfil - Obtener perfil del profesor autenticado (ApiScope)
app.MapGet("/api/profesores/me/perfil", async (
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var query = new GetPerfilProfesorQuery(emailClaim);
    var result = await mediator.Send(query);
    
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// PUT /api/profesores/me/perfil - Actualizar datos personales del profesor (ApiScope)
app.MapPut("/api/profesores/me/perfil", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db,
    [FromBody] UpdateDatosPersonalesProfesorDTO dto) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .FirstOrDefaultAsync(u => u.Correo == emailClaim);

    if (usuario == null)
        return Results.NotFound(new { error = "Usuario no encontrado" });

    var profesor = await db.Profesores
        .FirstOrDefaultAsync(p => p.IdUsuario == usuario.IdUsuario);

    if (profesor == null)
        return Results.NotFound(new { error = "Perfil de profesor no encontrado" });

    var command = new UpdateDatosPersonalesProfesorCommand(
        profesor.IdProfesor,
        dto.NombreCompleto,
        dto.Telefono
    );
    
    var result = await mediator.Send(command);
    
    return result.Succeeded
        ? Results.Ok(new { message = "Datos personales actualizados correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// PUT /api/profesores/me/perfil-profesional - Actualizar perfil profesional (AdminOrProfesor)
app.MapPut("/api/profesores/me/perfil-profesional", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db,
    [FromBody] UpdatePerfilProfesionalDTO dto) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .FirstOrDefaultAsync(u => u.Correo == emailClaim);

    if (usuario == null)
        return Results.NotFound(new { error = "Usuario no encontrado" });

    var profesor = await db.Profesores
        .FirstOrDefaultAsync(p => p.IdUsuario == usuario.IdUsuario);

    if (profesor == null)
        return Results.NotFound(new { error = "Perfil de profesor no encontrado" });

    var command = new UpdatePerfilProfesionalCommand(
        profesor.IdProfesor,
        dto.Biografia,
        dto.Especialidades
    );
    
    var result = await mediator.Send(command);
    
    return result.Succeeded
        ? Results.Ok(new { message = "Perfil profesional actualizado correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// PUT /api/profesores/me/configuracion - Actualizar configuración de notificaciones (AdminOrProfesor)
app.MapPut("/api/profesores/me/configuracion", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db,
    [FromBody] UpdateConfiguracionProfesorDTO dto) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .FirstOrDefaultAsync(u => u.Correo == emailClaim);

    if (usuario == null)
        return Results.NotFound(new { error = "Usuario no encontrado" });

    var profesor = await db.Profesores
        .FirstOrDefaultAsync(p => p.IdUsuario == usuario.IdUsuario);

    if (profesor == null)
        return Results.NotFound(new { error = "Perfil de profesor no encontrado" });

    var command = new UpdateConfiguracionProfesorCommand(
        profesor.IdProfesor,
        dto.NotificacionesEmail,
        dto.RecordatoriosClase,
        dto.AlertasCambios
    );
    
    var result = await mediator.Send(command);
    
    return result.Succeeded
        ? Results.Ok(new { message = "Configuración actualizada correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOrProfesor");

// ==========================================
// ENDPOINTS DE EVENTOS
// ==========================================

// GET /api/eventos - Listar todos los eventos (ApiScope)
app.MapGet("/api/eventos", async (
    IMediator mediator,
    bool? soloActivos,
    bool? soloFuturos) =>
{
    var query = new GetAllEventosQuery
    {
        SoloActivos = soloActivos,
        SoloFuturos = soloFuturos
    };

    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/eventos/{id} - Obtener evento por ID (ApiScope)
app.MapGet("/api/eventos/{id:guid}", async (
    Guid id,
    IMediator mediator) =>
{
    var query = new GetEventoByIdQuery { IdEvento = id };
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.NotFound(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// POST /api/eventos - Crear nuevo evento (Admin)
app.MapPost("/api/eventos", async (
    CreateEventoDto dto,
    HttpContext httpContext,
    IMediator mediator,
    ChetangoDbContext db) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    // Obtener ID del usuario
    var usuario = await db.Usuarios
        .FirstOrDefaultAsync(u => u.Correo == emailClaim);

    if (usuario == null)
        return Results.Unauthorized();

    var command = new CreateEventoCommand
    {
        Titulo = dto.Titulo,
        Descripcion = dto.Descripcion,
        Fecha = dto.Fecha,
        Hora = dto.Hora,
        Precio = dto.Precio,
        Destacado = dto.Destacado,
        ImagenUrl = dto.ImagenUrl,
        IdUsuarioCreador = usuario.IdUsuario
    };

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Created($"/api/eventos/{result.Value!.IdEvento}", result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("Admin");

// PUT /api/eventos/{id} - Actualizar evento (Admin)
app.MapPut("/api/eventos/{id:guid}", async (
    Guid id,
    UpdateEventoDto dto,
    IMediator mediator) =>
{
    var command = new UpdateEventoCommand
    {
        IdEvento = id,
        Titulo = dto.Titulo,
        Descripcion = dto.Descripcion,
        Fecha = dto.Fecha,
        Hora = dto.Hora,
        Precio = dto.Precio,
        Destacado = dto.Destacado,
        ImagenUrl = dto.ImagenUrl,
        Activo = dto.Activo
    };

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("Admin");

// DELETE /api/eventos/{id} - Eliminar evento (Admin)
app.MapDelete("/api/eventos/{id:guid}", async (
    Guid id,
    IMediator mediator) =>
{
    var command = new DeleteEventoCommand { IdEvento = id };
    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(new { message = "Evento eliminado correctamente" }) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("Admin");

// POST /api/eventos/upload-imagen - Subir imagen para evento (Admin)
app.MapPost("/api/eventos/upload-imagen", async (
    HttpRequest request,
    IWebHostEnvironment env) =>
{
    if (!request.HasFormContentType || !request.Form.Files.Any())
        return Results.BadRequest(new { error = "No se ha enviado ningún archivo" });

    var file = request.Form.Files[0];

    // Validar extensión
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!allowedExtensions.Contains(extension))
        return Results.BadRequest(new { error = "Formato de imagen no válido. Use jpg, jpeg, png o webp" });

    // Validar tamaño (máximo 5MB)
    if (file.Length > 5 * 1024 * 1024)
        return Results.BadRequest(new { error = "La imagen no puede superar 5MB" });

    // Crear directorio si no existe
    var uploadsPath = Path.Combine(env.WebRootPath, "uploads", "eventos");
    Directory.CreateDirectory(uploadsPath);

    // Generar nombre único
    var fileName = $"{Guid.NewGuid()}{extension}";
    var filePath = Path.Combine(uploadsPath, fileName);

    // Guardar archivo
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    // Retornar URL relativa
    var imageUrl = $"/uploads/eventos/{fileName}";
    return Results.Ok(new { url = imageUrl, nombre = fileName });
}).RequireAuthorization("Admin");

// ═══════════════════════════════════════════════════════════════════════════════════
// ENDPOINTS DE SOLICITUDES - Renovación Paquetes y Clases Privadas
// ═══════════════════════════════════════════════════════════════════════════════════

// POST /api/solicitudes/renovar-paquete - Alumno solicita renovación (ApiScope)
app.MapPost("/api/solicitudes/renovar-paquete", async (
    HttpContext httpContext,
    SolicitarRenovacionPaqueteCommand command,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    // Actualizar el command con el email del usuario autenticado
    var commandWithEmail = new SolicitarRenovacionPaqueteCommand(
        emailClaim,
        command.IdTipoPaqueteDeseado,
        command.MensajeAlumno
    );

    var result = await mediator.Send(commandWithEmail);
    return result.Succeeded 
        ? Results.Created($"/api/solicitudes/renovacion/{result.Value}", new { idSolicitud = result.Value }) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// POST /api/solicitudes/clase-privada - Alumno solicita clase privada (ApiScope)
app.MapPost("/api/solicitudes/clase-privada", async (
    HttpContext httpContext,
    SolicitarClasePrivadaCommand command,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    // Actualizar el command con el email del usuario autenticado
    var commandWithEmail = new SolicitarClasePrivadaCommand(
        emailClaim,
        command.IdTipoClaseDeseado,
        command.FechaPreferida,
        command.HoraPreferida,
        command.ObservacionesAlumno
    );

    var result = await mediator.Send(commandWithEmail);
    return result.Succeeded 
        ? Results.Created($"/api/solicitudes/clase-privada/{result.Value}", new { idSolicitud = result.Value }) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/solicitudes/renovacion-paquete/pendientes - Admin ve solicitudes pendientes (AdminOnly)
app.MapGet("/api/solicitudes/renovacion-paquete/pendientes", async (
    IMediator mediator) =>
{
    var query = new GetSolicitudesRenovacionPendientesQuery();
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/solicitudes/clase-privada/pendientes - Admin ve solicitudes pendientes (AdminOnly)
app.MapGet("/api/solicitudes/clase-privada/pendientes", async (
    IMediator mediator) =>
{
    var query = new GetSolicitudesClasePrivadaPendientesQuery();
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// ═══════════════════════════════════════════════════════════════════════════════════
// ENDPOINTS DE REFERIDOS - Programa "Invita un Amigo"
// ═══════════════════════════════════════════════════════════════════════════════════

// GET /api/referidos/mi-codigo - Obtener código de referido del alumno (ApiScope)
app.MapGet("/api/referidos/mi-codigo", async (
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var query = new GetMiCodigoReferidoQuery(emailClaim);
    var result = await mediator.Send(query);
    
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// POST /api/referidos/generar-codigo - Generar código de referido (ApiScope)
app.MapPost("/api/referidos/generar-codigo", async (
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var command = new GenerarCodigoReferidoCommand(emailClaim);
    var result = await mediator.Send(command);
    
    return result.Succeeded 
        ? Results.Created("/api/referidos/mi-codigo", result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// POST /api/pagos/upload-comprobante - Subir comprobante de pago (Admin)
app.MapPost("/api/pagos/upload-comprobante", async (
    HttpRequest request,
    IWebHostEnvironment env) =>
{
    if (!request.HasFormContentType || !request.Form.Files.Any())
        return Results.BadRequest(new { error = "No se ha enviado ningún archivo" });

    var file = request.Form.Files[0];

    // Validar extensión
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!allowedExtensions.Contains(extension))
        return Results.BadRequest(new { error = "Formato no válido. Use jpg, jpeg, png, webp o pdf" });

    // Validar tamaño (máximo 10MB)
    if (file.Length > 10 * 1024 * 1024)
        return Results.BadRequest(new { error = "El archivo no puede superar 10MB" });

    // Crear directorio si no existe
    var uploadsPath = Path.Combine(env.WebRootPath, "uploads", "comprobantes");
    Directory.CreateDirectory(uploadsPath);

    // Generar nombre único
    var fileName = $"{Guid.NewGuid()}{extension}";
    var filePath = Path.Combine(uploadsPath, fileName);

    // Guardar archivo
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    // Retornar URL relativa
    var comprobanteUrl = $"/uploads/comprobantes/{fileName}";
    return Results.Ok(new { url = comprobanteUrl, nombre = fileName });
}).RequireAuthorization("AdminOnly");

// ============================================
// PERFIL ALUMNO ENDPOINTS
// ============================================

// GET /api/alumnos/me/perfil - Obtener perfil del alumno autenticado (ApiScope)
app.MapGet("/api/alumnos/me/perfil", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db) =>
{
    var userEmail = httpContext.User.FindFirst("preferred_username")?.Value
                    ?? httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

    if (string.IsNullOrEmpty(userEmail))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .Include(u => u.Alumnos)
        .FirstOrDefaultAsync(u => u.Correo == userEmail);

    if (usuario?.Alumnos.FirstOrDefault() == null)
        return Results.NotFound(new { error = "Perfil de alumno no encontrado" });

    var idAlumno = usuario.Alumnos.First().IdAlumno;
    var result = await mediator.Send(new GetPerfilAlumnoQuery(idAlumno));

    return result.Succeeded
        ? Results.Ok(result.Value)
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// PUT /api/alumnos/me/perfil - Actualizar datos personales (ApiScope)
app.MapPut("/api/alumnos/me/perfil", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db,
    [FromBody] UpdateDatosPersonalesDto dto) =>
{
    var userEmail = httpContext.User.FindFirst("preferred_username")?.Value
                    ?? httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

    if (string.IsNullOrEmpty(userEmail))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .Include(u => u.Alumnos)
        .FirstOrDefaultAsync(u => u.Correo == userEmail);

    if (usuario?.Alumnos.FirstOrDefault() == null)
        return Results.NotFound(new { error = "Perfil de alumno no encontrado" });

    var idAlumno = usuario.Alumnos.First().IdAlumno;
    var command = new UpdateDatosPersonalesCommand(idAlumno, dto.NombreCompleto, dto.Telefono);
    var result = await mediator.Send(command);

    return result.Succeeded
        ? Results.Ok(new { message = "Datos personales actualizados correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// PUT /api/alumnos/me/contacto-emergencia - Actualizar contacto de emergencia (ApiScope)
app.MapPut("/api/alumnos/me/contacto-emergencia", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db,
    [FromBody] UpdateContactoEmergenciaDto dto) =>
{
    var userEmail = httpContext.User.FindFirst("preferred_username")?.Value
                    ?? httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

    if (string.IsNullOrEmpty(userEmail))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .Include(u => u.Alumnos)
        .FirstOrDefaultAsync(u => u.Correo == userEmail);

    if (usuario?.Alumnos.FirstOrDefault() == null)
        return Results.NotFound(new { error = "Perfil de alumno no encontrado" });

    var idAlumno = usuario.Alumnos.First().IdAlumno;
    var command = new UpdateContactoEmergenciaCommand(idAlumno, dto.NombreCompleto, dto.Telefono, dto.Relacion);
    var result = await mediator.Send(command);

    return result.Succeeded
        ? Results.Ok(new { message = "Contacto de emergencia actualizado correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// PUT /api/alumnos/me/configuracion - Actualizar configuración de notificaciones (ApiScope)
app.MapPut("/api/alumnos/me/configuracion", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db,
    [FromBody] UpdateConfiguracionDto dto) =>
{
    var userEmail = httpContext.User.FindFirst("preferred_username")?.Value
                    ?? httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

    if (string.IsNullOrEmpty(userEmail))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .Include(u => u.Alumnos)
        .FirstOrDefaultAsync(u => u.Correo == userEmail);

    if (usuario?.Alumnos.FirstOrDefault() == null)
        return Results.NotFound(new { error = "Perfil de alumno no encontrado" });

    var idAlumno = usuario.Alumnos.First().IdAlumno;
    var command = new UpdateConfiguracionCommand(idAlumno, dto.NotificacionesEmail, dto.RecordatoriosClase, dto.AlertasPaquete);
    var result = await mediator.Send(command);

    return result.Succeeded
        ? Results.Ok(new { message = "Configuración actualizada correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// GET /api/alumnos/me/paquetes/historial - Obtener historial de paquetes (ApiScope)
app.MapGet("/api/alumnos/me/paquetes/historial", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db) =>
{
    var userEmail = httpContext.User.FindFirst("preferred_username")?.Value
                    ?? httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

    if (string.IsNullOrEmpty(userEmail))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .Include(u => u.Alumnos)
        .FirstOrDefaultAsync(u => u.Correo == userEmail);

    if (usuario?.Alumnos.FirstOrDefault() == null)
        return Results.NotFound(new { error = "Perfil de alumno no encontrado" });

    var idAlumno = usuario.Alumnos.First().IdAlumno;
    var result = await mediator.Send(new GetPaquetesHistorialQuery(idAlumno));

    return result.Succeeded
        ? Results.Ok(result.Value)
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// POST /api/alumnos/me/avatar - Subir avatar del alumno (ApiScope)
app.MapPost("/api/alumnos/me/avatar", async (
    HttpContext httpContext,
    HttpRequest request,
    IMediator mediator,
    IAppDbContext db) =>
{
    var userEmail = httpContext.User.FindFirst("preferred_username")?.Value
                    ?? httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

    if (string.IsNullOrEmpty(userEmail))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .Include(u => u.Alumnos)
        .FirstOrDefaultAsync(u => u.Correo == userEmail);

    if (usuario?.Alumnos.FirstOrDefault() == null)
        return Results.NotFound(new { error = "Perfil de alumno no encontrado" });

    if (!request.HasFormContentType || !request.Form.Files.Any())
        return Results.BadRequest(new { error = "No se ha enviado ningún archivo" });

    var file = request.Form.Files[0];
    using var memoryStream = new MemoryStream();
    await file.CopyToAsync(memoryStream);
    var fileBytes = memoryStream.ToArray();

    var idAlumno = usuario.Alumnos.First().IdAlumno;
    var command = new UploadAvatarCommand(idAlumno, file.FileName, fileBytes);
    var result = await mediator.Send(command);

    return result.Succeeded
        ? Results.Ok(new { url = result.Value, message = "Avatar actualizado correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("ApiScope");

// ============================================
// PERFIL ADMIN ENDPOINTS
// ============================================

// GET /api/admin/me/perfil - Obtener perfil del admin autenticado (AdminOnly)
app.MapGet("/api/admin/me/perfil", async (
    HttpContext httpContext,
    IMediator mediator) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var query = new GetPerfilAdminQuery(emailClaim);
    var result = await mediator.Send(query);
    
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// PUT /api/admin/me/datos-personales - Actualizar datos personales del admin (AdminOnly)
app.MapPut("/api/admin/me/datos-personales", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db,
    [FromBody] UpdateDatosPersonalesAdminDTO dto) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .FirstOrDefaultAsync(u => u.Correo == emailClaim);

    if (usuario == null)
        return Results.NotFound(new { error = "Usuario no encontrado" });

    var command = new UpdateDatosPersonalesAdminCommand(
        usuario.IdUsuario,
        dto.NombreCompleto,
        dto.Telefono,
        dto.Direccion ?? string.Empty,
        dto.FechaNacimiento
    );
    
    var result = await mediator.Send(command);
    
    return result.Succeeded
        ? Results.Ok(new { message = "Datos personales actualizados correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// PUT /api/admin/me/datos-academia - Actualizar datos de la academia (AdminOnly)
app.MapPut("/api/admin/me/datos-academia", async (
    IMediator mediator,
    [FromBody] UpdateDatosAcademiaDTO dto) =>
{
    var command = new UpdateDatosAcademiaCommand(
        dto.Nombre,
        dto.Direccion,
        dto.Telefono,
        dto.Email,
        dto.Instagram,
        dto.Facebook,
        dto.WhatsApp
    );
    
    var result = await mediator.Send(command);
    
    return result.Succeeded
        ? Results.Ok(new { message = "Datos de academia actualizados correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// PUT /api/admin/me/configuracion - Actualizar configuración del admin (AdminOnly)
app.MapPut("/api/admin/me/configuracion", async (
    HttpContext httpContext,
    IMediator mediator,
    IAppDbContext db,
    [FromBody] UpdateConfiguracionAdminDTO dto) =>
{
    var emailClaim = httpContext.User.FindFirstValue(ClaimTypes.Email)
        ?? httpContext.User.FindFirst("preferred_username")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var usuario = await db.Usuarios
        .FirstOrDefaultAsync(u => u.Correo == emailClaim);

    if (usuario == null)
        return Results.NotFound(new { error = "Usuario no encontrado" });

    var command = new UpdateConfiguracionAdminCommand(
        usuario.IdUsuario,
        dto.NotificacionesEmail,
        dto.AlertasPagosPendientes,
        dto.ReportesAutomaticos,
        dto.AlertasPaquetesVencer,
        dto.AlertasAsistenciaBaja,
        dto.NotificacionesNuevosRegistros
    );
    
    var result = await mediator.Send(command);
    
    return result.Succeeded
        ? Results.Ok(new { message = "Configuración actualizada correctamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/reportes/exportar - Exportar reporte (AdminOrProfesor)
app.MapGet("/api/reportes/exportar", async (
    IMediator mediator,
    ExcelExportService excelService,
    PdfExportService pdfService,
    CsvExportService csvService,
    string tipoReporte,
    string formato,
    DateTime fechaDesde,
    DateTime fechaHasta,
    Guid? idClase = null,
    Guid? idAlumno = null,
    Guid? idProfesor = null,
    string? estado = null,
    Guid? idMetodoPago = null,
    Guid? idTipoPaquete = null) =>
{
    byte[] fileBytes;
    string contentType;
    string fileName;

    // Generar reporte según tipo
    if (tipoReporte.ToLower() == "asistencias")
    {
        var query = new GetReporteAsistenciasQuery
        {
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            IdClase = idClase,
            IdAlumno = idAlumno,
            IdProfesor = idProfesor,
            EmailUsuario = "",
            EsAdmin = true,
            EsProfesor = false
        };
        var result = await mediator.Send(query);
        
        if (!result.Succeeded)
            return Results.BadRequest(new { error = result.Error });

        fileBytes = formato.ToLower() switch
        {
            "excel" => excelService.ExportarAsistencias(result.Value!),
            "pdf" => pdfService.ExportarAsistencias(result.Value!, fechaDesde, fechaHasta),
            "csv" => csvService.ExportarAsistencias(result.Value!),
            _ => throw new InvalidOperationException("Formato no soportado")
        };
        fileName = $"reporte-asistencias-{DateTime.Now:yyyyMMdd}.{formato.ToLower()}";
    }
    else if (tipoReporte.ToLower() == "ingresos")
    {
        var query = new GetReporteIngresosQuery
        {
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            IdMetodoPago = idMetodoPago,
            ComparativaMesAnterior = false
        };
        var result = await mediator.Send(query);
        
        if (!result.Succeeded)
            return Results.BadRequest(new { error = result.Error });

        fileBytes = formato.ToLower() switch
        {
            "excel" => excelService.ExportarIngresos(result.Value!),
            "pdf" => pdfService.ExportarIngresos(result.Value!, fechaDesde, fechaHasta),
            "csv" => csvService.ExportarIngresos(result.Value!),
            _ => throw new InvalidOperationException("Formato no soportado")
        };
        fileName = $"reporte-ingresos-{DateTime.Now:yyyyMMdd}.{formato.ToLower()}";
    }
    else if (tipoReporte.ToLower() == "paquetes")
    {
        var query = new GetReportePaquetesQuery
        {
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Estado = estado,
            IdTipoPaquete = idTipoPaquete
        };
        var result = await mediator.Send(query);
        
        if (!result.Succeeded)
            return Results.BadRequest(new { error = result.Error });

        fileBytes = formato.ToLower() switch
        {
            "excel" => excelService.ExportarPaquetes(result.Value!),
            "pdf" => pdfService.ExportarPaquetes(result.Value!, fechaDesde, fechaHasta),
            "csv" => csvService.ExportarPaquetes(result.Value!),
            _ => throw new InvalidOperationException("Formato no soportado")
        };
        fileName = $"reporte-paquetes-{DateTime.Now:yyyyMMdd}.{formato.ToLower()}";
    }
    else if (tipoReporte.ToLower() == "clases")
    {
        var query = new GetReporteClasesQuery
        {
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            IdProfesor = idProfesor,
            EmailUsuario = "",
            EsAdmin = true,
            EsProfesor = false
        };
        var result = await mediator.Send(query);
        
        if (!result.Succeeded)
            return Results.BadRequest(new { error = result.Error });

        fileBytes = formato.ToLower() switch
        {
            "excel" => excelService.ExportarClases(result.Value!),
            "csv" => csvService.ExportarClases(result.Value!),
            _ => throw new InvalidOperationException("Formato no soportado")
        };
        fileName = $"reporte-clases-{DateTime.Now:yyyyMMdd}.{formato.ToLower()}";
    }
    else if (tipoReporte.ToLower() == "alumnos")
    {
        var query = new GetReporteAlumnosQuery
        {
            FechaInscripcionDesde = fechaDesde,
            FechaInscripcionHasta = fechaHasta,
            Estado = estado
        };
        var result = await mediator.Send(query);
        
        if (!result.Succeeded)
            return Results.BadRequest(new { error = result.Error });

        fileBytes = formato.ToLower() switch
        {
            "excel" => excelService.ExportarAlumnos(result.Value!),
            "csv" => csvService.ExportarAlumnos(result.Value!),
            _ => throw new InvalidOperationException("Formato no soportado")
        };
        fileName = $"reporte-alumnos-{DateTime.Now:yyyyMMdd}.{formato.ToLower()}";
    }
    else
    {
        return Results.BadRequest(new { error = "Tipo de reporte no soportado" });
    }

    // Definir contentType
    contentType = formato.ToLower() switch
    {
        "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "pdf" => "application/pdf",
        "csv" => "text/csv",
        _ => "application/octet-stream"
    };

    return Results.File(fileBytes, contentType, fileName);
}).RequireAuthorization("AdminOrProfesor");

// ====== ENDPOINTS DE USUARIOS ======

// GET /api/usuarios - Lista paginada de usuarios con filtros (AdminOnly)
app.MapGet("/api/usuarios", async (
    int page = 1,
    int pageSize = 10,
    string? searchTerm = null,
    string? rol = null,
    string? estado = null,
    IMediator mediator = null!) =>
{
    var query = new GetUsersQuery(page, pageSize, searchTerm, rol, estado);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/usuarios/{id} - Detalle de usuario (AdminOnly)
app.MapGet("/api/usuarios/{id:guid}", async (
    Guid id,
    IMediator mediator) =>
{
    var query = new GetUserDetailQuery(id);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.NotFound(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// POST /api/usuarios - Crear un nuevo usuario (AdminOnly)
app.MapPost("/api/usuarios", async (
    CreateUserRequest request,
    ClaimsPrincipal user,
    IMediator mediator) =>
{
    var email = user.FindFirst(ClaimTypes.Email)?.Value 
             ?? user.FindFirst("preferred_username")?.Value 
             ?? user.FindFirst("email")?.Value;

    var datosProfesor = request.DatosProfesor != null
        ? new DatosProfesorRequest(
            request.DatosProfesor.TipoProfesor,
            request.DatosProfesor.FechaIngreso,
            request.DatosProfesor.Biografia,
            request.DatosProfesor.Especialidades,
            request.DatosProfesor.TarifaActual)
        : null;

    var datosAlumno = request.DatosAlumno != null
        ? new DatosAlumnoRequest(
            request.DatosAlumno.ContactoEmergencia,
            request.DatosAlumno.TelefonoEmergencia,
            request.DatosAlumno.ObservacionesMedicas)
        : null;

    var command = new CreateUserCommand(
        request.NombreUsuario,
        request.Correo,
        request.Telefono,
        request.TipoDocumento,
        request.NumeroDocumento,
        request.Rol,
        request.FechaNacimiento,
        datosProfesor,
        datosAlumno,
        request.CorreoAzure,
        request.EnviarWhatsApp,
        request.EnviarEmail,
        null, // Sede - se heredará del usuario
        email // EmailUsuarioCreador
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Created($"/api/usuarios/{result.Value}", new { idUsuario = result.Value }) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// PUT /api/usuarios/{id} - Actualizar un usuario (AdminOnly)
app.MapPut("/api/usuarios/{id:guid}", async (
    Guid id,
    UpdateUserRequest request,
    IMediator mediator) =>
{
    if (id != request.IdUsuario)
        return Results.BadRequest(new { error = "El ID en la ruta no coincide con el ID del request" });

    var datosProfesor = request.DatosProfesor != null
        ? new DatosProfesorRequest(
            request.DatosProfesor.TipoProfesor,
            request.DatosProfesor.FechaIngreso,
            request.DatosProfesor.Biografia,
            request.DatosProfesor.Especialidades,
            request.DatosProfesor.TarifaActual)
        : null;

    var datosAlumno = request.DatosAlumno != null
        ? new DatosAlumnoRequest(
            request.DatosAlumno.ContactoEmergencia,
            request.DatosAlumno.TelefonoEmergencia,
            request.DatosAlumno.ObservacionesMedicas)
        : null;

    var command = new UpdateUserCommand(
        request.IdUsuario,
        request.NombreUsuario,
        request.Telefono,
        request.FechaNacimiento,
        datosProfesor,
        datosAlumno
    );

    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.NoContent() 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// DELETE /api/usuarios/{id} - Eliminar un usuario (AdminOnly)
app.MapDelete("/api/usuarios/{id:guid}", async (
    Guid id,
    IMediator mediator) =>
{
    var command = new DeleteUserCommand(id);
    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.NoContent() 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// POST /api/usuarios/{id}/activate - Activar un usuario (AdminOnly)
app.MapPost("/api/usuarios/{id:guid}/activate", async (
    Guid id,
    ActivateUserRequest request,
    IMediator mediator) =>
{
    var command = new ActivateUserCommand(id, request.CorreoAzure, request.ContrasenaTemporalAzure);
    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.NoContent() 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// ====== ENDPOINTS DE NÓMINA ======

// GET /api/nomina/clases-realizadas - Obtener clases realizadas con información de pago (AdminOnly)
app.MapGet("/api/nomina/clases-realizadas", async (
    DateTime? fechaDesde = null,
    DateTime? fechaHasta = null,
    Guid? idProfesor = null,
    string? estadoPago = null,
    IMediator mediator = null!) =>
{
    var query = new GetClasesRealizadasQuery(fechaDesde, fechaHasta, idProfesor, estadoPago);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/nomina/clases-aprobadas - Obtener clases aprobadas de un profesor para un mes (AdminOnly)
app.MapGet("/api/nomina/clases-aprobadas", async (
    Guid idProfesor,
    int mes,
    int año,
    IMediator mediator) =>
{
    var query = new GetClasesAprobadasQuery(idProfesor, mes, año);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/nomina/resumen-profesores - Obtener resumen de profesores (AdminOnly)
app.MapGet("/api/nomina/resumen-profesores", async (
    Guid? idProfesor = null,
    IMediator mediator = null!) =>
{
    var query = new GetResumenProfesorQuery(idProfesor);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/nomina/mi-resumen - Obtener resumen del profesor autenticado (Profesor)
app.MapGet("/api/nomina/mi-resumen", async (
    ClaimsPrincipal user,
    ChetangoDbContext db,
    IMediator mediator) =>
{
    var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value
        ?? user.FindFirst("preferred_username")?.Value
        ?? user.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    // Obtener el ID del profesor desde el email
    var profesor = await db.Profesores
        .Include(p => p.Usuario)
        .FirstOrDefaultAsync(p => p.Usuario.Correo == emailClaim);

    if (profesor is null)
        return Results.NotFound(new { error = "Profesor no encontrado" });

    var query = new GetResumenProfesorQuery(profesor.IdProfesor);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization();

// GET /api/nomina/liquidacion - Obtener detalle de liquidación (AdminOnly)
app.MapGet("/api/nomina/liquidacion", async (
    Guid? idLiquidacion = null,
    Guid? idProfesor = null,
    int? mes = null,
    int? año = null,
    IMediator mediator = null!) =>
{
    var query = new GetLiquidacionMensualQuery(idLiquidacion, idProfesor, mes, año);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/nomina/liquidaciones-por-estado - Obtener liquidaciones filtradas por estado (AdminOnly)
app.MapGet("/api/nomina/liquidaciones-por-estado", async (
    string? estado = null,
    int? año = null,
    IMediator mediator = null!) =>
{
    var query = new GetLiquidacionesPorEstadoQuery(estado, año);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/nomina/mis-liquidaciones-lista - Obtener lista de liquidaciones del profesor autenticado
app.MapGet("/api/nomina/mis-liquidaciones-lista", async (
    int? año = null,
    ClaimsPrincipal user = null!,
    ChetangoDbContext db = null!,
    IMediator mediator = null!) =>
{
    var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value
        ?? user.FindFirst("preferred_username")?.Value
        ?? user.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    var profesor = await db.Profesores
        .Include(p => p.Usuario)
        .FirstOrDefaultAsync(p => p.Usuario.Correo == emailClaim);

    if (profesor is null)
        return Results.NotFound(new { error = "Profesor no encontrado" });

    var query = new GetLiquidacionesProfesorQuery(profesor.IdProfesor, año);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization();

// GET /api/nomina/mis-liquidaciones - Obtener liquidaciones del profesor autenticado (Profesor)
app.MapGet("/api/nomina/mis-liquidaciones", async (
    Guid? idLiquidacion = null,
    int? mes = null,
    int? año = null,
    ClaimsPrincipal user = null!,
    ChetangoDbContext db = null!,
    IMediator mediator = null!) =>
{
    var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value
        ?? user.FindFirst("preferred_username")?.Value
        ?? user.FindFirst("upn")?.Value;

    if (string.IsNullOrWhiteSpace(emailClaim))
        return Results.Unauthorized();

    // Obtener el ID del profesor desde el email
    var profesor = await db.Profesores
        .Include(p => p.Usuario)
        .FirstOrDefaultAsync(p => p.Usuario.Correo == emailClaim);

    if (profesor is null)
        return Results.NotFound(new { error = "Profesor no encontrado" });

    var query = new GetLiquidacionMensualQuery(idLiquidacion, profesor.IdProfesor, mes, año);
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization();

// POST /api/nomina/aprobar-pago - Aprobar pago de una clase (AdminOnly)
app.MapPost("/api/nomina/aprobar-pago", async (
    AprobarPagoClaseRequest request,
    ClaimsPrincipal user,
    ChetangoDbContext db,
    IMediator mediator,
    ILogger<Program> logger) =>
{
    // Extraer email del claim correcto (Azure AD B2C usa 'preferred_username')
    var email = user.FindFirst(ClaimTypes.Email)?.Value 
             ?? user.FindFirst("preferred_username")?.Value 
             ?? user.FindFirst("email")?.Value;
    
    if (string.IsNullOrEmpty(email))
        return Results.Unauthorized();
    
    var admin = await db.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
    if (admin == null)
        return Results.Unauthorized();
    
    var command = new AprobarPagoClaseCommand(
        request.IdClaseProfesor,
        request.ValorAdicional,
        request.ConceptoAdicional,
        admin.IdUsuario
    );
    
    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(new { success = true, message = "Pago aprobado exitosamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// POST /api/nomina/liquidar-mes - Generar liquidación mensual (AdminOnly)
app.MapPost("/api/nomina/liquidar-mes", async (
    LiquidarMesRequest request,
    ClaimsPrincipal user,
    ChetangoDbContext db,
    IMediator mediator) =>
{
    var email = user.FindFirst(ClaimTypes.Email)?.Value 
             ?? user.FindFirst("preferred_username")?.Value 
             ?? user.FindFirst("email")?.Value;
    if (string.IsNullOrEmpty(email))
        return Results.Unauthorized();
    
    var admin = await db.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
    if (admin == null)
        return Results.Unauthorized();
    
    var command = new LiquidarMesCommand(
        request.IdProfesor,
        request.Mes,
        request.Año,
        admin.IdUsuario,
        request.Observaciones
    );
    
    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(result.Value)
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// POST /api/nomina/registrar-pago - Registrar pago realizado (AdminOnly)
app.MapPost("/api/nomina/registrar-pago", async (
    RegistrarPagoProfesorRequest request,
    IMediator mediator) =>
{
    var command = new RegistrarPagoProfesorCommand(
        request.IdLiquidacion,
        request.FechaPago,
        request.Observaciones
    );
    
    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(new { success = true, message = "Pago registrado exitosamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// DELETE /api/nomina/liquidacion/{id} - Eliminar liquidación (AdminOnly)
app.MapDelete("/api/nomina/liquidacion/{id:guid}", async (
    Guid id,
    IMediator mediator) =>
{
    var command = new EliminarLiquidacionCommand(id);
    var result = await mediator.Send(command);
    return result.Succeeded 
        ? Results.Ok(new { success = true, message = "Liquidación eliminada exitosamente" })
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

// GET /api/nomina/clases-profesor/{idProfesor} - Obtener clases de un profesor con filtros (AdminOnly)
app.MapGet("/api/nomina/clases-profesor/{idProfesor:guid}", async (
    Guid idProfesor,
    [FromQuery] DateTime? fechaDesde,
    [FromQuery] DateTime? fechaHasta,
    [FromQuery] string? estadoPago,
    IMediator mediator) =>
{
    var query = new GetClasesPorProfesorQuery(
        idProfesor,
        fechaDesde,
        fechaHasta,
        estadoPago
    );
    
    var result = await mediator.Send(query);
    return result.Succeeded 
        ? Results.Ok(result.Value)
        : Results.BadRequest(new { error = result.Error });
}).RequireAuthorization("AdminOnly");

app.Run();

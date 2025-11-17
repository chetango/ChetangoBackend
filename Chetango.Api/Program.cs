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
using Chetango.Domain.Entities; // Added for Usuario
using Chetango.Domain.Entities.Estados; // Added for TipoDocumento
using Chetango.Application.Common; // registrar IAppDbContext
using Chetango.Api.Infrastructure; // MigrationRunner
using Microsoft.OpenApi.Models; // Swagger security

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

// Endpoint protegido de prueba: permite verificar que el token es válido
app.MapGet("/auth/ping", (ClaimsPrincipal user) =>
{
    var oid = user.FindFirst("oid")?.Value ?? user.FindFirst("sub")?.Value;
    return Results.Ok(new { message = "pong", oid });
}).RequireAuthorization("ApiScope");

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
    IMediator mediator) =>
{
    var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase.GetAsistenciasPorClaseQuery(idClase);
    var result = await mediator.Send(query);
    return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
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

    // GET /api/dev/clases/{idClase}/asistencias - Obtener asistencias de clase sin autenticación
    app.MapGet("/api/dev/clases/{idClase:guid}/asistencias", async (
        Guid idClase,
        IMediator mediator) =>
    {
        var query = new Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase.GetAsistenciasPorClaseQuery(idClase);
        var result = await mediator.Send(query);
        return result.Succeeded ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
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
// ============================================
// CREATE USER COMMAND
// ============================================

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Chetango.Application.Usuarios.Commands;

public record CreateUserCommand(
    string NombreUsuario,
    string Correo,
    string Telefono,
    string TipoDocumento,
    string NumeroDocumento,
    string Rol, // "admin" | "profesor" | "alumno"
    string? FechaNacimiento,
    DatosProfesorRequest? DatosProfesor,
    DatosAlumnoRequest? DatosAlumno,
    string CorreoAzure,
    string ContrasenaTemporalAzure,
    bool EnviarWhatsApp,
    bool EnviarEmail
) : IRequest<Result<Guid>>;

public record DatosProfesorRequest(
    string TipoProfesor,
    DateTime FechaIngreso,
    string? Biografia,
    List<string> Especialidades,
    decimal TarifaActual
);

public record DatosAlumnoRequest(
    string? ContactoEmergencia,
    string? TelefonoEmergencia,
    string? ObservacionesMedicas
);

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public CreateUserCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que el correo no exista
        var existeCorreo = await _db.Set<Usuario>()
            .AnyAsync(u => u.Correo == request.Correo, cancellationToken);

        if (existeCorreo)
            return Result<Guid>.Failure("Ya existe un usuario con ese correo");

        // 2. Validar que el documento no exista
        var existeDocumento = await _db.Set<Usuario>()
            .AnyAsync(u => u.NumeroDocumento == request.NumeroDocumento, cancellationToken);

        if (existeDocumento)
            return Result<Guid>.Failure("Ya existe un usuario con ese documento");

        // 3. Obtener el tipo de documento
        // Log para debug
        Console.WriteLine($"DEBUG - TipoDocumento recibido: [{request.TipoDocumento}] Length: {request.TipoDocumento?.Length}");
        
        if (string.IsNullOrWhiteSpace(request.TipoDocumento))
            return Result<Guid>.Failure("El tipo de documento es requerido");
        
        var tipoDocumento = await _db.Set<TipoDocumento>()
            .FirstOrDefaultAsync(td => td.Nombre.Trim().ToLower() == request.TipoDocumento.Trim().ToLower(), cancellationToken);

        if (tipoDocumento == null)
        {
            // Mostrar todos los tipos disponibles para debug
            var tiposDisponibles = await _db.Set<TipoDocumento>().ToListAsync(cancellationToken);
            Console.WriteLine($"DEBUG - Tipos disponibles en BD: {string.Join(", ", tiposDisponibles.Select(t => $"[{t.Nombre}]"))}");
            return Result<Guid>.Failure("Tipo de documento no válido");
        }

        // 4. Obtener estado "Activo"
        var estadoActivo = await _db.Set<EstadoUsuario>()
            .FirstOrDefaultAsync(e => e.Nombre == "Activo", cancellationToken);

        if (estadoActivo == null)
            return Result<Guid>.Failure("Estado Activo no encontrado");

        // 5. Crear el usuario
        var usuario = new Usuario
        {
            IdUsuario = Guid.NewGuid(),
            NombreUsuario = request.NombreUsuario,
            Correo = request.Correo,
            Telefono = request.Telefono,
            IdTipoDocumento = tipoDocumento.Id,
            NumeroDocumento = request.NumeroDocumento,
            IdEstadoUsuario = estadoActivo.Id,
            FechaCreacion = DateTimeHelper.Now
        };

        _db.Set<Usuario>().Add(usuario);

        // 6. Crear registro según el rol
        switch (request.Rol.ToLower())
        {
            case "profesor":
                if (request.DatosProfesor == null)
                    return Result<Guid>.Failure("Datos de profesor requeridos");

                var tipoProfesor = await _db.Set<TipoProfesor>()
                    .FirstOrDefaultAsync(tp => tp.Nombre == request.DatosProfesor.TipoProfesor, cancellationToken);

                if (tipoProfesor == null)
                    return Result<Guid>.Failure("Tipo de profesor no válido");

                var profesor = new Profesor
                {
                    IdProfesor = Guid.NewGuid(),
                    IdUsuario = usuario.IdUsuario,
                    IdTipoProfesor = tipoProfesor.Id,
                    Biografia = request.DatosProfesor.Biografia,
                    Especialidades = JsonSerializer.Serialize(request.DatosProfesor.Especialidades),
                    TarifaActual = request.DatosProfesor.TarifaActual,
                    NotificacionesEmail = true,
                    RecordatoriosClase = true,
                    AlertasCambios = true
                };

                _db.Set<Profesor>().Add(profesor);
                break;

            case "alumno":
                var estadoAlumno = await _db.Set<EstadoAlumno>()
                    .FirstOrDefaultAsync(e => e.Nombre == "Activo", cancellationToken);

                if (estadoAlumno == null)
                    return Result<Guid>.Failure("Estado de alumno no encontrado");

                var alumno = new Alumno
                {
                    IdAlumno = Guid.NewGuid(),
                    IdUsuario = usuario.IdUsuario,
                    FechaInscripcion = DateTimeHelper.Now,
                    IdEstado = estadoAlumno.IdEstado,
                    ContactoEmergenciaNombre = request.DatosAlumno?.ContactoEmergencia,
                    ContactoEmergenciaTelefono = request.DatosAlumno?.TelefonoEmergencia,
                    // ObservacionesMedicas se guardaría en una tabla adicional si existiera
                    NotificacionesEmail = true,
                    RecordatoriosClase = true,
                    AlertasPaquete = true
                };

                _db.Set<Alumno>().Add(alumno);
                break;

            case "admin":
                // Admin no tiene tabla adicional, solo se diferencia por rol en Azure
                break;

            default:
                return Result<Guid>.Failure("Rol no válido");
        }

        await _db.SaveChangesAsync(cancellationToken);

        // TODO: Enviar notificaciones (WhatsApp/Email) con credenciales
        // Implementar servicio de notificaciones que envíe:
        // - CorreoAzure
        // - ContrasenaTemporalAzure
        // - Mensaje de bienvenida

        return Result<Guid>.Success(usuario.IdUsuario);
    }
}

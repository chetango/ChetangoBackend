// ============================================
// UPDATE USER COMMAND
// ============================================

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Chetango.Application.Usuarios.Commands;

public record UpdateUserCommand(
    Guid UsuarioId,
    string NombreUsuario,
    string Telefono,
    string? FechaNacimiento,
    DatosProfesorRequest? DatosProfesor,
    DatosAlumnoRequest? DatosAlumno
) : IRequest<Result<Unit>>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdateUserCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Usuario>()
            .Include(u => u.Profesores)
            .Include(u => u.Alumnos)
            .FirstOrDefaultAsync(u => u.IdUsuario == request.UsuarioId, cancellationToken);

        if (usuario == null)
            return Result<Unit>.Failure("Usuario no encontrado");

        // Actualizar datos básicos
        usuario.NombreUsuario = request.NombreUsuario;
        usuario.Telefono = request.Telefono;

        // Actualizar datos de profesor si aplica
        if (request.DatosProfesor != null)
        {
            var profesor = usuario.Profesores.FirstOrDefault();
            if (profesor != null)
            {
                var tipoProfesor = await _db.Set<TipoProfesor>()
                    .FirstOrDefaultAsync(tp => tp.Nombre == request.DatosProfesor.TipoProfesor, cancellationToken);

                if (tipoProfesor != null)
                {
                    profesor.IdTipoProfesor = tipoProfesor.Id;
                }

                profesor.Biografia = request.DatosProfesor.Biografia;
                profesor.Especialidades = JsonSerializer.Serialize(request.DatosProfesor.Especialidades);
                profesor.TarifaActual = request.DatosProfesor.TarifaActual;
            }
        }

        // Actualizar datos de alumno si aplica
        if (request.DatosAlumno != null)
        {
            var alumno = usuario.Alumnos.FirstOrDefault();
            if (alumno != null)
            {
                alumno.ContactoEmergenciaNombre = request.DatosAlumno.ContactoEmergencia;
                alumno.ContactoEmergenciaTelefono = request.DatosAlumno.TelefonoEmergencia;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

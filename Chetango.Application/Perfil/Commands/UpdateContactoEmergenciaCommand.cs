// ============================================
// UPDATE CONTACTO EMERGENCIA COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Perfil.Commands;

public record UpdateContactoEmergenciaCommand(
    Guid IdAlumno,
    string NombreCompleto,
    string Telefono,
    string Relacion
) : IRequest<Result<Unit>>;

public class UpdateContactoEmergenciaCommandHandler : IRequestHandler<UpdateContactoEmergenciaCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdateContactoEmergenciaCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(UpdateContactoEmergenciaCommand request, CancellationToken cancellationToken)
    {
        var alumno = await _db.Alumnos
            .FirstOrDefaultAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (alumno == null)
            return Result<Unit>.Failure("Alumno no encontrado");

        alumno.ContactoEmergenciaNombre = request.NombreCompleto;
        alumno.ContactoEmergenciaTelefono = request.Telefono;
        alumno.ContactoEmergenciaRelacion = request.Relacion;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

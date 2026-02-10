// ============================================
// UPDATE DATOS PERSONALES COMMAND
// ============================================

using Chetango.Application.Common;
using Chetango.Application.Perfil.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Perfil.Commands;

public record UpdateDatosPersonalesCommand(
    Guid IdAlumno,
    string NombreCompleto,
    string Telefono
) : IRequest<Result<Unit>>;

public class UpdateDatosPersonalesCommandHandler : IRequestHandler<UpdateDatosPersonalesCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdateDatosPersonalesCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(UpdateDatosPersonalesCommand request, CancellationToken cancellationToken)
    {
        var alumno = await _db.Alumnos
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (alumno == null)
            return Result<Unit>.Failure("Alumno no encontrado");

        // Actualizar Usuario
        alumno.Usuario.NombreUsuario = request.NombreCompleto;
        alumno.Usuario.Telefono = request.Telefono;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

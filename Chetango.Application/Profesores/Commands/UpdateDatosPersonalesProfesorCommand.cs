// ============================================
// UPDATE DATOS PERSONALES PROFESOR COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Profesores.Commands;

public record UpdateDatosPersonalesProfesorCommand(
    Guid IdProfesor,
    string NombreCompleto,
    string Telefono
) : IRequest<Result<Unit>>;

public class UpdateDatosPersonalesProfesorCommandHandler : IRequestHandler<UpdateDatosPersonalesProfesorCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdateDatosPersonalesProfesorCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(UpdateDatosPersonalesProfesorCommand request, CancellationToken cancellationToken)
    {
        var profesor = await _db.Set<Chetango.Domain.Entities.Estados.Profesor>()
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.IdProfesor == request.IdProfesor, cancellationToken);

        if (profesor == null)
            return Result<Unit>.Failure("Profesor no encontrado");

        // Actualizar datos del usuario
        profesor.Usuario.NombreUsuario = request.NombreCompleto;
        profesor.Usuario.Telefono = request.Telefono;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

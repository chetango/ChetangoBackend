// ============================================
// UPDATE DATOS PERSONALES ADMIN COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Admin.Commands;

public record UpdateDatosPersonalesAdminCommand(
    Guid IdUsuario,
    string NombreCompleto,
    string Telefono,
    string Direccion,
    DateTime? FechaNacimiento
) : IRequest<Result<Unit>>;

public class UpdateDatosPersonalesAdminCommandHandler : IRequestHandler<UpdateDatosPersonalesAdminCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdateDatosPersonalesAdminCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(UpdateDatosPersonalesAdminCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Chetango.Domain.Entities.Usuario>()
            .FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuario, cancellationToken);

        if (usuario == null)
            return Result<Unit>.Failure("Usuario no encontrado");

        // Actualizar datos del usuario
        usuario.NombreUsuario = request.NombreCompleto;
        usuario.Telefono = request.Telefono;
        // Direccion y FechaNacimiento se guardarían en una tabla extendida si existiera
        // Por ahora solo actualizamos los campos que existen en Usuario

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

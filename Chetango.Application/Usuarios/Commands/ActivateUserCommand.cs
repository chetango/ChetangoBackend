// ============================================
// ACTIVATE USER COMMAND (Azure Credentials)
// ============================================

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Usuarios.Commands;

public record ActivateUserCommand(
    Guid UsuarioId,
    string CorreoAzure,
    string ContrasenaTemporalAzure
) : IRequest<Result<Unit>>;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public ActivateUserCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Usuario>()
            .FirstOrDefaultAsync(u => u.IdUsuario == request.UsuarioId, cancellationToken);

        if (usuario == null)
            return Result<Unit>.Failure("Usuario no encontrado");

        // Obtener estado activo
        var estadoActivo = await _db.Set<EstadoUsuario>()
            .FirstOrDefaultAsync(e => e.Nombre == "Activo", cancellationToken);

        if (estadoActivo == null)
            return Result<Unit>.Failure("Estado activo no encontrado");

        // Actualizar estado
        usuario.IdEstadoUsuario = estadoActivo.Id;

        await _db.SaveChangesAsync(cancellationToken);

        // TODO: Enviar credenciales al usuario por WhatsApp/Email
        // - CorreoAzure
        // - ContrasenaTemporalAzure

        return Result<Unit>.Success(Unit.Value);
    }
}

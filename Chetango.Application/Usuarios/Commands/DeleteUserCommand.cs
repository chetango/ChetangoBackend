// ============================================
// DELETE USER COMMAND
// ============================================

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Usuarios.Commands;

public record DeleteUserCommand(Guid UsuarioId) : IRequest<Result<Unit>>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public DeleteUserCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Usuario>()
            .FirstOrDefaultAsync(u => u.IdUsuario == request.UsuarioId, cancellationToken);

        if (usuario == null)
            return Result<Unit>.Failure("Usuario no encontrado");

        // Soft delete: cambiar a estado inactivo
        var estadoInactivo = await _db.Set<EstadoUsuario>()
            .FirstOrDefaultAsync(e => e.Nombre == "Inactivo", cancellationToken);

        if (estadoInactivo == null)
            return Result<Unit>.Failure("Estado inactivo no encontrado");

        usuario.IdEstadoUsuario = estadoInactivo.Id;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

// ============================================
// CAMBIAR PASSWORD COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Admin.Commands;

public record CambiarPasswordCommand(
    Guid IdUsuario,
    string PasswordActual,
    string PasswordNuevo
) : IRequest<Result<Unit>>;

public class CambiarPasswordCommandHandler : IRequestHandler<CambiarPasswordCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public CambiarPasswordCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(CambiarPasswordCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Chetango.Domain.Entities.Usuario>()
            .FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuario, cancellationToken);

        if (usuario == null)
            return Result<Unit>.Failure("Usuario no encontrado");

        // NOTA: Como la autenticación se maneja con Azure Entra ID, 
        // el cambio de contraseña debe hacerse a través de Microsoft Graph API
        // o redirigir al usuario al portal de Microsoft para cambiar su contraseña
        // 
        // Este endpoint es principalmente para mantener compatibilidad con el frontend
        // pero debería implementarse la integración con Microsoft Graph API

        return Result<Unit>.Failure("El cambio de contraseña debe realizarse a través del portal de Microsoft. La autenticación está gestionada por Azure Entra ID.");
    }
}

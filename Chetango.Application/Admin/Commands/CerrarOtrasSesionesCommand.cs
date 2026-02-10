// ============================================
// CERRAR OTRAS SESIONES COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Admin.Commands;

public record CerrarOtrasSesionesCommand(
    Guid IdUsuario
) : IRequest<Result<Unit>>;

public class CerrarOtrasSesionesCommandHandler : IRequestHandler<CerrarOtrasSesionesCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(CerrarOtrasSesionesCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implementar manejo de sesiones activas
        // Esto requeriría una tabla de SesionesActivas o integración con el sistema de tokens
        // Por ahora retornamos éxito sin hacer nada
        
        await Task.CompletedTask; // Placeholder

        return Result<Unit>.Success(Unit.Value);
    }
}

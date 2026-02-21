using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Pagos.Commands;

public record EliminarPagoCommand(
    Guid IdPago,
    string EmailUsuarioEliminador
) : IRequest<Result<Unit>>;

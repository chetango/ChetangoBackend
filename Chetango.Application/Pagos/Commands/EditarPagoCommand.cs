using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Pagos.Commands;

public record EditarPagoCommand(
    Guid IdPago,
    decimal MontoTotal,
    Guid IdMetodoPago,
    string? Nota
) : IRequest<Result<Unit>>;

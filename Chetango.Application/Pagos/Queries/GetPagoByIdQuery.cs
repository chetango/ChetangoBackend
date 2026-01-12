using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;

namespace Chetango.Application.Pagos.Queries;

public record GetPagoByIdQuery(
    Guid IdPago,
    string EmailUsuario,
    bool EsAdmin
) : IRequest<Result<PagoDetalleDTO>>;

using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;

namespace Chetango.Application.Pagos.Queries;

public record GetMisPagosQuery(
    string EmailUsuario,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    Guid? IdMetodoPago,
    int PageNumber,
    int PageSize
) : IRequest<Result<PaginatedList<PagoDTO>>>;

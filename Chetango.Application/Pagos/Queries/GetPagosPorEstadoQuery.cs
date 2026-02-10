using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;

namespace Chetango.Application.Pagos.Queries;

public record GetPagosPorEstadoQuery(
    string NombreEstado, // "Pendiente Verificación", "Verificado", "Rechazado"
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int PageNumber,
    int PageSize
) : IRequest<Result<PaginatedList<PagoDTO>>>;

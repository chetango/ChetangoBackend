using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;

namespace Chetango.Application.Pagos.Queries;

public record GetPagosDeAlumnoQuery(
    Guid IdAlumno,
    string EmailUsuario,
    bool EsAdmin,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    Guid? IdMetodoPago,
    int PageNumber,
    int PageSize
) : IRequest<Result<PaginatedList<PagoDTO>>>;

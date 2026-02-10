using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;
using MediatR;

namespace Chetango.Application.Nomina.Queries;

public record GetClasesPorProfesorQuery(
    Guid IdProfesor,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    string? EstadoPago // null = todas, "Pendiente", "Aprobado", "Liquidado", "Pagado"
) : IRequest<Result<List<ClaseProfesorDTO>>>;

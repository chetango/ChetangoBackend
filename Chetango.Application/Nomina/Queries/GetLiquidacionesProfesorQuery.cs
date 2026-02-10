using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Nomina.Queries;

public record GetLiquidacionesProfesorQuery(
    Guid IdProfesor,
    int? Año = null
) : IRequest<Result<List<LiquidacionResumenDTO>>>;

public record LiquidacionResumenDTO(
    Guid IdLiquidacion,
    int Mes,
    int Año,
    int TotalClases,
    decimal TotalHoras,
    decimal TotalPagar,
    string Estado,
    DateTime? FechaPago,
    string? Observaciones
);

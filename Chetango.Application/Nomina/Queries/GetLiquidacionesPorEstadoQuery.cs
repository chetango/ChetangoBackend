using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Nomina.Queries;

public record GetLiquidacionesPorEstadoQuery(
    string? Estado = null,
    int? Año = null
) : IRequest<Result<List<LiquidacionConProfesorDTO>>>;

public record LiquidacionConProfesorDTO(
    Guid IdLiquidacion,
    Guid IdProfesor,
    string NombreProfesor,
    int Mes,
    int Año,
    int TotalClases,
    decimal TotalHoras,
    decimal TotalPagar,
    string Estado,
    DateTime? FechaPago,
    string? Observaciones
);

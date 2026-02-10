using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;

namespace Chetango.Application.Nomina.Queries;

public record GetLiquidacionMensualQuery(
    Guid? IdLiquidacion,
    Guid? IdProfesor,
    int? Mes,
    int? Año
) : IRequest<Result<LiquidacionDetalleDTO>>;

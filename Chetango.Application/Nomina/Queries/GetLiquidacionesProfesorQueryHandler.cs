using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Nomina.Queries;

public class GetLiquidacionesProfesorQueryHandler : IRequestHandler<GetLiquidacionesProfesorQuery, Result<List<LiquidacionResumenDTO>>>
{
    private readonly IAppDbContext _db;

    public GetLiquidacionesProfesorQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<LiquidacionResumenDTO>>> Handle(GetLiquidacionesProfesorQuery request, CancellationToken cancellationToken)
    {
        var query = _db.LiquidacionesMensuales
            .Where(l => l.IdProfesor == request.IdProfesor);

        if (request.Año.HasValue)
        {
            query = query.Where(l => l.Año == request.Año.Value);
        }

        var liquidaciones = await query
            .OrderByDescending(l => l.Año)
            .ThenByDescending(l => l.Mes)
            .Select(l => new LiquidacionResumenDTO(
                l.IdLiquidacion,
                l.Mes,
                l.Año,
                l.TotalClases,
                l.TotalHoras,
                l.TotalPagar,
                l.Estado,
                l.FechaPago,
                l.Observaciones
            ))
            .ToListAsync(cancellationToken);

        return Result<List<LiquidacionResumenDTO>>.Success(liquidaciones);
    }
}

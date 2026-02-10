using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Nomina.Queries;

public class GetLiquidacionesPorEstadoQueryHandler : IRequestHandler<GetLiquidacionesPorEstadoQuery, Result<List<LiquidacionConProfesorDTO>>>
{
    private readonly IAppDbContext _db;

    public GetLiquidacionesPorEstadoQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<LiquidacionConProfesorDTO>>> Handle(GetLiquidacionesPorEstadoQuery request, CancellationToken cancellationToken)
    {
        var query = _db.LiquidacionesMensuales
            .Include(l => l.Profesor)
                .ThenInclude(p => p.Usuario)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Estado))
        {
            query = query.Where(l => l.Estado == request.Estado);
        }

        if (request.Año.HasValue)
        {
            query = query.Where(l => l.Año == request.Año.Value);
        }

        var liquidaciones = await query
            .OrderByDescending(l => l.Año)
            .ThenByDescending(l => l.Mes)
            .Select(l => new LiquidacionConProfesorDTO(
                l.IdLiquidacion,
                l.IdProfesor,
                l.Profesor.Usuario.NombreUsuario,
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

        return Result<List<LiquidacionConProfesorDTO>>.Success(liquidaciones);
    }
}

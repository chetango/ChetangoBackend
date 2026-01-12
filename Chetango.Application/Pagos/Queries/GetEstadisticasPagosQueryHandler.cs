using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Pagos.Queries;

public class GetEstadisticasPagosQueryHandler : IRequestHandler<GetEstadisticasPagosQuery, Result<EstadisticasPagosDTO>>
{
    private readonly IAppDbContext _db;

    public GetEstadisticasPagosQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<EstadisticasPagosDTO>> Handle(GetEstadisticasPagosQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<Pago>().AsQueryable();

        // Aplicar filtros de fecha
        if (request.FechaDesde.HasValue)
        {
            query = query.Where(p => p.FechaPago >= request.FechaDesde.Value);
        }

        if (request.FechaHasta.HasValue)
        {
            query = query.Where(p => p.FechaPago <= request.FechaHasta.Value);
        }

        var pagos = await query
            .Include(p => p.MetodoPago)
            .ToListAsync(cancellationToken);

        var totalRecaudado = pagos.Sum(p => p.MontoTotal);
        var cantidadPagos = pagos.Count;
        var promedioMonto = cantidadPagos > 0 ? totalRecaudado / cantidadPagos : 0;

        var desgloseMetodosPago = pagos
            .GroupBy(p => p.MetodoPago.Nombre)
            .Select(g => new DesglosePagoDTO(
                g.Key,
                g.Sum(p => p.MontoTotal),
                g.Count()
            ))
            .OrderByDescending(d => d.TotalRecaudado)
            .ToList();

        var estadisticas = new EstadisticasPagosDTO(
            totalRecaudado,
            cantidadPagos,
            promedioMonto,
            desgloseMetodosPago
        );

        return Result<EstadisticasPagosDTO>.Success(estadisticas);
    }
}

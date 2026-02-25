using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;

namespace Chetango.Application.Finanzas.Queries;

public class GetOtrosGastosQueryHandler : IRequestHandler<GetOtrosGastosQuery, Result<List<OtroGastoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetOtrosGastosQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<OtroGastoDTO>>> Handle(GetOtrosGastosQuery request, CancellationToken cancellationToken)
    {
        var query = _db.OtrosGastos
            .Include(o => o.CategoriaGasto)
            .AsQueryable();

        // Aplicar filtros
        if (request.FechaDesde.HasValue)
        {
            query = query.Where(o => o.Fecha >= request.FechaDesde.Value);
        }

        if (request.FechaHasta.HasValue)
        {
            query = query.Where(o => o.Fecha <= request.FechaHasta.Value);
        }

        if (request.Sede.HasValue)
        {
            query = query.Where(o => o.Sede == request.Sede.Value);
        }

        if (request.IdCategoriaGasto.HasValue)
        {
            query = query.Where(o => o.IdCategoriaGasto == request.IdCategoriaGasto.Value);
        }

        var gastos = await query
            .OrderByDescending(o => o.Fecha)
            .Select(o => new OtroGastoDTO
            {
                IdOtroGasto = o.IdOtroGasto,
                Concepto = o.Concepto,
                Monto = o.Monto,
                Fecha = o.Fecha,
                Sede = o.Sede,
                IdCategoriaGasto = o.IdCategoriaGasto,
                NombreCategoria = o.CategoriaGasto != null ? o.CategoriaGasto.Nombre : null,
                Proveedor = o.Proveedor,
                Descripcion = o.Descripcion,
                UrlFactura = o.UrlFactura,
                NumeroFactura = o.NumeroFactura,
                FechaCreacion = o.FechaCreacion,
                UsuarioCreacion = o.UsuarioCreacion
            })
            .ToListAsync(cancellationToken);

        return Result<List<OtroGastoDTO>>.Success(gastos);
    }
}

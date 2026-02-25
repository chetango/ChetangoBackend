using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;

namespace Chetango.Application.Finanzas.Queries;

public class GetOtrosIngresosQueryHandler : IRequestHandler<GetOtrosIngresosQuery, Result<List<OtroIngresoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetOtrosIngresosQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<OtroIngresoDTO>>> Handle(GetOtrosIngresosQuery request, CancellationToken cancellationToken)
    {
        var query = _db.OtrosIngresos
            .Include(o => o.CategoriaIngreso)
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

        if (request.IdCategoriaIngreso.HasValue)
        {
            query = query.Where(o => o.IdCategoriaIngreso == request.IdCategoriaIngreso.Value);
        }

        var ingresos = await query
            .OrderByDescending(o => o.Fecha)
            .Select(o => new OtroIngresoDTO
            {
                IdOtroIngreso = o.IdOtroIngreso,
                Concepto = o.Concepto,
                Monto = o.Monto,
                Fecha = o.Fecha,
                Sede = o.Sede,
                IdCategoriaIngreso = o.IdCategoriaIngreso,
                NombreCategoria = o.CategoriaIngreso != null ? o.CategoriaIngreso.Nombre : null,
                Descripcion = o.Descripcion,
                UrlComprobante = o.UrlComprobante,
                FechaCreacion = o.FechaCreacion,
                UsuarioCreacion = o.UsuarioCreacion
            })
            .ToListAsync(cancellationToken);

        return Result<List<OtroIngresoDTO>>.Success(ingresos);
    }
}

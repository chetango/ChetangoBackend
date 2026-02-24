using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Queries.GetClases;

public class GetClasesQueryHandler : IRequestHandler<GetClasesQuery, Result<PaginatedList<ClaseDTO>>>
{
    private readonly IAppDbContext _db;

    public GetClasesQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PaginatedList<ClaseDTO>>> Handle(GetClasesQuery request, CancellationToken cancellationToken)
    {
        // Construir query base con todas las relaciones necesarias
        var query = _db.Set<Chetango.Domain.Entities.Clase>()
            .Include(c => c.TipoClase)
            .Include(c => c.ProfesorPrincipal)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Asistencias)
            .AsQueryable();

        // Filtro por rango de fechas
        if (request.FechaDesde.HasValue)
            query = query.Where(c => c.Fecha >= request.FechaDesde.Value.Date);

        if (request.FechaHasta.HasValue)
            query = query.Where(c => c.Fecha <= request.FechaHasta.Value.Date);

        // Filtro por tipo de clase
        if (request.IdTipoClase.HasValue)
            query = query.Where(c => c.IdTipoClase == request.IdTipoClase.Value);

        // Ordenar por fecha descendente, luego por hora
        query = query.OrderByDescending(c => c.Fecha).ThenByDescending(c => c.HoraInicio);

        // Contar total de registros
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginación
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ClaseDTO(
                c.IdClase,
                c.Fecha,
                c.HoraInicio,
                c.HoraFin,
                c.TipoClase.Nombre,
                c.IdProfesorPrincipal,
                c.ProfesorPrincipal.NombreCompleto,
                c.CupoMaximo,
                c.Asistencias.Count,
                c.Estado
            ))
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<ClaseDTO>(items, totalCount, request.PageNumber, request.PageSize);

        return Result<PaginatedList<ClaseDTO>>.Success(paginatedList);
    }
}

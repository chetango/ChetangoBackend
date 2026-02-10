using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Queries.GetClasesDeProfesor;

public class GetClasesDeProfesorQueryHandler : IRequestHandler<GetClasesDeProfesorQuery, Result<PaginatedList<ClaseDTO>>>
{
    private readonly IAppDbContext _db;

    public GetClasesDeProfesorQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PaginatedList<ClaseDTO>>> Handle(GetClasesDeProfesorQuery request, CancellationToken cancellationToken)
    {
        // 1. Validar que el profesor existe
        var profesor = await _db.Set<Chetango.Domain.Entities.Estados.Profesor>()
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.IdProfesor == request.IdProfesor, cancellationToken);

        if (profesor is null)
            return Result<PaginatedList<ClaseDTO>>.Failure("El profesor especificado no existe.");

        // 2. Construir query con filtros
        // Buscar clases donde el profesor esté asignado (Principal o Monitor)
        var query = _db.Set<Chetango.Domain.Entities.Clase>()
            .Include(c => c.TipoClase)
            .Include(c => c.ProfesorPrincipal)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Asistencias)
            .Include(c => c.Profesores) // Incluir todos los profesores asignados
            .Where(c => c.Profesores.Any(cp => cp.IdProfesor == request.IdProfesor));

        // Filtro por rango de fechas
        if (request.FechaDesde.HasValue)
            query = query.Where(c => c.Fecha >= request.FechaDesde.Value.Date);

        if (request.FechaHasta.HasValue)
            query = query.Where(c => c.Fecha <= request.FechaHasta.Value.Date);

        // Ordenar por fecha descendente
        query = query.OrderByDescending(c => c.Fecha).ThenByDescending(c => c.HoraInicio);

        // 3. Contar total de registros
        var totalCount = await query.CountAsync(cancellationToken);

        // 4. Aplicar paginación
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

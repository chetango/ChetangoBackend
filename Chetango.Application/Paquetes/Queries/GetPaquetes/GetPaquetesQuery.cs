using Chetango.Application.Common;
using Chetango.Application.Paquetes.DTOs;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Queries.GetPaquetes;

// Query para obtener TODOS los paquetes del sistema (vista administrativa)
public record GetPaquetesQuery(
    string? BusquedaAlumno = null, // Buscar por nombre o documento de alumno
    int? Estado = null,
    Guid? IdTipoPaquete = null,
    DateTime? FechaVencimientoDesde = null,
    DateTime? FechaVencimientoHasta = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedList<PaqueteAlumnoDTO>>>;

// Clase para la lista paginada (si no existe en Common)
public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<T>
        {
            Items = items,
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            TotalCount = count
        };
    }
}

// Handler
public class GetPaquetesQueryHandler : IRequestHandler<GetPaquetesQuery, Result<PaginatedList<PaqueteAlumnoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetPaquetesQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PaginatedList<PaqueteAlumnoDTO>>> Handle(GetPaquetesQuery request, CancellationToken cancellationToken)
    {
        // Construir query base con todas las relaciones necesarias
        var query = _db.Set<Paquete>()
            .Include(p => p.TipoPaquete)
            .Include(p => p.Alumno)
                .ThenInclude(a => a.Usuario)
            .AsQueryable();

        // Filtro por búsqueda de alumno (nombre o documento)
        if (!string.IsNullOrWhiteSpace(request.BusquedaAlumno))
        {
            var busqueda = request.BusquedaAlumno.Trim().ToLower();
            query = query.Where(p =>
                p.Alumno.Usuario.NombreUsuario.ToLower().Contains(busqueda) ||
                p.Alumno.Usuario.NumeroDocumento.Contains(busqueda)
            );
        }

        // Filtro por estado
        if (request.Estado.HasValue)
            query = query.Where(p => p.IdEstado == request.Estado.Value);

        // Filtro por tipo de paquete
        if (request.IdTipoPaquete.HasValue)
            query = query.Where(p => p.IdTipoPaquete == request.IdTipoPaquete.Value);

        // Filtro por fecha de vencimiento
        if (request.FechaVencimientoDesde.HasValue)
            query = query.Where(p => p.FechaVencimiento >= request.FechaVencimientoDesde.Value);

        if (request.FechaVencimientoHasta.HasValue)
            query = query.Where(p => p.FechaVencimiento <= request.FechaVencimientoHasta.Value);

        // Ordenar por fecha de vencimiento descendente (más recientes primero)
        query = query.OrderByDescending(p => p.FechaVencimiento);

        // Proyectar a DTO
        var dtoQuery = query
            .AsNoTracking()
            .Select(p => new PaqueteAlumnoDTO(
                p.IdPaquete,
                p.IdAlumno,
                p.Alumno.Usuario.NombreUsuario,
                p.Alumno.Usuario.NumeroDocumento,
                p.TipoPaquete.Nombre,
                p.ClasesDisponibles,
                p.ClasesUsadas,
                p.ClasesDisponibles - p.ClasesUsadas,
                p.FechaActivacion,
                p.FechaVencimiento,
                p.ValorPaquete,
                // Lógica de estado basada en IdEstado de la BD
                p.IdEstado == 2 ? "Vencido" : 
                p.IdEstado == 3 ? "Congelado" : 
                p.IdEstado == 4 ? "Agotado" : 
                "Activo",
                p.FechaVencimiento < DateTime.Today,
                (p.ClasesDisponibles - p.ClasesUsadas) > 0,
                null,
                null
            ));

        // Paginar
        var paginatedList = await PaginatedList<PaqueteAlumnoDTO>.CreateAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );

        return Result<PaginatedList<PaqueteAlumnoDTO>>.Success(paginatedList);
    }
}

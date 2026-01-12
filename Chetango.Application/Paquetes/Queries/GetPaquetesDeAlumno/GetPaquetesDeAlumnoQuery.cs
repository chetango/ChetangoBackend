using Chetango.Application.Common;
using Chetango.Application.Paquetes.DTOs;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Queries.GetPaquetesDeAlumno;

// Query para obtener los paquetes de un alumno con paginación
public record GetPaquetesDeAlumnoQuery(
    Guid IdAlumno,
    bool SoloActivos = true,
    int? Estado = null,
    Guid? IdTipoPaquete = null,
    DateTime? FechaVencimientoDesde = null,
    DateTime? FechaVencimientoHasta = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? CorreoUsuarioActual = null,
    bool EsAdmin = false
) : IRequest<Result<PaginatedList<PaqueteAlumnoDTO>>>;

// Clase para la lista paginada
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
public class GetPaquetesDeAlumnoQueryHandler : IRequestHandler<GetPaquetesDeAlumnoQuery, Result<PaginatedList<PaqueteAlumnoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetPaquetesDeAlumnoQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PaginatedList<PaqueteAlumnoDTO>>> Handle(GetPaquetesDeAlumnoQuery request, CancellationToken cancellationToken)
    {
        // 1. Validar que el alumno existe
        var alumno = await _db.Set<Chetango.Domain.Entities.Alumno>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (alumno is null)
            return Result<PaginatedList<PaqueteAlumnoDTO>>.Failure("El alumno especificado no existe.");

        // 2. Validar ownership: si no es admin, verificar que está consultando sus propios paquetes
        if (!request.EsAdmin && !string.IsNullOrEmpty(request.CorreoUsuarioActual))
        {
            var correoAlumno = await _db.Set<Usuario>()
                .Where(u => u.IdUsuario == alumno.IdUsuario)
                .Select(u => u.Correo)
                .FirstOrDefaultAsync(cancellationToken);

            if (!string.Equals(correoAlumno, request.CorreoUsuarioActual, StringComparison.OrdinalIgnoreCase))
                return Result<PaginatedList<PaqueteAlumnoDTO>>.Failure("No tienes permiso para ver los paquetes de otro alumno.");
        }

        // 3. Construir query base
        var query = _db.Set<Paquete>()
            .Include(p => p.TipoPaquete)
            .Include(p => p.Alumno)
                .ThenInclude(a => a.Usuario)
            .Where(p => p.IdAlumno == request.IdAlumno);

        // 4. Aplicar filtros
        if (request.SoloActivos)
            query = query.Where(p => p.IdEstado == 1); // 1 = Activo

        if (request.Estado.HasValue)
            query = query.Where(p => p.IdEstado == request.Estado.Value);

        if (request.IdTipoPaquete.HasValue)
            query = query.Where(p => p.IdTipoPaquete == request.IdTipoPaquete.Value);

        if (request.FechaVencimientoDesde.HasValue)
            query = query.Where(p => p.FechaVencimiento >= request.FechaVencimientoDesde.Value);

        if (request.FechaVencimientoHasta.HasValue)
            query = query.Where(p => p.FechaVencimiento <= request.FechaVencimientoHasta.Value);

        // 5. Ordenar por fecha de vencimiento descendente
        query = query.OrderByDescending(p => p.FechaVencimiento);

        // 6. Proyectar a DTO
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
                p.IdEstado == 1 ? "Activo" : p.IdEstado == 3 ? "Congelado" : p.ClasesUsadas >= p.ClasesDisponibles ? "Completado" : "Vencido",
                p.FechaVencimiento < DateTime.Today,
                (p.ClasesDisponibles - p.ClasesUsadas) > 0,
                null,
                null
            ));

        // 7. Paginar
        var paginatedList = await PaginatedList<PaqueteAlumnoDTO>.CreateAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );

        return Result<PaginatedList<PaqueteAlumnoDTO>>.Success(paginatedList);
    }
}

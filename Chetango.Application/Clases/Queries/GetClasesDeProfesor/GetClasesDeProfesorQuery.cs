using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Queries.GetClasesDeProfesor;

// Query para obtener las clases de un profesor con filtros y paginación
public record GetClasesDeProfesorQuery(
    Guid IdProfesor,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int PageNumber,
    int PageSize,
    string? IdUsuarioActual, // Para validación de ownership
    bool EsAdmin // Para bypass de ownership validation
) : IRequest<Result<PaginatedList<ClaseDTO>>>;

// Clase auxiliar para paginación
public class PaginatedList<T>
{
    public List<T> Items { get; init; } = new();
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}

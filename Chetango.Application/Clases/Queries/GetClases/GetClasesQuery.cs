using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Queries.GetClases;

// Query para obtener TODAS las clases del sistema con filtros y paginación (Admin)
public record GetClasesQuery(
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    Guid? IdTipoClase = null,
    int PageNumber = 1,
    int PageSize = 100
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

using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Queries.GetClaseById;

// Query para obtener el detalle de una clase por ID
public record GetClaseByIdQuery(
    Guid IdClase,
    string? IdUsuarioActual, // Para validación de ownership
    bool EsAdmin // Para bypass de ownership validation
) : IRequest<Result<ClaseDetalleDTO>>;

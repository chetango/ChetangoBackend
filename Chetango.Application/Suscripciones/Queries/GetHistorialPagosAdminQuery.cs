using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Query para obtener historial de pagos con filtros (SuperAdmin).
/// </summary>
public record GetHistorialPagosAdminQuery(
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    string? Estado = null
) : IRequest<Result<IReadOnlyList<PagoSuscripcionDto>>>;

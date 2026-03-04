using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Query para obtener el historial de pagos de suscripción de un tenant.
/// </summary>
public record GetHistorialPagosQuery(
    Guid TenantId
) : IRequest<Result<List<PagoSuscripcionDto>>>;

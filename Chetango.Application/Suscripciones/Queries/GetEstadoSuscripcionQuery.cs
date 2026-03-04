using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Query para obtener el estado actual de la suscripción de un tenant.
/// </summary>
public record GetEstadoSuscripcionQuery(
    Guid TenantId
) : IRequest<Result<EstadoSuscripcionDto>>;

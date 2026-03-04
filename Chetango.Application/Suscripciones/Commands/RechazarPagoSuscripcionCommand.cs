using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Suscripciones.Commands;

/// <summary>
/// Comando para rechazar un pago de suscripción.
/// Solo para SuperAdmin.
/// </summary>
public record RechazarPagoSuscripcionCommand(
    Guid PagoId,
    string RechazadoPor,
    string MotivoRechazo
) : IRequest<Result<bool>>;

using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Suscripciones.Commands;

/// <summary>
/// Comando para aprobar un pago de suscripción.
/// Solo para SuperAdmin.
/// </summary>
public record AprobarPagoSuscripcionCommand(
    Guid PagoId,
    string AprobadoPor,
    string? Observaciones
) : IRequest<Result<bool>>;

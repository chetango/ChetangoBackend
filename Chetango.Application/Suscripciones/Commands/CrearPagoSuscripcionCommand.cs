using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Suscripciones.Commands;

/// <summary>
/// Comando para crear un pago de suscripción (registrar comprobante).
/// </summary>
public record CrearPagoSuscripcionCommand(
    Guid TenantId,
    DateTime FechaPago,
    decimal Monto,
    string Referencia,
    string MetodoPago,
    string? ComprobanteUrl,
    string? NombreArchivo,
    int? TamanoArchivo,
    string EmailUsuarioCreador
) : IRequest<Result<Guid>>; // Retorna el ID del pago creado

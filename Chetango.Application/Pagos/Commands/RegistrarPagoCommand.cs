using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Domain.Enums;

namespace Chetango.Application.Pagos.Commands;

public record RegistrarPagoCommand(
    Guid? IdAlumno, // Opcional: solo para pagos de un alumno. Si null, se obtiene de los paquetes
    DateTime FechaPago,
    decimal MontoTotal,
    Guid IdMetodoPago,
    string? ReferenciaTransferencia,
    string? UrlComprobante,
    string? Nota,
    List<PaqueteParaCrearDTO> Paquetes,
    List<Guid>? IdsPaquetesExistentes = null, // IDs de paquetes a vincular
    Sede? Sede = null, // Opcional: si null, se hereda del usuario logueado
    string? EmailUsuarioCreador = null // Email del usuario que crea el pago
) : IRequest<Result<RegistrarPagoResponseDTO>>;


using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;

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
    List<Guid>? IdsPaquetesExistentes = null // IDs de paquetes a vincular
) : IRequest<Result<RegistrarPagoResponseDTO>>;


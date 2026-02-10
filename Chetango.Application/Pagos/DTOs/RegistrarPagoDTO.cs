namespace Chetango.Application.Pagos.DTOs;

public record RegistrarPagoDTO(
    Guid? IdAlumno, // Opcional: solo para pagos de un único alumno
    DateTime FechaPago,
    decimal MontoTotal,
    Guid IdMetodoPago,
    string? ReferenciaTransferencia,
    string? UrlComprobante,
    string? Nota,
    List<PaqueteParaCrearDTO> Paquetes,
    List<Guid>? IdsPaquetesExistentes = null
);


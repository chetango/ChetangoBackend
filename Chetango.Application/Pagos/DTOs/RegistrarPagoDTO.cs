namespace Chetango.Application.Pagos.DTOs;

public record RegistrarPagoDTO(
    Guid IdAlumno,
    DateTime FechaPago,
    decimal MontoTotal,
    Guid IdMetodoPago,
    string? Nota,
    List<PaqueteParaCrearDTO> Paquetes
);

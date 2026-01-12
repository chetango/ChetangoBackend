namespace Chetango.Application.Pagos.DTOs;

public record PagoDetalleDTO(
    Guid IdPago,
    Guid IdAlumno,
    string NombreAlumno,
    string CorreoAlumno,
    DateTime FechaPago,
    decimal MontoTotal,
    Guid IdMetodoPago,
    string NombreMetodoPago,
    string? Nota,
    DateTime FechaCreacion,
    List<PaqueteResumenDTO> Paquetes
);

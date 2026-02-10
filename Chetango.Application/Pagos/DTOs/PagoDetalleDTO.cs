namespace Chetango.Application.Pagos.DTOs;

public record PagoDetalleDTO(
    Guid IdPago,
    Guid? IdAlumno,
    string? NombreAlumno,
    string? CorreoAlumno,
    string? TelefonoAlumno,
    string? FotoUrlAlumno,
    DateTime FechaPago,
    decimal MontoTotal,
    Guid IdMetodoPago,
    string NombreMetodoPago,
    string? ReferenciaTransferencia,
    string? Nota,
    string EstadoPago,
    string? UrlComprobante,
    string? NotasVerificacion,
    DateTime? FechaVerificacion,
    string? UsuarioVerificacion,
    DateTime FechaCreacion,
    List<PaqueteResumenDTO> Paquetes
);


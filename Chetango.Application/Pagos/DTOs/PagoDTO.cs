namespace Chetango.Application.Pagos.DTOs;

public record PagoDTO(
    Guid IdPago,
    DateTime FechaPago,
    decimal MontoTotal,
    string NombreMetodoPago,
    string NombreAlumno,
    string EstadoPago,
    string? UrlComprobante,
    string? ReferenciaTransferencia,
    string? NotasVerificacion,
    DateTime? FechaVerificacion,
    string? UsuarioVerificacion,
    int CantidadPaquetes
);



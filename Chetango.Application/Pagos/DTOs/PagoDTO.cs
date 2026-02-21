namespace Chetango.Application.Pagos.DTOs;

using Chetango.Domain.Enums;

public record PagoDTO(
    Guid IdPago,
    Guid IdAlumno,
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
    int CantidadPaquetes,
    Sede Sede,
    string SedeNombre
);



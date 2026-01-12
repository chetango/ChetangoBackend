namespace Chetango.Application.Pagos.DTOs;

public record PagoDTO(
    Guid IdPago,
    DateTime FechaPago,
    decimal MontoTotal,
    string NombreMetodoPago,
    string NombreAlumno,
    int CantidadPaquetes
);

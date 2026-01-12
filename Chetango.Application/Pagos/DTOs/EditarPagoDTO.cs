namespace Chetango.Application.Pagos.DTOs;

public record EditarPagoDTO(
    decimal MontoTotal,
    Guid IdMetodoPago,
    string? Nota
);

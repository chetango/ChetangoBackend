namespace Chetango.Application.Pagos.DTOs;

public record MetodoPagoDTO(
    Guid IdMetodoPago,
    string Nombre,
    string? Descripcion
);

namespace Chetango.Application.Pagos.DTOs;

public record RegistrarPagoResponseDTO(
    Guid IdPago,
    List<Guid> IdPaquetesCreados,
    decimal MontoTotal
);

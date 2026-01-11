namespace Chetango.Application.Paquetes.DTOs;

// DTO para información de congelación activa
public record CongelacionDetalleDTO(
    Guid IdCongelacion,
    DateTime FechaInicio,
    DateTime FechaFin,
    int DiasCongelados
);

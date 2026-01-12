namespace Chetango.Application.Paquetes.DTOs;

// DTO para representar una congelación de paquete
public record CongelacionDTO(
    Guid IdCongelacion,
    DateTime FechaInicio,
    DateTime FechaFin,
    int DiasCongelados
);

namespace Chetango.Application.Paquetes.DTOs;

// DTO para congelar un paquete
public record CongelarPaqueteDTO(
    Guid IdPaquete,
    DateTime FechaInicio,
    DateTime FechaFin,
    string? Motivo = null
);

namespace Chetango.Application.Paquetes.DTOs;

// DTO para editar un paquete existente
public record EditarPaqueteDTO(
    Guid IdPaquete,
    int ClasesDisponibles,
    DateTime FechaVencimiento
);

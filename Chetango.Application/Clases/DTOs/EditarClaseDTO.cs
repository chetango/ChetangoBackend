namespace Chetango.Application.Clases.DTOs;

// DTO para editar una clase existente
public record EditarClaseDTO(
    Guid IdTipoClase,
    Guid IdProfesor,
    DateTime FechaHoraInicio,
    int DuracionMinutos,
    int CupoMaximo,
    string? Observaciones
);

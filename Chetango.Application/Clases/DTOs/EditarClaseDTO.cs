namespace Chetango.Application.Clases.DTOs;

// DTO para especificar profesor con su rol
public record ProfesorClaseRequestDTO(
    Guid IdProfesor,
    string RolEnClase // "Principal" | "Monitor"
);

// DTO para editar una clase existente
public record EditarClaseDTO(
    Guid IdTipoClase,
    List<ProfesorClaseRequestDTO> Profesores, // Lista de profesores con sus roles
    DateTime FechaHoraInicio,
    int DuracionMinutos,
    int CupoMaximo,
    string? Observaciones
);

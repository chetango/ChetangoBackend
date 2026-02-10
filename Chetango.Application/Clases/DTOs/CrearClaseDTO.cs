namespace Chetango.Application.Clases.DTOs;

// DTO para crear una nueva clase con soporte para múltiples profesores
public record CrearClaseDTO(
    // NUEVO: Sistema de múltiples profesores con roles
    List<ProfesorClaseRequestDTO>? Profesores, // Lista de profesores con sus roles
    
    // DEPRECATED: Sistema antiguo (mantener para retrocompatibilidad)
    Guid? IdProfesorPrincipal, // Nullable para compatibilidad con nuevo sistema
    List<Guid>? IdsMonitores, // IDs de profesores monitores (opcional)
    
    // Datos de la clase
    Guid IdTipoClase,
    DateTime Fecha,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    int CupoMaximo,
    string? Observaciones
);

// DTO auxiliar para especificar profesor y su rol
public record ProfesorClaseRequestDTO(
    Guid IdProfesor,
    string RolEnClase // "Principal" o "Monitor"
);

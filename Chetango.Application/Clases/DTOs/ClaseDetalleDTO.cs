namespace Chetango.Application.Clases.DTOs;

// DTO para representar el detalle completo de una clase
public record ClaseDetalleDTO(
    Guid IdClase,
    DateTime Fecha,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    string TipoClase,
    Guid? IdProfesorPrincipal, // NULLABLE: Retrocompatibilidad - usar Profesores en su lugar
    string NombreProfesor, // Retrocompatibilidad - usar Profesores en su lugar
    int CupoMaximo,
    string? Observaciones,
    int TotalAsistencias,
    List<MonitorClaseDTO> Monitores, // DEPRECATED: usar Profesores con rol Monitor
    List<ProfesorClaseDTO> Profesores, // NUEVO: Lista completa de profesores con sus roles
    string Estado
);

// DTO para profesor con su rol en una clase
public record ProfesorClaseDTO(
    Guid IdProfesor,
    string NombreProfesor,
    string RolEnClase // "Principal" o "Monitor"
);

public record MonitorClaseDTO(
    Guid IdProfesor,
    string NombreProfesor
);

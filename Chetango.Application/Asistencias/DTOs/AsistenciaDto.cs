namespace Chetango.Application.Asistencias.DTOs;

// DTO para representar una asistencia con datos proyectados de alumno y clase
public record AsistenciaDto(
    Guid IdAsistencia,
    Guid IdClase,
    DateTime FechaClase,
    string HoraInicio,
    string HoraFin,
    string TipoClase,
    Guid IdAlumno,
    string NombreAlumno,
    string EstadoAsistencia,
    Guid? IdPaqueteUsado, // Nullable: null = clase sin paquete
    int IdTipoAsistencia, // Tipo de asistencia (Normal, Cortesía, Clase de Prueba, Recuperación)
    string TipoAsistencia, // Nombre del tipo de asistencia
    string? Observacion
);

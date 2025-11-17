namespace Chetango.Application.Asistencias.DTOs;

// DTO para representar una asistencia con datos proyectados de alumno y clase
public record AsistenciaDto(
    Guid IdAsistencia,
    Guid IdClase,
    DateTime FechaClase,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    string TipoClase,
    Guid IdAlumno,
    string NombreAlumno,
    string EstadoAsistencia,
    Guid IdPaqueteUsado,
    string? Observacion
);

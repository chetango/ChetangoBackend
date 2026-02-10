namespace Chetango.Application.Asistencias.DTOs;

/// <summary>
/// DTO que representa una asistencia marcada por el profesor/admin
/// pero que aún no ha sido confirmada por el alumno
/// </summary>
public record AsistenciaPendienteConfirmacionDto(
    Guid IdAsistencia,
    Guid IdClase,
    string NombreClase,
    DateTime FechaClase,
    string HoraInicio,
    string HoraFin,
    List<string> Profesores, // Lista de nombres de profesores
    DateTime FechaRegistro, // Cuándo fue marcada por el profesor/admin
    string? Observacion
);

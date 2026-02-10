namespace Chetango.Application.Asistencias.Queries.GetAsistenciasPendientesConfirmar;

/// <summary>
/// DTO para una asistencia pendiente de confirmar por el alumno.
/// Contiene información de la clase para mostrar en la notificación.
/// </summary>
public record AsistenciaPendienteDto(
    Guid IdAsistencia,
    Guid IdClase,
    string NombreClase,
    DateTime FechaClase,
    string HoraInicio,
    string HoraFin,
    List<string> Profesores // Lista de nombres de profesores
);

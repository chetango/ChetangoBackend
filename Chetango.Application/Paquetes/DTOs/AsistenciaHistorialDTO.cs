namespace Chetango.Application.Paquetes.DTOs;

// DTO para el historial de uso de un paquete (asistencias)
public record AsistenciaHistorialDTO(
    Guid IdAsistencia,
    DateTime FechaClase,
    string NombreClase,
    string HoraInicio,
    string HoraFin,
    string Resultado,      // "Asistida", "Ausente", "Tardanza"
    string Impacto,        // "Descontada", "No descontada", "Cortesía"
    string? Nota
);

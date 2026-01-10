namespace Chetango.Application.Clases.DTOs;

// DTO para representar el detalle completo de una clase
public record ClaseDetalleDTO(
    Guid IdClase,
    DateTime Fecha,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    string TipoClase,
    Guid IdProfesorPrincipal,
    string NombreProfesor,
    int CupoMaximo,
    string? Observaciones,
    int TotalAsistencias,
    List<MonitorClaseDTO> Monitores
);

public record MonitorClaseDTO(
    Guid IdProfesor,
    string NombreProfesor
);

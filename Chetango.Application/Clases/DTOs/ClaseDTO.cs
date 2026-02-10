namespace Chetango.Application.Clases.DTOs;

// DTO para representar una clase en listados
public record ClaseDTO(
    Guid IdClase,
    DateTime Fecha,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    string TipoClase,
    Guid? IdProfesorPrincipal, // NULLABLE: Soporte para clases con múltiples profesores sin principal único
    string NombreProfesor,
    int CupoMaximo,
    int TotalAsistencias,
    string Estado
);

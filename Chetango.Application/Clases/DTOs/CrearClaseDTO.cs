namespace Chetango.Application.Clases.DTOs;

// DTO para crear una nueva clase
public record CrearClaseDTO(
    Guid IdProfesorPrincipal,
    Guid IdTipoClase,
    DateTime Fecha,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    int CupoMaximo,
    string? Observaciones
);

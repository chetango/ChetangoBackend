namespace Chetango.Application.Clases.DTOs;

// DTO para representar un alumno en listados
public record AlumnoDTO(
    Guid IdAlumno,
    Guid IdUsuario,
    string Nombre,
    string Correo,
    string? NumeroDocumento = null,
    string? Telefono = null
);

namespace Chetango.Application.Clases.DTOs;

// DTO para representar un profesor en listados
public record ProfesorDTO(
    Guid IdProfesor,
    Guid IdUsuario,
    string Nombre,
    string Correo
);

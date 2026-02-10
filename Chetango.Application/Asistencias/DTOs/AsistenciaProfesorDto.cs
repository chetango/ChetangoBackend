namespace Chetango.Application.Asistencias.DTOs;

/// <summary>
/// DTO para representar asistencias desde la vista del profesor
/// Incluye TODOS los alumnos activos, con o sin asistencia registrada
/// </summary>
/// <param name="IdAsistencia">ID de asistencia (null si no hay registro previo)</param>
/// <param name="IdAlumno">ID del alumno</param>
/// <param name="NombreAlumno">Nombre completo del alumno</param>
/// <param name="DocumentoIdentidad">Documento de identidad del alumno</param>
/// <param name="Presente">Estado de asistencia (false por defecto si no hay registro)</param>
/// <param name="Observacion">Observaciones del profesor</param>
/// <param name="EstadoPaquete">Estado del paquete ('Activo', 'Agotado', 'Congelado', 'SinPaquete')</param>
/// <param name="ClasesRestantes">Clases restantes en el paquete activo (null si no tiene paquete)</param>
/// <param name="IdPaquete">ID del paquete activo (null si no tiene)</param>
public record AsistenciaProfesorDto(
    Guid? IdAsistencia,
    Guid IdAlumno,
    string NombreAlumno,
    string DocumentoIdentidad,
    bool Presente,
    string? Observacion,
    string EstadoPaquete,
    int? ClasesRestantes,
    Guid? IdPaquete
);

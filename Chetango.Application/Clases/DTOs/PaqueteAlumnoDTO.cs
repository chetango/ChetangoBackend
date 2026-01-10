namespace Chetango.Application.Clases.DTOs;

// DTO para representar un paquete de un alumno
public record PaqueteAlumnoDTO(
    Guid IdPaquete,
    int ClasesDisponibles,
    int ClasesUsadas,
    int ClasesRestantes,
    DateTime FechaVencimiento,
    string Estado,
    bool EstaVencido,
    bool TieneClasesDisponibles
);

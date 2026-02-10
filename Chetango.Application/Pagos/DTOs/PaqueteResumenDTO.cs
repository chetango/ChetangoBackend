namespace Chetango.Application.Pagos.DTOs;

public record PaqueteResumenDTO(
    Guid IdPaquete,
    Guid IdAlumno,
    string NombreAlumno,
    string NombreTipoPaquete,
    int ClasesDisponibles,
    int ClasesUsadas,
    int ClasesRestantes,
    DateTime? FechaVencimiento,
    string Estado,
    decimal ValorPaquete
);

namespace Chetango.Application.Paquetes.DTOs;

// DTO para representar un paquete de un alumno
public record PaqueteAlumnoDTO(
    Guid IdPaquete,
    Guid IdAlumno,
    string NombreAlumno,
    string DocumentoAlumno,
    string NombreTipoPaquete,
    int ClasesDisponibles,
    int ClasesUsadas,
    int ClasesRestantes,
    DateTime FechaActivacion,
    DateTime FechaVencimiento,
    decimal ValorPaquete,
    string Estado,
    bool EstaVencido,
    bool TieneClasesDisponibles,
    CongelacionDetalleDTO? CongelacionActiva = null,
    List<AsistenciaHistorialDTO>? HistorialUso = null
);

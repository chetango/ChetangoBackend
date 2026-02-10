namespace Chetango.Application.Paquetes.DTOs;

// DTO para representar el detalle completo de un paquete
public record PaqueteDetalleDTO(
    Guid IdPaquete,
    Guid IdAlumno,
    string NombreAlumno,
    Guid IdTipoPaquete,
    string NombreTipoPaquete,
    int ClasesDisponibles,
    int ClasesUsadas,
    int ClasesRestantes,
    DateTime FechaActivacion,
    DateTime FechaVencimiento,
    decimal ValorPaquete,
    int IdEstado,
    string Estado,
    bool EstaVencido,
    bool TieneClasesDisponibles,
    List<CongelacionDTO> Congelaciones,
    List<AlumnoPaqueteDTO>? AlumnosDelPago = null
);

// DTO para representar un alumno en el mismo pago (para paquetes compartidos)
public record AlumnoPaqueteDTO(
    Guid IdAlumno,
    string NombreAlumno
);

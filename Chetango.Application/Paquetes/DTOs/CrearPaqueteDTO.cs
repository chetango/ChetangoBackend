namespace Chetango.Application.Paquetes.DTOs;

// DTO para crear un nuevo paquete
public record CrearPaqueteDTO(
    Guid IdAlumno,
    Guid IdTipoPaquete,
    int ClasesDisponibles,
    decimal ValorPaquete,
    int DiasVigencia,
    Guid? IdPago = null
);

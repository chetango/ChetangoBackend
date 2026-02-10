namespace Chetango.Application.Paquetes.DTOs;

public record PaqueteSinPagoDTO(
    Guid IdPaquete,
    string NombreTipoPaquete,
    int ClasesDisponibles,
    int ClasesUsadas,
    decimal ValorPaquete,
    DateTime FechaActivacion,
    DateTime FechaVencimiento,
    string Estado
);

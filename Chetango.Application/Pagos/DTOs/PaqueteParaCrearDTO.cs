namespace Chetango.Application.Pagos.DTOs;

public record PaqueteParaCrearDTO(
    Guid IdTipoPaquete,
    int ClasesDisponibles,
    int DiasVigencia,
    decimal? ValorPaquete = null
);

namespace Chetango.Application.Nomina.DTOs;

public record AprobarPagoClaseRequest(
    Guid IdClaseProfesor,
    decimal? ValorAdicional,
    string? ConceptoAdicional
);

public record LiquidarMesRequest(
    Guid IdProfesor,
    int Mes,
    int Año,
    string? Observaciones
);

public record RegistrarPagoProfesorRequest(
    Guid IdLiquidacion,
    DateTime FechaPago,
    string? Observaciones
);

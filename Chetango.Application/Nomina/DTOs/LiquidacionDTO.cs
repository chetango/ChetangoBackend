namespace Chetango.Application.Nomina.DTOs;

public record LiquidacionMensualDTO(
    Guid IdLiquidacion,
    Guid IdProfesor,
    string NombreProfesor,
    int Mes,
    int Año,
    int TotalClases,
    decimal TotalHoras,
    decimal TotalBase,
    decimal TotalAdicionales,
    decimal TotalPagar,
    string Estado,
    DateTime? FechaCierre,
    DateTime? FechaPago,
    string? Observaciones,
    DateTime FechaCreacion
);

public record LiquidacionDetalleDTO(
    Guid IdLiquidacion,
    Guid IdProfesor,
    string NombreProfesor,
    int Mes,
    int Año,
    int TotalClases,
    decimal TotalHoras,
    decimal TotalBase,
    decimal TotalAdicionales,
    decimal TotalPagar,
    string Estado,
    DateTime? FechaCierre,
    DateTime? FechaPago,
    string? Observaciones,
    List<ClaseProfesorDTO> Clases
);

public record ResumenProfesorDTO(
    Guid IdProfesor,
    string NombreProfesor,
    int ClasesPendientes,
    int ClasesAprobadas,
    int ClasesLiquidadas,
    int ClasesPagadas,
    decimal TotalPendiente,
    decimal TotalAprobado,
    decimal TotalLiquidado,
    decimal TotalPagado
);

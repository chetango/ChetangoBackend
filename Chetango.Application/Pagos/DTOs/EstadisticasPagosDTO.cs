namespace Chetango.Application.Pagos.DTOs;

public record EstadisticasPagosDTO(
    decimal TotalIngresos, // Total de pagos verificados hoy
    int TotalPagosHoy, // Cantidad de pagos verificados hoy
    int TotalPendientesVerificacion, // Pagos pendientes de verificación
    int TotalVerificadosHoy, // Pagos verificados hoy
    decimal IngresosMesActual, // Total verificado en el mes actual
    decimal ComparacionMesAnterior // % de cambio respecto al mes anterior
);


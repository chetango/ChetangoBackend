namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para reporte de ingresos con comparativas y desglose
/// </summary>
public class ReporteIngresosDTO
{
    public decimal TotalRecaudado { get; set; }
    public int Cantidad { get; set; }
    public decimal Promedio { get; set; }
    
    /// <summary>
    /// % de cambio respecto al mes anterior (positivo = crecimiento, negativo = decrecimiento)
    /// </summary>
    public decimal? ComparativaMesAnterior { get; set; }
    
    /// <summary>
    /// Tendencia mensual (últimos 12 meses)
    /// </summary>
    public List<TendenciaMensualDTO> TendenciaMensual { get; set; } = new();
    
    /// <summary>
    /// Gráfica de ingresos mensuales
    /// </summary>
    public ChartDataDTO? GraficaIngresosMensuales { get; set; }
    
    /// <summary>
    /// Desglose por métodos de pago
    /// </summary>
    public List<DesglosePagoDTO> DesgloseMetodosPago { get; set; } = new();
}

/// <summary>
/// Tendencia mensual de ingresos
/// </summary>
public class TendenciaMensualDTO
{
    public int Año { get; set; }
    public int Mes { get; set; }
    public string MesNombre { get; set; } = string.Empty;
    public decimal TotalIngresos { get; set; }
    public int CantidadPagos { get; set; }
}

/// <summary>
/// Desglose de ingresos por método de pago
/// </summary>
public class DesglosePagoDTO
{
    public string MetodoPago { get; set; } = string.Empty;
    public decimal TotalRecaudado { get; set; }
    public int CantidadPagos { get; set; }
    public decimal PorcentajeDelTotal { get; set; }
}

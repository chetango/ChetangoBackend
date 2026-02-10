namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para dashboard general con KPIs, gráficas y alertas
/// </summary>
public class DashboardDTO
{
    /// <summary>
    /// KPIs principales
    /// </summary>
    public DashboardKPIsDTO KPIs { get; set; } = new();
    
    /// <summary>
    /// Gráfica de ingresos (últimos 6 meses)
    /// </summary>
    public ChartDataDTO? GraficaIngresos { get; set; }
    
    /// <summary>
    /// Gráfica de asistencias por día de la semana
    /// </summary>
    public ChartDataDTO? GraficaAsistencias { get; set; }
    
    /// <summary>
    /// Gráfica de paquetes por estado
    /// </summary>
    public ChartDataDTO? GraficaPaquetes { get; set; }
    
    /// <summary>
    /// Gráfica de métodos de pago (distribución)
    /// </summary>
    public ChartDataDTO? GraficaMetodosPago { get; set; }
    
    /// <summary>
    /// Resumen del periodo con métricas de pagos
    /// </summary>
    public ResumenPeriodoDTO? ResumenPeriodo { get; set; }
    
    /// <summary>
    /// Últimos pagos registrados (Top 10)
    /// </summary>
    public List<UltimoPagoDTO> UltimosPagos { get; set; } = new();
    
    /// <summary>
    /// Alertas del sistema
    /// </summary>
    public List<AlertaDTO> Alertas { get; set; } = new();
}

/// <summary>
/// KPIs principales del dashboard
/// </summary>
public class DashboardKPIsDTO
{
    public int TotalAlumnosActivos { get; set; }
    public decimal IngresosEsteMes { get; set; }
    public int ClasesProximos7Dias { get; set; }
    public int PaquetesActivos { get; set; }
    public int PaquetesVencidos { get; set; }
    public int PaquetesPorVencer { get; set; }
    public int PaquetesVendidos { get; set; }
    public int AsistenciasHoy { get; set; }
    public int AsistenciasMes { get; set; }
    
    /// <summary>
    /// Comparativas con periodo anterior (% de cambio)
    /// </summary>
    public decimal? CrecimientoIngresosMesAnterior { get; set; }
    public decimal? ComparativaAsistenciasMesAnterior { get; set; }
    public decimal? ComparativaAlumnosMesAnterior { get; set; }
    public decimal? ComparativaPaquetesVendidosMesAnterior { get; set; }
}

/// <summary>
/// Alerta del sistema
/// </summary>
public class AlertaDTO
{
    public TipoAlerta Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaGeneracion { get; set; }
    public PrioridadAlerta Prioridad { get; set; }
    
    /// <summary>
    /// Datos adicionales de la alerta (JSON flexible)
    /// </summary>
    public Dictionary<string, object>? DatosAdicionales { get; set; }
}

/// <summary>
/// Tipo de alerta
/// </summary>
public enum TipoAlerta
{
    PaquetePorVencer,
    AlumnoInactivo,
    ClaseBajaAsistencia,
    PagosPendientes,
    ClasePocosCupos,
    PagoPendiente
}

/// <summary>
/// Prioridad de la alerta
/// </summary>
public enum PrioridadAlerta
{
    Baja,
    Media,
    Alta
}

/// <summary>
/// Resumen del periodo con métricas de pagos
/// </summary>
public class ResumenPeriodoDTO
{
    public decimal TotalRecaudado { get; set; }
    public decimal PromedioPorPago { get; set; }
    public int CantidadPagos { get; set; }
    
    /// <summary>
    /// Tasa de conversión: % de alumnos activos que compraron paquetes
    /// </summary>
    public decimal TasaConversion { get; set; }
}

/// <summary>
/// DTO para últimos pagos registrados
/// </summary>
public class UltimoPagoDTO
{
    public Guid IdPago { get; set; }
    public string NombreAlumno { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public string NombrePaquete { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

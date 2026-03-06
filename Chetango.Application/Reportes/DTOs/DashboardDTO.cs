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
    
    // Ingresos totales y desglose
    public decimal IngresosEsteMes { get; set; }
    public decimal IngresosAlumnosEsteMes { get; set; }
    public decimal OtrosIngresosEsteMes { get; set; }
    public decimal IngresosMedellinEsteMes { get; set; }
    public decimal IngresosManizalesEsteMes { get; set; }
    
    public int ClasesProximos7Dias { get; set; }
    public int PaquetesActivos { get; set; }
    public int PaquetesVencidos { get; set; }
    public int PaquetesPorVencer { get; set; }
    public int PaquetesAgotados { get; set; }
    public int PaquetesAgotadosMedellin { get; set; }
    public int PaquetesAgotadosManizales { get; set; }
    public int PaquetesVendidos { get; set; }
    public int AsistenciasHoy { get; set; }
    public int AsistenciasMes { get; set; }
    
    // Egresos totales y desglose
    public decimal EgresosEsteMes { get; set; }
    public decimal EgresosNominaEsteMes { get; set; }
    public decimal OtrosGastosEsteMes { get; set; }
    public decimal EgresosMedellinEsteMes { get; set; }
    public decimal EgresosManizalesEsteMes { get; set; }
    
    public decimal GananciaNeta { get; set; }

    /// <summary>
    /// Desglose dinámico de ingresos y egresos por sede del tenant.
    /// Reemplaza las propiedades hardcodeadas IngresosMedellinEsteMes / IngresosManizalesEsteMes
    /// para soportar cualquier número de sedes (multi-tenant genérico).
    /// </summary>
    public List<FinancialPorSedeDTO> IngresosEgresosPorSede { get; set; } = new();

    /// <summary>
    /// Desglose dinámico de paquetes agotados por sede del tenant.
    /// </summary>
    public List<PaquetesPorSedeDTO> PaquetesAgotadosPorSede { get; set; } = new();

    /// <summary>
    /// Comparativas con periodo anterior (% de cambio)
    /// </summary>
    public decimal? CrecimientoIngresosMesAnterior { get; set; }
    public decimal? ComparativaAsistenciasMesAnterior { get; set; }
    public decimal? ComparativaAlumnosMesAnterior { get; set; }
    public decimal? ComparativaPaquetesVendidosMesAnterior { get; set; }
    public decimal? ComparativaEgresosMesAnterior { get; set; }
    public decimal? ComparativaGananciaMesAnterior { get; set; }
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

/// <summary>
/// Ingresos y egresos de una sede específica del tenant.
/// Usado en IngresosEgresosPorSede del dashboard para eliminar propiedades hardcodeadas.
/// </summary>
public class FinancialPorSedeDTO
{
    /// <summary>Valor numérico de la sede (discriminador en tablas de negocio).</summary>
    public int SedeValor { get; set; }

    /// <summary>Nombre visible de la sede (ej: "Medellín", "Principal").</summary>
    public string NombreSede { get; set; } = string.Empty;

    /// <summary>Total de ingresos de la sede en el periodo.</summary>
    public decimal Ingresos { get; set; }

    /// <summary>Total de egresos de la sede en el periodo.</summary>
    public decimal Egresos { get; set; }

    /// <summary>Ganancia neta de la sede en el periodo (Ingresos - Egresos).</summary>
    public decimal Ganancia => Ingresos - Egresos;
}

/// <summary>
/// Paquetes agotados de una sede específica del tenant.
/// Usado en PaquetesAgotadosPorSede del dashboard para eliminar propiedades hardcodeadas.
/// </summary>
public class PaquetesPorSedeDTO
{
    /// <summary>Valor numérico de la sede.</summary>
    public int SedeValor { get; set; }

    /// <summary>Nombre visible de la sede.</summary>
    public string NombreSede { get; set; } = string.Empty;

    /// <summary>Cantidad de paquetes agotados (IdEstado==4) de la sede.</summary>
    public int Agotados { get; set; }
}

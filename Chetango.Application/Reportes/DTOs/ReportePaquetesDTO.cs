namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para reporte de paquetes con alertas y métricas
/// </summary>
public class ReportePaquetesDTO
{
    public int TotalActivos { get; set; }
    public int TotalVencidos { get; set; }
    public int TotalPorVencer { get; set; }
    public int TotalAgotados { get; set; }
    
    /// <summary>
    /// Alertas de paquetes próximos a vencer (dentro de 7 días)
    /// </summary>
    public List<PaqueteAlertaDTO> AlertasPorVencer { get; set; } = new();
    
    /// <summary>
    /// Desglose de paquetes por estado
    /// </summary>
    public List<PaquetesPorEstadoDTO> DesgloseEstados { get; set; } = new();
    
    /// <summary>
    /// Gráfica de paquetes por tipo
    /// </summary>
    public ChartDataDTO? GraficaPaquetesPorTipo { get; set; }
}

/// <summary>
/// Alerta de paquete próximo a vencer
/// </summary>
public class PaqueteAlertaDTO
{
    public Guid IdPaquete { get; set; }
    public string NombreAlumno { get; set; } = string.Empty;
    public string CorreoAlumno { get; set; } = string.Empty;
    public string NombreTipoPaquete { get; set; } = string.Empty;
    public DateTime FechaVencimiento { get; set; }
    public int DiasRestantes { get; set; }
    public int ClasesRestantes { get; set; }
}

/// <summary>
/// Paquetes agrupados por estado
/// </summary>
public class PaquetesPorEstadoDTO
{
    public string Estado { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PorcentajeDelTotal { get; set; }
}

namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para reporte de asistencias con métricas y detalles
/// </summary>
public class ReporteAsistenciasDTO
{
    public int TotalAsistencias { get; set; }
    public int Presentes { get; set; }
    public int Ausentes { get; set; }
    public int Justificadas { get; set; }
    public decimal PorcentajeAsistencia { get; set; }
    
    /// <summary>
    /// Lista detallada de asistencias (paginada si es necesario)
    /// </summary>
    public List<AsistenciaDetalleDTO> ListaDetallada { get; set; } = new();
    
    /// <summary>
    /// Gráfica de asistencias por día
    /// </summary>
    public ChartDataDTO? GraficaAsistenciasPorDia { get; set; }
}

/// <summary>
/// Detalle individual de una asistencia
/// </summary>
public class AsistenciaDetalleDTO
{
    public DateTime Fecha { get; set; }
    public string NombreAlumno { get; set; } = string.Empty;
    public string NombreClase { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string? NombreProfesor { get; set; }
}

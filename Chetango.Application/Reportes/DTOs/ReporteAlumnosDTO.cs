namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para reporte de alumnos con métricas de actividad
/// </summary>
public class ReporteAlumnosDTO
{
    public int TotalActivos { get; set; }
    public int TotalInactivos { get; set; }
    public int NuevosEsteMes { get; set; }
    public decimal TasaRetencion { get; set; }
    
    /// <summary>
    /// Alumnos sin asistencias en más de 30 días
    /// </summary>
    public List<AlumnoInactivoDTO> AlumnosInactivos { get; set; } = new();
    
    /// <summary>
    /// Alumnos con paquetes próximos a vencer
    /// </summary>
    public List<AlumnoPorVencerDTO> AlumnosPorVencer { get; set; } = new();
    
    /// <summary>
    /// Gráfica de alumnos nuevos por mes (últimos 12 meses)
    /// </summary>
    public ChartDataDTO? GraficaAlumnosPorMes { get; set; }
}

/// <summary>
/// Alumno inactivo (sin asistencias recientes)
/// </summary>
public class AlumnoInactivoDTO
{
    public Guid IdAlumno { get; set; }
    public string NombreAlumno { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public DateTime? UltimaAsistencia { get; set; }
    public int DiasInactivo { get; set; }
}

/// <summary>
/// Alumno con paquete próximo a vencer
/// </summary>
public class AlumnoPorVencerDTO
{
    public Guid IdAlumno { get; set; }
    public string NombreAlumno { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public DateTime FechaVencimiento { get; set; }
    public int DiasRestantes { get; set; }
}

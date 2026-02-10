namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para dashboard del profesor con datos del día y período
/// </summary>
public class DashboardProfesorDTO
{
    public string NombreProfesor { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    
    /// <summary>
    /// Clases programadas para HOY
    /// </summary>
    public List<ClaseHoyProfesorDTO> ClasesHoy { get; set; } = new();
    
    /// <summary>
    /// KPIs del profesor
    /// </summary>
    public KPIsProfesorDTO KPIs { get; set; } = new();
    
    /// <summary>
    /// Gráfica de asistencia últimos 30 días (por semana)
    /// </summary>
    public ChartDataDTO? GraficaAsistencia30Dias { get; set; }
    
    /// <summary>
    /// Próximas clases (excluyendo hoy)
    /// </summary>
    public List<ClaseProximaDTO> ProximasClases { get; set; } = new();
    
    /// <summary>
    /// Eventos próximos para profesores (contenido educativo/informativo)
    /// </summary>
    public List<EventoDTO> EventosProximos { get; set; } = new();
}

/// <summary>
/// Clase programada para hoy con estado
/// </summary>
public class ClaseHoyProfesorDTO
{
    public Guid IdClase { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string Tipo { get; set; } = string.Empty; // "grupal" | "particular"
    public string Estado { get; set; } = string.Empty; // "programada" | "en-curso" | "finalizada"
    public int AlumnosEsperados { get; set; }
    public int? AlumnosPresentes { get; set; }
    public int? MinutosParaInicio { get; set; }
}

/// <summary>
/// KPIs específicos del profesor
/// </summary>
public class KPIsProfesorDTO
{
    /// <summary>
    /// Total de clases dictadas en el mes actual
    /// </summary>
    public int ClasesDictadasMes { get; set; }
    
    /// <summary>
    /// Promedio de asistencia últimos 30 días
    /// </summary>
    public decimal PromedioAsistencia30Dias { get; set; }
    
    /// <summary>
    /// Alumnos únicos este mes
    /// </summary>
    public int AlumnosUnicosMes { get; set; }
    
    /// <summary>
    /// Clases esta semana
    /// </summary>
    public int ClasesEstaSemana { get; set; }
    
    /// <summary>
    /// Clases completadas esta semana
    /// </summary>
    public int ClasesCompletadasSemana { get; set; }
}

namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para reporte de clases del profesor autenticado
/// </summary>
public class MisClasesReporteDTO
{
    public string NombreProfesor { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    
    /// <summary>
    /// Total de clases impartidas en el periodo
    /// </summary>
    public int TotalClasesImpartidas { get; set; }
    
    /// <summary>
    /// Promedio de asistencia en las clases
    /// </summary>
    public decimal PromedioAsistencia { get; set; }
    
    /// <summary>
    /// Alumnos únicos atendidos
    /// </summary>
    public int AlumnosUnicos { get; set; }
    
    /// <summary>
    /// Clases próximos 7 días
    /// </summary>
    public List<ClaseProximaDTO> ClasesProximas { get; set; } = new();
    
    /// <summary>
    /// Gráfica de asistencia por tipo de clase
    /// </summary>
    public ChartDataDTO? GraficaAsistenciaPorTipo { get; set; }
    
    /// <summary>
    /// Desglose por tipo de clase
    /// </summary>
    public List<ClasesProfesorPorTipoDTO> DesgloseporTipo { get; set; } = new();
}

/// <summary>
/// Clases del profesor agrupadas por tipo
/// </summary>
public class ClasesProfesorPorTipoDTO
{
    public string NombreTipoClase { get; set; } = string.Empty;
    public int CantidadClases { get; set; }
    public decimal PromedioAsistencia { get; set; }
    public decimal OcupacionPromedio { get; set; }
}

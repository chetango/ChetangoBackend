namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para reporte de clases con métricas de asistencia y popularidad
/// </summary>
public class ReporteClasesDTO
{
    public int TotalClases { get; set; }
    public decimal PromedioAsistencia { get; set; }
    public decimal OcupacionPromedio { get; set; }
    
    /// <summary>
    /// Clases más populares (mayor asistencia promedio)
    /// </summary>
    public List<ClasePopularDTO> ClasesMasPopulares { get; set; } = new();
    
    /// <summary>
    /// Gráfica de asistencia por día de la semana
    /// </summary>
    public ChartDataDTO? GraficaAsistenciaPorDia { get; set; }
    
    /// <summary>
    /// Desglose por tipo de clase
    /// </summary>
    public List<ClasesPorTipoDTO> DesgloseporTipo { get; set; } = new();
}

/// <summary>
/// Clase popular con métricas
/// </summary>
public class ClasePopularDTO
{
    public string NombreTipoClase { get; set; } = string.Empty;
    public int TotalClases { get; set; }
    public decimal PromedioAsistencia { get; set; }
    public decimal OcupacionPorcentaje { get; set; }
}

/// <summary>
/// Clases agrupadas por tipo
/// </summary>
public class ClasesPorTipoDTO
{
    public string NombreTipoClase { get; set; } = string.Empty;
    public int CantidadClases { get; set; }
    public decimal PromedioAsistencia { get; set; }
}

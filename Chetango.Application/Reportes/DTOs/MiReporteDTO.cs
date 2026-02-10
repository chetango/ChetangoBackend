using Chetango.Application.Pagos.DTOs;

namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para reporte personal del alumno autenticado
/// </summary>
public class MiReporteDTO
{
    public string NombreAlumno { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    
    /// <summary>
    /// Total de clases tomadas históricamente
    /// </summary>
    public int TotalClasesTomadas { get; set; }
    
    /// <summary>
    /// Porcentaje de asistencia (presentes / total)
    /// </summary>
    public decimal PorcentajeAsistencia { get; set; }
    
    /// <summary>
    /// Clases restantes en paquete activo actual
    /// </summary>
    public int? ClasesRestantes { get; set; }
    
    /// <summary>
    /// Información del paquete actual activo
    /// </summary>
    public PaqueteResumenDTO? PaqueteActual { get; set; }
    
    /// <summary>
    /// Historial de pagos del alumno
    /// </summary>
    public List<PagoResumenDTO> HistorialPagos { get; set; } = new();
    
    /// <summary>
    /// Gráfica de asistencias mensuales (últimos 6 meses)
    /// </summary>
    public ChartDataDTO? GraficaAsistenciasMensuales { get; set; }
    
    /// <summary>
    /// Próximas clases programadas
    /// </summary>
    public List<ClaseProximaDTO> ProximasClases { get; set; } = new();
}

/// <summary>
/// Resumen de pago
/// </summary>
public class PagoResumenDTO
{
    public Guid IdPago { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public string? ConceptoPago { get; set; }
    public int? IdPaqueteGenerado { get; set; }
}

/// <summary>
/// Clase próxima programada
/// </summary>
public class ClaseProximaDTO
{
    public Guid IdClase { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public string TipoClase { get; set; } = string.Empty;
    public string NombreProfesor { get; set; } = string.Empty;
    public int CupoMaximo { get; set; }
    public int InscritosActual { get; set; }
}

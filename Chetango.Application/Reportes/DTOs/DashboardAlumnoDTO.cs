namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para dashboard del alumno con progreso, próximas clases y logros
/// </summary>
public class DashboardAlumnoDTO
{
    public string NombreAlumno { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty; // Para QR
    public DateTime FechaIngreso { get; set; }
    
    /// <summary>
    /// Información del paquete activo
    /// </summary>
    public PaqueteActivoDTO? PaqueteActivo { get; set; }
    
    /// <summary>
    /// Próxima clase del alumno
    /// </summary>
    public ProximaClaseAlumnoDTO? ProximaClase { get; set; }
    
    /// <summary>
    /// Estadísticas de asistencia del alumno
    /// </summary>
    public AsistenciaAlumnoDTO Asistencia { get; set; } = new();
    
    /// <summary>
    /// Logros desbloqueados del alumno
    /// </summary>
    public List<LogroDTO> Logros { get; set; } = new();
    
    /// <summary>
    /// Eventos próximos de la academia
    /// </summary>
    public List<EventoDTO> EventosProximos { get; set; } = new();
}

/// <summary>
/// Información del paquete activo del alumno
/// </summary>
public class PaqueteActivoDTO
{
    public Guid IdPaquete { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int ClasesRestantes { get; set; }
    public int ClasesTotales { get; set; }
    public string Estado { get; set; } = string.Empty; // "activo" | "agotado" | "congelado" | "vencido"
    public DateTime FechaVencimiento { get; set; }
    public int DiasParaVencer { get; set; }
}

/// <summary>
/// Próxima clase del alumno
/// </summary>
public class ProximaClaseAlumnoDTO
{
    public Guid IdClase { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Profesor { get; set; } = string.Empty;
    public int MinutosParaInicio { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
}

/// <summary>
/// Estadísticas de asistencia del alumno
/// </summary>
public class AsistenciaAlumnoDTO
{
    public decimal Porcentaje { get; set; }
    public int ClasesTomadas { get; set; }
    public int RachaSemanas { get; set; }
}

/// <summary>
/// Logro del alumno
/// </summary>
public class LogroDTO
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty; // "flame" | "trophy" | "target"
    public string Color { get; set; } = string.Empty;
    public bool Desbloqueado { get; set; }
}

/// <summary>
/// Evento próximo de la academia
/// </summary>
public class EventoDTO
{
    public Guid IdEvento { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
    public decimal? Precio { get; set; }
    public bool Destacado { get; set; }
}

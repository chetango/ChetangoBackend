namespace Chetango.Domain.Entities;

/// <summary>
/// Solicitud de clase privada realizada por un alumno
/// El admin recibe notificación y puede agendar la clase
/// </summary>
public class SolicitudClasePrivada
{
    public Guid IdSolicitud { get; set; }
    
    public Guid IdAlumno { get; set; }
    public Alumno Alumno { get; set; } = null!;
    
    public Guid? IdTipoClaseDeseado { get; set; }
    public string TipoClaseDeseado { get; set; } = string.Empty; // "Tango Salón Privado" o "Tango Escenario Privado"
    
    public DateTime? FechaPreferida { get; set; }
    public TimeSpan? HoraPreferida { get; set; }
    
    public string? ObservacionesAlumno { get; set; }
    
    /// <summary>
    /// Estados: Pendiente, Aprobada, Rechazada, Agendada
    /// </summary>
    public string Estado { get; set; } = "Pendiente";
    
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    
    public Guid? IdUsuarioRespondio { get; set; }
    
    public string? MensajeRespuesta { get; set; }
    
    public Guid? IdClaseCreada { get; set; }
}

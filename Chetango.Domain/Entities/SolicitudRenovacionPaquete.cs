namespace Chetango.Domain.Entities;

/// <summary>
/// Solicitud de renovación de paquete realizada por un alumno
/// El admin recibe notificación y puede aprobar/rechazar
/// </summary>
public class SolicitudRenovacionPaquete
{
    public Guid IdSolicitud { get; set; }
    
    public Guid IdAlumno { get; set; }
    public Alumno Alumno { get; set; } = null!;
    
    public Guid? IdPaqueteActual { get; set; }
    
    public Guid? IdTipoPaqueteDeseado { get; set; }
    public string TipoPaqueteDeseado { get; set; } = string.Empty; // Nombre del tipo de paquete
    
    public string? MensajeAlumno { get; set; }
    
    /// <summary>
    /// Estados: Pendiente, Aprobada, Rechazada, Completada
    /// </summary>
    public string Estado { get; set; } = "Pendiente";
    
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    
    public Guid? IdUsuarioRespondio { get; set; }
    
    public string? MensajeRespuesta { get; set; }
    
    public Guid? IdPaqueteCreado { get; set; }
}

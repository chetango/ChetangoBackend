using System.ComponentModel.DataAnnotations.Schema;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Domain.Entities
{
    public class Alumno
    {
        public Guid IdAlumno { get; set; }
        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [NotMapped]
        public string NombreCompleto => Usuario?.NombreUsuario ?? string.Empty;

        [NotMapped]
        public string DocumentoIdentidad => Usuario?.NumeroDocumento ?? string.Empty;

    // Metadata del alumno en sí
    public DateTime FechaInscripcion { get; set; }
    public int IdEstado { get; set; }
    public EstadoAlumno Estado { get; set; } = null!;

        // Perfil
        public string? AvatarUrl { get; set; }
        
        // Contacto de Emergencia
        public string? ContactoEmergenciaNombre { get; set; }
        public string? ContactoEmergenciaTelefono { get; set; }
        public string? ContactoEmergenciaRelacion { get; set; }
        
        // Configuración de Notificaciones
        public bool NotificacionesEmail { get; set; } = true;
        public bool RecordatoriosClase { get; set; } = true;
        public bool AlertasPaquete { get; set; } = true;

        // Relaciones
        // Paquetes ahora navegan desde Paquete -> Alumno
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    }
}

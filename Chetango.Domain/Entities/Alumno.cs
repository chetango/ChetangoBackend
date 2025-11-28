using System.ComponentModel.DataAnnotations.Schema;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Domain.Entities
{
    public class Alumno
    {
        public Guid IdAlumno { get; set; }
        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

<<<<<<< HEAD
        [NotMapped]
        public string NombreCompleto => Usuario?.NombreUsuario ?? string.Empty;

        [NotMapped]
        public string DocumentoIdentidad => Usuario?.NumeroDocumento ?? string.Empty;

    // Metadata del alumno en sí
    public DateTime FechaInscripcion { get; set; }
    public int IdEstado { get; set; }
    public EstadoAlumno Estado { get; set; } = null!;
=======
        // Nuevos campos agregados
        public DateTime FechaInscripcion { get; set; }
        public int IdEstado { get; set; }
        public EstadoAlumno Estado { get; set; } = null!;
>>>>>>> origin/develop

        // Relaciones
        // Paquetes ahora navegan desde Paquete -> Alumno
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    }
}

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

        // Relaciones
        // Paquetes ahora navegan desde Paquete -> Alumno
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    }
}

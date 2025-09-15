using Chetango.Domain.Entities.Estados;
namespace Chetango.Domain.Entities
{
    public class Alumno
    {
        public Guid IdAlumno { get; set; }
        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        // Relaciones
        // Paquetes ahora navegan desde Paquete -> Alumno
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    }
}

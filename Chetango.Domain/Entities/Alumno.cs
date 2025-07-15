namespace Chetango.Domain.Entities
{
    public class Alumno
    {
        public Guid IdAlumno { get; set; }
        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        // Relaciones
        public ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    }
}

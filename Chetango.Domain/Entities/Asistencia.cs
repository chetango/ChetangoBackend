namespace Chetango.Domain.Entities
{
    public class Asistencia
    {
        public Guid IdAsistencia { get; set; }

        public Guid IdClase { get; set; }
        public Clase Clase { get; set; } = null!;

        public Guid IdAlumno { get; set; }
        public Alumno Alumno { get; set; } = null!;

        public Guid IdPaqueteUsado { get; set; }
        public Paquete PaqueteUsado { get; set; } = null!;

        public EstadoAsistencia Estado { get; set; }
        public string? Observacion { get; set; }
    }
}

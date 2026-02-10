using Chetango.Domain.Entities.Estados;
namespace Chetango.Domain.Entities
{
    public class Clase
    {
        public Guid IdClase { get; set; }
        public DateTime Fecha { get; set; }
        public Guid IdTipoClase { get; set; }
        public TipoClase TipoClase { get; set; } = null!;
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int CupoMaximo { get; set; }
        public string? Observaciones { get; set; }
        public string Estado { get; set; } = "Programada"; // Programada, EnCurso, Completada, Cancelada

        // DEPRECATED: Usar ClaseProfesor para obtener profesores con sus roles
        // Mantenido temporalmente para retrocompatibilidad
        public Guid? IdProfesorPrincipal { get; set; }
        public Profesor? ProfesorPrincipal { get; set; }

        // Relaciones
        public ICollection<MonitorClase> Monitores { get; set; } = new List<MonitorClase>();
        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
        public ICollection<ClaseProfesor> Profesores { get; set; } = new List<ClaseProfesor>(); // USAR ESTE
    }
}

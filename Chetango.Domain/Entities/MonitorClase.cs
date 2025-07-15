namespace Chetango.Domain.Entities
{
    public class MonitorClase
    {
        public Guid IdMonitorClase { get; set; }

        public Guid IdClase { get; set; }
        public Clase Clase { get; set; } = null!;

        public Guid IdProfesor { get; set; }
        public Profesor Profesor { get; set; } = null!;
    }
}

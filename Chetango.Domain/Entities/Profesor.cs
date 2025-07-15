namespace Chetango.Domain.Entities
{
    public class Profesor
    {
        public Guid IdProfesor { get; set; }
        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public TipoProfesor TipoProfesor { get; set; }

        // Relaciones
        public ICollection<Clase> Clases { get; set; } = new List<Clase>();
        public ICollection<MonitorClase> MonitorClases { get; set; } = new List<MonitorClase>();
        public ICollection<TarifaProfesor> Tarifas { get; set; } = new List<TarifaProfesor>();
    }
}

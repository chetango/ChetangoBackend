namespace Chetango.Domain.Entities
{
    public class CongelacionPaquete
    {
        public Guid IdCongelacion { get; set; }

        public Guid IdPaquete { get; set; }
        public Paquete Paquete { get; set; } = null!;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}

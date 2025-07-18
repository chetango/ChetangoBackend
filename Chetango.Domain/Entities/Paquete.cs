namespace Chetango.Domain.Entities
{
    public class Paquete
    {
        public Guid IdPaquete { get; set; }

        public Guid IdPago { get; set; }
        public Pago Pago { get; set; } = null!;

        public int ClasesDisponibles { get; set; }
        public int ClasesUsadas { get; set; }
        public DateTime FechaActivacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public EstadoPaquete Estado { get; set; }
        public TipoPaquete TipoPaquete { get; set; }
        public decimal ValorPaquete { get; set; }

        // Relaciones
        public ICollection<CongelacionPaquete> Congelaciones { get; set; } = new List<CongelacionPaquete>();
    }
}

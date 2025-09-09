namespace Chetango.Domain.Entities.Estados
{
    public class MetodoPago
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}

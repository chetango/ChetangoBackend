namespace Chetango.Domain.Entities.Estados
{
    public class TipoPaquete
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
    }
}

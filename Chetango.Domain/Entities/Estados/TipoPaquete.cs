namespace Chetango.Domain.Entities.Estados
{
    public class TipoPaquete
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int NumeroClases { get; set; }
        public decimal Precio { get; set; }
        public int DiasVigencia { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;

        // Relaciones
        public ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
    }
}

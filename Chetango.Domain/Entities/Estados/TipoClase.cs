namespace Chetango.Domain.Entities.Estados
{
    public class TipoClase
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<Clase> Clases { get; set; } = new List<Clase>();
    }
}

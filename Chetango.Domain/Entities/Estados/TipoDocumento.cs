namespace Chetango.Domain.Entities.Estados
{
    public class TipoDocumento
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}

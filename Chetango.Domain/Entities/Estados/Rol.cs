namespace Chetango.Domain.Entities.Estados
{
    public class Rol
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<UsuarioRol> UsuariosRoles { get; set; } = new List<UsuarioRol>();
    }
}

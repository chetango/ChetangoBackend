namespace Chetango.Domain.Entities
{
    public class UsuarioRol
    {
        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public Rol Rol { get; set; }
    }
}

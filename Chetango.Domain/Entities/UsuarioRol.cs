using Chetango.Domain.Entities.Estados;
namespace Chetango.Domain.Entities
{
    public class UsuarioRol
    {
        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public Guid IdRol { get; set; } // Nueva FK explícita
        public Rol Rol { get; set; } = null!;
    }
}

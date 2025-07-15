namespace Chetango.Domain.Entities
{
    public class Notificacion
    {
        public Guid IdNotificacion { get; set; }

        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public TipoNotificacion Tipo { get; set; }
        public string Mensaje { get; set; } = null!;
        public bool Leida { get; set; }
        public DateTime FechaEnvio { get; set; }
    }
}

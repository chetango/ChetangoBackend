namespace Chetango.Domain.Entities.Estados
{
    public class Notificacion
    {
        public Guid IdNotificacion { get; set; }

        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int IdEstado { get; set; }
        public EstadoNotificacion Estado { get; set; } = null!;

        public string Mensaje { get; set; } = null!;
        public DateTime FechaEnvio { get; set; }
        public bool Leida { get; set; }
    }
}


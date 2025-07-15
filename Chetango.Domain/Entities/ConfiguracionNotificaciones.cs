namespace Chetango.Domain.Entities
{
    public class ConfiguracionNotificaciones
    {
        public Guid IdConfig { get; set; }

        public int AnticipacionAlerta { get; set; }
        public string TextoVencimiento { get; set; } = null!;
        public string TextoAgotamiento { get; set; } = null!;
    }
}

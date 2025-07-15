namespace Chetango.Domain.Entities
{
    public class Auditoria
    {
        public Guid IdAuditoria { get; set; }

        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public string Modulo { get; set; } = null!;
        public string Accion { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public DateTime FechaHora { get; set; }
    }
}

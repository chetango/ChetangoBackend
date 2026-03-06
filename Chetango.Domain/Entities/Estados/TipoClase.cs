namespace Chetango.Domain.Entities.Estados
{
    public class TipoClase
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Tenant propietario de este tipo de clase.
        /// NULL solo en registros de seed globales legacy (no visibles a ningún tenant).
        /// </summary>
        public Guid? TenantId { get; set; }

        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<Clase> Clases { get; set; } = new List<Clase>();
    }
}

namespace Chetango.Domain.Entities.Estados
{
    /// <summary>
    /// Catálogo de categorías para otros gastos (no nómina de profesores)
    /// Ejemplos: Arriendo, Servicios Públicos, Mantenimiento, Marketing, Suministros, Equipamiento, Impuestos, Otros
    /// </summary>
    public class CategoriaGasto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        // Relaciones
        public ICollection<OtroGasto> OtrosGastos { get; set; } = new List<OtroGasto>();
    }
}

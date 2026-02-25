namespace Chetango.Domain.Entities.Estados
{
    /// <summary>
    /// Catálogo de categorías para otros ingresos (no provenientes de pagos de alumnos)
    /// Ejemplos: Eventos, Alquiler de Espacio, Mercancía, Shows, Talleres, Otros
    /// </summary>
    public class CategoriaIngreso
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        // Relaciones
        public ICollection<OtroIngreso> OtrosIngresos { get; set; } = new List<OtroIngreso>();
    }
}

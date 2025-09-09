namespace Chetango.Domain.Entities.Estados
{
    public class RolEnClase
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<TarifaProfesor> Tarifas { get; set; } = new List<TarifaProfesor>();
    }
}

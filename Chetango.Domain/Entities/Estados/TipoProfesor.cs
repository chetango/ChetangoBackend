namespace Chetango.Domain.Entities.Estados
{
    public class TipoProfesor
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();
    }
}

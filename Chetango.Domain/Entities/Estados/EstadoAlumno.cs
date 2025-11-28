using Chetango.Domain.Entities;
namespace Chetango.Domain.Entities.Estados
{
    // Catálogo de estados posibles para un Alumno (Activo, Inactivo, Suspendido, Retirado)
    public class EstadoAlumno
    {
        public int IdEstado { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        // Relaciones
        public ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using Chetango.Domain.Entities;

namespace Chetango.Domain.Entities.Estados
{
    public class Profesor
    {
        public Guid IdProfesor { get; set; }
        public Guid IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public Guid IdTipoProfesor { get; set; }
        public TipoProfesor TipoProfesor { get; set; } = null!;

        [NotMapped]
        public string NombreCompleto => Usuario?.NombreUsuario ?? string.Empty;

        // Relaciones
        public ICollection<Clase> Clases { get; set; } = new List<Clase>();
        public ICollection<MonitorClase> MonitorClases { get; set; } = new List<MonitorClase>();
        public ICollection<TarifaProfesor> Tarifas { get; set; } = new List<TarifaProfesor>();
    }
}

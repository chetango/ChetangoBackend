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

        // Perfil Profesional
        public string? Biografia { get; set; }
        public string? Especialidades { get; set; } // JSON array: ["Salón", "Escenario", "Privadas"]
        public decimal TarifaActual { get; set; } // Tarifa personalizada por hora del profesor
        
        // Configuración de Notificaciones
        public bool NotificacionesEmail { get; set; } = true;
        public bool RecordatoriosClase { get; set; } = true;
        public bool AlertasCambios { get; set; } = true;

        // Relaciones
        public ICollection<Clase> Clases { get; set; } = new List<Clase>();
        public ICollection<MonitorClase> MonitorClases { get; set; } = new List<MonitorClase>();
        public ICollection<TarifaProfesor> Tarifas { get; set; } = new List<TarifaProfesor>();
        public ICollection<ClaseProfesor> ClasesProfesores { get; set; } = new List<ClaseProfesor>();
        public ICollection<LiquidacionMensual> Liquidaciones { get; set; } = new List<LiquidacionMensual>();
    }
}

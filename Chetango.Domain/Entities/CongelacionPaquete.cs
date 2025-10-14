using Chetango.Domain.Entities.Estados;
namespace Chetango.Domain.Entities
{
    // Periodo de suspensión temporal de un Paquete (pausa del conteo de vencimiento).
    // Validaciones futuras: no solaparse y FechaFin > FechaInicio.
    public class CongelacionPaquete
    {
        public Guid IdCongelacion { get; set; }

        public Guid IdPaquete { get; set; }
        public Paquete Paquete { get; set; } = null!;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}

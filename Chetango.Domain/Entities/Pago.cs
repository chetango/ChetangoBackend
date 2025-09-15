using Chetango.Domain.Entities.Estados;
namespace Chetango.Domain.Entities
{
    public class Pago
    {
        public Guid IdPago { get; set; }

        public Guid IdAlumno { get; set; }
        public Alumno Alumno { get; set; } = null!;

        public DateTime FechaPago { get; set; }
        public decimal MontoTotal { get; set; }
        public Guid IdMetodoPago { get; set; }
        public MetodoPago MetodoPago { get; set; } = null!;
        public string? Nota { get; set; }

        // Relaciones
        public ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
    }
}

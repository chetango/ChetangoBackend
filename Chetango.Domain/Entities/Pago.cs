namespace Chetango.Domain.Entities
{
    public class Pago
    {
        public Guid IdPago { get; set; }

        public Guid IdAlumno { get; set; }
        public Alumno Alumno { get; set; } = null!;

        public DateTime FechaPago { get; set; }
        public decimal MontoTotal { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public string? Nota { get; set; }

        // Relaciones
        public ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
    }
}

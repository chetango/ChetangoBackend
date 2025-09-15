namespace Chetango.Domain.Entities.Estados
{
    public class Paquete
    {
        public Guid IdPaquete { get; set; }

        // Propiedad de pertenencia directa al Alumno
        public Guid IdAlumno { get; set; }
        public Chetango.Domain.Entities.Alumno Alumno { get; set; } = null!;

        // Origen financiero (opcional si es bono / ajuste)
        public Guid? IdPago { get; set; }
        public Pago? Pago { get; set; }

        public int ClasesDisponibles { get; set; }
        public int ClasesUsadas { get; set; }
        public DateTime FechaActivacion { get; set; }
        public DateTime FechaVencimiento { get; set; }

        public int IdEstado { get; set; }
        public EstadoPaquete Estado { get; set; } = null!;

        public Guid IdTipoPaquete { get; set; }
        public TipoPaquete TipoPaquete { get; set; } = null!;
        public decimal ValorPaquete { get; set; }

        // Relaciones
        public ICollection<CongelacionPaquete> Congelaciones { get; set; } = new List<CongelacionPaquete>();
    }
}


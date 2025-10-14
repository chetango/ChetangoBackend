namespace Chetango.Domain.Entities.Estados
{
    // Representa un paquete de clases adquirido por un Alumno.
    // TODO: agregar métodos de dominio (UsarClase, Congelar, Descongelar) y validar invariantes de estado y conteo.
    public class Paquete
    {
        public Guid IdPaquete { get; set; }

        // Identifica al Alumno dueño del paquete (relación unidireccional por ahora)
        public Guid IdAlumno { get; set; }
        public Chetango.Domain.Entities.Alumno Alumno { get; set; } = null!;

        // Pago origen (puede ser nulo si es cortesía o ajuste administrativo)
        public Guid? IdPago { get; set; }
        public Pago? Pago { get; set; }

        public int ClasesDisponibles { get; set; } // Total asignadas
        public int ClasesUsadas { get; set; }      // Consumidas (invariante futura: <= ClasesDisponibles)
        public DateTime FechaActivacion { get; set; }
        public DateTime FechaVencimiento { get; set; } // Considerar extensión por congelaciones

        public int IdEstado { get; set; } // FK a catálogo EstadoPaquete (Activo, Vencido, etc.)
        public EstadoPaquete Estado { get; set; } = null!;

        public Guid IdTipoPaquete { get; set; }
        public TipoPaquete TipoPaquete { get; set; } = null!;
        public decimal ValorPaquete { get; set; }

        // Historial de intervalos donde el paquete estuvo congelado (pausas)
        public ICollection<CongelacionPaquete> Congelaciones { get; set; } = new List<CongelacionPaquete>();
    }
}


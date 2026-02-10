using Chetango.Domain.Entities.Estados;

namespace Chetango.Domain.Entities
{
    public class ClaseProfesor
    {
        public Guid IdClaseProfesor { get; set; }
        public Guid IdClase { get; set; }
        public Clase Clase { get; set; } = null!;
        public Guid IdProfesor { get; set; }
        public Profesor Profesor { get; set; } = null!;
        public Guid IdRolEnClase { get; set; }
        public RolEnClase RolEnClase { get; set; } = null!;
        
        // Tarifas y Pagos
        public decimal TarifaProgramada { get; set; } // Calculada al crear clase
        public decimal ValorAdicional { get; set; } = 0; // Ajustes manuales
        public string? ConceptoAdicional { get; set; } // Razón del ajuste
        public decimal TotalPago { get; set; } // TarifaProgramada + ValorAdicional
        
        // Estado del Pago
        public string EstadoPago { get; set; } = "Pendiente"; // Pendiente/Aprobado/Liquidado/Pagado
        public DateTime? FechaAprobacion { get; set; }
        public DateTime? FechaPago { get; set; }
        public Guid? AprobadoPorIdUsuario { get; set; }
        public Usuario? AprobadoPor { get; set; }
        
        // Auditoría
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}

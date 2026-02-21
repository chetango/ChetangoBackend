using Chetango.Domain.Entities.Estados;
using Chetango.Domain.Enums;

namespace Chetango.Domain.Entities
{
    public class Pago
    {
        public Guid IdPago { get; set; }

        public Guid? IdAlumno { get; set; }
        public Alumno? Alumno { get; set; }

        public DateTime FechaPago { get; set; }
        public decimal MontoTotal { get; set; }
        public Sede Sede { get; set; } = Sede.Medellin;
        public Guid IdMetodoPago { get; set; }
        public MetodoPago MetodoPago { get; set; } = null!;
        public string? Nota { get; set; }

        // Verificación de pagos
        public Guid IdEstadoPago { get; set; }
        public EstadoPago EstadoPago { get; set; } = null!;
        public string? UrlComprobante { get; set; }
        public string? ReferenciaTransferencia { get; set; }
        public string? NotasVerificacion { get; set; }
        public DateTime? FechaVerificacion { get; set; }
        public string? UsuarioVerificacion { get; set; }

        // Campos de auditoría
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioCreacion { get; set; } = null!;
        public string? UsuarioModificacion { get; set; }

        // Soft Delete
        public bool Eliminado { get; set; } = false;
        public DateTime? FechaEliminacion { get; set; }
        public string? UsuarioEliminacion { get; set; }

        // Relaciones
        public ICollection<Paquete> Paquetes { get; set; } = new List<Paquete>();
    }
}

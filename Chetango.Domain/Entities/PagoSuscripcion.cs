namespace Chetango.Domain.Entities
{
    /// <summary>
    /// Representa un pago de suscripción mensual de una academia.
    /// Incluye aprobación manual de comprobantes en Fase 1.
    /// </summary>
    public class PagoSuscripcion
    {
        public Guid Id { get; set; }
        
        // Relación con tenant
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        
        // Información del pago
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Referencia { get; set; } = null!; // Ej: APHE-CHETANGO-2026-03
        public string MetodoPago { get; set; } = null!; // Transferencia, Wompi, Stripe
        
        // Comprobante (solo para transferencias)
        public string? ComprobanteUrl { get; set; }
        public string? NombreArchivo { get; set; }
        public int? TamanoArchivo { get; set; } // En bytes
        
        // Estado y aprobación
        public string Estado { get; set; } = null!; // Pendiente, Aprobado, Rechazado
        public string? AprobadoPor { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string? Observaciones { get; set; } // Razón de rechazo u otros comentarios
        
        // Datos de transacción (para pagos automáticos Fase 3)
        public string? TransaccionId { get; set; } // ID de transacción de Wompi/Stripe
        public string? EstadoTransaccion { get; set; } // APPROVED, DECLINED, etc.
        
        // Auditoría
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? CreadoPor { get; set; }
        public string? ModificadoPor { get; set; }
    }
}

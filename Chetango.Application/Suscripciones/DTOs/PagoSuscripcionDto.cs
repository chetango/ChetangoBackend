namespace Chetango.Application.Suscripciones.DTOs
{
    /// <summary>
    /// DTO con información de un pago de suscripción en el historial.
    /// </summary>
    public class PagoSuscripcionDto
    {
        public Guid Id { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Referencia { get; set; } = null!;
        public string MetodoPago { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string? ComprobanteUrl { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string? Observaciones { get; set; }
        
        // Para panel de administración (SuperAdmin)
        public string? NombreAcademia { get; set; }
        public string? Subdomain { get; set; }
    }
}

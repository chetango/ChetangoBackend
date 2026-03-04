namespace Chetango.Application.Suscripciones.DTOs
{
    /// <summary>
    /// DTO para subir un comprobante de pago de suscripción.
    /// </summary>
    public class SubirComprobanteDto
    {
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Referencia { get; set; } = null!;
        // El archivo se maneja por separado en el controlador (IFormFile)
    }
}

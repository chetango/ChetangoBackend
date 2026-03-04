namespace Chetango.Application.Suscripciones.DTOs
{
    /// <summary>
    /// DTO con información bancaria para realizar pagos por transferencia.
    /// </summary>
    public class ConfiguracionPagoDto
    {
        public string Banco { get; set; } = null!;
        public string TipoCuenta { get; set; } = null!;
        public string NumeroCuenta { get; set; } = null!;
        public string Titular { get; set; } = null!;
        public string? NIT { get; set; }
    }
}

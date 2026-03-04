namespace Chetango.Domain.Entities
{
    /// <summary>
    /// Almacena la configuración de datos bancarios para transferencias manuales.
    /// Es una tabla de configuración global (sin TenantId).
    /// </summary>
    public class ConfiguracionPago
    {
        public int Id { get; set; }
        
        // Datos bancarios
        public string Banco { get; set; } = null!;
        public string TipoCuenta { get; set; } = null!; // Ahorros, Corriente
        public string NumeroCuenta { get; set; } = null!;
        public string Titular { get; set; } = null!;
        public string? NIT { get; set; }
        
        // Configuración
        public bool Activo { get; set; } = true;
        public bool MostrarEnPortal { get; set; } = true;
        
        // Auditoría
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? CreadoPor { get; set; }
        public string? ModificadoPor { get; set; }
    }
}

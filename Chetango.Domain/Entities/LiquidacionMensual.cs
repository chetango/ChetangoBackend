using Chetango.Domain.Entities.Estados;
using Chetango.Domain.Enums;

namespace Chetango.Domain.Entities
{
    public class LiquidacionMensual
    {
        public Guid IdLiquidacion { get; set; }
        public Guid IdProfesor { get; set; }
        public Profesor Profesor { get; set; } = null!;
        
        public int Mes { get; set; } // 1-12
        public int Año { get; set; }
        public Sede Sede { get; set; } = Sede.Medellin;
        
        public int TotalClases { get; set; }
        public decimal TotalHoras { get; set; }
        public decimal TotalBase { get; set; }
        public decimal TotalAdicionales { get; set; }
        public decimal TotalPagar { get; set; }
        
        public string Estado { get; set; } = "EnProceso"; // EnProceso/Cerrada/Pagada
        public DateTime? FechaCierre { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? Observaciones { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public Guid CreadoPorIdUsuario { get; set; }
        public Usuario? CreadoPor { get; set; }
    }
}

using Chetango.Domain.Entities.Estados;
using Chetango.Domain.Enums;

namespace Chetango.Domain.Entities
{
    /// <summary>
    /// Representa gastos operacionales de la academia que NO son nómina de profesores.
    /// Ejemplos: arriendo, servicios públicos (luz, agua, internet), mantenimiento, marketing, suministros, equipamiento, impuestos.
    /// </summary>
    public class OtroGasto
    {
        public Guid IdOtroGasto { get; set; }

        /// <summary>
        /// Descripción del gasto. Ejemplo: "Arriendo local febrero", "Factura luz", "Reparación espejo"
        /// </summary>
        public string Concepto { get; set; } = null!;

        /// <summary>
        /// Monto del gasto
        /// </summary>
        public decimal Monto { get; set; }

        /// <summary>
        /// Fecha en que se realizó el gasto
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Sede donde se generó el gasto
        /// </summary>
        public Sede Sede { get; set; } = Sede.Medellin;

        /// <summary>
        /// Categoría del gasto (FK opcional)
        /// </summary>
        public Guid? IdCategoriaGasto { get; set; }
        public CategoriaGasto? CategoriaGasto { get; set; }

        /// <summary>
        /// Proveedor o empresa a la que se le pagó
        /// </summary>
        public string? Proveedor { get; set; }

        /// <summary>
        /// Descripción adicional o notas sobre el gasto
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// URL de la factura o comprobante almacenado en Azure Blob Storage
        /// </summary>
        public string? UrlFactura { get; set; }

        /// <summary>
        /// Número de factura o referencia del proveedor
        /// </summary>
        public string? NumeroFactura { get; set; }

        // Campos de auditoría
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioCreacion { get; set; } = null!;
        public string? UsuarioModificacion { get; set; }

        // Soft Delete
        public bool Eliminado { get; set; } = false;
        public DateTime? FechaEliminacion { get; set; }
        public string? UsuarioEliminacion { get; set; }
    }
}

using Chetango.Domain.Entities.Estados;
using Chetango.Domain.Enums;

namespace Chetango.Domain.Entities
{
    /// <summary>
    /// Representa ingresos adicionales de la academia que NO provienen de pagos de alumnos.
    /// Ejemplos: shows, eventos especiales, alquiler de espacio, venta de mercancía, patrocinios, talleres externos.
    /// </summary>
    public class OtroIngreso
    {
        public Guid IdOtroIngreso { get; set; }

        /// <summary>
        /// Descripción del ingreso. Ejemplo: "Show privado evento corporativo", "Venta de camisetas"
        /// </summary>
        public string Concepto { get; set; } = null!;

        /// <summary>
        /// Monto del ingreso
        /// </summary>
        public decimal Monto { get; set; }

        /// <summary>
        /// Fecha en que se recibió el ingreso
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Sede donde se generó el ingreso
        /// </summary>
        public Sede Sede { get; set; } = Sede.Medellin;

        /// <summary>
        /// Categoría del ingreso (FK opcional)
        /// </summary>
        public Guid? IdCategoriaIngreso { get; set; }
        public CategoriaIngreso? CategoriaIngreso { get; set; }

        /// <summary>
        /// Descripción adicional o notas sobre el ingreso
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// URL del comprobante o recibo almacenado en Azure Blob Storage
        /// </summary>
        public string? UrlComprobante { get; set; }

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

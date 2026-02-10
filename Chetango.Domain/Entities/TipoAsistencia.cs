namespace Chetango.Domain.Entities;

/// <summary>
/// Catálogo de tipos de asistencia que define el comportamiento de descuento de paquetes
/// </summary>
public class TipoAsistencia
{
    public int IdTipoAsistencia { get; set; }
    
    public string Nombre { get; set; } = null!;
    
    public string Descripcion { get; set; } = null!;
    
    /// <summary>
    /// Indica si este tipo de asistencia requiere que el alumno tenga un paquete activo
    /// </summary>
    public bool RequierePaquete { get; set; }
    
    /// <summary>
    /// Indica si este tipo de asistencia descuenta una clase del paquete (solo aplica si RequierePaquete = true)
    /// </summary>
    public bool DescontarClase { get; set; }
    
    public bool Activo { get; set; }
}

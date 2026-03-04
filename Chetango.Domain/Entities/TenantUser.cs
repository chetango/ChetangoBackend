namespace Chetango.Domain.Entities;

/// <summary>
/// Tabla de relación many-to-many entre Usuarios y Tenants.
/// Un usuario puede pertenecer a múltiples academias (tenants).
/// Ejemplo: Carlos puede ser profesor en Chetango y en Salsa Latina.
/// </summary>
public class TenantUser
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid IdUsuario { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public bool Activo { get; set; }
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}

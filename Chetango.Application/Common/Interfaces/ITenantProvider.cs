namespace Chetango.Application.Common.Interfaces;

/// <summary>
/// Proveedor de TenantId para el request actual.
/// Se registra como Scoped para que cada request tenga su propia instancia.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Obtiene el TenantId del request actual (resuelto desde el subdomain).
    /// </summary>
    Guid? GetCurrentTenantId();
    
    /// <summary>
    /// Establece el TenantId para el request actual.
    /// </summary>
    void SetTenantId(Guid? tenantId);
}

using Chetango.Application.Common.Interfaces;

namespace Chetango.Infrastructure.Services;

/// <summary>
/// Implementación de ITenantProvider que almacena el TenantId por request.
/// Esta clase se registra como Scoped, por lo que cada HTTP request tiene su propia instancia.
/// </summary>
public class TenantProvider : ITenantProvider
{
    private Guid? _tenantId;
    
    public Guid? GetCurrentTenantId()
    {
        return _tenantId;
    }
    
    public void SetTenantId(Guid? tenantId)
    {
        _tenantId = tenantId;
    }
}

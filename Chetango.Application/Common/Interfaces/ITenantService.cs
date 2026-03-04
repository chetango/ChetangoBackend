namespace Chetango.Application.Common.Interfaces
{
    /// <summary>
    /// Servicio para resolver el TenantId del contexto actual.
    /// Usado para multi-tenancy y aislamiento de datos entre academias.
    /// </summary>
    public interface ITenantService
    {
        /// <summary>
        /// Obtiene el TenantId del usuario autenticado actual.
        /// </summary>
        /// <returns>TenantId o null si no está disponible</returns>
        Guid? GetCurrentTenantId();
        
        /// <summary>
        /// Establece el TenantId del contexto actual (usado en tests o background jobs).
        /// </summary>
        void SetTenantId(Guid tenantId);
    }
}

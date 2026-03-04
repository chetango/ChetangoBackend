using Chetango.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace Chetango.Api.Infrastructure.Middleware;

/// <summary>
/// Middleware que resuelve el TenantId desde el subdomain del request.
/// Ejemplo: academia1.aphelion.com -> busca tenant con subdomain "academia1"
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public TenantResolutionMiddleware(
        RequestDelegate next,
        ILogger<TenantResolutionMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        try
        {
            var host = context.Request.Host.Host;
            _logger.LogInformation("Resolviendo tenant para host: {Host}", host);

            // Extraer subdomain (primera parte antes del primer punto)
            var subdomain = ExtractSubdomain(host);
            
            if (string.IsNullOrEmpty(subdomain))
            {
                _logger.LogWarning("No se pudo extraer subdomain del host: {Host}", host);
                // Si no hay subdomain, continuar sin tenant (super admin puede no tener subdomain)
                await _next(context);
                return;
            }

            // Buscar tenant por subdomain
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var tenantId = await GetTenantIdBySubdomainAsync(subdomain, connectionString);

            if (tenantId.HasValue)
            {
                tenantProvider.SetTenantId(tenantId.Value);
                _logger.LogInformation("Tenant resuelto: {TenantId} para subdomain: {Subdomain}", 
                    tenantId.Value, subdomain);
            }
            else
            {
                _logger.LogWarning("No se encontró tenant para subdomain: {Subdomain}", subdomain);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al resolver tenant");
        }

        await _next(context);
    }

    private string ExtractSubdomain(string host)
    {
        // Ejemplo: academia1.aphelion.com -> "academia1"
        // Ejemplo: salsalatina.aphelion.com -> "salsalatina"
        // Ejemplo: localhost -> null (desarrollo local)
        
        if (host.StartsWith("localhost", StringComparison.OrdinalIgnoreCase) || 
            host.StartsWith("127.0.0.1"))
        {
            // En desarrollo local, usar subdomain desde query string o header
            return null;
        }

        var parts = host.Split('.');
        if (parts.Length >= 3)
        {
            return parts[0]; // Primera parte es el subdomain
        }

        return null;
    }

    private async Task<Guid?> GetTenantIdBySubdomainAsync(string subdomain, string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand(
            "SELECT Id FROM Tenants WHERE Subdomain = @Subdomain AND Estado = 'Activo'", 
            connection);
        command.Parameters.AddWithValue("@Subdomain", subdomain);

        var result = await command.ExecuteScalarAsync();
        return result != null ? (Guid?)result : null;
    }
}

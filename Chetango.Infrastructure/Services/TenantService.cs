using System.Security.Claims;
using Chetango.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Chetango.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de resolución de TenantId.
    /// Obtiene el TenantId basado en el dominio del email del usuario autenticado.
    /// Auto-provisiona tenants nuevos cuando se detecta un dominio no registrado.
    /// </summary>
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        private Guid? _tenantId;

        public TenantService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration.GetConnectionString("ChetangoConnection") 
                ?? throw new InvalidOperationException("ChetangoConnection no está configurada");
        }

        public Guid? GetCurrentTenantId()
        {
            // Si ya se resolvió en esta request, devolver el cacheado
            if (_tenantId.HasValue)
                return _tenantId;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            // Obtener email del usuario autenticado
            var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value 
                         ?? httpContext.User.FindFirst("preferred_username")?.Value
                         ?? httpContext.User.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(userEmail))
                return null;

            // Extraer dominio del email
            var emailParts = userEmail.Split('@');
            if (emailParts.Length != 2)
                return null;

            var domain = emailParts[1];

            // Buscar o crear tenant por dominio usando SQL directo (evita dependencia circular con DbContext)
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                
                // Buscar tenant existente por dominio
                using var selectCommand = new SqlCommand("SELECT IdTenant FROM Tenants WHERE Dominio = @Dominio", connection);
                selectCommand.Parameters.AddWithValue("@Dominio", domain);
                
                var result = selectCommand.ExecuteScalar();
                
                if (result != null)
                {
                    // Tenant existe, devolver su ID
                    _tenantId = (Guid)result;
                    return _tenantId;
                }

                // Tenant NO existe, crear automáticamente (auto-provisioning)
                var newTenantId = Guid.NewGuid();
                var tenantName = ExtractTenantNameFromDomain(domain);
                
                using var insertCommand = new SqlCommand(
                    @"INSERT INTO Tenants (Id, Nombre, Subdomain, Dominio, Plan, Estado, FechaRegistro, 
                                           MaxSedes, MaxAlumnos, MaxProfesores, MaxStorageMB, 
                                           EmailContacto, FechaCreacion) 
                      VALUES (@Id, @Nombre, @Subdomain, @Dominio, @Plan, @Estado, @FechaRegistro,
                              @MaxSedes, @MaxAlumnos, @MaxProfesores, @MaxStorageMB,
                              @EmailContacto, @FechaCreacion)", 
                    connection);
                
                insertCommand.Parameters.AddWithValue("@Id", newTenantId);
                insertCommand.Parameters.AddWithValue("@Nombre", tenantName);
                insertCommand.Parameters.AddWithValue("@Subdomain", domain.Split('.')[0]); // Primer parte del dominio
                insertCommand.Parameters.AddWithValue("@Dominio", domain);
                insertCommand.Parameters.AddWithValue("@Plan", "Basico");
                insertCommand.Parameters.AddWithValue("@Estado", "Activo");
                insertCommand.Parameters.AddWithValue("@FechaRegistro", DateTime.UtcNow);
                insertCommand.Parameters.AddWithValue("@MaxSedes", 3);
                insertCommand.Parameters.AddWithValue("@MaxAlumnos", 100);
                insertCommand.Parameters.AddWithValue("@MaxProfesores", 10);
                insertCommand.Parameters.AddWithValue("@MaxStorageMB", 1000);
                insertCommand.Parameters.AddWithValue("@EmailContacto", $"admin@{domain}");
                insertCommand.Parameters.AddWithValue("@FechaCreacion", DateTime.UtcNow);
                
                insertCommand.ExecuteNonQuery();
                
                _tenantId = newTenantId;
                return _tenantId;
            }
            catch
            {
                // En caso de error, devolver null
                return null;
            }
        }

        /// <summary>
        /// Extrae un nombre amigable del dominio para el tenant auto-provisionado.
        /// Ejemplo: "salsalatina.aphelion.com" -> "Salsa Latina"
        /// </summary>
        private string ExtractTenantNameFromDomain(string domain)
        {
            var subdomain = domain.Split('.')[0];
            
            // Convertir a título (primera letra mayúscula, separar por camel case)
            var name = string.Concat(subdomain.Select((ch, i) => 
                i > 0 && char.IsUpper(ch) ? " " + ch : ch.ToString()));
            
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        public void SetTenantId(Guid tenantId)
        {
            _tenantId = tenantId;
        }
    }
}

using Chetango.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace Chetango.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor que establece SESSION_CONTEXT con el TenantId antes de abrir cada conexión SQL.
/// Esto permite que Row-Level Security en SQL Server filtre automáticamente los datos.
/// <para>
/// Se usa <c>@read_only = 0</c> intencionalmente para que las conexiones reutilizadas del pool
/// puedan actualizar el TenantId entre requests de distintos tenants. Con <c>@read_only = 1</c>
/// la primera asignación sería permanente en la conexión, causando errores en escenarios multi-tenant.
/// </para>
/// </summary>
public class TenantDbConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<TenantDbConnectionInterceptor> _logger;

    public TenantDbConnectionInterceptor(
        ITenantProvider tenantProvider,
        ILogger<TenantDbConnectionInterceptor> logger)
    {
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await SetSessionContextAsync(connection, cancellationToken);
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        SetSessionContext(connection);
        base.ConnectionOpened(connection, eventData);
    }

    private async Task SetSessionContextAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        
        if (tenantId.HasValue && connection is SqlConnection sqlConnection)
        {
            try
            {
                var command = sqlConnection.CreateCommand();
                command.CommandText = "EXEC sp_set_session_context @key = N'TenantId', @value = @tenantId, @read_only = 0";
                command.Parameters.Add(new SqlParameter("@tenantId", SqlDbType.UniqueIdentifier) { Value = tenantId.Value });
                
                await command.ExecuteNonQueryAsync(cancellationToken);
                
                _logger.LogDebug("SESSION_CONTEXT configurado con TenantId: {TenantId}", tenantId.Value);
            }
            catch (Exception ex)
            {
                // No propagar: si sp_set_session_context falla el request debe continuar.
                // RLS no filtrará por tenant en esta conexión, pero es preferible a un 500 sin CORS.
                _logger.LogError(ex, "Error al configurar SESSION_CONTEXT con TenantId: {TenantId}", tenantId.Value);
            }
        }
        else if (!tenantId.HasValue)
        {
            _logger.LogWarning("No hay TenantId en el contexto actual. SESSION_CONTEXT no configurado.");
        }
    }

    private void SetSessionContext(DbConnection connection)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        
        if (tenantId.HasValue && connection is SqlConnection sqlConnection)
        {
            try
            {
                var command = sqlConnection.CreateCommand();
                command.CommandText = "EXEC sp_set_session_context @key = N'TenantId', @value = @tenantId, @read_only = 0";
                command.Parameters.Add(new SqlParameter("@tenantId", SqlDbType.UniqueIdentifier) { Value = tenantId.Value });
                
                command.ExecuteNonQuery();
                
                _logger.LogDebug("SESSION_CONTEXT configurado con TenantId: {TenantId}", tenantId.Value);
            }
            catch (Exception ex)
            {
                // No propagar: si sp_set_session_context falla el request debe continuar.
                _logger.LogError(ex, "Error al configurar SESSION_CONTEXT con TenantId: {TenantId}", tenantId.Value);
            }
        }
        else if (!tenantId.HasValue)
        {
            _logger.LogWarning("No hay TenantId en el contexto actual. SESSION_CONTEXT no configurado.");
        }
    }
}

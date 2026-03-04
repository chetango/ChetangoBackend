using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chetango.Application.Admin.Commands.AssignUserToTenant;

public class AssignUserToTenantCommandHandler : IRequestHandler<AssignUserToTenantCommand, Guid>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<AssignUserToTenantCommandHandler> _logger;

    public AssignUserToTenantCommandHandler(
        IAppDbContext context,
        ILogger<AssignUserToTenantCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> Handle(AssignUserToTenantCommand request, CancellationToken cancellationToken)
    {
        // Validar que exista el tenant
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant == null)
        {
            throw new InvalidOperationException($"No existe tenant con ID '{request.TenantId}'");
        }

        // Validar que exista el usuario
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuario, cancellationToken);

        if (usuario == null)
        {
            throw new InvalidOperationException($"No existe usuario con ID '{request.IdUsuario}'");
        }

        // Validar que no exista ya la asignación
        var existeAsignacion = await _context.TenantUsers
            .AnyAsync(tu => tu.TenantId == request.TenantId && tu.IdUsuario == request.IdUsuario, 
                cancellationToken);

        if (existeAsignacion)
        {
            throw new InvalidOperationException(
                $"El usuario '{usuario.NombreUsuario}' ya está asignado al tenant '{tenant.Nombre}'");
        }

        var tenantUser = new TenantUser
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            IdUsuario = request.IdUsuario,
            FechaAsignacion = DateTime.UtcNow,
            Activo = true
        };

        _context.TenantUsers.Add(tenantUser);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Usuario asignado a tenant: Usuario={UsuarioId}, Tenant={TenantId}", 
            request.IdUsuario, request.TenantId);

        return tenantUser.Id;
    }
}

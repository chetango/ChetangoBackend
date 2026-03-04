using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chetango.Application.Admin.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateTenantCommandHandler> _logger;

    public CreateTenantCommandHandler(
        IAppDbContext context,
        ILogger<CreateTenantCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Validar que no exista tenant con mismo subdomain
        var existeSubdomain = await _context.Tenants
            .AnyAsync(t => t.Subdomain == request.Subdomain, cancellationToken);

        if (existeSubdomain)
        {
            throw new InvalidOperationException($"Ya existe un tenant con subdomain '{request.Subdomain}'");
        }

        // Validar que no exista tenant con mismo dominio
        var existeDominio = await _context.Tenants
            .AnyAsync(t => t.Dominio == request.Dominio, cancellationToken);

        if (existeDominio)
        {
            throw new InvalidOperationException($"Ya existe un tenant con dominio '{request.Dominio}'");
        }

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Subdomain = request.Subdomain,
            Dominio = request.Dominio,
            Plan = request.Plan,
            Estado = "Activo",
            FechaRegistro = DateTime.UtcNow,
            MaxSedes = request.MaxSedes,
            MaxAlumnos = request.MaxAlumnos,
            MaxProfesores = request.MaxProfesores,
            MaxStorageMB = request.MaxStorageMB,
            EmailContacto = request.EmailContacto,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tenant creado exitosamente: {TenantId} - {Nombre} ({Subdomain})", 
            tenant.Id, tenant.Nombre, tenant.Subdomain);

        return tenant.Id;
    }
}

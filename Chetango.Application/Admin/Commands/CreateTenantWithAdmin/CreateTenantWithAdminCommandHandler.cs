using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using Chetango.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chetango.Application.Admin.Commands.CreateTenantWithAdmin;

public class CreateTenantWithAdminCommandHandler : IRequestHandler<CreateTenantWithAdminCommand, CreateTenantWithAdminResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateTenantWithAdminCommandHandler> _logger;

    public CreateTenantWithAdminCommandHandler(
        IAppDbContext context,
        ILogger<CreateTenantWithAdminCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CreateTenantWithAdminResponse> Handle(CreateTenantWithAdminCommand request, CancellationToken cancellationToken)
    {
        // ===================================================================
        // 1. VALIDAR QUE NO EXISTA TENANT CON MISMO SUBDOMAIN
        // ===================================================================
        var existeSubdomain = await _context.Tenants
            .AnyAsync(t => t.Subdomain == request.Subdomain, cancellationToken);

        if (existeSubdomain)
        {
            throw new InvalidOperationException($"Ya existe una academia con subdomain '{request.Subdomain}'");
        }

        // ===================================================================
        // 2. CREAR TENANT
        // ===================================================================
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Nombre = request.NombreTenant,
            Subdomain = request.Subdomain,
            Dominio = request.DominioCompleto,
            Plan = request.Plan,
            Estado = "Activo",
            FechaRegistro = DateTime.UtcNow,
            MaxSedes = request.MaxSedes,
            MaxAlumnos = request.MaxAlumnos,
            MaxProfesores = request.MaxProfesores,
            MaxStorageMB = request.MaxStorageMB,
            EmailContacto = request.CorreoAdmin,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tenant creado: {TenantId} - {Nombre} ({Subdomain})", 
            tenant.Id, tenant.Nombre, tenant.Subdomain);

        // ===================================================================
        // 3. BUSCAR O CREAR USUARIO EN BASE DE DATOS
        // ===================================================================
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == request.CorreoAdmin, cancellationToken);

        if (usuario == null)
        {
            // Usuario no existe en DB, crearlo
            usuario = new Usuario
            {
                IdUsuario = Guid.NewGuid(),
                NombreUsuario = request.NombreUsuario,
                Correo = request.CorreoAdmin,
                IdTipoDocumento = request.IdTipoDocumento,
                NumeroDocumento = request.NumeroDocumento,
                Telefono = request.Telefono,
                IdEstadoUsuario = 1, // Activo
                Sede = Sede.Medellin, // Por defecto
                TenantId = tenant.Id, // Asignar al tenant que se está creando
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Usuario creado: {IdUsuario} - {NombreUsuario} ({Correo})", 
                usuario.IdUsuario, usuario.NombreUsuario, usuario.Correo);
        }
        else
        {
            _logger.LogInformation("Usuario ya existe: {IdUsuario} - {NombreUsuario} ({Correo})", 
                usuario.IdUsuario, usuario.NombreUsuario, usuario.Correo);
        }

        // ===================================================================
        // 4. ASIGNAR USUARIO AL TENANT (TenantUsers)
        // ===================================================================
        var existeAsignacion = await _context.TenantUsers
            .AnyAsync(tu => tu.TenantId == tenant.Id && tu.IdUsuario == usuario.IdUsuario, 
                cancellationToken);

        if (!existeAsignacion)
        {
            var tenantUser = new TenantUser
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                IdUsuario = usuario.IdUsuario,
                FechaAsignacion = DateTime.UtcNow,
                Activo = true
            };

            _context.TenantUsers.Add(tenantUser);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Usuario asignado al tenant: {IdUsuario} → {TenantId}", 
                usuario.IdUsuario, tenant.Id);
        }

        // ===================================================================
        // 5. CREAR SEDE PREDETERMINADA "PRINCIPAL" PARA EL NUEVO TENANT
        //    Cada academia comienza con una sola sede genérica.
        //    Si el tenant necesita más sedes, se agregan desde el panel de administración.
        // ===================================================================
        var existeSedeDefault = await _context.SedeConfigs
            .AnyAsync(s => s.TenantId == tenant.Id, cancellationToken);

        if (!existeSedeDefault)
        {
            var sedeDefault = new SedeConfig
            {
                Id          = Guid.NewGuid(),
                TenantId    = tenant.Id,
                SedeValor   = (int)Sede.Medellin, // valor 1, reutiliza el slot del enum
                Nombre      = "Principal",
                Activa      = true,
                EsDefault   = true,
                Orden       = 1,
                FechaCreacion = DateTime.UtcNow
            };

            _context.SedeConfigs.Add(sedeDefault);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Sede predeterminada creada para tenant: {TenantId}", tenant.Id);
        }

        // ===================================================================
        // 6. CREAR TIPOS DE CLASE BASE PARA EL NUEVO TENANT
        //    Cada academia empieza con tipos genéricos que puede renombrar o
        //    ampliar desde la configuración de la plataforma.
        // ===================================================================
        var tieneTiposClase = await _context.Set<TipoClase>()
            .AnyAsync(tc => tc.TenantId == tenant.Id, cancellationToken);

        if (!tieneTiposClase)
        {
            var tiposClaseDefault = new[]
            {
                new TipoClase { Id = Guid.NewGuid(), TenantId = tenant.Id, Nombre = "General"  },
                new TipoClase { Id = Guid.NewGuid(), TenantId = tenant.Id, Nombre = "Privada"  },
                new TipoClase { Id = Guid.NewGuid(), TenantId = tenant.Id, Nombre = "Taller"   },
                new TipoClase { Id = Guid.NewGuid(), TenantId = tenant.Id, Nombre = "Evento"   },
            };
            _context.Set<TipoClase>().AddRange(tiposClaseDefault);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // ===================================================================
        // 7. RETORNAR RESULTADO
        // ===================================================================
        return new CreateTenantWithAdminResponse
        {
            TenantId = tenant.Id,
            IdUsuario = usuario.IdUsuario,
            Mensaje = $"Academia '{tenant.Nombre}' creada exitosamente con administrador '{usuario.NombreUsuario}'. " +
                      $"URL: https://{tenant.Subdomain}.aphellion.com"
        };
    }
}

// ============================================
// GET PERFIL ADMIN QUERY
// ============================================

using Chetango.Application.Admin.DTOs;
using Chetango.Application.Common;
using Chetango.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chetango.Application.Admin.Queries;

public record GetPerfilAdminQuery(string EmailUsuario) : IRequest<Result<AdminProfileDTO>>;

public class GetPerfilAdminQueryHandler : IRequestHandler<GetPerfilAdminQuery, Result<AdminProfileDTO>>
{
    private readonly IAppDbContext    _db;
    private readonly ITenantProvider  _tenantProvider;

    public GetPerfilAdminQueryHandler(IAppDbContext db, ITenantProvider tenantProvider)
    {
        _db             = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<AdminProfileDTO>> Handle(GetPerfilAdminQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Chetango.Domain.Entities.Usuario>()
            .Include(u => u.TipoDocumento)
            .FirstOrDefaultAsync(u => u.Correo == request.EmailUsuario, cancellationToken);

        if (usuario == null)
            return Result<AdminProfileDTO>.Failure("Usuario administrador no encontrado");

        // ─── Resolver nombre de sede dinámicamente desde SedeConfigs ───────────────
        var sedeConfigs = await _db.SedeConfigs
            .Where(s => s.Activa)
            .ToListAsync(cancellationToken);

        var sedeConfig = sedeConfigs.FirstOrDefault(s => s.SedeValor == (int)usuario.Sede);
        var sedeNombre = sedeConfig?.Nombre
            ?? (usuario.Sede == Domain.Enums.Sede.Medellin ? "Medellín" : "Manizales"); // fallback

        // ─── Resolver nombre de academia desde el Tenant ─────────────────────────
        var tenantId = _tenantProvider.GetCurrentTenantId();
        var tenant = tenantId.HasValue
            ? await _db.Tenants
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == tenantId.Value, cancellationToken)
            : null;

        var nombreAcademia  = tenant?.NombreComercial ?? tenant?.Nombre ?? "Mi Academia";
        var emailAcademia   = tenant?.EmailContacto   ?? string.Empty;
        var telefonoAcademia = tenant?.TelefonoContacto ?? string.Empty;

        var dto = new AdminProfileDTO(
            IdAdministrador: usuario.IdUsuario,
            NombreCompleto: usuario.NombreUsuario,
            Correo: usuario.Correo,
            Telefono: usuario.Telefono ?? string.Empty,
            DireccionPersonal: string.Empty,  // TODO: Agregar campo en BD
            FechaNacimiento: null,             // TODO: Agregar campo en BD
            TipoDocumento: usuario.TipoDocumento.Nombre,
            NumeroDocumento: usuario.NumeroDocumento ?? string.Empty,
            FechaIngreso: usuario.FechaCreacion,
            UltimaActividad: DateTime.Now,    // TODO: Implementar tracking real
            Cargo: "Administrador",
            Departamento: "Administración",
            Permisos: new List<string>
            {
                "Gestión de Clases",
                "Gestión de Paquetes",
                "Gestión de Pagos",
                "Gestión de Asistencias",
                "Reportes y Estadísticas",
                "Gestión de Usuarios"
            },
            Sede: usuario.Sede,
            SedeNombre: sedeNombre,
            DatosAcademia: new DatosAcademiaDTO(
                NombreAcademia:     nombreAcademia,
                Direccion:          "Dirección Principal",
                Telefono:           telefonoAcademia,
                EmailInstitucional: emailAcademia,
                Instagram:  null,
                Facebook:   null,
                WhatsApp:   null
            ),
            Configuracion: new ConfiguracionAdminDTO(
                NotificacionesEmail:          true,  // TODO: Leer de tabla ConfiguracionAdmin
                AlertasPagosPendientes:       true,
                ReportesAutomaticos:          false,
                AlertasPaquetesVencer:        true,
                AlertasAsistenciaBaja:        true,
                NotificacionesNuevosRegistros: true
            )
        );

        return Result<AdminProfileDTO>.Success(dto);
    }
}

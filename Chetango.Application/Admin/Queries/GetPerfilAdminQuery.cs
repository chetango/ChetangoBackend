// ============================================
// GET PERFIL ADMIN QUERY
// ============================================

using Chetango.Application.Admin.DTOs;
using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chetango.Application.Admin.Queries;

public record GetPerfilAdminQuery(string EmailUsuario) : IRequest<Result<AdminProfileDTO>>;

public class GetPerfilAdminQueryHandler : IRequestHandler<GetPerfilAdminQuery, Result<AdminProfileDTO>>
{
    private readonly IAppDbContext _db;

    public GetPerfilAdminQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<AdminProfileDTO>> Handle(GetPerfilAdminQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Chetango.Domain.Entities.Usuario>()
            .Include(u => u.TipoDocumento)
            .FirstOrDefaultAsync(u => u.Correo == request.EmailUsuario, cancellationToken);

        if (usuario == null)
            return Result<AdminProfileDTO>.Failure("Usuario administrador no encontrado");

        // Por ahora, datos hardcodeados para configuración y otros datos adicionales
        // Estos deberían venir de tablas adicionales en el futuro
        var dto = new AdminProfileDTO(
            IdAdministrador: usuario.IdUsuario,
            NombreCompleto: usuario.NombreUsuario,
            Correo: usuario.Correo,
            Telefono: usuario.Telefono ?? string.Empty,
            DireccionPersonal: string.Empty, // TODO: Agregar campo en BD
            FechaNacimiento: null, // TODO: Agregar campo en BD
            TipoDocumento: usuario.TipoDocumento.Nombre,
            NumeroDocumento: usuario.NumeroDocumento ?? string.Empty,
            FechaIngreso: usuario.FechaCreacion,
            UltimaActividad: DateTime.Now, // TODO: Implementar tracking real
            Cargo: "Administrador", // Hardcoded
            Departamento: "Administración", // Hardcoded
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
            SedeNombre: usuario.Sede == Domain.Enums.Sede.Medellin ? "Medellín" : "Manizales",
            DatosAcademia: new DatosAcademiaDTO(
                NombreAcademia: "Academia Chetango", // TODO: Leer de configuración global
                Direccion: "Dirección Principal",
                Telefono: "+57 300 123 4567",
                EmailInstitucional: "info@chetango.com",
                Instagram: "@chetango",
                Facebook: "ChetangoAcademia",
                WhatsApp: "+573001234567"
            ),
            Configuracion: new ConfiguracionAdminDTO(
                NotificacionesEmail: true, // TODO: Leer de tabla ConfiguracionAdmin
                AlertasPagosPendientes: true,
                ReportesAutomaticos: false,
                AlertasPaquetesVencer: true,
                AlertasAsistenciaBaja: true,
                NotificacionesNuevosRegistros: true
            )
        );

        return Result<AdminProfileDTO>.Success(dto);
    }
}
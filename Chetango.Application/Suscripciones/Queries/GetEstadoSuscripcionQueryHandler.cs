using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Handler para obtener el estado actual de la suscripción de un tenant.
/// </summary>
public class GetEstadoSuscripcionQueryHandler : IRequestHandler<GetEstadoSuscripcionQuery, Result<EstadoSuscripcionDto>>
{
    private readonly IAppDbContext _db;

    public GetEstadoSuscripcionQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<EstadoSuscripcionDto>> Handle(GetEstadoSuscripcionQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant == null)
        {
            return Result<EstadoSuscripcionDto>.Failure("Academia no encontrada.");
        }

        // Calcular días restantes
        int? diasRestantes = null;
        if (tenant.FechaVencimientoPlan.HasValue)
        {
            diasRestantes = (int)(tenant.FechaVencimientoPlan.Value.Date - DateTime.Today).TotalDays;
        }

        // Calcular uso actual (por ahora valores temporales, después se integrarán con las tablas reales)
        // TODO: Obtener contadores reales cuando se active multi-tenancy completo
        var sedesActuales = await _db.Usuarios
            .Select(u => u.Sede)
            .Distinct()
            .CountAsync(cancellationToken);

        var alumnosActivos = await _db.Alumnos
            .Include(a => a.Usuario)
            .CountAsync(a => a.Usuario.IdEstadoUsuario != 2, cancellationToken); // 2 = Inactivo

        var profesoresActivos = await _db.Profesores
            .Include(p => p.Usuario)
            .CountAsync(p => p.Usuario.IdEstadoUsuario != 2, cancellationToken); // 2 = Inactivo

        // Storage: por ahora simulado (después integrar con archivos de comprobantes/uploads)
        var storageMBUsado = 0;

        // Calcular progreso en porcentaje (máximo 100)
        var progresoCuotas = new ProgresoCuotasDto
        {
            Sedes = tenant.MaxSedes > 0 ? Math.Min(100, (sedesActuales * 100) / tenant.MaxSedes) : 0,
            Alumnos = tenant.MaxAlumnos > 0 ? Math.Min(100, (alumnosActivos * 100) / tenant.MaxAlumnos) : 0,
            Profesores = tenant.MaxProfesores > 0 ? Math.Min(100, (profesoresActivos * 100) / tenant.MaxProfesores) : 0,
            Storage = tenant.MaxStorageMB > 0 ? Math.Min(100, (storageMBUsado * 100) / tenant.MaxStorageMB) : 0
        };

        var resultado = new EstadoSuscripcionDto
        {
            TenantId = tenant.Id,
            NombreAcademia = tenant.Nombre,
            Plan = tenant.Plan,
            Estado = tenant.Estado,
            FechaRegistro = tenant.FechaRegistro,
            FechaVencimientoPlan = tenant.FechaVencimientoPlan,
            DiasRestantes = diasRestantes,
            MaxSedes = tenant.MaxSedes,
            MaxAlumnos = tenant.MaxAlumnos,
            MaxProfesores = tenant.MaxProfesores,
            MaxStorageMB = tenant.MaxStorageMB,
            SedesActuales = sedesActuales,
            AlumnosActivos = alumnosActivos,
            ProfesoresActivos = profesoresActivos,
            StorageMBUsado = storageMBUsado,
            ProgresoCuotas = progresoCuotas
        };

        return Result<EstadoSuscripcionDto>.Success(resultado);
    }
}

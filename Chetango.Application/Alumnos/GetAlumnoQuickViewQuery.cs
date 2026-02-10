// ============================================
// GET ALUMNO QUICK VIEW QUERY
// ============================================

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Alumnos;

/// <summary>
/// Query: Obtener información rápida de un alumno para Quick View
/// GET /api/alumnos/{idAlumno}/quick-view
/// Authorization: AdminOnly
/// </summary>
public record GetAlumnoQuickViewQuery(Guid IdAlumno) : IRequest<Result<AlumnoQuickViewDTO>>;

public class GetAlumnoQuickViewHandler : IRequestHandler<GetAlumnoQuickViewQuery, Result<AlumnoQuickViewDTO>>
{
    private readonly IAppDbContext _context;

    public GetAlumnoQuickViewHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AlumnoQuickViewDTO>> Handle(GetAlumnoQuickViewQuery request, CancellationToken cancellationToken)
    {
        var alumno = await _context.Set<Alumno>()
            .Include(a => a.Usuario)
            .Where(a => a.IdAlumno == request.IdAlumno)
            .FirstOrDefaultAsync(cancellationToken);

        if (alumno == null)
            return Result<AlumnoQuickViewDTO>.Failure("Alumno no encontrado");

        // Obtener paquetes activos
        var paquetesActivos = await _context.Set<Paquete>()
            .Where(p => p.IdAlumno == request.IdAlumno && p.IdEstado == 1) // 1 = Activo
            .CountAsync(cancellationToken);

        // Obtener último pago
        var ultimoPago = await _context.Set<Pago>()
            .Where(p => p.IdAlumno == request.IdAlumno)
            .OrderByDescending(p => p.FechaCreacion)
            .Select(p => new { p.FechaCreacion, p.MontoTotal })
            .FirstOrDefaultAsync(cancellationToken);

        // Obtener asistencias recientes (últimos 30 días)
        var asistenciasRecientes = await _context.Set<Asistencia>()
            .Include(a => a.Clase)
            .Where(a => a.IdAlumno == request.IdAlumno && a.Clase.Fecha >= DateTime.UtcNow.AddDays(-30))
            .CountAsync(cancellationToken);

        var dto = new AlumnoQuickViewDTO
        {
            IdAlumno = alumno.IdAlumno,
            IdUsuario = alumno.IdUsuario,
            Nombre = alumno.Usuario.NombreUsuario,
            Documento = alumno.Usuario.NumeroDocumento,
            Correo = alumno.Usuario.Correo,
            Telefono = alumno.Usuario.Telefono,
            FechaNacimiento = null,
            PaquetesActivos = paquetesActivos,
            ProximaClase = null,
            UltimoPago = ultimoPago != null ? new UltimoPagoDTO
            {
                Fecha = ultimoPago.FechaCreacion.ToString("dd MMM yyyy"),
                Monto = ultimoPago.MontoTotal
            } : null,
            AsistenciasRecientes = asistenciasRecientes
        };

        return Result<AlumnoQuickViewDTO>.Success(dto);
    }
}

/// <summary>
/// DTO para Quick View de Alumno
/// </summary>
public class AlumnoQuickViewDTO
{
    public Guid IdAlumno { get; set; }
    public Guid IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? FechaNacimiento { get; set; }
    public int PaquetesActivos { get; set; }
    public ProximaClaseDTO? ProximaClase { get; set; }
    public UltimoPagoDTO? UltimoPago { get; set; }
    public int AsistenciasRecientes { get; set; }
}

public class ProximaClaseDTO
{
    public string Fecha { get; set; } = string.Empty;
    public string Hora { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}

public class UltimoPagoDTO
{
    public string Fecha { get; set; } = string.Empty;
    public decimal Monto { get; set; }
}

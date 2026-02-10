// ============================================
// GET ALUMNO DETAIL QUERY - ADMIN
// ============================================

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Alumnos;

// Query
public record GetAlumnoDetailQuery(Guid IdAlumno) : IRequest<Result<AlumnoDetailDTO>>;

// DTO
public record AlumnoDetailDTO(
    Guid IdAlumno,
    Guid IdUsuario,
    string Nombre,
    string Correo,
    string? Telefono,
    string Documento,
    string TipoDocumento,
    string? Direccion,
    DateTime? FechaNacimiento,
    string Estado,
    DateTime FechaRegistro,
    int PaquetesActivos,
    int ClasesTomadas,
    double AsistenciaPromedio,
    AlumnoDetailUltimoPagoDTO? UltimoPago,
    List<PaqueteActivoDTO> PaquetesActivosDetalle
);

public record AlumnoDetailUltimoPagoDTO(
    Guid IdPago,
    decimal Monto,
    DateTime Fecha,
    string Concepto,
    string MetodoPago
);

public record PaqueteActivoDTO(
    Guid IdPaquete,
    string NombreTipoPaquete,
    int ClasesDisponibles,
    int ClasesUsadas,
    int ClasesRestantes,
    DateTime? FechaVencimiento
);

// Handler
public class GetAlumnoDetailQueryHandler : IRequestHandler<GetAlumnoDetailQuery, Result<AlumnoDetailDTO>>
{
    private readonly IAppDbContext _context;

    public GetAlumnoDetailQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AlumnoDetailDTO>> Handle(GetAlumnoDetailQuery request, CancellationToken cancellationToken)
    {
        // Obtener alumno con todas sus relaciones
        var alumno = await _context.Set<Alumno>()
            .Include(a => a.Usuario)
                .ThenInclude(u => u.TipoDocumento)
            .FirstOrDefaultAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (alumno == null)
            return Result<AlumnoDetailDTO>.Failure("Alumno no encontrado");

        // Contar paquetes activos
        var paquetesActivos = await _context.Set<Paquete>()
            .Where(p => p.IdAlumno == request.IdAlumno && p.IdEstado == 1) // 1 = Activo
            .CountAsync(cancellationToken);

        // Obtener detalle de paquetes activos
        var paquetesActivosDetalle = await _context.Set<Paquete>()
            .Include(p => p.TipoPaquete)
            .Where(p => p.IdAlumno == request.IdAlumno && p.IdEstado == 1)
            .Select(p => new PaqueteActivoDTO(
                p.IdPaquete,
                p.TipoPaquete.Nombre,
                p.ClasesDisponibles,
                p.ClasesUsadas,
                p.ClasesDisponibles - p.ClasesUsadas,
                p.FechaVencimiento
            ))
            .ToListAsync(cancellationToken);

        // Contar clases tomadas (asistencias confirmadas con estado Presente = 1)
        var clasesTomadas = await _context.Set<Asistencia>()
            .Where(a => a.IdAlumno == request.IdAlumno && a.IdEstado == 1) // 1 = Presente
            .CountAsync(cancellationToken);

        // Calcular promedio de asistencia
        var totalAsistencias = await _context.Set<Asistencia>()
            .Where(a => a.IdAlumno == request.IdAlumno)
            .CountAsync(cancellationToken);

        var asistenciasPresentes = await _context.Set<Asistencia>()
            .Where(a => a.IdAlumno == request.IdAlumno && a.IdEstado == 1) // 1 = Presente
            .CountAsync(cancellationToken);

        var asistenciaPromedio = totalAsistencias > 0 
            ? Math.Round((double)asistenciasPresentes / totalAsistencias * 100, 1)
            : 0;

        // Obtener último pago
        var ultimoPago = await _context.Set<Pago>()
            .Include(p => p.MetodoPago)
            .Include(p => p.Paquetes)
                .ThenInclude(paq => paq.TipoPaquete)
            .Where(p => p.IdAlumno == request.IdAlumno)
            .OrderByDescending(p => p.FechaPago)
            .Select(p => new AlumnoDetailUltimoPagoDTO(
                p.IdPago,
                p.MontoTotal,
                p.FechaPago,
                p.Paquetes.FirstOrDefault() != null 
                    ? p.Paquetes.First().TipoPaquete.Nombre 
                    : "Pago general",
                p.MetodoPago.Nombre
            ))
            .FirstOrDefaultAsync(cancellationToken);

        var dto = new AlumnoDetailDTO(
            alumno.IdAlumno,
            alumno.IdUsuario,
            alumno.Usuario.NombreUsuario,
            alumno.Usuario.Correo,
            alumno.Usuario.Telefono,
            alumno.Usuario.NumeroDocumento,
            alumno.Usuario.TipoDocumento?.Nombre ?? "N/A",
            null, // Direccion - no existe en Usuario
            null, // FechaNacimiento - no existe en Usuario
            alumno.Estado?.Nombre ?? "Activo",
            alumno.FechaInscripcion,
            paquetesActivos,
            clasesTomadas,
            asistenciaPromedio,
            ultimoPago,
            paquetesActivosDetalle
        );

        return Result<AlumnoDetailDTO>.Success(dto);
    }
}

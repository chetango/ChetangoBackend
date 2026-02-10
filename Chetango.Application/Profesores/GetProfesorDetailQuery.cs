// ============================================
// GET PROFESOR DETAIL QUERY - ADMIN
// ============================================

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Profesores;

// Query
public record GetProfesorDetailQuery(Guid IdProfesor) : IRequest<Result<ProfesorDetailDTO>>;

// DTO
public record ProfesorDetailDTO(
    Guid IdProfesor,
    Guid IdUsuario,
    string Nombre,
    string Correo,
    string? Telefono,
    string Documento,
    string TipoDocumento,
    string? Direccion,
    DateTime? FechaNacimiento,
    string TipoProfesor,
    string Estado,
    DateTime FechaRegistro,
    int ClasesAsignadas,
    int ClasesImpartidas,
    ProfesorDetailUltimaNominaDTO? UltimaNomina,
    List<ProfesorDetailClaseProximaDTO> ProximasClases
);

public record ProfesorDetailUltimaNominaDTO(
    Guid IdNomina,
    decimal MontoTotal,
    DateTime Periodo,
    string Estado
);

public record ProfesorDetailClaseProximaDTO(
    Guid IdClase,
    DateTime Fecha,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    string TipoClase,
    int AlumnosInscritos
);

// Handler
public class GetProfesorDetailQueryHandler : IRequestHandler<GetProfesorDetailQuery, Result<ProfesorDetailDTO>>
{
    private readonly IAppDbContext _context;

    public GetProfesorDetailQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProfesorDetailDTO>> Handle(GetProfesorDetailQuery request, CancellationToken cancellationToken)
    {
        // Obtener profesor con todas sus relaciones
        var profesor = await _context.Set<Profesor>()
            .Include(p => p.Usuario)
                .ThenInclude(u => u.TipoDocumento)
            .Include(p => p.TipoProfesor)
            .FirstOrDefaultAsync(p => p.IdProfesor == request.IdProfesor, cancellationToken);

        if (profesor == null)
            return Result<ProfesorDetailDTO>.Failure("Profesor no encontrado");

        // Contar clases asignadas (próximas clases como profesor principal)
        var hoy = DateTime.Today;
        var clasesAsignadas = await _context.Set<Clase>()
            .Where(c => c.IdProfesorPrincipal == request.IdProfesor && c.Fecha >= hoy)
            .CountAsync(cancellationToken);

        // Contar clases impartidas (clases pasadas)
        var clasesImpartidas = await _context.Set<Clase>()
            .Where(c => c.IdProfesorPrincipal == request.IdProfesor && c.Fecha < hoy)
            .CountAsync(cancellationToken);

        // Obtener próximas 5 clases
        var proximasClases = await _context.Set<Clase>()
            .Include(c => c.TipoClase)
            .Include(c => c.Asistencias)
            .Where(c => c.IdProfesorPrincipal == request.IdProfesor && c.Fecha >= hoy)
            .OrderBy(c => c.Fecha)
            .ThenBy(c => c.HoraInicio)
            .Take(5)
            .Select(c => new ProfesorDetailClaseProximaDTO(
                c.IdClase,
                c.Fecha,
                c.HoraInicio,
                c.HoraFin,
                c.TipoClase.Nombre,
                c.Asistencias.Count
            ))
            .ToListAsync(cancellationToken);

        // Obtener última nómina
        // TODO: Encontrar entidad correcta de Nómina
        ProfesorDetailUltimaNominaDTO? ultimaNomina = null;
        /*
        var ultimaNomina = await _context.Set<NominaProfesor>()
            .Include(n => n.EstadoNomina)
            .Where(n => n.IdProfesor == request.IdProfesor)
            .OrderByDescending(n => n.FechaInicio)
            .Select(n => new ProfesorDetailUltimaNominaDTO(
                n.IdNomina,
                n.Total,
                n.FechaInicio,
                n.EstadoNomina.Nombre
            ))
            .FirstOrDefaultAsync(cancellationToken);
        */

        var dto = new ProfesorDetailDTO(
            profesor.IdProfesor,
            profesor.IdUsuario,
            profesor.Usuario.NombreUsuario,
            profesor.Usuario.Correo,
            profesor.Usuario.Telefono,
            profesor.Usuario.NumeroDocumento,
            profesor.Usuario.TipoDocumento?.Nombre ?? "N/A",
            null, // Direccion - no existe en Usuario
            null, // FechaNacimiento - no existe en Usuario
            profesor.TipoProfesor.Nombre,
            "Activo", // Estado hardcodeado
            DateTime.Today, // Fecha de registro no existe en entidad
            clasesAsignadas,
            clasesImpartidas,
            ultimaNomina,
            proximasClases
        );

        return Result<ProfesorDetailDTO>.Success(dto);
    }
}

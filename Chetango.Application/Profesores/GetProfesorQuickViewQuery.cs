// ============================================
// GET PROFESOR QUICK VIEW QUERY
// ============================================

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Profesores;

/// <summary>
/// Query: Obtener información rápida de un profesor para Quick View
/// GET /api/profesores/{idProfesor}/quick-view
/// Authorization: AdminOnly
/// </summary>
public record GetProfesorQuickViewQuery(Guid IdProfesor) : IRequest<Result<ProfesorQuickViewDTO>>;

public class GetProfesorQuickViewHandler : IRequestHandler<GetProfesorQuickViewQuery, Result<ProfesorQuickViewDTO>>
{
    private readonly IAppDbContext _context;

    public GetProfesorQuickViewHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProfesorQuickViewDTO>> Handle(GetProfesorQuickViewQuery request, CancellationToken cancellationToken)
    {
        var profesor = await _context.Set<Profesor>()
            .Include(p => p.Usuario)
            .Include(p => p.TipoProfesor)
            .Where(p => p.IdProfesor == request.IdProfesor)
            .FirstOrDefaultAsync(cancellationToken);

        if (profesor == null)
            return Result<ProfesorQuickViewDTO>.Failure("Profesor no encontrado");

        // Obtener clases asignadas (futuras)
        var clasesAsignadas = await _context.Set<ClaseProfesor>()
            .Include(cp => cp.Clase)
            .Where(cp => cp.IdProfesor == request.IdProfesor && cp.Clase.Fecha >= DateTime.UtcNow)
            .CountAsync(cancellationToken);

        var dto = new ProfesorQuickViewDTO
        {
            IdProfesor = profesor.IdProfesor,
            IdUsuario = profesor.IdUsuario,
            Nombre = profesor.Usuario.NombreUsuario,
            Documento = profesor.Usuario.NumeroDocumento,
            Correo = profesor.Usuario.Correo,
            Telefono = profesor.Usuario.Telefono,
            TipoProfesor = profesor.TipoProfesor.Nombre,
            ClasesAsignadas = clasesAsignadas,
            ProximaClase = null,
            NominaActual = null
        };

        return Result<ProfesorQuickViewDTO>.Success(dto);
    }
}

/// <summary>
/// DTO para Quick View de Profesor
/// </summary>
public class ProfesorQuickViewDTO
{
    public Guid IdProfesor { get; set; }
    public Guid IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string TipoProfesor { get; set; } = string.Empty;
    public int ClasesAsignadas { get; set; }
    public ProximaClaseProfesorDTO? ProximaClase { get; set; }
    public NominaActualDTO? NominaActual { get; set; }
}

public class ProximaClaseProfesorDTO
{
    public string Fecha { get; set; } = string.Empty;
    public string Hora { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}

public class NominaActualDTO
{
    public string Periodo { get; set; } = string.Empty;
    public int TotalClases { get; set; }
}

using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Queries.GetPaquetesDeAlumno;

public class GetPaquetesDeAlumnoQueryHandler : IRequestHandler<GetPaquetesDeAlumnoQuery, Result<List<PaqueteAlumnoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetPaquetesDeAlumnoQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<List<PaqueteAlumnoDTO>>> Handle(GetPaquetesDeAlumnoQuery request, CancellationToken cancellationToken)
    {
        // Validar que el alumno existe
        var alumnoExiste = await _db.Set<Chetango.Domain.Entities.Alumno>()
            .AsNoTracking()
            .AnyAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (!alumnoExiste)
            return Result<List<PaqueteAlumnoDTO>>.Failure("El alumno especificado no existe.");

        var query = _db.Set<Paquete>()
            .Where(p => p.IdAlumno == request.IdAlumno);

        // Filtrar solo activos si se solicita (IdEstado = 1 es Activo)
        if (request.SoloActivos)
            query = query.Where(p => p.IdEstado == 1);

        var paquetes = await query
            .AsNoTracking()
            .OrderByDescending(p => p.FechaVencimiento)
            .Select(p => new PaqueteAlumnoDTO(
                p.IdPaquete,
                p.ClasesDisponibles,
                p.ClasesUsadas,
                p.ClasesDisponibles - p.ClasesUsadas,
                p.FechaVencimiento,
                p.IdEstado == 1 ? "Activo" : p.IdEstado == 2 ? "Vencido" : p.IdEstado == 3 ? "Congelado" : "Agotado",
                p.FechaVencimiento < DateTime.Today,
                (p.ClasesDisponibles - p.ClasesUsadas) > 0
            ))
            .ToListAsync(cancellationToken);

        return Result<List<PaqueteAlumnoDTO>>.Success(paquetes);
    }
}

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Queries.GetClasesDeAlumno;

// Handler: ejecuta consulta con proyección a DTO y sin tracking
public class GetClasesDeAlumnoQueryHandler : IRequestHandler<GetClasesDeAlumnoQuery, Result<IReadOnlyList<ClaseDto>>>
{
    private readonly IAppDbContext _db;

    public GetClasesDeAlumnoQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<ClaseDto>>> Handle(GetClasesDeAlumnoQuery request, CancellationToken cancellationToken)
    {
        var q = _db.Asistencias
            .AsNoTracking()
            .Where(a => a.IdAlumno == request.IdAlumno)
            .Include(a => a.Clase)
            .ThenInclude(c => c.TipoClase)
            .Include(a => a.Clase)
            .ThenInclude(c => c.ProfesorPrincipal)
            .OrderByDescending(a => a.Clase.Fecha)
            .Select(a => new ClaseDto(
                a.Clase.IdClase,
                a.Clase.Fecha,
                a.Clase.TipoClase.Nombre,
                a.Clase.HoraInicio,
                a.Clase.HoraFin,
                a.Clase.ProfesorPrincipal.IdProfesor.ToString() // TODO: reemplazar por nombre al tenerlo en entidad
            ));

        if (request.Desde.HasValue) q = q.Where(c => c.Fecha >= request.Desde.Value);
        if (request.Hasta.HasValue) q = q.Where(c => c.Fecha <= request.Hasta.Value);

        var list = await q.ToListAsync(cancellationToken);
        return Result<IReadOnlyList<ClaseDto>>.Success(list);
    }
}

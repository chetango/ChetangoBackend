using Chetango.Application.Asistencias.DTOs;
using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Queries.GetAsistenciasPorAlumno;

public class GetAsistenciasPorAlumnoQueryHandler : IRequestHandler<GetAsistenciasPorAlumnoQuery, Result<IReadOnlyList<AsistenciaDto>>>
{
    private readonly IAppDbContext _db;

    public GetAsistenciasPorAlumnoQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<AsistenciaDto>>> Handle(GetAsistenciasPorAlumnoQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Asistencias
            .AsNoTracking()
            .Where(a => a.IdAlumno == request.IdAlumno);

        // Filtros opcionales por rango de fechas
        if (request.FechaDesde.HasValue)
            query = query.Where(a => a.Clase.Fecha >= request.FechaDesde.Value);

        if (request.FechaHasta.HasValue)
            query = query.Where(a => a.Clase.Fecha <= request.FechaHasta.Value);

        var asistencias = await query
            .Include(a => a.Clase)
            .ThenInclude(c => c.TipoClase)
            .Include(a => a.Alumno)
            .ThenInclude(al => al.Usuario)
            .Include(a => a.Estado)
            .Include(a => a.TipoAsistencia)
            .OrderByDescending(a => a.Clase.Fecha)
            .Select(a => new AsistenciaDto(
                a.IdAsistencia,
                a.IdClase,
                a.Clase.Fecha,
                $"{a.Clase.HoraInicio.Hours:D2}:{a.Clase.HoraInicio.Minutes:D2}",
                $"{a.Clase.HoraFin.Hours:D2}:{a.Clase.HoraFin.Minutes:D2}",
                a.Clase.TipoClase.Nombre,
                a.IdAlumno,
                a.Alumno.Usuario.NombreUsuario,
                a.Estado.Nombre,
                a.IdPaqueteUsado,
                a.IdTipoAsistencia,
                a.TipoAsistencia.Nombre,
                a.Observacion
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<AsistenciaDto>>.Success(asistencias);
    }
}

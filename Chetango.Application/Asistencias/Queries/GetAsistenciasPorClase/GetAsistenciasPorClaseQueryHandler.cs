using Chetango.Application.Asistencias.DTOs;
using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase;

public class GetAsistenciasPorClaseQueryHandler : IRequestHandler<GetAsistenciasPorClaseQuery, Result<IReadOnlyList<AsistenciaDto>>>
{
    private readonly IAppDbContext _db;

    public GetAsistenciasPorClaseQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<AsistenciaDto>>> Handle(GetAsistenciasPorClaseQuery request, CancellationToken cancellationToken)
    {
        var asistencias = await _db.Asistencias
            .AsNoTracking()
            .Where(a => a.IdClase == request.IdClase)
            .Include(a => a.Clase)
            .ThenInclude(c => c.TipoClase)
            .Include(a => a.Alumno)
            .ThenInclude(al => al.Usuario)
            .Include(a => a.Estado)
            .OrderBy(a => a.Alumno.Usuario.NombreUsuario)
            .Select(a => new AsistenciaDto(
                a.IdAsistencia,
                a.IdClase,
                a.Clase.Fecha,
                a.Clase.HoraInicio,
                a.Clase.HoraFin,
                a.Clase.TipoClase.Nombre,
                a.IdAlumno,
                a.Alumno.Usuario.NombreUsuario,
                a.Estado.Nombre,
                a.IdPaqueteUsado,
                a.Observacion
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<AsistenciaDto>>.Success(asistencias);
    }
}

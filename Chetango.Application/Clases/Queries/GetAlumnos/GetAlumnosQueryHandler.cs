using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Queries.GetAlumnos;

public class GetAlumnosQueryHandler : IRequestHandler<GetAlumnosQuery, Result<List<AlumnoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetAlumnosQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<List<AlumnoDTO>>> Handle(GetAlumnosQuery request, CancellationToken cancellationToken)
    {
        var alumnos = await _db.Set<Alumno>()
            .Include(a => a.Usuario)
            .AsNoTracking()
            .OrderBy(a => a.Usuario.NombreUsuario)
            .Select(a => new AlumnoDTO(
                a.IdAlumno,
                a.IdUsuario,
                a.Usuario.NombreUsuario,
                a.Usuario.Correo
            ))
            .ToListAsync(cancellationToken);

        return Result<List<AlumnoDTO>>.Success(alumnos);
    }
}

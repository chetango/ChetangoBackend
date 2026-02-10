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
        var query = _db.Set<Alumno>()
            .Include(a => a.Usuario)
            .AsNoTracking();

        // Aplicar filtro si se proporciona
        if (!string.IsNullOrWhiteSpace(request.Filtro))
        {
            var filtro = request.Filtro.ToLower();
            query = query.Where(a => 
                a.Usuario.NombreUsuario.ToLower().Contains(filtro) ||
                a.Usuario.NumeroDocumento.Contains(filtro)
            );
        }

        var alumnos = await query
            .OrderBy(a => a.Usuario.NombreUsuario)
            .Select(a => new AlumnoDTO(
                a.IdAlumno,
                a.IdUsuario,
                a.Usuario.NombreUsuario,
                a.Usuario.Correo,
                a.Usuario.NumeroDocumento,
                a.Usuario.Telefono
            ))
            .ToListAsync(cancellationToken);

        return Result<List<AlumnoDTO>>.Success(alumnos);
    }
}

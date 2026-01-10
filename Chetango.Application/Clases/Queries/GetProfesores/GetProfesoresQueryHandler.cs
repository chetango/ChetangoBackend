using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Queries.GetProfesores;

public class GetProfesoresQueryHandler : IRequestHandler<GetProfesoresQuery, Result<List<ProfesorDTO>>>
{
    private readonly IAppDbContext _db;

    public GetProfesoresQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<List<ProfesorDTO>>> Handle(GetProfesoresQuery request, CancellationToken cancellationToken)
    {
        var profesores = await _db.Set<Profesor>()
            .Include(p => p.Usuario)
            .AsNoTracking()
            .OrderBy(p => p.Usuario.NombreUsuario)
            .Select(p => new ProfesorDTO(
                p.IdProfesor,
                p.IdUsuario,
                p.Usuario.NombreUsuario,
                p.Usuario.Correo
            ))
            .ToListAsync(cancellationToken);

        return Result<List<ProfesorDTO>>.Success(profesores);
    }
}

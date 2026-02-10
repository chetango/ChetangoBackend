using Chetango.Application.Asistencias.DTOs;
using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Queries.GetTiposAsistencia;

public class GetTiposAsistenciaQueryHandler : IRequestHandler<GetTiposAsistenciaQuery, Result<List<TipoAsistenciaDto>>>
{
    private readonly IAppDbContext _db;

    public GetTiposAsistenciaQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<TipoAsistenciaDto>>> Handle(GetTiposAsistenciaQuery request, CancellationToken ct)
    {
        var tipos = await _db.Set<TipoAsistencia>()
            .Where(t => t.Activo)
            .OrderBy(t => t.IdTipoAsistencia)
            .Select(t => new TipoAsistenciaDto
            {
                IdTipoAsistencia = t.IdTipoAsistencia,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                RequierePaquete = t.RequierePaquete,
                DescontarClase = t.DescontarClase
            })
            .ToListAsync(ct);

        return Result<List<TipoAsistenciaDto>>.Success(tipos);
    }
}

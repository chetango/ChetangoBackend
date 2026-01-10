using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Queries.GetTiposClase;

public class GetTiposClaseQueryHandler : IRequestHandler<GetTiposClaseQuery, Result<List<TipoClaseDTO>>>
{
    private readonly IAppDbContext _db;

    public GetTiposClaseQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<List<TipoClaseDTO>>> Handle(GetTiposClaseQuery request, CancellationToken cancellationToken)
    {
        var tiposClase = await _db.Set<TipoClase>()
            .AsNoTracking()
            .OrderBy(tc => tc.Nombre)
            .Select(tc => new TipoClaseDTO(
                tc.Id,
                tc.Nombre
            ))
            .ToListAsync(cancellationToken);

        return Result<List<TipoClaseDTO>>.Success(tiposClase);
    }
}

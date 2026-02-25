using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;

namespace Chetango.Application.Finanzas.Queries;

public class GetCategoriasGastoQueryHandler : IRequestHandler<GetCategoriasGastoQuery, Result<List<CategoriaGastoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetCategoriasGastoQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<CategoriaGastoDTO>>> Handle(GetCategoriasGastoQuery request, CancellationToken cancellationToken)
    {
        var categorias = await _db.Set<Domain.Entities.Estados.CategoriaGasto>()
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaGastoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            })
            .ToListAsync(cancellationToken);

        return Result<List<CategoriaGastoDTO>>.Success(categorias);
    }
}

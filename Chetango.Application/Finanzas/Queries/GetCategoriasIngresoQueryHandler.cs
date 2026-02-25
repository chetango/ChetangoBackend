using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;

namespace Chetango.Application.Finanzas.Queries;

public class GetCategoriasIngresoQueryHandler : IRequestHandler<GetCategoriasIngresoQuery, Result<List<CategoriaIngresoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetCategoriasIngresoQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<CategoriaIngresoDTO>>> Handle(GetCategoriasIngresoQuery request, CancellationToken cancellationToken)
    {
        var categorias = await _db.Set<Domain.Entities.Estados.CategoriaIngreso>()
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaIngresoDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            })
            .ToListAsync(cancellationToken);

        return Result<List<CategoriaIngresoDTO>>.Success(categorias);
    }
}

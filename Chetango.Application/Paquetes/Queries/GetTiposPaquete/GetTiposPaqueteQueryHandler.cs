using Chetango.Application.Common;
using Chetango.Application.Paquetes.DTOs;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Queries.GetTiposPaquete;

public class GetTiposPaqueteQueryHandler : IRequestHandler<GetTiposPaqueteQuery, Result<List<TipoPaqueteDTO>>>
{
    private readonly IAppDbContext _db;

    public GetTiposPaqueteQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<List<TipoPaqueteDTO>>> Handle(GetTiposPaqueteQuery request, CancellationToken cancellationToken)
    {
        var tiposPaquete = await _db.Set<TipoPaquete>()
            .Where(tp => tp.Activo) // Solo tipos activos
            .AsNoTracking()
            .OrderBy(tp => tp.Precio)
            .Select(tp => new TipoPaqueteDTO(
                tp.Id,
                tp.Nombre,
                tp.NumeroClases,
                tp.Precio,
                tp.DiasVigencia,
                tp.Descripcion,
                tp.Activo
            ))
            .ToListAsync(cancellationToken);

        return Result<List<TipoPaqueteDTO>>.Success(tiposPaquete);
    }
}

using Chetango.Application.Common;
using Chetango.Application.Sedes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Sedes.Queries;

/// <summary>
/// Consulta para obtener las sedes configuradas del tenant actual.
/// El query filter de SedeConfig aplica el TenantId automáticamente.
/// </summary>
public record GetSedesQuery : IRequest<Result<List<SedeConfigDTO>>>;

public class GetSedesQueryHandler : IRequestHandler<GetSedesQuery, Result<List<SedeConfigDTO>>>
{
    private readonly IAppDbContext _db;

    public GetSedesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<SedeConfigDTO>>> Handle(GetSedesQuery request, CancellationToken cancellationToken)
    {
        var sedes = await _db.SedeConfigs
            .Where(s => s.Activa)
            .OrderBy(s => s.Orden)
            .Select(s => new SedeConfigDTO
            {
                Id        = s.Id,
                SedeValor = s.SedeValor,
                Nombre    = s.Nombre,
                Activa    = s.Activa,
                EsDefault = s.EsDefault,
                Orden     = s.Orden
            })
            .ToListAsync(cancellationToken);

        return Result<List<SedeConfigDTO>>.Success(sedes);
    }
}

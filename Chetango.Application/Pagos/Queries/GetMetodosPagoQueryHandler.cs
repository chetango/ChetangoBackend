using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Application.Pagos.Queries;

public class GetMetodosPagoQueryHandler : IRequestHandler<GetMetodosPagoQuery, Result<List<MetodoPagoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetMetodosPagoQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<MetodoPagoDTO>>> Handle(GetMetodosPagoQuery request, CancellationToken cancellationToken)
    {
        var metodosPago = await _db.Set<MetodoPago>()
            .Select(m => new MetodoPagoDTO(
                m.Id,
                m.Nombre,
                null
            ))
            .ToListAsync(cancellationToken);

        return Result<List<MetodoPagoDTO>>.Success(metodosPago);
    }
}

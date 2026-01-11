using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Queries.GetEstadisticasPaquetes;

// Query para obtener estadísticas de paquetes por estado
public record GetEstadisticasPaquetesQuery() : IRequest<Result<EstadisticasPaquetesDTO>>;

// DTO para estadísticas
public record EstadisticasPaquetesDTO(
    int TotalPaquetes,
    int Activos,
    int Vencidos,
    int Congelados,
    int Agotados
);

// Handler
public class GetEstadisticasPaquetesQueryHandler : IRequestHandler<GetEstadisticasPaquetesQuery, Result<EstadisticasPaquetesDTO>>
{
    private readonly IAppDbContext _db;

    public GetEstadisticasPaquetesQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<EstadisticasPaquetesDTO>> Handle(GetEstadisticasPaquetesQuery request, CancellationToken cancellationToken)
    {
        var paquetes = await _db.Set<Paquete>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var estadisticas = new EstadisticasPaquetesDTO(
            TotalPaquetes: paquetes.Count,
            Activos: paquetes.Count(p => p.IdEstado == 1),
            Vencidos: paquetes.Count(p => p.IdEstado == 2),
            Congelados: paquetes.Count(p => p.IdEstado == 3),
            Agotados: paquetes.Count(p => p.IdEstado == 4)
        );

        return Result<EstadisticasPaquetesDTO>.Success(estadisticas);
    }
}

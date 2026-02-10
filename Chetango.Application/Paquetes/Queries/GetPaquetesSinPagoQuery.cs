using Chetango.Application.Common;
using Chetango.Application.Paquetes.DTOs;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Queries;

public record GetPaquetesSinPagoQuery(Guid IdAlumno) : IRequest<Result<List<PaqueteSinPagoDTO>>>;

public class GetPaquetesSinPagoQueryHandler : IRequestHandler<GetPaquetesSinPagoQuery, Result<List<PaqueteSinPagoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetPaquetesSinPagoQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<PaqueteSinPagoDTO>>> Handle(GetPaquetesSinPagoQuery request, CancellationToken cancellationToken)
    {
        var paquetes = await _db.Set<Paquete>()
            .Include(p => p.TipoPaquete)
            .Include(p => p.Estado)
            .Where(p => p.IdAlumno == request.IdAlumno && p.IdPago == null)
            .OrderByDescending(p => p.FechaCreacion)
            .Select(p => new PaqueteSinPagoDTO(
                p.IdPaquete,
                p.TipoPaquete.Nombre,
                p.ClasesDisponibles,
                p.ClasesUsadas,
                p.ValorPaquete,
                p.FechaActivacion,
                p.FechaVencimiento,
                p.Estado.Nombre
            ))
            .ToListAsync(cancellationToken);

        return Result<List<PaqueteSinPagoDTO>>.Success(paquetes);
    }
}

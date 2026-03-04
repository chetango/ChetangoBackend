using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Handler para obtener el historial de pagos de suscripción de un tenant.
/// </summary>
public class GetHistorialPagosQueryHandler : IRequestHandler<GetHistorialPagosQuery, Result<List<PagoSuscripcionDto>>>
{
    private readonly IAppDbContext _db;

    public GetHistorialPagosQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<PagoSuscripcionDto>>> Handle(GetHistorialPagosQuery request, CancellationToken cancellationToken)
    {
        var pagos = await _db.PagosSuscripcion
            .AsNoTracking()
            .Where(p => p.TenantId == request.TenantId)
            .OrderByDescending(p => p.FechaPago)
            .Select(p => new PagoSuscripcionDto
            {
                Id = p.Id,
                FechaPago = p.FechaPago,
                Monto = p.Monto,
                Referencia = p.Referencia,
                MetodoPago = p.MetodoPago,
                Estado = p.Estado,
                ComprobanteUrl = p.ComprobanteUrl,
                FechaAprobacion = p.FechaAprobacion,
                Observaciones = p.Observaciones
            })
            .ToListAsync(cancellationToken);

        return Result<List<PagoSuscripcionDto>>.Success(pagos);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Handler para obtener todos los pagos pendientes de aprobación (SuperAdmin).
/// </summary>
public class GetPagosPendientesAprobacionQueryHandler : IRequestHandler<GetPagosPendientesAprobacionQuery, Result<List<PagoSuscripcionDto>>>
{
    private readonly IAppDbContext _db;

    public GetPagosPendientesAprobacionQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<PagoSuscripcionDto>>> Handle(GetPagosPendientesAprobacionQuery request, CancellationToken cancellationToken)
    {
        var pagos = await _db.PagosSuscripcion
            .AsNoTracking()
            .Include(p => p.Tenant)
            .Where(p => p.Estado == "Pendiente")
            .OrderBy(p => p.FechaCreacion)
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
                Observaciones = p.Observaciones,
                NombreAcademia = p.Tenant.Nombre,
                Subdomain = p.Tenant.Subdomain
            })
            .ToListAsync(cancellationToken);

        return Result<List<PagoSuscripcionDto>>.Success(pagos);
    }
}

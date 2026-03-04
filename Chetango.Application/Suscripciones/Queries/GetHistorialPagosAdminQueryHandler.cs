using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Handler para obtener historial de pagos con filtros (SuperAdmin).
/// </summary>
public class GetHistorialPagosAdminQueryHandler 
    : IRequestHandler<GetHistorialPagosAdminQuery, Result<IReadOnlyList<PagoSuscripcionDto>>>
{
    private readonly IAppDbContext _db;

    public GetHistorialPagosAdminQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<PagoSuscripcionDto>>> Handle(
        GetHistorialPagosAdminQuery request, 
        CancellationToken cancellationToken)
    {
        var query = _db.PagosSuscripcion
            .Include(p => p.Tenant)
            .AsQueryable();

        // Aplicar filtros
        if (request.FechaDesde.HasValue)
        {
            query = query.Where(p => p.FechaAprobacion >= request.FechaDesde.Value);
        }

        if (request.FechaHasta.HasValue)
        {
            query = query.Where(p => p.FechaAprobacion <= request.FechaHasta.Value);
        }

        if (!string.IsNullOrEmpty(request.Estado))
        {
            query = query.Where(p => p.Estado == request.Estado);
        }

        var pagos = await query
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync(cancellationToken);

        var dtos = pagos.Select(p => new PagoSuscripcionDto
        {
            Id = p.Id,
            FechaPago = p.FechaPago,
            Monto = p.Monto,
            Referencia = p.Referencia,
            MetodoPago = p.MetodoPago,
            ComprobanteUrl = p.ComprobanteUrl,
            Estado = p.Estado,
            FechaAprobacion = p.FechaAprobacion,
            Observaciones = p.Observaciones,
            NombreAcademia = p.Tenant.Nombre,
            Subdomain = p.Tenant.Subdomain
        }).ToList();

        return Result<IReadOnlyList<PagoSuscripcionDto>>.Success(dtos);
    }
}

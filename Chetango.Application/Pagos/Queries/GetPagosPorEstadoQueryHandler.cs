using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Pagos.Queries;

public class GetPagosPorEstadoQueryHandler : IRequestHandler<GetPagosPorEstadoQuery, Result<PaginatedList<PagoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetPagosPorEstadoQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedList<PagoDTO>>> Handle(GetPagosPorEstadoQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<Pago>()
            .Include(p => p.MetodoPago)
            .Include(p => p.Alumno)
                .ThenInclude(a => a.Usuario)
            .Include(p => p.EstadoPago)
            .Include(p => p.Paquetes)
            .Where(p => p.EstadoPago.Nombre == request.NombreEstado);

        // Aplicar filtros opcionales
        if (request.FechaDesde.HasValue)
        {
            query = query.Where(p => p.FechaPago >= request.FechaDesde.Value);
        }

        if (request.FechaHasta.HasValue)
        {
            query = query.Where(p => p.FechaPago <= request.FechaHasta.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.FechaPago)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PagoDTO(
                p.IdPago,
                p.FechaPago,
                p.MontoTotal,
                p.MetodoPago.Nombre,
                p.Alumno.Usuario.NombreUsuario,
                p.EstadoPago.Nombre,
                p.UrlComprobante,
                p.ReferenciaTransferencia,
                p.NotasVerificacion,
                p.FechaVerificacion,
                p.UsuarioVerificacion,
                p.Paquetes.Count
            ))
            .ToListAsync(cancellationToken);

        var result = new PaginatedList<PagoDTO>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        return Result<PaginatedList<PagoDTO>>.Success(result);
    }
}

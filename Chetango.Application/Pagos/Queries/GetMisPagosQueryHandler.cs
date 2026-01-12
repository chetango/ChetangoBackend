using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Pagos.Queries;

public class GetMisPagosQueryHandler : IRequestHandler<GetMisPagosQuery, Result<PaginatedList<PagoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetMisPagosQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedList<PagoDTO>>> Handle(GetMisPagosQuery request, CancellationToken cancellationToken)
    {
        // Buscar alumno por email del usuario
        var alumno = await _db.Set<Alumno>()
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Usuario.Correo == request.EmailUsuario, cancellationToken);

        if (alumno == null)
        {
            return Result<PaginatedList<PagoDTO>>.Success(new PaginatedList<PagoDTO>
            {
                Items = new List<PagoDTO>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = 0,
                TotalPages = 0
            });
        }

        var query = _db.Set<Pago>()
            .Include(p => p.MetodoPago)
            .Include(p => p.Alumno)
            .Include(p => p.Paquetes)
            .Where(p => p.IdAlumno == alumno.IdAlumno);

        // Aplicar filtros opcionales
        if (request.FechaDesde.HasValue)
        {
            query = query.Where(p => p.FechaPago >= request.FechaDesde.Value);
        }

        if (request.FechaHasta.HasValue)
        {
            query = query.Where(p => p.FechaPago <= request.FechaHasta.Value);
        }

        if (request.IdMetodoPago.HasValue)
        {
            query = query.Where(p => p.IdMetodoPago == request.IdMetodoPago.Value);
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

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
            .Include(p => p.EstadoPago)
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

        // Cargar sedes del tenant para resolver nombres dinámicamente (query filter aplica TenantId)
        var sedeConfigs = await _db.SedeConfigs.AsNoTracking().ToListAsync(cancellationToken);
        string ResolverNombreSede(Domain.Enums.Sede sede)
        {
            var config = sedeConfigs.FirstOrDefault(s => s.SedeValor == (int)sede);
            return config?.Nombre ?? (sede == Domain.Enums.Sede.Medellin ? "Medellín" : "Manizales");
        }

        var rawItems = await query
            .OrderByDescending(p => p.FechaPago)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new
            {
                p.IdPago,
                p.IdAlumno,
                p.FechaPago,
                p.MontoTotal,
                MetodoPagoNombre = p.MetodoPago.Nombre,
                NombreAlumno = p.Alumno.Usuario.NombreUsuario,
                EstadoPagoNombre = p.EstadoPago.Nombre,
                p.UrlComprobante,
                p.ReferenciaTransferencia,
                p.NotasVerificacion,
                p.FechaVerificacion,
                p.UsuarioVerificacion,
                CantidadPaquetes = p.Paquetes.Count,
                p.Sede
            })
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(p => new PagoDTO(
            p.IdPago,
            p.IdAlumno ?? Guid.Empty,
            p.FechaPago,
            p.MontoTotal,
            p.MetodoPagoNombre,
            p.NombreAlumno,
            p.EstadoPagoNombre,
            p.UrlComprobante,
            p.ReferenciaTransferencia,
            p.NotasVerificacion,
            p.FechaVerificacion,
            p.UsuarioVerificacion,
            p.CantidadPaquetes,
            p.Sede,
            ResolverNombreSede(p.Sede)
        )).ToList();

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

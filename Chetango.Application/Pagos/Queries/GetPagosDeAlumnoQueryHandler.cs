using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Pagos.Queries;

public class GetPagosDeAlumnoQueryHandler : IRequestHandler<GetPagosDeAlumnoQuery, Result<PaginatedList<PagoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetPagosDeAlumnoQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedList<PagoDTO>>> Handle(GetPagosDeAlumnoQuery request, CancellationToken cancellationToken)
    {
        // Verificar que el alumno existe
        var alumnoConsultado = await _db.Set<Alumno>()
            .FirstOrDefaultAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (alumnoConsultado == null)
        {
            return Result<PaginatedList<PagoDTO>>.Failure("El alumno especificado no existe.");
        }

        // Ownership validation por email
        if (!request.EsAdmin)
        {
            var alumnoAutenticado = await _db.Set<Alumno>()
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Usuario.Correo == request.EmailUsuario, cancellationToken);

            if (alumnoAutenticado == null)
            {
                return Result<PaginatedList<PagoDTO>>.Failure("Usuario no encontrado como alumno.");
            }

            if (request.IdAlumno != alumnoAutenticado.IdAlumno)
            {
                return Result<PaginatedList<PagoDTO>>.Failure("No tienes permiso para ver los pagos de este alumno.");
            }
        }

        var query = _db.Set<Pago>()
            .Include(p => p.MetodoPago)
            .Include(p => p.Alumno)
            .Include(p => p.EstadoPago)
            .Include(p => p.Paquetes)
            .Where(p => p.IdAlumno == request.IdAlumno);

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
                p.IdAlumno ?? Guid.Empty,
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
                p.Paquetes.Count,
                p.Sede,
                p.Sede == Domain.Enums.Sede.Medellin ? "Medellín" : "Manizales"
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

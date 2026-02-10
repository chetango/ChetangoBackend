using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Nomina.Queries;

public class GetClasesPorProfesorQueryHandler : IRequestHandler<GetClasesPorProfesorQuery, Result<List<ClaseProfesorDTO>>>
{
    private readonly IAppDbContext _db;

    public GetClasesPorProfesorQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ClaseProfesorDTO>>> Handle(GetClasesPorProfesorQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<ClaseProfesor>()
            .Include(cp => cp.Clase)
                .ThenInclude(c => c.TipoClase)
            .Include(cp => cp.Profesor)
                .ThenInclude(p => p.Usuario)
            .Include(cp => cp.RolEnClase)
            .Where(cp => cp.IdProfesor == request.IdProfesor);

        // Filtrar por fecha si se proporciona
        if (request.FechaDesde.HasValue)
            query = query.Where(cp => cp.Clase.Fecha >= request.FechaDesde.Value);

        if (request.FechaHasta.HasValue)
            query = query.Where(cp => cp.Clase.Fecha <= request.FechaHasta.Value);

        // Filtrar por estado de pago si se proporciona
        if (!string.IsNullOrEmpty(request.EstadoPago))
            query = query.Where(cp => cp.EstadoPago == request.EstadoPago);

        var clases = await query
            .OrderByDescending(cp => cp.Clase.Fecha)
            .ThenByDescending(cp => cp.Clase.HoraInicio)
            .ToListAsync(cancellationToken);

        var resultado = clases.Select(cp => new ClaseProfesorDTO(
            cp.IdClaseProfesor,
            cp.IdClase,
            cp.Clase.Fecha,
            cp.Clase.TipoClase.Nombre,
            cp.Clase.TipoClase.Nombre,
            cp.IdProfesor,
            cp.Profesor.Usuario.NombreUsuario,
            cp.RolEnClase.Nombre,
            cp.TarifaProgramada,
            cp.ValorAdicional,
            cp.ConceptoAdicional,
            cp.TotalPago,
            cp.EstadoPago,
            cp.FechaAprobacion,
            cp.FechaPago,
            null // NombreAprobador - se puede agregar si se requiere
        )).ToList();

        return Result<List<ClaseProfesorDTO>>.Success(resultado);
    }
}

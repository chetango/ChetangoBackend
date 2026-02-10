using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Nomina.Queries;

public class GetClasesAprobadasQueryHandler : IRequestHandler<GetClasesAprobadasQuery, Result<List<ClaseProfesorDTO>>>
{
    private readonly IAppDbContext _db;

    public GetClasesAprobadasQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ClaseProfesorDTO>>> Handle(GetClasesAprobadasQuery request, CancellationToken cancellationToken)
    {
        var clases = await _db.Set<ClaseProfesor>()
            .Include(cp => cp.Clase)
                .ThenInclude(c => c.TipoClase)
            .Include(cp => cp.Profesor)
                .ThenInclude(p => p.Usuario)
            .Include(cp => cp.RolEnClase)
            .Where(cp => cp.IdProfesor == request.IdProfesor
                && cp.EstadoPago == "Aprobado"
                && cp.Clase.Fecha.Month == request.Mes
                && cp.Clase.Fecha.Year == request.Año)
            .OrderBy(cp => cp.Clase.Fecha)
            .ToListAsync(cancellationToken);

        var resultado = clases.Select(cp => new ClaseProfesorDTO(
            cp.IdClaseProfesor,
            cp.IdClase,
            cp.Clase.Fecha,
            cp.Clase.TipoClase.Nombre, // Usar tipo como nombre
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
            null // NombreAprobador - no hay navegación disponible
        )).ToList();

        return Result<List<ClaseProfesorDTO>>.Success(resultado);
    }
}

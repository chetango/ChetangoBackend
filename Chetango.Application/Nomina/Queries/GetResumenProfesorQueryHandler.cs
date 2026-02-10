using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Application.Nomina.Queries;

public class GetResumenProfesorQueryHandler : IRequestHandler<GetResumenProfesorQuery, Result<List<ResumenProfesorDTO>>>
{
    private readonly IAppDbContext _db;

    public GetResumenProfesorQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ResumenProfesorDTO>>> Handle(GetResumenProfesorQuery request, CancellationToken cancellationToken)
    {
        var profesoresQuery = _db.Set<Profesor>()
            .Include(p => p.Usuario)
            .Include(p => p.ClasesProfesores)
            .AsQueryable();

        if (request.IdProfesor.HasValue)
            profesoresQuery = profesoresQuery.Where(p => p.IdProfesor == request.IdProfesor.Value);

        var profesores = await profesoresQuery.ToListAsync(cancellationToken);

        var resultado = profesores.Select(p =>
        {
            var clasesPendientes = p.ClasesProfesores.Count(cp => cp.EstadoPago == "Pendiente");
            var clasesAprobadas = p.ClasesProfesores.Count(cp => cp.EstadoPago == "Aprobado");
            var clasesLiquidadas = p.ClasesProfesores.Count(cp => cp.EstadoPago == "Liquidado");
            var clasesPagadas = p.ClasesProfesores.Count(cp => cp.EstadoPago == "Pagado");

            var totalPendiente = p.ClasesProfesores.Where(cp => cp.EstadoPago == "Pendiente").Sum(cp => cp.TotalPago);
            var totalAprobado = p.ClasesProfesores.Where(cp => cp.EstadoPago == "Aprobado").Sum(cp => cp.TotalPago);
            var totalLiquidado = p.ClasesProfesores.Where(cp => cp.EstadoPago == "Liquidado").Sum(cp => cp.TotalPago);
            var totalPagado = p.ClasesProfesores.Where(cp => cp.EstadoPago == "Pagado").Sum(cp => cp.TotalPago);

            return new ResumenProfesorDTO(
                p.IdProfesor,
                p.Usuario.NombreUsuario,
                clasesPendientes,
                clasesAprobadas,
                clasesLiquidadas,
                clasesPagadas,
                totalPendiente,
                totalAprobado,
                totalLiquidado,
                totalPagado
            );
        }).ToList();

        return Result<List<ResumenProfesorDTO>>.Success(resultado);
    }
}

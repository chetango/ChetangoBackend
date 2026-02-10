using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Nomina.Queries;

public class GetClasesRealizadasQueryHandler : IRequestHandler<GetClasesRealizadasQuery, Result<List<ClaseRealizadaDTO>>>
{
    private readonly IAppDbContext _db;

    public GetClasesRealizadasQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ClaseRealizadaDTO>>> Handle(GetClasesRealizadasQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<Clase>()
            .Include(c => c.TipoClase)
            .Include(c => c.Profesores)
                .ThenInclude(cp => cp.Profesor)
                    .ThenInclude(p => p.Usuario)
            .Include(c => c.Profesores)
                .ThenInclude(cp => cp.RolEnClase)
            .Where(c => c.Estado == "Completada"); // Solo clases completadas

        if (request.FechaDesde.HasValue)
            query = query.Where(c => c.Fecha >= request.FechaDesde.Value);

        if (request.FechaHasta.HasValue)
            query = query.Where(c => c.Fecha <= request.FechaHasta.Value);

        if (request.IdProfesor.HasValue)
            query = query.Where(c => c.Profesores.Any(p => p.IdProfesor == request.IdProfesor.Value));

        var clases = await query
            .OrderByDescending(c => c.Fecha)
            .ToListAsync(cancellationToken);

        var resultado = clases.Select(c => new ClaseRealizadaDTO(
            c.IdClase,
            c.Fecha,
            c.TipoClase.Nombre, // Usar el tipo como nombre
            c.TipoClase.Nombre,
            "Realizada", // Estado fijo para clases finalizadas
            c.Profesores
                .Where(cp => string.IsNullOrEmpty(request.EstadoPago) || cp.EstadoPago == request.EstadoPago)
                .Select(cp => new ProfesorClaseDTO(
                    cp.IdClaseProfesor,
                    cp.IdProfesor,
                    cp.Profesor.Usuario.NombreUsuario,
                    cp.RolEnClase.Nombre,
                    cp.TarifaProgramada,
                    cp.ValorAdicional,
                    cp.TotalPago,
                    cp.EstadoPago
                )).ToList()
        )).Where(c => c.Profesores.Any()).ToList(); // Solo incluir clases con profesores que cumplan el filtro

        return Result<List<ClaseRealizadaDTO>>.Success(resultado);
    }
}

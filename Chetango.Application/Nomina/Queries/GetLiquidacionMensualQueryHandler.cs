using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Nomina.Queries;

public class GetLiquidacionMensualQueryHandler : IRequestHandler<GetLiquidacionMensualQuery, Result<LiquidacionDetalleDTO>>
{
    private readonly IAppDbContext _db;

    public GetLiquidacionMensualQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<LiquidacionDetalleDTO>> Handle(GetLiquidacionMensualQuery request, CancellationToken cancellationToken)
    {
        LiquidacionMensual? liquidacion = null;

        if (request.IdLiquidacion.HasValue)
        {
            liquidacion = await _db.Set<LiquidacionMensual>()
                .Include(l => l.Profesor)
                    .ThenInclude(p => p.Usuario)
                .FirstOrDefaultAsync(l => l.IdLiquidacion == request.IdLiquidacion.Value, cancellationToken);
        }
        else if (request.IdProfesor.HasValue && request.Mes.HasValue && request.Año.HasValue)
        {
            liquidacion = await _db.Set<LiquidacionMensual>()
                .Include(l => l.Profesor)
                    .ThenInclude(p => p.Usuario)
                .FirstOrDefaultAsync(l => l.IdProfesor == request.IdProfesor.Value
                    && l.Mes == request.Mes.Value
                    && l.Año == request.Año.Value, cancellationToken);
        }

        if (liquidacion == null)
            return Result<LiquidacionDetalleDTO>.Failure("Liquidación no encontrada");

        // Obtener las clases liquidadas
        var clases = await _db.Set<ClaseProfesor>()
            .Include(cp => cp.Clase)
                .ThenInclude(c => c.TipoClase)
            .Include(cp => cp.Profesor)
                .ThenInclude(p => p.Usuario)
            .Include(cp => cp.RolEnClase)
            .Where(cp => cp.IdProfesor == liquidacion.IdProfesor
                && (cp.EstadoPago == "Liquidado" || cp.EstadoPago == "Pagado")
                && cp.Clase.Fecha.Month == liquidacion.Mes
                && cp.Clase.Fecha.Year == liquidacion.Año)
            .OrderBy(cp => cp.Clase.Fecha)
            .ToListAsync(cancellationToken);

        var clasesDTO = clases.Select(cp => new ClaseProfesorDTO(
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

        var resultado = new LiquidacionDetalleDTO(
            liquidacion.IdLiquidacion,
            liquidacion.IdProfesor,
            liquidacion.Profesor.Usuario.NombreUsuario,
            liquidacion.Mes,
            liquidacion.Año,
            liquidacion.TotalClases,
            liquidacion.TotalHoras,
            liquidacion.TotalBase,
            liquidacion.TotalAdicionales,
            liquidacion.TotalPagar,
            liquidacion.Estado,
            liquidacion.FechaCierre,
            liquidacion.FechaPago,
            liquidacion.Observaciones,
            clasesDTO
        );

        return Result<LiquidacionDetalleDTO>.Success(resultado);
    }
}

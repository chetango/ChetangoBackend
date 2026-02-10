using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Pagos.Queries;

public class GetEstadisticasPagosQueryHandler : IRequestHandler<GetEstadisticasPagosQuery, Result<EstadisticasPagosDTO>>
{
    private readonly IAppDbContext _db;

    public GetEstadisticasPagosQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<EstadisticasPagosDTO>> Handle(GetEstadisticasPagosQuery request, CancellationToken cancellationToken)
    {
        var hoy = DateTime.Today;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        var inicioMesAnterior = inicioMes.AddMonths(-1);
        var finMesAnterior = inicioMes.AddDays(-1);

        // Pagos verificados hoy
        var pagosVerificadosHoy = await _db.Set<Pago>()
            .Include(p => p.EstadoPago)
            .Where(p => p.EstadoPago.Nombre == "Verificado" && 
                        p.FechaVerificacion.HasValue && 
                        p.FechaVerificacion.Value.Date == hoy)
            .ToListAsync(cancellationToken);

        var totalIngresosHoy = pagosVerificadosHoy.Sum(p => p.MontoTotal);
        var totalPagosHoy = pagosVerificadosHoy.Count;

        // Pagos pendientes de verificación
        var totalPendientes = await _db.Set<Pago>()
            .Include(p => p.EstadoPago)
            .Where(p => p.EstadoPago.Nombre == "Pendiente Verificación")
            .CountAsync(cancellationToken);

        // Ingresos del mes actual (todos los verificados)
        var ingresosMesActual = await _db.Set<Pago>()
            .Include(p => p.EstadoPago)
            .Where(p => p.EstadoPago.Nombre == "Verificado" && 
                        p.FechaPago >= inicioMes)
            .SumAsync(p => p.MontoTotal, cancellationToken);

        // Ingresos del mes anterior para comparación
        var ingresosMesAnterior = await _db.Set<Pago>()
            .Include(p => p.EstadoPago)
            .Where(p => p.EstadoPago.Nombre == "Verificado" && 
                        p.FechaPago >= inicioMesAnterior && 
                        p.FechaPago <= finMesAnterior)
            .SumAsync(p => p.MontoTotal, cancellationToken);

        // Calcular porcentaje de cambio
        decimal comparacion = 0;
        if (ingresosMesAnterior > 0)
        {
            comparacion = ((ingresosMesActual - ingresosMesAnterior) / ingresosMesAnterior) * 100;
        }
        else if (ingresosMesActual > 0)
        {
            comparacion = 100; // Si el mes anterior fue 0 y este mes hay ingresos, es +100%
        }

        var estadisticas = new EstadisticasPagosDTO(
            totalIngresosHoy,
            totalPagosHoy,
            totalPendientes,
            totalPagosHoy,
            ingresosMesActual,
            comparacion
        );

        return Result<EstadisticasPagosDTO>.Success(estadisticas);
    }
}


using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Chetango.Application.Reportes.Queries;

public class GetReporteIngresosHandler : IRequestHandler<GetReporteIngresosQuery, Result<ReporteIngresosDTO>>
{
    private readonly IAppDbContext _db;

    public GetReporteIngresosHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ReporteIngresosDTO>> Handle(GetReporteIngresosQuery request, CancellationToken cancellationToken)
    {
        // Validaciones
        if (request.FechaDesde > request.FechaHasta)
            return Result<ReporteIngresosDTO>.Failure("La fecha inicial no puede ser mayor a la fecha final.");

        if (request.FechaDesde > DateTime.Today)
            return Result<ReporteIngresosDTO>.Failure("No se pueden generar reportes de fechas futuras.");

        if ((request.FechaHasta - request.FechaDesde).Days > 365)
            return Result<ReporteIngresosDTO>.Failure("El rango de fechas no puede ser mayor a 1 año.");

        // Base query
        var query = _db.Pagos
            .Include(p => p.MetodoPago)
            .Where(p => p.FechaPago >= request.FechaDesde && p.FechaPago <= request.FechaHasta)
            .AsQueryable();

        // Aplicar filtros opcionales
        if (request.IdMetodoPago.HasValue)
            query = query.Where(p => p.IdMetodoPago == request.IdMetodoPago.Value);

        // Ejecutar query
        var pagos = await query.ToListAsync(cancellationToken);

        // Calcular métricas
        var totalRecaudado = pagos.Sum(p => p.MontoTotal);
        var cantidad = pagos.Count;
        var promedio = cantidad > 0 ? totalRecaudado / cantidad : 0;

        // Comparativa con mes anterior (si se solicita)
        decimal? comparativaMesAnterior = null;
        if (request.ComparativaMesAnterior)
        {
            var diasPeriodo = (request.FechaHasta - request.FechaDesde).Days;
            var fechaDesdeAnterior = request.FechaDesde.AddDays(-diasPeriodo);
            var fechaHastaAnterior = request.FechaDesde.AddDays(-1);

            var pagosAnterior = await _db.Pagos
                .Where(p => p.FechaPago >= fechaDesdeAnterior && p.FechaPago <= fechaHastaAnterior)
                .ToListAsync(cancellationToken);

            var totalAnterior = pagosAnterior.Sum(p => p.MontoTotal);

            if (totalAnterior > 0)
            {
                comparativaMesAnterior = ((totalRecaudado - totalAnterior) / totalAnterior) * 100;
            }
        }

        // Tendencia mensual (últimos 12 meses desde fechaHasta)
        var fechaInicio12Meses = request.FechaHasta.AddMonths(-11).Date;
        var fechaInicio12MesesPrimerDia = new DateTime(fechaInicio12Meses.Year, fechaInicio12Meses.Month, 1);

        var pagosMensuales = await _db.Pagos
            .Where(p => p.FechaPago >= fechaInicio12MesesPrimerDia && p.FechaPago <= request.FechaHasta)
            .ToListAsync(cancellationToken);

        var tendenciaMensual = pagosMensuales
            .GroupBy(p => new { p.FechaPago.Year, p.FechaPago.Month })
            .Select(g => new TendenciaMensualDTO
            {
                Año = g.Key.Year,
                Mes = g.Key.Month,
                MesNombre = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                TotalIngresos = g.Sum(p => p.MontoTotal),
                CantidadPagos = g.Count()
            })
            .OrderBy(t => t.Año)
            .ThenBy(t => t.Mes)
            .ToList();

        // Gráfica de ingresos mensuales
        var graficaIngresos = new ChartDataDTO
        {
            Type = "line",
            Labels = tendenciaMensual.Select(t => $"{t.MesNombre.Substring(0, 3)} {t.Año}").ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Ingresos ($)",
                    Data = tendenciaMensual.Select(t => t.TotalIngresos).ToList(),
                    BackgroundColor = "#10B981",
                    BorderColor = "#059669"
                }
            }
        };

        // Desglose por métodos de pago
        var desglosePagos = pagos
            .GroupBy(p => p.MetodoPago.Nombre)
            .Select(g => new DesglosePagoDTO
            {
                MetodoPago = g.Key,
                TotalRecaudado = g.Sum(p => p.MontoTotal),
                CantidadPagos = g.Count(),
                PorcentajeDelTotal = totalRecaudado > 0 ? (g.Sum(p => p.MontoTotal) / totalRecaudado * 100) : 0
            })
            .OrderByDescending(d => d.TotalRecaudado)
            .ToList();

        // Resultado
        var reporte = new ReporteIngresosDTO
        {
            TotalRecaudado = totalRecaudado,
            Cantidad = cantidad,
            Promedio = Math.Round(promedio, 2),
            ComparativaMesAnterior = comparativaMesAnterior.HasValue ? Math.Round(comparativaMesAnterior.Value, 2) : null,
            TendenciaMensual = tendenciaMensual,
            GraficaIngresosMensuales = graficaIngresos,
            DesgloseMetodosPago = desglosePagos
        };

        return Result<ReporteIngresosDTO>.Success(reporte);
    }
}

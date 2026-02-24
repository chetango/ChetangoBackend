using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using Chetango.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Chetango.Application.Reportes.Queries;

public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, Result<DashboardDTO>>
{
    private readonly IAppDbContext _db;
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public GetDashboardHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<DashboardDTO>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        // Calcular rango de fechas según parámetros
        var (fechaDesde, fechaHasta) = CalcularRangoFechas(request);
        
        // Intentar obtener del cache
        var cacheKey = $"dashboard_{fechaDesde:yyyyMMdd}_{fechaHasta:yyyyMMdd}";
        
        if (_cache.TryGetValue(cacheKey, out DashboardDTO? cachedDashboard) && cachedDashboard != null)
        {
            return Result<DashboardDTO>.Success(cachedDashboard);
        }

        // Generar dashboard
        var dashboard = await GenerarDashboard(fechaDesde, fechaHasta, cancellationToken);

        // Guardar en cache por 5 minutos
        _cache.Set(cacheKey, dashboard, TimeSpan.FromMinutes(5));

        return Result<DashboardDTO>.Success(dashboard);
    }

    private (DateTime fechaDesde, DateTime fechaHasta) CalcularRangoFechas(GetDashboardQuery request)
    {
        // Si hay fechas específicas, usarlas
        if (request.FechaDesde.HasValue && request.FechaHasta.HasValue)
        {
            return (request.FechaDesde.Value, request.FechaHasta.Value);
        }

        var hoy = DateTime.Today;

        // Si hay periodo preset, calcularlo
        if (!string.IsNullOrEmpty(request.PeriodoPreset))
        {
            return request.PeriodoPreset.ToLower() switch
            {
                "hoy" => (hoy, hoy),
                "semana" => (hoy.AddDays(-7), hoy),
                "30dias" => (hoy.AddDays(-30), hoy),
                "año" or "ano" => (new DateTime(hoy.Year, 1, 1), new DateTime(hoy.Year, 12, 31)),
                _ => (new DateTime(hoy.Year, hoy.Month, 1), hoy) // "mes" por defecto
            };
        }

        // Por defecto: este mes
        return (new DateTime(hoy.Year, hoy.Month, 1), hoy);
    }

    private async Task<DashboardDTO> GenerarDashboard(DateTime fechaDesde, DateTime fechaHasta, CancellationToken cancellationToken)
    {
        // Usar zona horaria de Colombia (UTC-5)
        var colombiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
        var hoy = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, colombiaTimeZone).Date;
        var primerDiaMes = fechaDesde;
        var ultimoDiaMes = fechaHasta;
        var proximos7Dias = hoy.AddDays(7);

        // KPIs principales
        var totalAlumnosActivos = await _db.Alumnos
            .Include(a => a.Usuario)
                .ThenInclude(u => u.Estado)
            .CountAsync(a => a.Usuario.Estado.Nombre == "Activo", cancellationToken);

        var ingresosEsteMes = await _db.Pagos
            .Where(p => p.FechaPago >= primerDiaMes && p.FechaPago <= ultimoDiaMes)
            .SumAsync(p => p.MontoTotal, cancellationToken);

        var ingresosMedellinEsteMes = await _db.Pagos
            .Where(p => p.FechaPago >= primerDiaMes && p.FechaPago <= ultimoDiaMes && p.Sede == Sede.Medellin)
            .SumAsync(p => p.MontoTotal, cancellationToken);

        var ingresosManizalesEsteMes = await _db.Pagos
            .Where(p => p.FechaPago >= primerDiaMes && p.FechaPago <= ultimoDiaMes && p.Sede == Sede.Manizales)
            .SumAsync(p => p.MontoTotal, cancellationToken);

        var clasesProximos7Dias = await _db.Clases
            .CountAsync(c => c.Fecha >= hoy && c.Fecha <= proximos7Dias, cancellationToken);

        var paquetesActivos = await _db.Paquetes
            .CountAsync(p => p.IdEstado == 1, cancellationToken);

        var paquetesVencidos = await _db.Paquetes
            .CountAsync(p => p.IdEstado == 2, cancellationToken);

        var paquetesPorVencer = await _db.Paquetes
            .CountAsync(p => p.IdEstado == 1 && p.FechaVencimiento <= proximos7Dias, cancellationToken);

        // Paquetes agotados (IdEstado == 4 "Agotado")
        var paquetesAgotados = await _db.Paquetes
            .CountAsync(p => p.IdEstado == 4, cancellationToken);

        var paquetesAgotadosMedellin = await _db.Paquetes
            .Include(p => p.Pago)
            .CountAsync(p => p.IdEstado == 4 && p.Pago != null && p.Pago.Sede == Sede.Medellin, cancellationToken);

        var paquetesAgotadosManizales = await _db.Paquetes
            .Include(p => p.Pago)
            .CountAsync(p => p.IdEstado == 4 && p.Pago != null && p.Pago.Sede == Sede.Manizales, cancellationToken);

        // Paquetes agotados (IdEstado == 4 "Agotado")
        var paquetesAgotados = await _db.Paquetes
            .CountAsync(p => p.IdEstado == 4, cancellationToken);

        var paquetesAgotadosMedellin = await _db.Paquetes
            .Include(p => p.Pago)
            .CountAsync(p => p.IdEstado == 4 && p.Pago != null && p.Pago.Sede == Sede.Medellin, cancellationToken);

        var paquetesAgotadosManizales = await _db.Paquetes
            .Include(p => p.Pago)
            .CountAsync(p => p.IdEstado == 4 && p.Pago != null && p.Pago.Sede == Sede.Manizales, cancellationToken);

        // Contar asistencias registradas hoy con estado "Presente"
        var asistenciasHoy = await _db.Asistencias
            .Include(a => a.Estado)
            .Include(a => a.Clase)
            .Where(a => a.Clase.Fecha.Date == hoy && a.Estado.Nombre == "Presente")
            .CountAsync(cancellationToken);

        var asistenciasMes = await _db.Asistencias
            .Include(a => a.Estado)
            .Include(a => a.Clase)
            .CountAsync(a => a.Clase.Fecha >= primerDiaMes && a.Clase.Fecha <= ultimoDiaMes && a.Estado.Nombre == "Presente", cancellationToken);

        var paquetesVendidos = await _db.Paquetes
            .CountAsync(p => p.FechaActivacion >= primerDiaMes && p.FechaActivacion <= ultimoDiaMes, cancellationToken);

        // Comparativas con periodo anterior
        var duracionPeriodo = (ultimoDiaMes - primerDiaMes).Days;
        var primerDiaPeriodoAnterior = primerDiaMes.AddDays(-duracionPeriodo - 1);
        var ultimoDiaPeriodoAnterior = primerDiaMes.AddDays(-1);

        var ingresosPeriodoAnterior = await _db.Pagos
            .Where(p => p.FechaPago >= primerDiaPeriodoAnterior && p.FechaPago <= ultimoDiaPeriodoAnterior)
            .SumAsync(p => p.MontoTotal, cancellationToken);

        var asistenciasPeriodoAnterior = await _db.Asistencias
            .Include(a => a.Estado)
            .Include(a => a.Clase)
            .CountAsync(a => a.Clase.Fecha >= primerDiaPeriodoAnterior && a.Clase.Fecha <= ultimoDiaPeriodoAnterior && a.Estado.Nombre == "Presente", cancellationToken);

        var alumnosPeriodoAnterior = await _db.Alumnos
            .Include(a => a.Usuario)
                .ThenInclude(u => u.Estado)
            .CountAsync(a => a.Usuario.Estado.Nombre == "Activo" && a.FechaInscripcion <= ultimoDiaPeriodoAnterior, cancellationToken);

        var paquetesVendidosPeriodoAnterior = await _db.Paquetes
            .CountAsync(p => p.FechaActivacion >= primerDiaPeriodoAnterior && p.FechaActivacion <= ultimoDiaPeriodoAnterior, cancellationToken);

        // Calcular porcentajes de cambio
        decimal? crecimientoIngresos = ingresosPeriodoAnterior > 0 
            ? ((ingresosEsteMes - ingresosPeriodoAnterior) / ingresosPeriodoAnterior) * 100 
            : null;

        decimal? comparativaAsistencias = asistenciasPeriodoAnterior > 0 
            ? ((asistenciasMes - asistenciasPeriodoAnterior) / (decimal)asistenciasPeriodoAnterior) * 100 
            : null;

        decimal? comparativaAlumnos = alumnosPeriodoAnterior > 0 
            ? ((totalAlumnosActivos - alumnosPeriodoAnterior) / (decimal)alumnosPeriodoAnterior) * 100 
            : null;

        decimal? comparativaPaquetes = paquetesVendidosPeriodoAnterior > 0 
            ? ((paquetesVendidos - paquetesVendidosPeriodoAnterior) / (decimal)paquetesVendidosPeriodoAnterior) * 100 
            : null;

        // Calcular egresos del mes (pagos a profesores)
        var egresosEsteMes = await _db.LiquidacionesMensuales
            .Where(l => l.Estado == "Pagada" && l.FechaPago >= primerDiaMes && l.FechaPago <= ultimoDiaMes)
            .SumAsync(l => l.TotalPagar, cancellationToken);

        var egresosMedellinEsteMes = await _db.LiquidacionesMensuales
            .Where(l => l.Estado == "Pagada" && l.FechaPago >= primerDiaMes && l.FechaPago <= ultimoDiaMes && l.Sede == Sede.Medellin)
            .SumAsync(l => l.TotalPagar, cancellationToken);

        var egresosManizalesEsteMes = await _db.LiquidacionesMensuales
            .Where(l => l.Estado == "Pagada" && l.FechaPago >= primerDiaMes && l.FechaPago <= ultimoDiaMes && l.Sede == Sede.Manizales)
            .SumAsync(l => l.TotalPagar, cancellationToken);

        var egresosPeriodoAnterior = await _db.LiquidacionesMensuales
            .Where(l => l.Estado == "Pagada" && l.FechaPago >= primerDiaPeriodoAnterior && l.FechaPago <= ultimoDiaPeriodoAnterior)
            .SumAsync(l => l.TotalPagar, cancellationToken);

        // Calcular ganancia neta
        var gananciaNeta = ingresosEsteMes - egresosEsteMes;
        var gananciaPeriodoAnterior = ingresosPeriodoAnterior - egresosPeriodoAnterior;

        // Comparativas de egresos y ganancia
        decimal? comparativaEgresos = egresosPeriodoAnterior > 0 
            ? ((egresosEsteMes - egresosPeriodoAnterior) / egresosPeriodoAnterior) * 100 
            : null;

        decimal? comparativaGanancia = gananciaPeriodoAnterior != 0 
            ? ((gananciaNeta - gananciaPeriodoAnterior) / Math.Abs(gananciaPeriodoAnterior)) * 100 
            : null;

        var kpis = new DashboardKPIsDTO
        {
            TotalAlumnosActivos = totalAlumnosActivos,
            IngresosEsteMes = ingresosEsteMes,
            IngresosMedellinEsteMes = ingresosMedellinEsteMes,
            IngresosManizalesEsteMes = ingresosManizalesEsteMes,
            ClasesProximos7Dias = clasesProximos7Dias,
            PaquetesActivos = paquetesActivos,
            PaquetesVencidos = paquetesVencidos,
            PaquetesPorVencer = paquetesPorVencer,
            PaquetesAgotados = paquetesAgotados,
            PaquetesAgotadosMedellin = paquetesAgotadosMedellin,
            PaquetesAgotadosManizales = paquetesAgotadosManizales,
            PaquetesVendidos = paquetesVendidos,
            AsistenciasHoy = asistenciasHoy,
            AsistenciasMes = asistenciasMes,
            EgresosEsteMes = egresosEsteMes,
            EgresosMedellinEsteMes = egresosMedellinEsteMes,
            EgresosManizalesEsteMes = egresosManizalesEsteMes,
            GananciaNeta = gananciaNeta,
            CrecimientoIngresosMesAnterior = crecimientoIngresos.HasValue ? Math.Round(crecimientoIngresos.Value, 2) : null,
            ComparativaAsistenciasMesAnterior = comparativaAsistencias.HasValue ? Math.Round(comparativaAsistencias.Value, 2) : null,
            ComparativaAlumnosMesAnterior = comparativaAlumnos.HasValue ? Math.Round(comparativaAlumnos.Value, 2) : null,
            ComparativaPaquetesVendidosMesAnterior = comparativaPaquetes.HasValue ? Math.Round(comparativaPaquetes.Value, 2) : null,
            ComparativaEgresosMesAnterior = comparativaEgresos.HasValue ? Math.Round(comparativaEgresos.Value, 2) : null,
            ComparativaGananciaMesAnterior = comparativaGanancia.HasValue ? Math.Round(comparativaGanancia.Value, 2) : null
        };

        // Gráfica de ingresos (últimos 6 meses)
        var fechaInicio6Meses = hoy.AddMonths(-5);
        var fechaInicio6MesesPrimerDia = new DateTime(fechaInicio6Meses.Year, fechaInicio6Meses.Month, 1);

        var ingresosMensuales = await _db.Pagos
            .Where(p => p.FechaPago >= fechaInicio6MesesPrimerDia)
            .ToListAsync(cancellationToken);

        var ingresosPorMes = ingresosMensuales
            .GroupBy(p => new { p.FechaPago.Year, p.FechaPago.Month })
            .Select(g => new
            {
                Año = g.Key.Year,
                Mes = g.Key.Month,
                Total = g.Sum(p => p.MontoTotal)
            })
            .OrderBy(x => x.Año)
            .ThenBy(x => x.Mes)
            .ToList();

        var graficaIngresos = new ChartDataDTO
        {
            Type = "line",
            Labels = ingresosPorMes.Select(x => 
                $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Mes).Substring(0, 3)} {x.Año}").ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Ingresos ($)",
                    Data = ingresosPorMes.Select(x => x.Total).ToList(),
                    BackgroundColor = "#10B981",
                    BorderColor = "#059669"
                }
            }
        };

        // Gráfica de asistencias por día de la semana (últimos 30 días)
        var fecha30DiasAtras = hoy.AddDays(-30);
        var asistenciasRecientes = await _db.Asistencias
            .Include(a => a.Clase)
            .Include(a => a.Estado)
            .Where(a => a.Clase.Fecha >= fecha30DiasAtras && a.Estado.Nombre == "Presente")
            .Select(a => new { a.Clase.Fecha.DayOfWeek })
            .ToListAsync(cancellationToken);

        var asistenciasPorDia = asistenciasRecientes
            .GroupBy(a => a.DayOfWeek)
            .Select(g => new
            {
                DiaSemana = g.Key,
                Cantidad = g.Count()
            })
            .OrderBy(x => x.DiaSemana)
            .ToList();

        var graficaAsistencias = new ChartDataDTO
        {
            Type = "bar",
            Labels = asistenciasPorDia.Select(x => GetDiaSemanaEspanol(x.DiaSemana)).ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Asistencias",
                    Data = asistenciasPorDia.Select(x => (decimal)x.Cantidad).ToList(),
                    BackgroundColor = "#6366F1",
                    BorderColor = "#4F46E5"
                }
            }
        };

        // Gráfica de paquetes por estado
        var paquetesPorEstado = await _db.Paquetes
            .Include(p => p.Estado)
            .GroupBy(p => p.Estado.Nombre)
            .Select(g => new
            {
                Estado = g.Key,
                Cantidad = g.Count()
            })
            .ToListAsync(cancellationToken);

        var graficaPaquetes = new ChartDataDTO
        {
            Type = "doughnut",
            Labels = paquetesPorEstado.Select(x => x.Estado).ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Paquetes",
                    Data = paquetesPorEstado.Select(x => (decimal)x.Cantidad).ToList(),
                    BackgroundColor = "#F59E0B"
                }
            }
        };

        // Gráfica de métodos de pago (distribución)
        var pagosPorMetodo = await _db.Pagos
            .Include(p => p.MetodoPago)
            .Where(p => p.FechaPago >= primerDiaMes && p.FechaPago <= ultimoDiaMes)
            .GroupBy(p => p.MetodoPago.Nombre)
            .Select(g => new
            {
                Metodo = g.Key,
                Total = g.Sum(p => p.MontoTotal),
                Cantidad = g.Count()
            })
            .ToListAsync(cancellationToken);

        var totalIngresos = pagosPorMetodo.Sum(x => x.Total);
        var graficaMetodosPago = new ChartDataDTO
        {
            Type = "pie",
            Labels = pagosPorMetodo.Select(x => x.Metodo).ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Métodos de Pago",
                    Data = pagosPorMetodo.Select(x => x.Total).ToList(),
                    BackgroundColor = "#8B5CF6"
                }
            }
        };

        // Resumen del periodo
        var totalPagos = await _db.Pagos
            .Where(p => p.FechaPago >= primerDiaMes && p.FechaPago <= ultimoDiaMes)
            .CountAsync(cancellationToken);

        var alumnosConPaquetes = await _db.Paquetes
            .Where(p => p.FechaActivacion >= primerDiaMes && p.FechaActivacion <= ultimoDiaMes)
            .Select(p => p.IdAlumno)
            .Distinct()
            .CountAsync(cancellationToken);

        var tasaConversion = totalAlumnosActivos > 0 
            ? (alumnosConPaquetes / (decimal)totalAlumnosActivos) * 100 
            : 0;

        var resumenPeriodo = new ResumenPeriodoDTO
        {
            TotalRecaudado = ingresosEsteMes,
            CantidadPagos = totalPagos,
            PromedioPorPago = totalPagos > 0 ? ingresosEsteMes / totalPagos : 0,
            TasaConversion = Math.Round(tasaConversion, 2)
        };

        // Últimos pagos registrados (Top 10)
        var ultimosPagos = await _db.Pagos
            .Include(p => p.Alumno)
                .ThenInclude(a => a.Usuario)
            .Include(p => p.MetodoPago)
            .Include(p => p.Paquetes)
                .ThenInclude(paq => paq.TipoPaquete)
            .OrderByDescending(p => p.FechaPago)
            .Take(10)
            .Select(p => new UltimoPagoDTO
            {
                IdPago = p.IdPago,
                NombreAlumno = p.Alumno.Usuario.NombreUsuario,
                Fecha = p.FechaPago,
                Monto = p.MontoTotal,
                MetodoPago = p.MetodoPago.Nombre,
                NombrePaquete = p.Paquetes.Any() ? p.Paquetes.First().TipoPaquete.Nombre : "N/A",
                Estado = "Pagado" // Todos los pagos registrados están pagados
            })
            .ToListAsync(cancellationToken);

        // Alertas del sistema
        var alertas = new List<AlertaDTO>();

        // Alerta: Paquetes venciendo esta semana
        var paquetesVenciendoSemana = await _db.Paquetes
            .Include(p => p.Alumno)
            .ThenInclude(a => a.Usuario)
            .Include(p => p.Estado)
            .Where(p => p.Estado.Nombre == "Activo" && p.FechaVencimiento <= proximos7Dias && p.FechaVencimiento >= hoy)
            .CountAsync(cancellationToken);

        if (paquetesVenciendoSemana > 0)
        {
            alertas.Add(new AlertaDTO
            {
                Tipo = TipoAlerta.PaquetePorVencer,
                Titulo = "Paquetes por Vencer",
                Descripcion = $"{paquetesVenciendoSemana} paquete(s) vencen en los próximos 7 días",
                FechaGeneracion = DateTime.Now,
                Prioridad = PrioridadAlerta.Alta,
                DatosAdicionales = new Dictionary<string, object>
                {
                    { "cantidad", paquetesVenciendoSemana }
                }
            });
        }

        // Alerta: Alumnos inactivos (sin asistencias en 30 días)
        var alumnosConAsistencias = await _db.Alumnos
            .Include(a => a.Usuario)
                .ThenInclude(u => u.Estado)
            .Where(a => a.Usuario.Estado.Nombre == "Activo")
            .Select(a => new
            {
                a.IdAlumno,
                UltimaFechaAsistencia = a.Asistencias
                    .OrderByDescending(ast => ast.Clase.Fecha)
                    .Select(ast => ast.Clase.Fecha)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var cantidadInactivos = alumnosConAsistencias
            .Count(a => a.UltimaFechaAsistencia == default || a.UltimaFechaAsistencia < fecha30DiasAtras);

        if (cantidadInactivos > 0)
        {
            alertas.Add(new AlertaDTO
            {
                Tipo = TipoAlerta.AlumnoInactivo,
                Titulo = "Alumnos Inactivos",
                Descripcion = $"{cantidadInactivos} alumno(s) sin asistencias en más de 30 días",
                FechaGeneracion = DateTime.Now,
                Prioridad = PrioridadAlerta.Media,
                DatosAdicionales = new Dictionary<string, object>
                {
                    { "cantidad", cantidadInactivos }
                }
            });
        }

        // Alerta: Clases con pocos inscritos (próximos 7 días con menos de 5 inscritos)
        var clasesPocosCupos = await _db.Clases
            .Include(c => c.TipoClase)
            .Where(c => c.Fecha >= hoy && c.Fecha <= proximos7Dias)
            .Select(c => new
            {
                Clase = c,
                Inscritos = _db.Asistencias.Count(a => a.IdClase == c.IdClase)
            })
            .ToListAsync(cancellationToken);

        var clasesConPocos = clasesPocosCupos.Where(x => x.Inscritos < 5).ToList();

        if (clasesConPocos.Any())
        {
            var primeraClase = clasesConPocos.First();
            alertas.Add(new AlertaDTO
            {
                Tipo = TipoAlerta.ClasePocosCupos,
                Titulo = "Clases con Pocos Inscritos",
                Descripcion = $"Clase \"{primeraClase.Clase.TipoClase.Nombre}\" con solo {primeraClase.Inscritos} inscrito(s)",
                FechaGeneracion = DateTime.Now,
                Prioridad = PrioridadAlerta.Alta,
                DatosAdicionales = new Dictionary<string, object>
                {
                    { "cantidad", clasesConPocos.Count },
                    { "idClase", primeraClase.Clase.IdClase }
                }
            });
        }

        // Alerta: Pagos pendientes de confirmación
        // Nota: Modelo Pago no tiene estado, todos los pagos en BD están confirmados
        // Esta alerta se mantiene como 0 o se puede implementar con lógica diferente
        var pagosPendientes = 0; // Placeholder - ajustar según lógica de negocio

        if (pagosPendientes > 0)
        {
            alertas.Add(new AlertaDTO
            {
                Tipo = TipoAlerta.PagoPendiente,
                Titulo = "Pagos Pendientes",
                Descripcion = $"{pagosPendientes} pago(s) pendientes de confirmación",
                FechaGeneracion = DateTime.Now,
                Prioridad = PrioridadAlerta.Media,
                DatosAdicionales = new Dictionary<string, object>
                {
                    { "cantidad", pagosPendientes }
                }
            });
        }

        // Construir dashboard
        var dashboard = new DashboardDTO
        {
            KPIs = kpis,
            GraficaIngresos = graficaIngresos,
            GraficaAsistencias = graficaAsistencias,
            GraficaPaquetes = graficaPaquetes,
            GraficaMetodosPago = graficaMetodosPago,
            ResumenPeriodo = resumenPeriodo,
            UltimosPagos = ultimosPagos,
            Alertas = alertas
        };

        return dashboard;
    }

    private static string GetDiaSemanaEspanol(DayOfWeek dia)
    {
        return dia switch
        {
            DayOfWeek.Monday => "Lunes",
            DayOfWeek.Tuesday => "Martes",
            DayOfWeek.Wednesday => "Miércoles",
            DayOfWeek.Thursday => "Jueves",
            DayOfWeek.Friday => "Viernes",
            DayOfWeek.Saturday => "Sábado",
            DayOfWeek.Sunday => "Domingo",
            _ => dia.ToString()
        };
    }
}

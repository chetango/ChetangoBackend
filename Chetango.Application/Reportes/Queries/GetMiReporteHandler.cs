using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Application.Reportes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Chetango.Application.Reportes.Queries;

public class GetMiReporteHandler : IRequestHandler<GetMiReporteQuery, Result<MiReporteDTO>>
{
    private readonly IAppDbContext _db;

    public GetMiReporteHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<MiReporteDTO>> Handle(GetMiReporteQuery request, CancellationToken cancellationToken)
    {
        // Buscar alumno por email (ownership validation)
        var alumno = await _db.Alumnos
            .Include(a => a.Usuario)
            .Include(a => a.Asistencias)
                .ThenInclude(ast => ast.Estado)
            .Include(a => a.Asistencias)
                .ThenInclude(ast => ast.Clase)
                .ThenInclude(c => c.TipoClase)
            .Include(a => a.Pagos)
                .ThenInclude(p => p.MetodoPago)
            .FirstOrDefaultAsync(a => a.Usuario.Correo == request.EmailUsuario, cancellationToken);

        if (alumno == null)
            return Result<MiReporteDTO>.Failure("No se encontró el alumno autenticado.");

        // Buscar paquetes del alumno (relación unidireccional desde Paquete)
        var paquetesAlumno = await _db.Paquetes
            .Include(p => p.TipoPaquete)
            .Include(p => p.Estado)
            .Where(p => p.IdAlumno == alumno.IdAlumno)
            .ToListAsync(cancellationToken);

        // Total clases tomadas (presentes)
        var totalClasesTomadas = alumno.Asistencias.Count(a => a.Estado.Nombre == "Presente");

        // Porcentaje de asistencia
        var totalAsistenciasRegistradas = alumno.Asistencias.Count;
        var porcentajeAsistencia = totalAsistenciasRegistradas > 0
            ? (decimal)totalClasesTomadas / totalAsistenciasRegistradas * 100
            : 0;

        // Paquete actual activo
        var paqueteActivo = paquetesAlumno
            .Where(p => p.Estado.Nombre == "Activo")
            .OrderByDescending(p => p.FechaActivacion)
            .FirstOrDefault();

        PaqueteResumenDTO? paqueteActual = null;
        int? clasesRestantes = null;

        if (paqueteActivo != null)
        {
            var clasesRestantesCalc = paqueteActivo.ClasesDisponibles - paqueteActivo.ClasesUsadas;
            paqueteActual = new PaqueteResumenDTO(
                paqueteActivo.IdPaquete,
                paqueteActivo.IdAlumno,
                alumno.Usuario.NombreUsuario,
                paqueteActivo.TipoPaquete.Nombre,
                paqueteActivo.ClasesDisponibles,
                paqueteActivo.ClasesUsadas,
                clasesRestantesCalc,
                paqueteActivo.FechaVencimiento,
                paqueteActivo.Estado.Nombre,
                paqueteActivo.ValorPaquete
            );
            clasesRestantes = clasesRestantesCalc;
        }

        // Historial de pagos
        var historialPagos = alumno.Pagos
            .OrderByDescending(p => p.FechaPago)
            .Take(10)
            .Select(p => new PagoResumenDTO
            {
                IdPago = p.IdPago,
                FechaPago = p.FechaPago,
                Monto = p.MontoTotal,
                MetodoPago = p.MetodoPago.Nombre,
                ConceptoPago = p.Nota ?? "",
                IdPaqueteGenerado = null
            })
            .ToList();

        // Gráfica de asistencias mensuales (últimos 6 meses)
        var fechaInicio6Meses = DateTime.Today.AddMonths(-5);
        var fechaInicio6MesesPrimerDia = new DateTime(fechaInicio6Meses.Year, fechaInicio6Meses.Month, 1);

        var asistenciasPorMes = alumno.Asistencias
            .Where(a => a.Estado.Nombre == "Presente" && a.Clase.Fecha >= fechaInicio6MesesPrimerDia)
            .GroupBy(a => new { a.Clase.Fecha.Year, a.Clase.Fecha.Month })
            .Select(g => new
            {
                Año = g.Key.Year,
                Mes = g.Key.Month,
                Cantidad = g.Count()
            })
            .OrderBy(x => x.Año)
            .ThenBy(x => x.Mes)
            .ToList();

        var graficaAsistencias = new ChartDataDTO
        {
            Type = "bar",
            Labels = asistenciasPorMes.Select(x =>
                $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Mes).Substring(0, 3)} {x.Año}").ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Asistencias",
                    Data = asistenciasPorMes.Select(x => (decimal)x.Cantidad).ToList(),
                    BackgroundColor = "#8B5CF6",
                    BorderColor = "#7C3AED"
                }
            }
        };

        // Próximas clases programadas (próximos 7 días)
        var hoy = DateTime.Today;
        var proximos7Dias = hoy.AddDays(7);

        var proximasClases = await _db.Clases
            .Include(c => c.TipoClase)
            .Include(c => c.ProfesorPrincipal)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Asistencias)
            .Where(c => c.Fecha >= hoy && c.Fecha <= proximos7Dias)
            .OrderBy(c => c.Fecha)
            .ThenBy(c => c.HoraInicio)
            .Take(5)
            .Select(c => new ClaseProximaDTO
            {
                IdClase = c.IdClase,
                Fecha = c.Fecha,
                HoraInicio = c.HoraInicio,
                TipoClase = c.TipoClase.Nombre,
                NombreProfesor = c.ProfesorPrincipal.Usuario.NombreUsuario,
                CupoMaximo = c.CupoMaximo,
                InscritosActual = c.Asistencias.Count
            })
            .ToListAsync(cancellationToken);

        // Resultado
        var reporte = new MiReporteDTO
        {
            NombreAlumno = alumno.Usuario.NombreUsuario,
            Correo = alumno.Usuario.Correo,
            FechaInscripcion = alumno.Usuario.FechaCreacion,
            TotalClasesTomadas = totalClasesTomadas,
            PorcentajeAsistencia = Math.Round(porcentajeAsistencia, 2),
            ClasesRestantes = clasesRestantes,
            PaqueteActual = paqueteActual,
            HistorialPagos = historialPagos,
            GraficaAsistenciasMensuales = graficaAsistencias,
            ProximasClases = proximasClases
        };

        return Result<MiReporteDTO>.Success(reporte);
    }
}

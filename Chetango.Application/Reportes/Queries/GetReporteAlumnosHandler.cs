using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Chetango.Application.Reportes.Queries;

public class GetReporteAlumnosHandler : IRequestHandler<GetReporteAlumnosQuery, Result<ReporteAlumnosDTO>>
{
    private readonly IAppDbContext _db;

    public GetReporteAlumnosHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ReporteAlumnosDTO>> Handle(GetReporteAlumnosQuery request, CancellationToken cancellationToken)
    {
        // Base query con TODOS los includes necesarios
        var query = _db.Alumnos
            .Include(a => a.Usuario)
                .ThenInclude(u => u.Estado)  // ← Incluir EstadoUsuario
            .Include(a => a.Asistencias)
                .ThenInclude(ast => ast.Estado)  // ← Incluir EstadoAsistencia
            .Include(a => a.Asistencias)
                .ThenInclude(ast => ast.Clase)
            .AsQueryable();

        // Aplicar filtros opcionales
        if (request.FechaInscripcionDesde.HasValue)
            query = query.Where(a => a.Usuario.FechaCreacion >= request.FechaInscripcionDesde.Value);

        if (request.FechaInscripcionHasta.HasValue)
            query = query.Where(a => a.Usuario.FechaCreacion <= request.FechaInscripcionHasta.Value);

        if (!string.IsNullOrEmpty(request.Estado))
            query = query.Where(a => a.Usuario.Estado.Nombre == request.Estado);

        // Ejecutar query
        var alumnos = await query.ToListAsync(cancellationToken);

        // Calcular métricas
        var totalActivos = alumnos.Count(a => a.Usuario.Estado.Nombre == "Activo");
        var totalInactivos = alumnos.Count(a => a.Usuario.Estado.Nombre != "Activo");

        // Nuevos este mes
        var primerDiaMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var nuevosEsteMes = alumnos.Count(a => a.Usuario.FechaCreacion >= primerDiaMes);

        // Tasa de retención (alumnos con paquetes activos / total activos)
        var idsAlumnosConPaquetesActivos = await _db.Paquetes
            .Include(p => p.Estado)  // ← Incluir EstadoPaquete
            .Where(p => p.Estado.Nombre == "Activo")
            .Select(p => p.IdAlumno)
            .Distinct()
            .ToListAsync(cancellationToken);
        
        var alumnosConPaquetesActivos = alumnos.Count(a => 
            a.Usuario.Estado.Nombre == "Activo" && idsAlumnosConPaquetesActivos.Contains(a.IdAlumno));
        var tasaRetencion = totalActivos > 0 
            ? (decimal)alumnosConPaquetesActivos / totalActivos * 100 
            : 0;

        // Alumnos inactivos (sin asistencias en más de 30 días)
        var fecha30DiasAtras = DateTime.Today.AddDays(-30);
        var alumnosInactivos = alumnos
            .Where(a => a.Usuario.Estado.Nombre == "Activo")
            .Select(a => new
            {
                Alumno = a,
                UltimaAsistencia = a.Asistencias
                    .Where(ast => ast.Estado.Nombre == "Presente")
                    .OrderByDescending(ast => ast.Clase.Fecha)
                    .FirstOrDefault()
            })
            .Where(x => x.UltimaAsistencia == null || x.UltimaAsistencia.Clase.Fecha < fecha30DiasAtras)
            .Select(x => new AlumnoInactivoDTO
            {
                IdAlumno = x.Alumno.IdAlumno,
                NombreAlumno = x.Alumno.Usuario.NombreUsuario,
                Correo = x.Alumno.Usuario.Correo,
                UltimaAsistencia = x.UltimaAsistencia != null ? x.UltimaAsistencia.Clase.Fecha : (DateTime?)null,
                DiasInactivo = x.UltimaAsistencia != null 
                    ? (int)(DateTime.Today - x.UltimaAsistencia.Clase.Fecha).TotalDays 
                    : 999
            })
            .OrderByDescending(x => x.DiasInactivo)
            .Take(20)
            .ToList();

        // Alumnos con paquetes próximos a vencer (7 días)
        var fechaLimite = DateTime.Today.AddDays(7);
        var alumnosPorVencer = await _db.Paquetes
            .Include(p => p.Estado)  // ← Incluir EstadoPaquete
            .Include(p => p.Alumno)
                .ThenInclude(a => a.Usuario)
            .Where(p => p.Estado.Nombre == "Activo" && p.FechaVencimiento <= fechaLimite && p.FechaVencimiento >= DateTime.Today)
            .OrderBy(p => p.FechaVencimiento)  // ← Ordenar por fecha directamente (traducible a SQL)
            .ToListAsync(cancellationToken);

        // Proyectar a DTO después de cargar en memoria
        var alumnosPorVencerDTO = alumnosPorVencer
            .Select(p => new AlumnoPorVencerDTO
            {
                IdAlumno = p.IdAlumno,
                NombreAlumno = p.Alumno.Usuario.NombreUsuario,
                Correo = p.Alumno.Usuario.Correo,
                FechaVencimiento = p.FechaVencimiento,
                DiasRestantes = (int)(p.FechaVencimiento - DateTime.Today).TotalDays
            })
            .ToList();

        // Gráfica de alumnos nuevos por mes (últimos 12 meses)
        var fechaInicio12Meses = DateTime.Today.AddMonths(-11);
        var fechaInicio12MesesPrimerDia = new DateTime(fechaInicio12Meses.Year, fechaInicio12Meses.Month, 1);

        var alumnosPorMes = alumnos
            .Where(a => a.Usuario.FechaCreacion >= fechaInicio12MesesPrimerDia)
            .GroupBy(a => new { a.Usuario.FechaCreacion.Year, a.Usuario.FechaCreacion.Month })
            .Select(g => new
            {
                Año = g.Key.Year,
                Mes = g.Key.Month,
                Cantidad = g.Count()
            })
            .OrderBy(x => x.Año)
            .ThenBy(x => x.Mes)
            .ToList();

        var graficaAlumnos = new ChartDataDTO
        {
            Type = "line",
            Labels = alumnosPorMes.Select(x => 
                $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Mes).Substring(0, 3)} {x.Año}").ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Nuevos Alumnos",
                    Data = alumnosPorMes.Select(x => (decimal)x.Cantidad).ToList(),
                    BackgroundColor = "#3B82F6",
                    BorderColor = "#2563EB"
                }
            }
        };

        // Resultado
        var reporte = new ReporteAlumnosDTO
        {
            TotalActivos = totalActivos,
            TotalInactivos = totalInactivos,
            NuevosEsteMes = nuevosEsteMes,
            TasaRetencion = Math.Round(tasaRetencion, 2),
            AlumnosInactivos = alumnosInactivos,
            AlumnosPorVencer = alumnosPorVencerDTO,
            GraficaAlumnosPorMes = graficaAlumnos
        };

        return Result<ReporteAlumnosDTO>.Success(reporte);
    }
}

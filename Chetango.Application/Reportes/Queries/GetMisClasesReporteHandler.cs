using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Reportes.Queries;

public class GetMisClasesReporteHandler : IRequestHandler<GetMisClasesReporteQuery, Result<MisClasesReporteDTO>>
{
    private readonly IAppDbContext _db;

    public GetMisClasesReporteHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<MisClasesReporteDTO>> Handle(GetMisClasesReporteQuery request, CancellationToken cancellationToken)
    {
        // Validaciones
        if (request.FechaDesde > request.FechaHasta)
            return Result<MisClasesReporteDTO>.Failure("La fecha inicial no puede ser mayor a la fecha final.");

        if ((request.FechaHasta - request.FechaDesde).Days > 365)
            return Result<MisClasesReporteDTO>.Failure("El rango de fechas no puede ser mayor a 1 año.");

        // Buscar profesor por email (ownership validation)
        var profesor = await _db.Profesores
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Usuario.Correo == request.EmailUsuario, cancellationToken);

        if (profesor == null)
            return Result<MisClasesReporteDTO>.Failure("No se encontró el profesor autenticado.");

        // Obtener clases del profesor en el periodo
        var clases = await _db.Clases
            .Include(c => c.TipoClase)
            .Include(c => c.Asistencias)
                .ThenInclude(a => a.Estado)
            .Include(c => c.Asistencias)
                .ThenInclude(a => a.Alumno)
            .Where(c => c.IdProfesorPrincipal == profesor.IdProfesor &&
                       c.Fecha >= request.FechaDesde &&
                       c.Fecha <= request.FechaHasta)
            .ToListAsync(cancellationToken);

        // Total clases impartidas
        var totalClasesImpartidas = clases.Count;

        // Promedio de asistencia
        var clasesConAsistencias = clases.Where(c => c.Asistencias.Any()).ToList();
        var promedioAsistencia = clasesConAsistencias.Any()
            ? (decimal)clasesConAsistencias.Average(c => c.Asistencias.Count(a => a.Estado.Nombre == "Presente"))
            : 0;

        // Alumnos únicos atendidos
        var alumnosUnicos = clases
            .SelectMany(c => c.Asistencias)
            .Where(a => a.Estado.Nombre == "Presente")
            .Select(a => a.IdAlumno)
            .Distinct()
            .Count();

        // Clases próximos 7 días
        var hoy = DateTime.Today;
        var proximos7Dias = hoy.AddDays(7);

        var clasesProximas = await _db.Clases
            .Include(c => c.TipoClase)
            .Include(c => c.Asistencias)
            .Where(c => c.IdProfesorPrincipal == profesor.IdProfesor &&
                       c.Fecha >= hoy &&
                       c.Fecha <= proximos7Dias)
            .OrderBy(c => c.Fecha)
            .ThenBy(c => c.HoraInicio)
            .Select(c => new ClaseProximaDTO
            {
                IdClase = c.IdClase,
                Fecha = c.Fecha,
                HoraInicio = c.HoraInicio,
                TipoClase = c.TipoClase.Nombre,
                NombreProfesor = profesor.Usuario.NombreUsuario,
                CupoMaximo = c.CupoMaximo,
                InscritosActual = c.Asistencias.Count
            })
            .ToListAsync(cancellationToken);

        // Gráfica de asistencia por tipo de clase
        var asistenciasPorTipo = clases
            .GroupBy(c => c.TipoClase.Nombre)
            .Select(g => new
            {
                TipoClase = g.Key,
                TotalAsistencias = g.SelectMany(c => c.Asistencias)
                    .Count(a => a.Estado.Nombre == "Presente")
            })
            .OrderByDescending(x => x.TotalAsistencias)
            .ToList();

        var graficaAsistencias = new ChartDataDTO
        {
            Type = "bar",
            Labels = asistenciasPorTipo.Select(x => x.TipoClase).ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Asistencias",
                    Data = asistenciasPorTipo.Select(x => (decimal)x.TotalAsistencias).ToList(),
                    BackgroundColor = "#EC4899",
                    BorderColor = "#DB2777"
                }
            }
        };

        // Desglose por tipo de clase
        var desglosePorTipo = clases
            .GroupBy(c => c.TipoClase.Nombre)
            .Select(g => new ClasesProfesorPorTipoDTO
            {
                NombreTipoClase = g.Key,
                CantidadClases = g.Count(),
                PromedioAsistencia = g.Where(c => c.Asistencias.Any()).Any()
                    ? (decimal)g.Where(c => c.Asistencias.Any())
                        .Average(c => c.Asistencias.Count(a => a.Estado.Nombre == "Presente"))
                    : 0,
                OcupacionPromedio = g.Where(c => c.Asistencias.Any() && c.CupoMaximo > 0).Any()
                    ? (decimal)g.Where(c => c.Asistencias.Any() && c.CupoMaximo > 0)
                        .Average(c => (decimal)c.Asistencias.Count(a => a.Estado.Nombre == "Presente") / c.CupoMaximo * 100)
                    : 0
            })
            .OrderByDescending(d => d.CantidadClases)
            .ToList();

        // Resultado
        var reporte = new MisClasesReporteDTO
        {
            NombreProfesor = profesor.Usuario.NombreUsuario,
            Correo = profesor.Usuario.Correo,
            TotalClasesImpartidas = totalClasesImpartidas,
            PromedioAsistencia = Math.Round(promedioAsistencia, 2),
            AlumnosUnicos = alumnosUnicos,
            ClasesProximas = clasesProximas,
            GraficaAsistenciaPorTipo = graficaAsistencias,
            DesgloseporTipo = desglosePorTipo
        };

        return Result<MisClasesReporteDTO>.Success(reporte);
    }
}

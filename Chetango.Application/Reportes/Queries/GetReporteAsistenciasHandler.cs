using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Chetango.Application.Reportes.Queries;

public class GetReporteAsistenciasHandler : IRequestHandler<GetReporteAsistenciasQuery, Result<ReporteAsistenciasDTO>>
{
    private readonly IAppDbContext _db;

    public GetReporteAsistenciasHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ReporteAsistenciasDTO>> Handle(GetReporteAsistenciasQuery request, CancellationToken cancellationToken)
    {
        // Validaciones
        if (request.FechaDesde > request.FechaHasta)
            return Result<ReporteAsistenciasDTO>.Failure("La fecha inicial no puede ser mayor a la fecha final.");

        if (request.FechaDesde > DateTime.Today)
            return Result<ReporteAsistenciasDTO>.Failure("No se pueden generar reportes de fechas futuras.");

        if ((request.FechaHasta - request.FechaDesde).Days > 365)
            return Result<ReporteAsistenciasDTO>.Failure("El rango de fechas no puede ser mayor a 1 año.");

        // Base query
        var query = _db.Asistencias
            .Include(a => a.Estado)
            .Include(a => a.Alumno)
            .ThenInclude(al => al.Usuario)
            .Include(a => a.Clase)
            .ThenInclude(c => c.TipoClase)
            .Include(a => a.Clase)
            .ThenInclude(c => c.ProfesorPrincipal)
            .ThenInclude(p => p.Usuario)
            .Where(a => a.Clase.Fecha >= request.FechaDesde && a.Clase.Fecha <= request.FechaHasta)
            .AsQueryable();

        // Ownership validation: Profesor solo ve asistencias de SUS clases
        if (request.EsProfesor && !request.EsAdmin)
        {
            var profesor = await _db.Profesores
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Usuario.Correo == request.EmailUsuario, cancellationToken);

            if (profesor == null)
                return Result<ReporteAsistenciasDTO>.Failure("No se encontró el profesor autenticado.");

            query = query.Where(a => a.Clase.IdProfesorPrincipal == profesor.IdProfesor);
        }

        // Aplicar filtros opcionales
        if (request.IdClase.HasValue)
            query = query.Where(a => a.IdClase == request.IdClase.Value);

        if (request.IdAlumno.HasValue)
            query = query.Where(a => a.IdAlumno == request.IdAlumno.Value);

        if (request.IdProfesor.HasValue)
            query = query.Where(a => a.Clase.IdProfesorPrincipal == request.IdProfesor.Value);

        if (!string.IsNullOrEmpty(request.EstadoAsistencia))
            query = query.Where(a => a.Estado.Nombre == request.EstadoAsistencia);

        // Ejecutar query
        var asistencias = await query
            .OrderByDescending(a => a.Clase.Fecha)
            .ToListAsync(cancellationToken);

        // Calcular métricas
        var totalAsistencias = asistencias.Count;
        var presentes = asistencias.Count(a => a.Estado.Nombre == "Presente");
        var ausentes = asistencias.Count(a => a.Estado.Nombre == "Ausente");
        var justificadas = asistencias.Count(a => a.Estado.Nombre == "Justificada");
        var porcentajeAsistencia = totalAsistencias > 0 ? (decimal)presentes / totalAsistencias * 100 : 0;

        // Lista detallada (limitada a 100 registros para performance)
        var listaDetallada = asistencias
            .Take(100)
            .Select(a => new AsistenciaDetalleDTO
            {
                Fecha = a.Clase.Fecha,
                NombreAlumno = a.Alumno.Usuario.NombreUsuario,
                NombreClase = a.Clase.TipoClase.Nombre,
                Estado = a.Estado.Nombre,
                Observaciones = a.Observacion,
                NombreProfesor = a.Clase.ProfesorPrincipal.Usuario.NombreUsuario
            })
            .ToList();

        // Gráfica de asistencias por día de la semana
        var asistenciasPorDia = asistencias
            .Where(a => a.Estado.Nombre == "Presente")
            .GroupBy(a => a.Clase.Fecha.DayOfWeek)
            .Select(g => new
            {
                DiaSemana = g.Key,
                Cantidad = g.Count()
            })
            .OrderBy(x => x.DiaSemana)
            .ToList();

        var chartData = new ChartDataDTO
        {
            Type = "bar",
            Labels = asistenciasPorDia.Select(x => GetDiaSemanaEspanol(x.DiaSemana)).ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Asistencias",
                    Data = asistenciasPorDia.Select(x => (decimal)x.Cantidad).ToList(),
                    BackgroundColor = "#4F46E5",
                    BorderColor = "#4338CA"
                }
            }
        };

        // Resultado
        var reporte = new ReporteAsistenciasDTO
        {
            TotalAsistencias = totalAsistencias,
            Presentes = presentes,
            Ausentes = ausentes,
            Justificadas = justificadas,
            PorcentajeAsistencia = Math.Round(porcentajeAsistencia, 2),
            ListaDetallada = listaDetallada,
            GraficaAsistenciasPorDia = chartData
        };

        return Result<ReporteAsistenciasDTO>.Success(reporte);
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

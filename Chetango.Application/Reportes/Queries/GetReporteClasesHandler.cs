using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Reportes.Queries;

public class GetReporteClasesHandler : IRequestHandler<GetReporteClasesQuery, Result<ReporteClasesDTO>>
{
    private readonly IAppDbContext _db;

    public GetReporteClasesHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ReporteClasesDTO>> Handle(GetReporteClasesQuery request, CancellationToken cancellationToken)
    {
        // Validaciones
        if (request.FechaDesde > request.FechaHasta)
            return Result<ReporteClasesDTO>.Failure("La fecha inicial no puede ser mayor a la fecha final.");

        if ((request.FechaHasta - request.FechaDesde).Days > 365)
            return Result<ReporteClasesDTO>.Failure("El rango de fechas no puede ser mayor a 1 año.");

        // Base query
        var query = _db.Clases
            .Include(c => c.TipoClase)
            .Include(c => c.ProfesorPrincipal)
            .ThenInclude(p => p.Usuario)
            .Include(c => c.Asistencias)
                .ThenInclude(a => a.Estado)
            .Where(c => c.Fecha >= request.FechaDesde && c.Fecha <= request.FechaHasta)
            .AsQueryable();

        // Ownership validation: Profesor solo ve SUS clases
        if (request.EsProfesor && !request.EsAdmin)
        {
            var profesor = await _db.Profesores
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Usuario.Correo == request.EmailUsuario, cancellationToken);

            if (profesor == null)
                return Result<ReporteClasesDTO>.Failure("No se encontró el profesor autenticado.");

            query = query.Where(c => c.IdProfesorPrincipal == profesor.IdProfesor);
        }

        // Aplicar filtros opcionales
        if (request.IdTipoClase.HasValue)
            query = query.Where(c => c.IdTipoClase == request.IdTipoClase.Value);

        if (request.IdProfesor.HasValue)
            query = query.Where(c => c.IdProfesorPrincipal == request.IdProfesor.Value);

        // Ejecutar query
        var clases = await query.ToListAsync(cancellationToken);

        // Calcular métricas
        var totalClases = clases.Count;
        
        var clasesConAsistencias = clases.Where(c => c.Asistencias.Any()).ToList();
        var promedioAsistencia = clasesConAsistencias.Any()
            ? (decimal)clasesConAsistencias.Average(c => c.Asistencias.Count(a => a.Estado.Nombre == "Presente"))
            : 0;

        var ocupacionPromedio = clasesConAsistencias.Any()
            ? (decimal)clasesConAsistencias.Average(c => 
                c.CupoMaximo > 0 ? ((decimal)c.Asistencias.Count(a => a.Estado.Nombre == "Presente") / c.CupoMaximo * 100) : 0)
            : 0;

        // Clases más populares (por tipo)
        var clasesPorTipo = clases
            .GroupBy(c => new { c.IdTipoClase, c.TipoClase.Nombre })
            .Select(g => new
            {
                g.Key.IdTipoClase,
                g.Key.Nombre,
                Clases = g.ToList()
            })
            .ToList();

        var clasesMasPopulares = clasesPorTipo
            .Select(g => new ClasePopularDTO
            {
                NombreTipoClase = g.Nombre,
                TotalClases = g.Clases.Count,
                PromedioAsistencia = g.Clases.Any(c => c.Asistencias.Any())
                    ? (decimal)g.Clases.Where(c => c.Asistencias.Any())
                        .Average(c => c.Asistencias.Count(a => a.Estado.Nombre == "Presente"))
                    : 0,
                OcupacionPorcentaje = g.Clases.Any(c => c.Asistencias.Any() && c.CupoMaximo > 0)
                    ? (decimal)g.Clases.Where(c => c.Asistencias.Any() && c.CupoMaximo > 0)
                        .Average(c => (decimal)c.Asistencias.Count(a => a.Estado.Nombre == "Presente") / c.CupoMaximo * 100)
                    : 0
            })
            .OrderByDescending(c => c.PromedioAsistencia)
            .Take(5)
            .ToList();

        // Gráfica de asistencia por día de la semana
        var asistenciasPorDia = clases
            .SelectMany(c => c.Asistencias.Where(a => a.Estado.Nombre == "Presente"))
            .GroupBy(a => a.Clase.Fecha.DayOfWeek)
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
                    BackgroundColor = "#8B5CF6",
                    BorderColor = "#7C3AED"
                }
            }
        };

        // Desglose por tipo
        var desglosePorTipo = clasesPorTipo
            .Select(g => new ClasesPorTipoDTO
            {
                NombreTipoClase = g.Nombre,
                CantidadClases = g.Clases.Count,
                PromedioAsistencia = g.Clases.Any(c => c.Asistencias.Any())
                    ? (decimal)g.Clases.Where(c => c.Asistencias.Any())
                        .Average(c => c.Asistencias.Count(a => a.Estado.Nombre == "Presente"))
                    : 0
            })
            .OrderByDescending(d => d.CantidadClases)
            .ToList();

        // Resultado
        var reporte = new ReporteClasesDTO
        {
            TotalClases = totalClases,
            PromedioAsistencia = Math.Round(promedioAsistencia, 2),
            OcupacionPromedio = Math.Round(ocupacionPromedio, 2),
            ClasesMasPopulares = clasesMasPopulares,
            GraficaAsistenciaPorDia = graficaAsistencias,
            DesgloseporTipo = desglosePorTipo
        };

        return Result<ReporteClasesDTO>.Success(reporte);
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

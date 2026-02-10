using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Reportes.Queries;

public class GetReportePaquetesHandler : IRequestHandler<GetReportePaquetesQuery, Result<ReportePaquetesDTO>>
{
    private readonly IAppDbContext _db;

    public GetReportePaquetesHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ReportePaquetesDTO>> Handle(GetReportePaquetesQuery request, CancellationToken cancellationToken)
    {
        // Validaciones
        if (request.FechaDesde > request.FechaHasta)
            return Result<ReportePaquetesDTO>.Failure("La fecha inicial no puede ser mayor a la fecha final.");

        if ((request.FechaHasta - request.FechaDesde).Days > 365)
            return Result<ReportePaquetesDTO>.Failure("El rango de fechas no puede ser mayor a 1 año.");

        // Base query
        var query = _db.Paquetes
            .Include(p => p.Alumno)
            .ThenInclude(a => a.Usuario)
            .Include(p => p.TipoPaquete)
            .Include(p => p.Estado)
            .Where(p => p.FechaActivacion >= request.FechaDesde && p.FechaActivacion <= request.FechaHasta)
            .AsQueryable();

        // Aplicar filtros opcionales
        if (!string.IsNullOrEmpty(request.Estado))
            query = query.Where(p => p.Estado.Nombre == request.Estado);

        if (request.IdTipoPaquete.HasValue)
            query = query.Where(p => p.IdTipoPaquete == request.IdTipoPaquete.Value);

        // Ejecutar query
        var paquetes = await query.ToListAsync(cancellationToken);

        // Calcular métricas por estado actual (no solo del periodo filtrado)
        var todosPaquetes = await _db.Paquetes
            .Include(p => p.TipoPaquete)
            .Include(p => p.Estado)
            .ToListAsync(cancellationToken);

        var totalActivos = todosPaquetes.Count(p => p.Estado.Nombre == "Activo");
        var totalVencidos = todosPaquetes.Count(p => p.Estado.Nombre == "Vencido");
        var totalCongelados = todosPaquetes.Count(p => p.Estado.Nombre == "Congelado");

        // Paquetes por vencer en los próximos 7 días
        var fechaLimite = DateTime.Today.AddDays(7);
        var paquetesPorVencer = todosPaquetes
            .Where(p => p.Estado.Nombre == "Activo" && p.FechaVencimiento <= fechaLimite)
            .Count();

        // Alertas de paquetes por vencer
        var paquetesPorVencerQuery = await _db.Paquetes
            .Include(p => p.Alumno)
            .ThenInclude(a => a.Usuario)
            .Include(p => p.TipoPaquete)
            .Include(p => p.Estado)
            .Where(p => p.Estado.Nombre == "Activo" && p.FechaVencimiento <= fechaLimite && p.FechaVencimiento >= DateTime.Today)
            .ToListAsync(cancellationToken);
        
        var alertas = paquetesPorVencerQuery
            .Select(p => new PaqueteAlertaDTO
            {
                IdPaquete = p.IdPaquete,
                NombreAlumno = p.Alumno.Usuario.NombreUsuario,
                CorreoAlumno = p.Alumno.Usuario.Correo,
                NombreTipoPaquete = p.TipoPaquete.Nombre,
                FechaVencimiento = p.FechaVencimiento,
                DiasRestantes = (int)(p.FechaVencimiento - DateTime.Today).TotalDays,
                ClasesRestantes = p.ClasesDisponibles - p.ClasesUsadas
            })
            .OrderBy(a => a.DiasRestantes)
            .ToList();

        // Desglose por estado
        var totalPaquetes = todosPaquetes.Count;
        var desgloseEstados = todosPaquetes
            .GroupBy(p => p.Estado.Nombre)
            .Select(g => new PaquetesPorEstadoDTO
            {
                Estado = g.Key,
                Cantidad = g.Count(),
                PorcentajeDelTotal = totalPaquetes > 0 ? (decimal)g.Count() / totalPaquetes * 100 : 0
            })
            .OrderByDescending(d => d.Cantidad)
            .ToList();

        // Gráfica de paquetes por tipo
        var paquetesPorTipo = paquetes
            .GroupBy(p => p.TipoPaquete.Nombre)
            .Select(g => new
            {
                Tipo = g.Key,
                Cantidad = g.Count()
            })
            .OrderByDescending(x => x.Cantidad)
            .ToList();

        var graficaPaquetes = new ChartDataDTO
        {
            Type = "pie",
            Labels = paquetesPorTipo.Select(x => x.Tipo).ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Paquetes",
                    Data = paquetesPorTipo.Select(x => (decimal)x.Cantidad).ToList(),
                    BackgroundColor = "#F59E0B"
                }
            }
        };

        // Resultado
        var reporte = new ReportePaquetesDTO
        {
            TotalActivos = totalActivos,
            TotalVencidos = totalVencidos,
            TotalPorVencer = paquetesPorVencer,
            TotalAgotados = totalCongelados, // Usando Congelados ya que Agotados no existe en la BD
            AlertasPorVencer = alertas,
            DesgloseEstados = desgloseEstados,
            GraficaPaquetesPorTipo = graficaPaquetes
        };

        return Result<ReportePaquetesDTO>.Success(reporte);
    }
}

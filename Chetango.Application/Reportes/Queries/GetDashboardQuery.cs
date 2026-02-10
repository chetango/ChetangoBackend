using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;

namespace Chetango.Application.Reportes.Queries;

/// <summary>
/// Query para obtener dashboard general con KPIs y gráficas
/// </summary>
public class GetDashboardQuery : IRequest<Result<DashboardDTO>>
{
    /// <summary>
    /// Fecha inicial del periodo (opcional)
    /// </summary>
    public DateTime? FechaDesde { get; set; }
    
    /// <summary>
    /// Fecha final del periodo (opcional)
    /// </summary>
    public DateTime? FechaHasta { get; set; }
    
    /// <summary>
    /// Periodo preset: "hoy", "semana", "mes", "30dias", "año" (opcional)
    /// </summary>
    public string? PeriodoPreset { get; set; }
}

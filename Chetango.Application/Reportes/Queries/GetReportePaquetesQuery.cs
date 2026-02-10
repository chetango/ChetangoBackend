using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;

namespace Chetango.Application.Reportes.Queries;

/// <summary>
/// Query para obtener reporte de paquetes con alertas
/// </summary>
public class GetReportePaquetesQuery : IRequest<Result<ReportePaquetesDTO>>
{
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public string? Estado { get; set; }
    public Guid? IdTipoPaquete { get; set; }
}

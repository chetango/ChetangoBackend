using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;

namespace Chetango.Application.Reportes.Queries;

/// <summary>
/// Query para obtener reporte de ingresos con comparativas
/// </summary>
public class GetReporteIngresosQuery : IRequest<Result<ReporteIngresosDTO>>
{
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public Guid? IdMetodoPago { get; set; }
    public bool ComparativaMesAnterior { get; set; }
}

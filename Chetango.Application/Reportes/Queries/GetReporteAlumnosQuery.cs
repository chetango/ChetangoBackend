using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;

namespace Chetango.Application.Reportes.Queries;

/// <summary>
/// Query para obtener reporte de alumnos con métricas de actividad
/// </summary>
public class GetReporteAlumnosQuery : IRequest<Result<ReporteAlumnosDTO>>
{
    public DateTime? FechaInscripcionDesde { get; set; }
    public DateTime? FechaInscripcionHasta { get; set; }
    public string? Estado { get; set; }
}

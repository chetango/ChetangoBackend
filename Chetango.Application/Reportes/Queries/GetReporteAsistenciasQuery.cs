using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;

namespace Chetango.Application.Reportes.Queries;

/// <summary>
/// Query para obtener reporte de asistencias con filtros y métricas
/// </summary>
public class GetReporteAsistenciasQuery : IRequest<Result<ReporteAsistenciasDTO>>
{
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public Guid? IdClase { get; set; }
    public Guid? IdAlumno { get; set; }
    public Guid? IdProfesor { get; set; }
    public string? EstadoAsistencia { get; set; }
    
    // Claims del usuario autenticado (se inyectan desde el endpoint)
    public string EmailUsuario { get; set; } = string.Empty;
    public bool EsAdmin { get; set; }
    public bool EsProfesor { get; set; }
}

using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;

namespace Chetango.Application.Reportes.Queries;

/// <summary>
/// Query para obtener reporte de clases con métricas
/// </summary>
public class GetReporteClasesQuery : IRequest<Result<ReporteClasesDTO>>
{
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public Guid? IdTipoClase { get; set; }
    public Guid? IdProfesor { get; set; }
    
    // Claims del usuario autenticado (se inyectan desde el endpoint)
    public string EmailUsuario { get; set; } = string.Empty;
    public bool EsAdmin { get; set; }
    public bool EsProfesor { get; set; }
}

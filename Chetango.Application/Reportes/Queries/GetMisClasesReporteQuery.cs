using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;

namespace Chetango.Application.Reportes.Queries;

/// <summary>
/// Query para obtener reporte de clases del profesor autenticado
/// </summary>
public class GetMisClasesReporteQuery : IRequest<Result<MisClasesReporteDTO>>
{
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    
    // El email se extrae del token JWT automáticamente
    public string EmailUsuario { get; set; } = string.Empty;
}

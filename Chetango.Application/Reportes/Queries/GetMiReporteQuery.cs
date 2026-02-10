using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;

namespace Chetango.Application.Reportes.Queries;

/// <summary>
/// Query para obtener reporte personal del alumno autenticado
/// </summary>
public class GetMiReporteQuery : IRequest<Result<MiReporteDTO>>
{
    // El email se extrae del token JWT automáticamente
    public string EmailUsuario { get; set; } = string.Empty;
}

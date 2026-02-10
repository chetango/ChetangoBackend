using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;

namespace Chetango.Application.Reportes.Queries;

public class GetDashboardProfesorQuery : IRequest<Result<DashboardProfesorDTO>>
{
    public string EmailUsuario { get; set; } = string.Empty;
}

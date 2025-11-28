using Chetango.Application.Asistencias.Admin.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Admin.Queries.GetResumenAsistenciasClaseAdmin;

public sealed record GetResumenAsistenciasClaseAdminQuery(Guid IdClase) : IRequest<Result<ResumenAsistenciasClaseAdminDto>>;

using Chetango.Application.Asistencias.Admin.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Admin.Queries.GetClasesDelDiaAdmin;

public sealed record GetClasesDelDiaAdminQuery(DateOnly Fecha) : IRequest<Result<ClasesDelDiaAdminDto>>;

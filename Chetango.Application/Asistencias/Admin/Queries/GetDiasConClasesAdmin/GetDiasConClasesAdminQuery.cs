using Chetango.Application.Common;
using Chetango.Application.Asistencias.Admin.DTOs;
using MediatR;

namespace Chetango.Application.Asistencias.Admin.Queries.GetDiasConClasesAdmin;

public sealed record GetDiasConClasesAdminQuery() : IRequest<Result<DiasConClasesAdminDto>>;

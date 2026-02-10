using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;

namespace Chetango.Application.Nomina.Queries;

public record GetResumenProfesorQuery(
    Guid? IdProfesor // null = todos los profesores
) : IRequest<Result<List<ResumenProfesorDTO>>>;

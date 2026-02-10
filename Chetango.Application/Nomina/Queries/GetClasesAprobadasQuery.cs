using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;

namespace Chetango.Application.Nomina.Queries;

public record GetClasesAprobadasQuery(
    Guid IdProfesor,
    int Mes,
    int Año
) : IRequest<Result<List<ClaseProfesorDTO>>>;

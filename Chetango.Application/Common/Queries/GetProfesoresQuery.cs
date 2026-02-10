using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Common.Queries;

public record GetProfesoresQuery : IRequest<Result<List<ProfesorDTO>>>;

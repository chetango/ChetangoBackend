using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Commands;

public record CompletarClaseCommand(Guid IdClase) : IRequest<Result<bool>>;

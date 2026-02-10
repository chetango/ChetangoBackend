using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Eventos.Commands;

public class DeleteEventoCommand : IRequest<Result<bool>>
{
    public Guid IdEvento { get; set; }
}

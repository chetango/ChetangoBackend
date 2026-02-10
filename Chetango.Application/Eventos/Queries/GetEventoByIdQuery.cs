using Chetango.Application.Common;
using Chetango.Application.Eventos.DTOs;
using MediatR;

namespace Chetango.Application.Eventos.Queries;

public class GetEventoByIdQuery : IRequest<Result<EventoDto>>
{
    public Guid IdEvento { get; set; }
}

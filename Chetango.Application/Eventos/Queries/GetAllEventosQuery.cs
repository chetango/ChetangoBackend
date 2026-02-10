using Chetango.Application.Common;
using Chetango.Application.Eventos.DTOs;
using MediatR;

namespace Chetango.Application.Eventos.Queries;

public class GetAllEventosQuery : IRequest<Result<List<EventoDto>>>
{
    public bool? SoloActivos { get; set; } = true;
    public bool? SoloFuturos { get; set; } = false;
}

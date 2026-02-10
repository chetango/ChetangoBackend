using Chetango.Application.Common;
using Chetango.Application.Eventos.DTOs;
using MediatR;

namespace Chetango.Application.Eventos.Commands;

public class UpdateEventoCommand : IRequest<Result<EventoDto>>
{
    public Guid IdEvento { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan? Hora { get; set; }
    public decimal? Precio { get; set; }
    public bool Destacado { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; }
}

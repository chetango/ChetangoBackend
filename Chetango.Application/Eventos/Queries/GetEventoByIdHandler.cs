using Chetango.Application.Common;
using Chetango.Application.Eventos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Eventos.Queries;

public class GetEventoByIdHandler : IRequestHandler<GetEventoByIdQuery, Result<EventoDto>>
{
    private readonly IAppDbContext _db;

    public GetEventoByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<EventoDto>> Handle(GetEventoByIdQuery request, CancellationToken cancellationToken)
    {
        var evento = await _db.Eventos
            .Where(e => e.IdEvento == request.IdEvento)
            .Select(e => new EventoDto
            {
                IdEvento = e.IdEvento,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                Fecha = e.Fecha,
                Hora = e.Hora,
                Precio = e.Precio,
                Destacado = e.Destacado,
                ImagenUrl = e.ImagenUrl,
                Activo = e.Activo,
                FechaCreacion = e.FechaCreacion
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (evento == null)
            return Result<EventoDto>.Failure("Evento no encontrado.");

        return Result<EventoDto>.Success(evento);
    }
}

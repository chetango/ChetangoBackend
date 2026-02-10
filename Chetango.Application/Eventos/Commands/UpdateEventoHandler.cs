using Chetango.Application.Common;
using Chetango.Application.Eventos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Eventos.Commands;

public class UpdateEventoHandler : IRequestHandler<UpdateEventoCommand, Result<EventoDto>>
{
    private readonly IAppDbContext _db;

    public UpdateEventoHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<EventoDto>> Handle(UpdateEventoCommand request, CancellationToken cancellationToken)
    {
        var evento = await _db.Eventos
            .FirstOrDefaultAsync(e => e.IdEvento == request.IdEvento, cancellationToken);

        if (evento == null)
            return Result<EventoDto>.Failure("Evento no encontrado.");

        // Actualizar propiedades
        evento.Titulo = request.Titulo;
        evento.Descripcion = request.Descripcion;
        evento.Fecha = request.Fecha;
        evento.Hora = request.Hora;
        evento.Precio = request.Precio;
        evento.Destacado = request.Destacado;
        evento.ImagenUrl = request.ImagenUrl;
        evento.Activo = request.Activo;
        evento.FechaModificacion = DateTimeHelper.Now;

        await _db.SaveChangesAsync(cancellationToken);

        // Mapear a DTO
        var dto = new EventoDto
        {
            IdEvento = evento.IdEvento,
            Titulo = evento.Titulo,
            Descripcion = evento.Descripcion,
            Fecha = evento.Fecha,
            Hora = evento.Hora,
            Precio = evento.Precio,
            Destacado = evento.Destacado,
            ImagenUrl = evento.ImagenUrl,
            Activo = evento.Activo,
            FechaCreacion = evento.FechaCreacion
        };

        return Result<EventoDto>.Success(dto);
    }
}

using Chetango.Application.Common;
using Chetango.Application.Eventos.DTOs;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Eventos.Commands;

public class CreateEventoHandler : IRequestHandler<CreateEventoCommand, Result<EventoDto>>
{
    private readonly IAppDbContext _db;

    public CreateEventoHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<EventoDto>> Handle(CreateEventoCommand request, CancellationToken cancellationToken)
    {
        // Validar que el usuario creador existe
        var usuarioExiste = await _db.Usuarios
            .AnyAsync(u => u.IdUsuario == request.IdUsuarioCreador, cancellationToken);

        if (!usuarioExiste)
            return Result<EventoDto>.Failure("Usuario creador no encontrado.");

        // Crear entidad
        var evento = new Evento
        {
            IdEvento = Guid.NewGuid(),
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            Fecha = request.Fecha,
            Hora = request.Hora,
            Precio = request.Precio,
            Destacado = request.Destacado,
            ImagenUrl = request.ImagenUrl,
            Activo = true,
            FechaCreacion = DateTimeHelper.Now,
            IdUsuarioCreador = request.IdUsuarioCreador
        };

        _db.Eventos.Add(evento);
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

using Chetango.Application.Common;
using Chetango.Application.Eventos.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Eventos.Queries;

public class GetAllEventosHandler : IRequestHandler<GetAllEventosQuery, Result<List<EventoDto>>>
{
    private readonly IAppDbContext _db;

    public GetAllEventosHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<EventoDto>>> Handle(GetAllEventosQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Eventos.AsQueryable();

        // Filtrar por activos si se especifica
        if (request.SoloActivos == true)
        {
            query = query.Where(e => e.Activo);
        }

        // Filtrar por eventos futuros si se especifica
        if (request.SoloFuturos == true)
        {
            var hoy = DateTime.Today;
            query = query.Where(e => e.Fecha >= hoy);
        }

        var eventos = await query
            .OrderBy(e => e.Fecha)
            .ThenBy(e => e.Hora)
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
            .ToListAsync(cancellationToken);

        return Result<List<EventoDto>>.Success(eventos);
    }
}

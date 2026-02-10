using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Reportes.Queries;

/// <summary>
/// Helper para queries de eventos compartidas entre dashboards
/// </summary>
public static class EventosQueryHelper
{
    /// <summary>
    /// Obtiene eventos próximos filtrados por tipo de audiencia
    /// </summary>
    /// <param name="db">Contexto de base de datos</param>
    /// <param name="tipoAudiencia">Tipo de audiencia ("Alumno", "Profesor")</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de EventoDTO</returns>
    public static async Task<List<EventoDTO>> ObtenerEventosProximos(
        IAppDbContext db,
        string tipoAudiencia,
        CancellationToken cancellationToken)
    {
        var hoy = DateTime.Today;
        const string imagenPorDefecto = "https://images.unsplash.com/photo-1504609813442-a8924e83f76e?w=400";

        return await db.Eventos
            .Where(e => e.Activo &&
                       e.Fecha >= hoy &&
                       (e.TipoAudiencia == tipoAudiencia || e.TipoAudiencia == "Todos"))
            .OrderBy(e => e.Fecha)
            .Take(3)
            .Select(e => new EventoDTO
            {
                IdEvento = e.IdEvento,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion ?? string.Empty,
                Fecha = e.Fecha,
                ImagenUrl = e.ImagenUrl ?? imagenPorDefecto,
                Precio = e.Precio,
                Destacado = e.Destacado
            })
            .ToListAsync(cancellationToken);
    }
}

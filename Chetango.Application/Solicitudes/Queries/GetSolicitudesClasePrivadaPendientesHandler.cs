using Chetango.Application.Common;
using Chetango.Application.Solicitudes.DTOs;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Solicitudes.Queries;

public class GetSolicitudesClasePrivadaPendientesHandler 
    : IRequestHandler<GetSolicitudesClasePrivadaPendientesQuery, Result<List<SolicitudClasePrivadaDTO>>>
{
    private readonly IAppDbContext _db;

    public GetSolicitudesClasePrivadaPendientesHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<SolicitudClasePrivadaDTO>>> Handle(
        GetSolicitudesClasePrivadaPendientesQuery request, 
        CancellationToken cancellationToken)
    {
        var solicitudes = await _db.Set<SolicitudClasePrivada>()
            .Include(s => s.Alumno)
                .ThenInclude(a => a.Usuario)
            .Where(s => s.Estado == "Pendiente")
            .OrderBy(s => s.FechaSolicitud)
            .Select(s => new SolicitudClasePrivadaDTO(
                s.IdSolicitud,
                s.IdAlumno,
                s.Alumno.Usuario.NombreUsuario,
                s.Alumno.Usuario.Correo,
                s.TipoClaseDeseado,
                s.FechaPreferida,
                s.HoraPreferida != null ? $"{s.HoraPreferida.Value.Hours:D2}:{s.HoraPreferida.Value.Minutes:D2}" : null,
                s.ObservacionesAlumno,
                s.Estado,
                s.FechaSolicitud,
                s.FechaRespuesta,
                s.MensajeRespuesta
            ))
            .ToListAsync(cancellationToken);

        return Result<List<SolicitudClasePrivadaDTO>>.Success(solicitudes);
    }
}

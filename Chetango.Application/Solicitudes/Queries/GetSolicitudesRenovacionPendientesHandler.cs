using Chetango.Application.Common;
using Chetango.Application.Solicitudes.DTOs;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Solicitudes.Queries;

public class GetSolicitudesRenovacionPendientesHandler 
    : IRequestHandler<GetSolicitudesRenovacionPendientesQuery, Result<List<SolicitudRenovacionPaqueteDTO>>>
{
    private readonly IAppDbContext _db;

    public GetSolicitudesRenovacionPendientesHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<SolicitudRenovacionPaqueteDTO>>> Handle(
        GetSolicitudesRenovacionPendientesQuery request, 
        CancellationToken cancellationToken)
    {
        var solicitudes = await _db.Set<SolicitudRenovacionPaquete>()
            .Include(s => s.Alumno)
                .ThenInclude(a => a.Usuario)
            .Where(s => s.Estado == "Pendiente")
            .OrderBy(s => s.FechaSolicitud)
            .Select(s => new SolicitudRenovacionPaqueteDTO(
                s.IdSolicitud,
                s.IdAlumno,
                s.Alumno.Usuario.NombreUsuario,
                s.Alumno.Usuario.Correo,
                s.IdPaqueteActual,
                null, // TipoPaqueteActual - se puede agregar después con join manual si es necesario
                null, // ClasesRestantes - se puede agregar después con join manual si es necesario
                s.TipoPaqueteDeseado,
                s.MensajeAlumno,
                s.Estado,
                s.FechaSolicitud,
                s.FechaRespuesta,
                s.MensajeRespuesta
            ))
            .ToListAsync(cancellationToken);

        return Result<List<SolicitudRenovacionPaqueteDTO>>.Success(solicitudes);
    }
}

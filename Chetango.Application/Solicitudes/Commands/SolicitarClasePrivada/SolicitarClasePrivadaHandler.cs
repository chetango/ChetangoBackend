using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Solicitudes.Commands.SolicitarClasePrivada;

public class SolicitarClasePrivadaHandler : IRequestHandler<SolicitarClasePrivadaCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public SolicitarClasePrivadaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(SolicitarClasePrivadaCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar alumno por email
        var alumno = await _db.Alumnos
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Usuario.Correo == request.EmailAlumno, cancellationToken);

        if (alumno == null)
            return Result<Guid>.Failure("No se encontró el alumno autenticado.");

        // 2. Obtener nombre del tipo de clase deseado
        string tipoClaseDeseado = "Clase Privada";
        
        if (request.IdTipoClaseDeseado.HasValue)
        {
            var tipoClase = await _db.Set<TipoClase>()
                .FirstOrDefaultAsync(tc => tc.Id == request.IdTipoClaseDeseado.Value, cancellationToken);
            
            if (tipoClase != null)
                tipoClaseDeseado = tipoClase.Nombre;
        }

        // 3. Validar que no exista una solicitud pendiente reciente (últimos 7 días)
        var hace7Dias = DateTime.Now.AddDays(-7);
        var solicitudReciente = await _db.Set<SolicitudClasePrivada>()
            .AnyAsync(s => s.IdAlumno == alumno.IdAlumno && 
                          s.Estado == "Pendiente" && 
                          s.FechaSolicitud >= hace7Dias, cancellationToken);

        if (solicitudReciente)
            return Result<Guid>.Failure("Ya tienes una solicitud de clase privada pendiente.");

        // 4. Crear solicitud
        var solicitud = new SolicitudClasePrivada
        {
            IdSolicitud = Guid.NewGuid(),
            IdAlumno = alumno.IdAlumno,
            IdTipoClaseDeseado = request.IdTipoClaseDeseado,
            TipoClaseDeseado = tipoClaseDeseado,
            FechaPreferida = request.FechaPreferida,
            HoraPreferida = request.HoraPreferida,
            ObservacionesAlumno = request.ObservacionesAlumno,
            Estado = "Pendiente",
            FechaSolicitud = DateTime.Now
        };

        _db.Set<SolicitudClasePrivada>().Add(solicitud);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(solicitud.IdSolicitud);
    }
}

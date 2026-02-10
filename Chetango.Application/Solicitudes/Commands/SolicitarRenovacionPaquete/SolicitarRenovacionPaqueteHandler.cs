using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Solicitudes.Commands.SolicitarRenovacionPaquete;

public class SolicitarRenovacionPaqueteHandler : IRequestHandler<SolicitarRenovacionPaqueteCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public SolicitarRenovacionPaqueteHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(SolicitarRenovacionPaqueteCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar alumno por email
        var alumno = await _db.Alumnos
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Usuario.Correo == request.EmailAlumno, cancellationToken);

        if (alumno == null)
            return Result<Guid>.Failure("No se encontró el alumno autenticado.");

        // 2. Buscar paquete activo actual
        var paqueteActivo = await _db.Paquetes
            .Include(p => p.TipoPaquete)
            .Include(p => p.Estado)
            .Where(p => p.IdAlumno == alumno.IdAlumno && p.Estado.Nombre == "Activo")
            .OrderByDescending(p => p.FechaActivacion)
            .FirstOrDefaultAsync(cancellationToken);

        // 3. Obtener nombre del tipo de paquete deseado
        string tipoPaqueteDeseado = "Mismo paquete actual";
        
        if (request.IdTipoPaqueteDeseado.HasValue)
        {
            var tipoPaquete = await _db.Set<TipoPaquete>()
                .FirstOrDefaultAsync(tp => tp.Id == request.IdTipoPaqueteDeseado.Value, cancellationToken);
            
            if (tipoPaquete != null)
                tipoPaqueteDeseado = tipoPaquete.Nombre;
        }
        else if (paqueteActivo != null)
        {
            tipoPaqueteDeseado = paqueteActivo.TipoPaquete.Nombre;
        }

        // 4. Validar que no exista una solicitud pendiente
        var solicitudPendiente = await _db.Set<SolicitudRenovacionPaquete>()
            .AnyAsync(s => s.IdAlumno == alumno.IdAlumno && s.Estado == "Pendiente", cancellationToken);

        if (solicitudPendiente)
            return Result<Guid>.Failure("Ya tienes una solicitud de renovación pendiente.");

        // 5. Crear solicitud
        var solicitud = new SolicitudRenovacionPaquete
        {
            IdSolicitud = Guid.NewGuid(),
            IdAlumno = alumno.IdAlumno,
            IdPaqueteActual = paqueteActivo?.IdPaquete,
            IdTipoPaqueteDeseado = request.IdTipoPaqueteDeseado ?? paqueteActivo?.IdTipoPaquete,
            TipoPaqueteDeseado = tipoPaqueteDeseado,
            MensajeAlumno = request.MensajeAlumno,
            Estado = "Pendiente",
            FechaSolicitud = DateTime.Now
        };

        _db.Set<SolicitudRenovacionPaquete>().Add(solicitud);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(solicitud.IdSolicitud);
    }
}

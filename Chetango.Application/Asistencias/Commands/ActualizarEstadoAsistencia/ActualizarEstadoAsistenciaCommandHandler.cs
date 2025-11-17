using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Commands.ActualizarEstadoAsistencia;

public class ActualizarEstadoAsistenciaCommandHandler : IRequestHandler<ActualizarEstadoAsistenciaCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public ActualizarEstadoAsistenciaCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(ActualizarEstadoAsistenciaCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar la asistencia con navegación al paquete y clase
        var asistencia = await _db.Asistencias
            .Include(a => a.PaqueteUsado)
            .Include(a => a.Clase)
            .FirstOrDefaultAsync(a => a.IdAsistencia == request.IdAsistencia, cancellationToken);

        if (asistencia is null)
            return Result<Unit>.Failure("La asistencia especificada no existe.");

        // 2. Validar que la clase no sea muy antigua (máx 7 días atrás)
        var diasTranscurridos = (DateTime.Today - asistencia.Clase.Fecha).Days;
        if (diasTranscurridos > 7)
            return Result<Unit>.Failure("No se puede modificar asistencias de clases con más de 7 días de antigüedad.");

        // 3. Si cambia de Presente a otro estado, devolver clase al paquete
        var estadoAnterior = asistencia.IdEstado;
        var nuevoEstado = request.NuevoEstado;

        if (estadoAnterior == 1 && nuevoEstado != 1) // Era Presente, ya no lo es
        {
            if (asistencia.PaqueteUsado.ClasesUsadas > 0)
                asistencia.PaqueteUsado.ClasesUsadas--;
        }
        else if (estadoAnterior != 1 && nuevoEstado == 1) // No era Presente, ahora sí
        {
            if (asistencia.PaqueteUsado.ClasesUsadas >= asistencia.PaqueteUsado.ClasesDisponibles)
                return Result<Unit>.Failure("El paquete no tiene clases disponibles para marcar como Presente.");

            asistencia.PaqueteUsado.ClasesUsadas++;
        }

        // 4. Actualizar estado y observación
        asistencia.IdEstado = nuevoEstado;
        if (request.Observacion is not null)
            asistencia.Observacion = request.Observacion;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

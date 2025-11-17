using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Commands.RegistrarAsistencia;

public class RegistrarAsistenciaCommandHandler : IRequestHandler<RegistrarAsistenciaCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public RegistrarAsistenciaCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(RegistrarAsistenciaCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que la clase existe y no es futura
        var clase = await _db.Set<Chetango.Domain.Entities.Clase>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase is null)
            return Result<Guid>.Failure("La clase especificada no existe.");

        if (clase.Fecha > DateTime.Today)
            return Result<Guid>.Failure("No se puede registrar asistencia a una clase futura.");

        // 2. Validar que el alumno existe
        var alumnoExiste = await _db.Set<Chetango.Domain.Entities.Alumno>()
            .AsNoTracking()
            .AnyAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (!alumnoExiste)
            return Result<Guid>.Failure("El alumno especificado no existe.");

        // 3. Validar que el paquete pertenece al alumno y está activo
        var paquete = await _db.Set<Paquete>()
            .FirstOrDefaultAsync(p => p.IdPaquete == request.IdPaquete && p.IdAlumno == request.IdAlumno, cancellationToken);

        if (paquete is null)
            return Result<Guid>.Failure("El paquete especificado no existe o no pertenece al alumno.");

        // Estados: 1=Activo, 2=Vencido, 3=Congelado, 4=Agotado
        if (paquete.IdEstado != 1)
            return Result<Guid>.Failure($"El paquete no está activo (estado actual: {paquete.IdEstado}).");

        if (paquete.FechaVencimiento < DateTime.Today)
            return Result<Guid>.Failure("El paquete está vencido.");

        // 4. Validar que no existe asistencia duplicada (índice único en BD, pero validamos aquí también)
        var existeAsistencia = await _db.Asistencias
            .AsNoTracking()
            .AnyAsync(a => a.IdClase == request.IdClase && a.IdAlumno == request.IdAlumno, cancellationToken);

        if (existeAsistencia)
            return Result<Guid>.Failure("Ya existe un registro de asistencia para este alumno en esta clase.");

        // 5. Si el estado es Presente (1), validar que haya clases disponibles
        if (request.IdEstado == 1) // Presente
        {
            if (paquete.ClasesUsadas >= paquete.ClasesDisponibles)
                return Result<Guid>.Failure("El paquete no tiene clases disponibles.");

            // Incrementar clases usadas
            paquete.ClasesUsadas++;
        }

        // 6. Crear el registro de asistencia
        var asistencia = new Asistencia
        {
            IdAsistencia = Guid.NewGuid(),
            IdClase = request.IdClase,
            IdAlumno = request.IdAlumno,
            IdPaqueteUsado = request.IdPaquete,
            IdEstado = request.IdEstado,
            Observacion = request.Observacion
        };

        _db.Asistencias.Add(asistencia);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(asistencia.IdAsistencia);
    }
}

using Chetango.Application.Common;
using Chetango.Application.Paquetes.Commands.DescontarClase;
using Chetango.Application.Paquetes.Queries.ValidarPaqueteDisponible;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Commands.RegistrarAsistencia;

public class RegistrarAsistenciaCommandHandler : IRequestHandler<RegistrarAsistenciaCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly IMediator _mediator;

    public RegistrarAsistenciaCommandHandler(IAppDbContext db, IMediator mediator)
    {
        _db = db;
        _mediator = mediator;
    }

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

        // 3. Validar que el paquete pertenece al alumno
        var paquete = await _db.Set<Paquete>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPaquete == request.IdPaqueteUsado && p.IdAlumno == request.IdAlumno, cancellationToken);

        if (paquete is null)
            return Result<Guid>.Failure("El paquete especificado no existe o no pertenece al alumno.");

        // 4. Validar que no existe asistencia duplicada (índice único en BD, pero validamos aquí también)
        var existeAsistencia = await _db.Asistencias
            .AsNoTracking()
            .AnyAsync(a => a.IdClase == request.IdClase && a.IdAlumno == request.IdAlumno, cancellationToken);

        if (existeAsistencia)
            return Result<Guid>.Failure("Ya existe un registro de asistencia para este alumno en esta clase.");

        // 5. Si el estado es Presente (1), validar y descontar clase del paquete
        if (request.IdEstadoAsistencia == 1) // Presente
        {
            // Validar que el paquete está disponible
            var validarPaqueteResult = await _mediator.Send(
                new ValidarPaqueteDisponibleQuery(request.IdPaqueteUsado),
                cancellationToken
            );

            if (!validarPaqueteResult.Succeeded)
                return Result<Guid>.Failure(validarPaqueteResult.Error!);

            // Descontar la clase del paquete
            var descontarResult = await _mediator.Send(
                new DescontarClaseCommand(request.IdPaqueteUsado),
                cancellationToken
            );

            if (!descontarResult.Succeeded)
                return Result<Guid>.Failure($"No se pudo descontar la clase del paquete: {descontarResult.Error}");
        }

        // 6. Crear el registro de asistencia
        var asistencia = new Asistencia
        {
            IdAsistencia = Guid.NewGuid(),
            IdClase = request.IdClase,
            IdAlumno = request.IdAlumno,
            IdPaqueteUsado = request.IdPaqueteUsado,
            IdEstado = request.IdEstadoAsistencia,
            Observacion = request.Observaciones
        };

        _db.Asistencias.Add(asistencia);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(asistencia.IdAsistencia);
    }
}

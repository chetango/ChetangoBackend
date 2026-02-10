using Chetango.Application.Common;
using Chetango.Application.Paquetes.Commands.DescontarClase;
using Chetango.Application.Paquetes.Queries.ValidarPaqueteDisponible;
using Chetango.Domain.Entities;
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

        // 3. Validar que el tipo de asistencia existe
        var tipoAsistencia = await _db.Set<TipoAsistencia>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdTipoAsistencia == request.IdTipoAsistencia && t.Activo, cancellationToken);

        if (tipoAsistencia is null)
            return Result<Guid>.Failure("El tipo de asistencia especificado no existe o no está activo.");

        // 4. Validar reglas de negocio según tipo de asistencia
        if (tipoAsistencia.RequierePaquete && !request.IdPaqueteUsado.HasValue)
            return Result<Guid>.Failure($"El tipo de asistencia '{tipoAsistencia.Nombre}' requiere un paquete activo.");

        if (!tipoAsistencia.RequierePaquete && request.IdPaqueteUsado.HasValue)
            return Result<Guid>.Failure($"El tipo de asistencia '{tipoAsistencia.Nombre}' no debe incluir un paquete.");

        // 5. Validar que el paquete pertenece al alumno (solo si se proporcionó)
        Paquete? paquete = null;
        List<Guid>? alumnosDelPaqueteCompartido = null;
        
        if (request.IdPaqueteUsado.HasValue)
        {
            paquete = await _db.Set<Paquete>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPaquete == request.IdPaqueteUsado.Value && p.IdAlumno == request.IdAlumno, cancellationToken);

            if (paquete is null)
                return Result<Guid>.Failure("El paquete especificado no existe o no pertenece al alumno.");

            // Verificar si es un paquete compartido (mismo IdPago con múltiples alumnos)
            if (paquete.IdPago.HasValue)
            {
                var paquetesCompartidos = await _db.Set<Paquete>()
                    .AsNoTracking()
                    .Where(p => p.IdPago == paquete.IdPago && p.IdPaquete != paquete.IdPaquete)
                    .Select(p => p.IdAlumno)
                    .ToListAsync(cancellationToken);

                if (paquetesCompartidos.Any())
                {
                    alumnosDelPaqueteCompartido = paquetesCompartidos;
                }
            }
        }

        // 6. Validar que no existe asistencia duplicada (índice único en BD, pero validamos aquí también)
        var existeAsistencia = await _db.Asistencias
            .AsNoTracking()
            .AnyAsync(a => a.IdClase == request.IdClase && a.IdAlumno == request.IdAlumno, cancellationToken);

        if (existeAsistencia)
            return Result<Guid>.Failure("Ya existe un registro de asistencia para este alumno en esta clase.");

        // 7. Preparar observación enriquecida
        string? observacionFinal = request.Observaciones;
        
        // Si es cortesía/prueba sin observación personalizada, agregar nota automática
        if (!tipoAsistencia.RequierePaquete && string.IsNullOrWhiteSpace(request.Observaciones))
        {
            observacionFinal = $"{tipoAsistencia.Nombre} - Sin descuento de paquete";
        }
        else if (!tipoAsistencia.RequierePaquete && !string.IsNullOrWhiteSpace(request.Observaciones))
        {
            observacionFinal = $"{request.Observaciones} [{tipoAsistencia.Nombre}]";
        }

        // 8. Si el estado es Presente Y el tipo requiere descontar, validar y descontar
        if (request.IdEstadoAsistencia == 1 && tipoAsistencia.DescontarClase && request.IdPaqueteUsado.HasValue)
        {
            // Validar que el paquete está disponible
            var validarPaqueteResult = await _mediator.Send(
                new ValidarPaqueteDisponibleQuery(request.IdPaqueteUsado.Value),
                cancellationToken
            );

            if (!validarPaqueteResult.Succeeded)
                return Result<Guid>.Failure(validarPaqueteResult.Error!);

            // Descontar la clase del paquete
            var descontarResult = await _mediator.Send(
                new DescontarClaseCommand(request.IdPaqueteUsado.Value),
                cancellationToken
            );

            if (!descontarResult.Succeeded)
                return Result<Guid>.Failure($"No se pudo descontar la clase del paquete: {descontarResult.Error}");
        }

        // 9. Crear el registro de asistencia
        var asistencia = new Asistencia
        {
            IdAsistencia = Guid.NewGuid(),
            IdClase = request.IdClase,
            IdAlumno = request.IdAlumno,
            IdTipoAsistencia = request.IdTipoAsistencia,
            IdPaqueteUsado = request.IdPaqueteUsado,
            IdEstado = request.IdEstadoAsistencia,
            Observacion = observacionFinal
        };

        _db.Asistencias.Add(asistencia);

        // 10. Si es un paquete compartido, crear asistencias para los demás alumnos automáticamente
        if (alumnosDelPaqueteCompartido != null && alumnosDelPaqueteCompartido.Any())
        {
            // Obtener los paquetes de los demás alumnos en el mismo pago
            var paquetesOtrosAlumnos = await _db.Set<Paquete>()
                .AsNoTracking()
                .Where(p => p.IdPago == paquete!.IdPago && p.IdPaquete != paquete.IdPaquete)
                .ToListAsync(cancellationToken);

            foreach (var paqueteOtro in paquetesOtrosAlumnos)
            {
                // Verificar que no exista asistencia duplicada
                var existeOtraAsistencia = await _db.Asistencias
                    .AsNoTracking()
                    .AnyAsync(a => a.IdClase == request.IdClase && a.IdAlumno == paqueteOtro.IdAlumno, cancellationToken);

                if (!existeOtraAsistencia)
                {
                    // Si el estado es Presente y requiere descontar, descontar del paquete del otro alumno
                    if (request.IdEstadoAsistencia == 1 && tipoAsistencia.DescontarClase)
                    {
                        var validarOtroPaquete = await _mediator.Send(
                            new ValidarPaqueteDisponibleQuery(paqueteOtro.IdPaquete),
                            cancellationToken
                        );

                        if (validarOtroPaquete.Succeeded)
                        {
                            await _mediator.Send(
                                new DescontarClaseCommand(paqueteOtro.IdPaquete),
                                cancellationToken
                            );
                        }
                    }

                    var asistenciaOtro = new Asistencia
                    {
                        IdAsistencia = Guid.NewGuid(),
                        IdClase = request.IdClase,
                        IdAlumno = paqueteOtro.IdAlumno,
                        IdTipoAsistencia = request.IdTipoAsistencia,
                        IdPaqueteUsado = paqueteOtro.IdPaquete,
                        IdEstado = request.IdEstadoAsistencia,
                        Observacion = observacionFinal + " [Paquete compartido]"
                    };

                    _db.Asistencias.Add(asistenciaOtro);
                }
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(asistencia.IdAsistencia);
    }
}

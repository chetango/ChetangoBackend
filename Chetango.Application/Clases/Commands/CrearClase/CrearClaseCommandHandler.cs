using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Commands.CrearClase;

public class CrearClaseCommandHandler : IRequestHandler<CrearClaseCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public CrearClaseCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CrearClaseCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que la fecha es futura
        var fechaHoraInicio = request.Fecha.Date.Add(request.HoraInicio);
        if (fechaHoraInicio <= DateTime.Now)
            return Result<Guid>.Failure("La clase debe programarse para una fecha y hora futura.");

        // 2. Validar que HoraFin es posterior a HoraInicio
        if (request.HoraFin <= request.HoraInicio)
            return Result<Guid>.Failure("La hora de fin debe ser posterior a la hora de inicio.");

        // 3. Validar que el tipo de clase existe
        var tipoClaseExiste = await _db.Set<TipoClase>()
            .AsNoTracking()
            .AnyAsync(tc => tc.Id == request.IdTipoClase, cancellationToken);

        if (!tipoClaseExiste)
            return Result<Guid>.Failure("El tipo de clase especificado no existe.");

        // 4. Validar que el profesor existe y está activo
        var profesor = await _db.Set<Profesor>()
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.IdProfesor == request.IdProfesorPrincipal, cancellationToken);

        if (profesor is null)
            return Result<Guid>.Failure("El profesor especificado no existe.");

        // 5. Validación de ownership: Profesor solo puede crear clases para sí mismo
        if (!request.EsAdmin)
        {
            if (profesor.IdUsuario.ToString() != request.IdUsuarioActual)
                return Result<Guid>.Failure("No tienes permiso para crear clases para otro profesor.");
        }

        // 6. Validar que no hay conflicto de horario para el profesor
        var tieneConflicto = await _db.Set<Chetango.Domain.Entities.Clase>()
            .Where(c => c.IdProfesorPrincipal == request.IdProfesorPrincipal 
                     && c.Fecha == request.Fecha.Date)
            .AnyAsync(c => 
                // Conflicto si los horarios se solapan
                (request.HoraInicio >= c.HoraInicio && request.HoraInicio < c.HoraFin) || // Inicia durante otra clase
                (request.HoraFin > c.HoraInicio && request.HoraFin <= c.HoraFin) || // Termina durante otra clase
                (request.HoraInicio <= c.HoraInicio && request.HoraFin >= c.HoraFin), // Envuelve otra clase
                cancellationToken);

        if (tieneConflicto)
            return Result<Guid>.Failure("El profesor ya tiene una clase programada en ese horario.");

        // 7. Crear la clase
        var clase = new Chetango.Domain.Entities.Clase
        {
            IdClase = Guid.NewGuid(),
            IdProfesorPrincipal = request.IdProfesorPrincipal,
            IdTipoClase = request.IdTipoClase,
            Fecha = request.Fecha.Date,
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin,
            CupoMaximo = request.CupoMaximo,
            Observaciones = request.Observaciones
        };

        _db.Set<Chetango.Domain.Entities.Clase>().Add(clase);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(clase.IdClase);
    }
}

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Commands.EditarClase;

public class EditarClaseCommandHandler : IRequestHandler<EditarClaseCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public EditarClaseCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(EditarClaseCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que la clase existe
        var clase = await _db.Set<Chetango.Domain.Entities.Clase>()
            .Include(c => c.ProfesorPrincipal)
            .ThenInclude(p => p.Usuario)
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase is null)
            return Result<bool>.Failure("La clase especificada no existe.");

        // 2. Validación de ownership: Profesor solo puede editar sus clases
        if (!request.EsAdmin)
        {
            if (clase.ProfesorPrincipal.IdUsuario.ToString() != request.IdUsuarioActual)
                return Result<bool>.Failure("No tienes permiso para editar esta clase.");
        }

        // 3. Validar fecha según estado de la clase
        var ahora = DateTime.Now;
        var fechaHoraOriginal = new DateTime(clase.Fecha.Year, clase.Fecha.Month, clase.Fecha.Day)
            .Add(clase.HoraInicio);
        var claseYaPaso = fechaHoraOriginal <= ahora;

        // Si la clase ya pasó o es hoy, no permitir cambiar fecha/hora
        if (claseYaPaso)
        {
            // Validar que NO se está intentando cambiar la fecha/hora
            if (fechaHoraOriginal != request.FechaHoraInicio)
            {
                return Result<bool>.Failure(
                    "No se puede cambiar la fecha/hora de una clase que ya pasó o está en curso. " +
                    "Puedes modificar profesores, alumnos, cupo u observaciones."
                );
            }
            // Si no cambia fecha/hora, permitir editar otros datos (profesores, alumnos, etc.)
        }
        else
        {
            // Clase futura: validar que la nueva fecha también sea futura
            if (request.FechaHoraInicio <= ahora)
            {
                return Result<bool>.Failure("La clase debe programarse para una fecha y hora futura.");
            }
        }

        // 4. Validar duración mínima
        if (request.DuracionMinutos < 30)
            return Result<bool>.Failure("La duración mínima de una clase es 30 minutos.");

        // 5. Validar cupo máximo
        if (request.CupoMaximo < 1)
            return Result<bool>.Failure("El cupo máximo debe ser al menos 1.");

        // 6. Validar que el tipo de clase existe
        var tipoClaseExiste = await _db.Set<TipoClase>()
            .AsNoTracking()
            .AnyAsync(tc => tc.Id == request.IdTipoClase, cancellationToken);

        if (!tipoClaseExiste)
            return Result<bool>.Failure("El tipo de clase especificado no existe.");

        // 7. Validar que el profesor existe
        var profesor = await _db.Set<Profesor>()
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.IdProfesor == request.IdProfesor, cancellationToken);

        if (profesor is null)
            return Result<bool>.Failure("El profesor especificado no existe.");

        // 8. Validar que no hay conflicto de horario (excluyendo esta misma clase)
        var fechaHoraFin = request.FechaHoraInicio.AddMinutes(request.DuracionMinutos);
        var horaInicioTimeSpan = request.FechaHoraInicio.TimeOfDay;
        var horaFinTimeSpan = fechaHoraFin.TimeOfDay;
        
        // Buscar si el profesor tiene alguna clase en ese horario (en cualquier rol), excluyendo la clase actual
        var tieneConflicto = await _db.Set<ClaseProfesor>()
            .Include(cp => cp.Clase)
            .Where(cp => cp.IdProfesor == request.IdProfesor
                     && cp.IdClase != request.IdClase // Excluir la clase actual
                     && cp.Clase.Fecha == request.FechaHoraInicio.Date)
            .AnyAsync(cp => 
                (horaInicioTimeSpan >= cp.Clase.HoraInicio && horaInicioTimeSpan < cp.Clase.HoraFin) ||
                (horaFinTimeSpan > cp.Clase.HoraInicio && horaFinTimeSpan <= cp.Clase.HoraFin) ||
                (horaInicioTimeSpan <= cp.Clase.HoraInicio && horaFinTimeSpan >= cp.Clase.HoraFin),
                cancellationToken);

        if (tieneConflicto)
            return Result<bool>.Failure("El profesor ya tiene una clase programada en ese horario.");

        // 9. Actualizar la clase
        clase.IdTipoClase = request.IdTipoClase;
        clase.IdProfesorPrincipal = request.IdProfesor;
        clase.Fecha = request.FechaHoraInicio.Date;
        clase.HoraInicio = horaInicioTimeSpan;
        clase.HoraFin = horaFinTimeSpan;
        clase.CupoMaximo = request.CupoMaximo;
        clase.Observaciones = request.Observaciones;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

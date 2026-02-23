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
            .Include(c => c.Profesores)
            .ThenInclude(cp => cp.Profesor)
            .ThenInclude(p => p.Usuario)
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase is null)
            return Result<bool>.Failure("La clase especificada no existe.");

        // 1b. Normalizar y validar lista de profesores
        var profesoresRequest = request.Profesores ?? new List<ProfesorClaseRequest>();
        
        if (!profesoresRequest.Any())
            return Result<bool>.Failure("Debe especificar al menos un profesor para la clase.");

        var profesoresPrincipales = profesoresRequest.Where(p => p.RolEnClase == "Principal").ToList();
        if (profesoresPrincipales.Count == 0)
            return Result<bool>.Failure("Debe haber al menos un profesor con rol Principal.");

        // Validar que no hay profesores duplicados
        var profesoresUnicos = profesoresRequest.Select(p => p.IdProfesor).Distinct().ToList();
        if (profesoresUnicos.Count != profesoresRequest.Count)
            return Result<bool>.Failure("No se pueden asignar profesores duplicados a la misma clase.");

        // 2. Validación de ownership: Profesor solo puede editar clases donde él sea uno de los principales
        if (!request.EsAdmin)
        {
            var idsProfesoresPrincipales = profesoresPrincipales.Select(pp => pp.IdProfesor).ToList();
            var profesoresActuales = clase.Profesores.Where(cp => idsProfesoresPrincipales.Contains(cp.IdProfesor)).ToList();
            var esUnoDeLosrincipales = profesoresActuales.Any(cp => cp.Profesor.IdUsuario.ToString() == request.IdUsuarioActual);
            
            if (!esUnoDeLosrincipales)
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

        // 7. Validar que todos los profesores existen y están activos
        var profesores = await _db.Set<Profesor>()
            .Include(p => p.Usuario)
            .Where(p => profesoresUnicos.Contains(p.IdProfesor))
            .ToListAsync(cancellationToken);

        if (profesores.Count != profesoresUnicos.Count)
            return Result<bool>.Failure("Uno o más profesores especificados no existen.");

        // 8. Validar que los roles existen
        var rolesNombres = profesoresRequest.Select(p => p.RolEnClase).Distinct().ToList();
        var roles = await _db.Set<RolEnClase>()
            .Where(r => rolesNombres.Contains(r.Nombre))
            .ToListAsync(cancellationToken);

        if (roles.Count != rolesNombres.Count)
            return Result<bool>.Failure("Uno o más roles especificados no son válidos.");

        // 9. Validar que ningún profesor tiene conflicto de horario (excluyendo esta clase)
        var fechaHoraFin = request.FechaHoraInicio.AddMinutes(request.DuracionMinutos);
        var horaInicioTimeSpan = request.FechaHoraInicio.TimeOfDay;
        var horaFinTimeSpan = fechaHoraFin.TimeOfDay;
        
        foreach (var idProfesor in profesoresUnicos)
        {
            var tieneConflicto = await _db.Set<ClaseProfesor>()
                .Include(cp => cp.Clase)
                .Where(cp => cp.IdProfesor == idProfesor
                         && cp.IdClase != request.IdClase // Excluir la clase actual
                         && cp.Clase.Fecha == request.FechaHoraInicio.Date)
                .AnyAsync(cp => 
                    (horaInicioTimeSpan >= cp.Clase.HoraInicio && horaInicioTimeSpan < cp.Clase.HoraFin) ||
                    (horaFinTimeSpan > cp.Clase.HoraInicio && horaFinTimeSpan <= cp.Clase.HoraFin) ||
                    (horaInicioTimeSpan <= cp.Clase.HoraInicio && horaFinTimeSpan >= cp.Clase.HoraFin),
                    cancellationToken);

            if (tieneConflicto)
            {
                var profesor = profesores.First(p => p.IdProfesor == idProfesor);
                return Result<bool>.Failure($"El profesor {profesor.NombreCompleto} ya tiene una clase programada en ese horario.");
            }
        }

        // 10. Actualizar datos básicos de la clase
        clase.IdTipoClase = request.IdTipoClase;
        clase.Fecha = request.FechaHoraInicio.Date;
        clase.HoraInicio = horaInicioTimeSpan;
        clase.HoraFin = horaFinTimeSpan;
        clase.CupoMaximo = request.CupoMaximo;
        clase.Observaciones = request.Observaciones;
        clase.IdProfesorPrincipal = profesoresPrincipales[0].IdProfesor; // Retrocompatibilidad

        // 11. Actualizar profesores: eliminar los actuales y agregar los nuevos
        var profesoresActualesIds = clase.Profesores.Select(cp => cp.IdClaseProfesor).ToList();
        var profesoresAEliminar = await _db.Set<ClaseProfesor>()
            .Where(cp => profesoresActualesIds.Contains(cp.IdClaseProfesor))
            .ToListAsync(cancellationToken);
        
        _db.Set<ClaseProfesor>().RemoveRange(profesoresAEliminar);

        // 12. Agregar nuevos profesores con sus roles y tarifas
        foreach (var profesorReq in profesoresRequest)
        {
            var profesor = profesores.First(p => p.IdProfesor == profesorReq.IdProfesor);
            var rol = roles.First(r => r.Nombre == profesorReq.RolEnClase);

            var claseProfesor = new ClaseProfesor
            {
                IdClaseProfesor = Guid.NewGuid(),
                IdClase = clase.IdClase,
                IdProfesor = profesorReq.IdProfesor,
                IdRolEnClase = rol.Id,
                TarifaProgramada = profesor.TarifaActual,
                ValorAdicional = 0,
                TotalPago = profesor.TarifaActual,
                EstadoPago = "Pendiente",
                FechaCreacion = DateTimeHelper.Now
            };

            _db.Set<ClaseProfesor>().Add(claseProfesor);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

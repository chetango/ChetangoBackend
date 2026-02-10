using Chetango.Application.Common;
using Chetango.Domain.Entities;
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
        // RETROCOMPATIBILIDAD: Convertir formato antiguo al nuevo si es necesario
        var profesoresRequest = request.Profesores ?? new List<ProfesorClaseRequest>();
        
        if (profesoresRequest.Count == 0 && request.IdProfesorPrincipal.HasValue)
        {
            // Modo retrocompatibilidad: convertir del formato antiguo
            profesoresRequest.Add(new ProfesorClaseRequest(request.IdProfesorPrincipal.Value, "Principal"));
            
            if (request.IdsMonitores != null && request.IdsMonitores.Any())
            {
                foreach (var idMonitor in request.IdsMonitores)
                {
                    profesoresRequest.Add(new ProfesorClaseRequest(idMonitor, "Monitor"));
                }
            }
        }

        // 1. Validar que se especificó al menos un profesor
        if (!profesoresRequest.Any())
            return Result<Guid>.Failure("Debe especificar al menos un profesor para la clase.");

        // 2. Validar que hay al menos un profesor principal
        var profesoresPrincipales = profesoresRequest.Where(p => p.RolEnClase == "Principal").ToList();
        if (profesoresPrincipales.Count == 0)
            return Result<Guid>.Failure("Debe especificar al menos un profesor principal para la clase.");

        // 3. Validar que no hay profesores duplicados
        var profesoresUnicos = profesoresRequest.Select(p => p.IdProfesor).Distinct().ToList();
        if (profesoresUnicos.Count != profesoresRequest.Count)
            return Result<Guid>.Failure("No se pueden asignar profesores duplicados a la misma clase.");

        // 4. Validar que la fecha es futura
        var fechaHoraInicio = request.Fecha.Date.Add(request.HoraInicio);
        if (fechaHoraInicio <= DateTimeHelper.Now)
            return Result<Guid>.Failure("La clase debe programarse para una fecha y hora futura.");

        // 5. Validar que HoraFin es posterior a HoraInicio
        if (request.HoraFin <= request.HoraInicio)
            return Result<Guid>.Failure("La hora de fin debe ser posterior a la hora de inicio.");

        // 6. Validar que el tipo de clase existe
        var tipoClaseExiste = await _db.Set<TipoClase>()
            .AsNoTracking()
            .AnyAsync(tc => tc.Id == request.IdTipoClase, cancellationToken);

        if (!tipoClaseExiste)
            return Result<Guid>.Failure("El tipo de clase especificado no existe.");

        // 7. Validar que todos los profesores existen y están activos
        var profesores = await _db.Set<Profesor>()
            .Include(p => p.Usuario)
            .Include(p => p.TipoProfesor)
            .Where(p => profesoresUnicos.Contains(p.IdProfesor))
            .ToListAsync(cancellationToken);

        if (profesores.Count != profesoresUnicos.Count)
            return Result<Guid>.Failure("Uno o más profesores especificados no existen.");

        // 8. Validar que los roles existen
        var rolesNombres = profesoresRequest.Select(p => p.RolEnClase).Distinct().ToList();
        var roles = await _db.Set<RolEnClase>()
            .Where(r => rolesNombres.Contains(r.Nombre))
            .ToListAsync(cancellationToken);

        if (roles.Count != rolesNombres.Count)
            return Result<Guid>.Failure("Uno o más roles especificados no son válidos.");

        // 9. Validación de ownership: Profesor solo puede crear clases donde él sea uno de los principales
        if (!request.EsAdmin)
        {
            var idsProfesoresPrincipales = profesoresPrincipales.Select(pp => pp.IdProfesor).ToList();
            var esUnoDeLosrincipales = profesores
                .Where(p => idsProfesoresPrincipales.Contains(p.IdProfesor))
                .Any(p => p.IdUsuario.ToString() == request.IdUsuarioActual);
            
            if (!esUnoDeLosrincipales)
                return Result<Guid>.Failure("No tienes permiso para crear clases donde no seas uno de los profesores principales.");
        }

        // 10. Validar que ningún profesor tiene conflicto de horario
        foreach (var idProfesor in profesoresUnicos)
        {
            // Buscar si el profesor tiene alguna clase en ese horario (en cualquier rol)
            var tieneConflicto = await _db.Set<ClaseProfesor>()
                .Include(cp => cp.Clase)
                .Where(cp => cp.IdProfesor == idProfesor && cp.Clase.Fecha == request.Fecha.Date)
                .AnyAsync(cp => 
                    (request.HoraInicio >= cp.Clase.HoraInicio && request.HoraInicio < cp.Clase.HoraFin) ||
                    (request.HoraFin > cp.Clase.HoraInicio && request.HoraFin <= cp.Clase.HoraFin) ||
                    (request.HoraInicio <= cp.Clase.HoraInicio && request.HoraFin >= cp.Clase.HoraFin),
                    cancellationToken);

            if (tieneConflicto)
            {
                var profesor = profesores.First(p => p.IdProfesor == idProfesor);
                return Result<Guid>.Failure($"El profesor {profesor.NombreCompleto} ya tiene una clase programada en ese horario.");
            }
        }

        // 11. Crear la clase (con IdProfesorPrincipal nullable para retrocompatibilidad)
        var clase = new Chetango.Domain.Entities.Clase
        {
            IdClase = Guid.NewGuid(),
            IdProfesorPrincipal = profesoresPrincipales[0].IdProfesor, // Temporal para retrocompatibilidad
            IdTipoClase = request.IdTipoClase,
            Fecha = request.Fecha.Date,
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin,
            CupoMaximo = request.CupoMaximo,
            Observaciones = request.Observaciones
        };

        _db.Set<Chetango.Domain.Entities.Clase>().Add(clase);

        // 12. Agregar profesores con sus roles y tarifas programadas
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
                TarifaProgramada = profesor.TarifaActual, // Usar tarifa individual del profesor
                ValorAdicional = 0,
                TotalPago = profesor.TarifaActual,
                EstadoPago = "Pendiente",
                FechaCreacion = DateTimeHelper.Now
            };

            _db.Set<ClaseProfesor>().Add(claseProfesor);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(clase.IdClase);
    }
}

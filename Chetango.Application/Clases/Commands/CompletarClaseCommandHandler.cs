using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Commands;

public class CompletarClaseCommandHandler : IRequestHandler<CompletarClaseCommand, Result<bool>>
{
    private readonly IAppDbContext _context;

    public CompletarClaseCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CompletarClaseCommand request, CancellationToken cancellationToken)
    {
        var clase = await _context.Clases
            .Include(c => c.ProfesorPrincipal)
                .ThenInclude(p => p.TipoProfesor)
            .Include(c => c.Monitores)
            .Include(c => c.Profesores) // Cargar profesores con nuevo sistema
                .ThenInclude(cp => cp.Profesor)
            .Include(c => c.TipoClase)
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase == null)
            return Result<bool>.Failure("Clase no encontrada");

        if (clase.Estado == "Completada")
            return Result<bool>.Failure("La clase ya está completada");

        if (clase.Estado == "Cancelada")
            return Result<bool>.Failure("No se puede completar una clase cancelada");

        // Cambiar estado de la clase
        clase.Estado = "Completada";

        // Calcular duración en horas
        var duracion = (clase.HoraFin - clase.HoraInicio).TotalHours;

        // NUEVO SISTEMA: Si la clase ya tiene ClaseProfesor, solo actualizar tarifas
        if (clase.Profesores.Any())
        {
            // Los profesores ya están registrados (nuevo sistema)
            // Solo necesitamos actualizar TotalPago según duración si es necesario
            foreach (var claseProfesor in clase.Profesores)
            {
                // Verificar si ya tiene pago calculado
                if (claseProfesor.TotalPago == 0 || claseProfesor.TotalPago == claseProfesor.TarifaProgramada)
                {
                    // Recalcular basado en duración
                    claseProfesor.TotalPago = (claseProfesor.TarifaProgramada + claseProfesor.ValorAdicional) * (decimal)duracion;
                }
                // Si TotalPago ya estaba calculado, respetarlo (podría ser ajuste manual)
            }
        }
        else
        {
            // SISTEMA ANTIGUO: Migración en tiempo real
            // Generar ClaseProfesor basado en IdProfesorPrincipal y Monitores

            var rolPrincipal = await _context.RolesEnClase
                .FirstOrDefaultAsync(r => r.Nombre == "Principal", cancellationToken);

            if (rolPrincipal == null)
                return Result<bool>.Failure("No se encontró el rol Principal");

            // Generar pago para el profesor principal
            if (clase.IdProfesorPrincipal.HasValue)
            {
                var profesorPrincipal = await _context.Profesores
                    .Include(p => p.TipoProfesor)
                    .FirstOrDefaultAsync(p => p.IdProfesor == clase.IdProfesorPrincipal.Value, cancellationToken);

                if (profesorPrincipal != null)
                {
                    // USAR NUEVO SISTEMA: TarifaActual del profesor
                    decimal tarifaProfesor = profesorPrincipal.TarifaActual;

                    // Fallback al sistema antiguo si no tiene tarifa configurada
                    if (tarifaProfesor == 0)
                    {
                        var valorTarifaAntigua = await _context.Set<TarifaProfesor>()
                            .Where(t => t.IdTipoProfesor == profesorPrincipal.IdTipoProfesor && 
                                       t.IdRolEnClase == rolPrincipal.Id)
                            .Select(t => t.ValorPorClase)
                            .FirstOrDefaultAsync(cancellationToken);

                        if (valorTarifaAntigua == 0)
                            return Result<bool>.Failure($"El profesor {profesorPrincipal.NombreCompleto} no tiene tarifa configurada");

                        tarifaProfesor = valorTarifaAntigua;
                    }

                    var pagoProfesor = new ClaseProfesor
                    {
                        IdClaseProfesor = Guid.NewGuid(),
                        IdClase = clase.IdClase,
                        IdProfesor = profesorPrincipal.IdProfesor,
                        IdRolEnClase = rolPrincipal.Id,
                        TarifaProgramada = tarifaProfesor,
                        ValorAdicional = 0,
                        TotalPago = tarifaProfesor * (decimal)duracion,
                        EstadoPago = "Pendiente",
                        FechaCreacion = DateTimeHelper.Now
                    };

                    _context.ClasesProfesores.Add(pagoProfesor);
                }
            }

            // Obtener el rol de monitor
            var rolMonitor = await _context.RolesEnClase
                .FirstOrDefaultAsync(r => r.Nombre == "Monitor", cancellationToken);

            if (rolMonitor != null && clase.Monitores.Any())
            {
                var idsMonitores = clase.Monitores.Select(m => m.IdProfesor).ToList();
                var profesoresMonitores = await _context.Profesores
                    .Include(p => p.TipoProfesor)
                    .Where(p => idsMonitores.Contains(p.IdProfesor))
                    .ToListAsync(cancellationToken);
                
                foreach (var monitorClase in clase.Monitores)
                {
                    var profesorMonitor = profesoresMonitores.FirstOrDefault(p => p.IdProfesor == monitorClase.IdProfesor);
                    if (profesorMonitor == null) continue;
                    
                    // USAR NUEVO SISTEMA: TarifaActual del profesor
                    decimal tarifaMonitor = profesorMonitor.TarifaActual;

                    // Fallback al sistema antiguo si no tiene tarifa configurada
                    if (tarifaMonitor == 0)
                    {
                        var valorTarifaAntigua = await _context.Set<TarifaProfesor>()
                            .Where(t => t.IdTipoProfesor == profesorMonitor.IdTipoProfesor && 
                                       t.IdRolEnClase == rolMonitor.Id)
                            .Select(t => t.ValorPorClase)
                            .FirstOrDefaultAsync(cancellationToken);

                        if (valorTarifaAntigua > 0)
                            tarifaMonitor = valorTarifaAntigua;
                    }

                    if (tarifaMonitor > 0)
                    {
                        var pagoMonitor = new ClaseProfesor
                        {
                            IdClaseProfesor = Guid.NewGuid(),
                            IdClase = clase.IdClase,
                            IdProfesor = monitorClase.IdProfesor,
                            IdRolEnClase = rolMonitor.Id,
                            TarifaProgramada = tarifaMonitor,
                            ValorAdicional = 0,
                            TotalPago = tarifaMonitor * (decimal)duracion,
                            EstadoPago = "Pendiente",
                            FechaCreacion = DateTimeHelper.Now
                        };

                        _context.ClasesProfesores.Add(pagoMonitor);
                    }
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

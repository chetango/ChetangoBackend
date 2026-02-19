using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Nomina.Commands;

public record LiquidarMesCommand(
    Guid IdProfesor,
    int Mes,
    int Año,
    Guid CreadoPorIdUsuario,
    string? Observaciones,
    Sede? Sede = null // Opcional: si null, se hereda del usuario logueado
) : IRequest<Result<Guid>>;

public class LiquidarMesHandler : IRequestHandler<LiquidarMesCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public LiquidarMesHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(LiquidarMesCommand request, CancellationToken cancellationToken)
    {
        // Obtener sede del usuario creador si no se especificó
        var sedeAUsar = request.Sede;
        if (!sedeAUsar.HasValue)
        {
            var usuarioCreador = await _db.Set<Usuario>()
                .FirstOrDefaultAsync(u => u.IdUsuario == request.CreadoPorIdUsuario, cancellationToken);
            sedeAUsar = usuarioCreador?.Sede ?? Sede.Medellin;
        }

        // Obtener todas las clases aprobadas del mes
        var clasesAprobadas = await _db.Set<ClaseProfesor>()
            .Include(cp => cp.Clase)
            .Where(cp => cp.IdProfesor == request.IdProfesor
                && cp.EstadoPago == "Aprobado"
                && cp.Clase.Fecha.Month == request.Mes
                && cp.Clase.Fecha.Year == request.Año)
            .ToListAsync(cancellationToken);

        if (!clasesAprobadas.Any())
            return Result<Guid>.Failure("No hay clases aprobadas para liquidar en este periodo");

        // Verificar si existe liquidación NO pagada para este mes
        var liquidacionExistente = await _db.Set<LiquidacionMensual>()
            .FirstOrDefaultAsync(l => l.IdProfesor == request.IdProfesor 
                && l.Mes == request.Mes 
                && l.Año == request.Año
                && l.Estado != "Pagada", cancellationToken);

        Guid idLiquidacionRetornar;

        if (liquidacionExistente != null)
        {
            // LIQUIDACIÓN INCREMENTAL: Actualizar liquidación existente en estado "Cerrada"
            // Calcular totales de las nuevas clases
            var nuevasClases = clasesAprobadas.Count;
            var nuevasHoras = clasesAprobadas.Sum(c => (decimal)(c.Clase.HoraFin - c.Clase.HoraInicio).TotalHours);
            var nuevoTotalBase = clasesAprobadas.Sum(c => c.TarifaProgramada);
            var nuevoTotalAdicionales = clasesAprobadas.Sum(c => c.ValorAdicional);
            var nuevoTotalPagar = nuevoTotalBase + nuevoTotalAdicionales;

            // Actualizar la liquidación existente (incremental)
            liquidacionExistente.TotalClases += nuevasClases;
            liquidacionExistente.TotalHoras += nuevasHoras;
            liquidacionExistente.TotalBase += nuevoTotalBase;
            liquidacionExistente.TotalAdicionales += nuevoTotalAdicionales;
            liquidacionExistente.TotalPagar += nuevoTotalPagar;
            liquidacionExistente.FechaCierre = DateTime.Now; // Actualizar fecha de cierre
            
            // Agregar nota a observaciones
            var notaIncremental = $"\n[{DateTime.Now:dd/MM/yyyy HH:mm}] Agregadas {nuevasClases} clase(s) adicional(es). Total acumulado: {liquidacionExistente.TotalClases} clases.";
            liquidacionExistente.Observaciones = string.IsNullOrEmpty(liquidacionExistente.Observaciones) 
                ? notaIncremental.Trim() 
                : liquidacionExistente.Observaciones + notaIncremental;

            idLiquidacionRetornar = liquidacionExistente.IdLiquidacion;
        }
        else
        {
            // NUEVA LIQUIDACIÓN: Crear desde cero
            var totalClases = clasesAprobadas.Count;
            var totalHoras = clasesAprobadas.Sum(c => (decimal)(c.Clase.HoraFin - c.Clase.HoraInicio).TotalHours);
            var totalBase = clasesAprobadas.Sum(c => c.TarifaProgramada);
            var totalAdicionales = clasesAprobadas.Sum(c => c.ValorAdicional);
            var totalPagar = totalBase + totalAdicionales;

            var nuevaLiquidacion = new LiquidacionMensual
            {
                IdLiquidacion = Guid.NewGuid(),
                IdProfesor = request.IdProfesor,
                Mes = request.Mes,
                Año = request.Año,
                Sede = sedeAUsar.Value,
                TotalClases = totalClases,
                TotalHoras = totalHoras,
                TotalBase = totalBase,
                TotalAdicionales = totalAdicionales,
                TotalPagar = totalPagar,
                Estado = "Cerrada",
                FechaCierre = DateTime.Now,
                Observaciones = request.Observaciones,
                FechaCreacion = DateTime.Now,
                CreadoPorIdUsuario = request.CreadoPorIdUsuario
            };

            _db.Set<LiquidacionMensual>().Add(nuevaLiquidacion);
            idLiquidacionRetornar = nuevaLiquidacion.IdLiquidacion;
        }

        // Actualizar estado de las clases a "Liquidado"
        foreach (var clase in clasesAprobadas)
        {
            clase.EstadoPago = "Liquidado";
            clase.FechaModificacion = DateTime.Now;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(idLiquidacionRetornar);
    }
}

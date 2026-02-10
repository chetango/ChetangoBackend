using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Nomina.Commands;

public record EliminarLiquidacionCommand(Guid IdLiquidacion) : IRequest<Result<bool>>;

public class EliminarLiquidacionHandler : IRequestHandler<EliminarLiquidacionCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public EliminarLiquidacionHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(EliminarLiquidacionCommand request, CancellationToken cancellationToken)
    {
        var liquidacion = await _db.Set<LiquidacionMensual>()
            .FirstOrDefaultAsync(l => l.IdLiquidacion == request.IdLiquidacion, cancellationToken);

        if (liquidacion == null)
            return Result<bool>.Failure("Liquidación no encontrada");

        // VALIDACIÓN: No permitir eliminar liquidaciones ya pagadas
        if (liquidacion.Estado == "Pagada")
            return Result<bool>.Failure("No se puede eliminar una liquidación que ya fue pagada. Contacte al administrador del sistema.");

        // Revertir estado de todas las clases de "Liquidado" a "Aprobado"
        var clasesLiquidadas = await _db.Set<ClaseProfesor>()
            .Include(cp => cp.Clase)
            .Where(cp => cp.IdProfesor == liquidacion.IdProfesor
                && cp.EstadoPago == "Liquidado"
                && cp.Clase.Fecha.Month == liquidacion.Mes
                && cp.Clase.Fecha.Year == liquidacion.Año)
            .ToListAsync(cancellationToken);

        foreach (var clase in clasesLiquidadas)
        {
            clase.EstadoPago = "Aprobado";
            clase.FechaModificacion = DateTime.Now;
        }

        // Eliminar la liquidación
        _db.Set<LiquidacionMensual>().Remove(liquidacion);

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

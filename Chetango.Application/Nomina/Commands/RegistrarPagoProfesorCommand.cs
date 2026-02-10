using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Nomina.Commands;

public record RegistrarPagoProfesorCommand(
    Guid IdLiquidacion,
    DateTime FechaPago,
    string? Observaciones
) : IRequest<Result<bool>>;

public class RegistrarPagoProfesorHandler : IRequestHandler<RegistrarPagoProfesorCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public RegistrarPagoProfesorHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(RegistrarPagoProfesorCommand request, CancellationToken cancellationToken)
    {
        var liquidacion = await _db.Set<LiquidacionMensual>()
            .FirstOrDefaultAsync(l => l.IdLiquidacion == request.IdLiquidacion, cancellationToken);

        if (liquidacion == null)
            return Result<bool>.Failure("Liquidación no encontrada");

        if (liquidacion.Estado != "Cerrada")
            return Result<bool>.Failure($"La liquidación no está cerrada (Estado actual: {liquidacion.Estado})");

        // Actualizar liquidación
        liquidacion.Estado = "Pagada";
        liquidacion.FechaPago = request.FechaPago;
        if (!string.IsNullOrEmpty(request.Observaciones))
            liquidacion.Observaciones = $"{liquidacion.Observaciones}\n{request.Observaciones}".Trim();

        // Actualizar clases a "Pagado"
        var clasesPagadas = await _db.Set<ClaseProfesor>()
            .Include(cp => cp.Clase)
            .Where(cp => cp.IdProfesor == liquidacion.IdProfesor
                && cp.EstadoPago == "Liquidado"
                && cp.Clase.Fecha.Month == liquidacion.Mes
                && cp.Clase.Fecha.Year == liquidacion.Año)
            .ToListAsync(cancellationToken);

        foreach (var clase in clasesPagadas)
        {
            clase.EstadoPago = "Pagado";
            clase.FechaPago = request.FechaPago;
            clase.FechaModificacion = DateTime.Now;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

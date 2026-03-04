using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;

namespace Chetango.Application.Suscripciones.Commands;

/// <summary>
/// Handler para aprobar un pago de suscripción y extender la suscripción del tenant.
/// </summary>
public class AprobarPagoSuscripcionCommandHandler : IRequestHandler<AprobarPagoSuscripcionCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public AprobarPagoSuscripcionCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(AprobarPagoSuscripcionCommand request, CancellationToken cancellationToken)
    {
        // Buscar el pago con su tenant
        var pago = await _db.PagosSuscripcion
            .Include(p => p.Tenant)
            .FirstOrDefaultAsync(p => p.Id == request.PagoId, cancellationToken);

        if (pago == null)
        {
            return Result<bool>.Failure("Pago no encontrado.");
        }

        if (pago.Estado != "Pendiente")
        {
            return Result<bool>.Failure("Este pago ya fue procesado.");
        }

        // Aprobar pago
        pago.Estado = "Aprobado";
        pago.AprobadoPor = request.AprobadoPor;
        pago.FechaAprobacion = DateTime.UtcNow;
        pago.Observaciones = request.Observaciones;
        pago.FechaModificacion = DateTime.UtcNow;
        pago.ModificadoPor = request.AprobadoPor;

        // Extender suscripción (sumar 1 mes)
        if (pago.Tenant.FechaVencimientoPlan < DateTime.Today)
        {
            // Si está vencida, empezar desde hoy
            pago.Tenant.FechaVencimientoPlan = DateTime.Today.AddMonths(1);
        }
        else
        {
            // Si está vigente, extender desde fecha actual de vencimiento
            pago.Tenant.FechaVencimientoPlan = pago.Tenant.FechaVencimientoPlan?.AddMonths(1);
        }

        // Reactivar si estaba suspendido
        if (pago.Tenant.Estado == "Suspendido")
        {
            pago.Tenant.Estado = "Activo";
        }

        pago.Tenant.FechaActualizacion = DateTime.UtcNow;
        pago.Tenant.ActualizadoPor = request.AprobadoPor;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

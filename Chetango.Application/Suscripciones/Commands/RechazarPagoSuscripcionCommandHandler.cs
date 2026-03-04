using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;

namespace Chetango.Application.Suscripciones.Commands;

/// <summary>
/// Handler para rechazar un pago de suscripción.
/// </summary>
public class RechazarPagoSuscripcionCommandHandler : IRequestHandler<RechazarPagoSuscripcionCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public RechazarPagoSuscripcionCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(RechazarPagoSuscripcionCommand request, CancellationToken cancellationToken)
    {
        // Buscar el pago
        var pago = await _db.PagosSuscripcion
            .FirstOrDefaultAsync(p => p.Id == request.PagoId, cancellationToken);

        if (pago == null)
        {
            return Result<bool>.Failure("Pago no encontrado.");
        }

        if (pago.Estado != "Pendiente")
        {
            return Result<bool>.Failure("Este pago ya fue procesado.");
        }

        // Validar que se proporcione un motivo
        if (string.IsNullOrWhiteSpace(request.MotivoRechazo))
        {
            return Result<bool>.Failure("Debe proporcionar un motivo de rechazo.");
        }

        // Rechazar pago
        pago.Estado = "Rechazado";
        pago.AprobadoPor = request.RechazadoPor; // Reutilizamos el campo
        pago.FechaAprobacion = DateTime.UtcNow; // Reutilizamos el campo
        pago.Observaciones = request.MotivoRechazo;
        pago.FechaModificacion = DateTime.UtcNow;
        pago.ModificadoPor = request.RechazadoPor;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

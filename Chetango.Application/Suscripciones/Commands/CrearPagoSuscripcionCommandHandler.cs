using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Domain.Entities;

namespace Chetango.Application.Suscripciones.Commands;

/// <summary>
/// Handler para crear un pago de suscripción (registrar comprobante).
/// </summary>
public class CrearPagoSuscripcionCommandHandler : IRequestHandler<CrearPagoSuscripcionCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public CrearPagoSuscripcionCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(CrearPagoSuscripcionCommand request, CancellationToken cancellationToken)
    {
        // Validaciones
        if (request.Monto <= 0)
        {
            return Result<Guid>.Failure("El monto debe ser mayor a cero.");
        }

        if (string.IsNullOrWhiteSpace(request.Referencia))
        {
            return Result<Guid>.Failure("La referencia es obligatoria.");
        }

        // Verificar que el tenant existe
        var tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant == null)
        {
            return Result<Guid>.Failure("Academia no encontrada.");
        }

        // Verificar que no exista ya un pago con la misma referencia
        var existeReferencia = await _db.PagosSuscripcion
            .AnyAsync(p => p.Referencia == request.Referencia, cancellationToken);

        if (existeReferencia)
        {
            return Result<Guid>.Failure("Ya existe un pago con esta referencia.");
        }

        // Crear pago de suscripción
        var pago = new PagoSuscripcion
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            FechaPago = request.FechaPago,
            Monto = request.Monto,
            Referencia = request.Referencia,
            MetodoPago = request.MetodoPago,
            ComprobanteUrl = request.ComprobanteUrl,
            NombreArchivo = request.NombreArchivo,
            TamanoArchivo = request.TamanoArchivo,
            Estado = "Pendiente",
            FechaCreacion = DateTime.UtcNow,
            CreadoPor = request.EmailUsuarioCreador
        };

        _db.PagosSuscripcion.Add(pago);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(pago.Id);
    }
}

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Pagos.Commands;

public record VerificarPagoCommand(
    Guid IdPago,
    bool Aprobar, // true = aprobar, false = rechazar
    string? NotasVerificacion,
    bool EnviarNotificacionWhatsApp,
    string UsuarioVerificacion
) : IRequest<Result<bool>>;

public class VerificarPagoHandler : IRequestHandler<VerificarPagoCommand, Result<bool>>
{
    private readonly IAppDbContext _db;
    private readonly IWhatsAppService _whatsAppService;

    public VerificarPagoHandler(IAppDbContext db, IWhatsAppService whatsAppService)
    {
        _db = db;
        _whatsAppService = whatsAppService;
    }

    public async Task<Result<bool>> Handle(VerificarPagoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var pago = await _db.Set<Chetango.Domain.Entities.Pago>()
                .Include(p => p.Alumno)
                    .ThenInclude(a => a.Usuario)
                .Include(p => p.EstadoPago)
                .Include(p => p.Paquetes)
                    .ThenInclude(paq => paq.TipoPaquete)
                .FirstOrDefaultAsync(p => p.IdPago == request.IdPago, cancellationToken);

            if (pago == null)
                return Result<bool>.Failure("Pago no encontrado");

            if (pago.Alumno == null || pago.Alumno.Usuario == null)
                return Result<bool>.Failure("Información del alumno incompleta");

            // Obtener el estado correspondiente
            var nuevoEstado = request.Aprobar ? "Verificado" : "Rechazado";
            var estadoPago = await _db.Set<Chetango.Domain.Entities.Estados.EstadoPago>()
                .FirstOrDefaultAsync(e => e.Nombre == nuevoEstado, cancellationToken);

            if (estadoPago == null)
                return Result<bool>.Failure($"Estado '{nuevoEstado}' no encontrado en el sistema");

            // Actualizar el pago
            pago.IdEstadoPago = estadoPago.Id;
            pago.NotasVerificacion = request.NotasVerificacion;
            pago.FechaVerificacion = DateTime.Now;
            pago.UsuarioVerificacion = request.UsuarioVerificacion;
            pago.FechaModificacion = DateTime.Now;
            pago.UsuarioModificacion = request.UsuarioVerificacion;

            await _db.SaveChangesAsync(cancellationToken);

            // Enviar notificación por WhatsApp si está habilitado
            if (request.EnviarNotificacionWhatsApp && !string.IsNullOrEmpty(pago.Alumno.Usuario.Telefono))
            {
                try
                {
                    var paquetesNombres = pago.Paquetes
                        .Where(p => p.TipoPaquete != null)
                        .Select(p => $"{p.TipoPaquete!.Nombre} ({p.ClasesDisponibles} clases)")
                        .ToList();

                    if (request.Aprobar)
                    {
                        await _whatsAppService.EnviarNotificacionPagoAprobadoAsync(
                            pago.Alumno.Usuario.Telefono,
                            pago.Alumno.Usuario.NombreUsuario,
                            pago.MontoTotal,
                            pago.ReferenciaTransferencia ?? "N/A",
                            pago.FechaPago,
                            paquetesNombres
                        );
                    }
                    else
                    {
                        await _whatsAppService.EnviarNotificacionPagoRechazadoAsync(
                            pago.Alumno.Usuario.Telefono,
                            pago.Alumno.Usuario.NombreUsuario,
                            pago.MontoTotal,
                            request.NotasVerificacion ?? "Revisar comprobante"
                        );
                    }
                }
                catch (Exception ex)
                {
                    // Log error pero no fallar la verificación del pago
                    Console.WriteLine($"Error enviando notificación WhatsApp: {ex.Message}");
                }
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al verificar pago: {ex.Message}");
        }
    }
}



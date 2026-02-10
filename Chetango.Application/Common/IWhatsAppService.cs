namespace Chetango.Application.Common;

public interface IWhatsAppService
{
    /// <summary>
    /// Envía notificación de pago aprobado por WhatsApp
    /// </summary>
    Task<bool> EnviarNotificacionPagoAprobadoAsync(
        string numeroWhatsApp,
        string nombreAlumno,
        decimal monto,
        string referencia,
        DateTime fechaPago,
        List<string> paquetes);

    /// <summary>
    /// Envía notificación de pago rechazado por WhatsApp
    /// </summary>
    Task<bool> EnviarNotificacionPagoRechazadoAsync(
        string numeroWhatsApp,
        string nombreAlumno,
        decimal monto,
        string motivo);
}

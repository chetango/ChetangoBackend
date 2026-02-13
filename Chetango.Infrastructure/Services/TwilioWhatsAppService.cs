using Chetango.Application.Common;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Chetango.Infrastructure.Services;

public class TwilioWhatsAppService : IWhatsAppService
{
    private readonly string? _accountSid;
    private readonly string? _authToken;
    private readonly string? _fromWhatsApp;
    private readonly bool _isConfigured;

    public TwilioWhatsAppService(IConfiguration configuration)
    {
        _accountSid = configuration["Twilio:AccountSid"];
        _authToken = configuration["Twilio:AuthToken"];
        _fromWhatsApp = configuration["Twilio:WhatsAppFrom"];

        _isConfigured = !string.IsNullOrEmpty(_accountSid) 
                        && !string.IsNullOrEmpty(_authToken) 
                        && !string.IsNullOrEmpty(_fromWhatsApp);

        if (_isConfigured)
        {
            TwilioClient.Init(_accountSid!, _authToken!);
        }
    }

    public async Task<bool> EnviarNotificacionPagoAprobadoAsync(
        string numeroWhatsApp,
        string nombreAlumno,
        decimal monto,
        string referencia,
        DateTime fechaPago,
        List<string> paquetes)
    {
        if (!_isConfigured)
        {
            Console.WriteLine("WhatsApp no configurado - saltando notificación de pago aprobado");
            return false;
        }

        try
        {
            // Formatear el número para WhatsApp
            var toWhatsApp = FormatearNumeroWhatsApp(numeroWhatsApp);

            // Construir mensaje
            var mensaje = ConstruirMensajeAprobado(nombreAlumno, monto, referencia, fechaPago, paquetes);

            // Enviar mensaje
            var message = await MessageResource.CreateAsync(
                to: new PhoneNumber(toWhatsApp),
                from: new PhoneNumber(_fromWhatsApp),
                body: mensaje
            );

            return message.ErrorCode == null;
        }
        catch (Exception ex)
        {
            // Log error (implementar logging según necesidad)
            Console.WriteLine($"Error enviando WhatsApp: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EnviarNotificacionPagoRechazadoAsync(
        string numeroWhatsApp,
        string nombreAlumno,
        decimal monto,
        string motivo)
    {
        if (!_isConfigured)
        {
            Console.WriteLine("WhatsApp no configurado - saltando notificación de pago rechazado");
            return false;
        }

        try
        {
            var toWhatsApp = FormatearNumeroWhatsApp(numeroWhatsApp);
            var mensaje = ConstruirMensajeRechazado(nombreAlumno, monto, motivo);

            var message = await MessageResource.CreateAsync(
                to: new PhoneNumber(toWhatsApp),
                from: new PhoneNumber(_fromWhatsApp),
                body: mensaje
            );

            return message.ErrorCode == null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando WhatsApp: {ex.Message}");
            return false;
        }
    }

    private string FormatearNumeroWhatsApp(string numero)
    {
        // Si ya tiene el prefijo whatsapp:, devolverlo tal cual
        if (numero.StartsWith("whatsapp:"))
            return numero;

        // Limpiar el número (quitar espacios, guiones, etc.)
        var numeroLimpio = new string(numero.Where(char.IsDigit).ToArray());

        // Si no empieza con +, agregarlo (asumiendo números colombianos +57)
        if (!numeroLimpio.StartsWith("57"))
            numeroLimpio = "57" + numeroLimpio;

        return $"whatsapp:+{numeroLimpio}";
    }

    private string ConstruirMensajeAprobado(
        string nombreAlumno,
        decimal monto,
        string referencia,
        DateTime fechaPago,
        List<string> paquetes)
    {
        var paquetesTexto = paquetes.Any() 
            ? string.Join("\n• ", paquetes) 
            : "Paquete estándar";

        return $@"─────────────────────────────
✅ *Pago Verificado*

¡Hola {nombreAlumno}! 

Tu pago de *${monto:N0} COP* ha sido 
✅ *APROBADO*

📋 *Detalles:*
• Referencia: {referencia}
• Fecha: {fechaPago:dd/MM/yyyy}
• Paquete(s):
• {paquetesTexto}

🎉 *¡Ya puedes agendar tus clases!*

_Academia Chetango_
─────────────────────────────";
    }

    private string ConstruirMensajeRechazado(
        string nombreAlumno,
        decimal monto,
        string motivo)
    {
        return $@"─────────────────────────────
⚠️ *Verificación Requerida*

Hola {nombreAlumno},

Tu pago de *${monto:N0} COP* necesita 
una revisión adicional.

📋 *Motivo:*
{motivo}

📞 Por favor contáctanos para 
resolver esta situación.

_Academia Chetango_
─────────────────────────────";
    }
}

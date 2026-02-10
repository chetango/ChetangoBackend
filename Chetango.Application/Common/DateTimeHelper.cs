namespace Chetango.Application.Common;

/// <summary>
/// Helper para manejar fechas y horas en la zona horaria de Bogotá, Colombia (UTC-5)
/// </summary>
public static class DateTimeHelper
{
    private static readonly TimeZoneInfo BogotaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

    /// <summary>
    /// Obtiene la fecha y hora actual en la zona horaria de Bogotá, Colombia
    /// </summary>
    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BogotaTimeZone);

    /// <summary>
    /// Obtiene solo la fecha actual en la zona horaria de Bogotá, Colombia
    /// </summary>
    public static DateTime Today => Now.Date;
}

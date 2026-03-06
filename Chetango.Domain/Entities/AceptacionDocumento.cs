namespace Chetango.Domain.Entities;

/// <summary>
/// Registro inmutable de que un usuario aceptó una versión concreta de un documento legal.
/// Es la evidencia de trazabilidad: nunca se modifica ni elimina.
/// </summary>
public class AceptacionDocumento
{
    public Guid Id { get; set; }

    /// <summary>Tenant (academia) al que pertenece esta aceptación</summary>
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    /// <summary>Usuario que hizo clic en "Acepto" (el admin de la academia)</summary>
    public Guid IdUsuario { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public Guid VersionDocumentoLegalId { get; set; }
    public VersionDocumentoLegal VersionDocumentoLegal { get; set; } = null!;

    /// <summary>Fecha y hora exacta de la aceptación (UTC)</summary>
    public DateTime FechaAceptacion { get; set; }

    /// <summary>IP desde donde se aceptó el documento</summary>
    public string IpOrigen { get; set; } = null!;

    /// <summary>User-Agent del navegador del usuario en el momento de aceptar</summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Contexto de aceptación: Onboarding | Reacepacion | Manual
    /// </summary>
    public string Contexto { get; set; } = "Onboarding";
}

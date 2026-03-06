// ============================================
// COMPLIANCE DTOs
// ============================================

namespace Chetango.Application.Compliance.DTOs;

/// <summary>
/// Documento legal con su versión activa vigente, listo para mostrar al usuario.
/// </summary>
public record DocumentoPendienteDto(
    Guid VersionId,
    string Codigo,
    string Nombre,
    string? Descripcion,
    string NumeroVersion,
    string UrlDocumento,
    bool EsObligatorio
);

/// <summary>
/// Estado de cumplimiento legal del tenant.
/// El frontend lo usa para decidir si muestra el onboarding o el banner de reaceptación.
/// </summary>
public record EstadoCumplimientoDto(
    Guid TenantId,
    string NombreAcademia,
    bool OnboardingCompletado,
    bool RequiereReaceptacion,
    DateTime? FechaActivacion,
    List<DocumentoPendienteDto> DocumentosPendientes
)
{
    /// <summary>true si no hay documentos pendientes y el onboarding está completo.</summary>
    public bool PuedeOperar => OnboardingCompletado && !RequiereReaceptacion && DocumentosPendientes.Count == 0;
};

/// <summary>
/// Registro de una aceptación devuelta al cliente (solo lectura, nunca se edita).
/// </summary>
public record AceptacionDocumentoDto(
    Guid Id,
    string CodigoDocumento,
    string NombreDocumento,
    string NumeroVersion,
    DateTime FechaAceptacion,
    string IpOrigen,
    string Contexto
);

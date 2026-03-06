namespace Chetango.Domain.Entities;

/// <summary>
/// Catálogo de documentos legales que la plataforma maneja (Términos, DPA, Política, etc.).
/// Este registro define el tipo de documento; las versiones concretas viven en VersionDocumentoLegal.
/// </summary>
public class DocumentoLegal
{
    public Guid Id { get; set; }

    /// <summary>Clave única de negocio: TERMINOS, DPA, POLITICA_PRIVACIDAD, AVISO_PRIVACIDAD</summary>
    public string Codigo { get; set; } = null!;

    /// <summary>Nombre legible: "Términos y Condiciones del Servicio"</summary>
    public string Nombre { get; set; } = null!;

    /// <summary>Descripción breve del propósito del documento</summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo de destinatario que debe aceptar este documento.
    /// Valores: Admin | Todos
    /// Admin = solo el admin de la academia.
    /// Todos = cada usuario en su primer login.
    /// </summary>
    public string Destinatario { get; set; } = "Admin";

    /// <summary>Si es true, el tenant no puede activarse sin aceptar este documento.</summary>
    public bool EsObligatorio { get; set; } = true;

    /// <summary>Si es true, una nueva versión exige reaceptación para continuar usando la plataforma.</summary>
    public bool RequiereReaceptacion { get; set; } = true;

    public bool Activo { get; set; } = true;

    // Auditoría
    public DateTime FechaCreacion { get; set; }
    public string CreadoPor { get; set; } = null!;

    // Navegación
    public ICollection<VersionDocumentoLegal> Versiones { get; set; } = new List<VersionDocumentoLegal>();
}

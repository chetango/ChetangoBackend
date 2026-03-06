namespace Chetango.Domain.Entities;

/// <summary>
/// Versión concreta de un DocumentoLegal.
/// Cada vez que cambia un documento se crea una nueva versión; las anteriores quedan archivadas.
/// </summary>
public class VersionDocumentoLegal
{
    public Guid Id { get; set; }

    public Guid DocumentoLegalId { get; set; }
    public DocumentoLegal DocumentoLegal { get; set; } = null!;

    /// <summary>Número de versión semántico: "1.0", "1.1", "2.0"</summary>
    public string NumeroVersion { get; set; } = null!;

    /// <summary>URL al texto completo del documento (Azure Blob o ruta estática)</summary>
    public string UrlDocumento { get; set; } = null!;

    /// <summary>Resumen de los cambios respecto a la versión anterior</summary>
    public string? ResumenCambios { get; set; }

    /// <summary>Cuando esta versión entró en vigencia</summary>
    public DateTime FechaVigencia { get; set; }

    /// <summary>
    /// Si es true, los tenants que ya aceptaron una versión anterior deben reaceptar esta.
    /// Solo aplica cuando DocumentoLegal.RequiereReaceptacion = true.
    /// </summary>
    public bool EsCambioSignificativo { get; set; } = false;

    public bool Activa { get; set; } = true;

    // Auditoría
    public DateTime FechaCreacion { get; set; }
    public string CreadoPor { get; set; } = null!;

    // Navegación
    public ICollection<AceptacionDocumento> Aceptaciones { get; set; } = new List<AceptacionDocumento>();
}

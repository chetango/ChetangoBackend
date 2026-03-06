namespace Chetango.Application.Sedes.DTOs;

/// <summary>
/// DTO de una sede configurada para el tenant actual.
/// </summary>
public class SedeConfigDTO
{
    /// <summary>Identificador único del registro (requerido para operaciones PUT/DELETE).</summary>
    public Guid Id { get; set; }

    /// <summary>Valor numérico de la sede (discriminador en tablas de negocio).</summary>
    public int SedeValor { get; set; }

    /// <summary>Nombre visible de la sede (personalizado por cada academia).</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Indica si la sede está activa.</summary>
    public bool Activa { get; set; }

    /// <summary>Indica si es la sede predeterminada del tenant.</summary>
    public bool EsDefault { get; set; }

    /// <summary>Orden de presentación en la UI.</summary>
    public int Orden { get; set; }
}

namespace Chetango.Domain.Entities;

/// <summary>
/// Configuración de sedes por tenant.
/// Permite que cada academia defina sus propias sedes con nombres personalizados,
/// mapeando el valor entero del enum Sede a un nombre legible específico del tenant.
/// </summary>
public class SedeConfig
{
    /// <summary>Identificador único del registro.</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant al que pertenece esta sede.</summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Valor numérico de la sede (corresponde al enum Sede: 1=Medellin, 2=Manizales, etc.).
    /// Los datos existentes en otras tablas usan este int como discriminador.
    /// </summary>
    public int SedeValor { get; set; }

    /// <summary>Nombre visible para el usuario (ej: "Medellín", "Manizales", "Principal").</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Indica si esta sede está activa y visible.</summary>
    public bool Activa { get; set; } = true;

    /// <summary>Indica si es la sede predeterminada para usuarios nuevos del tenant.</summary>
    public bool EsDefault { get; set; } = false;

    /// <summary>Orden de presentación en filtros y listas.</summary>
    public int Orden { get; set; } = 1;

    /// <summary>Fecha de creación del registro.</summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}

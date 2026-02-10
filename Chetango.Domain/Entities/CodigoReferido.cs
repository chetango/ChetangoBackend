namespace Chetango.Domain.Entities;

/// <summary>
/// Código de referido de alumno para programa "Invita un Amigo"
/// El alumno comparte su código, cuando alguien se inscribe con ese código,
/// ambos reciben beneficios
/// </summary>
public class CodigoReferido
{
    public Guid IdCodigo { get; set; }
    
    public Guid IdAlumno { get; set; }
    public Alumno Alumno { get; set; } = null!;
    
    /// <summary>
    /// Código único de 6-8 caracteres (ej: "JUAN2026", "MARIA01")
    /// </summary>
    public string Codigo { get; set; } = string.Empty;
    
    public bool Activo { get; set; } = true;
    
    public int VecesUsado { get; set; } = 0;
    
    /// <summary>
    /// Beneficio para el alumno referidor: ej. "1 clase gratis"
    /// </summary>
    public string? BeneficioReferidor { get; set; }
    
    /// <summary>
    /// Beneficio para el nuevo alumno: ej. "10% descuento en primer paquete"
    /// </summary>
    public string? BeneficioNuevoAlumno { get; set; }
    
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    
    // Navigation
    public ICollection<UsoCodigoReferido> Usos { get; set; } = new List<UsoCodigoReferido>();
}

namespace Chetango.Domain.Entities;

/// <summary>
/// Registro de uso de un código de referido
/// Se crea cuando un nuevo alumno se inscribe usando el código
/// </summary>
public class UsoCodigoReferido
{
    public Guid IdUso { get; set; }
    
    public Guid IdCodigoReferido { get; set; }
    public CodigoReferido CodigoReferido { get; set; } = null!;
    
    public Guid IdAlumnoReferidor { get; set; }
    public Alumno AlumnoReferidor { get; set; } = null!;
    
    public Guid IdAlumnoNuevo { get; set; }
    public Alumno AlumnoNuevo { get; set; } = null!;
    
    public DateTime FechaUso { get; set; }
    
    /// <summary>
    /// Estados: Pendiente, Aplicado, Cancelado
    /// </summary>
    public string Estado { get; set; } = "Pendiente";
    
    public bool BeneficioAplicadoReferidor { get; set; } = false;
    public DateTime? FechaBeneficioReferidor { get; set; }
    
    public bool BeneficioAplicadoNuevo { get; set; } = false;
    public DateTime? FechaBeneficioNuevo { get; set; }
    
    public string? Observaciones { get; set; }
}

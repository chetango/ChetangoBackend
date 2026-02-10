namespace Chetango.Domain.Entities;

/// <summary>
/// Eventos de la academia (talleres, milongas, workshops, etc.)
/// </summary>
public class Evento
{
    public Guid IdEvento { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan? Hora { get; set; }
    public decimal? Precio { get; set; }
    public bool Destacado { get; set; }
    public string? ImagenUrl { get; set; }
    public string? ImagenNombre { get; set; }
    public bool Activo { get; set; } = true;
    public string TipoAudiencia { get; set; } = "Todos"; // "Alumno", "Profesor", "Todos"
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public Guid IdUsuarioCreador { get; set; }
    
    // Navigation
    public Usuario UsuarioCreador { get; set; } = null!;
}

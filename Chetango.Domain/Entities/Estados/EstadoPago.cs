namespace Chetango.Domain.Entities.Estados;

public class EstadoPago
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!; // "Pendiente Verificación", "Verificado", "Rechazado"
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public string UsuarioCreacion { get; set; } = null!;
    public string? UsuarioModificacion { get; set; }
    
    // Relaciones
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}

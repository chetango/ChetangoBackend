namespace Chetango.Application.Eventos.DTOs;

/// <summary>
/// DTO para respuesta de evento
/// </summary>
public class EventoDto
{
    public Guid IdEvento { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan? Hora { get; set; }
    public decimal? Precio { get; set; }
    public bool Destacado { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para crear un nuevo evento
/// </summary>
public class CreateEventoDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan? Hora { get; set; }
    public decimal? Precio { get; set; }
    public bool Destacado { get; set; }
    public string? ImagenUrl { get; set; }
}

/// <summary>
/// DTO para actualizar un evento existente
/// </summary>
public class UpdateEventoDto
{
    public Guid IdEvento { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan? Hora { get; set; }
    public decimal? Precio { get; set; }
    public bool Destacado { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; }
}

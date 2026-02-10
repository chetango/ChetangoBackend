// ============================================
// PERFIL ALUMNO DTOs
// ============================================

namespace Chetango.Application.Perfil.DTOs;

public record AlumnoPerfilDto
{
    public Guid IdAlumno { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string TipoDocumento { get; init; } = string.Empty;
    public string NumeroDocumento { get; init; } = string.Empty;
    public DateTime FechaInscripcion { get; init; }
    public string? AvatarUrl { get; init; }
    public ContactoEmergenciaDto? ContactoEmergencia { get; init; }
    public ConfiguracionAlumnoDto Configuracion { get; init; } = new();
}

public record ContactoEmergenciaDto
{
    public string NombreCompleto { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Relacion { get; init; } = string.Empty;
}

public record ConfiguracionAlumnoDto
{
    public bool NotificacionesEmail { get; init; } = true;
    public bool RecordatoriosClase { get; init; } = true;
    public bool AlertasPaquete { get; init; } = true;
}

public record PaqueteHistorialDto
{
    public Guid IdPaquete { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public DateTime FechaCompra { get; init; }
    public DateTime? FechaVencimiento { get; init; }
    public int ClasesTotales { get; init; }
    public int ClasesUsadas { get; init; }
    public decimal Precio { get; init; }
    public string Estado { get; init; } = string.Empty;
}

public record UpdateDatosPersonalesDto
{
    public string NombreCompleto { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
}

public record UpdateContactoEmergenciaDto
{
    public string NombreCompleto { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Relacion { get; init; } = string.Empty;
}

public record UpdateConfiguracionDto
{
    public bool NotificacionesEmail { get; init; }
    public bool RecordatoriosClase { get; init; }
    public bool AlertasPaquete { get; init; }
}

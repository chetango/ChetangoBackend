// ============================================
// USUARIOS DTOs
// ============================================

using System.Text.Json.Serialization;

namespace Chetango.Application.Usuarios.DTOs;

public class UsuarioDTO
{
    [JsonPropertyName("idUsuario")]
    public Guid UsuarioId { get; set; }
    
    [JsonPropertyName("nombreUsuario")]
    public string NombreUsuario { get; set; } = null!;
    
    [JsonPropertyName("correo")]
    public string Correo { get; set; } = null!;
    
    [JsonPropertyName("telefono")]
    public string Telefono { get; set; } = null!;
    
    [JsonPropertyName("tipoDocumento")]
    public string TipoDocumento { get; set; } = null!;
    
    [JsonPropertyName("numeroDocumento")]
    public string NumeroDocumento { get; set; } = null!;
    
    [JsonPropertyName("rol")]
    public string Rol { get; set; } = null!; // "admin" | "profesor" | "alumno"
    
    [JsonPropertyName("estado")]
    public string Estado { get; set; } = null!; // "activo" | "inactivo" | "pendiente_azure"
    
    [JsonPropertyName("fechaCreacion")]
    public DateTime FechaCreacion { get; set; }
}

public class UsuarioDetalleDTO
{
    [JsonPropertyName("idUsuario")]
    public Guid UsuarioId { get; set; }
    
    [JsonPropertyName("nombreUsuario")]
    public string NombreUsuario { get; set; } = null!;
    
    [JsonPropertyName("correo")]
    public string Correo { get; set; } = null!;
    
    [JsonPropertyName("telefono")]
    public string Telefono { get; set; } = null!;
    
    [JsonPropertyName("tipoDocumento")]
    public string TipoDocumento { get; set; } = null!;
    
    [JsonPropertyName("numeroDocumento")]
    public string NumeroDocumento { get; set; } = null!;
    
    [JsonPropertyName("fechaNacimiento")]
    public string? FechaNacimiento { get; set; }
    
    [JsonPropertyName("rol")]
    public string Rol { get; set; } = null!;
    
    [JsonPropertyName("estado")]
    public string Estado { get; set; } = null!;
    
    [JsonPropertyName("fechaCreacion")]
    public DateTime FechaCreacion { get; set; }
    
    // Datos específicos por rol
    [JsonPropertyName("datosProfesor")]
    public DatosProfesorDTO? DatosProfesor { get; set; }
    
    [JsonPropertyName("datosAlumno")]
    public DatosAlumnoDTO? DatosAlumno { get; set; }
    
    // Credenciales Azure
    [JsonPropertyName("correoAzure")]
    public string? CorreoAzure { get; set; }
}

public class DatosProfesorDTO
{
    [JsonPropertyName("idProfesor")]
    public Guid IdProfesor { get; set; }
    
    [JsonPropertyName("tipoProfesor")]
    public string TipoProfesor { get; set; } = null!;
    
    [JsonPropertyName("fechaIngreso")]
    public DateTime FechaIngreso { get; set; }
    
    [JsonPropertyName("biografia")]
    public string? Biografia { get; set; }
    
    [JsonPropertyName("especialidades")]
    public List<string> Especialidades { get; set; } = new();
    
    [JsonPropertyName("tarifaActual")]
    public decimal TarifaActual { get; set; }
}

public class DatosAlumnoDTO
{
    [JsonPropertyName("idAlumno")]
    public Guid IdAlumno { get; set; }
    
    [JsonPropertyName("fechaInscripcion")]
    public DateTime FechaInscripcion { get; set; }
    
    [JsonPropertyName("contactoEmergencia")]
    public string? ContactoEmergencia { get; set; }
    
    [JsonPropertyName("telefonoEmergencia")]
    public string? TelefonoEmergencia { get; set; }
    
    [JsonPropertyName("observacionesMedicas")]
    public string? ObservacionesMedicas { get; set; }
}

public class UsuariosPaginadosDTO
{
    public List<UsuarioDTO> Usuarios { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}

// ============================================
// REQUEST DTOs
// ============================================

public class CreateUserRequest
{
    public string NombreUsuario { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string TipoDocumento { get; set; } = null!;
    public string NumeroDocumento { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public string? FechaNacimiento { get; set; }
    public ProfesorDataRequest? DatosProfesor { get; set; }
    public AlumnoDataRequest? DatosAlumno { get; set; }
    public string CorreoAzure { get; set; } = null!;
    public string ContrasenaTemporalAzure { get; set; } = null!;
    public bool EnviarWhatsApp { get; set; }
    public bool EnviarEmail { get; set; }
}

public class UpdateUserRequest
{
    public Guid IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string? FechaNacimiento { get; set; }
    public ProfesorDataRequest? DatosProfesor { get; set; }
    public AlumnoDataRequest? DatosAlumno { get; set; }
}

public class ProfesorDataRequest
{
    public string TipoProfesor { get; set; } = null!;
    public DateTime FechaIngreso { get; set; }
    public string? Biografia { get; set; }
    public List<string> Especialidades { get; set; } = new();
    public decimal TarifaActual { get; set; }
}

public class AlumnoDataRequest
{
    public string? ContactoEmergencia { get; set; }
    public string? TelefonoEmergencia { get; set; }
    public string? ObservacionesMedicas { get; set; }
}

public class ActivateUserRequest
{
    public string CorreoAzure { get; set; } = null!;
    public string ContrasenaTemporalAzure { get; set; } = null!;
}

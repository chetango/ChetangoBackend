// ============================================
// PROFESOR PROFILE DTO
// ============================================

namespace Chetango.Application.Profesores.DTOs;

public record ProfesorProfileDTO(
    Guid IdProfesor,
    string NombreCompleto,
    string Correo,
    string Telefono,
    string TipoDocumento,
    string NumeroDocumento,
    string TipoProfesor,
    DateTime FechaIngreso,
    string? Biografia,
    List<string> Especialidades,
    ConfiguracionProfesorDTO Configuracion
);

public record ConfiguracionProfesorDTO(
    bool NotificacionesEmail,
    bool RecordatoriosClase,
    bool AlertasCambios
);

public record UpdateDatosPersonalesProfesorDTO(
    string NombreCompleto,
    string Telefono
);

public record UpdatePerfilProfesionalDTO(
    string? Biografia,
    List<string> Especialidades
);

public record UpdateConfiguracionProfesorDTO(
    bool NotificacionesEmail,
    bool RecordatoriosClase,
    bool AlertasCambios
);

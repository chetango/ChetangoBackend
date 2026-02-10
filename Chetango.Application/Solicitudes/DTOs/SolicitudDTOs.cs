namespace Chetango.Application.Solicitudes.DTOs;

/// <summary>
/// DTO para solicitud de renovación de paquete
/// </summary>
public record SolicitudRenovacionPaqueteDTO(
    Guid IdSolicitud,
    Guid IdAlumno,
    string NombreAlumno,
    string CorreoAlumno,
    Guid? IdPaqueteActual,
    string? TipoPaqueteActual,
    int? ClasesRestantes,
    string TipoPaqueteDeseado,
    string? MensajeAlumno,
    string Estado,
    DateTime FechaSolicitud,
    DateTime? FechaRespuesta,
    string? MensajeRespuesta
);

/// <summary>
/// DTO para solicitud de clase privada
/// </summary>
public record SolicitudClasePrivadaDTO(
    Guid IdSolicitud,
    Guid IdAlumno,
    string NombreAlumno,
    string CorreoAlumno,
    string TipoClaseDeseado,
    DateTime? FechaPreferida,
    string? HoraPreferida,
    string? ObservacionesAlumno,
    string Estado,
    DateTime FechaSolicitud,
    DateTime? FechaRespuesta,
    string? MensajeRespuesta
);

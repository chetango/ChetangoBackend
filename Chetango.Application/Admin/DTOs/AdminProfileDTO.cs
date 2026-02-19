// ============================================
// ADMIN PROFILE DTO
// ============================================

using Chetango.Domain.Enums;
using System.Text.Json.Serialization;

namespace Chetango.Application.Admin.DTOs;

public record AdminProfileDTO(
    Guid IdAdministrador,
    string NombreCompleto,
    string Correo,
    string Telefono,
    string DireccionPersonal,
    DateTime? FechaNacimiento,
    string TipoDocumento,
    string NumeroDocumento,
    DateTime FechaIngreso,
    DateTime? UltimaActividad,
    string Cargo,
    string Departamento,
    List<string> Permisos,
    [property: JsonPropertyName("sede")] Sede Sede,
    [property: JsonPropertyName("sedeNombre")] string SedeNombre,
    DatosAcademiaDTO DatosAcademia,
    ConfiguracionAdminDTO Configuracion
);

public record DatosAcademiaDTO(
    string NombreAcademia,
    string Direccion,
    string Telefono,
    string EmailInstitucional,
    string? Instagram,
    string? Facebook,
    string? WhatsApp
);

public record ConfiguracionAdminDTO(
    bool NotificacionesEmail,
    bool AlertasPagosPendientes,
    bool ReportesAutomaticos,
    bool AlertasPaquetesVencer,
    bool AlertasAsistenciaBaja,
    bool NotificacionesNuevosRegistros
);

public record SeguridadInfoDTO(
    int SesionesActivas,
    DateTime? UltimoCambioPassword,
    List<HistorialAccesoDTO> HistorialAccesos
);

public record HistorialAccesoDTO(
    DateTime Fecha,
    string Dispositivo,
    string Navegador,
    string Ip
);

// DTOs para actualización
public record UpdateDatosPersonalesAdminDTO(
    string NombreCompleto,
    string Telefono,
    string Direccion,
    DateTime? FechaNacimiento
);

public record UpdateDatosAcademiaDTO(
    string Nombre,
    string Direccion,
    string Telefono,
    string Email,
    string? Instagram,
    string? Facebook,
    string? WhatsApp
);

public record UpdateConfiguracionAdminDTO(
    bool NotificacionesEmail,
    bool AlertasPagosPendientes,
    bool ReportesAutomaticos,
    bool AlertasPaquetesVencer,
    bool AlertasAsistenciaBaja,
    bool NotificacionesNuevosRegistros
);

public record CambiarPasswordDTO(
    string PasswordActual,
    string PasswordNuevo,
    string ConfirmarPassword
);

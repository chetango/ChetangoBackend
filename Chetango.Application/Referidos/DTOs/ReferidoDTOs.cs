namespace Chetango.Application.Referidos.DTOs;

/// <summary>
/// DTO para código de referido del alumno
/// </summary>
public record CodigoReferidoDTO(
    Guid IdCodigo,
    string Codigo,
    bool Activo,
    int VecesUsado,
    string? BeneficioReferidor,
    string? BeneficioNuevoAlumno,
    DateTime FechaCreacion
);

/// <summary>
/// DTO para uso de código de referido
/// </summary>
public record UsoCodigoReferidoDTO(
    Guid IdUso,
    string Codigo,
    string NombreAlumnoReferidor,
    string NombreAlumnoNuevo,
    DateTime FechaUso,
    string Estado,
    bool BeneficioAplicadoReferidor,
    bool BeneficioAplicadoNuevo
);

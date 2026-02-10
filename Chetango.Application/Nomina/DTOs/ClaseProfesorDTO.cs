namespace Chetango.Application.Nomina.DTOs;

public record ClaseProfesorDTO(
    Guid IdClaseProfesor,
    Guid IdClase,
    DateTime FechaClase,
    string NombreClase,
    string TipoClase,
    Guid IdProfesor,
    string NombreProfesor,
    string RolEnClase,
    decimal TarifaProgramada,
    decimal ValorAdicional,
    string? ConceptoAdicional,
    decimal TotalPago,
    string EstadoPago,
    DateTime? FechaAprobacion,
    DateTime? FechaPago,
    string? NombreAprobador
);

public record ClaseRealizadaDTO(
    Guid IdClase,
    DateTime FechaClase,
    string NombreClase,
    string TipoClase,
    string Estado,
    List<ProfesorClaseDTO> Profesores
);

public record ProfesorClaseDTO(
    Guid? IdClaseProfesor,
    Guid IdProfesor,
    string NombreProfesor,
    string RolEnClase,
    decimal TarifaProgramada,
    decimal ValorAdicional,
    decimal TotalPago,
    string EstadoPago
);

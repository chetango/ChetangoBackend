using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Commands.CrearClase;

// DTO para especificar profesor con su rol
public record ProfesorClaseRequest(
    Guid IdProfesor,
    string RolEnClase // "Principal" | "Monitor"
);

// Command para crear una nueva clase con múltiples profesores
public record CrearClaseCommand(
    List<ProfesorClaseRequest> Profesores, // Lista de profesores con sus roles
    Guid IdTipoClase,
    DateTime Fecha,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    int CupoMaximo,
    string? Observaciones,
    
    // DEPRECATED: Mantener para retrocompatibilidad temporal
    Guid? IdProfesorPrincipal,
    List<Guid>? IdsMonitores,
    
    // Validación
    string? IdUsuarioActual,
    bool EsAdmin
) : IRequest<Result<Guid>>;

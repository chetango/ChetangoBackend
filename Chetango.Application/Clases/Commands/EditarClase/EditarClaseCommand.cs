using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Commands.EditarClase;

// DTO para especificar profesor con su rol (reutilizado de CrearClase)
public record ProfesorClaseRequest(
    Guid IdProfesor,
    string RolEnClase // "Principal" | "Monitor"
);

// Command para editar una clase existente
public record EditarClaseCommand(
    Guid IdClase,
    Guid IdTipoClase,
    List<ProfesorClaseRequest> Profesores, // Lista de profesores con sus roles
    DateTime FechaHoraInicio,
    int DuracionMinutos,
    int CupoMaximo,
    string? Observaciones,
    string? IdUsuarioActual, // Para validación de ownership
    bool EsAdmin // Para bypass de ownership validation
) : IRequest<Result<bool>>;

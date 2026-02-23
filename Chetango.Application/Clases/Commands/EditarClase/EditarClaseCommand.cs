using Chetango.Application.Clases.Commands.CrearClase;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Commands.EditarClase;

// Command para editar una clase existente
// Reutiliza ProfesorClaseRequest de CrearClase
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

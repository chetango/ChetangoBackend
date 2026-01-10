using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Commands.EditarClase;

// Command para editar una clase existente
public record EditarClaseCommand(
    Guid IdClase,
    Guid IdTipoClase,
    Guid IdProfesor,
    DateTime FechaHoraInicio,
    int DuracionMinutos,
    int CupoMaximo,
    string? Observaciones,
    string? IdUsuarioActual, // Para validación de ownership
    bool EsAdmin // Para bypass de ownership validation
) : IRequest<Result<bool>>;

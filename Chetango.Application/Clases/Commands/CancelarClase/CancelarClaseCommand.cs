using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Commands.CancelarClase;

// Command para cancelar una clase programada
public record CancelarClaseCommand(
    Guid IdClase,
    string? IdUsuarioActual, // Para validación de ownership
    bool EsAdmin // Para bypass de ownership validation
) : IRequest<Result<bool>>;

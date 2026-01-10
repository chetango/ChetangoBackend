using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Commands.CrearClase;

// Command para crear una nueva clase
public record CrearClaseCommand(
    Guid IdProfesorPrincipal,
    Guid IdTipoClase,
    DateTime Fecha,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    int CupoMaximo,
    string? Observaciones,
    string? IdUsuarioActual, // Para validación de ownership
    bool EsAdmin // Para bypass de ownership validation
) : IRequest<Result<Guid>>;

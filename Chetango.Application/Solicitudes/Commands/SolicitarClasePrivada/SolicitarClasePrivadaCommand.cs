using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Solicitudes.Commands.SolicitarClasePrivada;

/// <summary>
/// Command para que un alumno solicite una clase privada
/// </summary>
public record SolicitarClasePrivadaCommand(
    string EmailAlumno,
    Guid? IdTipoClaseDeseado,
    DateTime? FechaPreferida,
    TimeSpan? HoraPreferida,
    string? ObservacionesAlumno
) : IRequest<Result<Guid>>;

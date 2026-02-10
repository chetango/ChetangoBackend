using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Solicitudes.Commands.SolicitarRenovacionPaquete;

/// <summary>
/// Command para que un alumno solicite renovación de su paquete
/// </summary>
public record SolicitarRenovacionPaqueteCommand(
    string EmailAlumno,
    Guid? IdTipoPaqueteDeseado,
    string? MensajeAlumno
) : IRequest<Result<Guid>>;

using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Commands.RegistrarAsistencia;

// Command para registrar una asistencia de un alumno a una clase
public record RegistrarAsistenciaCommand(
    Guid IdClase,
    Guid IdAlumno,
    Guid IdPaqueteUsado,
    int IdEstadoAsistencia, // 1=Presente, 2=Ausente, 3=Justificada
    string? Observaciones
) : IRequest<Result<Guid>>;

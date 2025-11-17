using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Commands.RegistrarAsistencia;

// Command para registrar una asistencia de un alumno a una clase
public record RegistrarAsistenciaCommand(
    Guid IdClase,
    Guid IdAlumno,
    Guid IdPaquete,
    int IdEstado, // 1=Presente, 2=Ausente, 3=Justificada
    string? Observacion
) : IRequest<Result<Guid>>;

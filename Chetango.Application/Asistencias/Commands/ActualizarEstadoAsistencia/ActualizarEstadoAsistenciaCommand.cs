using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Commands.ActualizarEstadoAsistencia;

// Command para actualizar el estado de una asistencia existente
public record ActualizarEstadoAsistenciaCommand(
    Guid IdAsistencia,
    int NuevoEstado, // 1=Presente, 2=Ausente, 3=Justificada
    string? Observacion
) : IRequest<Result<Unit>>;

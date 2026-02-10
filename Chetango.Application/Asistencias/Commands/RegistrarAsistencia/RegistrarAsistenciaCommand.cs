using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Commands.RegistrarAsistencia;

// Command para registrar una asistencia de un alumno a una clase
public record RegistrarAsistenciaCommand(
    Guid IdClase,
    Guid IdAlumno,
    int IdTipoAsistencia, // Tipo: 1=Normal, 2=Cortesía, 3=Prueba, 4=Recuperación
    Guid? IdPaqueteUsado, // Nullable: requerido solo si TipoAsistencia.RequierePaquete = true
    int IdEstadoAsistencia, // 1=Presente, 2=Ausente, 3=Justificada
    string? Observaciones
) : IRequest<Result<Guid>>;

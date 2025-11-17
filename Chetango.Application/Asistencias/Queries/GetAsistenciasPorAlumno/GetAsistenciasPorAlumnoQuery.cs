using Chetango.Application.Asistencias.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Queries.GetAsistenciasPorAlumno;

// Query para obtener el historial de asistencias de un alumno
public record GetAsistenciasPorAlumnoQuery(
    Guid IdAlumno,
    DateTime? FechaDesde,
    DateTime? FechaHasta
) : IRequest<Result<IReadOnlyList<AsistenciaDto>>>;

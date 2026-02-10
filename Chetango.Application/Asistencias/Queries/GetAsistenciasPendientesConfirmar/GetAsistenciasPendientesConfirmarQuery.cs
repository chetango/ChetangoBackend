using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Asistencias.Queries.GetAsistenciasPendientesConfirmar;

/// <summary>
/// Query para obtener las asistencias pendientes de confirmar de un alumno.
/// Retorna solo las asistencias que:
/// - Están marcadas como "Presente" por el profesor/admin
/// - NO han sido confirmadas por el alumno (Confirmado = false)
/// </summary>
public record GetAsistenciasPendientesConfirmarQuery(Guid IdAlumno) 
    : IRequest<Result<IReadOnlyList<AsistenciaPendienteDto>>>;

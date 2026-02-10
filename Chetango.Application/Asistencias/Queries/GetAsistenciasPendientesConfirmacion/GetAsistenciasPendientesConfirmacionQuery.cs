using Chetango.Application.Asistencias.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Queries.GetAsistenciasPendientesConfirmacion;

/// <summary>
/// Query para obtener todas las asistencias pendientes de confirmación del alumno autenticado
/// Retorna asistencias donde Presente = true pero Confirmado = false
/// </summary>
public record GetAsistenciasPendientesConfirmacionQuery : IRequest<Result<IReadOnlyList<AsistenciaPendienteConfirmacionDto>>>;

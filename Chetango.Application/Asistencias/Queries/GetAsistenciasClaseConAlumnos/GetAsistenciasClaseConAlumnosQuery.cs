using Chetango.Application.Asistencias.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Queries.GetAsistenciasClaseConAlumnos;

/// <summary>
/// Query para obtener asistencias de una clase incluyendo TODOS los alumnos activos
/// Utilizado por profesores para marcar asistencias
/// </summary>
public record GetAsistenciasClaseConAlumnosQuery(Guid IdClase) 
    : IRequest<Result<IReadOnlyList<AsistenciaProfesorDto>>>;

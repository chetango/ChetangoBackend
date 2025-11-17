using Chetango.Application.Asistencias.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Queries.GetAsistenciasPorClase;

// Query para obtener todas las asistencias de una clase específica
public record GetAsistenciasPorClaseQuery(Guid IdClase) : IRequest<Result<IReadOnlyList<AsistenciaDto>>>;

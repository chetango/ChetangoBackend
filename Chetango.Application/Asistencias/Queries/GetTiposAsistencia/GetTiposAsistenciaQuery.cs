using Chetango.Application.Asistencias.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Asistencias.Queries.GetTiposAsistencia;

public record GetTiposAsistenciaQuery : IRequest<Result<List<TipoAsistenciaDto>>>;

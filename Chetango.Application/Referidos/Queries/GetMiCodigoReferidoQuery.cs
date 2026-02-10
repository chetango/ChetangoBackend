using Chetango.Application.Common;
using Chetango.Application.Referidos.DTOs;
using MediatR;

namespace Chetango.Application.Referidos.Queries;

/// <summary>
/// Query para obtener el código de referido del alumno autenticado
/// </summary>
public record GetMiCodigoReferidoQuery(
    string EmailAlumno
) : IRequest<Result<CodigoReferidoDTO?>>;

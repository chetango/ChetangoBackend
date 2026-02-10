using Chetango.Application.Common;
using Chetango.Application.Referidos.DTOs;
using MediatR;

namespace Chetango.Application.Referidos.Commands;

/// <summary>
/// Command para generar código de referido para un alumno
/// </summary>
public record GenerarCodigoReferidoCommand(
    string EmailAlumno
) : IRequest<Result<CodigoReferidoDTO>>;

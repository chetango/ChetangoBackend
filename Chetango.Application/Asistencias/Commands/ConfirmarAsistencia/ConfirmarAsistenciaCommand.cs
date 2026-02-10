using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Asistencias.Commands.ConfirmarAsistencia;

/// <summary>
/// Comando para que un alumno confirme su asistencia a una clase.
/// La asistencia ya fue marcada como "Presente" por el profesor/admin.
/// El alumno solo confirma que efectivamente estuvo.
/// </summary>
public record ConfirmarAsistenciaCommand(Guid IdAsistencia) : IRequest<Result<bool>>;

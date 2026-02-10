// ============================================
// UPDATE CONFIGURACION COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Perfil.Commands;

public record UpdateConfiguracionCommand(
    Guid IdAlumno,
    bool NotificacionesEmail,
    bool RecordatoriosClase,
    bool AlertasPaquete
) : IRequest<Result<Unit>>;

public class UpdateConfiguracionCommandHandler : IRequestHandler<UpdateConfiguracionCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdateConfiguracionCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(UpdateConfiguracionCommand request, CancellationToken cancellationToken)
    {
        var alumno = await _db.Alumnos
            .FirstOrDefaultAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (alumno == null)
            return Result<Unit>.Failure("Alumno no encontrado");

        alumno.NotificacionesEmail = request.NotificacionesEmail;
        alumno.RecordatoriosClase = request.RecordatoriosClase;
        alumno.AlertasPaquete = request.AlertasPaquete;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

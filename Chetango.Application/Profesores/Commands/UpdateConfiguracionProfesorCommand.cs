// ============================================
// UPDATE CONFIGURACION PROFESOR COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Profesores.Commands;

public record UpdateConfiguracionProfesorCommand(
    Guid IdProfesor,
    bool NotificacionesEmail,
    bool RecordatoriosClase,
    bool AlertasCambios
) : IRequest<Result<Unit>>;

public class UpdateConfiguracionProfesorCommandHandler : IRequestHandler<UpdateConfiguracionProfesorCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdateConfiguracionProfesorCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(UpdateConfiguracionProfesorCommand request, CancellationToken cancellationToken)
    {
        var profesor = await _db.Set<Chetango.Domain.Entities.Estados.Profesor>()
            .FirstOrDefaultAsync(p => p.IdProfesor == request.IdProfesor, cancellationToken);

        if (profesor == null)
            return Result<Unit>.Failure("Profesor no encontrado");

        // Actualizar configuración
        profesor.NotificacionesEmail = request.NotificacionesEmail;
        profesor.RecordatoriosClase = request.RecordatoriosClase;
        profesor.AlertasCambios = request.AlertasCambios;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

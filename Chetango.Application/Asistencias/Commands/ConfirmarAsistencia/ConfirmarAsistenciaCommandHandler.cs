using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Application.Asistencias.Commands.ConfirmarAsistencia;

public class ConfirmarAsistenciaCommandHandler : IRequestHandler<ConfirmarAsistenciaCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public ConfirmarAsistenciaCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(ConfirmarAsistenciaCommand request, CancellationToken cancellationToken)
    {
        // Buscar la asistencia
        var asistencia = await _db.Set<Asistencia>()
            .Include(a => a.Estado)
            .FirstOrDefaultAsync(a => a.IdAsistencia == request.IdAsistencia, cancellationToken);

        if (asistencia == null)
        {
            return Result<bool>.Failure("Asistencia no encontrada");
        }

        // Verificar que la asistencia esté marcada como "Presente"
        // Solo se pueden confirmar asistencias donde el profesor/admin ya marcó presente
        if (asistencia.Estado.Nombre != "Presente")
        {
            return Result<bool>.Failure("Solo se pueden confirmar asistencias marcadas como Presente");
        }

        // Verificar que no esté ya confirmada
        if (asistencia.Confirmado)
        {
            return Result<bool>.Success(true); // Ya estaba confirmada, retornar éxito
        }

        // Confirmar la asistencia
        asistencia.Confirmado = true;
        asistencia.FechaConfirmacion = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

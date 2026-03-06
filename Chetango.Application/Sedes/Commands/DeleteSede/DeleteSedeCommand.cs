using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Sedes.Commands.DeleteSede;

/// <summary>
/// Comando para desactivar una sede (soft delete).
/// No elimina datos históricos (Asistencias, Pagos, etc.) asociados a esa sede.
/// Si la sede eliminada era la default, promueve automáticamente la siguiente sede activa.
/// Regla de negocio: no se puede eliminar la única sede activa.
/// </summary>
public record DeleteSedeCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteSedeCommandHandler : IRequestHandler<DeleteSedeCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public DeleteSedeCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(DeleteSedeCommand request, CancellationToken cancellationToken)
    {
        // El query filter aplica TenantId automáticamente → protege cross-tenant
        var sede = await _db.SedeConfigs
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.Activa, cancellationToken);

        if (sede is null)
            return Result<bool>.Failure("La sede no existe o no pertenece a tu academia.");

        // ─── Regla de negocio: no puede quedar una academia sin sedes ────────────
        var totalActivas = await _db.SedeConfigs
            .CountAsync(s => s.Activa, cancellationToken);

        if (totalActivas <= 1)
            return Result<bool>.Failure("No puedes eliminar la única sede activa de tu academia.");

        // ─── Soft delete ──────────────────────────────────────────────────────────
        var eraDefault = sede.EsDefault;
        sede.Activa    = false;
        sede.EsDefault = false;

        // ─── Si era la default, promover la primera sede restante ─────────────────
        if (eraDefault)
        {
            var nuevaDefault = await _db.SedeConfigs
                .Where(s => s.Activa && s.Id != request.Id)
                .OrderBy(s => s.Orden)
                .FirstOrDefaultAsync(cancellationToken);

            if (nuevaDefault is not null)
                nuevaDefault.EsDefault = true;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

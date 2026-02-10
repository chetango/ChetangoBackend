using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Commands.ToggleTipoPaqueteActivo;

public record ToggleTipoPaqueteActivoCommand(Guid IdTipoPaquete) : IRequest<Result<bool>>;

public class ToggleTipoPaqueteActivoCommandHandler : IRequestHandler<ToggleTipoPaqueteActivoCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public ToggleTipoPaqueteActivoCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(ToggleTipoPaqueteActivoCommand request, CancellationToken cancellationToken)
    {
        var tipoPaquete = await _db.Set<TipoPaquete>()
            .FirstOrDefaultAsync(tp => tp.Id == request.IdTipoPaquete, cancellationToken);

        if (tipoPaquete == null)
            return Result<bool>.Failure("Tipo de paquete no encontrado");

        // Toggle activo
        tipoPaquete.Activo = !tipoPaquete.Activo;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(tipoPaquete.Activo);
    }
}

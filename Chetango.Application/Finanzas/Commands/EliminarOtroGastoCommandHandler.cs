using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;

namespace Chetango.Application.Finanzas.Commands;

public class EliminarOtroGastoCommandHandler : IRequestHandler<EliminarOtroGastoCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public EliminarOtroGastoCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(EliminarOtroGastoCommand request, CancellationToken cancellationToken)
    {
        var otroGasto = await _db.OtrosGastos
            .FirstOrDefaultAsync(o => o.IdOtroGasto == request.IdOtroGasto, cancellationToken);

        if (otroGasto == null)
        {
            return Result<bool>.Failure("El gasto especificado no existe.");
        }

        // Soft delete
        otroGasto.Eliminado = true;
        otroGasto.FechaEliminacion = DateTime.Now;
        otroGasto.UsuarioEliminacion = request.EmailUsuarioEliminador ?? "Sistema";

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

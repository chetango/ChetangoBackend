using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;

namespace Chetango.Application.Finanzas.Commands;

public class EliminarOtroIngresoCommandHandler : IRequestHandler<EliminarOtroIngresoCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public EliminarOtroIngresoCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(EliminarOtroIngresoCommand request, CancellationToken cancellationToken)
    {
        var otroIngreso = await _db.OtrosIngresos
            .FirstOrDefaultAsync(o => o.IdOtroIngreso == request.IdOtroIngreso, cancellationToken);

        if (otroIngreso == null)
        {
            return Result<bool>.Failure("El ingreso especificado no existe.");
        }

        // Soft delete
        otroIngreso.Eliminado = true;
        otroIngreso.FechaEliminacion = DateTime.Now;
        otroIngreso.UsuarioEliminacion = request.EmailUsuarioEliminador ?? "Sistema";

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

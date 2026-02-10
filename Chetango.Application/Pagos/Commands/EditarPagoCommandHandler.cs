using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Application.Pagos.Commands;

public class EditarPagoCommandHandler : IRequestHandler<EditarPagoCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public EditarPagoCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(EditarPagoCommand request, CancellationToken cancellationToken)
    {
        // Validaciones inline
        if (request.MontoTotal <= 0)
        {
            return Result<Unit>.Failure("El monto total debe ser mayor a cero.");
        }

        // Buscar el pago
        var pago = await _db.Set<Pago>().FindAsync(new object[] { request.IdPago }, cancellationToken);
        if (pago == null)
        {
            return Result<Unit>.Failure("El pago especificado no existe.");
        }

        // Validar que el método de pago existe
        var metodoPago = await _db.Set<MetodoPago>().FindAsync(new object[] { request.IdMetodoPago }, cancellationToken);
        if (metodoPago == null)
        {
            return Result<Unit>.Failure("El método de pago especificado no existe.");
        }

        // Actualizar campos permitidos
        pago.MontoTotal = request.MontoTotal;
        pago.IdMetodoPago = request.IdMetodoPago;
        pago.Nota = request.Nota;
        pago.FechaModificacion = DateTimeHelper.Now;
        pago.UsuarioModificacion = "Sistema";

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

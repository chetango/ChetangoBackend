using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Commands.EditarPaquete;

// Command para editar un paquete existente
public record EditarPaqueteCommand(
    Guid IdPaquete,
    int ClasesDisponibles,
    DateTime FechaVencimiento
) : IRequest<Result<Unit>>;

// Handler
public class EditarPaqueteCommandHandler : IRequestHandler<EditarPaqueteCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public EditarPaqueteCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(EditarPaqueteCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener el paquete con tracking para actualizarlo
        var paquete = await _db.Set<Paquete>()
            .FirstOrDefaultAsync(p => p.IdPaquete == request.IdPaquete, cancellationToken);

        if (paquete is null)
            return Result<Unit>.Failure("El paquete especificado no existe.");

        // 2. Validar que las clases disponibles no sean menores a las ya usadas
        if (request.ClasesDisponibles < paquete.ClasesUsadas)
            return Result<Unit>.Failure($"Las clases disponibles ({request.ClasesDisponibles}) no pueden ser menores a las clases ya usadas ({paquete.ClasesUsadas}).");

        // 3. Actualizar los campos
        paquete.ClasesDisponibles = request.ClasesDisponibles;
        paquete.FechaVencimiento = request.FechaVencimiento.Date;
        paquete.FechaModificacion = DateTime.Now;
        paquete.UsuarioModificacion = "Sistema"; // TODO: Obtener del contexto de usuario

        // 4. Recalcular el estado según los nuevos valores
        if (paquete.ClasesUsadas >= paquete.ClasesDisponibles)
        {
            paquete.IdEstado = 4; // 4 = Agotado
        }
        else if (paquete.FechaVencimiento < DateTime.Today)
        {
            paquete.IdEstado = 2; // 2 = Vencido
        }
        else if (paquete.IdEstado != 3) // Si no está congelado, poner como activo
        {
            paquete.IdEstado = 1; // 1 = Activo
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

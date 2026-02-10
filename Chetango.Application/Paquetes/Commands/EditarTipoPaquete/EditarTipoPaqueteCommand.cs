using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Commands.EditarTipoPaquete;

public record EditarTipoPaqueteCommand(
    Guid IdTipoPaquete,
    string Nombre,
    int NumeroClases,
    decimal Precio,
    int DiasVigencia,
    string? Descripcion
) : IRequest<Result<Unit>>;

public class EditarTipoPaqueteCommandHandler : IRequestHandler<EditarTipoPaqueteCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public EditarTipoPaqueteCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(EditarTipoPaqueteCommand request, CancellationToken cancellationToken)
    {
        var tipoPaquete = await _db.Set<TipoPaquete>()
            .FirstOrDefaultAsync(tp => tp.Id == request.IdTipoPaquete, cancellationToken);

        if (tipoPaquete == null)
            return Result<Unit>.Failure("Tipo de paquete no encontrado");

        // Validar que no exista otro tipo con el mismo nombre
        var existente = await _db.Set<TipoPaquete>()
            .FirstOrDefaultAsync(tp => 
                tp.Id != request.IdTipoPaquete && 
                tp.Nombre.ToLower() == request.Nombre.ToLower(), 
                cancellationToken);

        if (existente != null)
            return Result<Unit>.Failure("Ya existe otro tipo de paquete con ese nombre");

        tipoPaquete.Nombre = request.Nombre;
        tipoPaquete.NumeroClases = request.NumeroClases;
        tipoPaquete.Precio = request.Precio;
        tipoPaquete.DiasVigencia = request.DiasVigencia;
        tipoPaquete.Descripcion = request.Descripcion;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

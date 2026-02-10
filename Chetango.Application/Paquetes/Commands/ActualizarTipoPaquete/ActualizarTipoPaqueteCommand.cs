using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Commands.ActualizarTipoPaquete;

public record ActualizarTipoPaqueteCommand(
    Guid IdTipoPaquete,
    string Nombre,
    int NumeroClases,
    decimal Precio,
    int DiasVigencia,
    string? Descripcion
) : IRequest<Result<Unit>>;

public class ActualizarTipoPaqueteCommandHandler : IRequestHandler<ActualizarTipoPaqueteCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public ActualizarTipoPaqueteCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(ActualizarTipoPaqueteCommand request, CancellationToken cancellationToken)
    {
        var tipoPaquete = await _db.Set<TipoPaquete>()
            .FirstOrDefaultAsync(tp => tp.Id == request.IdTipoPaquete, cancellationToken);

        if (tipoPaquete == null)
            return Result<Unit>.Failure("Tipo de paquete no encontrado");

        // Validar que no exista otro tipo con el mismo nombre
        var existente = await _db.Set<TipoPaquete>()
            .FirstOrDefaultAsync(tp => 
                tp.Nombre.ToLower() == request.Nombre.ToLower() && 
                tp.Id != request.IdTipoPaquete, 
                cancellationToken);

        if (existente != null)
            return Result<Unit>.Failure("Ya existe otro tipo de paquete con ese nombre");

        // Validaciones
        if (request.NumeroClases < 0)
            return Result<Unit>.Failure("El número de clases no puede ser negativo");

        if (request.Precio <= 0)
            return Result<Unit>.Failure("El precio debe ser mayor a cero");

        if (request.DiasVigencia <= 0)
            return Result<Unit>.Failure("Los días de vigencia deben ser mayor a cero");

        // Actualizar
        tipoPaquete.Nombre = request.Nombre.Trim();
        tipoPaquete.NumeroClases = request.NumeroClases;
        tipoPaquete.Precio = request.Precio;
        tipoPaquete.DiasVigencia = request.DiasVigencia;
        tipoPaquete.Descripcion = request.Descripcion?.Trim();

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

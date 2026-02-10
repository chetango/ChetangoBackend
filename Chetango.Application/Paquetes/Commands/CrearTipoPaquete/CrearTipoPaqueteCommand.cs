using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Commands.CrearTipoPaquete;

public record CrearTipoPaqueteCommand(
    string Nombre,
    int NumeroClases,
    decimal Precio,
    int DiasVigencia,
    string? Descripcion
) : IRequest<Result<Guid>>;

public class CrearTipoPaqueteCommandHandler : IRequestHandler<CrearTipoPaqueteCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public CrearTipoPaqueteCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CrearTipoPaqueteCommand request, CancellationToken cancellationToken)
    {
        // Validar que no exista un tipo con el mismo nombre
        var existente = await _db.Set<TipoPaquete>()
            .FirstOrDefaultAsync(tp => tp.Nombre.ToLower() == request.Nombre.ToLower(), cancellationToken);

        if (existente != null)
            return Result<Guid>.Failure("Ya existe un tipo de paquete con ese nombre");

        var tipoPaquete = new TipoPaquete
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            NumeroClases = request.NumeroClases,
            Precio = request.Precio,
            DiasVigencia = request.DiasVigencia,
            Descripcion = request.Descripcion,
            Activo = true
        };

        _db.Set<TipoPaquete>().Add(tipoPaquete);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(tipoPaquete.Id);
    }
}

using Chetango.Application.Common;
using Chetango.Application.Common.Interfaces;
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
    private readonly IAppDbContext   _db;
    private readonly ITenantProvider _tenantProvider;

    public CrearTipoPaqueteCommandHandler(IAppDbContext db, ITenantProvider tenantProvider)
    {
        _db             = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<Guid>> Handle(CrearTipoPaqueteCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (tenantId is null)
            return Result<Guid>.Failure("No se pudo resolver el tenant actual.");

        // El query filter ya restringe por TenantId → la unicidad es por tenant
        var existente = await _db.Set<TipoPaquete>()
            .FirstOrDefaultAsync(tp => tp.Nombre.ToLower() == request.Nombre.ToLower(), cancellationToken);

        if (existente != null)
            return Result<Guid>.Failure("Ya existe un tipo de paquete con ese nombre en tu academia.");

        var tipoPaquete = new TipoPaquete
        {
            Id           = Guid.NewGuid(),
            TenantId     = tenantId.Value,
            Nombre       = request.Nombre,
            NumeroClases = request.NumeroClases,
            Precio       = request.Precio,
            DiasVigencia = request.DiasVigencia,
            Descripcion  = request.Descripcion,
            Activo       = true
        };

        _db.Set<TipoPaquete>().Add(tipoPaquete);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(tipoPaquete.Id);
    }
}

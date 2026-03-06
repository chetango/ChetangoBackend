using Chetango.Application.Common;
using Chetango.Application.Common.Interfaces;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Commands.CrearTipoClase;

/// <summary>
/// Crea un nuevo tipo de clase para el tenant actual.
/// El nombre debe ser único dentro del tenant (otros tenants pueden tener el mismo nombre).
/// </summary>
public record CrearTipoClaseCommand(string Nombre) : IRequest<Result<Guid>>;

public class CrearTipoClaseCommandHandler : IRequestHandler<CrearTipoClaseCommand, Result<Guid>>
{
    private readonly IAppDbContext   _db;
    private readonly ITenantProvider _tenantProvider;

    public CrearTipoClaseCommandHandler(IAppDbContext db, ITenantProvider tenantProvider)
    {
        _db             = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<Guid>> Handle(CrearTipoClaseCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (tenantId is null)
            return Result<Guid>.Failure("No se pudo resolver el tenant actual.");

        // El query filter ya filtra por TenantId → unicidad es por tenant
        var existente = await _db.Set<TipoClase>()
            .FirstOrDefaultAsync(tc => tc.Nombre.ToLower() == request.Nombre.ToLower(), cancellationToken);

        if (existente != null)
            return Result<Guid>.Failure("Ya existe un tipo de clase con ese nombre en tu academia.");

        var tipoClase = new TipoClase
        {
            Id       = Guid.NewGuid(),
            TenantId = tenantId.Value,
            Nombre   = request.Nombre.Trim(),
        };

        _db.Set<TipoClase>().Add(tipoClase);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(tipoClase.Id);
    }
}

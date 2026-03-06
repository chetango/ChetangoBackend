using Chetango.Application.Common;
using Chetango.Application.Sedes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Sedes.Commands.UpdateSede;

/// <summary>
/// Comando para actualizar el nombre y/o el orden de una sede existente.
/// El SedeValor es inmutable: los registros históricos (Asistencias, Pagos, etc.)
/// lo usan como discriminador y no deben verse afectados.
/// </summary>
public record UpdateSedeCommand(
    Guid   Id,
    string Nombre,
    int    Orden) : IRequest<Result<SedeConfigDTO>>;

public class UpdateSedeCommandHandler : IRequestHandler<UpdateSedeCommand, Result<SedeConfigDTO>>
{
    private readonly IAppDbContext _db;

    public UpdateSedeCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<SedeConfigDTO>> Handle(UpdateSedeCommand request, CancellationToken cancellationToken)
    {
        // El query filter aplica TenantId automáticamente → protege cross-tenant
        var sede = await _db.SedeConfigs
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.Activa, cancellationToken);

        if (sede is null)
            return Result<SedeConfigDTO>.Failure("La sede no existe o no pertenece a tu academia.");

        sede.Nombre = request.Nombre.Trim();
        sede.Orden  = request.Orden;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<SedeConfigDTO>.Success(new SedeConfigDTO
        {
            Id        = sede.Id,
            SedeValor = sede.SedeValor,
            Nombre    = sede.Nombre,
            Activa    = sede.Activa,
            EsDefault = sede.EsDefault,
            Orden     = sede.Orden,
        });
    }
}

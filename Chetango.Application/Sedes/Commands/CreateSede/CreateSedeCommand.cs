using Chetango.Application.Common;
using Chetango.Application.Common.Interfaces;
using Chetango.Application.Sedes.DTOs;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Sedes.Commands.CreateSede;

/// <summary>
/// Comando para crear una nueva sede en el tenant actual.
/// Valida que no se exceda el límite MaxSedes del plan de suscripción.
/// El SedeValor es auto-asignado como el siguiente entero disponible (nunca se reutiliza).
/// </summary>
public record CreateSedeCommand(
    string Nombre,
    int?   Orden = null) : IRequest<Result<SedeConfigDTO>>;

public class CreateSedeCommandHandler : IRequestHandler<CreateSedeCommand, Result<SedeConfigDTO>>
{
    private readonly IAppDbContext    _db;
    private readonly ITenantProvider  _tenantProvider;

    public CreateSedeCommandHandler(IAppDbContext db, ITenantProvider tenantProvider)
    {
        _db             = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<SedeConfigDTO>> Handle(CreateSedeCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (tenantId is null)
            return Result<SedeConfigDTO>.Failure("No se pudo resolver el tenant actual.");

        // ─── 1. Verificar límite del plan ────────────────────────────────────────
        // IgnoreQueryFilters porque Tenants no está filtrado por TenantId,
        // pero lo usamos para ser explícitos y evitar problemas futuros.
        var tenant = await _db.Tenants
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tenantId.Value, cancellationToken);

        if (tenant is null)
            return Result<SedeConfigDTO>.Failure("Tenant no encontrado.");

        // El query filter de SedeConfig aplica TenantId automáticamente
        var sedesActivas = await _db.SedeConfigs
            .CountAsync(s => s.Activa, cancellationToken);

        if (sedesActivas >= tenant.MaxSedes)
            return Result<SedeConfigDTO>.Failure(
                $"Has alcanzado el límite de {tenant.MaxSedes} sede(s) incluida(s) en el plan '{tenant.Plan}'. " +
                "Actualiza tu suscripción para agregar más sedes.");

        // ─── 2. Calcular siguiente SedeValor (nunca reutilizar valores históricos) ─
        // Se usa IgnoreQueryFilters + filtro explícito de TenantId para incluir sedes
        // inactivas y garantizar que no haya colisiones con datos históricos.
        var maxSedeValor = await _db.SedeConfigs
            .IgnoreQueryFilters()
            .Where(s => s.TenantId == tenantId.Value)
            .MaxAsync(s => (int?)s.SedeValor, cancellationToken) ?? 0;

        var nextSedeValor = maxSedeValor + 1;

        // ─── 3. Determinar orden: si no se provee, va al final ────────────────────
        var orden = request.Orden ?? (sedesActivas + 1);

        // ─── 4. Crear la sede ─────────────────────────────────────────────────────
        var nuevaSede = new SedeConfig
        {
            Id            = Guid.NewGuid(),
            TenantId      = tenantId.Value,
            SedeValor     = nextSedeValor,
            Nombre        = request.Nombre.Trim(),
            Activa        = true,
            EsDefault     = sedesActivas == 0, // Primera sede → es la default
            Orden         = orden,
            FechaCreacion = DateTime.UtcNow,
        };

        _db.SedeConfigs.Add(nuevaSede);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<SedeConfigDTO>.Success(new SedeConfigDTO
        {
            Id        = nuevaSede.Id,
            SedeValor = nuevaSede.SedeValor,
            Nombre    = nuevaSede.Nombre,
            Activa    = nuevaSede.Activa,
            EsDefault = nuevaSede.EsDefault,
            Orden     = nuevaSede.Orden,
        });
    }
}

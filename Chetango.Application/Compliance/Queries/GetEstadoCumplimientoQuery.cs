using Chetango.Application.Common;
using Chetango.Application.Compliance.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Compliance.Queries;

/// <summary>
/// Devuelve el estado completo de cumplimiento del tenant actual:
/// - si completó el onboarding
/// - si requiere reaceptación de algún documento crítico
/// - qué versiones activas aún no ha aceptado
/// </summary>
public record GetEstadoCumplimientoQuery(Guid TenantId) : IRequest<Result<EstadoCumplimientoDto>>;

public class GetEstadoCumplimientoQueryHandler
    : IRequestHandler<GetEstadoCumplimientoQuery, Result<EstadoCumplimientoDto>>
{
    private readonly IAppDbContext _db;

    public GetEstadoCumplimientoQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<EstadoCumplimientoDto>> Handle(
        GetEstadoCumplimientoQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await _db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant is null)
            return Result<EstadoCumplimientoDto>.Failure("Academia no encontrada.");

        // Versiones activas de documentos obligatorios que debe aceptar el admin
        var versionesActivas = await _db.VersionesDocumentoLegal
            .AsNoTracking()
            .Include(v => v.DocumentoLegal)
            .Where(v => v.Activa
                     && v.DocumentoLegal.Activo
                     && v.DocumentoLegal.Destinatario == "Admin")
            .ToListAsync(cancellationToken);

        // Versiones que el tenant YA aceptó
        var versionesAceptadas = await _db.AceptacionesDocumento
            .AsNoTracking()
            .Where(a => a.TenantId == request.TenantId)
            .Select(a => a.VersionDocumentoLegalId)
            .ToListAsync(cancellationToken);

        // Pendientes = activas y obligatorias que aún no están aceptadas
        var pendientes = versionesActivas
            .Where(v => v.DocumentoLegal.EsObligatorio
                     && !versionesAceptadas.Contains(v.Id))
            .Select(v => new DocumentoPendienteDto(
                v.Id,
                v.DocumentoLegal.Codigo,
                v.DocumentoLegal.Nombre,
                v.DocumentoLegal.Descripcion,
                v.NumeroVersion,
                v.UrlDocumento,
                v.DocumentoLegal.EsObligatorio))
            .ToList();

        return Result<EstadoCumplimientoDto>.Success(new EstadoCumplimientoDto(
            tenant.Id,
            tenant.Nombre,
            tenant.OnboardingCompletado,
            tenant.RequiereReaceptacion,
            tenant.FechaActivacion,
            pendientes));
    }
}

using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Compliance.Commands;

/// <summary>
/// Registra la aceptación de uno o más documentos legales por parte del admin de una academia.
/// Si tras la aceptación todos los documentos obligatorios están cubiertos,
/// se marca el onboarding como completado automáticamente.
/// </summary>
public record AceptarDocumentosCommand(
    Guid TenantId,
    Guid IdUsuario,
    List<Guid> VersionesDocumentoLegalIds,  // puede aceptar varios a la vez
    string IpOrigen,
    string? UserAgent,
    string Contexto   // "Onboarding" | "Reacepacion" | "Manual"
) : IRequest<Result<AceptarDocumentosResult>>;

public record AceptarDocumentosResult(
    bool OnboardingCompletado,
    int DocumentosRegistrados
);

public class AceptarDocumentosCommandHandler
    : IRequestHandler<AceptarDocumentosCommand, Result<AceptarDocumentosResult>>
{
    private readonly IAppDbContext _db;

    public AceptarDocumentosCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<AceptarDocumentosResult>> Handle(
        AceptarDocumentosCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validar tenant
        var tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant is null)
            return Result<AceptarDocumentosResult>.Failure("Academia no encontrada.");

        // 2. Validar que todas las versiones existen y están activas
        var versiones = await _db.VersionesDocumentoLegal
            .Include(v => v.DocumentoLegal)
            .Where(v => request.VersionesDocumentoLegalIds.Contains(v.Id) && v.Activa)
            .ToListAsync(cancellationToken);

        if (versiones.Count != request.VersionesDocumentoLegalIds.Distinct().Count())
            return Result<AceptarDocumentosResult>.Failure(
                "Una o más versiones de documento no existen o no están activas.");

        // 3. Identificar cuáles ya fueron aceptadas por este tenant (evitar duplicados)
        var yaAceptadas = await _db.AceptacionesDocumento
            .Where(a => a.TenantId == request.TenantId
                     && request.VersionesDocumentoLegalIds.Contains(a.VersionDocumentoLegalId))
            .Select(a => a.VersionDocumentoLegalId)
            .ToListAsync(cancellationToken);

        // 4. Insertar solo las que aún no están aceptadas
        var nuevas = versiones
            .Where(v => !yaAceptadas.Contains(v.Id))
            .ToList();

        if (nuevas.Count == 0 && request.Contexto == "Onboarding")
            return Result<AceptarDocumentosResult>.Failure(
                "Todos los documentos indicados ya fueron aceptados anteriormente.");

        foreach (var version in nuevas)
        {
            _db.AceptacionesDocumento.Add(new AceptacionDocumento
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                IdUsuario = request.IdUsuario,
                VersionDocumentoLegalId = version.Id,
                FechaAceptacion = DateTime.UtcNow,
                IpOrigen = request.IpOrigen,
                UserAgent = request.UserAgent,
                Contexto = request.Contexto
            });
        }

        // 5. Verificar si todos los documentos obligatorios para Admin ya están cubiertos
        var idsVersionesObligatoriasActivas = await _db.VersionesDocumentoLegal
            .Where(v => v.Activa
                     && v.DocumentoLegal.Activo
                     && v.DocumentoLegal.EsObligatorio
                     && v.DocumentoLegal.Destinatario == "Admin")
            .Select(v => v.Id)
            .ToListAsync(cancellationToken);

        // Combinar las ya persistidas + las que vamos a insertar ahora
        var todasAceptadas = yaAceptadas
            .Concat(nuevas.Select(v => v.Id))
            .Distinct()
            .ToList();

        bool todasCubiertas = idsVersionesObligatoriasActivas
            .All(id => todasAceptadas.Contains(id));

        if (todasCubiertas && !tenant.OnboardingCompletado)
        {
            tenant.OnboardingCompletado = true;
            tenant.FechaActivacion = DateTime.UtcNow;
            tenant.RequiereReaceptacion = false;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<AceptarDocumentosResult>.Success(
            new AceptarDocumentosResult(tenant.OnboardingCompletado, nuevas.Count));
    }
}

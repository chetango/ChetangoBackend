using Chetango.Application.Common;
using Chetango.Application.Compliance.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Compliance.Queries;

/// <summary>
/// Devuelve el historial de aceptaciones de documentos legales de un tenant.
/// Útil para el panel de administración y para auditorías.
/// </summary>
public record GetHistorialAceptacionesQuery(Guid TenantId) : IRequest<Result<List<AceptacionDocumentoDto>>>;

public class GetHistorialAceptacionesQueryHandler
    : IRequestHandler<GetHistorialAceptacionesQuery, Result<List<AceptacionDocumentoDto>>>
{
    private readonly IAppDbContext _db;

    public GetHistorialAceptacionesQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<List<AceptacionDocumentoDto>>> Handle(
        GetHistorialAceptacionesQuery request,
        CancellationToken cancellationToken)
    {
        var historial = await _db.AceptacionesDocumento
            .AsNoTracking()
            .Include(a => a.VersionDocumentoLegal)
                .ThenInclude(v => v.DocumentoLegal)
            .Where(a => a.TenantId == request.TenantId)
            .OrderByDescending(a => a.FechaAceptacion)
            .Select(a => new AceptacionDocumentoDto(
                a.Id,
                a.VersionDocumentoLegal.DocumentoLegal.Codigo,
                a.VersionDocumentoLegal.DocumentoLegal.Nombre,
                a.VersionDocumentoLegal.NumeroVersion,
                a.FechaAceptacion,
                a.IpOrigen,
                a.Contexto))
            .ToListAsync(cancellationToken);

        return Result<List<AceptacionDocumentoDto>>.Success(historial);
    }
}

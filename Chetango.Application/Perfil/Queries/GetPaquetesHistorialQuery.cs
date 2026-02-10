// ============================================
// GET PAQUETES HISTORIAL QUERY
// ============================================

using Chetango.Application.Common;
using Chetango.Application.Perfil.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Perfil.Queries;

public record GetPaquetesHistorialQuery(Guid IdAlumno) : IRequest<Result<List<PaqueteHistorialDto>>>;

public class GetPaquetesHistorialQueryHandler : IRequestHandler<GetPaquetesHistorialQuery, Result<List<PaqueteHistorialDto>>>
{
    private readonly IAppDbContext _db;

    public GetPaquetesHistorialQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<List<PaqueteHistorialDto>>> Handle(GetPaquetesHistorialQuery request, CancellationToken cancellationToken)
    {
        var paquetes = await _db.Paquetes
            .Include(p => p.TipoPaquete)
            .Include(p => p.Estado)
            .Where(p => p.IdAlumno == request.IdAlumno)
            .OrderByDescending(p => p.FechaCreacion)
            .Select(p => new PaqueteHistorialDto
            {
                IdPaquete = p.IdPaquete,
                Tipo = p.TipoPaquete.Nombre,
                FechaCompra = p.FechaCreacion,
                FechaVencimiento = p.FechaVencimiento,
                ClasesTotales = p.ClasesDisponibles,
                ClasesUsadas = p.ClasesUsadas,
                Precio = p.ValorPaquete,
                Estado = p.Estado.Nombre.ToLower()
            })
            .ToListAsync(cancellationToken);

        return Result<List<PaqueteHistorialDto>>.Success(paquetes);
    }
}

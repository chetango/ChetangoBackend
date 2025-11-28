using Chetango.Application.Asistencias.Admin.DTOs;
using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Admin.Queries.GetClasesDelDiaAdmin;

public sealed class GetClasesDelDiaAdminQueryHandler
    : IRequestHandler<GetClasesDelDiaAdminQuery, Result<ClasesDelDiaAdminDto>>
{
    private readonly IAppDbContext _db;

    public GetClasesDelDiaAdminQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ClasesDelDiaAdminDto>> Handle(
        GetClasesDelDiaAdminQuery request,
        CancellationToken cancellationToken)
    {
        // Cargamos las clases exactamente en la fecha indicada (ignorando hora)
        var fechaInicio = request.Fecha.ToDateTime(TimeOnly.MinValue);
        var fechaFin = request.Fecha.ToDateTime(TimeOnly.MaxValue);

        var clasesQuery = _db.Set<Clase>()
            .AsNoTracking()
            .Where(c => c.Fecha >= fechaInicio && c.Fecha <= fechaFin)
            .Include(c => c.TipoClase)
            .Include(c => c.ProfesorPrincipal)
                .ThenInclude(p => p.Usuario);

        var clases = await clasesQuery
            .OrderBy(c => c.HoraInicio)
            .ThenBy(c => c.HoraFin)
            .Select(c => new ClaseDelDiaAdminItemDto
            {
                IdClase = c.IdClase,
                Nombre = c.TipoClase.Nombre,
                HoraInicio = TimeOnly.FromTimeSpan(c.HoraInicio),
                HoraFin = TimeOnly.FromTimeSpan(c.HoraFin),
                ProfesorPrincipal = c.ProfesorPrincipal.Usuario.NombreUsuario
            })
            .ToListAsync(cancellationToken);

        var dto = new ClasesDelDiaAdminDto
        {
            Fecha = request.Fecha,
            Clases = clases
        };

        return Result<ClasesDelDiaAdminDto>.Success(dto);
    }
}

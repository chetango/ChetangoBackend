using Chetango.Application.Asistencias.Admin.DTOs;
using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Admin.Queries.GetDiasConClasesAdmin;

public sealed class GetDiasConClasesAdminQueryHandler
    : IRequestHandler<GetDiasConClasesAdminQuery, Result<DiasConClasesAdminDto>>
{
    private readonly IAppDbContext _db;

    public GetDiasConClasesAdminQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<DiasConClasesAdminDto>> Handle(
        GetDiasConClasesAdminQuery request,
        CancellationToken cancellationToken)
    {
        // Usamos la fecha actual del servidor (sin hora)
        var hoyDateTime = DateTime.UtcNow.Date;
        var hoy = DateOnly.FromDateTime(hoyDateTime);
        var desde = hoy.AddDays(-7);
        var hasta = hoy;

        // Proyectar solo fechas de clases dentro del rango y agrupar/distinct en servidor
        var diasConClases = await _db.Set<Clase>()
            .AsNoTracking()
            .Where(c => c.Fecha >= desde.ToDateTime(TimeOnly.MinValue)
                     && c.Fecha <= hasta.ToDateTime(TimeOnly.MaxValue))
            .Select(c => DateOnly.FromDateTime(c.Fecha))
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync(cancellationToken);

        var dto = new DiasConClasesAdminDto
        {
            Hoy = hoy,
            Desde = desde,
            Hasta = hasta,
            DiasConClases = diasConClases
        };

        return Result<DiasConClasesAdminDto>.Success(dto);
    }
}

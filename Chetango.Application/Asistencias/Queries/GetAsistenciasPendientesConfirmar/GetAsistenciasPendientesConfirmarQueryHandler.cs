using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Application.Asistencias.Queries.GetAsistenciasPendientesConfirmar;

public class GetAsistenciasPendientesConfirmarQueryHandler 
    : IRequestHandler<GetAsistenciasPendientesConfirmarQuery, Result<IReadOnlyList<AsistenciaPendienteDto>>>
{
    private readonly IAppDbContext _db;

    public GetAsistenciasPendientesConfirmarQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<AsistenciaPendienteDto>>> Handle(
        GetAsistenciasPendientesConfirmarQuery request, 
        CancellationToken cancellationToken)
    {
        // Obtener asistencias pendientes de confirmar:
        // - Estado = "Presente" (ya marcadas por profesor/admin)
        // - Confirmado = false (alumno aún no ha confirmado)
        var asistenciasPendientes = await _db.Set<Asistencia>()
            .Include(a => a.Clase)
                .ThenInclude(c => c.TipoClase)
            .Include(a => a.Clase)
                .ThenInclude(c => c.Profesores)
                    .ThenInclude(cp => cp.Profesor)
                        .ThenInclude(p => p.Usuario)
            .Include(a => a.Estado)
            .Where(a => a.IdAlumno == request.IdAlumno 
                     && a.Estado.Nombre == "Presente" 
                     && !a.Confirmado)
            .OrderByDescending(a => a.Clase.Fecha) // Más recientes primero
            .ToListAsync(cancellationToken);

        var dtos = asistenciasPendientes.Select(a =>
        {
            var profesores = a.Clase.Profesores
                .Select(cp => cp.Profesor.Usuario.NombreUsuario)
                .ToList();

            return new AsistenciaPendienteDto(
                IdAsistencia: a.IdAsistencia,
                IdClase: a.IdClase,
                NombreClase: a.Clase.TipoClase.Nombre,
                FechaClase: a.Clase.Fecha,
                HoraInicio: $"{a.Clase.HoraInicio.Hours:D2}:{a.Clase.HoraInicio.Minutes:D2}",
                HoraFin: $"{a.Clase.HoraFin.Hours:D2}:{a.Clase.HoraFin.Minutes:D2}",
                Profesores: profesores
            );
        }).ToList();

        return Result<IReadOnlyList<AsistenciaPendienteDto>>.Success(dtos);
    }
}

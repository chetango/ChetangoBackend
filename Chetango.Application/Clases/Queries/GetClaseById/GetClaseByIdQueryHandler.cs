using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Queries.GetClaseById;

public class GetClaseByIdQueryHandler : IRequestHandler<GetClaseByIdQuery, Result<ClaseDetalleDTO>>
{
    private readonly IAppDbContext _db;

    public GetClaseByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<ClaseDetalleDTO>> Handle(GetClaseByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscar la clase con todas sus relaciones
        var clase = await _db.Set<Chetango.Domain.Entities.Clase>()
            .Include(c => c.TipoClase)
            .Include(c => c.ProfesorPrincipal)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Monitores)
                .ThenInclude(m => m.Profesor)
                    .ThenInclude(p => p.Usuario)
            .Include(c => c.Asistencias)
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase is null)
            return Result<ClaseDetalleDTO>.Failure("La clase especificada no existe.");

        // 2. Validación de ownership: Profesor solo puede ver sus clases
        // (Los alumnos pueden ver clases donde están inscritos, pero eso requeriría otra validación)
        if (!request.EsAdmin)
        {
            if (clase.ProfesorPrincipal.IdUsuario.ToString() != request.IdUsuarioActual)
            {
                // Verificar si es monitor de la clase
                var esMonitor = clase.Monitores.Any(m => m.Profesor.IdUsuario.ToString() == request.IdUsuarioActual);
                if (!esMonitor)
                    return Result<ClaseDetalleDTO>.Failure("No tienes permiso para ver esta clase.");
            }
        }

        // 3. Proyectar a DTO
        var dto = new ClaseDetalleDTO(
            clase.IdClase,
            clase.Fecha,
            clase.HoraInicio,
            clase.HoraFin,
            clase.TipoClase.Nombre,
            clase.IdProfesorPrincipal,
            clase.ProfesorPrincipal.NombreCompleto,
            clase.CupoMaximo,
            clase.Observaciones,
            clase.Asistencias.Count,
            clase.Monitores.Select(m => new MonitorClaseDTO(
                m.IdProfesor,
                m.Profesor.NombreCompleto
            )).ToList()
        );

        return Result<ClaseDetalleDTO>.Success(dto);
    }
}

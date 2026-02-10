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
            .Include(c => c.Profesores)
                .ThenInclude(cp => cp.Profesor)
                    .ThenInclude(p => p.Usuario)
            .Include(c => c.Profesores)
                .ThenInclude(cp => cp.RolEnClase)
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase is null)
            return Result<ClaseDetalleDTO>.Failure("La clase especificada no existe.");

        // 2. Validación de ownership: Profesor solo puede ver clases donde esté asignado
        if (!request.EsAdmin)
        {
            if (string.IsNullOrWhiteSpace(request.IdUsuarioActual))
                return Result<ClaseDetalleDTO>.Failure("No se pudo identificar al usuario.");
            
            var estaAsignado = clase.Profesores.Any(cp => cp.Profesor.IdUsuario.ToString() == request.IdUsuarioActual);
            if (!estaAsignado)
                return Result<ClaseDetalleDTO>.Failure("No tienes permiso para ver esta clase.");
        }

        // 3. Proyectar a DTO con todos los profesores
        var profesoresDto = clase.Profesores
            .Select(cp => new ProfesorClaseDTO(
                cp.IdProfesor,
                cp.Profesor.NombreCompleto,
                cp.RolEnClase.Nombre
            ))
            .ToList();
        
        // Para retrocompatibilidad, obtener el primer profesor principal
        var primerPrincipal = clase.Profesores
            .FirstOrDefault(cp => cp.RolEnClase.Nombre == "Principal");
        
        var dto = new ClaseDetalleDTO(
            clase.IdClase,
            clase.Fecha,
            clase.HoraInicio,
            clase.HoraFin,
            clase.TipoClase.Nombre,
            primerPrincipal?.IdProfesor, // Retrocompatibilidad
            primerPrincipal?.Profesor.NombreCompleto ?? "Sin profesor", // Retrocompatibilidad
            clase.CupoMaximo,
            clase.Observaciones,
            clase.Asistencias.Count,
            clase.Monitores.Select(m => new MonitorClaseDTO( // Retrocompatibilidad
                m.IdProfesor,
                m.Profesor.NombreCompleto
            )).ToList(),
            profesoresDto, // NUEVO: Lista completa con roles
            clase.Estado
        );

        return Result<ClaseDetalleDTO>.Success(dto);
    }
}

using Chetango.Application.Common;
using Chetango.Application.Paquetes.DTOs;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Queries.GetMisPaquetes;

public class GetMisPaquetesQueryHandler : IRequestHandler<GetMisPaquetesQuery, Result<List<PaqueteAlumnoDTO>>>
{
    private readonly IAppDbContext _db;

    public GetMisPaquetesQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<List<PaqueteAlumnoDTO>>> Handle(GetMisPaquetesQuery request, CancellationToken cancellationToken)
    {
        // 1. Buscar el alumno por correo electrónico del usuario
        var alumno = await _db.Set<Alumno>()
            .Include(a => a.Usuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Usuario.Correo.ToLower() == request.CorreoUsuario.ToLower(), cancellationToken);

        if (alumno is null)
            return Result<List<PaqueteAlumnoDTO>>.Failure("No se encontró un perfil de alumno asociado a tu usuario.");

        // 2. Construir query de paquetes
        var query = _db.Set<Paquete>()
            .Include(p => p.TipoPaquete)
            .Include(p => p.Congelaciones)
            .Where(p => p.IdAlumno == alumno.IdAlumno);

        // 3. Aplicar filtros opcionales
        if (request.Estado.HasValue)
            query = query.Where(p => p.IdEstado == request.Estado.Value);

        if (request.IdTipoPaquete.HasValue)
            query = query.Where(p => p.IdTipoPaquete == request.IdTipoPaquete.Value);

        // 4. Obtener paquetes
        var paquetes = await query
            .AsNoTracking()
            .OrderByDescending(p => p.FechaActivacion)
            .ToListAsync(cancellationToken);

        // 5. Para cada paquete, obtener su historial de asistencias
        var result = new List<PaqueteAlumnoDTO>();

        foreach (var paquete in paquetes)
        {
            // Obtener historial de asistencias del paquete
            var asistencias = await _db.Set<Asistencia>()
                .Include(a => a.Clase)
                    .ThenInclude(c => c.TipoClase)
                .Where(a => a.IdPaqueteUsado == paquete.IdPaquete)
                .OrderByDescending(a => a.Clase.Fecha)
                .AsNoTracking()
                .Select(a => new AsistenciaHistorialDTO(
                    a.IdAsistencia,
                    a.Clase.Fecha,
                    a.Clase.TipoClase.Nombre,
                    a.Clase.HoraInicio.ToString(@"hh\:mm"),
                    a.Clase.HoraFin.ToString(@"hh\:mm"),
                    // Resultado: 1=Presente, 2=Ausente, 3=Justificada
                    a.IdEstado == 1 ? "Asistida" : a.IdEstado == 2 ? "Ausente" : "Justificada",
                    // Impacto: si tiene paquete usado significa que se descontó
                    a.IdPaqueteUsado != Guid.Empty ? "Descontada" : "No descontada",
                    a.Observacion
                ))
                .ToListAsync(cancellationToken);

            // Obtener congelación activa si existe
            var congelacionActiva = paquete.Congelaciones
                .Where(c => c.FechaInicio <= DateTime.Today && c.FechaFin >= DateTime.Today)
                .OrderByDescending(c => c.FechaInicio)
                .Select(c => new CongelacionDetalleDTO(
                    c.IdCongelacion,
                    c.FechaInicio,
                    c.FechaFin,
                    (c.FechaFin - c.FechaInicio).Days
                ))
                .FirstOrDefault();

            var dto = new PaqueteAlumnoDTO(
                paquete.IdPaquete,
                paquete.IdAlumno,
                alumno.Usuario.NombreUsuario,
                alumno.Usuario.NumeroDocumento,
                paquete.TipoPaquete.Nombre,
                paquete.ClasesDisponibles,
                paquete.ClasesUsadas,
                paquete.ClasesDisponibles - paquete.ClasesUsadas,
                paquete.FechaActivacion,
                paquete.FechaVencimiento,
                paquete.ValorPaquete,
                // Estados: 1=Activo, 2=Vencido, 3=Congelado
                // Si está completado (clases usadas == disponibles), mostrar "Completado"
                paquete.IdEstado == 1 ? "Activo" : 
                paquete.IdEstado == 3 ? "Congelado" :
                paquete.ClasesUsadas >= paquete.ClasesDisponibles ? "Completado" : "Vencido",
                paquete.FechaVencimiento < DateTime.Today,
                (paquete.ClasesDisponibles - paquete.ClasesUsadas) > 0,
                congelacionActiva,
                asistencias
            );

            result.Add(dto);
        }

        return Result<List<PaqueteAlumnoDTO>>.Success(result);
    }
}

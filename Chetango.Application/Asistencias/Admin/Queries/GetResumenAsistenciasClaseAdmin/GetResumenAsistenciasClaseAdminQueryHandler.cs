using Chetango.Application.Asistencias.Admin.DTOs;
using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Admin.Queries.GetResumenAsistenciasClaseAdmin;

public sealed class GetResumenAsistenciasClaseAdminQueryHandler
    : IRequestHandler<GetResumenAsistenciasClaseAdminQuery, Result<ResumenAsistenciasClaseAdminDto>>
{
    private readonly IAppDbContext _db;

    public GetResumenAsistenciasClaseAdminQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ResumenAsistenciasClaseAdminDto>> Handle(
        GetResumenAsistenciasClaseAdminQuery request,
        CancellationToken cancellationToken)
    {
        // Cargamos la clase con profesor y asistencias + alumno + paquete
        var clase = await _db.Set<Clase>()
            .AsNoTracking()
            .Include(c => c.TipoClase)
            .Include(c => c.ProfesorPrincipal)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Asistencias)
                .ThenInclude(a => a.Alumno)
                    .ThenInclude(a => a.Usuario)
            .Include(c => c.Asistencias)
                .ThenInclude(a => a.Estado)  // Incluir el estado de la asistencia
            .Include(c => c.Asistencias)
                .ThenInclude(a => a.PaqueteUsado!) // Nullable: puede ser null para clases de cortesía
                    .ThenInclude(p => p.Estado)
            .Include(c => c.Asistencias)
                .ThenInclude(a => a.PaqueteUsado!) // Nullable: puede ser null para clases de cortesía
                    .ThenInclude(p => p.TipoPaquete)
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase is null)
        {
            return Result<ResumenAsistenciasClaseAdminDto>.Failure("Clase no encontrada");
        }

        // Obtener todos los alumnos activos
        var alumnosActivos = await _db.Set<Alumno>()
            .AsNoTracking()
            .Include(a => a.Usuario)
                .ThenInclude(u => u.Estado)
            .Where(a => a.Usuario.Estado.Nombre == "Activo")
            .ToListAsync(cancellationToken);

        // Obtener todos los paquetes activos de esos alumnos con información del pago
        var idsAlumnos = alumnosActivos.Select(a => a.IdAlumno).ToList();
        var paquetesActivos = await _db.Set<Paquete>()
            .AsNoTracking()
            .Include(p => p.Estado)
            .Include(p => p.TipoPaquete)
            .Include(p => p.Alumno)
                .ThenInclude(a => a.Usuario)
            .Where(p => idsAlumnos.Contains(p.IdAlumno) && p.Estado.Nombre == "Activo")
            .ToListAsync(cancellationToken);

        // Agrupar paquetes por IdPago para identificar paquetes compartidos
        var paquetesPorPago = paquetesActivos
            .Where(p => p.IdPago.HasValue)
            .GroupBy(p => p.IdPago!.Value)
            .Where(g => g.Count() > 1) // Solo paquetes compartidos (más de 1 alumno)
            .ToDictionary(g => g.Key, g => g.ToList());

        var alumnos = new List<AlumnoEnClaseAdminDto>();
        var alumnosConAsistencia = new HashSet<Guid>(); // Alumnos que ya tienen asistencia registrada

        // Primero, agregar alumnos que ya tienen asistencia registrada (usar el paquete que usaron)
        foreach (var asistencia in clase.Asistencias)
        {
            alumnosConAsistencia.Add(asistencia.IdAlumno);
            
            var alumno = alumnosActivos.FirstOrDefault(a => a.IdAlumno == asistencia.IdAlumno);
            if (alumno == null) continue;

            var paquete = asistencia.PaqueteUsado;

            // Verificar si este paquete es compartido
            bool esCompartido = false;
            List<Guid>? idsAlumnosCompartidos = null;
            List<string>? nombresAlumnosCompartidos = null;

            if (paquete?.IdPago.HasValue == true && paquetesPorPago.ContainsKey(paquete.IdPago.Value))
            {
                var paquetesDelGrupo = paquetesPorPago[paquete.IdPago.Value];
                if (paquetesDelGrupo.Count > 1)
                {
                    esCompartido = true;
                    // Obtener IDs y nombres de los OTROS alumnos (no el actual)
                    idsAlumnosCompartidos = paquetesDelGrupo
                        .Where(p => p.IdAlumno != alumno.IdAlumno)
                        .Select(p => p.IdAlumno)
                        .ToList();
                    nombresAlumnosCompartidos = paquetesDelGrupo
                        .Where(p => p.IdAlumno != alumno.IdAlumno)
                        .Select(p => p.Alumno.Usuario.NombreUsuario)
                        .ToList();
                }
            }

            var estadoPaqueteAdmin = paquete is null
                ? EstadoPaqueteAdmin.SinPaquete
                : MapEstadoPaquete(paquete.Estado?.Nombre);

            int? clasesTotales = paquete?.ClasesDisponibles;
            int? clasesUsadas = paquete?.ClasesUsadas;
            int? clasesRestantes = paquete is null ? null : paquete.ClasesDisponibles - paquete.ClasesUsadas;

            var paqueteDto = new PaqueteAlumnoAdminDto
            {
                IdPaquete = paquete?.IdPaquete,
                Estado = estadoPaqueteAdmin,
                Descripcion = paquete?.TipoPaquete?.Nombre ?? string.Empty,
                ClasesTotales = clasesTotales,
                ClasesUsadas = clasesUsadas,
                ClasesRestantes = clasesRestantes,
                EsCompartido = esCompartido,
                IdsAlumnosCompartidos = idsAlumnosCompartidos,
                NombresAlumnosCompartidos = nombresAlumnosCompartidos
            };

            var estadoAsistenciaAdmin = asistencia.Estado?.Nombre == "Presente" 
                ? EstadoAsistenciaAdmin.Presente 
                : EstadoAsistenciaAdmin.Ausente;

            var asistenciaDto = new AsistenciaAlumnoAdminDto
            {
                IdAsistencia = asistencia.IdAsistencia,
                Estado = estadoAsistenciaAdmin,
                Observacion = asistencia.Observacion
            };

            var nombreCompleto = alumno.NombreCompleto;
            var iniciales = CalcularIniciales(nombreCompleto);

            var alumnoDto = new AlumnoEnClaseAdminDto
            {
                IdAlumno = alumno.IdAlumno,
                NombreCompleto = nombreCompleto,
                DocumentoIdentidad = alumno.DocumentoIdentidad,
                AvatarIniciales = iniciales,
                Paquete = paqueteDto,
                Asistencia = asistenciaDto
            };

            alumnos.Add(alumnoDto);
        }

        // Luego, agregar paquetes de alumnos que NO tienen asistencia registrada
        foreach (var paquete in paquetesActivos)
        {
            var alumno = paquete.Alumno;

            // Si el alumno ya tiene asistencia registrada, no mostrar sus otros paquetes
            if (alumnosConAsistencia.Contains(alumno.IdAlumno))
                continue;

            // Si el alumno ya tiene asistencia registrada, no mostrar sus otros paquetes
            if (alumnosConAsistencia.Contains(alumno.IdAlumno))
                continue;

            // Verificar si este paquete es compartido
            bool esCompartido = false;
            List<Guid>? idsAlumnosCompartidos = null;
            List<string>? nombresAlumnosCompartidos = null;

            if (paquete.IdPago.HasValue && paquetesPorPago.ContainsKey(paquete.IdPago.Value))
            {
                var paquetesDelGrupo = paquetesPorPago[paquete.IdPago.Value];
                if (paquetesDelGrupo.Count > 1)
                {
                    esCompartido = true;
                    // Obtener IDs y nombres de los OTROS alumnos (no el actual)
                    idsAlumnosCompartidos = paquetesDelGrupo
                        .Where(p => p.IdAlumno != alumno.IdAlumno)
                        .Select(p => p.IdAlumno)
                        .ToList();
                    nombresAlumnosCompartidos = paquetesDelGrupo
                        .Where(p => p.IdAlumno != alumno.IdAlumno)
                        .Select(p => p.Alumno.Usuario.NombreUsuario)
                        .ToList();
                }
            }

            // Mapear estado de paquete de dominio a DTO admin
            var estadoPaqueteAdmin = MapEstadoPaquete(paquete.Estado?.Nombre);

            int? clasesTotales = paquete.ClasesDisponibles;
            int? clasesUsadas = paquete.ClasesUsadas;
            int? clasesRestantes = paquete.ClasesDisponibles - paquete.ClasesUsadas;

            var paqueteDto = new PaqueteAlumnoAdminDto
            {
                IdPaquete = paquete.IdPaquete,
                Estado = estadoPaqueteAdmin,
                Descripcion = paquete.TipoPaquete?.Nombre ?? string.Empty,
                ClasesTotales = clasesTotales,
                ClasesUsadas = clasesUsadas,
                ClasesRestantes = clasesRestantes,
                EsCompartido = esCompartido,
                IdsAlumnosCompartidos = idsAlumnosCompartidos,
                NombresAlumnosCompartidos = nombresAlumnosCompartidos
            };

            // No hay asistencia registrada, marcar como Ausente por defecto
            var asistenciaDto = new AsistenciaAlumnoAdminDto
            {
                IdAsistencia = null,
                Estado = EstadoAsistenciaAdmin.Ausente,
                Observacion = null
            };

            var nombreCompleto = alumno.NombreCompleto;
            var iniciales = CalcularIniciales(nombreCompleto);

            var alumnoDto = new AlumnoEnClaseAdminDto
            {
                IdAlumno = alumno.IdAlumno,
                NombreCompleto = nombreCompleto,
                DocumentoIdentidad = alumno.DocumentoIdentidad,
                AvatarIniciales = iniciales,
                Paquete = paqueteDto,
                Asistencia = asistenciaDto
            };

            alumnos.Add(alumnoDto);
        }

        var presentes = alumnos.Count(a => a.Asistencia.Estado == EstadoAsistenciaAdmin.Presente);
        var ausentes = alumnos.Count(a => a.Asistencia.Estado == EstadoAsistenciaAdmin.Ausente);
        var sinPaquete = alumnos.Count(a => a.Paquete.Estado == EstadoPaqueteAdmin.SinPaquete);

        var resumen = new ResumenAsistenciasClaseAdminDto
        {
            IdClase = clase.IdClase,
            Fecha = DateOnly.FromDateTime(clase.Fecha),
            NombreClase = clase.TipoClase.Nombre,
            ProfesorPrincipal = clase.ProfesorPrincipal.NombreCompleto,
            Alumnos = alumnos,
            Presentes = presentes,
            Ausentes = ausentes,
            SinPaquete = sinPaquete
        };

        return Result<ResumenAsistenciasClaseAdminDto>.Success(resumen);
    }

    private static EstadoPaqueteAdmin MapEstadoPaquete(string? nombreEstado)
    {
        if (string.IsNullOrWhiteSpace(nombreEstado))
        {
            return EstadoPaqueteAdmin.SinPaquete;
        }

        return nombreEstado switch
        {
            "Activo" => EstadoPaqueteAdmin.Activo,
            "Agotado" => EstadoPaqueteAdmin.Agotado,
            "Congelado" => EstadoPaqueteAdmin.Congelado,
            _ => EstadoPaqueteAdmin.SinPaquete
        };
    }

    private static string CalcularIniciales(string nombreCompleto)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
        {
            return string.Empty;
        }

        var partes = nombreCompleto
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (partes.Length == 0)
        {
            return string.Empty;
        }

        if (partes.Length == 1)
        {
            return partes[0].Substring(0, 1).ToUpperInvariant();
        }

        return (partes[0].Substring(0, 1) + partes[^1].Substring(0, 1)).ToUpperInvariant();
    }
}

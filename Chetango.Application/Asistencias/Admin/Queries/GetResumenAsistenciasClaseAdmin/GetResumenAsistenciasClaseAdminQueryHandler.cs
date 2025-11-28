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
                .ThenInclude(a => a.PaqueteUsado)
                    .ThenInclude(p => p.Estado)
            .Include(c => c.Asistencias)
                .ThenInclude(a => a.PaqueteUsado)
                    .ThenInclude(p => p.TipoPaquete)
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase is null)
        {
            return Result<ResumenAsistenciasClaseAdminDto>.Failure("Clase no encontrada");
        }

        var alumnos = new List<AlumnoEnClaseAdminDto>();

        foreach (var asistencia in clase.Asistencias)
        {
            var alumno = asistencia.Alumno;
            var paquete = asistencia.PaqueteUsado;

            // Mapear estado de paquete de dominio a DTO admin
            var estadoPaqueteAdmin = paquete is null
                ? EstadoPaqueteAdmin.SinPaquete
                : MapEstadoPaquete(paquete.Estado?.Nombre);

            int? clasesTotales = paquete?.ClasesDisponibles;
            int? clasesUsadas = paquete?.ClasesUsadas;
            int? clasesRestantes = paquete is null ? null : paquete.ClasesDisponibles - paquete.ClasesUsadas;

            var paqueteDto = new PaqueteAlumnoAdminDto
            {
                Estado = estadoPaqueteAdmin,
                Descripcion = paquete?.TipoPaquete?.Nombre ?? string.Empty,
                ClasesTotales = clasesTotales,
                ClasesUsadas = clasesUsadas,
                ClasesRestantes = clasesRestantes
            };

            var estadoAsistenciaAdmin = asistencia.Estado?.Nombre == "Presente"
                ? EstadoAsistenciaAdmin.Presente
                : EstadoAsistenciaAdmin.Ausente;

            var asistenciaDto = new AsistenciaAlumnoAdminDto
            {
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

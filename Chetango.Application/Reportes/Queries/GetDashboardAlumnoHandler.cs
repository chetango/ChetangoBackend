using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Reportes.Queries;

public class GetDashboardAlumnoHandler : IRequestHandler<GetDashboardAlumnoQuery, Result<DashboardAlumnoDTO>>
{
    private readonly IAppDbContext _db;

    public GetDashboardAlumnoHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<DashboardAlumnoDTO>> Handle(GetDashboardAlumnoQuery request, CancellationToken cancellationToken)
    {
        // Buscar alumno por email (ownership validation)
        var alumno = await _db.Alumnos
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Usuario.Correo == request.EmailUsuario, cancellationToken);

        if (alumno == null)
            return Result<DashboardAlumnoDTO>.Failure("No se encontró el alumno autenticado.");

        var hoy = DateTime.Today;
        var ahora = DateTime.Now;

        // ==========================================
        // 1. INFORMACIÓN DEL ALUMNO
        // ==========================================
        var codigo = $"ALU-{alumno.FechaInscripcion:yyyy}-{alumno.IdAlumno.ToString().Substring(0, 5).ToUpper()}";

        // ==========================================
        // 2. PAQUETE ACTIVO
        // ==========================================
        var paqueteActivo = await _db.Paquetes
            .Include(p => p.TipoPaquete)
            .Include(p => p.Estado)
            .Where(p => p.IdAlumno == alumno.IdAlumno && 
                       p.Estado.Nombre == "Activo")
            .OrderByDescending(p => p.FechaActivacion)
            .FirstOrDefaultAsync(cancellationToken);

        PaqueteActivoDTO? paqueteDTO = null;
        if (paqueteActivo != null)
        {
            var clasesUsadas = await _db.Asistencias
                .Include(a => a.Estado)
                .Where(a => a.IdPaqueteUsado == paqueteActivo.IdPaquete &&
                           a.Estado.Nombre == "Presente")
                .CountAsync(cancellationToken);

            var clasesRestantes = paqueteActivo.ClasesDisponibles - clasesUsadas;
            var diasParaVencer = (paqueteActivo.FechaVencimiento.Date - hoy).Days;

            var estadoPaquete = "activo";
            if (clasesRestantes == 0)
                estadoPaquete = "agotado";
            else if (paqueteActivo.FechaVencimiento < hoy)
                estadoPaquete = "vencido";

            paqueteDTO = new PaqueteActivoDTO
            {
                IdPaquete = paqueteActivo.IdPaquete,
                Tipo = paqueteActivo.TipoPaquete.Nombre,
                ClasesRestantes = clasesRestantes > 0 ? clasesRestantes : 0,
                ClasesTotales = paqueteActivo.ClasesDisponibles,
                Estado = estadoPaquete,
                FechaVencimiento = paqueteActivo.FechaVencimiento,
                DiasParaVencer = diasParaVencer > 0 ? diasParaVencer : 0
            };
        }

        // ==========================================
        // 3. PRÓXIMA CLASE
        // ==========================================
        var proximaClase = await _db.Clases
            .Include(c => c.TipoClase)
            .Include(c => c.ProfesorPrincipal)
                .ThenInclude(p => p.Usuario)
            .Where(c => c.Fecha >= hoy &&
                       _db.Asistencias.Any(a => a.IdClase == c.IdClase && 
                                               a.IdAlumno == alumno.IdAlumno))
            .OrderBy(c => c.Fecha)
            .ThenBy(c => c.HoraInicio)
            .FirstOrDefaultAsync(cancellationToken);

        ProximaClaseAlumnoDTO? proximaClaseDTO = null;
        if (proximaClase != null)
        {
            var fechaHoraClase = proximaClase.Fecha.Date.Add(proximaClase.HoraInicio);
            var minutosParaInicio = (int)(fechaHoraClase - ahora).TotalMinutes;

            proximaClaseDTO = new ProximaClaseAlumnoDTO
            {
                IdClase = proximaClase.IdClase,
                Nombre = proximaClase.TipoClase.Nombre,
                Nivel = "Intermedio", // Por defecto, se puede mejorar agregando nivel al TipoClase
                Fecha = proximaClase.Fecha,
                Hora = proximaClase.HoraInicio,
                Profesor = proximaClase.ProfesorPrincipal.Usuario.NombreUsuario,
                MinutosParaInicio = minutosParaInicio > 0 ? minutosParaInicio : 0,
                Ubicacion = "Salón Principal - Academia Chetango" // Hardcoded, se puede agregar campo después
            };
        }

        // ==========================================
        // 4. ESTADÍSTICAS DE ASISTENCIA
        // ==========================================
        var todasAsistencias = await _db.Asistencias
            .Include(a => a.Estado)
            .Include(a => a.Clase)
            .Where(a => a.IdAlumno == alumno.IdAlumno)
            .ToListAsync(cancellationToken);

        var clasesTomadas = todasAsistencias.Count(a => a.Estado.Nombre == "Presente");
        var totalAsistencias = todasAsistencias.Count;
        var porcentajeAsistencia = totalAsistencias > 0 
            ? (decimal)clasesTomadas / totalAsistencias * 100 
            : 0;

        // Calcular racha (semanas consecutivas con al menos 1 clase)
        var asistenciasPresentes = todasAsistencias.Where(a => a.Estado.Nombre == "Presente").ToList();
        var racha = CalcularRacha(asistenciasPresentes);

        var asistenciaDTO = new AsistenciaAlumnoDTO
        {
            Porcentaje = Math.Round(porcentajeAsistencia, 0),
            ClasesTomadas = clasesTomadas,
            RachaSemanas = racha
        };

        // ==========================================
        // 5. LOGROS (GAMIFICACIÓN)
        // ==========================================
        var logros = new List<LogroDTO>();

        // Logro: Constancia (5 clases seguidas)
        var racha5Clases = racha >= 5;
        logros.Add(new LogroDTO
        {
            Id = "constancia",
            Nombre = "Constancia",
            Descripcion = "5 clases seguidas",
            Icono = "flame",
            Color = "#f59e0b",
            Desbloqueado = racha5Clases
        });

        // Logro: Progreso (10 clases en un mes)
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        var clasesEsteMes = todasAsistencias.Count(a => 
            a.Estado.Nombre == "Presente" && 
            a.Clase.Fecha >= inicioMes && 
            a.Clase.Fecha <= hoy);
        
        logros.Add(new LogroDTO
        {
            Id = "progreso",
            Nombre = "Progreso",
            Descripcion = "10 clases en un mes",
            Icono = "trophy",
            Color = "#34d399",
            Desbloqueado = clasesEsteMes >= 10
        });

        // Logro: Dedicación (25 clases totales)
        logros.Add(new LogroDTO
        {
            Id = "dedicacion",
            Nombre = "Dedicación",
            Descripcion = "25 clases totales",
            Icono = "target",
            Color = "#7c5af8",
            Desbloqueado = clasesTomadas >= 25
        });

        // ==========================================
        // 6. EVENTOS PRÓXIMOS (Solo para alumnos)
        // ==========================================
        var eventos = await EventosQueryHelper.ObtenerEventosProximos(_db, "Alumno", cancellationToken);

        // ==========================================
        // 7. CONSTRUIR RESPONSE
        // ==========================================
        var dashboard = new DashboardAlumnoDTO
        {
            NombreAlumno = alumno.Usuario.NombreUsuario,
            Correo = alumno.Usuario.Correo,
            Telefono = alumno.Usuario.Telefono ?? string.Empty,
            Codigo = codigo,
            FechaIngreso = alumno.FechaInscripcion,
            PaqueteActivo = paqueteDTO,
            ProximaClase = proximaClaseDTO,
            Asistencia = asistenciaDTO,
            Logros = logros,
            EventosProximos = eventos
        };

        return Result<DashboardAlumnoDTO>.Success(dashboard);
    }

    /// <summary>
    /// Calcula la racha de semanas consecutivas con al menos 1 clase
    /// </summary>
    private int CalcularRacha(List<Asistencia> asistenciasPresentes)
    {
        if (!asistenciasPresentes.Any())
            return 0;

        var fechasClases = asistenciasPresentes
            .Select(a => a.Clase.Fecha.Date)
            .Distinct()
            .OrderByDescending(f => f)
            .ToList();

        var hoy = DateTime.Today;
        var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);
        
        int racha = 0;
        var semanasConClase = new HashSet<DateTime>();

        foreach (var fecha in fechasClases)
        {
            var inicioSemanaDeFecha = fecha.AddDays(-(int)fecha.DayOfWeek);
            semanasConClase.Add(inicioSemanaDeFecha);
        }

        var semanasOrdenadas = semanasConClase.OrderByDescending(s => s).ToList();
        var semanaActual = inicioSemana;

        foreach (var semana in semanasOrdenadas)
        {
            if (semana == semanaActual)
            {
                racha++;
                semanaActual = semanaActual.AddDays(-7);
            }
            else
            {
                break;
            }
        }

        return racha;
    }
}

using Chetango.Application.Common;
using Chetango.Application.Reportes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Reportes.Queries;

public class GetDashboardProfesorHandler : IRequestHandler<GetDashboardProfesorQuery, Result<DashboardProfesorDTO>>
{
    private readonly IAppDbContext _db;

    public GetDashboardProfesorHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<DashboardProfesorDTO>> Handle(GetDashboardProfesorQuery request, CancellationToken cancellationToken)
    {
        // Buscar profesor por email (ownership validation)
        var profesor = await _db.Profesores
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Usuario.Correo == request.EmailUsuario, cancellationToken);

        if (profesor == null)
            return Result<DashboardProfesorDTO>.Failure("No se encontró el profesor autenticado.");

        var hoy = DateTime.Today;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);
        var ultimos30Dias = hoy.AddDays(-30);

        // ==========================================
        // 1. CLASES DE HOY
        // ==========================================
        var clasesHoyQuery = await _db.Clases
            .Include(c => c.TipoClase)
            .Where(c => c.IdProfesorPrincipal == profesor.IdProfesor &&
                       c.Fecha.Date == hoy)
            .OrderBy(c => c.HoraInicio)
            .Select(c => new
            {
                c.IdClase,
                c.TipoClase,
                c.HoraInicio,
                c.HoraFin,
                c.CupoMaximo,
                c.Fecha,
                AlumnosPresentes = c.Asistencias.Count(a => a.Estado.Nombre == "Presente")
            })
            .ToListAsync(cancellationToken);

        var ahora = DateTime.Now.TimeOfDay;
        var clasesHoy = clasesHoyQuery.Select(c =>
        {
            var estado = "programada";
            int? minutosParaInicio = null;

            if (c.Fecha.Date == hoy)
            {
                if (ahora >= c.HoraInicio && ahora <= c.HoraFin)
                {
                    estado = "en-curso";
                }
                else if (ahora > c.HoraFin)
                {
                    estado = "finalizada";
                }
                else if (ahora < c.HoraInicio)
                {
                    estado = "programada";
                    minutosParaInicio = (int)(c.HoraInicio - ahora).TotalMinutes;
                }
            }

            return new ClaseHoyProfesorDTO
            {
                IdClase = c.IdClase,
                Nombre = c.TipoClase.Nombre,
                Nivel = "General",
                HoraInicio = c.HoraInicio,
                HoraFin = c.HoraFin,
                Tipo = c.CupoMaximo == 1 ? "particular" : "grupal",
                Estado = estado,
                AlumnosEsperados = c.CupoMaximo,
                AlumnosPresentes = c.AlumnosPresentes,
                MinutosParaInicio = minutosParaInicio
            };
        }).ToList();

        // ==========================================
        // 2. KPIs
        // ==========================================
        
        // Clases dictadas este mes
        var clasesMes = await _db.Clases
            .Where(c => c.IdProfesorPrincipal == profesor.IdProfesor &&
                       c.Fecha >= inicioMes &&
                       c.Fecha <= hoy)
            .CountAsync(cancellationToken);

        // Clases esta semana
        var clasesSemana = await _db.Clases
            .Where(c => c.IdProfesorPrincipal == profesor.IdProfesor &&
                       c.Fecha >= inicioSemana &&
                       c.Fecha <= hoy.AddDays(7))
            .CountAsync(cancellationToken);

        // Clases completadas esta semana (ya pasaron)
        var clasesCompletadasSemana = await _db.Clases
            .Where(c => c.IdProfesorPrincipal == profesor.IdProfesor &&
                       c.Fecha >= inicioSemana &&
                       c.Fecha < hoy)
            .CountAsync(cancellationToken);

        // Promedio de asistencia últimos 30 días
        var asistenciasUltimos30Dias = await _db.Asistencias
            .Where(a => a.Clase.IdProfesorPrincipal == profesor.IdProfesor &&
                       a.Clase.Fecha >= ultimos30Dias &&
                       a.Clase.Fecha <= hoy)
            .Select(a => new { EstadoNombre = a.Estado.Nombre })
            .ToListAsync(cancellationToken);

        var totalAsistencias = asistenciasUltimos30Dias.Count;
        var asistenciasPresentes = asistenciasUltimos30Dias.Count(a => a.EstadoNombre == "Presente");
        var promedioAsistencia = totalAsistencias > 0 
            ? (decimal)asistenciasPresentes / totalAsistencias * 100 
            : 0;

        // Alumnos únicos este mes
        var alumnosUnicosMes = await _db.Asistencias
            .Where(a => a.Clase.IdProfesorPrincipal == profesor.IdProfesor &&
                       a.Clase.Fecha >= inicioMes &&
                       a.Clase.Fecha <= hoy &&
                       a.Estado.Nombre == "Presente")
            .Select(a => a.IdAlumno)
            .Distinct()
            .CountAsync(cancellationToken);

        var kpis = new KPIsProfesorDTO
        {
            ClasesDictadasMes = clasesMes,
            PromedioAsistencia30Dias = Math.Round(promedioAsistencia, 0),
            AlumnosUnicosMes = alumnosUnicosMes,
            ClasesEstaSemana = clasesSemana,
            ClasesCompletadasSemana = clasesCompletadasSemana
        };

        // ==========================================
        // 3. GRÁFICA ASISTENCIA ÚLTIMOS 30 DÍAS (POR SEMANA)
        // ==========================================
        var asistenciasPorSemana = new List<(string Semana, int Presentes, int Total)>();
        
        for (int i = 4; i >= 0; i--)
        {
            var inicioSem = ultimos30Dias.AddDays(i * 7);
            var finSem = inicioSem.AddDays(6);
            
            var asistenciasSemana = await _db.Asistencias
                .Where(a => a.Clase.IdProfesorPrincipal == profesor.IdProfesor &&
                           a.Clase.Fecha >= inicioSem &&
                           a.Clase.Fecha <= finSem)
                .Select(a => new { EstadoNombre = a.Estado.Nombre })
                .ToListAsync(cancellationToken);

            var total = asistenciasSemana.Count;
            var presentes = asistenciasSemana.Count(a => a.EstadoNombre == "Presente");
            
            var labelSemana = i == 0 ? "Esta sem" : $"Sem {5 - i}";
            
            asistenciasPorSemana.Add((labelSemana, presentes, total));
        }

        var graficaAsistencia = new ChartDataDTO
        {
            Type = "bar",
            Labels = asistenciasPorSemana.Select(x => x.Semana).ToList(),
            Datasets = new List<ChartDatasetDTO>
            {
                new ChartDatasetDTO
                {
                    Label = "Asistencia",
                    Data = asistenciasPorSemana.Select(x => 
                        x.Total > 0 ? Math.Round((decimal)x.Presentes / x.Total * 100, 0) : 0
                    ).ToList(),
                    BackgroundColor = "#34d399",
                    BorderColor = "#059669"
                }
            }
        };

        // ==========================================
        // 4. PRÓXIMAS CLASES (excluyendo hoy)
        // ==========================================
        var proximasClases = await _db.ClasesProfesores
            .Include(cp => cp.Clase)
                .ThenInclude(c => c.TipoClase)
            .Where(cp => cp.IdProfesor == profesor.IdProfesor &&
                       cp.Clase.Fecha > hoy &&
                       cp.Clase.Fecha <= hoy.AddDays(7))
            .OrderBy(cp => cp.Clase.Fecha)
            .ThenBy(cp => cp.Clase.HoraInicio)
            .Select(cp => new ClaseProximaDTO
            {
                IdClase = cp.Clase.IdClase,
                Fecha = cp.Clase.Fecha,
                HoraInicio = cp.Clase.HoraInicio,
                TipoClase = cp.Clase.TipoClase.Nombre,
                NombreProfesor = profesor.Usuario.NombreUsuario,
                CupoMaximo = cp.Clase.CupoMaximo,
                InscritosActual = cp.Clase.Asistencias.Count
            })
            .ToListAsync(cancellationToken);

        // ==========================================
        // 6. EVENTOS PRÓXIMOS (Solo para profesores)
        // ==========================================
        var eventos = await EventosQueryHelper.ObtenerEventosProximos(_db, "Profesor", cancellationToken);

        // ==========================================
        // RESULTADO
        // ==========================================
        var dashboard = new DashboardProfesorDTO
        {
            NombreProfesor = profesor.Usuario.NombreUsuario,
            Correo = profesor.Usuario.Correo,
            ClasesHoy = clasesHoy,
            KPIs = kpis,
            GraficaAsistencia30Dias = graficaAsistencia,
            ProximasClases = proximasClases,
            EventosProximos = eventos
        };

        return Result<DashboardProfesorDTO>.Success(dashboard);
    }
}

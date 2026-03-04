using Chetango.Application.Asistencias.DTOs;
using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Asistencias.Queries.GetAsistenciasClaseConAlumnos;

/// <summary>
/// Handler para obtener asistencias de una clase incluyendo TODOS los alumnos activos del sistema
/// Permite al profesor ver y marcar asistencias de cualquier alumno
/// Basado en la lógica de GetResumenAsistenciasClaseAdminQueryHandler
/// </summary>
public class GetAsistenciasClaseConAlumnosQueryHandler 
    : IRequestHandler<GetAsistenciasClaseConAlumnosQuery, Result<IReadOnlyList<AsistenciaProfesorDto>>>
{
    private readonly IAppDbContext _db;

    public GetAsistenciasClaseConAlumnosQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<AsistenciaProfesorDto>>> Handle(
        GetAsistenciasClaseConAlumnosQuery request,
        CancellationToken cancellationToken)
    {
        // Verificar que la clase existe
        var clase = await _db.Set<Clase>()
            .AsNoTracking()
            .Include(c => c.Asistencias)
                .ThenInclude(a => a.Estado)
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase is null)
        {
            return Result<IReadOnlyList<AsistenciaProfesorDto>>.Failure("Clase no encontrada");
        }

        // Obtener TODOS los alumnos activos del sistema
        var alumnosActivos = await _db.Set<Alumno>()
            .AsNoTracking()
            .Include(a => a.Usuario)
                .ThenInclude(u => u.Estado)
            .Where(a => a.Usuario.Estado.Nombre == "Activo")
            .ToListAsync(cancellationToken);

        // Obtener todos los paquetes activos de esos alumnos
        var idsAlumnos = alumnosActivos.Select(a => a.IdAlumno).ToList();
        var paquetesActivos = await _db.Set<Paquete>()
            .AsNoTracking()
            .Include(p => p.Estado)
            .Include(p => p.TipoPaquete)
            .Where(p => idsAlumnos.Contains(p.IdAlumno) 
                && p.Estado.Nombre == "Activo"
                && p.FechaVencimiento >= DateTime.Today) // Excluir paquetes vencidos
            .ToListAsync(cancellationToken);

        var resultado = new List<AsistenciaProfesorDto>();

        foreach (var alumno in alumnosActivos)
        {
            // Buscar si ya tiene asistencia registrada para esta clase
            var asistencia = clase.Asistencias.FirstOrDefault(a => a.IdAlumno == alumno.IdAlumno);

            // Obtener el paquete activo del alumno
            // Si tiene asistencia registrada y usó un paquete, necesitamos buscarlo
            Guid? idPaqueteUsado = asistencia?.IdPaqueteUsado;
            Paquete? paquete = null;

            if (idPaqueteUsado.HasValue)
            {
                // Si ya registró asistencia, usar el paquete que usó
                paquete = await _db.Set<Paquete>()
                    .AsNoTracking()
                    .Include(p => p.Estado)
                    .Include(p => p.TipoPaquete)
                    .FirstOrDefaultAsync(p => p.IdPaquete == idPaqueteUsado.Value, cancellationToken);
            }
            else
            {
                // Si no hay asistencia, buscar el paquete activo del alumno
                paquete = paquetesActivos
                    .Where(p => p.IdAlumno == alumno.IdAlumno)
                    .OrderBy(p => p.FechaVencimiento)
                    .FirstOrDefault();
            }

            // Mapear estado de paquete (considera fecha de vencimiento además del estado en BD)
            string estadoPaquete = paquete is null
                ? "SinPaquete"
                : MapEstadoPaquete(paquete.Estado?.Nombre, paquete.FechaVencimiento);

            int? clasesRestantes = paquete is null 
                ? null 
                : paquete.ClasesDisponibles - paquete.ClasesUsadas;

            // Determinar estado de asistencia (presente/ausente)
            bool presente = asistencia?.Estado?.Nombre == "Presente";

            var dto = new AsistenciaProfesorDto(
                IdAsistencia: asistencia?.IdAsistencia,  // null si no hay registro previo
                IdAlumno: alumno.IdAlumno,
                NombreAlumno: alumno.NombreCompleto,
                DocumentoIdentidad: alumno.DocumentoIdentidad,
                Presente: presente,
                Observacion: asistencia?.Observacion,
                EstadoPaquete: estadoPaquete,
                ClasesRestantes: clasesRestantes,
                IdPaquete: paquete?.IdPaquete
            );

            resultado.Add(dto);
        }

        // Ordenar por nombre para consistencia con la vista
        var resultadoOrdenado = resultado
            .OrderBy(a => a.NombreAlumno)
            .ToList();

        return Result<IReadOnlyList<AsistenciaProfesorDto>>.Success(resultadoOrdenado);
    }

    private static string MapEstadoPaquete(string? nombreEstado, DateTime? fechaVencimiento = null)
    {
        if (string.IsNullOrWhiteSpace(nombreEstado))
            return "SinPaquete";

        // Si el estado en BD es "Activo" pero la fecha ya venció, se considera Vencido
        if (nombreEstado == "Activo" && fechaVencimiento.HasValue && fechaVencimiento.Value.Date < DateTime.Today)
            return "Vencido";

        return nombreEstado switch
        {
            "Activo" => "Activo",
            "Agotado" => "Agotado",
            "Congelado" => "Congelado",
            "Vencido" => "Vencido",
            _ => "SinPaquete"
        };
    }
}

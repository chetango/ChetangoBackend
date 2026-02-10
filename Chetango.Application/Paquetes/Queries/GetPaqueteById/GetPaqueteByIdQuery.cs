using Chetango.Application.Common;
using Chetango.Application.Paquetes.DTOs;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Queries.GetPaqueteById;

// Query para obtener el detalle completo de un paquete
public record GetPaqueteByIdQuery(
    Guid IdPaquete,
    string? CorreoUsuarioActual = null,
    bool EsAdmin = false
) : IRequest<Result<PaqueteDetalleDTO>>;

// Handler
public class GetPaqueteByIdQueryHandler : IRequestHandler<GetPaqueteByIdQuery, Result<PaqueteDetalleDTO>>
{
    private readonly IAppDbContext _db;

    public GetPaqueteByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PaqueteDetalleDTO>> Handle(GetPaqueteByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtener el paquete con sus relaciones
        var paquete = await _db.Set<Paquete>()
            .Include(p => p.Alumno)
                .ThenInclude(a => a.Usuario)
            .Include(p => p.TipoPaquete)
            .Include(p => p.Congelaciones)
            .Include(p => p.Pago)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPaquete == request.IdPaquete, cancellationToken);

        if (paquete is null)
            return Result<PaqueteDetalleDTO>.Failure("El paquete especificado no existe.");

        // 2. Validar ownership: si no es admin, verificar que el paquete pertenece al usuario
        if (!request.EsAdmin && !string.IsNullOrEmpty(request.CorreoUsuarioActual))
        {
            if (!string.Equals(paquete.Alumno.Usuario.Correo, request.CorreoUsuarioActual, StringComparison.OrdinalIgnoreCase))
                return Result<PaqueteDetalleDTO>.Failure("No tienes permiso para ver este paquete.");
        }

        // 3. Mapear estado a texto (ajustado a los estados reales de la BD)
        var estadoNombre = paquete.IdEstado switch
        {
            1 => "Activo",
            2 => "Vencido",
            3 => "Congelado",
            _ => paquete.ClasesUsadas >= paquete.ClasesDisponibles ? "Completado" : "Vencido"
        };

        // 4. Mapear congelaciones
        var congelaciones = paquete.Congelaciones?
            .OrderByDescending(c => c.FechaInicio)
            .Select(c => new CongelacionDTO(
                c.IdCongelacion,
                c.FechaInicio,
                c.FechaFin,
                (c.FechaFin - c.FechaInicio).Days
            ))
            .ToList() ?? new List<CongelacionDTO>();

        // 5. Obtener todos los alumnos del mismo pago (si existe)
        List<AlumnoPaqueteDTO>? alumnosDelPago = null;
        if (paquete.IdPago.HasValue)
        {
            alumnosDelPago = await _db.Set<Paquete>()
                .Where(p => p.IdPago == paquete.IdPago && p.IdPaquete != paquete.IdPaquete)
                .Include(p => p.Alumno)
                    .ThenInclude(a => a.Usuario)
                .Select(p => new AlumnoPaqueteDTO(
                    p.IdAlumno,
                    p.Alumno.Usuario.NombreUsuario
                ))
                .Distinct()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Agregar el alumno actual a la lista
            if (alumnosDelPago.Count > 0)
            {
                alumnosDelPago.Insert(0, new AlumnoPaqueteDTO(
                    paquete.IdAlumno,
                    paquete.Alumno.Usuario.NombreUsuario
                ));
            }
        }

        // 6. Crear el DTO de detalle
        var clasesRestantes = paquete.ClasesDisponibles - paquete.ClasesUsadas;
        var estaVencido = paquete.FechaVencimiento < DateTime.Today;
        var tieneClasesDisponibles = clasesRestantes > 0;

        var dto = new PaqueteDetalleDTO(
            paquete.IdPaquete,
            paquete.IdAlumno,
            paquete.Alumno.Usuario.NombreUsuario,
            paquete.IdTipoPaquete,
            paquete.TipoPaquete.Nombre,
            paquete.ClasesDisponibles,
            paquete.ClasesUsadas,
            clasesRestantes,
            paquete.FechaActivacion,
            paquete.FechaVencimiento,
            paquete.ValorPaquete,
            paquete.IdEstado,
            estadoNombre,
            estaVencido,
            tieneClasesDisponibles,
            congelaciones,
            alumnosDelPago
        );

        return Result<PaqueteDetalleDTO>.Success(dto);
    }
}

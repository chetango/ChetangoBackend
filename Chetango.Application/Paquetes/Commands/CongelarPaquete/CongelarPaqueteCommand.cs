using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Commands.CongelarPaquete;

// Command para congelar un paquete
public record CongelarPaqueteCommand(
    Guid IdPaquete,
    DateTime FechaInicio,
    DateTime FechaFin,
    string? Motivo = null
) : IRequest<Result<Unit>>;

// Handler
public class CongelarPaqueteCommandHandler : IRequestHandler<CongelarPaqueteCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public CongelarPaqueteCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(CongelarPaqueteCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener el paquete con tracking para actualizarlo
        var paquete = await _db.Set<Paquete>()
            .Include(p => p.Congelaciones)
            .FirstOrDefaultAsync(p => p.IdPaquete == request.IdPaquete, cancellationToken);

        if (paquete is null)
            return Result<Unit>.Failure("El paquete especificado no existe.");

        // 2. Validar que el paquete está en estado Activo
        if (paquete.IdEstado != 1) // 1 = Activo
        {
            var estadoNombre = paquete.IdEstado switch
            {
                2 => "Vencido",
                3 => "Congelado",
                4 => "Agotado",
                _ => "Inválido"
            };
            return Result<Unit>.Failure($"Solo se pueden congelar paquetes activos (estado actual: {estadoNombre}).");
        }

        // 3. Validar que las fechas sean futuras o actuales
        if (request.FechaInicio.Date < DateTime.Today)
            return Result<Unit>.Failure("La fecha de inicio no puede ser anterior a hoy.");

        // 4. Validar que no haya solapamiento con otras congelaciones
        var tieneSolapamiento = paquete.Congelaciones?.Any(c =>
            (request.FechaInicio >= c.FechaInicio && request.FechaInicio <= c.FechaFin) ||
            (request.FechaFin >= c.FechaInicio && request.FechaFin <= c.FechaFin) ||
            (request.FechaInicio <= c.FechaInicio && request.FechaFin >= c.FechaFin)
        ) ?? false;

        if (tieneSolapamiento)
            return Result<Unit>.Failure("Ya existe una congelación en el período especificado.");

        // 5. Cambiar estado del paquete a Congelado
        paquete.IdEstado = 3; // 3 = Congelado
        paquete.FechaModificacion = DateTime.Now;
        paquete.UsuarioModificacion = "Sistema"; // TODO: Obtener del contexto de usuario

        // 6. Crear el registro de congelación
        var congelacion = new CongelacionPaquete
        {
            IdCongelacion = Guid.NewGuid(),
            IdPaquete = request.IdPaquete,
            FechaInicio = request.FechaInicio.Date,
            FechaFin = request.FechaFin.Date
        };

        _db.Set<CongelacionPaquete>().Add(congelacion);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

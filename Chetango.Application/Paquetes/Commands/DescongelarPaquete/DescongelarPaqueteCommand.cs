using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Commands.DescongelarPaquete;

// Command para descongelar un paquete
public record DescongelarPaqueteCommand(
    Guid IdPaquete,
    Guid IdCongelacion
) : IRequest<Result<Unit>>;

// Handler
public class DescongelarPaqueteCommandHandler : IRequestHandler<DescongelarPaqueteCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public DescongelarPaqueteCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(DescongelarPaqueteCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener el paquete con tracking para actualizarlo
        var paquete = await _db.Set<Paquete>()
            .Include(p => p.Congelaciones)
            .FirstOrDefaultAsync(p => p.IdPaquete == request.IdPaquete, cancellationToken);

        if (paquete is null)
            return Result<Unit>.Failure("El paquete especificado no existe.");

        // 2. Validar que el paquete está en estado Congelado
        if (paquete.IdEstado != 3) // 3 = Congelado
        {
            var estadoNombre = paquete.IdEstado switch
            {
                1 => "Activo",
                2 => "Vencido",
                4 => "Agotado",
                _ => "Inválido"
            };
            return Result<Unit>.Failure($"El paquete no está congelado (estado actual: {estadoNombre}).");
        }

        // 3. Obtener la congelación
        var congelacion = paquete.Congelaciones?
            .FirstOrDefault(c => c.IdCongelacion == request.IdCongelacion);

        if (congelacion is null)
            return Result<Unit>.Failure("La congelación especificada no existe para este paquete.");

        // 4. Actualizar fecha fin de la congelación al día actual
        var fechaFinReal = DateTime.Today;
        if (fechaFinReal < congelacion.FechaInicio)
            fechaFinReal = congelacion.FechaInicio;

        congelacion.FechaFin = fechaFinReal;

        // 5. Calcular días congelados y extender fecha de vencimiento
        var diasCongelados = (congelacion.FechaFin - congelacion.FechaInicio).Days;
        if (diasCongelados > 0)
        {
            paquete.FechaVencimiento = paquete.FechaVencimiento.AddDays(diasCongelados);
        }

        // 6. Cambiar estado del paquete de vuelta a Activo
        // Validar primero que no esté agotado
        if (paquete.ClasesUsadas >= paquete.ClasesDisponibles)
        {
            paquete.IdEstado = 4; // 4 = Agotado
        }
        else if (paquete.FechaVencimiento < DateTime.Today)
        {
            paquete.IdEstado = 2; // 2 = Vencido
        }
        else
        {
            paquete.IdEstado = 1; // 1 = Activo
        }

        paquete.FechaModificacion = DateTime.Now;
        paquete.UsuarioModificacion = "Sistema"; // TODO: Obtener del contexto de usuario

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

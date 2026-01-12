using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Commands.DescontarClase;

// Command para descontar una clase de un paquete
public record DescontarClaseCommand(
    Guid IdPaquete
) : IRequest<Result<Unit>>;

// Handler
public class DescontarClaseCommandHandler : IRequestHandler<DescontarClaseCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public DescontarClaseCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Unit>> Handle(DescontarClaseCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener el paquete con tracking para actualizarlo
        var paquete = await _db.Set<Paquete>()
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
            return Result<Unit>.Failure($"El paquete no está activo (estado: {estadoNombre}).");
        }

        // 3. Validar que hay clases disponibles
        if (paquete.ClasesUsadas >= paquete.ClasesDisponibles)
            return Result<Unit>.Failure("El paquete no tiene clases disponibles.");

        // 4. Validar que el paquete no está vencido
        if (paquete.FechaVencimiento < DateTime.Today)
            return Result<Unit>.Failure("El paquete está vencido.");

        // 5. Incrementar las clases usadas
        paquete.ClasesUsadas++;
        paquete.FechaModificacion = DateTime.Now;
        paquete.UsuarioModificacion = "Sistema"; // TODO: Obtener del contexto de usuario

        // 6. Si se agotaron las clases, cambiar estado a Agotado
        if (paquete.ClasesUsadas >= paquete.ClasesDisponibles)
        {
            paquete.IdEstado = 4; // 4 = Agotado
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

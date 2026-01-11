using Chetango.Application.Common;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Queries.ValidarPaqueteDisponible;

// Query para validar si un paquete está disponible para usar
public record ValidarPaqueteDisponibleQuery(
    Guid IdPaquete
) : IRequest<Result<bool>>;

// Handler
public class ValidarPaqueteDisponibleQueryHandler : IRequestHandler<ValidarPaqueteDisponibleQuery, Result<bool>>
{
    private readonly IAppDbContext _db;

    public ValidarPaqueteDisponibleQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(ValidarPaqueteDisponibleQuery request, CancellationToken cancellationToken)
    {
        // 1. Obtener el paquete
        var paquete = await _db.Set<Paquete>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPaquete == request.IdPaquete, cancellationToken);

        if (paquete is null)
            return Result<bool>.Failure("El paquete especificado no existe.");

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
            return Result<bool>.Failure($"El paquete no está activo (estado: {estadoNombre}).");
        }

        // 3. Validar que hay clases disponibles
        var clasesRestantes = paquete.ClasesDisponibles - paquete.ClasesUsadas;
        if (clasesRestantes <= 0)
            return Result<bool>.Failure("El paquete no tiene clases disponibles.");

        // 4. Validar que el paquete no está vencido
        if (paquete.FechaVencimiento < DateTime.Today)
            return Result<bool>.Failure("El paquete está vencido.");

        // Todas las validaciones pasaron
        return Result<bool>.Success(true);
    }
}

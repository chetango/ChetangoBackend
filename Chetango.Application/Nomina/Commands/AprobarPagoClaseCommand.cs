using Chetango.Application.Common;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Nomina.Commands;

public record AprobarPagoClaseCommand(
    Guid IdClaseProfesor,
    decimal? ValorAdicional,
    string? ConceptoAdicional,
    Guid AprobadoPorIdUsuario
) : IRequest<Result<bool>>;

public class AprobarPagoClaseHandler : IRequestHandler<AprobarPagoClaseCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public AprobarPagoClaseHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(AprobarPagoClaseCommand request, CancellationToken cancellationToken)
    {
        var claseProfesor = await _db.Set<ClaseProfesor>()
            .FirstOrDefaultAsync(cp => cp.IdClaseProfesor == request.IdClaseProfesor, cancellationToken);

        if (claseProfesor == null)
            return Result<bool>.Failure("Registro de clase-profesor no encontrado");

        if (claseProfesor.EstadoPago != "Pendiente")
            return Result<bool>.Failure($"El pago no está en estado Pendiente (Estado actual: {claseProfesor.EstadoPago})");

        // Aplicar ajustes si hay
        if (request.ValorAdicional.HasValue)
        {
            claseProfesor.ValorAdicional = request.ValorAdicional.Value;
            claseProfesor.ConceptoAdicional = request.ConceptoAdicional;
            claseProfesor.TotalPago = claseProfesor.TarifaProgramada + claseProfesor.ValorAdicional;
        }

        claseProfesor.EstadoPago = "Aprobado";
        claseProfesor.FechaAprobacion = DateTime.Now;
        claseProfesor.AprobadoPorIdUsuario = request.AprobadoPorIdUsuario;
        claseProfesor.FechaModificacion = DateTime.Now;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

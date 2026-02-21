using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Application.Pagos.Commands;

public class EliminarPagoCommandHandler : IRequestHandler<EliminarPagoCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public EliminarPagoCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(EliminarPagoCommand request, CancellationToken cancellationToken)
    {
        // Buscar el pago (IgnoreQueryFilters para poder eliminar pagos que ya estén marcados como eliminados)
        var pago = await _db.Set<Pago>()
            .IgnoreQueryFilters()
            .Include(p => p.Paquetes)
                .ThenInclude(paq => paq.Estado)
            .FirstOrDefaultAsync(p => p.IdPago == request.IdPago, cancellationToken);

        if (pago == null)
        {
            return Result<Unit>.Failure("El pago especificado no existe.");
        }

        // Validar si ya está eliminado
        if (pago.Eliminado)
        {
            return Result<Unit>.Failure("El pago ya fue eliminado previamente.");
        }

        // Validar si tiene paquetes asociados
        if (pago.Paquetes != null && pago.Paquetes.Any())
        {
            // Verificar si algún paquete tiene asistencias registradas
            var paquetesConAsistencias = new List<Guid>();
            
            foreach (var paquete in pago.Paquetes)
            {
                var tieneAsistencias = await _db.Set<Asistencia>()
                    .AnyAsync(a => a.IdPaqueteUsado == paquete.IdPaquete, cancellationToken);
                
                if (tieneAsistencias)
                {
                    paquetesConAsistencias.Add(paquete.IdPaquete);
                }
            }

            if (paquetesConAsistencias.Any())
            {
                return Result<Unit>.Failure(
                    $"No se puede eliminar el pago porque tiene {paquetesConAsistencias.Count} paquete(s) con asistencias registradas. " +
                    "Esto afectaría la integridad de los registros históricos."
                );
            }

            // Nota: Los paquetes asociados NO se eliminan ni cambian de estado
            // Se mantienen como están para preservar el historial
            // Solo se marca el pago como eliminado (soft delete)
        }

        // Realizar soft delete del pago
        pago.Eliminado = true;
        pago.FechaEliminacion = DateTimeHelper.Now;
        pago.UsuarioEliminacion = request.EmailUsuarioEliminador;
        pago.FechaModificacion = DateTimeHelper.Now;
        pago.UsuarioModificacion = request.EmailUsuarioEliminador;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

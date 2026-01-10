using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Clases.Commands.CancelarClase;

public class CancelarClaseCommandHandler : IRequestHandler<CancelarClaseCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public CancelarClaseCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(CancelarClaseCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que la clase existe
        var clase = await _db.Set<Chetango.Domain.Entities.Clase>()
            .Include(c => c.ProfesorPrincipal)
            .ThenInclude(p => p.Usuario)
            .FirstOrDefaultAsync(c => c.IdClase == request.IdClase, cancellationToken);

        if (clase is null)
            return Result<bool>.Failure("La clase especificada no existe.");

        // 2. Validación de ownership: Profesor solo puede cancelar sus clases
        if (!request.EsAdmin)
        {
            if (clase.ProfesorPrincipal.IdUsuario.ToString() != request.IdUsuarioActual)
                return Result<bool>.Failure("No tienes permiso para cancelar esta clase.");
        }

        // 3. Validar que la clase aún no ha pasado
        var fechaHoraInicio = clase.Fecha.Date.Add(clase.HoraInicio);
        if (fechaHoraInicio <= DateTime.Now)
            return Result<bool>.Failure("No se puede cancelar una clase que ya ha comenzado o pasado.");

        // 4. Verificar si hay asistencias registradas (opcional: podemos permitir cancelar con asistencias)
        var tieneAsistencias = await _db.Asistencias
            .AnyAsync(a => a.IdClase == request.IdClase, cancellationToken);

        if (tieneAsistencias)
            return Result<bool>.Failure("No se puede cancelar una clase con asistencias registradas. Contacte al administrador.");

        // 5. Eliminar la clase (soft delete podría ser mejor, pero por ahora eliminamos)
        _db.Set<Chetango.Domain.Entities.Clase>().Remove(clase);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

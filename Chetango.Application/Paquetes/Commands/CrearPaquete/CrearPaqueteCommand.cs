using Chetango.Application.Common;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Paquetes.Commands.CrearPaquete;

// Command para crear un nuevo paquete
public record CrearPaqueteCommand(
    Guid IdAlumno,
    Guid IdTipoPaquete,
    int ClasesDisponibles,
    decimal ValorPaquete,
    int DiasVigencia,
    Guid? IdPago = null
) : IRequest<Result<Guid>>;

// Handler
public class CrearPaqueteCommandHandler : IRequestHandler<CrearPaqueteCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public CrearPaqueteCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CrearPaqueteCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que el alumno existe y está activo
        var alumno = await _db.Set<Chetango.Domain.Entities.Alumno>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (alumno is null)
            return Result<Guid>.Failure("El alumno especificado no existe.");

        if (alumno.IdEstado != 1)
            return Result<Guid>.Failure("El alumno no está activo.");

        // 2. Validar que el tipo de paquete existe
        var tipoPaqueteExiste = await _db.Set<TipoPaquete>()
            .AsNoTracking()
            .AnyAsync(tp => tp.Id == request.IdTipoPaquete, cancellationToken);

        if (!tipoPaqueteExiste)
            return Result<Guid>.Failure("El tipo de paquete especificado no existe.");

        // 3. Si se proporciona IdPago, validar que existe
        if (request.IdPago.HasValue)
        {
            var pagoExiste = await _db.Set<Pago>()
                .AsNoTracking()
                .AnyAsync(p => p.IdPago == request.IdPago.Value, cancellationToken);

            if (!pagoExiste)
                return Result<Guid>.Failure("El pago especificado no existe.");
        }

        // 4. Crear el paquete
        var fechaActivacion = DateTime.Today;
        var fechaVencimiento = fechaActivacion.AddDays(request.DiasVigencia);

        var paquete = new Paquete
        {
            IdPaquete = Guid.NewGuid(),
            IdAlumno = request.IdAlumno,
            IdTipoPaquete = request.IdTipoPaquete,
            IdPago = request.IdPago,
            ClasesDisponibles = request.ClasesDisponibles,
            ClasesUsadas = 0,
            FechaActivacion = fechaActivacion,
            FechaVencimiento = fechaVencimiento,
            IdEstado = 1, // 1 = Activo
            ValorPaquete = request.ValorPaquete,
            FechaCreacion = DateTime.Now,
            UsuarioCreacion = "Sistema" // TODO: Obtener del contexto de usuario
        };

        _db.Set<Paquete>().Add(paquete);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(paquete.IdPaquete);
    }
}

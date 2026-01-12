using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Application.Pagos.Commands;

public class RegistrarPagoCommandHandler : IRequestHandler<RegistrarPagoCommand, Result<RegistrarPagoResponseDTO>>
{
    private readonly IAppDbContext _db;

    public RegistrarPagoCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<RegistrarPagoResponseDTO>> Handle(RegistrarPagoCommand request, CancellationToken cancellationToken)
    {
        // Validaciones inline
        if (request.MontoTotal <= 0)
        {
            return Result<RegistrarPagoResponseDTO>.Failure("El monto total debe ser mayor a cero.");
        }

        if (request.Paquetes == null || request.Paquetes.Count == 0)
        {
            return Result<RegistrarPagoResponseDTO>.Failure("Debe especificar al menos un paquete a crear.");
        }

        // Validar que el alumno existe
        var alumno = await _db.Set<Alumno>().FindAsync(new object[] { request.IdAlumno }, cancellationToken);
        if (alumno == null)
        {
            return Result<RegistrarPagoResponseDTO>.Failure("El alumno especificado no existe.");
        }

        // Validar que el método de pago existe
        var metodoPago = await _db.Set<MetodoPago>().FindAsync(new object[] { request.IdMetodoPago }, cancellationToken);
        if (metodoPago == null)
        {
            return Result<RegistrarPagoResponseDTO>.Failure("El método de pago especificado no existe.");
        }

        // Validar que todos los tipos de paquete existen
        var idsTiposPaquete = request.Paquetes.Select(p => p.IdTipoPaquete).Distinct().ToList();
        var tiposPaquete = await _db.Set<TipoPaquete>()
            .Where(tp => idsTiposPaquete.Contains(tp.Id))
            .ToListAsync(cancellationToken);

        if (tiposPaquete.Count != idsTiposPaquete.Count)
        {
            return Result<RegistrarPagoResponseDTO>.Failure("Uno o más tipos de paquete especificados no existen.");
        }

        // Validar suma de valores de paquetes si se especificaron
        var paquetesConValor = request.Paquetes.Where(p => p.ValorPaquete.HasValue).ToList();
        if (paquetesConValor.Any())
        {
            var sumaValores = paquetesConValor.Sum(p => p.ValorPaquete!.Value);
            if (sumaValores > request.MontoTotal)
            {
                return Result<RegistrarPagoResponseDTO>.Failure("La suma de los valores de los paquetes no puede ser mayor al monto total del pago.");
            }
        }

        // Iniciar transacción
        // Cast a DbContext para acceder a Database
        var dbContext = _db as DbContext;
        if (dbContext == null)
        {
            return Result<RegistrarPagoResponseDTO>.Failure("Error al iniciar transacción.");
        }

        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Crear el pago
            var pago = new Pago
            {
                IdPago = Guid.NewGuid(),
                IdAlumno = request.IdAlumno,
                FechaPago = request.FechaPago,
                MontoTotal = request.MontoTotal,
                IdMetodoPago = request.IdMetodoPago,
                Nota = request.Nota,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = "Sistema"
            };

            _db.Set<Pago>().Add(pago);
            await _db.SaveChangesAsync(cancellationToken);

            // Calcular valor por paquete si no se especificó
            var valorPorPaquetePorDefecto = request.MontoTotal / request.Paquetes.Count;

            // Crear los paquetes asociados
            var idsPaquetesCreados = new List<Guid>();

            foreach (var paqueteDTO in request.Paquetes)
            {
                var tipoPaquete = tiposPaquete.First(tp => tp.Id == paqueteDTO.IdTipoPaquete);

                var paquete = new Paquete
                {
                    IdPaquete = Guid.NewGuid(),
                    IdAlumno = request.IdAlumno,
                    IdPago = pago.IdPago, // Vincular al pago
                    IdTipoPaquete = paqueteDTO.IdTipoPaquete,
                    ClasesDisponibles = paqueteDTO.ClasesDisponibles,
                    ClasesUsadas = 0,
                    FechaActivacion = request.FechaPago,
                    FechaVencimiento = request.FechaPago.AddDays(paqueteDTO.DiasVigencia),
                    IdEstado = 1, // Activo
                    ValorPaquete = paqueteDTO.ValorPaquete ?? valorPorPaquetePorDefecto,
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioCreacion = "Sistema"
                };

                _db.Set<Paquete>().Add(paquete);
                idsPaquetesCreados.Add(paquete.IdPaquete);
            }

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new RegistrarPagoResponseDTO(
                pago.IdPago,
                idsPaquetesCreados,
                pago.MontoTotal
            );

            return Result<RegistrarPagoResponseDTO>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<RegistrarPagoResponseDTO>.Failure($"Error al registrar el pago: {ex.Message}");
        }
    }
}

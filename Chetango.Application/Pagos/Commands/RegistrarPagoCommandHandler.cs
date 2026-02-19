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
        // Obtener sede del usuario creador si no se especificó
        var sedeAUsar = request.Sede;
        if (!sedeAUsar.HasValue && !string.IsNullOrEmpty(request.EmailUsuarioCreador))
        {
            var usuarioCreador = await _db.Set<Usuario>()
                .FirstOrDefaultAsync(u => u.Correo == request.EmailUsuarioCreador, cancellationToken);
            sedeAUsar = usuarioCreador?.Sede;
        }
        // Si aún no hay sede, usar Medellín por defecto
        sedeAUsar ??= Domain.Enums.Sede.Medellin;

        // Validaciones inline
        if (request.MontoTotal <= 0)
        {
            return Result<RegistrarPagoResponseDTO>.Failure("El monto total debe ser mayor a cero.");
        }

        // Validar que al menos haya paquetes nuevos o existentes
        if ((request.Paquetes == null || request.Paquetes.Count == 0) && 
            (request.IdsPaquetesExistentes == null || request.IdsPaquetesExistentes.Count == 0))
        {
            return Result<RegistrarPagoResponseDTO>.Failure("Debe especificar al menos un paquete nuevo o vincular paquetes existentes.");
        }

        // Validar que todos los alumnos en los paquetes existen
        var idsAlumnos = request.Paquetes?.Select(p => p.IdAlumno).Distinct().ToList() ?? new();
        if (request.IdAlumno.HasValue && !idsAlumnos.Contains(request.IdAlumno.Value))
        {
            idsAlumnos.Add(request.IdAlumno.Value);
        }
        
        if (!idsAlumnos.Any())
        {
            return Result<RegistrarPagoResponseDTO>.Failure("Debe especificar al menos un alumno.");
        }

        var alumnos = await _db.Set<Alumno>()
            .Where(a => idsAlumnos.Contains(a.IdAlumno))
            .ToListAsync(cancellationToken);
        
        if (alumnos.Count != idsAlumnos.Count)
        {
            return Result<RegistrarPagoResponseDTO>.Failure("Uno o más alumnos especificados no existen.");
        }

        // Validar que el método de pago existe
        var metodoPago = await _db.Set<MetodoPago>().FindAsync(new object[] { request.IdMetodoPago }, cancellationToken);
        if (metodoPago == null)
        {
            return Result<RegistrarPagoResponseDTO>.Failure("El método de pago especificado no existe.");
        }

        // Validar paquetes existentes si se especificaron
        List<Paquete> paquetesExistentes = new();
        if (request.IdsPaquetesExistentes != null && request.IdsPaquetesExistentes.Any())
        {
            paquetesExistentes = await _db.Set<Paquete>()
                .Where(p => request.IdsPaquetesExistentes.Contains(p.IdPaquete) && p.IdPago == null)
                .ToListAsync(cancellationToken);

            if (paquetesExistentes.Count != request.IdsPaquetesExistentes.Count)
            {
                return Result<RegistrarPagoResponseDTO>.Failure("Uno o más paquetes especificados no existen o ya tienen pago asociado.");
            }
        }

        // Validar que todos los tipos de paquete existen (solo para nuevos)
        List<TipoPaquete> tiposPaquete = new();
        if (request.Paquetes != null && request.Paquetes.Any())
        {
            var idsTiposPaquete = request.Paquetes.Select(p => p.IdTipoPaquete).Distinct().ToList();
            tiposPaquete = await _db.Set<TipoPaquete>()
                .Where(tp => idsTiposPaquete.Contains(tp.Id))
                .ToListAsync(cancellationToken);

            if (tiposPaquete.Count != idsTiposPaquete.Count)
            {
                return Result<RegistrarPagoResponseDTO>.Failure("Uno o más tipos de paquete especificados no existen.");
            }
        }

        // Validar suma de valores de paquetes si se especificaron
        var paquetesConValor = request.Paquetes?.Where(p => p.ValorPaquete.HasValue).ToList() ?? new();
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
            // Obtener el estado "Pendiente Verificación" por defecto
            var estadoPendiente = await _db.Set<EstadoPago>()
                .FirstOrDefaultAsync(e => e.Nombre == "Pendiente Verificación", cancellationToken);

            if (estadoPendiente == null)
            {
                return Result<RegistrarPagoResponseDTO>.Failure("Estado 'Pendiente Verificación' no encontrado en el sistema.");
            }

            // Crear el pago (IdAlumno puede ser null para pagos compartidos)
            var pago = new Pago
            {
                IdPago = Guid.NewGuid(),
                IdAlumno = request.IdAlumno ?? idsAlumnos.FirstOrDefault(),
                FechaPago = request.FechaPago,
                MontoTotal = request.MontoTotal,
                IdMetodoPago = request.IdMetodoPago,
                IdEstadoPago = estadoPendiente.Id,
                Sede = sedeAUsar.Value,
                ReferenciaTransferencia = request.ReferenciaTransferencia,
                UrlComprobante = request.UrlComprobante,
                Nota = request.Nota,
                FechaCreacion = DateTimeHelper.Now,
                UsuarioCreacion = "Sistema"
            };

            _db.Set<Pago>().Add(pago);
            await _db.SaveChangesAsync(cancellationToken);

            var idsPaquetes = new List<Guid>();

            // Vincular paquetes existentes si se especificaron
            if (paquetesExistentes.Any())
            {
                foreach (var paquete in paquetesExistentes)
                {
                    paquete.IdPago = pago.IdPago;
                    paquete.FechaModificacion = DateTimeHelper.Now;
                    paquete.UsuarioModificacion = "Sistema";
                    _db.Set<Paquete>().Update(paquete);
                    idsPaquetes.Add(paquete.IdPaquete);
                }
            }

            // Crear nuevos paquetes si se especificaron
            if (request.Paquetes != null && request.Paquetes.Any())
            {
                var totalPaquetes = request.Paquetes.Count + paquetesExistentes.Count;
                var valorPorPaquetePorDefecto = request.MontoTotal / totalPaquetes;

                foreach (var paqueteDTO in request.Paquetes)
                {
                    var tipoPaquete = tiposPaquete.First(tp => tp.Id == paqueteDTO.IdTipoPaquete);

                    var paquete = new Paquete
                    {
                        IdPaquete = Guid.NewGuid(),
                        IdAlumno = paqueteDTO.IdAlumno, // Usar IdAlumno del paquete
                        IdPago = pago.IdPago,
                        IdTipoPaquete = paqueteDTO.IdTipoPaquete,
                        ClasesDisponibles = paqueteDTO.ClasesDisponibles,
                        ClasesUsadas = 0,
                        FechaActivacion = request.FechaPago,
                        FechaVencimiento = request.FechaPago.AddDays(paqueteDTO.DiasVigencia),
                        IdEstado = 1, // Activo
                        ValorPaquete = paqueteDTO.ValorPaquete ?? valorPorPaquetePorDefecto,
                        FechaCreacion = DateTimeHelper.Now,
                        UsuarioCreacion = "Sistema"
                    };

                    _db.Set<Paquete>().Add(paquete);
                    idsPaquetes.Add(paquete.IdPaquete);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new RegistrarPagoResponseDTO(
                pago.IdPago,
                idsPaquetes,
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

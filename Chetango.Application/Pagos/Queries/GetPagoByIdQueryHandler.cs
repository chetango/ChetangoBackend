using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Pagos.Queries;

public class GetPagoByIdQueryHandler : IRequestHandler<GetPagoByIdQuery, Result<PagoDetalleDTO>>
{
    private readonly IAppDbContext _db;

    public GetPagoByIdQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagoDetalleDTO>> Handle(GetPagoByIdQuery request, CancellationToken cancellationToken)
    {
        var pago = await _db.Set<Pago>()
            .Include(p => p.Alumno)
            .ThenInclude(a => a.Usuario)
            .Include(p => p.MetodoPago)
            .Include(p => p.EstadoPago)
            .Include(p => p.Paquetes)
            .ThenInclude(paq => paq.TipoPaquete)
            .Include(p => p.Paquetes)
            .ThenInclude(paq => paq.Estado)
            .Include(p => p.Paquetes)
            .ThenInclude(paq => paq.Alumno)
            .ThenInclude(a => a.Usuario)
            .FirstOrDefaultAsync(p => p.IdPago == request.IdPago, cancellationToken);

        if (pago == null)
        {
            return Result<PagoDetalleDTO>.Failure("El pago especificado no existe.");
        }

        // Ownership validation por email
        if (!request.EsAdmin)
        {
            var alumno = await _db.Set<Alumno>()
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Usuario.Correo == request.EmailUsuario, cancellationToken);

            if (alumno == null)
            {
                return Result<PagoDetalleDTO>.Failure("Usuario no encontrado como alumno.");
            }

            if (pago.IdAlumno != alumno.IdAlumno)
            {
                return Result<PagoDetalleDTO>.Failure("No tienes permiso para ver este pago.");
            }
        }

        var pagoDetalle = new PagoDetalleDTO(
            pago.IdPago,
            pago.IdAlumno,
            pago.Alumno?.Usuario?.NombreUsuario,
            pago.Alumno?.Usuario?.Correo,
            pago.Alumno?.Usuario?.Telefono,
            pago.Alumno?.AvatarUrl,
            pago.FechaPago,
            pago.MontoTotal,
            pago.IdMetodoPago,
            pago.MetodoPago.Nombre,
            pago.ReferenciaTransferencia,
            pago.Nota,
            pago.EstadoPago.Nombre,
            pago.UrlComprobante,
            pago.NotasVerificacion,
            pago.FechaVerificacion,
            pago.UsuarioVerificacion,
            pago.FechaCreacion,
            pago.Paquetes.Select(paq => new PaqueteResumenDTO(
                paq.IdPaquete,
                paq.IdAlumno,
                paq.Alumno.Usuario.NombreUsuario,
                paq.TipoPaquete.Nombre,
                paq.ClasesDisponibles,
                paq.ClasesUsadas,
                paq.ClasesDisponibles - paq.ClasesUsadas,
                paq.FechaVencimiento,
                paq.Estado.Nombre,
                paq.ValorPaquete
            )).ToList()
        );

        return Result<PagoDetalleDTO>.Success(pagoDetalle);
    }
}

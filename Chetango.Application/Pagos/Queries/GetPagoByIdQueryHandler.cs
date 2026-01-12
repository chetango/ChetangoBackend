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
            .Include(p => p.Alumno.Usuario)
            .Include(p => p.MetodoPago)
            .Include(p => p.Paquetes)
            .ThenInclude(paq => paq.TipoPaquete)
            .Include(p => p.Paquetes)
            .ThenInclude(paq => paq.Estado)
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
            pago.Alumno.Usuario.NombreUsuario,
            pago.Alumno.Usuario.Correo,
            pago.FechaPago,
            pago.MontoTotal,
            pago.IdMetodoPago,
            pago.MetodoPago.Nombre,
            pago.Nota,
            pago.FechaCreacion,
            pago.Paquetes.Select(paq => new PaqueteResumenDTO(
                paq.IdPaquete,
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

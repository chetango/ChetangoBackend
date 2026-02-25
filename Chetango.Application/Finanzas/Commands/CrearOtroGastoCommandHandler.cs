using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Finanzas.Commands;

public class CrearOtroGastoCommandHandler : IRequestHandler<CrearOtroGastoCommand, Result<OtroGastoDTO>>
{
    private readonly IAppDbContext _db;

    public CrearOtroGastoCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<OtroGastoDTO>> Handle(CrearOtroGastoCommand request, CancellationToken cancellationToken)
    {
        // Validaciones
        if (request.Monto <= 0)
        {
            return Result<OtroGastoDTO>.Failure("El monto debe ser mayor a cero.");
        }

        if (request.Fecha > DateTime.Now)
        {
            return Result<OtroGastoDTO>.Failure("La fecha no puede ser futura.");
        }

        if (string.IsNullOrWhiteSpace(request.Concepto))
        {
            return Result<OtroGastoDTO>.Failure("El concepto es requerido.");
        }

        // Validar categoría si se especificó
        string? nombreCategoria = null;
        if (request.IdCategoriaGasto.HasValue)
        {
            var categoria = await _db.Set<Domain.Entities.Estados.CategoriaGasto>()
                .FindAsync(new object[] { request.IdCategoriaGasto.Value }, cancellationToken);
            
            if (categoria == null)
            {
                return Result<OtroGastoDTO>.Failure("La categoría especificada no existe.");
            }
            nombreCategoria = categoria.Nombre;
        }

        // Crear entidad
        var otroGasto = new OtroGasto
        {
            IdOtroGasto = Guid.NewGuid(),
            Concepto = request.Concepto.Trim(),
            Monto = request.Monto,
            Fecha = request.Fecha,
            Sede = request.Sede,
            IdCategoriaGasto = request.IdCategoriaGasto,
            Proveedor = request.Proveedor?.Trim(),
            Descripcion = request.Descripcion?.Trim(),
            UrlFactura = request.UrlFactura?.Trim(),
            NumeroFactura = request.NumeroFactura?.Trim(),
            FechaCreacion = DateTime.Now,
            UsuarioCreacion = request.EmailUsuarioCreador ?? "Sistema"
        };

        _db.OtrosGastos.Add(otroGasto);
        await _db.SaveChangesAsync(cancellationToken);

        // Retornar DTO
        var dto = new OtroGastoDTO
        {
            IdOtroGasto = otroGasto.IdOtroGasto,
            Concepto = otroGasto.Concepto,
            Monto = otroGasto.Monto,
            Fecha = otroGasto.Fecha,
            Sede = otroGasto.Sede,
            IdCategoriaGasto = otroGasto.IdCategoriaGasto,
            NombreCategoria = nombreCategoria,
            Proveedor = otroGasto.Proveedor,
            Descripcion = otroGasto.Descripcion,
            UrlFactura = otroGasto.UrlFactura,
            NumeroFactura = otroGasto.NumeroFactura,
            FechaCreacion = otroGasto.FechaCreacion,
            UsuarioCreacion = otroGasto.UsuarioCreacion
        };

        return Result<OtroGastoDTO>.Success(dto);
    }
}

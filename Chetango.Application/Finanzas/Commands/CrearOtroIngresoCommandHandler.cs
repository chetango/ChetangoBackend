using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;
using Chetango.Domain.Entities;

namespace Chetango.Application.Finanzas.Commands;

public class CrearOtroIngresoCommandHandler : IRequestHandler<CrearOtroIngresoCommand, Result<OtroIngresoDTO>>
{
    private readonly IAppDbContext _db;

    public CrearOtroIngresoCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<OtroIngresoDTO>> Handle(CrearOtroIngresoCommand request, CancellationToken cancellationToken)
    {
        // Validaciones
        if (request.Monto <= 0)
        {
            return Result<OtroIngresoDTO>.Failure("El monto debe ser mayor a cero.");
        }

        if (request.Fecha > DateTime.Now)
        {
            return Result<OtroIngresoDTO>.Failure("La fecha no puede ser futura.");
        }

        if (string.IsNullOrWhiteSpace(request.Concepto))
        {
            return Result<OtroIngresoDTO>.Failure("El concepto es requerido.");
        }

        // Validar categoría si se especificó
        string? nombreCategoria = null;
        if (request.IdCategoriaIngreso.HasValue)
        {
            var categoria = await _db.Set<Domain.Entities.Estados.CategoriaIngreso>()
                .FindAsync(new object[] { request.IdCategoriaIngreso.Value }, cancellationToken);
            
            if (categoria == null)
            {
                return Result<OtroIngresoDTO>.Failure("La categoría especificada no existe.");
            }
            nombreCategoria = categoria.Nombre;
        }

        // Crear entidad
        var otroIngreso = new OtroIngreso
        {
            IdOtroIngreso = Guid.NewGuid(),
            Concepto = request.Concepto.Trim(),
            Monto = request.Monto,
            Fecha = request.Fecha,
            Sede = request.Sede,
            IdCategoriaIngreso = request.IdCategoriaIngreso,
            Descripcion = request.Descripcion?.Trim(),
            UrlComprobante = request.UrlComprobante?.Trim(),
            FechaCreacion = DateTime.Now,
            UsuarioCreacion = request.EmailUsuarioCreador ?? "Sistema"
        };

        _db.OtrosIngresos.Add(otroIngreso);
        await _db.SaveChangesAsync(cancellationToken);

        // Retornar DTO
        var dto = new OtroIngresoDTO
        {
            IdOtroIngreso = otroIngreso.IdOtroIngreso,
            Concepto = otroIngreso.Concepto,
            Monto = otroIngreso.Monto,
            Fecha = otroIngreso.Fecha,
            Sede = otroIngreso.Sede,
            IdCategoriaIngreso = otroIngreso.IdCategoriaIngreso,
            NombreCategoria = nombreCategoria,
            Descripcion = otroIngreso.Descripcion,
            UrlComprobante = otroIngreso.UrlComprobante,
            FechaCreacion = otroIngreso.FechaCreacion,
            UsuarioCreacion = otroIngreso.UsuarioCreacion
        };

        return Result<OtroIngresoDTO>.Success(dto);
    }
}

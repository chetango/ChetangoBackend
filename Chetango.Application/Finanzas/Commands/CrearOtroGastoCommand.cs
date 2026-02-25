using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;
using Chetango.Domain.Enums;

namespace Chetango.Application.Finanzas.Commands;

/// <summary>
/// Command para crear un nuevo gasto adicional
/// </summary>
public record CrearOtroGastoCommand(
    string Concepto,
    decimal Monto,
    DateTime Fecha,
    Sede Sede,
    Guid? IdCategoriaGasto,
    string? Proveedor,
    string? Descripcion,
    string? UrlFactura,
    string? NumeroFactura,
    string? EmailUsuarioCreador
) : IRequest<Result<OtroGastoDTO>>;

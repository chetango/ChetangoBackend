using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;
using Chetango.Domain.Enums;

namespace Chetango.Application.Finanzas.Commands;

/// <summary>
/// Command para crear un nuevo ingreso adicional
/// </summary>
public record CrearOtroIngresoCommand(
    string Concepto,
    decimal Monto,
    DateTime Fecha,
    Sede Sede,
    Guid? IdCategoriaIngreso,
    string? Descripcion,
    string? UrlComprobante,
    string? EmailUsuarioCreador
) : IRequest<Result<OtroIngresoDTO>>;

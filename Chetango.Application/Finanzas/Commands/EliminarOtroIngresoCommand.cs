using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Finanzas.Commands;

/// <summary>
/// Command para eliminar (soft delete) un ingreso adicional
/// </summary>
public record EliminarOtroIngresoCommand(
    Guid IdOtroIngreso,
    string? EmailUsuarioEliminador
) : IRequest<Result<bool>>;

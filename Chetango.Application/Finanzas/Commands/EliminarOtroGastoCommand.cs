using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Finanzas.Commands;

/// <summary>
/// Command para eliminar (soft delete) un gasto adicional
/// </summary>
public record EliminarOtroGastoCommand(
    Guid IdOtroGasto,
    string? EmailUsuarioEliminador
) : IRequest<Result<bool>>;

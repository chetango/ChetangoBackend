using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;

namespace Chetango.Application.Finanzas.Queries;

/// <summary>
/// Query para obtener catálogo de categorías de gastos
/// </summary>
public record GetCategoriasGastoQuery() : IRequest<Result<List<CategoriaGastoDTO>>>;

using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;
using Chetango.Domain.Enums;

namespace Chetango.Application.Finanzas.Queries;

/// <summary>
/// Query para obtener listado de otros gastos con filtros
/// </summary>
public record GetOtrosGastosQuery(
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    Sede? Sede = null,
    Guid? IdCategoriaGasto = null
) : IRequest<Result<List<OtroGastoDTO>>>;

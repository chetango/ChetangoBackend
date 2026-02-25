using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Finanzas.DTOs;
using Chetango.Domain.Enums;

namespace Chetango.Application.Finanzas.Queries;

/// <summary>
/// Query para obtener listado de otros ingresos con filtros
/// </summary>
public record GetOtrosIngresosQuery(
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    Sede? Sede = null,
    Guid? IdCategoriaIngreso = null
) : IRequest<Result<List<OtroIngresoDTO>>>;

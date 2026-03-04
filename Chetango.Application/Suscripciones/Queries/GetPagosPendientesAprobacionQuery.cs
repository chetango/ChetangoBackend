using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Query para obtener todos los pagos pendientes de aprobación.
/// Solo para SuperAdmin.
/// </summary>
public record GetPagosPendientesAprobacionQuery() : IRequest<Result<List<PagoSuscripcionDto>>>;

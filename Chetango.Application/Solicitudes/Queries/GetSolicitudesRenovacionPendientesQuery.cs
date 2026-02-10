using Chetango.Application.Common;
using Chetango.Application.Solicitudes.DTOs;
using MediatR;

namespace Chetango.Application.Solicitudes.Queries;

/// <summary>
/// Query para obtener solicitudes pendientes de renovación de paquete (Admin)
/// </summary>
public record GetSolicitudesRenovacionPendientesQuery : IRequest<Result<List<SolicitudRenovacionPaqueteDTO>>>;

using Chetango.Application.Common;
using Chetango.Application.Solicitudes.DTOs;
using MediatR;

namespace Chetango.Application.Solicitudes.Queries;

/// <summary>
/// Query para obtener solicitudes pendientes de clase privada (Admin)
/// </summary>
public record GetSolicitudesClasePrivadaPendientesQuery : IRequest<Result<List<SolicitudClasePrivadaDTO>>>;

using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Nomina.DTOs;

namespace Chetango.Application.Nomina.Queries;

public record GetClasesRealizadasQuery(
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    Guid? IdProfesor,
    string? EstadoPago // null = todas, "Pendiente", "Aprobado", etc.
) : IRequest<Result<List<ClaseRealizadaDTO>>>;

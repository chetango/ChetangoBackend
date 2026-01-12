using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;

namespace Chetango.Application.Pagos.Commands;

public record RegistrarPagoCommand(
    Guid IdAlumno,
    DateTime FechaPago,
    decimal MontoTotal,
    Guid IdMetodoPago,
    string? Nota,
    List<PaqueteParaCrearDTO> Paquetes
) : IRequest<Result<RegistrarPagoResponseDTO>>;

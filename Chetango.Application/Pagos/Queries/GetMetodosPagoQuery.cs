using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Pagos.DTOs;

namespace Chetango.Application.Pagos.Queries;

public record GetMetodosPagoQuery : IRequest<Result<List<MetodoPagoDTO>>>;

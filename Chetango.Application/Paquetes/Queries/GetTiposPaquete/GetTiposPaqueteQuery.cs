using Chetango.Application.Common;
using Chetango.Application.Paquetes.DTOs;
using MediatR;

namespace Chetango.Application.Paquetes.Queries.GetTiposPaquete;

public record GetTiposPaqueteQuery() : IRequest<Result<List<TipoPaqueteDTO>>>;

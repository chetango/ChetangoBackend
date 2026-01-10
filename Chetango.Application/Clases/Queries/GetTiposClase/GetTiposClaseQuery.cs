using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Queries.GetTiposClase;

// Query para obtener todos los tipos de clase disponibles
public record GetTiposClaseQuery() : IRequest<Result<List<TipoClaseDTO>>>;

using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Queries.GetProfesores;

// Query para obtener todos los profesores
public record GetProfesoresQuery() : IRequest<Result<List<ProfesorDTO>>>;

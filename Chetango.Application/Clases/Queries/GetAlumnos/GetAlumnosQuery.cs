using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Queries.GetAlumnos;

// Query para obtener todos los alumnos
public record GetAlumnosQuery() : IRequest<Result<List<AlumnoDTO>>>;

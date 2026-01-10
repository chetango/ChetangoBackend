using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Clases.Queries.GetPaquetesDeAlumno;

// Query para obtener los paquetes de un alumno
public record GetPaquetesDeAlumnoQuery(
    Guid IdAlumno,
    bool SoloActivos = true
) : IRequest<Result<List<PaqueteAlumnoDTO>>>;

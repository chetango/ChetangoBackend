using MediatR;
using Chetango.Application.Common;

namespace Chetango.Application.Clases.Queries.GetClasesDeAlumno;

// Query para obtener clases asociadas a un alumno (historial o próximas)
public record GetClasesDeAlumnoQuery(Guid IdAlumno, DateTime? Desde, DateTime? Hasta) : IRequest<Result<IReadOnlyList<ClaseDto>>>;

public record ClaseDto(Guid IdClase, DateTime Fecha, string Tipo, TimeSpan HoraInicio, TimeSpan HoraFin, string ProfesorPrincipal);

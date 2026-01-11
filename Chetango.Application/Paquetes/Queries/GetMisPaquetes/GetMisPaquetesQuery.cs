using Chetango.Application.Common;
using Chetango.Application.Paquetes.DTOs;
using MediatR;

namespace Chetango.Application.Paquetes.Queries.GetMisPaquetes;

// Query para que el alumno obtenga sus propios paquetes (usa correo del JWT)
public record GetMisPaquetesQuery(
    string CorreoUsuario,
    int? Estado = null,
    Guid? IdTipoPaquete = null
) : IRequest<Result<List<PaqueteAlumnoDTO>>>;

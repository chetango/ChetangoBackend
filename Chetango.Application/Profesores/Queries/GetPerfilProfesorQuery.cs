// ============================================
// GET PERFIL PROFESOR QUERY
// ============================================

using Chetango.Application.Common;
using Chetango.Application.Profesores.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Chetango.Application.Profesores.Queries;

public record GetPerfilProfesorQuery(string EmailUsuario) : IRequest<Result<ProfesorProfileDTO>>;

public class GetPerfilProfesorQueryHandler : IRequestHandler<GetPerfilProfesorQuery, Result<ProfesorProfileDTO>>
{
    private readonly IAppDbContext _db;

    public GetPerfilProfesorQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ProfesorProfileDTO>> Handle(GetPerfilProfesorQuery request, CancellationToken cancellationToken)
    {
        var profesor = await _db.Set<Chetango.Domain.Entities.Estados.Profesor>()
            .Include(p => p.Usuario)
                .ThenInclude(u => u.TipoDocumento)
            .Include(p => p.TipoProfesor)
            .FirstOrDefaultAsync(p => p.Usuario.Correo == request.EmailUsuario, cancellationToken);

        if (profesor == null)
            return Result<ProfesorProfileDTO>.Failure("Profesor no encontrado");

        // Parse especialidades JSON
        var especialidades = new List<string>();
        if (!string.IsNullOrEmpty(profesor.Especialidades))
        {
            try
            {
                especialidades = JsonSerializer.Deserialize<List<string>>(profesor.Especialidades) ?? new List<string>();
            }
            catch
            {
                especialidades = new List<string>();
            }
        }

        var dto = new ProfesorProfileDTO(
            profesor.IdProfesor,
            profesor.Usuario.NombreUsuario,
            profesor.Usuario.Correo,
            profesor.Usuario.Telefono ?? string.Empty,
            profesor.Usuario.TipoDocumento.Nombre,
            profesor.Usuario.NumeroDocumento ?? string.Empty,
            profesor.TipoProfesor.Nombre,
            profesor.Usuario.FechaCreacion,
            profesor.Biografia,
            especialidades,
            new ConfiguracionProfesorDTO(
                profesor.NotificacionesEmail,
                profesor.RecordatoriosClase,
                profesor.AlertasCambios
            )
        );

        return Result<ProfesorProfileDTO>.Success(dto);
    }
}

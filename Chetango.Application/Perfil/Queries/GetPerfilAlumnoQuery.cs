// ============================================
// GET PERFIL ALUMNO QUERY
// ============================================

using Chetango.Application.Common;
using Chetango.Application.Perfil.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Perfil.Queries;

public record GetPerfilAlumnoQuery(Guid IdAlumno) : IRequest<Result<AlumnoPerfilDto>>;

public class GetPerfilAlumnoQueryHandler : IRequestHandler<GetPerfilAlumnoQuery, Result<AlumnoPerfilDto>>
{
    private readonly IAppDbContext _db;

    public GetPerfilAlumnoQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<AlumnoPerfilDto>> Handle(GetPerfilAlumnoQuery request, CancellationToken cancellationToken)
    {
        var alumno = await _db.Alumnos
            .Include(a => a.Usuario)
                .ThenInclude(u => u.TipoDocumento)
            .FirstOrDefaultAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (alumno == null)
            return Result<AlumnoPerfilDto>.Failure("Alumno no encontrado");

        var perfil = new AlumnoPerfilDto
        {
            IdAlumno = alumno.IdAlumno,
            NombreCompleto = alumno.Usuario.NombreUsuario,
            Correo = alumno.Usuario.Correo,
            Telefono = alumno.Usuario.Telefono,
            TipoDocumento = alumno.Usuario.TipoDocumento.Nombre,
            NumeroDocumento = alumno.Usuario.NumeroDocumento,
            FechaInscripcion = alumno.FechaInscripcion,
            AvatarUrl = alumno.AvatarUrl,
            ContactoEmergencia = string.IsNullOrEmpty(alumno.ContactoEmergenciaNombre) ? null : new ContactoEmergenciaDto
            {
                NombreCompleto = alumno.ContactoEmergenciaNombre,
                Telefono = alumno.ContactoEmergenciaTelefono ?? string.Empty,
                Relacion = alumno.ContactoEmergenciaRelacion ?? string.Empty
            },
            Configuracion = new ConfiguracionAlumnoDto
            {
                NotificacionesEmail = alumno.NotificacionesEmail,
                RecordatoriosClase = alumno.RecordatoriosClase,
                AlertasPaquete = alumno.AlertasPaquete
            }
        };

        return Result<AlumnoPerfilDto>.Success(perfil);
    }
}

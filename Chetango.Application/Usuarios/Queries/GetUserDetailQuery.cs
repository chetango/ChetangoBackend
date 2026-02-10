// ============================================
// GET USER DETAIL QUERY
// ============================================

using Chetango.Application.Common;
using Chetango.Application.Usuarios.DTOs;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Chetango.Application.Usuarios.Queries;

public record GetUserDetailQuery(Guid UsuarioId) : IRequest<Result<UsuarioDetalleDTO>>;

public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, Result<UsuarioDetalleDTO>>
{
    private readonly IAppDbContext _db;

    public GetUserDetailQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<UsuarioDetalleDTO>> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Usuario>()
            .Include(u => u.TipoDocumento)
            .Include(u => u.Estado)
            .Include(u => u.Profesores).ThenInclude(p => p.TipoProfesor)
            .Include(u => u.Alumnos).ThenInclude(a => a.Estado)
            .FirstOrDefaultAsync(u => u.IdUsuario == request.UsuarioId, cancellationToken);

        if (usuario == null)
            return Result<UsuarioDetalleDTO>.Failure("Usuario no encontrado");

        var dto = new UsuarioDetalleDTO
        {
            UsuarioId = usuario.IdUsuario,
            NombreUsuario = usuario.NombreUsuario,
            Correo = usuario.Correo,
            Telefono = usuario.Telefono,
            TipoDocumento = usuario.TipoDocumento.Nombre,
            NumeroDocumento = usuario.NumeroDocumento,
            Rol = DeterminarRol(usuario),
            Estado = usuario.Estado.Nombre,
            FechaCreacion = usuario.FechaCreacion
        };

        // Datos de profesor
        var profesor = usuario.Profesores.FirstOrDefault();
        if (profesor != null)
        {
            List<string> especialidades = new();
            try
            {
                if (!string.IsNullOrEmpty(profesor.Especialidades))
                {
                    especialidades = JsonSerializer.Deserialize<List<string>>(profesor.Especialidades) ?? new();
                }
            }
            catch { }

            dto.DatosProfesor = new DatosProfesorDTO
            {
                IdProfesor = profesor.IdProfesor,
                TipoProfesor = profesor.TipoProfesor.Nombre,
                FechaIngreso = DateTimeHelper.Now, // TODO: agregar campo FechaIngreso a la tabla Profesor
                Biografia = profesor.Biografia,
                Especialidades = especialidades,
                TarifaActual = profesor.TarifaActual
            };
        }

        // Datos de alumno
        var alumno = usuario.Alumnos.FirstOrDefault();
        if (alumno != null)
        {
            dto.DatosAlumno = new DatosAlumnoDTO
            {
                IdAlumno = alumno.IdAlumno,
                FechaInscripcion = alumno.FechaInscripcion,
                ContactoEmergencia = alumno.ContactoEmergenciaNombre,
                TelefonoEmergencia = alumno.ContactoEmergenciaTelefono,
                ObservacionesMedicas = null // TODO: agregar si se crea tabla de observaciones médicas
            };
        }

        return Result<UsuarioDetalleDTO>.Success(dto);
    }

    private string DeterminarRol(Usuario usuario)
    {
        if (usuario.Profesores.Any()) return "profesor";
        if (usuario.Alumnos.Any()) return "alumno";
        return "admin";
    }
}

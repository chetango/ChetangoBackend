// ============================================
// GET USERS QUERY (con filtros y paginación)
// ============================================

using Chetango.Application.Common;
using Chetango.Application.Usuarios.DTOs;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Usuarios.Queries;

public record GetUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? Rol = null,
    string? Estado = null
) : IRequest<Result<UsuariosPaginadosDTO>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<UsuariosPaginadosDTO>>
{
    private readonly IAppDbContext _db;

    public GetUsersQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<UsuariosPaginadosDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Set<Usuario>()
            .Include(u => u.TipoDocumento)
            .Include(u => u.Estado)
            .Include(u => u.Profesores)
            .Include(u => u.Alumnos)
            .AsQueryable();

        // Filtrar por término de búsqueda
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(u =>
                u.NombreUsuario.Contains(request.SearchTerm) ||
                u.Correo.Contains(request.SearchTerm) ||
                u.NumeroDocumento.Contains(request.SearchTerm)
            );
        }

        // Filtrar por estado
        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            query = query.Where(u => u.Estado.Nombre.ToLower() == request.Estado.ToLower());
        }

        // Filtrar por rol
        if (!string.IsNullOrWhiteSpace(request.Rol))
        {
            switch (request.Rol.ToLower())
            {
                case "profesor":
                    query = query.Where(u => u.Profesores.Any());
                    break;
                case "alumno":
                    query = query.Where(u => u.Alumnos.Any());
                    break;
                case "admin":
                    query = query.Where(u => !u.Profesores.Any() && !u.Alumnos.Any());
                    break;
            }
        }

        // Contar total
        var totalItems = await query.CountAsync(cancellationToken);

        // Paginar
        var usuarios = await query
            .OrderByDescending(u => u.FechaCreacion)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Mapear a DTOs
        var usuariosDTO = usuarios.Select(u => new UsuarioDTO
        {
            UsuarioId = u.IdUsuario,
            NombreUsuario = u.NombreUsuario,
            Correo = u.Correo,
            Telefono = u.Telefono,
            TipoDocumento = u.TipoDocumento.Nombre,
            NumeroDocumento = u.NumeroDocumento,
            Rol = DeterminarRol(u),
            Estado = u.Estado.Nombre,
            FechaCreacion = u.FechaCreacion
        }).ToList();

        var resultado = new UsuariosPaginadosDTO
        {
            Usuarios = usuariosDTO,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize)
        };

        return Result<UsuariosPaginadosDTO>.Success(resultado);
    }

    private string DeterminarRol(Usuario usuario)
    {
        if (usuario.Profesores.Any()) return "profesor";
        if (usuario.Alumnos.Any()) return "alumno";
        return "admin";
    }
}

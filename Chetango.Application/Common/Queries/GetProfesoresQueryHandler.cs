using Chetango.Application.Clases.DTOs;
using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Common.Queries;

public class GetProfesoresQueryHandler : IRequestHandler<GetProfesoresQuery, Result<List<ProfesorDTO>>>
{
    private readonly IAppDbContext _context;

    public GetProfesoresQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ProfesorDTO>>> Handle(GetProfesoresQuery request, CancellationToken cancellationToken)
    {
        var profesores = await _context.Profesores
            .Include(p => p.Usuario)
            .Select(p => new ProfesorDTO(
                p.IdProfesor,
                p.IdUsuario,
                p.Usuario.NombreUsuario,
                p.Usuario.Correo
            ))
            .ToListAsync(cancellationToken);

        return Result<List<ProfesorDTO>>.Success(profesores);
    }
}

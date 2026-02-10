using Chetango.Application.Common;
using Chetango.Application.Referidos.DTOs;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Referidos.Queries;

public class GetMiCodigoReferidoHandler : IRequestHandler<GetMiCodigoReferidoQuery, Result<CodigoReferidoDTO?>>
{
    private readonly IAppDbContext _db;

    public GetMiCodigoReferidoHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<CodigoReferidoDTO?>> Handle(GetMiCodigoReferidoQuery request, CancellationToken cancellationToken)
    {
        // Buscar alumno por email
        var alumno = await _db.Alumnos
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Usuario.Correo == request.EmailAlumno, cancellationToken);

        if (alumno == null)
            return Result<CodigoReferidoDTO?>.Failure("No se encontró el alumno autenticado.");

        // Buscar código activo
        var codigo = await _db.Set<CodigoReferido>()
            .FirstOrDefaultAsync(c => c.IdAlumno == alumno.IdAlumno && c.Activo, cancellationToken);

        if (codigo == null)
            return Result<CodigoReferidoDTO?>.Success(null);

        var dto = new CodigoReferidoDTO(
            codigo.IdCodigo,
            codigo.Codigo,
            codigo.Activo,
            codigo.VecesUsado,
            codigo.BeneficioReferidor,
            codigo.BeneficioNuevoAlumno,
            codigo.FechaCreacion
        );

        return Result<CodigoReferidoDTO?>.Success(dto);
    }
}

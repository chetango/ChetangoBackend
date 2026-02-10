// ============================================
// GET SEGURIDAD INFO QUERY
// ============================================

using Chetango.Application.Admin.DTOs;
using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Admin.Queries;

public record GetSeguridadInfoQuery(string EmailUsuario) : IRequest<Result<SeguridadInfoDTO>>;

public class GetSeguridadInfoQueryHandler : IRequestHandler<GetSeguridadInfoQuery, Result<SeguridadInfoDTO>>
{
    private readonly IAppDbContext _db;

    public GetSeguridadInfoQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<SeguridadInfoDTO>> Handle(GetSeguridadInfoQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _db.Set<Chetango.Domain.Entities.Usuario>()
            .FirstOrDefaultAsync(u => u.Correo == request.EmailUsuario, cancellationToken);

        if (usuario == null)
            return Result<SeguridadInfoDTO>.Failure("Usuario no encontrado");

        // Datos mock - en producción deberían venir de tablas de sesiones/auditoría
        var dto = new SeguridadInfoDTO(
            SesionesActivas: 1, // TODO: Implementar tracking de sesiones
            UltimoCambioPassword: null, // TODO: Agregar campo en BD
            HistorialAccesos: new List<HistorialAccesoDTO>
            {
                new HistorialAccesoDTO(
                    Fecha: DateTime.Now.AddHours(-2),
                    Dispositivo: "Windows 10 - PC",
                    Navegador: "Chrome 120",
                    Ip: "192.168.1.1"
                ),
                new HistorialAccesoDTO(
                    Fecha: DateTime.Now.AddDays(-1),
                    Dispositivo: "Windows 10 - PC",
                    Navegador: "Chrome 120",
                    Ip: "192.168.1.1"
                )
            }
        );

        return Result<SeguridadInfoDTO>.Success(dto);
    }
}

// ============================================
// UPDATE DATOS ACADEMIA COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Admin.Commands;

public record UpdateDatosAcademiaCommand(
    string Nombre,
    string Direccion,
    string Telefono,
    string Email,
    string? Instagram,
    string? Facebook,
    string? WhatsApp
) : IRequest<Result<Unit>>;

public class UpdateDatosAcademiaCommandHandler : IRequestHandler<UpdateDatosAcademiaCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdateDatosAcademiaCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(UpdateDatosAcademiaCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implementar tabla de ConfiguracionGlobal para almacenar estos datos
        // Por ahora solo retornamos éxito sin guardar nada
        // En producción, deberías crear una tabla ConfiguracionAcademia con estos campos

        await Task.CompletedTask; // Placeholder para async

        return Result<Unit>.Success(Unit.Value);
    }
}

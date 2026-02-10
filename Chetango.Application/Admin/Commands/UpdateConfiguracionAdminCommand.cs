// ============================================
// UPDATE CONFIGURACION ADMIN COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;

namespace Chetango.Application.Admin.Commands;

public record UpdateConfiguracionAdminCommand(
    Guid IdUsuario,
    bool NotificacionesEmail,
    bool AlertasPagosPendientes,
    bool ReportesAutomaticos,
    bool AlertasPaquetesVencer,
    bool AlertasAsistenciaBaja,
    bool NotificacionesNuevosRegistros
) : IRequest<Result<Unit>>;

public class UpdateConfiguracionAdminCommandHandler : IRequestHandler<UpdateConfiguracionAdminCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdateConfiguracionAdminCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(UpdateConfiguracionAdminCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implementar tabla ConfiguracionAdmin para almacenar estas preferencias
        // Por ahora solo retornamos éxito sin guardar nada
        // En producción, deberías crear una tabla ConfiguracionAdmin con estos campos
        // Similar a como Profesor tiene campos de configuración

        await Task.CompletedTask; // Placeholder para async

        return Result<Unit>.Success(Unit.Value);
    }
}

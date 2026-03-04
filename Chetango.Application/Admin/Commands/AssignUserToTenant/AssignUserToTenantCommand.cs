using MediatR;

namespace Chetango.Application.Admin.Commands.AssignUserToTenant;

/// <summary>
/// Comando para asignar un usuario a un tenant.
/// Permite que un usuario trabaje en múltiples academias.
/// Solo ejecutable por Super Administrador.
/// </summary>
public record AssignUserToTenantCommand : IRequest<Guid>
{
    public Guid TenantId { get; init; }
    public Guid IdUsuario { get; init; }
}

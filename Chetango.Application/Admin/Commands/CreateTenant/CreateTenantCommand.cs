using MediatR;

namespace Chetango.Application.Admin.Commands.CreateTenant;

/// <summary>
/// Comando para crear un nuevo tenant (academia).
/// Solo ejecutable por Super Administrador.
/// </summary>
public record CreateTenantCommand : IRequest<Guid>
{
    public string Nombre { get; init; } = null!;
    public string Subdomain { get; init; } = null!;
    public string Dominio { get; init; } = null!;
    public string Plan { get; init; } = "Basic";
    public int MaxSedes { get; init; } = 1;
    public int MaxAlumnos { get; init; } = 50;
    public int MaxProfesores { get; init; } = 10;
    public int MaxStorageMB { get; init; } = 1000;
    public string EmailContacto { get; init; } = null!;
}

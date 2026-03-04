using MediatR;

namespace Chetango.Application.Admin.Commands.CreateTenantWithAdmin;

/// <summary>
/// Comando para crear un tenant (academia) con su usuario administrador en un solo paso.
/// 
/// Flujo:
/// 1. Super Admin ya creó el usuario en Azure AD con su correo
/// 2. Super Admin ejecuta este comando desde la vista de administración
/// 3. Sistema crea Tenant + Usuario en DB + asignación en un solo paso
/// 
/// Solo ejecutable por Super Administrador.
/// </summary>
public record CreateTenantWithAdminCommand : IRequest<CreateTenantWithAdminResponse>
{
    // Datos del Tenant
    public string NombreTenant { get; init; } = null!;
    public string Subdomain { get; init; } = null!;
    public string DominioCompleto { get; init; } = null!;
    public string Plan { get; init; } = "Basic";
    public int MaxSedes { get; init; } = 1;
    public int MaxAlumnos { get; init; } = 50;
    public int MaxProfesores { get; init; } = 10;
    public int MaxStorageMB { get; init; } = 1000;
    
    // Datos del Usuario Administrador
    public string NombreUsuario { get; init; } = null!;
    public string CorreoAdmin { get; init; } = null!;
    public Guid IdTipoDocumento { get; init; }
    public string NumeroDocumento { get; init; } = null!;
    public string Telefono { get; init; } = null!;
}

public record CreateTenantWithAdminResponse
{
    public Guid TenantId { get; init; }
    public Guid IdUsuario { get; init; }
    public string Mensaje { get; init; } = null!;
}

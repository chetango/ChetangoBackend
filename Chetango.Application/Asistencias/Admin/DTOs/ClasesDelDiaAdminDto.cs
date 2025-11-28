namespace Chetango.Application.Asistencias.Admin.DTOs;

public sealed class ClaseDelDiaAdminItemDto
{
    public Guid IdClase { get; init; }
    public string Nombre { get; init; } = null!;
    public TimeOnly HoraInicio { get; init; }
    public TimeOnly HoraFin { get; init; }
    public string ProfesorPrincipal { get; init; } = null!;
}

public sealed class ClasesDelDiaAdminDto
{
    public DateOnly Fecha { get; init; }
    public IReadOnlyList<ClaseDelDiaAdminItemDto> Clases { get; init; } = Array.Empty<ClaseDelDiaAdminItemDto>();
}

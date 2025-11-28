namespace Chetango.Application.Asistencias.Admin.DTOs;

public sealed class DiasConClasesAdminDto
{
    public DateOnly Hoy { get; init; }
    public DateOnly Desde { get; init; }
    public DateOnly Hasta { get; init; }

    public IReadOnlyList<DateOnly> DiasConClases { get; init; } = Array.Empty<DateOnly>();
}

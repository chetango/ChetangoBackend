namespace Chetango.Application.Asistencias.Admin.DTOs;

public enum EstadoPaqueteAdmin
{
    Activo,
    Agotado,
    Congelado,
    SinPaquete
}

public enum EstadoAsistenciaAdmin
{
    Ausente,
    Presente
}

public sealed class PaqueteAlumnoAdminDto
{
    public EstadoPaqueteAdmin Estado { get; init; }
    public string Descripcion { get; init; } = null!;
    public int? ClasesTotales { get; init; }
    public int? ClasesUsadas { get; init; }
    public int? ClasesRestantes { get; init; }
}

public sealed class AsistenciaAlumnoAdminDto
{
    public EstadoAsistenciaAdmin Estado { get; init; }
    public string? Observacion { get; init; }
}

public sealed class AlumnoEnClaseAdminDto
{
    public Guid IdAlumno { get; init; }
    public string NombreCompleto { get; init; } = null!;
    public string DocumentoIdentidad { get; init; } = null!;
    public string AvatarIniciales { get; init; } = null!;
    public PaqueteAlumnoAdminDto Paquete { get; init; } = null!;
    public AsistenciaAlumnoAdminDto Asistencia { get; init; } = null!;
}

public sealed class ResumenAsistenciasClaseAdminDto
{
    public Guid IdClase { get; init; }
    public DateOnly Fecha { get; init; }
    public string NombreClase { get; init; } = null!;
    public string ProfesorPrincipal { get; init; } = null!;

    public IReadOnlyList<AlumnoEnClaseAdminDto> Alumnos { get; init; } = Array.Empty<AlumnoEnClaseAdminDto>();

    public int Presentes { get; init; }
    public int Ausentes { get; init; }
    public int SinPaquete { get; init; }
}

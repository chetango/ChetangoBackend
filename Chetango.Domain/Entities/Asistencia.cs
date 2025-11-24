namespace Chetango.Domain.Entities.Estados
{
    // Registro de participación de un Alumno en una Clase usando un Paquete específico.
    // Índice único (IdClase, IdAlumno) evita múltiples asistencias duplicadas.
    public class Asistencia
    {
        public Guid IdAsistencia { get; set; }

        public Guid IdClase { get; set; }
        public Clase Clase { get; set; } = null!;

        public Guid IdAlumno { get; set; }
        public Alumno Alumno { get; set; } = null!;

        public Guid IdPaqueteUsado { get; set; }
        public Paquete PaqueteUsado { get; set; } = null!; // Debe corresponder a paquete activo del alumno

        public int IdEstado { get; set; } // Presente / Ausente / Justificada
        public EstadoAsistencia Estado { get; set; } = null!;

        public string? Observacion { get; set; }

        // Campos de auditoría
        public DateTime FechaRegistro { get; set; }  // Equivalente a FechaCreacion
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioCreacion { get; set; } = null!;
        public string? UsuarioModificacion { get; set; }
    }
}

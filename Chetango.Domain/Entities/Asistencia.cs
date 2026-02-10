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

public Guid? IdPaqueteUsado { get; set; } // Nullable: si es null = clase sin paquete
    public Paquete? PaqueteUsado { get; set; } // Nullable: null cuando no se usa paquete

    public int IdTipoAsistencia { get; set; } // Tipo: Normal, Cortesía, Prueba, Recuperación, etc.
    public TipoAsistencia TipoAsistencia { get; set; } = null!;

        public int IdEstado { get; set; } // Presente / Ausente / Justificada
        public EstadoAsistencia Estado { get; set; } = null!;

        public string? Observacion { get; set; }

        // Campo para confirmación del alumno
        public bool Confirmado { get; set; } = false; // Indica si el alumno confirmó su asistencia
        public DateTime? FechaConfirmacion { get; set; } // Fecha cuando el alumno confirmó

        // Campos de auditoría
        public DateTime FechaRegistro { get; set; }  // Equivalente a FechaCreacion
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioCreacion { get; set; } = null!;
        public string? UsuarioModificacion { get; set; }
    }
}

namespace Chetango.Application.Suscripciones.DTOs
{
    /// <summary>
    /// DTO con información del estado actual de la suscripción de una academia.
    /// </summary>
    public class EstadoSuscripcionDto
    {
        public Guid TenantId { get; set; }
        public string NombreAcademia { get; set; } = null!;
        public string Plan { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaVencimientoPlan { get; set; }
        public int? DiasRestantes { get; set; }
        
        // Límites del plan
        public int MaxSedes { get; set; }
        public int MaxAlumnos { get; set; }
        public int MaxProfesores { get; set; }
        public int MaxStorageMB { get; set; }
        
        // Uso actual
        public int SedesActuales { get; set; }
        public int AlumnosActivos { get; set; }
        public int ProfesoresActivos { get; set; }
        public int StorageMBUsado { get; set; }
        
        // Progreso en porcentaje
        public ProgresoCuotasDto ProgresoCuotas { get; set; } = new();
    }

    public class ProgresoCuotasDto
    {
        public int Sedes { get; set; }
        public int Alumnos { get; set; }
        public int Profesores { get; set; }
        public int Storage { get; set; }
    }
}

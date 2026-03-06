namespace Chetango.Domain.Entities
{
    /// <summary>
    /// Representa una academia suscrita al SaaS Aphellion.
    /// Cada tenant es un cliente independiente con datos aislados.
    /// </summary>
    public class Tenant
    {
        public Guid Id { get; set; }
        
        // Información básica
        public string Nombre { get; set; } = null!;
        public string Subdomain { get; set; } = null!;
        public string Dominio { get; set; } = null!; // Dominio completo del email: corporacionchetango.aphelion.com
        
        // Plan y estado
        public string Plan { get; set; } = null!; // Basico, Profesional, Enterprise
        public string Estado { get; set; } = null!; // Activo, Suspendido, Cancelado, PendientePago
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaVencimientoPlan { get; set; }
        
        // Límites según plan
        public int MaxSedes { get; set; }
        public int MaxAlumnos { get; set; }
        public int MaxProfesores { get; set; }
        public int MaxStorageMB { get; set; }
        
        // Información de contacto
        public string EmailContacto { get; set; } = null!;
        public string? TelefonoContacto { get; set; }
        
        // Personalización (branding)
        public string? LogoUrl { get; set; }
        public string? ColorPrimario { get; set; }
        public string? ColorSecundario { get; set; }
        public string? ColorAccent { get; set; }
        public string? NombreComercial { get; set; }
        public string? FaviconUrl { get; set; }
        
        // Integración con pasarela de pagos (para Fase 3)
        public string? WompiSubscriptionId { get; set; }
        public string? StripeCustomerId { get; set; }
        public string? MetodoPagoPreferido { get; set; } // Transferencia, Wompi, Stripe
        
        // Cumplimiento y onboarding legal
        /// <summary>
        /// true cuando el admin aceptó todos los documentos obligatorios.
        /// Mientras sea false, la academia está en modo lectura (onboarding pendiente).
        /// </summary>
        public bool OnboardingCompletado { get; set; } = false;

        /// <summary>Fecha en que se completó el onboarding legal por primera vez</summary>
        public DateTime? FechaActivacion { get; set; }

        /// <summary>
        /// true cuando existe una versión nueva de un documento crítico que el admin aún no aceptó.
        /// El frontend muestra un banner de reaceptación obligatoria.
        /// </summary>
        public bool RequiereReaceptacion { get; set; } = false;

        // Auditoría
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string? CreadoPor { get; set; }
        public string? ActualizadoPor { get; set; }
        
        // Relaciones
        public ICollection<PagoSuscripcion> PagosSuscripcion { get; set; } = new List<PagoSuscripcion>();
        public ICollection<AceptacionDocumento> AceptacionesDocumentos { get; set; } = new List<AceptacionDocumento>();
    }
}

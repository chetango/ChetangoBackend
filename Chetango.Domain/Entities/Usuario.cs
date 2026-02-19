using Chetango.Domain.Entities.Estados;
using Chetango.Domain.Enums;

namespace Chetango.Domain.Entities
{
    public class Usuario
    {
        public Guid IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        // PasswordHash eliminado: autenticación delegada a Azure Entra ID
        public Guid IdTipoDocumento { get; set; } // FK explícita
        public TipoDocumento TipoDocumento { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public int IdEstadoUsuario { get; set; } // FK explícita
        public EstadoUsuario Estado { get; set; } = null!;
        public Sede Sede { get; set; } = Sede.Medellin;
        public DateTime FechaCreacion { get; set; }

        // Relaciones
        public ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
        public ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();
        public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
    }
}

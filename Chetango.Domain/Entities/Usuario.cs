namespace Chetango.Domain.Entities
{
    public class Usuario
    {
        public Guid IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
        public TipoDocumento TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public EstadoUsuario Estado { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Relaciones
        public ICollection<UsuarioRol> Roles { get; set; } = new List<UsuarioRol>();
        public ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
        public ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();
        public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
    }
}

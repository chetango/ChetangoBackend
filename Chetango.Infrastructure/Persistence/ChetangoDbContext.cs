using Microsoft.EntityFrameworkCore;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Infrastructure.Persistence
{
    public class ChetangoDbContext : DbContext
    {
        public ChetangoDbContext(DbContextOptions<ChetangoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alumno> Alumnos => Set<Alumno>();
        public DbSet<Asistencia> Asistencias => Set<Asistencia>();
        public DbSet<Auditoria> Auditorias => Set<Auditoria>();
        public DbSet<Clase> Clases => Set<Clase>();
        public DbSet<ConfiguracionNotificaciones> ConfiguracionesNotificaciones => Set<ConfiguracionNotificaciones>();
        public DbSet<CongelacionPaquete> CongelacionesPaquete => Set<CongelacionPaquete>();
        public DbSet<MonitorClase> MonitoresClase => Set<MonitorClase>();
        public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<Paquete> Paquetes => Set<Paquete>();
        public DbSet<Profesor> Profesores => Set<Profesor>();
        public DbSet<TarifaProfesor> TarifasProfesor => Set<TarifaProfesor>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<UsuarioRol> UsuariosRoles => Set<UsuarioRol>();

        public DbSet<EstadoAsistencia> EstadosAsistencia => Set<EstadoAsistencia>();
        public DbSet<EstadoUsuario> EstadosUsuario => Set<EstadoUsuario>();
        public DbSet<EstadoPaquete> EstadosPaquete => Set<EstadoPaquete>();
        public DbSet<EstadoNotificacion> EstadosNotificacion => Set<EstadoNotificacion>();
        public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
        public DbSet<TipoClase> TiposClase => Set<TipoClase>();
        public DbSet<TipoPaquete> TiposPaquete => Set<TipoPaquete>();
        public DbSet<TipoProfesor> TiposProfesor => Set<TipoProfesor>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<RolEnClase> RolesEnClase => Set<RolEnClase>();
        public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aplica automáticamente todas las configuraciones IEntityTypeConfiguration de este ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChetangoDbContext).Assembly);
        }
    }
}

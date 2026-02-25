using Microsoft.EntityFrameworkCore;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using Chetango.Application.Common; // añadido

namespace Chetango.Infrastructure.Persistence
{
    // DbContext principal: orquesta acceso a datos y aplica las configuraciones Fluent de cada entidad
    // Expone DbSet solo para agregados / catálogos necesarios en consultas y reglas de dominio
    public class ChetangoDbContext : DbContext, IAppDbContext // implementa interfaz
    {
        public ChetangoDbContext(DbContextOptions<ChetangoDbContext> options)
            : base(options)
        {
        }

        // Catálogos y entidades de negocio
        public DbSet<Alumno> Alumnos => Set<Alumno>();
        public DbSet<Asistencia> Asistencias => Set<Asistencia>();
        public DbSet<Auditoria> Auditorias => Set<Auditoria>();
        public DbSet<Clase> Clases => Set<Clase>();
        public DbSet<ClaseProfesor> ClasesProfesores => Set<ClaseProfesor>();
        public DbSet<ConfiguracionNotificaciones> ConfiguracionesNotificaciones => Set<ConfiguracionNotificaciones>();
        public DbSet<CongelacionPaquete> CongelacionesPaquete => Set<CongelacionPaquete>();
        public DbSet<Evento> Eventos => Set<Evento>();
        public DbSet<LiquidacionMensual> LiquidacionesMensuales => Set<LiquidacionMensual>();
        public DbSet<MonitorClase> MonitoresClase => Set<MonitorClase>();
        public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<Paquete> Paquetes => Set<Paquete>();
        public DbSet<Profesor> Profesores => Set<Profesor>();
        public DbSet<TarifaProfesor> TarifasProfesor => Set<TarifaProfesor>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<SolicitudRenovacionPaquete> SolicitudesRenovacionPaquete => Set<SolicitudRenovacionPaquete>();
        public DbSet<SolicitudClasePrivada> SolicitudesClasePrivada => Set<SolicitudClasePrivada>();
        public DbSet<CodigoReferido> CodigosReferido => Set<CodigoReferido>();
        public DbSet<UsoCodigoReferido> UsosCodigoReferido => Set<UsoCodigoReferido>();
        public DbSet<OtroIngreso> OtrosIngresos => Set<OtroIngreso>();
        public DbSet<OtroGasto> OtrosGastos => Set<OtroGasto>();

        // Estados y tipos
        public DbSet<EstadoAlumno> EstadosAlumno => Set<EstadoAlumno>();
        public DbSet<EstadoAsistencia> EstadosAsistencia => Set<EstadoAsistencia>();
        public DbSet<EstadoUsuario> EstadosUsuario => Set<EstadoUsuario>();
        public DbSet<EstadoPaquete> EstadosPaquete => Set<EstadoPaquete>();
        public DbSet<EstadoPago> EstadosPago => Set<EstadoPago>();
        public DbSet<EstadoNotificacion> EstadosNotificacion => Set<EstadoNotificacion>();
        public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
        public DbSet<TipoClase> TiposClase => Set<TipoClase>();
        public DbSet<TipoPaquete> TiposPaquete => Set<TipoPaquete>();
        public DbSet<TipoProfesor> TiposProfesor => Set<TipoProfesor>();
        public DbSet<TipoAsistencia> TiposAsistencia => Set<TipoAsistencia>();
        public DbSet<RolEnClase> RolesEnClase => Set<RolEnClase>();
        public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
        public DbSet<CategoriaIngreso> CategoriasIngreso => Set<CategoriaIngreso>();
        public DbSet<CategoriaGasto> CategoriasGasto => Set<CategoriaGasto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aplica automáticamente todas las clases IEntityTypeConfiguration en el ensamblado (mapeos centralizados)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChetangoDbContext).Assembly);
            // Punto de extensión futuro: interceptores de auditoría / soft delete / filtros globales
        }
    }
}

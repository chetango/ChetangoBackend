using Microsoft.EntityFrameworkCore;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using Chetango.Application.Common; // añadido
using Chetango.Application.Common.Interfaces;

namespace Chetango.Infrastructure.Persistence
{
    // DbContext principal: orquesta acceso a datos y aplica las configuraciones Fluent de cada entidad
    // Expone DbSet solo para agregados / catálogos necesarios en consultas y reglas de dominio
    public class ChetangoDbContext : DbContext, IAppDbContext // implementa interfaz
    {
        private readonly ITenantProvider? _tenantProvider;

        public ChetangoDbContext(DbContextOptions<ChetangoDbContext> options, ITenantProvider? tenantProvider = null)
            : base(options)
        {
            _tenantProvider = tenantProvider;
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

        // Multi-tenancy y Suscripciones
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
        public DbSet<PagoSuscripcion> PagosSuscripcion => Set<PagoSuscripcion>();
        public DbSet<ConfiguracionPago> ConfiguracionPagos => Set<ConfiguracionPago>();

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
            
            // Query Filters globales para Multi-Tenancy
            // Se aplican automáticamente a TODAS las queries, no hace falta .Where manual
            ConfigureQueryFilters(modelBuilder);
        }

        private void ConfigureQueryFilters(ModelBuilder modelBuilder)
        {
            // IMPORTANTE: Las lambdas capturan la REFERENCIA a _tenantProvider (no el valor de tenantId).
            // EF Core evalúa estas lambdas en CADA query, así que GetCurrentTenantId() se llama
            // por request, no una sola vez al iniciar el servidor.
            // Si tenantId es null → sin filtro (super admin o sin sesión).
            // Si tenantId tiene valor → filtra estrictamente por ese tenant.

            // Filtro para Clases
            modelBuilder.Entity<Clase>().HasQueryFilter(c =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                c.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para Pagos
            modelBuilder.Entity<Pago>().HasQueryFilter(p =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                p.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para Paquetes
            modelBuilder.Entity<Paquete>().HasQueryFilter(p =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                p.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para Asistencias
            modelBuilder.Entity<Asistencia>().HasQueryFilter(a =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                a.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para Eventos
            modelBuilder.Entity<Evento>().HasQueryFilter(e =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                e.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para Solicitudes de Clase Privada
            modelBuilder.Entity<SolicitudClasePrivada>().HasQueryFilter(s =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                s.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para Solicitudes de Renovación
            modelBuilder.Entity<SolicitudRenovacionPaquete>().HasQueryFilter(s =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                s.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para Usuarios - Solo usuarios del tenant actual
            modelBuilder.Entity<Usuario>().HasQueryFilter(u =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                u.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para Alumnos - Solo alumnos del tenant actual
            modelBuilder.Entity<Alumno>().HasQueryFilter(a =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                a.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para Profesores - Solo profesores del tenant actual
            modelBuilder.Entity<Profesor>().HasQueryFilter(p =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                p.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para OtrosIngresos - Solo ingresos del tenant actual
            modelBuilder.Entity<OtroIngreso>().HasQueryFilter(o =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                o.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para OtrosGastos - Solo gastos del tenant actual
            modelBuilder.Entity<OtroGasto>().HasQueryFilter(o =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                o.TenantId == _tenantProvider.GetCurrentTenantId());

            // Filtro para LiquidacionesMensuales - Solo liquidaciones del tenant actual
            modelBuilder.Entity<LiquidacionMensual>().HasQueryFilter(l =>
                _tenantProvider == null ||
                !_tenantProvider.GetCurrentTenantId().HasValue ||
                l.TenantId == _tenantProvider.GetCurrentTenantId());
        }
    }
}

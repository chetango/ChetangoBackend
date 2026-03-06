using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Application.Common
{
    // Abstracción mínima para desacoplar Application de Infrastructure
    public interface IAppDbContext
    {
        DbSet<Alumno> Alumnos { get; }
        DbSet<Asistencia> Asistencias { get; }
        DbSet<Clase> Clases { get; }
        DbSet<ClaseProfesor> ClasesProfesores { get; }
        DbSet<LiquidacionMensual> LiquidacionesMensuales { get; }
        DbSet<Evento> Eventos { get; }
        DbSet<Pago> Pagos { get; }
        DbSet<Paquete> Paquetes { get; }
        DbSet<Profesor> Profesores { get; }
        DbSet<Usuario> Usuarios { get; }
        DbSet<RolEnClase> RolesEnClase { get; }
        DbSet<SolicitudRenovacionPaquete> SolicitudesRenovacionPaquete { get; }
        DbSet<SolicitudClasePrivada> SolicitudesClasePrivada { get; }
        DbSet<CodigoReferido> CodigosReferido { get; }
        DbSet<UsoCodigoReferido> UsosCodigoReferido { get; }
        DbSet<OtroIngreso> OtrosIngresos { get; }
        DbSet<OtroGasto> OtrosGastos { get; }
        
        // Multi-tenancy y Suscripciones
        DbSet<Tenant> Tenants { get; }
        DbSet<TenantUser> TenantUsers { get; }
        DbSet<PagoSuscripcion> PagosSuscripcion { get; }
        DbSet<ConfiguracionPago> ConfiguracionPagos { get; }

        // Configuración de sedes por tenant
        DbSet<SedeConfig> SedeConfigs { get; }
        
        // Método genérico para acceder a otros DbSets sin exponerlos explícitamente
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

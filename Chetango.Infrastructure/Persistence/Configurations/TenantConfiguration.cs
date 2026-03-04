using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Tenant.
/// Define la estructura de la tabla Tenants en base de datos.
/// </summary>
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(t => t.Id);
        
        // Información básica
        builder.Property(t => t.Nombre)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(t => t.Subdomain)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(t => t.Subdomain).IsUnique(); // Subdomain debe ser único
        
        // Plan y estado
        builder.Property(t => t.Plan)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(t => t.Estado)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Activo");
            
        builder.Property(t => t.FechaRegistro)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
        
        // Límites del plan
        builder.Property(t => t.MaxSedes)
            .IsRequired();
            
        builder.Property(t => t.MaxAlumnos)
            .IsRequired();
            
        builder.Property(t => t.MaxProfesores)
            .IsRequired();
            
        builder.Property(t => t.MaxStorageMB)
            .IsRequired();
        
        // Información de contacto
        builder.Property(t => t.EmailContacto)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(t => t.EmailContacto).IsUnique(); // Email único por academia
            
        builder.Property(t => t.TelefonoContacto)
            .HasMaxLength(20);
        
        // Personalización (branding)
        builder.Property(t => t.LogoUrl)
            .HasMaxLength(500);
            
        builder.Property(t => t.ColorPrimario)
            .HasMaxLength(7); // #RRGGBB
            
        builder.Property(t => t.ColorSecundario)
            .HasMaxLength(7);
            
        builder.Property(t => t.ColorAccent)
            .HasMaxLength(7);
            
        builder.Property(t => t.NombreComercial)
            .HasMaxLength(200);
            
        builder.Property(t => t.FaviconUrl)
            .HasMaxLength(500);
        
        // Integración con pasarelas de pago
        builder.Property(t => t.WompiSubscriptionId)
            .HasMaxLength(100);
            
        builder.Property(t => t.StripeCustomerId)
            .HasMaxLength(100);
            
        builder.Property(t => t.MetodoPagoPreferido)
            .HasMaxLength(50);
        
        // Auditoría
        builder.Property(t => t.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        builder.Property(t => t.CreadoPor)
            .HasMaxLength(256);
            
        builder.Property(t => t.ActualizadoPor)
            .HasMaxLength(256);
    }
}

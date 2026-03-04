using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad ConfiguracionPago.
/// Define la estructura de la tabla ConfiguracionPagos en base de datos.
/// </summary>
public class ConfiguracionPagoConfiguration : IEntityTypeConfiguration<ConfiguracionPago>
{
    public void Configure(EntityTypeBuilder<ConfiguracionPago> builder)
    {
        builder.ToTable("ConfiguracionPagos");
        builder.HasKey(c => c.Id);
        
        // Datos bancarios
        builder.Property(c => c.Banco)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(c => c.TipoCuenta)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(c => c.NumeroCuenta)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(c => c.Titular)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(c => c.NIT)
            .HasMaxLength(50);
        
        // Configuración
        builder.Property(c => c.Activo)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(c => c.MostrarEnPortal)
            .IsRequired()
            .HasDefaultValue(true);
        
        // Auditoría
        builder.Property(c => c.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        builder.Property(c => c.CreadoPor)
            .HasMaxLength(256)
            .HasDefaultValueSql("SUSER_SNAME()");
            
        builder.Property(c => c.ModificadoPor)
            .HasMaxLength(256);
    }
}

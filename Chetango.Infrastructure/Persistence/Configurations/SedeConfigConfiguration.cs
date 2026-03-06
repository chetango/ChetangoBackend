using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para SedeConfig.
/// Un registro por sede de cada tenant: (TenantId, SedeValor) es único.
/// </summary>
public class SedeConfigConfiguration : IEntityTypeConfiguration<SedeConfig>
{
    public void Configure(EntityTypeBuilder<SedeConfig> builder)
    {
        builder.ToTable("SedeConfigs");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId)
            .IsRequired();

        builder.Property(s => s.SedeValor)
            .IsRequired();

        builder.Property(s => s.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Activa)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.EsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.Orden)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(s => s.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // Un tenant no puede tener dos registros con el mismo valor de sede
        builder.HasIndex(s => new { s.TenantId, s.SedeValor })
            .IsUnique()
            .HasDatabaseName("IX_SedeConfigs_TenantId_SedeValor");

        builder.HasIndex(s => s.TenantId)
            .HasDatabaseName("IX_SedeConfigs_TenantId");
    }
}

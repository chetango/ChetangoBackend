using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        builder.ToTable("TenantUsers");

        builder.HasKey(tu => tu.Id);

        builder.Property(tu => tu.TenantId)
            .IsRequired();

        builder.Property(tu => tu.IdUsuario)
            .IsRequired();

        builder.Property(tu => tu.FechaAsignacion)
            .IsRequired();

        builder.Property(tu => tu.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Relación con Tenant
        builder.HasOne(tu => tu.Tenant)
            .WithMany()
            .HasForeignKey(tu => tu.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Usuario
        builder.HasOne(tu => tu.Usuario)
            .WithMany()
            .HasForeignKey(tu => tu.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice único: un usuario puede estar asignado solo una vez a un tenant
        builder.HasIndex(tu => new { tu.TenantId, tu.IdUsuario })
            .IsUnique();

        // Índice para búsquedas por usuario
        builder.HasIndex(tu => tu.IdUsuario);
    }
}

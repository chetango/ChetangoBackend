using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class AceptacionDocumentoConfiguration : IEntityTypeConfiguration<AceptacionDocumento>
{
    public void Configure(EntityTypeBuilder<AceptacionDocumento> builder)
    {
        builder.ToTable("AceptacionesDocumento");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FechaAceptacion)
            .IsRequired();

        builder.Property(a => a.IpOrigen)
            .IsRequired()
            .HasMaxLength(45); // IPv6 max = 45 chars

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.Contexto)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Onboarding");

        // Índice para consultar rápido todas las aceptaciones de un tenant
        builder.HasIndex(a => a.TenantId);

        // Índice para consultar aceptaciones por usuario
        builder.HasIndex(a => a.IdUsuario);

        // Índice para saber si un tenant ya aceptó una versión concreta
        builder.HasIndex(a => new { a.TenantId, a.VersionDocumentoLegalId }).IsUnique();

        builder.HasOne(a => a.Tenant)
            .WithMany(t => t.AceptacionesDocumentos)
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.VersionDocumentoLegal)
            .WithMany(v => v.Aceptaciones)
            .HasForeignKey(a => a.VersionDocumentoLegalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

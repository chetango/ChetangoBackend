using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        builder.ToTable("Auditorias");
        builder.HasKey(a => a.IdAuditoria);

        builder.Property(a => a.Modulo).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Accion).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Descripcion).IsRequired().HasMaxLength(1000);

        builder.Property(a => a.FechaHora)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(a => a.Usuario)
            .WithMany(u => u.Auditorias)
            .HasForeignKey(a => a.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.FechaHora);
    }
}

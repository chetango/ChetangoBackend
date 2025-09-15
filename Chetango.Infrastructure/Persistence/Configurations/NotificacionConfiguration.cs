using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("Notificaciones");
        builder.HasKey(n => n.IdNotificacion);

        builder.Property(n => n.Mensaje)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.FechaEnvio)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(n => n.Usuario)
            .WithMany(u => u.Notificaciones)
            .HasForeignKey(n => n.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Estado)
            .WithMany()
            .HasForeignKey(n => n.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => new { n.IdUsuario, n.Leida });
    }
}

// Nueva configuración para ConfiguracionNotificaciones
public class ConfiguracionNotificacionesConfiguration : IEntityTypeConfiguration<ConfiguracionNotificaciones>
{
    public void Configure(EntityTypeBuilder<ConfiguracionNotificaciones> builder)
    {
        builder.ToTable("ConfiguracionesNotificaciones");
        builder.HasKey(c => c.IdConfig);
        builder.Property(c => c.AnticipacionAlerta).IsRequired();
        builder.Property(c => c.TextoVencimiento).IsRequired().HasMaxLength(300);
        builder.Property(c => c.TextoAgotamiento).IsRequired().HasMaxLength(300);
    }
}

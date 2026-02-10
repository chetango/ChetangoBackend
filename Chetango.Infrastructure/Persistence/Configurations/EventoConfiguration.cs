using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class EventoConfiguration : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.ToTable("Eventos");
        builder.HasKey(e => e.IdEvento);

        builder.Property(e => e.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Descripcion)
            .HasMaxLength(1000);

        builder.Property(e => e.Fecha)
            .IsRequired();

        builder.Property(e => e.Hora)
            .HasColumnType("time");

        builder.Property(e => e.Precio)
            .HasColumnType("decimal(10,2)");

        builder.Property(e => e.Destacado)
            .HasDefaultValue(false);

        builder.Property(e => e.ImagenUrl)
            .HasMaxLength(500);

        builder.Property(e => e.ImagenNombre)
            .HasMaxLength(200);

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.FechaCreacion)
            .HasDefaultValueSql("GETDATE()");

        // Relación con Usuario (Creador)
        builder.HasOne(e => e.UsuarioCreador)
            .WithMany()
            .HasForeignKey(e => e.IdUsuarioCreador)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para mejorar consultas
        builder.HasIndex(e => e.Fecha);
        builder.HasIndex(e => e.Activo);
        builder.HasIndex(e => e.Destacado);
    }
}

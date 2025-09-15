using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.IdUsuario);

        builder.Property(u => u.NombreUsuario)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.NombreUsuario).IsUnique();

        // PasswordHash eliminado

        builder.Property(u => u.NumeroDocumento)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(u => u.NumeroDocumento).IsUnique();

        builder.Property(u => u.Correo)
            .IsRequired()
            .HasMaxLength(150);
        builder.HasIndex(u => u.Correo).IsUnique();

        builder.Property(u => u.Telefono)
            .HasMaxLength(30);

        builder.Property(u => u.FechaCreacion)
            .HasDefaultValueSql("GETUTCDATE()");

        // FKs explícitas
        builder.HasOne(u => u.TipoDocumento)
            .WithMany(td => td.Usuarios)
            .HasForeignKey(u => u.IdTipoDocumento)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Estado)
            .WithMany()
            .HasForeignKey(u => u.IdEstadoUsuario)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

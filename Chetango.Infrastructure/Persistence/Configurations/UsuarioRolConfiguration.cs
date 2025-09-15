using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    public void Configure(EntityTypeBuilder<UsuarioRol> builder)
    {
        builder.ToTable("UsuariosRoles");
        builder.HasKey(ur => new { ur.IdUsuario, ur.IdRol });

        builder.HasOne(ur => ur.Usuario)
            .WithMany(u => u.Roles)
            .HasForeignKey(ur => ur.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Rol)
            .WithMany(r => r.UsuariosRoles)
            .HasForeignKey(ur => ur.IdRol)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

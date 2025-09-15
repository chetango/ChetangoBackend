using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class ProfesorConfiguration : IEntityTypeConfiguration<Profesor>
{
    public void Configure(EntityTypeBuilder<Profesor> builder)
    {
        builder.ToTable("Profesores");
        builder.HasKey(p => p.IdProfesor);

        builder.HasOne(p => p.Usuario)
            .WithMany(u => u.Profesores)
            .HasForeignKey(p => p.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.TipoProfesor)
            .WithMany(tp => tp.Profesores)
            .HasForeignKey(p => p.IdTipoProfesor)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

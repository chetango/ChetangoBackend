using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class ClaseConfiguration : IEntityTypeConfiguration<Clase>
{
    public void Configure(EntityTypeBuilder<Clase> builder)
    {
        builder.ToTable("Clases");
        builder.HasKey(c => c.IdClase);

        builder.Property(c => c.Fecha).IsRequired();
        builder.Property(c => c.HoraInicio).IsRequired();
        builder.Property(c => c.HoraFin).IsRequired();
        builder.Property(c => c.Estado)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Programada");

        // DEPRECATED: IdProfesorPrincipal ahora es nullable
        // Usar Clase.Profesores para obtener lista de profesores con roles
        builder.HasOne(c => c.ProfesorPrincipal)
            .WithMany(p => p.Clases)
            .HasForeignKey(c => c.IdProfesorPrincipal)
            .IsRequired(false) // Ahora nullable
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.TipoClase)
            .WithMany(tc => tc.Clases)
            .HasForeignKey(c => c.IdTipoClase)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.Fecha, c.IdTipoClase });
    }
}

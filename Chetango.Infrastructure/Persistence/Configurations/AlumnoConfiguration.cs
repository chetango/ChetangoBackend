using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class AlumnoConfiguration : IEntityTypeConfiguration<Alumno>
{
    public void Configure(EntityTypeBuilder<Alumno> builder)
    {
        builder.ToTable("Alumnos");
        builder.HasKey(a => a.IdAlumno);

        builder.Property(a => a.FechaInscripcion)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(a => a.IdEstado)
            .HasDefaultValue(1); // Default: Activo

        builder.HasOne(a => a.Usuario)
            .WithMany(u => u.Alumnos)
            .HasForeignKey(a => a.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Estado)
            .WithMany(e => e.Alumnos)
            .HasForeignKey(a => a.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

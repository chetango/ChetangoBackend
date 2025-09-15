using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class AsistenciaConfiguration : IEntityTypeConfiguration<Asistencia>
{
    public void Configure(EntityTypeBuilder<Asistencia> builder)
    {
        builder.ToTable("Asistencias");
        builder.HasKey(a => a.IdAsistencia);

        builder.HasOne(a => a.Clase)
            .WithMany(c => c.Asistencias)
            .HasForeignKey(a => a.IdClase)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Alumno)
            .WithMany(al => al.Asistencias)
            .HasForeignKey(a => a.IdAlumno)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.PaqueteUsado)
            .WithMany()
            .HasForeignKey(a => a.IdPaqueteUsado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Estado)
            .WithMany()
            .HasForeignKey(a => a.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.Observacion)
            .HasMaxLength(500);

        builder.HasIndex(a => new { a.IdClase, a.IdAlumno }).IsUnique(); // Evita doble asistencia del mismo alumno a misma clase
    }
}

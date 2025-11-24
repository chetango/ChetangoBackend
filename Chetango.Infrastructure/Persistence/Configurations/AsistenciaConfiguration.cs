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
            .OnDelete(DeleteBehavior.Cascade); // Borrar clase -> elimina asistencias relacionadas

        builder.HasOne(a => a.Alumno)
            .WithMany(al => al.Asistencias)
            .HasForeignKey(a => a.IdAlumno)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.PaqueteUsado)
            .WithMany()
            .HasForeignKey(a => a.IdPaqueteUsado)
            .OnDelete(DeleteBehavior.Restrict); // Evita borrar paquete si existe asistencia referenciándolo

        builder.HasOne(a => a.Estado)
            .WithMany()
            .HasForeignKey(a => a.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.Observacion)
            .HasMaxLength(500);

        // Campos de auditoría
        builder.Property(a => a.FechaRegistro)
            .HasDefaultValueSql("GETDATE()");
        builder.Property(a => a.UsuarioCreacion)
            .HasMaxLength(256)
            .HasDefaultValueSql("SUSER_SNAME()");
        builder.Property(a => a.UsuarioModificacion)
            .HasMaxLength(256);

        builder.HasIndex(a => new { a.IdClase, a.IdAlumno }).IsUnique(); // Impide duplicar asistencia del mismo alumno a la misma clase
    }
}

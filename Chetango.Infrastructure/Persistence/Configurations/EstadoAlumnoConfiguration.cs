using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class EstadoAlumnoConfiguration : IEntityTypeConfiguration<EstadoAlumno>
{
    public void Configure(EntityTypeBuilder<EstadoAlumno> builder)
    {
        builder.ToTable("EstadosAlumno");
        builder.HasKey(e => e.IdEstado);

        builder.Property(e => e.Nombre)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => e.Nombre)
            .IsUnique();

        builder.Property(e => e.Descripcion)
            .HasMaxLength(200);

        // Seed data
        builder.HasData(
            new EstadoAlumno { IdEstado = 1, Nombre = "Activo", Descripcion = "Alumno activo asistiendo a clases" },
            new EstadoAlumno { IdEstado = 2, Nombre = "Inactivo", Descripcion = "Alumno que dejó de asistir temporalmente" },
            new EstadoAlumno { IdEstado = 3, Nombre = "Suspendido", Descripcion = "Alumno suspendido por razones administrativas" },
            new EstadoAlumno { IdEstado = 4, Nombre = "Retirado", Descripcion = "Alumno que se retiró definitivamente" }
        );
    }
}

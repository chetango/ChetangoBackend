using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class TipoAsistenciaConfiguration : IEntityTypeConfiguration<TipoAsistencia>
{
    public void Configure(EntityTypeBuilder<TipoAsistencia> builder)
    {
        builder.ToTable("TiposAsistencia");
        builder.HasKey(t => t.IdTipoAsistencia);

        builder.Property(t => t.Nombre)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Descripcion)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.RequierePaquete)
            .IsRequired();

        builder.Property(t => t.DescontarClase)
            .IsRequired();

        builder.Property(t => t.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(t => t.Nombre)
            .IsUnique();

        // Seed data inicial
        builder.HasData(
            new TipoAsistencia
            {
                IdTipoAsistencia = 1,
                Nombre = "Normal",
                Descripcion = "Asistencia normal con paquete activo",
                RequierePaquete = true,
                DescontarClase = true,
                Activo = true
            },
            new TipoAsistencia
            {
                IdTipoAsistencia = 2,
                Nombre = "Cortesía",
                Descripcion = "Clase de cortesía sin descuento de paquete",
                RequierePaquete = false,
                DescontarClase = false,
                Activo = true
            },
            new TipoAsistencia
            {
                IdTipoAsistencia = 3,
                Nombre = "Clase de Prueba",
                Descripcion = "Clase de prueba para nuevos alumnos",
                RequierePaquete = false,
                DescontarClase = false,
                Activo = true
            },
            new TipoAsistencia
            {
                IdTipoAsistencia = 4,
                Nombre = "Recuperación",
                Descripcion = "Clase de recuperación por inasistencia justificada",
                RequierePaquete = true,
                DescontarClase = false, // No descuenta porque es recuperación
                Activo = true
            }
        );
    }
}

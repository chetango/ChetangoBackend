using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class SolicitudRenovacionPaqueteConfiguration : IEntityTypeConfiguration<SolicitudRenovacionPaquete>
{
    public void Configure(EntityTypeBuilder<SolicitudRenovacionPaquete> builder)
    {
        builder.ToTable("SolicitudesRenovacionPaquete");
        builder.HasKey(s => s.IdSolicitud);

        builder.Property(s => s.TipoPaqueteDeseado)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.MensajeAlumno)
            .HasMaxLength(1000);

        builder.Property(s => s.Estado)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pendiente");

        builder.Property(s => s.FechaSolicitud)
            .IsRequired();

        builder.Property(s => s.MensajeRespuesta)
            .HasMaxLength(1000);

        // Relaciones
        builder.HasOne(s => s.Alumno)
            .WithMany()
            .HasForeignKey(s => s.IdAlumno)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(s => s.IdAlumno);
        builder.HasIndex(s => s.Estado);
        builder.HasIndex(s => s.FechaSolicitud);
    }
}

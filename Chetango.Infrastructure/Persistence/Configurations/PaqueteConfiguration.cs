using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class PaqueteConfiguration : IEntityTypeConfiguration<Paquete>
{
    public void Configure(EntityTypeBuilder<Paquete> builder)
    {
        builder.ToTable("Paquetes");
        builder.HasKey(p => p.IdPaquete);

        builder.Property(p => p.ClasesDisponibles).IsRequired();
        builder.Property(p => p.ClasesUsadas).IsRequired();
        builder.Property(p => p.ValorPaquete).HasColumnType("decimal(18,2)"); // Manejo consistente de dinero
        builder.Property(p => p.FechaActivacion).IsRequired();
        builder.Property(p => p.FechaVencimiento).IsRequired();

        // Relación unidireccional Alumno->Paquete (no se navega desde Alumno de momento para reducir carga)
        builder.HasOne(p => p.Alumno)
            .WithMany()
            .HasForeignKey(p => p.IdAlumno)
            .OnDelete(DeleteBehavior.Restrict);

        // Pago opcional (puede ser cortesía). SetNull preserva historial si se elimina el pago.
        builder.HasOne(p => p.Pago)
            .WithMany(pg => pg.Paquetes)
            .HasForeignKey(p => p.IdPago)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.TipoPaquete)
            .WithMany(tp => tp.Paquetes)
            .HasForeignKey(p => p.IdTipoPaquete)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Estado)
            .WithMany()
            .HasForeignKey(p => p.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);

        // Consultas frecuentes: paquetes activos próximos a vencer por alumno
        builder.HasIndex(p => new { p.IdAlumno, p.FechaVencimiento });
    }
}

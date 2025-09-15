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
        builder.Property(p => p.ValorPaquete).HasColumnType("decimal(18,2)");
        builder.Property(p => p.FechaActivacion).IsRequired();
        builder.Property(p => p.FechaVencimiento).IsRequired();

        builder.HasOne(p => p.Alumno)
            .WithMany() // unidireccional
            .HasForeignKey(p => p.IdAlumno)
            .OnDelete(DeleteBehavior.Restrict);

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

        builder.HasIndex(p => new { p.IdAlumno, p.FechaVencimiento });
    }
}

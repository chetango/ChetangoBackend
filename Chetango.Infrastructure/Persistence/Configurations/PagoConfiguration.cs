using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("Pagos");
        builder.HasKey(p => p.IdPago);

        builder.Property(p => p.MontoTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.FechaPago)
            .IsRequired();

        // Campos de auditoría
        builder.Property(p => p.FechaCreacion)
            .HasDefaultValueSql("GETDATE()");
        builder.Property(p => p.UsuarioCreacion)
            .HasMaxLength(256)
            .HasDefaultValueSql("SUSER_SNAME()");
        builder.Property(p => p.UsuarioModificacion)
            .HasMaxLength(256);

        builder.HasOne(p => p.Alumno)
            .WithMany(a => a.Pagos)
            .HasForeignKey(p => p.IdAlumno)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.MetodoPago)
            .WithMany(mp => mp.Pagos)
            .HasForeignKey(p => p.IdMetodoPago)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.EstadoPago)
            .WithMany(e => e.Pagos)
            .HasForeignKey(p => p.IdEstadoPago)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

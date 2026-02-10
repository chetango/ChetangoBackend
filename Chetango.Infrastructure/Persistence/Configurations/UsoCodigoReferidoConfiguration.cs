using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class UsoCodigoReferidoConfiguration : IEntityTypeConfiguration<UsoCodigoReferido>
{
    public void Configure(EntityTypeBuilder<UsoCodigoReferido> builder)
    {
        builder.ToTable("UsosCodigoReferido");
        builder.HasKey(u => u.IdUso);

        builder.Property(u => u.FechaUso)
            .IsRequired();

        builder.Property(u => u.Estado)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pendiente");

        builder.Property(u => u.BeneficioAplicadoReferidor)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.BeneficioAplicadoNuevo)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.Observaciones)
            .HasMaxLength(1000);

        // Relaciones
        builder.HasOne(u => u.CodigoReferido)
            .WithMany(c => c.Usos)
            .HasForeignKey(u => u.IdCodigoReferido)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.AlumnoReferidor)
            .WithMany()
            .HasForeignKey(u => u.IdAlumnoReferidor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.AlumnoNuevo)
            .WithMany()
            .HasForeignKey(u => u.IdAlumnoNuevo)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(u => u.IdCodigoReferido);
        builder.HasIndex(u => u.IdAlumnoReferidor);
        builder.HasIndex(u => u.IdAlumnoNuevo);
        builder.HasIndex(u => u.FechaUso);
        builder.HasIndex(u => u.Estado);
    }
}

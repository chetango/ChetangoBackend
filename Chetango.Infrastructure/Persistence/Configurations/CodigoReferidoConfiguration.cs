using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class CodigoReferidoConfiguration : IEntityTypeConfiguration<CodigoReferido>
{
    public void Configure(EntityTypeBuilder<CodigoReferido> builder)
    {
        builder.ToTable("CodigosReferido");
        builder.HasKey(c => c.IdCodigo);

        builder.Property(c => c.Codigo)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.VecesUsado)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.BeneficioReferidor)
            .HasMaxLength(500);

        builder.Property(c => c.BeneficioNuevoAlumno)
            .HasMaxLength(500);

        builder.Property(c => c.FechaCreacion)
            .IsRequired();

        // Relaciones
        builder.HasOne(c => c.Alumno)
            .WithMany()
            .HasForeignKey(c => c.IdAlumno)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(c => c.Codigo)
            .IsUnique();
        builder.HasIndex(c => c.IdAlumno);
        builder.HasIndex(c => c.Activo);
    }
}

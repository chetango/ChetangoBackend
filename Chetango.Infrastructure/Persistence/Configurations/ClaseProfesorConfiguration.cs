using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class ClaseProfesorConfiguration : IEntityTypeConfiguration<ClaseProfesor>
{
    public void Configure(EntityTypeBuilder<ClaseProfesor> builder)
    {
        builder.ToTable("ClasesProfesores");
        builder.HasKey(cp => cp.IdClaseProfesor);

        builder.Property(cp => cp.TarifaProgramada)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(cp => cp.ValorAdicional)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(cp => cp.TotalPago)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(cp => cp.EstadoPago)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Pendiente");

        builder.Property(cp => cp.ConceptoAdicional)
            .HasMaxLength(500);

        builder.Property(cp => cp.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // Relaciones
        builder.HasOne(cp => cp.Clase)
            .WithMany(c => c.Profesores)
            .HasForeignKey(cp => cp.IdClase)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cp => cp.Profesor)
            .WithMany(p => p.ClasesProfesores)
            .HasForeignKey(cp => cp.IdProfesor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cp => cp.RolEnClase)
            .WithMany()
            .HasForeignKey(cp => cp.IdRolEnClase)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cp => cp.AprobadoPor)
            .WithMany()
            .HasForeignKey(cp => cp.AprobadoPorIdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(cp => new { cp.IdClase, cp.IdProfesor });
        builder.HasIndex(cp => cp.EstadoPago);
        builder.HasIndex(cp => cp.FechaAprobacion);
    }
}

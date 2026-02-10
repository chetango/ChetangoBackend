using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class LiquidacionMensualConfiguration : IEntityTypeConfiguration<LiquidacionMensual>
{
    public void Configure(EntityTypeBuilder<LiquidacionMensual> builder)
    {
        builder.ToTable("LiquidacionesMensuales");
        builder.HasKey(l => l.IdLiquidacion);

        builder.Property(l => l.Mes)
            .IsRequired();

        builder.Property(l => l.Año)
            .IsRequired();

        builder.Property(l => l.TotalHoras)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(l => l.TotalBase)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(l => l.TotalAdicionales)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(l => l.TotalPagar)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(l => l.Estado)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("EnProceso");

        builder.Property(l => l.Observaciones)
            .HasMaxLength(1000);

        builder.Property(l => l.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // Relaciones
        builder.HasOne(l => l.Profesor)
            .WithMany(p => p.Liquidaciones)
            .HasForeignKey(l => l.IdProfesor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.CreadoPor)
            .WithMany()
            .HasForeignKey(l => l.CreadoPorIdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(l => new { l.IdProfesor, l.Mes, l.Año })
            .IsUnique();
        builder.HasIndex(l => l.Estado);
    }
}

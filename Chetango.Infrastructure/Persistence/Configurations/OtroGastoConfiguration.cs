using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class OtroGastoConfiguration : IEntityTypeConfiguration<OtroGasto>
{
    public void Configure(EntityTypeBuilder<OtroGasto> builder)
    {
        builder.ToTable("OtrosGastos");
        builder.HasKey(o => o.IdOtroGasto);

        builder.Property(o => o.Concepto)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.Monto)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Fecha)
            .IsRequired();

        builder.Property(o => o.Sede)
            .IsRequired();

        builder.Property(o => o.Proveedor)
            .HasMaxLength(200);

        builder.Property(o => o.Descripcion)
            .HasMaxLength(1000);

        builder.Property(o => o.UrlFactura)
            .HasMaxLength(500);

        builder.Property(o => o.NumeroFactura)
            .HasMaxLength(100);

        // Campos de auditoría
        builder.Property(o => o.FechaCreacion)
            .HasDefaultValueSql("GETDATE()");
        builder.Property(o => o.UsuarioCreacion)
            .HasMaxLength(256)
            .HasDefaultValueSql("SUSER_SNAME()");
        builder.Property(o => o.UsuarioModificacion)
            .HasMaxLength(256);
        
        // Soft Delete
        builder.Property(o => o.Eliminado)
            .HasDefaultValue(false);
        builder.Property(o => o.UsuarioEliminacion)
            .HasMaxLength(256);

        // Query Filter para excluir registros eliminados por defecto
        builder.HasQueryFilter(o => !o.Eliminado);

        // Relación con CategoriaGasto
        builder.HasOne(o => o.CategoriaGasto)
            .WithMany(c => c.OtrosGastos)
            .HasForeignKey(o => o.IdCategoriaGasto)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para mejorar rendimiento de consultas
        builder.HasIndex(o => o.Fecha);
        builder.HasIndex(o => o.Sede);
        builder.HasIndex(o => new { o.Fecha, o.Sede });
    }
}

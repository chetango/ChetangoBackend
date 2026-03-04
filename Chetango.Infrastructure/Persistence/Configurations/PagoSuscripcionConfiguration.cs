using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad PagoSuscripcion.
/// Define la estructura de la tabla PagosSuscripcion en base de datos.
/// </summary>
public class PagoSuscripcionConfiguration : IEntityTypeConfiguration<PagoSuscripcion>
{
    public void Configure(EntityTypeBuilder<PagoSuscripcion> builder)
    {
        builder.ToTable("PagosSuscripcion");
        builder.HasKey(p => p.Id);
        
        // Información del pago
        builder.Property(p => p.FechaPago)
            .IsRequired();
            
        builder.Property(p => p.Monto)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
            
        builder.Property(p => p.Referencia)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(p => p.Referencia).IsUnique(); // Referencia única
            
        builder.Property(p => p.MetodoPago)
            .IsRequired()
            .HasMaxLength(50);
        
        // Comprobante
        builder.Property(p => p.ComprobanteUrl)
            .HasMaxLength(500);
            
        builder.Property(p => p.NombreArchivo)
            .HasMaxLength(200);
        
        // Estado y aprobación
        builder.Property(p => p.Estado)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Pendiente");
            
        builder.Property(p => p.AprobadoPor)
            .HasMaxLength(100);
            
        builder.Property(p => p.Observaciones)
            .HasMaxLength(500);
        
        // Datos de transacción (para pagos automáticos)
        builder.Property(p => p.TransaccionId)
            .HasMaxLength(100);
            
        builder.Property(p => p.EstadoTransaccion)
            .HasMaxLength(50);
        
        // Auditoría
        builder.Property(p => p.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        builder.Property(p => p.CreadoPor)
            .HasMaxLength(256)
            .HasDefaultValueSql("SUSER_SNAME()");
            
        builder.Property(p => p.ModificadoPor)
            .HasMaxLength(256);
        
        // Relación con Tenant
        builder.HasOne(p => p.Tenant)
            .WithMany(t => t.PagosSuscripcion)
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Índices para optimizar búsquedas
        builder.HasIndex(p => p.TenantId);
        builder.HasIndex(p => p.Estado);
        builder.HasIndex(p => p.FechaPago);
        builder.HasIndex(p => new { p.TenantId, p.FechaPago }); // Índice compuesto
    }
}

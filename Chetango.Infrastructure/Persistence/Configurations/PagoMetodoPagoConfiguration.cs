using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

// Catálogo MetodoPago
public class MetodoPagoConfiguration : IEntityTypeConfiguration<MetodoPago>
{
    public void Configure(EntityTypeBuilder<MetodoPago> builder)
    {
        builder.ToTable("MetodosPago");
        builder.HasKey(mp => mp.Id);
        builder.Property(mp => mp.Nombre).IsRequired().HasMaxLength(100);
        builder.HasIndex(mp => mp.Nombre).IsUnique();
        // Seed fijo (GUIDs determinísticos) para entornos iniciales
        builder.HasData(
            new MetodoPago { Id = Guid.Parse("10101010-1010-1010-1010-101010101010"), Nombre = "Efectivo" },
            new MetodoPago { Id = Guid.Parse("20202020-2020-2020-2020-202020202020"), Nombre = "Transferencia" },
            new MetodoPago { Id = Guid.Parse("30303030-3030-3030-3030-303030303030"), Nombre = "Tarjeta" },
            new MetodoPago { Id = Guid.Parse("40404040-4040-4040-4040-404040404040"), Nombre = "Bono" },
            new MetodoPago { Id = Guid.Parse("50505050-5050-5050-5050-505050505050"), Nombre = "Cortesia" }
        );
    }
}

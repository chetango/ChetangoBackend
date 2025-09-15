using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class CongelacionPaqueteConfiguration : IEntityTypeConfiguration<CongelacionPaquete>
{
    public void Configure(EntityTypeBuilder<CongelacionPaquete> builder)
    {
        builder.ToTable("CongelacionesPaquete");
        builder.HasKey(c => c.IdCongelacion);

        builder.HasOne(c => c.Paquete)
            .WithMany(p => p.Congelaciones)
            .HasForeignKey(c => c.IdPaquete)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.IdPaquete, c.FechaInicio, c.FechaFin });
    }
}

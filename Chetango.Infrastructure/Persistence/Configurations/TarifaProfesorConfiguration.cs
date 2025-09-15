using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class TarifaProfesorConfiguration : IEntityTypeConfiguration<TarifaProfesor>
{
    public void Configure(EntityTypeBuilder<TarifaProfesor> builder)
    {
        builder.ToTable("TarifasProfesor");
        builder.HasKey(t => t.IdTarifa);

        builder.Property(t => t.ValorPorClase)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(t => t.TipoProfesor)
            .WithMany() // TipoProfesor aún no expone colección de Tarifas
            .HasForeignKey(t => t.IdTipoProfesor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.RolEnClase)
            .WithMany(r => r.Tarifas)
            .HasForeignKey(t => t.IdRolEnClase)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.IdTipoProfesor, t.IdRolEnClase }).IsUnique();
    }
}

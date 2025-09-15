using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class MonitorClaseConfiguration : IEntityTypeConfiguration<MonitorClase>
{
    public void Configure(EntityTypeBuilder<MonitorClase> builder)
    {
        builder.ToTable("MonitoresClase");
        builder.HasKey(m => m.IdMonitorClase);

        builder.HasOne(m => m.Clase)
            .WithMany(c => c.Monitores)
            .HasForeignKey(m => m.IdClase)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Profesor)
            .WithMany(p => p.MonitorClases)
            .HasForeignKey(m => m.IdProfesor)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.IdClase, m.IdProfesor }).IsUnique();
    }
}

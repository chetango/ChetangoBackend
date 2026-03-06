using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class VersionDocumentoLegalConfiguration : IEntityTypeConfiguration<VersionDocumentoLegal>
{
    public void Configure(EntityTypeBuilder<VersionDocumentoLegal> builder)
    {
        builder.ToTable("VersionesDocumentoLegal");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.NumeroVersion)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(v => v.UrlDocumento)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(v => v.ResumenCambios)
            .HasMaxLength(1000);

        builder.Property(v => v.FechaVigencia)
            .IsRequired();

        builder.Property(v => v.EsCambioSignificativo)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(v => v.Activa)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(v => v.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(v => v.CreadoPor)
            .IsRequired()
            .HasMaxLength(256);

        // Solo puede haber una versión activa por documento
        builder.HasIndex(v => new { v.DocumentoLegalId, v.NumeroVersion }).IsUnique();

        builder.HasOne(v => v.DocumentoLegal)
            .WithMany(d => d.Versiones)
            .HasForeignKey(v => v.DocumentoLegalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed: versión 1.0 de cada documento (placeholder — el texto real lo carga el abogado)
        builder.HasData(
            new VersionDocumentoLegal
            {
                Id = new Guid("bb000001-0000-0000-0000-000000000001"),
                DocumentoLegalId = new Guid("aa000001-0000-0000-0000-000000000001"),
                NumeroVersion = "1.0",
                UrlDocumento = "/docs/terminos-v1.0.html",
                ResumenCambios = "Versión inicial.",
                FechaVigencia = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                EsCambioSignificativo = false,
                Activa = true,
                FechaCreacion = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                CreadoPor = "SISTEMA"
            },
            new VersionDocumentoLegal
            {
                Id = new Guid("bb000002-0000-0000-0000-000000000002"),
                DocumentoLegalId = new Guid("aa000002-0000-0000-0000-000000000002"),
                NumeroVersion = "1.0",
                UrlDocumento = "/docs/dpa-v1.0.html",
                ResumenCambios = "Versión inicial.",
                FechaVigencia = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                EsCambioSignificativo = false,
                Activa = true,
                FechaCreacion = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                CreadoPor = "SISTEMA"
            },
            new VersionDocumentoLegal
            {
                Id = new Guid("bb000003-0000-0000-0000-000000000003"),
                DocumentoLegalId = new Guid("aa000003-0000-0000-0000-000000000003"),
                NumeroVersion = "1.0",
                UrlDocumento = "/docs/politica-privacidad-v1.0.html",
                ResumenCambios = "Versión inicial.",
                FechaVigencia = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                EsCambioSignificativo = false,
                Activa = true,
                FechaCreacion = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                CreadoPor = "SISTEMA"
            },
            new VersionDocumentoLegal
            {
                Id = new Guid("bb000004-0000-0000-0000-000000000004"),
                DocumentoLegalId = new Guid("aa000004-0000-0000-0000-000000000004"),
                NumeroVersion = "1.0",
                UrlDocumento = "/docs/aviso-privacidad-v1.0.html",
                ResumenCambios = "Versión inicial.",
                FechaVigencia = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                EsCambioSignificativo = false,
                Activa = true,
                FechaCreacion = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                CreadoPor = "SISTEMA"
            }
        );
    }
}

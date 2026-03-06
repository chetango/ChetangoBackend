using Chetango.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class DocumentoLegalConfiguration : IEntityTypeConfiguration<DocumentoLegal>
{
    public void Configure(EntityTypeBuilder<DocumentoLegal> builder)
    {
        builder.ToTable("DocumentosLegales");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Codigo)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(d => d.Codigo).IsUnique();

        builder.Property(d => d.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Descripcion)
            .HasMaxLength(500);

        builder.Property(d => d.Destinatario)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Admin");

        builder.Property(d => d.EsObligatorio)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.RequiereReaceptacion)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(d => d.CreadoPor)
            .IsRequired()
            .HasMaxLength(256);

        // Seed inicial de documentos base
        builder.HasData(
            new DocumentoLegal
            {
                Id = new Guid("aa000001-0000-0000-0000-000000000001"),
                Codigo = "TERMINOS",
                Nombre = "Términos y Condiciones del Servicio",
                Descripcion = "Contrato SaaS entre Aphellion y la academia cliente.",
                Destinatario = "Admin",
                EsObligatorio = true,
                RequiereReaceptacion = true,
                Activo = true,
                FechaCreacion = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                CreadoPor = "SISTEMA"
            },
            new DocumentoLegal
            {
                Id = new Guid("aa000002-0000-0000-0000-000000000002"),
                Codigo = "DPA",
                Nombre = "Acuerdo de Tratamiento de Datos (DPA)",
                Descripcion = "Define las responsabilidades de Aphellion como encargado y de la academia como responsable del tratamiento.",
                Destinatario = "Admin",
                EsObligatorio = true,
                RequiereReaceptacion = true,
                Activo = true,
                FechaCreacion = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                CreadoPor = "SISTEMA"
            },
            new DocumentoLegal
            {
                Id = new Guid("aa000003-0000-0000-0000-000000000003"),
                Codigo = "POLITICA_PRIVACIDAD",
                Nombre = "Política de Privacidad",
                Descripcion = "Cómo Aphellion maneja los datos comerciales del cliente.",
                Destinatario = "Admin",
                EsObligatorio = true,
                RequiereReaceptacion = false,
                Activo = true,
                FechaCreacion = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                CreadoPor = "SISTEMA"
            },
            new DocumentoLegal
            {
                Id = new Guid("aa000004-0000-0000-0000-000000000004"),
                Codigo = "AVISO_PRIVACIDAD",
                Nombre = "Aviso de Privacidad",
                Descripcion = "Aviso corto para usuarios finales (profesores y alumnos) sobre el tratamiento de sus datos.",
                Destinatario = "Todos",
                EsObligatorio = true,
                RequiereReaceptacion = false,
                Activo = true,
                FechaCreacion = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                CreadoPor = "SISTEMA"
            }
        );
    }
}

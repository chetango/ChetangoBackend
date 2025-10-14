using Chetango.Domain.Entities.Estados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chetango.Infrastructure.Persistence.Configurations;

public class TipoDocumentoConfiguration : IEntityTypeConfiguration<TipoDocumento>
{
    public void Configure(EntityTypeBuilder<TipoDocumento> builder)
    {
        builder.ToTable("TiposDocumento");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(50);
        builder.HasIndex(t => t.Nombre).IsUnique();
        builder.HasData(
            new TipoDocumento { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Nombre = "CC" },
            new TipoDocumento { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Nombre = "CE" },
            new TipoDocumento { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Nombre = "PAS" },
            new TipoDocumento { Id = Guid.Parse("44444444-1111-2222-3333-555555555555"), Nombre = "OID" } // usado para identidad externa
        );
    }
}

public class TipoClaseConfiguration : IEntityTypeConfiguration<TipoClase>
{
    public void Configure(EntityTypeBuilder<TipoClase> builder)
    {
        builder.ToTable("TiposClase");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
        builder.HasIndex(t => t.Nombre).IsUnique();
        builder.HasData(
            new TipoClase { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Nombre = "Regular" },
            new TipoClase { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Nombre = "Taller" },
            new TipoClase { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Nombre = "Evento" }
        );
    }
}

public class TipoPaqueteConfiguration : IEntityTypeConfiguration<TipoPaquete>
{
    public void Configure(EntityTypeBuilder<TipoPaquete> builder)
    {
        builder.ToTable("TiposPaquete");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
        builder.HasIndex(t => t.Nombre).IsUnique();
        builder.HasData(
            new TipoPaquete { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Nombre = "Mensual" },
            new TipoPaquete { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Nombre = "BonoClases" },
            new TipoPaquete { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Nombre = "Ilimitado" }
        );
    }
}

public class TipoProfesorConfiguration : IEntityTypeConfiguration<TipoProfesor>
{
    public void Configure(EntityTypeBuilder<TipoProfesor> builder)
    {
        builder.ToTable("TiposProfesor");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
        builder.HasIndex(t => t.Nombre).IsUnique();
        builder.HasData(
            new TipoProfesor { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Nombre = "Principal" },
            new TipoProfesor { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Nombre = "Monitor" }
        );
    }
}

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Nombre).IsRequired().HasMaxLength(100);
        builder.HasIndex(r => r.Nombre).IsUnique();
        builder.HasData(
            new Rol { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Nombre = "Administrador" },
            new Rol { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Nombre = "Alumno" },
            new Rol { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Nombre = "Profesor" }
        );
    }
}

public class RolEnClaseConfiguration : IEntityTypeConfiguration<RolEnClase>
{
    public void Configure(EntityTypeBuilder<RolEnClase> builder)
    {
        builder.ToTable("RolesEnClase");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Nombre).IsRequired().HasMaxLength(100);
        builder.HasIndex(r => r.Nombre).IsUnique();
        builder.HasData(
            new RolEnClase { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Nombre = "Principal" },
            new RolEnClase { Id = Guid.Parse("12121212-1212-1212-1212-121212121212"), Nombre = "Monitor" }
        );
    }
}

public class EstadoUsuarioConfiguration : IEntityTypeConfiguration<EstadoUsuario>
{
    public void Configure(EntityTypeBuilder<EstadoUsuario> builder)
    {
        builder.ToTable("EstadosUsuario");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.Nombre).IsUnique();
        builder.HasData(
            new EstadoUsuario { Id = 1, Nombre = "Activo" },
            new EstadoUsuario { Id = 2, Nombre = "Inactivo" },
            new EstadoUsuario { Id = 3, Nombre = "Bloqueado" }
        );
    }
}

public class EstadoPaqueteConfiguration : IEntityTypeConfiguration<EstadoPaquete>
{
    public void Configure(EntityTypeBuilder<EstadoPaquete> builder)
    {
        builder.ToTable("EstadosPaquete");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.Nombre).IsUnique();
        builder.HasData(
            new EstadoPaquete { Id = 1, Nombre = "Activo" },
            new EstadoPaquete { Id = 2, Nombre = "Vencido" },
            new EstadoPaquete { Id = 3, Nombre = "Congelado" },
            new EstadoPaquete { Id = 4, Nombre = "Agotado" }
        );
    }
}

public class EstadoAsistenciaConfiguration : IEntityTypeConfiguration<EstadoAsistencia>
{
    public void Configure(EntityTypeBuilder<EstadoAsistencia> builder)
    {
        builder.ToTable("EstadosAsistencia");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.Nombre).IsUnique();
        builder.HasData(
            new EstadoAsistencia { Id = 1, Nombre = "Presente" },
            new EstadoAsistencia { Id = 2, Nombre = "Ausente" },
            new EstadoAsistencia { Id = 3, Nombre = "Justificada" }
        );
    }
}

public class EstadoNotificacionConfiguration : IEntityTypeConfiguration<EstadoNotificacion>
{
    public void Configure(EntityTypeBuilder<EstadoNotificacion> builder)
    {
        builder.ToTable("EstadosNotificacion");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
        builder.HasIndex(e => e.Nombre).IsUnique();
        builder.HasData(
            new EstadoNotificacion { Id = 1, Nombre = "Pendiente" },
            new EstadoNotificacion { Id = 2, Nombre = "Enviada" },
            new EstadoNotificacion { Id = 3, Nombre = "Leida" }
        );
    }
}

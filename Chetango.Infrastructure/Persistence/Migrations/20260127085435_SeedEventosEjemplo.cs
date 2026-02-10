using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedEventosEjemplo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Obtener un usuario admin para usar como creador
            migrationBuilder.Sql(@"
                DECLARE @IdUsuarioAdmin UNIQUEIDENTIFIER;
                SELECT TOP 1 @IdUsuarioAdmin = IdUsuario 
                FROM Usuarios 
                WHERE Correo = 'Chetango@chetangoprueba.onmicrosoft.com';

                -- Si no existe el admin, usar el primer usuario disponible
                IF @IdUsuarioAdmin IS NULL
                BEGIN
                    SELECT TOP 1 @IdUsuarioAdmin = IdUsuario FROM Usuarios;
                END

                -- Evento 1: Taller de Musicalidad (Destacado)
                IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Taller de Musicalidad')
                BEGIN
                    INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, ImagenNombre, Activo, FechaCreacion, FechaModificacion, IdUsuarioCreador)
                    VALUES (
                        NEWID(),
                        'Taller de Musicalidad',
                        'Mejora tu interpretación musical en el tango. Aprende a escuchar y expresar la música a través del movimiento.',
                        DATEADD(DAY, 9, GETDATE()),
                        '15:00:00',
                        25000,
                        1,
                        'https://images.unsplash.com/photo-1504609813442-a8924e83f76e?w=800',
                        'taller-musicalidad.jpg',
                        1,
                        GETDATE(),
                        GETDATE(),
                        @IdUsuarioAdmin
                    );
                END

                -- Evento 2: Milonga de Práctica (Gratis)
                IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Milonga de Práctica')
                BEGIN
                    INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, ImagenNombre, Activo, FechaCreacion, FechaModificacion, IdUsuarioCreador)
                    VALUES (
                        NEWID(),
                        'Milonga de Práctica',
                        'Noche de baile social para practicar lo aprendido en clases. Ambiente relajado y amigable para todos los niveles.',
                        DATEADD(DAY, 4, GETDATE()),
                        '21:00:00',
                        NULL,
                        0,
                        'https://images.unsplash.com/photo-1571156230214-50e0b9d14d45?w=800',
                        'milonga-practica.jpg',
                        1,
                        GETDATE(),
                        GETDATE(),
                        @IdUsuarioAdmin
                    );
                END

                -- Evento 3: Workshop Avanzado
                IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Workshop Avanzado de Tango')
                BEGIN
                    INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, ImagenNombre, Activo, FechaCreacion, FechaModificacion, IdUsuarioCreador)
                    VALUES (
                        NEWID(),
                        'Workshop Avanzado de Tango',
                        'Técnicas avanzadas de tango para bailarines experimentados. Incluye boleos, ganchos y sacadas complejas.',
                        DATEADD(DAY, 16, GETDATE()),
                        '16:00:00',
                        40000,
                        1,
                        'https://images.unsplash.com/photo-1508700929628-666bc8bd84ea?w=800',
                        'workshop-avanzado.jpg',
                        1,
                        GETDATE(),
                        GETDATE(),
                        @IdUsuarioAdmin
                    );
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM Eventos WHERE Titulo IN (
                    'Taller de Musicalidad',
                    'Milonga de Práctica',
                    'Workshop Avanzado de Tango'
                );
            ");
        }
    }
}

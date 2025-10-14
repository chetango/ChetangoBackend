using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chetango.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedMinimalDataForTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed minimal data for testing endpoints without auth
            migrationBuilder.Sql(@"
-- Usuario de prueba
IF NOT EXISTS (SELECT 1 FROM [Usuarios] WHERE [Correo] = N'test.user@local')
BEGIN
    DECLARE @TipoOID uniqueidentifier = (SELECT TOP 1 [Id] FROM [TiposDocumento] WHERE [Nombre] = N'OID');
    IF @TipoOID IS NULL BEGIN
        -- fallback a cualquier tipo
        SET @TipoOID = (SELECT TOP 1 [Id] FROM [TiposDocumento]);
    END
    INSERT INTO [Usuarios] ([IdUsuario],[NombreUsuario],[IdTipoDocumento],[NumeroDocumento],[Correo],[Telefono],[IdEstadoUsuario])
    VALUES ('aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee', N'Test User', @TipoOID, N'oid-test', N'test.user@local', N'0000000000', 1);
END

-- Alumno ligado al usuario de prueba
IF NOT EXISTS (SELECT 1 FROM [Alumnos] WHERE [IdUsuario] = 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee')
BEGIN
    INSERT INTO [Alumnos] ([IdAlumno],[IdUsuario])
    VALUES ('bbbbbbbb-cccc-dddd-eeee-ffffffffffff', 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee');
END

-- Paquete para el alumno de prueba (requerido por FK en Asistencias)
DECLARE @PaqueteId uniqueidentifier = 'f0f0f0f0-0000-0000-0000-000000000000';
IF NOT EXISTS (SELECT 1 FROM [Paquetes] WHERE [IdPaquete] = @PaqueteId)
BEGIN
    DECLARE @TipoPaquete uniqueidentifier = (SELECT TOP 1 [Id] FROM [TiposPaquete] WHERE [Nombre] = N'Mensual');
    IF @TipoPaquete IS NULL SET @TipoPaquete = (SELECT TOP 1 [Id] FROM [TiposPaquete]);
    INSERT INTO [Paquetes] ([IdPaquete],[ClasesDisponibles],[ClasesUsadas],[FechaActivacion],[FechaVencimiento],[IdAlumno],[IdEstado],[IdPago],[IdTipoPaquete],[ValorPaquete])
    VALUES (@PaqueteId, 4, 0, CAST(GETDATE() AS date), DATEADD(day,30,CAST(GETDATE() AS date)), 'bbbbbbbb-cccc-dddd-eeee-ffffffffffff', 1, NULL, @TipoPaquete, 100000);
END

-- TipoProfesor ya existe (Principal) y TipoClase Regular ya existe
-- Profesor para el usuario de prueba
IF NOT EXISTS (SELECT 1 FROM [Profesores] WHERE [IdUsuario] = 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee')
BEGIN
    DECLARE @TipoProfesor uniqueidentifier = (SELECT TOP 1 [Id] FROM [TiposProfesor] WHERE [Nombre] = N'Principal');
    INSERT INTO [Profesores] ([IdProfesor],[IdTipoProfesor],[IdUsuario])
    VALUES ('cccccccc-dddd-eeee-ffff-000000000000', @TipoProfesor, 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee');
END

-- Clase de prueba para hoy
IF NOT EXISTS (SELECT 1 FROM [Clases] WHERE [IdClase] = 'dddddddd-eeee-ffff-0000-111111111111')
BEGIN
    DECLARE @TipoClase uniqueidentifier = (SELECT TOP 1 [Id] FROM [TiposClase] WHERE [Nombre] = N'Regular');
    INSERT INTO [Clases] ([IdClase],[Fecha],[HoraInicio],[HoraFin],[IdTipoClase],[IdProfesorPrincipal])
    VALUES ('dddddddd-eeee-ffff-0000-111111111111', CAST(GETDATE() AS date), '10:00:00', '11:00:00', @TipoClase, 'cccccccc-dddd-eeee-ffff-000000000000');
END

-- Asistencia del alumno a esa clase (usa paquete creado)
IF NOT EXISTS (SELECT 1 FROM [Asistencias] WHERE [IdClase] = 'dddddddd-eeee-ffff-0000-111111111111' AND [IdAlumno] = 'bbbbbbbb-cccc-dddd-eeee-ffffffffffff')
BEGIN
    INSERT INTO [Asistencias] ([IdAsistencia],[IdClase],[IdAlumno],[IdEstado],[IdPaqueteUsado],[Observacion])
    VALUES ('eeeeeeee-ffff-0000-1111-222222222222', 'dddddddd-eeee-ffff-0000-111111111111', 'bbbbbbbb-cccc-dddd-eeee-ffffffffffff', 1, @PaqueteId, N'Asistencia de prueba');
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM [Asistencias] WHERE [IdAsistencia] = 'eeeeeeee-ffff-0000-1111-222222222222';
DELETE FROM [Clases] WHERE [IdClase] = 'dddddddd-eeee-ffff-0000-111111111111';
DELETE FROM [Profesores] WHERE [IdProfesor] = 'cccccccc-dddd-eeee-ffff-000000000000';
DELETE FROM [Paquetes] WHERE [IdPaquete] = 'f0f0f0f0-0000-0000-0000-000000000000';
DELETE FROM [Alumnos] WHERE [IdAlumno] = 'bbbbbbbb-cccc-dddd-eeee-ffffffffffff';
DELETE FROM [Usuarios] WHERE [IdUsuario] = 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee';
");
        }
    }
}

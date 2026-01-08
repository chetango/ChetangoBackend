/********************************************************************************************************
 Script: seed_admin_asistencias.sql
 Objetivo: Poblar datos mínimos para probar los endpoints /api/admin/asistencias/* en QA.

 Instrucciones:
   1. Ajusta la sentencia USE para apuntar a tu base QA.
   2. Ejecuta el script completo desde SQL Server Management Studio o sqlcmd.
   3. El script es idempotente: elimina los registros con los mismos GUID antes de insertar.
*********************************************************************************************************/

-- USE [ChetangoDB_QA]; -- << reemplaza por el nombre real de tu base

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Ahora DATETIME2(0) = SYSDATETIME();
DECLARE @Hoy DATE = CONVERT(date, SYSDATETIME());
DECLARE @ClaseDiaActual DATE = @Hoy;
DECLARE @ClaseDiaAnterior DATE = DATEADD(day, -2, @Hoy);

-- Catálogos existentes
DECLARE @TipoDocCC UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @TipoProfesorPrincipal UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @TipoClaseRegular UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';
DECLARE @TipoPaqueteMensual UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777777';
DECLARE @EstadoUsuarioActivo INT = 1;
DECLARE @EstadoAlumnoActivo INT = 1;
DECLARE @EstadoPaqueteActivo INT = 1;
DECLARE @EstadoPaqueteAgotado INT = 4;
DECLARE @EstadoAsistenciaPresente INT = 1;
DECLARE @EstadoAsistenciaAusente INT = 2;

-- Identificadores fijos (coinciden con la documentación para facilitar pruebas de frontend)
DECLARE @UsuarioProfesor UNIQUEIDENTIFIER = '0c35b7e3-94c1-4d53-8f8f-8c9f4ab1d001';
DECLARE @UsuarioMaria UNIQUEIDENTIFIER    = '50f70680-ff0c-4d9f-b7c7-4a5a4a0b2001';
DECLARE @UsuarioDiego UNIQUEIDENTIFIER    = '50f70680-ff0c-4d9f-b7c7-4a5a4a0b2002';
DECLARE @UsuarioCarlos UNIQUEIDENTIFIER   = '50f70680-ff0c-4d9f-b7c7-4a5a4a0b2003';

DECLARE @ProfesorPrincipal UNIQUEIDENTIFIER = '58a9fcb8-90c4-4d10-9a8a-46a2b938b111';
DECLARE @AlumnoMaria UNIQUEIDENTIFIER       = '3a7db4a6-3f25-4f33-8b93-0a68b6c1a90b';
DECLARE @AlumnoDiego UNIQUEIDENTIFIER       = 'c2eb2364-700f-4df3-b035-534233bf11fc';
DECLARE @AlumnoCarlos UNIQUEIDENTIFIER      = '4af7f7c8-6a02-47ba-8a87-2e4178dbdfc2';

DECLARE @ClaseTangoIntermedio UNIQUEIDENTIFIER = 'c9d5af98-2b1a-4cb4-a1a8-13f3381c93ef';
DECLARE @ClaseTecnicaIndividual UNIQUEIDENTIFIER = 'c6a8a2a0-1b5f-44e1-9bcd-9f1197f4a3ed';

DECLARE @PaqueteMaria UNIQUEIDENTIFIER = '9b2b73cf-1ed4-4e6b-bfb7-882c8c9b0a11';
DECLARE @PaqueteDiego UNIQUEIDENTIFIER = 'aa57a7c0-0ea9-4d40-9e8c-09bb3d6c4a22';

DECLARE @AsistenciaMaria UNIQUEIDENTIFIER = 'cbe88bf9-1a65-4a3f-8c4c-d2ef7b2e4c10';
DECLARE @AsistenciaDiego UNIQUEIDENTIFIER = 'd9f887a2-9f6c-4b6d-b7e7-02fda8a4ad11';

BEGIN TRAN;

    /* Limpieza previa para ejecutar múltiples veces sin duplicar datos */
    DELETE FROM Asistencias WHERE IdAsistencia IN (@AsistenciaMaria, @AsistenciaDiego);
    DELETE FROM Paquetes WHERE IdPaquete IN (@PaqueteMaria, @PaqueteDiego);
    DELETE FROM Clases WHERE IdClase IN (@ClaseTangoIntermedio, @ClaseTecnicaIndividual);
    DELETE FROM Profesores WHERE IdProfesor = @ProfesorPrincipal;
    DELETE FROM Alumnos WHERE IdAlumno IN (@AlumnoMaria, @AlumnoDiego, @AlumnoCarlos);
    DELETE FROM Usuarios WHERE IdUsuario IN (@UsuarioProfesor, @UsuarioMaria, @UsuarioDiego, @UsuarioCarlos);

    /* Usuarios base */
    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES
        (@UsuarioProfesor, 'Prof. María Gómez', @TipoDocCC, '42567123', 'maria.gomez@demo.chetango.com', '+57 3000000001', @EstadoUsuarioActivo, @Ahora),
        (@UsuarioMaria,    'María Rodríguez',   @TipoDocCC, '42567123-ALU', 'maria.rodriguez@demo.chetango.com', '+57 3000000011', @EstadoUsuarioActivo, @Ahora),
        (@UsuarioDiego,    'Diego Sánchez',     @TipoDocCC, '39567890',      'diego.sanchez@demo.chetango.com', '+57 3000000012', @EstadoUsuarioActivo, @Ahora),
        (@UsuarioCarlos,   'Carlos Martínez',   @TipoDocCC, '40123789',      'carlos.martinez@demo.chetango.com', '+57 3000000013', @EstadoUsuarioActivo, @Ahora);

    /* Profesor principal */
    INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor)
    VALUES (@ProfesorPrincipal, @UsuarioProfesor, @TipoProfesorPrincipal);

    /* Alumnos */
    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
    VALUES
        (@AlumnoMaria,  @UsuarioMaria,  DATEADD(day, -120, @Hoy), @EstadoAlumnoActivo),
        (@AlumnoDiego,  @UsuarioDiego,  DATEADD(day, -200, @Hoy), @EstadoAlumnoActivo),
        (@AlumnoCarlos, @UsuarioCarlos, DATEADD(day, -90,  @Hoy), @EstadoAlumnoActivo);

    /* Clases (dos fechas dentro de la ventana de 7 días) */
    INSERT INTO Clases (IdClase, Fecha, IdTipoClase, HoraInicio, HoraFin, IdProfesorPrincipal)
    VALUES
        (@ClaseTangoIntermedio,  CAST(@ClaseDiaAnterior AS DATETIME2(0)), @TipoClaseRegular, '19:00:00', '20:30:00', @ProfesorPrincipal),
        (@ClaseTecnicaIndividual,CAST(@ClaseDiaActual  AS DATETIME2(0)), @TipoClaseRegular, '20:30:00', '21:30:00', @ProfesorPrincipal);

    /* Paquetes asociados a los alumnos */
    INSERT INTO Paquetes (IdPaquete, IdAlumno, IdPago, ClasesDisponibles, ClasesUsadas, FechaActivacion, FechaVencimiento, IdEstado, IdTipoPaquete, ValorPaquete, FechaCreacion, FechaModificacion, UsuarioCreacion, UsuarioModificacion)
    VALUES
        (@PaqueteMaria, @AlumnoMaria, NULL, 8, 3, DATEADD(day, -30, @Hoy), DATEADD(day, +30, @Hoy), @EstadoPaqueteActivo,  @TipoPaqueteMensual, 350000, @Ahora, NULL, 'seed-script', NULL),
        (@PaqueteDiego, @AlumnoDiego, NULL, 12, 12, DATEADD(day, -60, @Hoy), DATEADD(day, -5,  @Hoy), @EstadoPaqueteAgotado, @TipoPaqueteMensual, 420000, @Ahora, NULL, 'seed-script', NULL);

    /* Asistencias registradas para la clase principal */
    INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, Observacion, FechaRegistro, FechaModificacion, UsuarioCreacion, UsuarioModificacion)
    VALUES
        (@AsistenciaMaria, @ClaseTangoIntermedio, @AlumnoMaria, @PaqueteMaria, @EstadoAsistenciaPresente, NULL, @Ahora, NULL, 'seed-script', NULL),
        (@AsistenciaDiego, @ClaseTangoIntermedio, @AlumnoDiego, @PaqueteDiego, @EstadoAsistenciaAusente, 'Llegó tarde / no ingresó', @Ahora, NULL, 'seed-script', NULL);

COMMIT;

PRINT 'Datos de prueba para admin asistencias insertados correctamente.';

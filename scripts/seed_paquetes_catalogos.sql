/********************************************************************************************************
 Script: seed_paquetes_catalogos.sql
 Objetivo: Poblar catálogos y datos de prueba para el módulo de Paquetes.

 Instrucciones:
   1. Ajusta la sentencia USE para apuntar a tu base (Dev/QA).
   2. Ejecuta el script completo desde SQL Server Management Studio o sqlcmd.
   3. El script es idempotente: elimina los registros con los mismos GUID antes de insertar.
*********************************************************************************************************/

-- USE [ChetangoDB_Dev]; -- << reemplaza por el nombre real de tu base

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Ahora DATETIME2(0) = SYSDATETIME();
DECLARE @Hoy DATE = CONVERT(date, SYSDATETIME());

-- ========================================
-- 1. CATÁLOGOS: TiposPaquete
-- ========================================
DECLARE @TipoPaquete4Clases UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @TipoPaquete8Clases UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @TipoPaquete12Clases UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @TipoPaqueteMensual UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';

BEGIN TRAN;

    -- Limpieza previa
    DELETE FROM TiposPaquete WHERE Id IN (@TipoPaquete4Clases, @TipoPaquete8Clases, @TipoPaquete12Clases, @TipoPaqueteMensual);

    -- Insertar tipos de paquete
    INSERT INTO TiposPaquete (Id, Nombre)
    VALUES
        (@TipoPaquete4Clases, 'Paquete 4 clases'),
        (@TipoPaquete8Clases, 'Paquete 8 clases'),
        (@TipoPaquete12Clases, 'Paquete 12 clases'),
        (@TipoPaqueteMensual, 'Mensual (ilimitado)');

    PRINT 'Catálogos TiposPaquete insertados correctamente.';

COMMIT;

-- ========================================
-- 2. DATOS DE PRUEBA: Paquetes
-- ========================================

-- Estados existentes
DECLARE @EstadoActivo INT = 1;
DECLARE @EstadoVencido INT = 2;
DECLARE @EstadoCongelado INT = 3;

-- Verificar que existen alumnos
DECLARE @CantidadAlumnos INT;
SELECT @CantidadAlumnos = COUNT(*) FROM Alumnos;

IF @CantidadAlumnos = 0
BEGIN
    PRINT 'ERROR: No existen alumnos en la base de datos. Ejecuta primero seed_personas_roles.sql o seed_usuarios_prueba_ciam.sql';
    RETURN;
END

-- Obtener IDs de alumnos existentes
DECLARE @IdAlumno1 UNIQUEIDENTIFIER;
DECLARE @IdAlumno2 UNIQUEIDENTIFIER;
DECLARE @IdAlumno3 UNIQUEIDENTIFIER;

SELECT TOP 1 @IdAlumno1 = IdAlumno FROM Alumnos ORDER BY FechaInscripcion;
SELECT TOP 1 @IdAlumno2 = IdAlumno FROM Alumnos WHERE IdAlumno <> @IdAlumno1 ORDER BY FechaInscripcion;
SELECT TOP 1 @IdAlumno3 = IdAlumno FROM Alumnos WHERE IdAlumno NOT IN (@IdAlumno1, @IdAlumno2) ORDER BY FechaInscripcion;

-- GUIDs fijos para paquetes de prueba
DECLARE @PaqueteTest1 UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @PaqueteTest2 UNIQUEIDENTIFIER = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @PaqueteTest3 UNIQUEIDENTIFIER = 'cccccccc-cccc-cccc-cccc-cccccccccccc';
DECLARE @PaqueteTest4 UNIQUEIDENTIFIER = 'dddddddd-dddd-dddd-dddd-dddddddddddd';
DECLARE @PaqueteTest5 UNIQUEIDENTIFIER = 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee';

BEGIN TRAN;

    -- Limpieza previa
    DELETE FROM CongelacionesPaquete WHERE IdPaquete IN (@PaqueteTest1, @PaqueteTest2, @PaqueteTest3, @PaqueteTest4, @PaqueteTest5);
    DELETE FROM Paquetes WHERE IdPaquete IN (@PaqueteTest1, @PaqueteTest2, @PaqueteTest3, @PaqueteTest4, @PaqueteTest5);

    -- Insertar paquetes de prueba (solo si hay alumnos disponibles)
    IF @IdAlumno1 IS NOT NULL
    BEGIN
        INSERT INTO Paquetes (
            IdPaquete, 
            IdAlumno, 
            IdPago, 
            ClasesDisponibles, 
            ClasesUsadas, 
            FechaActivacion, 
            FechaVencimiento, 
            IdEstado, 
            IdTipoPaquete, 
            ValorPaquete, 
            FechaCreacion, 
            UsuarioCreacion
        )
        VALUES
            -- Paquete activo con clases disponibles
            (
                @PaqueteTest1, 
                @IdAlumno1, 
                NULL, 
                8, 
                2, 
                DATEADD(day, -15, @Hoy), 
                DATEADD(day, +45, @Hoy), 
                @EstadoActivo, 
                @TipoPaquete8Clases, 
                280000, 
                @Ahora, 
                'seed-script'
            ),
            -- Paquete vencido
            (
                @PaqueteTest2, 
                @IdAlumno1, 
                NULL, 
                4, 
                4, 
                DATEADD(day, -90, @Hoy), 
                DATEADD(day, -10, @Hoy), 
                @EstadoVencido, 
                @TipoPaquete4Clases, 
                150000, 
                DATEADD(day, -90, @Ahora), 
                'seed-script'
            );

        PRINT 'Paquetes insertados para Alumno 1.';
    END

    IF @IdAlumno2 IS NOT NULL
    BEGIN
        INSERT INTO Paquetes (
            IdPaquete, 
            IdAlumno, 
            IdPago, 
            ClasesDisponibles, 
            ClasesUsadas, 
            FechaActivacion, 
            FechaVencimiento, 
            IdEstado, 
            IdTipoPaquete, 
            ValorPaquete, 
            FechaCreacion, 
            UsuarioCreacion
        )
        VALUES
            -- Paquete congelado
            (
                @PaqueteTest3, 
                @IdAlumno2, 
                NULL, 
                12, 
                5, 
                DATEADD(day, -30, @Hoy), 
                DATEADD(day, +30, @Hoy), 
                @EstadoCongelado, 
                @TipoPaquete12Clases, 
                420000, 
                @Ahora, 
                'seed-script'
            ),
            -- Paquete activo reciente
            (
                @PaqueteTest4, 
                @IdAlumno2, 
                NULL, 
                8, 
                0, 
                @Hoy, 
                DATEADD(day, +60, @Hoy), 
                @EstadoActivo, 
                @TipoPaquete8Clases, 
                280000, 
                @Ahora, 
                'seed-script'
            );

        -- Agregar congelación al paquete congelado
        INSERT INTO CongelacionesPaquete (
            IdCongelacion,
            IdPaquete,
            FechaInicio,
            FechaFin
        )
        VALUES (
            NEWID(),
            @PaqueteTest3,
            DATEADD(day, -5, @Hoy),
            DATEADD(day, +25, @Hoy)
        );

        PRINT 'Paquetes insertados para Alumno 2 (incluye paquete congelado).';
    END

    IF @IdAlumno3 IS NOT NULL
    BEGIN
        INSERT INTO Paquetes (
            IdPaquete, 
            IdAlumno, 
            IdPago, 
            ClasesDisponibles, 
            ClasesUsadas, 
            FechaActivacion, 
            FechaVencimiento, 
            IdEstado, 
            IdTipoPaquete, 
            ValorPaquete, 
            FechaCreacion, 
            UsuarioCreacion
        )
        VALUES
            -- Paquete mensual ilimitado activo
            (
                @PaqueteTest5, 
                @IdAlumno3, 
                NULL, 
                999, -- Valor alto para simular ilimitado
                15, 
                DATEADD(day, -10, @Hoy), 
                DATEADD(day, +20, @Hoy), 
                @EstadoActivo, 
                @TipoPaqueteMensual, 
                350000, 
                @Ahora, 
                'seed-script'
            );

        PRINT 'Paquete mensual insertado para Alumno 3.';
    END

COMMIT;

PRINT '';
PRINT '========================================';
PRINT 'Seed completado exitosamente.';
PRINT '========================================';
PRINT 'TiposPaquete: 4 registros';
PRINT 'Paquetes de prueba: hasta 5 registros (según alumnos disponibles)';
PRINT '';

-- Verificación final
SELECT 'Tipos de Paquete insertados:' AS Resultado;
SELECT Id, Nombre FROM TiposPaquete ORDER BY Nombre;

SELECT 'Paquetes insertados:' AS Resultado;
SELECT 
    p.IdPaquete,
    u.NombreUsuario AS Alumno,
    tp.Nombre AS TipoPaquete,
    ep.Nombre AS Estado,
    p.ClasesDisponibles,
    p.ClasesUsadas,
    FORMAT(p.FechaVencimiento, 'yyyy-MM-dd') AS FechaVencimiento
FROM Paquetes p
    INNER JOIN Alumnos a ON p.IdAlumno = a.IdAlumno
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
    LEFT JOIN TiposPaquete tp ON p.IdTipoPaquete = tp.Id
    LEFT JOIN EstadosPaquete ep ON p.IdEstado = ep.Id
WHERE p.IdPaquete IN (@PaqueteTest1, @PaqueteTest2, @PaqueteTest3, @PaqueteTest4, @PaqueteTest5)
ORDER BY p.FechaCreacion DESC;

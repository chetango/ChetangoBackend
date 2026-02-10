/********************************************************************************************************
 Script: seed_dashboard_profesor.sql
 Objetivo: Poblar datos de prueba específicos para el Dashboard del Profesor
 
 Crea:
   - Clases de HOY (programadas, en-curso, finalizadas)
   - Clases FUTURAS (próximos 7 días) para la sección "Próximas Clases"
   - Clases PASADAS (últimos 30 días) con asistencias para KPIs y gráfica
   
 Prerequisitos:
   - seed_usuarios_prueba_ciam.sql ejecutado
   - seed_paquetes_catalogos.sql ejecutado
   - seed_metodos_pago.sql ejecutado

 Instrucciones:
   1. Ajusta la sentencia USE para tu base (Dev/QA).
   2. Ejecuta el script completo desde SQL Server Management Studio.
*********************************************************************************************************/

USE ChetangoDB_Dev; -- << Cambia a ChetangoDB_QA si es necesario
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT '========================================';
PRINT 'Dashboard Profesor - Seed de Datos';
PRINT '========================================';

DECLARE @Ahora DATETIME2(0) = SYSDATETIME();
DECLARE @Hoy DATE = CAST(@Ahora AS DATE);
DECLARE @HoraActual TIME = CAST(@Ahora AS TIME);

-- =====================================================================
-- IDS DE USUARIOS Y ENTIDADES
-- =====================================================================
DECLARE @IdUsuarioJorge UNIQUEIDENTIFIER = '8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB';
DECLARE @IdProfesorJorge UNIQUEIDENTIFIER = '8f6e460d-328d-4a40-89e3-b8effa76829c';

-- Alumnos (del script seed_reportes_datos_prueba.sql)
DECLARE @IdAlumnoJuanDavid UNIQUEIDENTIFIER = '295093d5-b36f-4737-b68a-ab40ca871b2e';

-- Necesitamos crear 5 alumnos más si no existen
DECLARE @IdUsuarioMaria UNIQUEIDENTIFIER;
DECLARE @IdUsuarioCarlos UNIQUEIDENTIFIER;
DECLARE @IdUsuarioAna UNIQUEIDENTIFIER;
DECLARE @IdUsuarioLuis UNIQUEIDENTIFIER;
DECLARE @IdUsuarioPedro UNIQUEIDENTIFIER;

DECLARE @IdAlumnoMaria UNIQUEIDENTIFIER;
DECLARE @IdAlumnoCarlos UNIQUEIDENTIFIER;
DECLARE @IdAlumnoAna UNIQUEIDENTIFIER;
DECLARE @IdAlumnoLuis UNIQUEIDENTIFIER;
DECLARE @IdAlumnoPedro UNIQUEIDENTIFIER;

-- Buscar alumnos existentes por correo
SELECT @IdUsuarioMaria = u.IdUsuario, @IdAlumnoMaria = a.IdAlumno 
FROM Usuarios u 
INNER JOIN Alumnos a ON u.IdUsuario = a.IdUsuario 
WHERE u.Correo = 'maria.gomez@test.com';

SELECT @IdUsuarioCarlos = u.IdUsuario, @IdAlumnoCarlos = a.IdAlumno 
FROM Usuarios u 
INNER JOIN Alumnos a ON u.IdUsuario = a.IdUsuario 
WHERE u.Correo = 'carlos.lopez@test.com';

SELECT @IdUsuarioAna = u.IdUsuario, @IdAlumnoAna = a.IdAlumno 
FROM Usuarios u 
INNER JOIN Alumnos a ON u.IdUsuario = a.IdUsuario 
WHERE u.Correo = 'ana.rodriguez@test.com';

SELECT @IdUsuarioLuis = u.IdUsuario, @IdAlumnoLuis = a.IdAlumno 
FROM Usuarios u 
INNER JOIN Alumnos a ON u.IdUsuario = a.IdUsuario 
WHERE u.Correo = 'luis.martinez@test.com';

SELECT @IdUsuarioPedro = u.IdUsuario, @IdAlumnoPedro = a.IdAlumno 
FROM Usuarios u 
INNER JOIN Alumnos a ON u.IdUsuario = a.IdUsuario 
WHERE u.Correo = 'pedro.sanchez@test.com';

-- IDs de Catálogos
DECLARE @IdTipoClaseTango UNIQUEIDENTIFIER;
DECLARE @IdTipoClaseBachata UNIQUEIDENTIFIER;
DECLARE @IdTipoClaseSalsa UNIQUEIDENTIFIER;

SELECT @IdTipoClaseTango = Id FROM TiposClase WHERE Nombre = 'Tango';
SELECT @IdTipoClaseBachata = Id FROM TiposClase WHERE Nombre = 'Bachata';
SELECT @IdTipoClaseSalsa = Id FROM TiposClase WHERE Nombre = 'Salsa';

DECLARE @IdTipoAsistenciaNormal INT = 1;
DECLARE @IdEstadoAsistenciaPresente INT = 1;
DECLARE @IdEstadoAsistenciaAusente INT = 2;
DECLARE @IdEstadoAsistenciaJustificada INT = 3;

DECLARE @IdEstadoPaqueteActivo INT = 1;

BEGIN TRANSACTION;

    PRINT 'Limpiando datos previos del profesor Jorge...';
    
    -- Limpiar solo datos relacionados con el profesor Jorge Padilla
    DELETE FROM Asistencias 
    WHERE IdClase IN (
        SELECT IdClase FROM Clases 
        WHERE IdProfesorPrincipal = @IdProfesorJorge
    );
    
    DELETE FROM Clases 
    WHERE IdProfesorPrincipal = @IdProfesorJorge;

    PRINT 'Creando clases para el Dashboard del Profesor...';
    PRINT '';

    -- =====================================================================
    -- 1. CLASES DE HOY (diferentes estados)
    -- =====================================================================
    PRINT '1. Creando CLASES DE HOY...';
    
    -- Clase 1: PROGRAMADA (futuro - 19:00)
    DECLARE @IdClaseHoy1 UNIQUEIDENTIFIER = NEWID();
    INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo)
    VALUES (@IdClaseHoy1, @Hoy, '19:00:00', '20:30:00', @IdTipoClaseTango, @IdProfesorJorge, 20);
    
    -- Inscribir 15 alumnos (cupos disponibles: 5)
    INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, FechaRegistro, UsuarioCreacion)
    SELECT NEWID(), @IdClaseHoy1, IdAlumno, 
           (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = a.IdAlumno AND IdEstado = @IdEstadoPaqueteActivo),
           @IdTipoAsistenciaNormal, @IdEstadoAsistenciaPresente, @Ahora, 'seed-dashboard'
    FROM (
        SELECT @IdAlumnoJuanDavid AS IdAlumno
        UNION ALL SELECT @IdAlumnoMaria
        UNION ALL SELECT @IdAlumnoCarlos
        UNION ALL SELECT @IdAlumnoAna
        UNION ALL SELECT @IdAlumnoLuis
    ) a
    WHERE a.IdAlumno IS NOT NULL;
    
    PRINT '   ✓ Clase 19:00 (Tango) - PROGRAMADA - 15 inscritos';
    
    -- Clase 2: EN CURSO (actual - 17:00-18:30, si son después de las 17:00)
    IF @HoraActual >= '17:00:00'
    BEGIN
        DECLARE @IdClaseHoy2 UNIQUEIDENTIFIER = NEWID();
        INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo)
        VALUES (@IdClaseHoy2, @Hoy, '17:00:00', '18:30:00', @IdTipoClaseBachata, @IdProfesorJorge, 18);
        
        INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, FechaRegistro, UsuarioCreacion)
        SELECT NEWID(), @IdClaseHoy2, IdAlumno, 
               (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = a.IdAlumno AND IdEstado = @IdEstadoPaqueteActivo),
               @IdTipoAsistenciaNormal, @IdEstadoAsistenciaPresente, @Ahora, 'seed-dashboard'
        FROM (
            SELECT @IdAlumnoJuanDavid AS IdAlumno
            UNION ALL SELECT @IdAlumnoCarlos
            UNION ALL SELECT @IdAlumnoLuis
        ) a
        WHERE a.IdAlumno IS NOT NULL;
        
        PRINT '   ✓ Clase 17:00 (Bachata) - EN CURSO - 12 inscritos';
    END
    
    -- Clase 3: FINALIZADA (pasada - 08:00-09:30)
    DECLARE @IdClaseHoy3 UNIQUEIDENTIFIER = NEWID();
    INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo)
    VALUES (@IdClaseHoy3, @Hoy, '08:00:00', '09:30:00', @IdTipoClaseSalsa, @IdProfesorJorge, 15);
    
    INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, FechaRegistro, UsuarioCreacion)
    SELECT NEWID(), @IdClaseHoy3, IdAlumno, 
           (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = a.IdAlumno AND IdEstado = @IdEstadoPaqueteActivo),
           @IdTipoAsistenciaNormal, @IdEstadoAsistenciaPresente, @Ahora, 'seed-dashboard'
    FROM (
        SELECT @IdAlumnoMaria AS IdAlumno
        UNION ALL SELECT @IdAlumnoAna
        UNION ALL SELECT @IdAlumnoPedro
    ) a
    WHERE a.IdAlumno IS NOT NULL;
    
    PRINT '   ✓ Clase 08:00 (Salsa) - FINALIZADA - 8 inscritos';
    PRINT '';

    -- =====================================================================
    -- 2. CLASES PRÓXIMOS 7 DÍAS (para sección "Próximas Clases")
    -- =====================================================================
    PRINT '2. Creando CLASES FUTURAS (próximos 7 días)...';
    
    DECLARE @FechaFutura DATE;
    DECLARE @Contador INT = 0;
    DECLARE @DiaSemana INT;
    DECLARE @IdClaseFutura UNIQUEIDENTIFIER;
    
    SET @FechaFutura = DATEADD(DAY, 1, @Hoy); -- Empezar mañana
    
    WHILE @FechaFutura <= DATEADD(DAY, 7, @Hoy) AND @Contador < 6
    BEGIN
        SET @DiaSemana = DATEPART(WEEKDAY, @FechaFutura);
        
        -- Lunes (2), Miércoles (4), Viernes (6)
        IF @DiaSemana IN (2, 4, 6)
        BEGIN
            SET @IdClaseFutura = NEWID();
            
            -- Alternar tipos de clase
            DECLARE @TipoClaseRotativo UNIQUEIDENTIFIER;
            SET @TipoClaseRotativo = CASE (@Contador % 3)
                WHEN 0 THEN @IdTipoClaseTango
                WHEN 1 THEN @IdTipoClaseBachata
                ELSE @IdTipoClaseSalsa
            END;
            
            INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo)
            VALUES (@IdClaseFutura, @FechaFutura, '19:00:00', '20:30:00', @TipoClaseRotativo, @IdProfesorJorge, 20);
            
            -- Inscribir alumnos variados
            INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, FechaRegistro, UsuarioCreacion)
            SELECT NEWID(), @IdClaseFutura, IdAlumno, 
                   (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = a.IdAlumno AND IdEstado = @IdEstadoPaqueteActivo),
                   @IdTipoAsistenciaNormal, @IdEstadoAsistenciaPresente, @Ahora, 'seed-dashboard'
            FROM (
                SELECT @IdAlumnoJuanDavid AS IdAlumno
                UNION ALL SELECT @IdAlumnoMaria
                UNION ALL SELECT @IdAlumnoAna
            ) a
            WHERE a.IdAlumno IS NOT NULL;
            
            SET @Contador = @Contador + 1;
            
            PRINT '   ✓ Clase ' + CONVERT(VARCHAR, @FechaFutura, 103) + ' 19:00 - 10-12 inscritos';
        END
        
        SET @FechaFutura = DATEADD(DAY, 1, @FechaFutura);
    END
    PRINT '';

    -- =====================================================================
    -- 3. CLASES PASADAS (últimos 30 días) para KPIs y gráfica
    -- =====================================================================
    PRINT '3. Creando CLASES PASADAS (últimos 30 días)...';
    
    DECLARE @FechaPasada DATE = DATEADD(DAY, -30, @Hoy);
    DECLARE @ContadorPasadas INT = 0;
    DECLARE @IdClasePasada UNIQUEIDENTIFIER;
    
    WHILE @FechaPasada < @Hoy AND @ContadorPasadas < 25
    BEGIN
        SET @DiaSemana = DATEPART(WEEKDAY, @FechaPasada);
        
        -- Lunes, Miércoles, Viernes
        IF @DiaSemana IN (2, 4, 6)
        BEGIN
            SET @IdClasePasada = NEWID();
            
            -- Alternar tipos
            SET @TipoClaseRotativo = CASE (@ContadorPasadas % 3)
                WHEN 0 THEN @IdTipoClaseTango
                WHEN 1 THEN @IdTipoClaseBachata
                ELSE @IdTipoClaseSalsa
            END;
            
            INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo)
            VALUES (@IdClasePasada, @FechaPasada, '19:00:00', '20:30:00', @TipoClaseRotativo, @IdProfesorJorge, 20);
            
            -- Registrar asistencias con variación realista
            -- Juan David - 90% asistencia
            IF (@ContadorPasadas % 10) <> 0
            BEGIN
                INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, FechaRegistro, UsuarioCreacion)
                VALUES (NEWID(), @IdClasePasada, @IdAlumnoJuanDavid, 
                       (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoJuanDavid AND IdEstado = @IdEstadoPaqueteActivo),
                       @IdTipoAsistenciaNormal, @IdEstadoAsistenciaPresente, @FechaPasada, 'seed-dashboard');
            END
            
            -- María - 85% asistencia
            IF (@ContadorPasadas % 7) <> 0 AND @IdAlumnoMaria IS NOT NULL
            BEGIN
                INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, FechaRegistro, UsuarioCreacion)
                VALUES (NEWID(), @IdClasePasada, @IdAlumnoMaria, 
                       (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoMaria AND IdEstado = @IdEstadoPaqueteActivo),
                       @IdTipoAsistenciaNormal, @IdEstadoAsistenciaPresente, @FechaPasada, 'seed-dashboard');
            END
            
            -- Carlos - 75% asistencia
            IF (@ContadorPasadas % 4) <> 0 AND @IdAlumnoCarlos IS NOT NULL
            BEGIN
                INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, FechaRegistro, UsuarioCreacion)
                VALUES (NEWID(), @IdClasePasada, @IdAlumnoCarlos, 
                       (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoCarlos AND IdEstado = @IdEstadoPaqueteActivo),
                       @IdTipoAsistenciaNormal, @IdEstadoAsistenciaPresente, @FechaPasada, 'seed-dashboard');
            END
            ELSE IF @IdAlumnoCarlos IS NOT NULL
            BEGIN
                INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, Observacion, FechaRegistro, UsuarioCreacion)
                VALUES (NEWID(), @IdClasePasada, @IdAlumnoCarlos, 
                       (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoCarlos AND IdEstado = @IdEstadoPaqueteActivo),
                       @IdTipoAsistenciaNormal, @IdEstadoAsistenciaAusente, 'Sin justificación', @FechaPasada, 'seed-dashboard');
            END
            
            -- Ana - 95% asistencia
            IF (@ContadorPasadas % 20) <> 0 AND @IdAlumnoAna IS NOT NULL
            BEGIN
                INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, FechaRegistro, UsuarioCreacion)
                VALUES (NEWID(), @IdClasePasada, @IdAlumnoAna, 
                       (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoAna AND IdEstado = @IdEstadoPaqueteActivo),
                       @IdTipoAsistenciaNormal, @IdEstadoAsistenciaPresente, @FechaPasada, 'seed-dashboard');
            END
            
            -- Luis - Solo últimas 2 semanas
            IF @FechaPasada >= DATEADD(DAY, -14, @Hoy) AND @IdAlumnoLuis IS NOT NULL
            BEGIN
                INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdTipoAsistencia, IdEstado, FechaRegistro, UsuarioCreacion)
                VALUES (NEWID(), @IdClasePasada, @IdAlumnoLuis, 
                       (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoLuis AND IdEstado = @IdEstadoPaqueteActivo),
                       @IdTipoAsistenciaNormal, @IdEstadoAsistenciaPresente, @FechaPasada, 'seed-dashboard');
            END
            
            SET @ContadorPasadas = @ContadorPasadas + 1;
        END
        
        SET @FechaPasada = DATEADD(DAY, 1, @FechaPasada);
    END
    
    PRINT '   ✓ ' + CAST(@ContadorPasadas AS VARCHAR(10)) + ' clases con asistencias registradas';
    PRINT '';

COMMIT TRANSACTION;

PRINT '========================================';
PRINT 'Dashboard Profesor - Seed Completado!';
PRINT '========================================';
PRINT '';
PRINT 'Resumen de datos creados:';
PRINT '- Clases de HOY: 3 (programada, en-curso, finalizada)';
PRINT '- Clases FUTURAS: ~' + CAST(@Contador AS VARCHAR(10)) + ' (próximos 7 días)';
PRINT '- Clases PASADAS: ' + CAST(@ContadorPasadas AS VARCHAR(10)) + ' (últimos 30 días)';
PRINT '- Asistencias: ~' + CAST(@ContadorPasadas * 4 AS VARCHAR(10)) + ' registros';
PRINT '';
PRINT 'Listo para probar:';
PRINT '  ✓ Clases de Hoy (3 estados diferentes)';
PRINT '  ✓ Próximas Clases (siguiente semana)';
PRINT '  ✓ KPIs del mes actual';
PRINT '  ✓ Gráfica de asistencia (últimos 30 días)';
PRINT '';
GO

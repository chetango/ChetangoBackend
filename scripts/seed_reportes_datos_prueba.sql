/********************************************************************************************************
 Script: seed_reportes_datos_prueba.sql
 Objetivo: Poblar datos de prueba para el módulo de Reportes con datos realistas y variados.
 
 Crea:
   - Pagos distribuidos en los últimos 6 meses
   - Clases y asistencias para los últimos 3 meses
   - Paquetes con diferentes estados (Activos, Vencidos, Por vencer, Agotados)
   - Alumnos con diferentes niveles de actividad
   - Variedad de métodos de pago
   
 Prerequisitos:
   - seed_usuarios_prueba_ciam.sql ejecutado (crea usuarios base)
   - seed_metodos_pago.sql ejecutado (crea métodos de pago)
   - seed_paquetes_catalogos.sql ejecutado (crea tipos de paquete)

 Instrucciones:
   1. Ajusta la sentencia USE para tu base (Dev/QA).
   2. Ejecuta el script completo desde SQL Server Management Studio.
   3. El script es idempotente: limpia registros previos antes de insertar.
*********************************************************************************************************/

USE ChetangoDB_Dev; -- << Cambia a ChetangoDB_QA si es necesario
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT '========================================';
PRINT 'Iniciando seed de datos para Reportes';
PRINT '========================================';

DECLARE @Ahora DATETIME2(0) = SYSDATETIME();
DECLARE @Hoy DATE = CAST(@Ahora AS DATE);
DECLARE @FechaBase DATE = DATEADD(MONTH, -6, @Hoy); -- Empezar hace 6 meses

-- =====================================================================
-- IDS DE USUARIOS EXISTENTES (de seed_usuarios_prueba_ciam.sql)
-- =====================================================================
DECLARE @IdUsuarioJorge UNIQUEIDENTIFIER = '8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB';
DECLARE @IdUsuarioJuanDavid UNIQUEIDENTIFIER = '6f84b6a7-b0ae-456b-a455-eabae44d2930';
DECLARE @IdProfesorJorge UNIQUEIDENTIFIER = '8f6e460d-328d-4a40-89e3-b8effa76829c';
DECLARE @IdAlumnoJuanDavid UNIQUEIDENTIFIER = '295093d5-b36f-4737-b68a-ab40ca871b2e';

-- =====================================================================
-- CREAR ALUMNOS ADICIONALES PARA PRUEBAS DE REPORTES
-- =====================================================================
DECLARE @IdUsuarioMaria UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioCarlos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioAna UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioLuis UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioPedro UNIQUEIDENTIFIER = NEWID();

DECLARE @IdAlumnoMaria UNIQUEIDENTIFIER = NEWID();
DECLARE @IdAlumnoCarlos UNIQUEIDENTIFIER = NEWID();
DECLARE @IdAlumnoAna UNIQUEIDENTIFIER = NEWID();
DECLARE @IdAlumnoLuis UNIQUEIDENTIFIER = NEWID();
DECLARE @IdAlumnoPedro UNIQUEIDENTIFIER = NEWID();

-- Obtener IDs de catálogos
DECLARE @IdTipoDocCC UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111'; -- Cédula de Ciudadanía
DECLARE @IdEstadoActivo INT = 1;
DECLARE @IdEstadoInactivo INT = 2;
DECLARE @IdEstadoPaqueteActivo INT = 1;
DECLARE @IdEstadoPaqueteVencido INT = 2;
DECLARE @IdEstadoPaqueteCongelado INT = 3; -- Usaremos Congelado en lugar de Agotado
DECLARE @IdEstadoAsistenciaPresente INT = 1;
DECLARE @IdEstadoAsistenciaAusente INT = 2;
DECLARE @IdEstadoAsistenciaJustificada INT = 3;

-- IDs de tipos
DECLARE @IdTipoClaseTango UNIQUEIDENTIFIER;
DECLARE @IdTipoPaquete4 UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @IdTipoPaquete8 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @IdTipoPaquete12 UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';

SELECT @IdTipoClaseTango = Id FROM TiposClase WHERE Nombre = 'Tango';

-- IDs de métodos de pago
DECLARE @IdMetodoEfectivo UNIQUEIDENTIFIER;
DECLARE @IdMetodoTransferencia UNIQUEIDENTIFIER;
DECLARE @IdMetodoNequi UNIQUEIDENTIFIER;

SELECT @IdMetodoEfectivo = Id FROM MetodosPago WHERE Nombre = 'Efectivo';
SELECT @IdMetodoTransferencia = Id FROM MetodosPago WHERE Nombre = 'Transferencia Bancaria';
SELECT @IdMetodoNequi = Id FROM MetodosPago WHERE Nombre = 'Nequi';

BEGIN TRANSACTION;

    PRINT 'Limpiando datos previos...';
    
    -- Limpiar datos de reportes previos (mantener estructura)
    DELETE FROM Asistencias 
    WHERE IdClase IN (
        SELECT IdClase FROM Clases 
        WHERE IdProfesorPrincipal = @IdProfesorJorge 
        AND Fecha >= @FechaBase
    );
    
    DELETE FROM Clases 
    WHERE IdProfesorPrincipal = @IdProfesorJorge 
    AND Fecha >= @FechaBase;
    
    DELETE FROM Paquetes 
    WHERE IdAlumno IN (@IdAlumnoJuanDavid, @IdAlumnoMaria, @IdAlumnoCarlos, @IdAlumnoAna, @IdAlumnoLuis, @IdAlumnoPedro);
    
    DELETE FROM Pagos 
    WHERE IdAlumno IN (@IdAlumnoJuanDavid, @IdAlumnoMaria, @IdAlumnoCarlos, @IdAlumnoAna, @IdAlumnoLuis, @IdAlumnoPedro);
    
    DELETE FROM Alumnos 
    WHERE IdAlumno IN (@IdAlumnoMaria, @IdAlumnoCarlos, @IdAlumnoAna, @IdAlumnoLuis, @IdAlumnoPedro);
    
    DELETE FROM Usuarios 
    WHERE IdUsuario IN (@IdUsuarioMaria, @IdUsuarioCarlos, @IdUsuarioAna, @IdUsuarioLuis, @IdUsuarioPedro);

    PRINT 'Creando usuarios y alumnos adicionales...';
    
    -- Insertar usuarios adicionales
    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES
        (@IdUsuarioMaria, 'María Rodríguez', @IdTipoDocCC, 'DOC-MARIA-001', 'maria@chetango.com', '+57 300 1111111', @IdEstadoActivo, @Ahora),
        (@IdUsuarioCarlos, 'Carlos Martínez', @IdTipoDocCC, 'DOC-CARLOS-002', 'carlos@chetango.com', '+57 300 2222222', @IdEstadoActivo, @Ahora),
        (@IdUsuarioAna, 'Ana González', @IdTipoDocCC, 'DOC-ANA-003', 'ana@chetango.com', '+57 300 3333333', @IdEstadoActivo, @Ahora),
        (@IdUsuarioLuis, 'Luis Fernández', @IdTipoDocCC, 'DOC-LUIS-004', 'luis@chetango.com', '+57 300 4444444', @IdEstadoActivo, @Ahora),
        (@IdUsuarioPedro, 'Pedro López', @IdTipoDocCC, 'DOC-PEDRO-005', 'pedro@chetango.com', '+57 300 5555555', @IdEstadoActivo, @Ahora);

    -- Insertar alumnos
    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
    VALUES
        (@IdAlumnoMaria, @IdUsuarioMaria, DATEADD(MONTH, -5, @Hoy), @IdEstadoActivo),
        (@IdAlumnoCarlos, @IdUsuarioCarlos, DATEADD(MONTH, -4, @Hoy), @IdEstadoActivo),
        (@IdAlumnoAna, @IdUsuarioAna, DATEADD(MONTH, -3, @Hoy), @IdEstadoActivo),
        (@IdAlumnoLuis, @IdUsuarioLuis, DATEADD(MONTH, -2, @Hoy), @IdEstadoActivo),
        (@IdAlumnoPedro, @IdUsuarioPedro, DATEADD(MONTH, -6, @Hoy), @IdEstadoInactivo); -- Alumno inactivo para alertas

    PRINT 'Creando pagos distribuidos en los últimos 6 meses...';
    
    -- =====================================================================
    -- CREAR PAGOS DISTRIBUIDOS EN 6 MESES
    -- =====================================================================
    DECLARE @ContadorPagos INT = 0;
    DECLARE @FechaPago DATE = @FechaBase;
    DECLARE @IdPago UNIQUEIDENTIFIER;
    DECLARE @MontoPago DECIMAL(18,2);
    DECLARE @IdMetodo UNIQUEIDENTIFIER;
    DECLARE @IdAlumnoActual UNIQUEIDENTIFIER;
    DECLARE @NotaPago NVARCHAR(200);
    
    -- Tabla temporal para distribuir alumnos
    DECLARE @AlumnosTemp TABLE (Id INT IDENTITY(1,1), IdAlumno UNIQUEIDENTIFIER);
    INSERT INTO @AlumnosTemp (IdAlumno) 
    VALUES (@IdAlumnoJuanDavid), (@IdAlumnoMaria), (@IdAlumnoCarlos), (@IdAlumnoAna), (@IdAlumnoLuis);
    
    -- Crear 45 pagos distribuidos (aprox 7-8 por mes)
    WHILE @ContadorPagos < 45
    BEGIN
        SET @IdPago = NEWID();
        
        -- Variar montos según tipo de paquete
        SET @MontoPago = CASE (@ContadorPagos % 3)
            WHEN 0 THEN 80000   -- Paquete 4 clases
            WHEN 1 THEN 140000  -- Paquete 8 clases
            ELSE 200000         -- Paquete 12 clases
        END;
        
        -- Variar métodos de pago
        SET @IdMetodo = CASE (@ContadorPagos % 3)
            WHEN 0 THEN @IdMetodoEfectivo
            WHEN 1 THEN @IdMetodoTransferencia
            ELSE @IdMetodoNequi
        END;
        
        -- Seleccionar alumno rotativo
        SELECT @IdAlumnoActual = IdAlumno 
        FROM @AlumnosTemp 
        WHERE Id = ((@ContadorPagos % 5) + 1);
        
        -- Nota descriptiva
        SET @NotaPago = CASE (@ContadorPagos % 3)
            WHEN 0 THEN 'Paquete 4 clases'
            WHEN 1 THEN 'Paquete 8 clases'
            ELSE 'Paquete 12 clases'
        END;
        
        -- Avanzar fecha (cada 4-5 días)
        SET @FechaPago = DATEADD(DAY, 4 + (@ContadorPagos % 2), @FechaPago);
        IF @FechaPago > @Hoy SET @FechaPago = @Hoy;
        
        INSERT INTO Pagos (IdPago, IdAlumno, FechaPago, MontoTotal, IdMetodoPago, Nota, FechaCreacion, UsuarioCreacion)
        VALUES (@IdPago, @IdAlumnoActual, @FechaPago, @MontoPago, @IdMetodo, @NotaPago, @Ahora, 'seed-script');
        
        SET @ContadorPagos = @ContadorPagos + 1;
    END;
    
    PRINT CONCAT('Creados ', @ContadorPagos, ' pagos');

    PRINT 'Creando paquetes con diferentes estados...';
    
    -- =====================================================================
    -- CREAR PAQUETES CON DIFERENTES ESTADOS
    -- =====================================================================
    
    -- Paquetes ACTIVOS con clases disponibles
    DECLARE @PagoJuan1 UNIQUEIDENTIFIER = (SELECT TOP 1 IdPago FROM Pagos WHERE IdAlumno = @IdAlumnoJuanDavid ORDER BY FechaPago DESC);
    DECLARE @PagoMaria1 UNIQUEIDENTIFIER = (SELECT TOP 1 IdPago FROM Pagos WHERE IdAlumno = @IdAlumnoMaria ORDER BY FechaPago DESC);
    DECLARE @PagoCarlos1 UNIQUEIDENTIFIER = (SELECT TOP 1 IdPago FROM Pagos WHERE IdAlumno = @IdAlumnoCarlos ORDER BY FechaPago DESC);
    DECLARE @PagoAna1 UNIQUEIDENTIFIER = (SELECT TOP 1 IdPago FROM Pagos WHERE IdAlumno = @IdAlumnoAna ORDER BY FechaPago DESC);
    
    INSERT INTO Paquetes (IdPaquete, IdAlumno, IdPago, ClasesDisponibles, ClasesUsadas, FechaActivacion, FechaVencimiento, IdEstado, IdTipoPaquete, ValorPaquete, FechaCreacion, UsuarioCreacion)
    VALUES
        (NEWID(), @IdAlumnoJuanDavid, @PagoJuan1, 8, 3, DATEADD(DAY, -15, @Hoy), DATEADD(DAY, 15, @Hoy), @IdEstadoPaqueteActivo, @IdTipoPaquete8, 140000, @Ahora, 'seed-script'),
        (NEWID(), @IdAlumnoMaria, @PagoMaria1, 12, 5, DATEADD(DAY, -20, @Hoy), DATEADD(DAY, 10, @Hoy), @IdEstadoPaqueteActivo, @IdTipoPaquete12, 200000, @Ahora, 'seed-script'),
        (NEWID(), @IdAlumnoCarlos, @PagoCarlos1, 8, 2, DATEADD(DAY, -10, @Hoy), DATEADD(DAY, 20, @Hoy), @IdEstadoPaqueteActivo, @IdTipoPaquete8, 140000, @Ahora, 'seed-script'),
        (NEWID(), @IdAlumnoAna, @PagoAna1, 4, 1, DATEADD(DAY, -5, @Hoy), DATEADD(DAY, 25, @Hoy), @IdEstadoPaqueteActivo, @IdTipoPaquete4, 80000, @Ahora, 'seed-script');
    
    -- Paquetes POR VENCER (próximos 3-7 días) - PARA ALERTAS
    DECLARE @PagoLuis1 UNIQUEIDENTIFIER = (SELECT TOP 1 IdPago FROM Pagos WHERE IdAlumno = @IdAlumnoLuis ORDER BY FechaPago DESC);
    DECLARE @PagoJuan2 UNIQUEIDENTIFIER = (SELECT IdPago FROM (SELECT IdPago, ROW_NUMBER() OVER (ORDER BY FechaPago DESC) AS rn FROM Pagos WHERE IdAlumno = @IdAlumnoJuanDavid) AS t WHERE rn = 2);
    
    INSERT INTO Paquetes (IdPaquete, IdAlumno, IdPago, ClasesDisponibles, ClasesUsadas, FechaActivacion, FechaVencimiento, IdEstado, IdTipoPaquete, ValorPaquete, FechaCreacion, UsuarioCreacion)
    VALUES
        (NEWID(), @IdAlumnoLuis, @PagoLuis1, 8, 4, DATEADD(DAY, -25, @Hoy), DATEADD(DAY, 3, @Hoy), @IdEstadoPaqueteActivo, @IdTipoPaquete8, 140000, @Ahora, 'seed-script'),
        (NEWID(), @IdAlumnoJuanDavid, @PagoJuan2, 4, 2, DATEADD(DAY, -25, @Hoy), DATEADD(DAY, 5, @Hoy), @IdEstadoPaqueteActivo, @IdTipoPaquete4, 80000, @Ahora, 'seed-script');
    
    -- Paquetes VENCIDOS
    DECLARE @PagoMaria2 UNIQUEIDENTIFIER = (SELECT IdPago FROM (SELECT IdPago, ROW_NUMBER() OVER (ORDER BY FechaPago DESC) AS rn FROM Pagos WHERE IdAlumno = @IdAlumnoMaria) AS t WHERE rn = 2);
    DECLARE @PagoCarlos2 UNIQUEIDENTIFIER = (SELECT IdPago FROM (SELECT IdPago, ROW_NUMBER() OVER (ORDER BY FechaPago DESC) AS rn FROM Pagos WHERE IdAlumno = @IdAlumnoCarlos) AS t WHERE rn = 2);
    
    INSERT INTO Paquetes (IdPaquete, IdAlumno, IdPago, ClasesDisponibles, ClasesUsadas, FechaActivacion, FechaVencimiento, IdEstado, IdTipoPaquete, ValorPaquete, FechaCreacion, UsuarioCreacion)
    VALUES
        (NEWID(), @IdAlumnoMaria, @PagoMaria2, 8, 6, DATEADD(DAY, -60, @Hoy), DATEADD(DAY, -10, @Hoy), @IdEstadoPaqueteVencido, @IdTipoPaquete8, 140000, @Ahora, 'seed-script'),
        (NEWID(), @IdAlumnoCarlos, @PagoCarlos2, 4, 3, DATEADD(DAY, -50, @Hoy), DATEADD(DAY, -20, @Hoy), @IdEstadoPaqueteVencido, @IdTipoPaquete4, 80000, @Ahora, 'seed-script');
    
    -- Paquetes AGOTADOS (usando estado Congelado ya que Agotado no existe)
    DECLARE @PagoAna2 UNIQUEIDENTIFIER = (SELECT IdPago FROM (SELECT IdPago, ROW_NUMBER() OVER (ORDER BY FechaPago DESC) AS rn FROM Pagos WHERE IdAlumno = @IdAlumnoAna) AS t WHERE rn = 2);
    
    INSERT INTO Paquetes (IdPaquete, IdAlumno, IdPago, ClasesDisponibles, ClasesUsadas, FechaActivacion, FechaVencimiento, IdEstado, IdTipoPaquete, ValorPaquete, FechaCreacion, UsuarioCreacion)
    VALUES
        (NEWID(), @IdAlumnoAna, @PagoAna2, 4, 4, DATEADD(DAY, -30, @Hoy), DATEADD(DAY, 30, @Hoy), @IdEstadoPaqueteCongelado, @IdTipoPaquete4, 80000, @Ahora, 'seed-script');
    
    PRINT 'Paquetes creados con estados variados';

    PRINT 'Creando clases de los últimos 3 meses...';
    
    -- =====================================================================
    -- CREAR CLASES Y ASISTENCIAS ÚLTIMOS 3 MESES
    -- =====================================================================
    DECLARE @FechaClase DATE = DATEADD(MONTH, -3, @Hoy);
    DECLARE @ContadorClases INT = 0;
    DECLARE @IdClase UNIQUEIDENTIFIER;
    DECLARE @DiaSemana INT;
    
    -- Crear clases 3 veces por semana (Lunes, Miércoles, Viernes)
    WHILE @FechaClase <= DATEADD(DAY, 7, @Hoy) AND @ContadorClases < 40
    BEGIN
        SET @DiaSemana = DATEPART(WEEKDAY, @FechaClase);
        
        -- Solo Lunes (2), Miércoles (4), Viernes (6)
        IF @DiaSemana IN (2, 4, 6)
        BEGIN
            SET @IdClase = NEWID();
            
            INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo)
            VALUES (@IdClase, @FechaClase, '19:00:00', '20:30:00', @IdTipoClaseTango, @IdProfesorJorge, 25);
            
            -- Crear asistencias para clases pasadas
            IF @FechaClase < @Hoy
            BEGIN
                -- Juan David - Presente (90% del tiempo)
                IF (@ContadorClases % 10) <> 0
                BEGIN
                    INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, FechaRegistro, UsuarioCreacion)
                    VALUES (NEWID(), @IdClase, @IdAlumnoJuanDavid, 
                           (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoJuanDavid AND IdEstado = @IdEstadoPaqueteActivo), 
                           @IdEstadoAsistenciaPresente, @FechaClase, 'seed-script');
                END
                
                -- María - Presente (85% del tiempo)
                IF (@ContadorClases % 7) <> 0
                BEGIN
                    INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, FechaRegistro, UsuarioCreacion)
                    VALUES (NEWID(), @IdClase, @IdAlumnoMaria, 
                           (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoMaria AND IdEstado = @IdEstadoPaqueteActivo), 
                           @IdEstadoAsistenciaPresente, @FechaClase, 'seed-script');
                END
                
                -- Carlos - Presente (70% del tiempo)
                IF (@ContadorClases % 3) <> 0
                BEGIN
                    INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, FechaRegistro, UsuarioCreacion)
                    VALUES (NEWID(), @IdClase, @IdAlumnoCarlos, 
                           (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoCarlos AND IdEstado = @IdEstadoPaqueteActivo), 
                           @IdEstadoAsistenciaPresente, @FechaClase, 'seed-script');
                END
                ELSE
                BEGIN
                    -- Algunas ausencias
                    INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, Observacion, FechaRegistro, UsuarioCreacion)
                    VALUES (NEWID(), @IdClase, @IdAlumnoCarlos, 
                           (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoCarlos AND IdEstado = @IdEstadoPaqueteActivo), 
                           @IdEstadoAsistenciaAusente, 'Sin justificación', @FechaClase, 'seed-script');
                END
                
                -- Ana - Presente (95% del tiempo)
                IF (@ContadorClases % 20) <> 0
                BEGIN
                    INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, FechaRegistro, UsuarioCreacion)
                    VALUES (NEWID(), @IdClase, @IdAlumnoAna, 
                           (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoAna AND IdEstado = @IdEstadoPaqueteActivo), 
                           @IdEstadoAsistenciaPresente, @FechaClase, 'seed-script');
                END
                
                -- Luis - Presente solo últimas 2 semanas (para simular nuevo alumno)
                IF @FechaClase >= DATEADD(DAY, -14, @Hoy)
                BEGIN
                    INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, FechaRegistro, UsuarioCreacion)
                    VALUES (NEWID(), @IdClase, @IdAlumnoLuis, 
                           (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoLuis AND IdEstado = @IdEstadoPaqueteActivo), 
                           @IdEstadoAsistenciaPresente, @FechaClase, 'seed-script');
                END
            END
            
            SET @ContadorClases = @ContadorClases + 1;
        END
        
        SET @FechaClase = DATEADD(DAY, 1, @FechaClase);
    END;
    
    PRINT 'Creadas ' + CAST(@ContadorClases AS VARCHAR(10)) + ' clases con asistencias';
    
    -- Crear clases FUTURAS para próximos 7 días (para KPI ClasesProximos7Dias)
    DECLARE @FechaFutura DATE = DATEADD(DAY, 1, @Hoy);
    DECLARE @ContadorFuturas INT = 0;
    
    WHILE @FechaFutura <= DATEADD(DAY, 7, @Hoy) AND @ContadorFuturas < 5
    BEGIN
        SET @DiaSemana = DATEPART(WEEKDAY, @FechaFutura);
        
        IF @DiaSemana IN (2, 4, 6) -- Lunes, Miércoles, Viernes
        BEGIN
            SET @IdClase = NEWID();
            
            INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo)
            VALUES (@IdClase, @FechaFutura, '19:00:00', '20:30:00', @IdTipoClaseTango, @IdProfesorJorge, 25);
            
            -- Inscribir solo 3 alumnos para generar alerta de "ClasePocosCupos"
            INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, FechaRegistro, UsuarioCreacion)
            VALUES 
                (NEWID(), @IdClase, @IdAlumnoJuanDavid, (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoJuanDavid AND IdEstado = @IdEstadoPaqueteActivo), @IdEstadoAsistenciaPresente, @Ahora, 'seed-script'),
                (NEWID(), @IdClase, @IdAlumnoMaria, (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoMaria AND IdEstado = @IdEstadoPaqueteActivo), @IdEstadoAsistenciaPresente, @Ahora, 'seed-script'),
                (NEWID(), @IdClase, @IdAlumnoAna, (SELECT TOP 1 IdPaquete FROM Paquetes WHERE IdAlumno = @IdAlumnoAna AND IdEstado = @IdEstadoPaqueteActivo), @IdEstadoAsistenciaPresente, @Ahora, 'seed-script');
            
            SET @ContadorFuturas = @ContadorFuturas + 1;
        END
        
        SET @FechaFutura = DATEADD(DAY, 1, @FechaFutura);
    END;
    
    PRINT 'Creadas ' + CAST(@ContadorFuturas AS VARCHAR(10)) + ' clases futuras con pocos inscritos (para alertas)';

COMMIT TRANSACTION;

PRINT '========================================';
PRINT 'Seed de datos de Reportes completado!';
PRINT '========================================';
PRINT '';
PRINT 'Resumen de datos creados:';
PRINT '- Usuarios adicionales: 5';
PRINT '- Alumnos adicionales: 5';
PRINT '- Pagos: 45 (distribuidos en 6 meses)';
PRINT '- Paquetes: ~12 (Activos, Vencidos, Por vencer, Congelados)';
PRINT '- Clases: ~' + CAST(@ContadorClases + @ContadorFuturas AS VARCHAR(10)) + ' (3 meses pasados + próximos 7 días)';
PRINT '- Asistencias: ~' + CAST(@ContadorClases * 4 AS VARCHAR(10)) + ' (con diferentes tasas de asistencia)';
PRINT '';
PRINT 'Datos listos para pruebas de:';
PRINT '  ✓ Dashboard con KPIs y gráficas';
PRINT '  ✓ Reportes de Asistencias, Ingresos, Paquetes, Clases, Alumnos';
PRINT '  ✓ Alertas (Paquetes por vencer, Alumnos inactivos, Clases con pocos cupos)';
PRINT '  ✓ Exportaciones a Excel, PDF, CSV';
PRINT '  ✓ Mi Reporte (alumno) y Mis Clases (profesor)';
PRINT '';
GO

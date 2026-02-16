/********************************************************************************************************
 Script: 03_transacciones_financieras.sql
 Objetivo: Crear paquetes y pagos para los 50 alumnos
           Distribución: ~100 paquetes activos + históricos, 95 pagos registrados
 Fecha: Febrero 2025
 Uso: Marketing video - Dashboard ingresos, gráficos de ventas
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT 'Iniciando población de transacciones financieras...';

DECLARE @TipoPaqueteBasico UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @TipoPaquetePremium UNIQUEIDENTIFIER = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @MetodoEfectivo UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @MetodoTransferencia UNIQUEIDENTIFIER = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @MetodoTarjeta UNIQUEIDENTIFIER = 'cccccccc-cccc-cccc-cccc-cccccccccccc';
DECLARE @MetodoNequi UNIQUEIDENTIFIER = 'dddddddd-dddd-dddd-dddd-dddddddddddd';

BEGIN TRANSACTION;

-- ============================================
-- LIMPIAR TRANSACCIONES DE MARKETING
-- ============================================
PRINT 'Limpiando transacciones financieras previas...';

DELETE FROM Pagos 
WHERE IdAlumno IN (
    SELECT a.IdAlumno FROM Alumnos a
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
    WHERE u.Correo LIKE '%@marketing.chetango.com'
);

DELETE FROM Paquetes 
WHERE IdAlumno IN (
    SELECT a.IdAlumno FROM Alumnos a
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
    WHERE u.Correo LIKE '%@marketing.chetango.com'
);

PRINT 'Limpieza completada.';

-- ============================================
-- GENERAR PAQUETES Y PAGOS
-- ============================================
PRINT 'Creando paquetes y pagos para 50 alumnos...';

DECLARE @AlumnosMarketing TABLE (IdAlumno UNIQUEIDENTIFIER, NumAlumno INT, FechaInscripcion DATETIME2);

INSERT INTO @AlumnosMarketing (IdAlumno, NumAlumno, FechaInscripcion)
SELECT a.IdAlumno, 
       CAST(REPLACE(u.NumeroDocumento, '20000', '') AS INT) as NumAlumno,
       a.FechaInscripcion
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com'
ORDER BY a.FechaInscripcion;

DECLARE @IdAlumno UNIQUEIDENTIFIER;
DECLARE @NumAlumno INT;
DECLARE @FechaInscripcion DATETIME2;
DECLARE @FechaPaquete DATETIME2;
DECLARE @FechaPago DATETIME2;
DECLARE @IdPaquete UNIQUEIDENTIFIER;
DECLARE @IdPago UNIQUEIDENTIFIER;
DECLARE @TipoPaquete UNIQUEIDENTIFIER;
DECLARE @Monto DECIMAL(18,2);
DECLARE @ClasesDisponibles INT;
DECLARE @MetodoPago UNIQUEIDENTIFIER;
DECLARE @PaqueteNum INT;

DECLARE cursorAlumnos CURSOR FAST_FORWARD FOR
SELECT IdAlumno, NumAlumno, FechaInscripcion FROM @AlumnosMarketing;

OPEN cursorAlumnos;
FETCH NEXT FROM cursorAlumnos INTO @IdAlumno, @NumAlumno, @FechaInscripcion;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Cada alumno tiene 1-3 paquetes dependiendo de antigüedad
    SET @PaqueteNum = 1;
    
    -- PAQUETE 1: Inicial (todos los alumnos)
    SET @IdPaquete = NEWID();
    SET @IdPago = NEWID();
    SET @FechaPaquete = @FechaInscripcion;
    
    -- 60% Básico (12 clases), 40% Premium (20 clases)
    IF (@NumAlumno % 5) < 3
    BEGIN
        SET @TipoPaquete = @TipoPaqueteBasico;
        SET @ClasesDisponibles = 12;
        SET @Monto = 180000 + ((@NumAlumno * 1234) % 40000); -- 180k-220k
    END
    ELSE
    BEGIN
        SET @TipoPaquete = @TipoPaquetePremium;
        SET @ClasesDisponibles = 20;
        SET @Monto = 300000 + ((@NumAlumno * 5678) % 80000); -- 300k-380k
    END;
    
    -- Método de pago variado
    SET @MetodoPago = CASE (@NumAlumno % 4)
        WHEN 0 THEN @MetodoEfectivo
        WHEN 1 THEN @MetodoTransferencia
        WHEN 2 THEN @MetodoTarjeta
        ELSE @MetodoNequi
    END;
    
    SET @FechaPago = DATEADD(HOUR, -2, @FechaPaquete);
    
    INSERT INTO Pagos (IdPago, IdAlumno, Monto, FechaPago, IdMetodoPago)
    VALUES (@IdPago, @IdAlumno, @Monto, @FechaPago, @MetodoPago);
    
    INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, FechaCompra, Estado, IdPago)
    VALUES (@IdPaquete, @IdAlumno, @TipoPaquete, @ClasesDisponibles, 
            CASE WHEN DATEDIFF(MONTH, @FechaPaquete, GETDATE()) > 3 THEN @ClasesDisponibles ELSE (@ClasesDisponibles * 60) / 100 END,
            @FechaPaquete, 1, @IdPago);
    
    -- PAQUETE 2: Renovación (alumnos con más de 2 meses)
    IF DATEDIFF(MONTH, @FechaInscripcion, GETDATE()) >= 2
    BEGIN
        SET @IdPaquete = NEWID();
        SET @IdPago = NEWID();
        SET @FechaPaquete = DATEADD(DAY, 45 + ((@NumAlumno * 7) % 15), @FechaInscripcion);
        
        -- Algunos suben a Premium
        IF @TipoPaquete = @TipoPaqueteBasico AND (@NumAlumno % 3) = 0
        BEGIN
            SET @TipoPaquete = @TipoPaquetePremium;
            SET @ClasesDisponibles = 20;
            SET @Monto = 320000 + ((@NumAlumno * 3456) % 60000);
        END
        ELSE IF @TipoPaquete = @TipoPaqueteBasico
        BEGIN
            SET @ClasesDisponibles = 12;
            SET @Monto = 185000 + ((@NumAlumno * 2345) % 35000);
        END
        ELSE
        BEGIN
            SET @ClasesDisponibles = 20;
            SET @Monto = 310000 + ((@NumAlumno * 4567) % 70000);
        END;
        
        SET @MetodoPago = CASE ((@NumAlumno + 1) % 4)
            WHEN 0 THEN @MetodoEfectivo
            WHEN 1 THEN @MetodoTransferencia
            WHEN 2 THEN @MetodoTarjeta
            ELSE @MetodoNequi
        END;
        
        SET @FechaPago = DATEADD(HOUR, -3, @FechaPaquete);
        
        INSERT INTO Pagos (IdPago, IdAlumno, Monto, FechaPago, IdMetodoPago)
        VALUES (@IdPago, @IdAlumno, @Monto, @FechaPago, @MetodoPago);
        
        INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, FechaCompra, Estado, IdPago)
        VALUES (@IdPaquete, @IdAlumno, @TipoPaquete, @ClasesDisponibles,
                CASE WHEN DATEDIFF(DAY, @FechaPaquete, GETDATE()) > 60 THEN @ClasesDisponibles ELSE (@ClasesDisponibles * 50) / 100 END,
                @FechaPaquete, 1, @IdPago);
    END;
    
    -- PAQUETE 3: Segunda renovación (alumnos antiguos - más de 4 meses)
    IF DATEDIFF(MONTH, @FechaInscripcion, GETDATE()) >= 4 AND (@NumAlumno % 2) = 0
    BEGIN
        SET @IdPaquete = NEWID();
        SET @IdPago = NEWID();
        SET @FechaPaquete = DATEADD(DAY, 100 + ((@NumAlumno * 11) % 20), @FechaInscripcion);
        
        -- Mayoría se quedan en su nivel
        IF @TipoPaquete = @TipoPaqueteBasico
        BEGIN
            SET @ClasesDisponibles = 12;
            SET @Monto = 190000 + ((@NumAlumno * 6789) % 30000);
        END
        ELSE
        BEGIN
            SET @ClasesDisponibles = 20;
            SET @Monto = 325000 + ((@NumAlumno * 8901) % 55000);
        END;
        
        SET @MetodoPago = CASE ((@NumAlumno + 2) % 4)
            WHEN 0 THEN @MetodoEfectivo
            WHEN 1 THEN @MetodoTransferencia
            WHEN 2 THEN @MetodoTarjeta
            ELSE @MetodoNequi
        END;
        
        SET @FechaPago = DATEADD(HOUR, -1, @FechaPaquete);
        
        INSERT INTO Pagos (IdPago, IdAlumno, Monto, FechaPago, IdMetodoPago)
        VALUES (@IdPago, @IdAlumno, @Monto, @FechaPago, @MetodoPago);
        
        INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, FechaCompra, Estado, IdPago)
        VALUES (@IdPaquete, @IdAlumno, @TipoPaquete, @ClasesDisponibles,
                (@ClasesDisponibles * 30) / 100, -- Paquete activo, aún en uso
                @FechaPaquete, 1, @IdPago);
    END;

    FETCH NEXT FROM cursorAlumnos INTO @IdAlumno, @NumAlumno, @FechaInscripcion;
END;

CLOSE cursorAlumnos;
DEALLOCATE cursorAlumnos;

-- ============================================
-- ESTADÍSTICAS
-- ============================================
DECLARE @TotalPagos INT;
DECLARE @TotalPaquetes INT;
DECLARE @MontoTotal DECIMAL(18,2);

SELECT @TotalPagos = COUNT(*) FROM Pagos WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing);
SELECT @TotalPaquetes = COUNT(*) FROM Paquetes WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing);
SELECT @MontoTotal = SUM(Monto) FROM Pagos WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing);

PRINT 'Transacciones creadas:';
PRINT '  - Pagos registrados: ' + CAST(@TotalPagos AS VARCHAR);
PRINT '  - Paquetes creados: ' + CAST(@TotalPaquetes AS VARCHAR);
PRINT '  - Monto total: $' + CAST(@MontoTotal AS VARCHAR);

COMMIT TRANSACTION;

PRINT 'Transacciones financieras pobladas exitosamente.';
PRINT '========================================';
GO

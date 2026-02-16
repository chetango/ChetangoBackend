/********************************************************************************************************
 Script: 06_liquidaciones_mensuales.sql
 Objetivo: Crear liquidaciones de pago para profesores (últimos 6 meses)
           Incluye estado Pagada y Pendiente según antigüedad
 Fecha: Febrero 2025
 Uso: Marketing video - Nómina, historial pagos, egresos dashboard
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT 'Iniciando generación de liquidaciones mensuales...';

BEGIN TRANSACTION;

-- ============================================
-- LIMPIAR LIQUIDACIONES DE MARKETING
-- ============================================
PRINT 'Limpiando liquidaciones previas...';

DELETE FROM LiquidacionesMensuales WHERE IdProfesor IN (
    SELECT p.IdProfesor FROM Profesores p
    INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
    WHERE u.Correo LIKE '%@marketing.chetango.com'
);

PRINT 'Limpieza completada.';

-- ============================================
-- CONFIGURACIÓN DE TARIFAS
-- ============================================
DECLARE @TarifaHoraBase DECIMAL(18,2) = 25000; -- $25k por hora
DECLARE @BonoClaseExtra DECIMAL(18,2) = 10000; -- Bono por clase extra

-- ============================================
-- GENERAR LIQUIDACIONES (Ago 2024 - Ene 2025)
-- ============================================
PRINT 'Generando liquidaciones para 5 profesores (últimos 6 meses)...';

DECLARE @Profesores TABLE (IdProfesor UNIQUEIDENTIFIER, NumProf INT);

INSERT INTO @Profesores (IdProfesor, NumProf)
SELECT p.IdProfesor, ROW_NUMBER() OVER (ORDER BY u.FechaCreacion)
FROM Profesores p
INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com';

DECLARE @IdProfesor UNIQUEIDENTIFIER;
DECLARE @NumProf INT;
DECLARE @MesInicio DATE;
DECLARE @MesFin DATE;
DECLARE @PeriodoActual DATE = '2024-08-01';
DECLARE @PeriodoFinal DATE = '2025-01-01';

DECLARE cursorProfesores CURSOR FAST_FORWARD FOR
SELECT IdProfesor, NumProf FROM @Profesores;

OPEN cursorProfesores;
FETCH NEXT FROM cursorProfesores INTO @IdProfesor, @NumProf;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @PeriodoActual = '2024-08-01';
    
    WHILE @PeriodoActual <= @PeriodoFinal
    BEGIN
        SET @MesInicio = @PeriodoActual;
        SET @MesFin = EOMONTH(@PeriodoActual);
        
        -- Contar clases del profesor en ese mes
        DECLARE @TotalClases INT;
        DECLARE @TotalHoras DECIMAL(18,2);
        
        SELECT @TotalClases = COUNT(DISTINCT cp.IdClase),
               @TotalHoras = SUM(cp.HorasTrabajadas)
        FROM ClaseProfesor cp
        INNER JOIN Clases c ON cp.IdClase = c.IdClase
        WHERE cp.IdProfesor = @IdProfesor
          AND c.Fecha >= @MesInicio
          AND c.Fecha <= @MesFin
          AND c.Descripcion LIKE '%[MKT]%';
        
        -- Solo crear liquidación si tuvo clases
        IF @TotalClases > 0
        BEGIN
            DECLARE @IdLiquidacion UNIQUEIDENTIFIER = NEWID();
            DECLARE @PagoBase DECIMAL(18,2) = @TotalHoras * @TarifaHoraBase;
            DECLARE @PagosAdicionales DECIMAL(18,2) = 0;
            DECLARE @TotalPagar DECIMAL(18,2);
            DECLARE @Estado VARCHAR(20);
            DECLARE @FechaPago DATETIME2;
            
            -- Bonos por clases extras (más de 20 clases/mes)
            IF @TotalClases > 20
                SET @PagosAdicionales = (@TotalClases - 20) * @BonoClaseExtra;
            
            SET @TotalPagar = @PagoBase + @PagosAdicionales;
            
            -- Estado: Pagada si es de hace más de 1 mes, Pendiente si es reciente
            IF @PeriodoActual < DATEADD(MONTH, -1, GETDATE())
            BEGIN
                SET @Estado = 'Pagada';
                SET @FechaPago = DATEADD(DAY, 5, EOMONTH(@PeriodoActual)); -- Pago el 5 del mes siguiente
            END
            ELSE
            BEGIN
                SET @Estado = 'Pendiente';
                SET @FechaPago = NULL;
            END;
            
            INSERT INTO LiquidacionesMensuales (
                IdLiquidacion, IdProfesor, Mes, Anio, 
                TotalClases, TotalHoras, PagoBase, PagosAdicionales, TotalPagar,
                Estado, FechaPago, FechaCreacion
            )
            VALUES (
                @IdLiquidacion, @IdProfesor, MONTH(@PeriodoActual), YEAR(@PeriodoActual),
                @TotalClases, @TotalHoras, @PagoBase, @PagosAdicionales, @TotalPagar,
                @Estado, @FechaPago, DATEADD(DAY, 1, EOMONTH(@PeriodoActual))
            );
            
            -- Actualizar estado de pago en ClaseProfesor
            IF @Estado = 'Pagada'
            BEGIN
                UPDATE ClaseProfesor
                SET EstadoPago = 'Pagada', IdLiquidacion = @IdLiquidacion
                WHERE IdProfesor = @IdProfesor
                  AND IdClase IN (
                      SELECT IdClase FROM Clases 
                      WHERE Fecha >= @MesInicio AND Fecha <= @MesFin
                        AND Descripcion LIKE '%[MKT]%'
                  );
            END;
        END;
        
        SET @PeriodoActual = DATEADD(MONTH, 1, @PeriodoActual);
    END;
    
    FETCH NEXT FROM cursorProfesores INTO @IdProfesor, @NumProf;
END;

CLOSE cursorProfesores;
DEALLOCATE cursorProfesores;

-- ============================================
-- ESTADÍSTICAS
-- ============================================
DECLARE @TotalLiquidaciones INT;
DECLARE @LiquidacionesPagadas INT;
DECLARE @LiquidacionesPendientes INT;
DECLARE @MontoTotalPagado DECIMAL(18,2);
DECLARE @MontoTotalPendiente DECIMAL(18,2);

SELECT @TotalLiquidaciones = COUNT(*),
       @LiquidacionesPagadas = SUM(CASE WHEN Estado = 'Pagada' THEN 1 ELSE 0 END),
       @LiquidacionesPendientes = SUM(CASE WHEN Estado = 'Pendiente' THEN 1 ELSE 0 END)
FROM LiquidacionesMensuales
WHERE IdProfesor IN (SELECT IdProfesor FROM @Profesores);

SELECT @MontoTotalPagado = ISNULL(SUM(TotalPagar), 0)
FROM LiquidacionesMensuales
WHERE IdProfesor IN (SELECT IdProfesor FROM @Profesores)
  AND Estado = 'Pagada';

SELECT @MontoTotalPendiente = ISNULL(SUM(TotalPagar), 0)
FROM LiquidacionesMensuales
WHERE IdProfesor IN (SELECT IdProfesor FROM @Profesores)
  AND Estado = 'Pendiente';

PRINT 'Liquidaciones generadas:';
PRINT '  - Total liquidaciones: ' + CAST(@TotalLiquidaciones AS VARCHAR);
PRINT '  - Pagadas: ' + CAST(@LiquidacionesPagadas AS VARCHAR) + ' ($' + CAST(@MontoTotalPagado AS VARCHAR) + ')';
PRINT '  - Pendientes: ' + CAST(@LiquidacionesPendientes AS VARCHAR) + ' ($' + CAST(@MontoTotalPendiente AS VARCHAR) + ')';

COMMIT TRANSACTION;

PRINT 'Liquidaciones mensuales generadas exitosamente.';
PRINT '========================================';
GO

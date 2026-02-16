USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT '========================================';
PRINT 'Actualizando Liquidaciones a Febrero 2026';
PRINT '========================================';

-- Obtener 15 liquidaciones aleatorias para actualizar
DECLARE @LiqsActualizar TABLE (IdLiquidacion UNIQUEIDENTIFIER);

INSERT INTO @LiqsActualizar
SELECT TOP 15 IdLiquidacion
FROM LiquidacionesMensuales
WHERE Observaciones LIKE '%[MKT]%'
ORDER BY NEWID();

-- Actualizar a Febrero 2026 con estado Pagada
UPDATE lm
SET 
    Mes = 2,
    Año = 2026,
    Estado = 'Pagada',
    FechaPago = CASE (ABS(CHECKSUM(lm.IdLiquidacion)) % 5)
        WHEN 0 THEN '2026-02-10'
        WHEN 1 THEN '2026-02-11'
        WHEN 2 THEN '2026-02-12'
        WHEN 3 THEN '2026-02-13'
        ELSE '2026-02-14'
    END,
    Observaciones = '[MKT] Liquidación Febrero 2026'
FROM LiquidacionesMensuales lm
INNER JOIN @LiqsActualizar la ON lm.IdLiquidacion = la.IdLiquidacion;

DECLARE @Actualizadas INT = @@ROWCOUNT;
PRINT '✓ ' + CAST(@Actualizadas AS VARCHAR(10)) + ' liquidaciones actualizadas';

PRINT '';
PRINT '========================================';
PRINT 'RESUMEN EGRESOS FEBRERO 2026';
PRINT '========================================';

-- Verificar egresos
SELECT 
    'Liquidaciones Febrero' AS Concepto,
    COUNT(*) AS Cantidad,
    '$' + CAST(CAST(SUM(TotalPagar) AS INT) AS VARCHAR(20)) AS Monto
FROM LiquidacionesMensuales
WHERE Mes = 2 
  AND Año = 2026 
  AND Estado = 'Pagada'
  AND Observaciones LIKE '%[MKT]%';

-- Ver distribución por profesor
PRINT '';
PRINT 'Distribución por Profesor:';
SELECT 
    p.Nombre + ' ' + p.Apellido AS Profesor,
    lm.TotalClases AS Clases,
    '$' + CAST(CAST(lm.TotalPagar AS INT) AS VARCHAR(20)) AS Pago
FROM LiquidacionesMensuales lm
INNER JOIN Profesores p ON p.IdProfesor = lm.IdProfesor
WHERE lm.Mes = 2 
  AND lm.Año = 2026
  AND lm.Observaciones LIKE '%[MKT]%'
ORDER BY lm.TotalPagar DESC;

GO

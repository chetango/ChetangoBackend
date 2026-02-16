USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT '========================================';
PRINT 'Actualizando fechas a rango reciente';
PRINT '(7 días atrás y 7 días adelante)';
PRINT '========================================';

-- 1. CLASES PARA LOS PRÓXIMOS 7 DÍAS (16-20 febrero 2026, Lun-Vie)
PRINT '';
PRINT 'Actualizando clases para próximos 7 días...';

-- Lunes 16 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-16',
    HoraInicio = '18:00',
    HoraFin = '19:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-14';

-- Martes 17 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-17',
    HoraInicio = '20:00',
    HoraFin = '21:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-14'
  AND Fecha != '2026-02-16';

-- Miércoles 18 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-18',
    HoraInicio = '18:00',
    HoraFin = '19:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-14'
  AND Fecha NOT IN ('2026-02-16', '2026-02-17');

-- Jueves 19 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-19',
    HoraInicio = '20:00',
    HoraFin = '21:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-14'
  AND Fecha NOT IN ('2026-02-16', '2026-02-17', '2026-02-18');

-- Viernes 20 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-20',
    HoraInicio = '18:00',
    HoraFin = '19:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-14'
  AND Fecha NOT IN ('2026-02-16', '2026-02-17', '2026-02-18', '2026-02-19');

PRINT '✓ 25 clases actualizadas para próximos días';

-- 2. CLASES PARA LOS ÚLTIMOS 7 DÍAS (7, 10, 11, 12, 13 febrero)
PRINT '';
PRINT 'Actualizando clases para últimos 7 días...';

-- Viernes 7 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-07',
    HoraInicio = '18:00',
    HoraFin = '19:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-07';

-- Lunes 10 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-10',
    HoraInicio = '18:00',
    HoraFin = '19:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-07';

-- Martes 11 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-11',
    HoraInicio = '20:00',
    HoraFin = '21:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-07';

-- Miércoles 12 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-12',
    HoraInicio = '18:00',
    HoraFin = '19:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-07';

-- Jueves 13 febrero
UPDATE TOP (5) Clases
SET Fecha = '2026-02-13',
    HoraInicio = '20:00',
    HoraFin = '21:30'
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < '2026-02-07';

PRINT '✓ 25 clases actualizadas para últimos 7 días';

-- 3. ACTUALIZAR ASISTENCIAS PARA COINCIDIR CON LAS CLASES RECIENTES
PRINT '';
PRINT 'Actualizando asistencias...';

UPDATE a
SET a.FechaRegistro = c.Fecha
FROM Asistencias a
INNER JOIN Clases c ON a.IdClase = c.IdClase
WHERE c.Fecha BETWEEN '2026-02-07' AND '2026-02-13'
  AND c.Observaciones LIKE '%[MKT]%';

DECLARE @AsistenciasActualizadas INT = @@ROWCOUNT;
PRINT '✓ ' + CAST(@AsistenciasActualizadas AS VARCHAR(10)) + ' asistencias actualizadas';

-- 4. ACTUALIZAR 10 PAGOS PARA LOS ÚLTIMOS 7 DÍAS
PRINT '';
PRINT 'Actualizando pagos recientes...';

-- Crear pagos distribuidos en los últimos 7 días
WITH PagosACambiar AS (
    SELECT TOP 10 
        IdPago,
        ROW_NUMBER() OVER (ORDER BY FechaPago) AS RowNum
    FROM Pagos
    WHERE IdAlumno IN (
        SELECT IdAlumno 
        FROM Alumnos 
        WHERE IdUsuario IN (
            SELECT IdUsuario 
            FROM Usuarios 
            WHERE Correo LIKE '%@marketing.chetango.com'
        )
    )
    ORDER BY FechaPago
)
UPDATE p
SET p.FechaPago = CASE (pac.RowNum % 7)
    WHEN 0 THEN '2026-02-14'  -- Hoy
    WHEN 1 THEN '2026-02-13'
    WHEN 2 THEN '2026-02-12'
    WHEN 3 THEN '2026-02-11'
    WHEN 4 THEN '2026-02-10'
    WHEN 5 THEN '2026-02-09'
    ELSE '2026-02-08'
END
FROM Pagos p
INNER JOIN PagosACambiar pac ON p.IdPago = pac.IdPago;

PRINT '✓ 10 pagos actualizados';

-- 5. ACTUALIZAR ALGUNOS PAQUETES CON FECHAS RECIENTES
PRINT '';
PRINT 'Actualizando paquetes con fechas recientes...';

WITH PaquetesACambiar AS (
    SELECT TOP 10 
        IdPaquete,
        ROW_NUMBER() OVER (ORDER BY FechaActivacion) AS RowNum
    FROM Paquetes
    WHERE IdAlumno IN (
        SELECT IdAlumno 
        FROM Alumnos 
        WHERE IdUsuario IN (
            SELECT IdUsuario 
            FROM Usuarios 
            WHERE Correo LIKE '%@marketing.chetango.com'
        )
    )
    ORDER BY FechaActivacion
)
UPDATE p
SET p.FechaActivacion = CASE (pac.RowNum % 7)
    WHEN 0 THEN '2026-02-14'
    WHEN 1 THEN '2026-02-13'
    WHEN 2 THEN '2026-02-12'
    WHEN 3 THEN '2026-02-11'
    WHEN 4 THEN '2026-02-10'
    WHEN 5 THEN '2026-02-09'
    ELSE '2026-02-08'
END,
p.FechaVencimiento = DATEADD(DAY, 30, CASE (pac.RowNum % 7)
    WHEN 0 THEN '2026-02-14'
    WHEN 1 THEN '2026-02-13'
    WHEN 2 THEN '2026-02-12'
    WHEN 3 THEN '2026-02-11'
    WHEN 4 THEN '2026-02-10'
    WHEN 5 THEN '2026-02-09'
    ELSE '2026-02-08'
END)
FROM Paquetes p
INNER JOIN PaquetesACambiar pac ON p.IdPaquete = pac.IdPaquete;

PRINT '✓ 10 paquetes actualizados';

-- 6. RESUMEN FINAL
PRINT '';
PRINT '========================================';
PRINT 'RESUMEN ACTUALIZACIÓN';
PRINT '========================================';

SELECT 
    'Clases Próximos 7 Días' AS Metrica,
    COUNT(*) AS Cantidad
FROM Clases
WHERE Fecha BETWEEN '2026-02-14' AND '2026-02-21'
  AND Observaciones LIKE '%[MKT]%'
UNION ALL
SELECT 
    'Clases HOY (14 feb)',
    COUNT(*)
FROM Clases
WHERE Fecha = '2026-02-14'
  AND Observaciones LIKE '%[MKT]%'
UNION ALL
SELECT 
    'Clases Últimos 7 Días',
    COUNT(*)
FROM Clases
WHERE Fecha BETWEEN '2026-02-07' AND '2026-02-13'
  AND Observaciones LIKE '%[MKT]%'
UNION ALL
SELECT 
    'Asistencias Últimos 7 Días',
    COUNT(*)
FROM Asistencias
WHERE FechaRegistro BETWEEN '2026-02-07' AND '2026-02-13'
  AND Observacion LIKE '%[MKT]%'
UNION ALL
SELECT 
    'Pagos Últimos 7 Días',
    COUNT(*)
FROM Pagos
WHERE FechaPago BETWEEN '2026-02-07' AND '2026-02-14'
  AND IdAlumno IN (
      SELECT IdAlumno 
      FROM Alumnos 
      WHERE IdUsuario IN (
          SELECT IdUsuario 
          FROM Usuarios 
          WHERE Correo LIKE '%@marketing.chetango.com'
      )
  )
UNION ALL
SELECT 
    'Paquetes por Vencer (próx 15 días)',
    COUNT(*)
FROM Paquetes
WHERE FechaVencimiento BETWEEN '2026-02-14' AND '2026-02-29'
  AND IdAlumno IN (
      SELECT IdAlumno 
      FROM Alumnos 
      WHERE IdUsuario IN (
          SELECT IdUsuario 
          FROM Usuarios 
          WHERE Correo LIKE '%@marketing.chetango.com'
      )
  );

PRINT '';
PRINT '========================================';
PRINT 'Actualización completada exitosamente';
PRINT '========================================';

GO

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT '========================================';
PRINT 'Creando Liquidaciones Mensuales';
PRINT '========================================';

-- Obtener estados como texto
DECLARE @EstadoPendiente NVARCHAR(50) = 'Pendiente';
DECLARE @EstadoPagada NVARCHAR(50) = 'Pagada';

PRINT 'Estados a usar:';
PRINT '  - Pendiente: ' + @EstadoPendiente;
PRINT '  - Pagada: ' + @EstadoPagada;

-- Crear liquidaciones agrupadas por Profesor + Mes/Año
DECLARE @Liquidaciones TABLE (
    IdProfesor UNIQUEIDENTIFIER,
    Mes INT,
    Año INT,
    TotalClases INT,
    TotalHoras DECIMAL(10,2),
    TotalBase DECIMAL(18,2),
    TotalAdicionales DECIMAL(18,2),
    TotalPagar DECIMAL(18,2),
    Estado NVARCHAR(50)
);

-- Agrupar ClasesProfesores por profesor y mes
INSERT INTO @Liquidaciones
SELECT 
    cp.IdProfesor,
    MONTH(c.Fecha) AS Mes,
    YEAR(c.Fecha) AS Año,
    COUNT(DISTINCT c.IdClase) AS TotalClases,
    SUM(DATEDIFF(MINUTE, c.HoraInicio, c.HoraFin)) / 60.0 AS TotalHoras,
    SUM(cp.TarifaProgramada) AS TotalBase,
    0 AS TotalAdicionales,  -- Sin adicionales por ahora
    SUM(cp.TarifaProgramada) AS TotalPagar,
    CASE 
        WHEN DATEDIFF(MONTH, MAX(c.Fecha), GETDATE()) >= 1 THEN @EstadoPagada
        ELSE @EstadoPendiente
    END AS Estado
FROM ClasesProfesores cp
INNER JOIN Clases c ON c.IdClase = cp.IdClase
WHERE c.Observaciones LIKE '%[MKT]%'
  AND c.Fecha < GETDATE()
GROUP BY cp.IdProfesor, MONTH(c.Fecha), YEAR(c.Fecha)
ORDER BY cp.IdProfesor, YEAR(c.Fecha), MONTH(c.Fecha);

DECLARE @TotalLiquidaciones INT = (SELECT COUNT(*) FROM @Liquidaciones);
PRINT 'Liquidaciones a crear: ' + CAST(@TotalLiquidaciones AS VARCHAR(10));

-- Insertar liquidaciones
DECLARE @IdProf UNIQUEIDENTIFIER;
DECLARE @Mes INT;
DECLARE @Año INT;
DECLARE @TotalClases INT;
DECLARE @TotalHoras DECIMAL(10,2);
DECLARE @TotalBase DECIMAL(18,2);
DECLARE @TotalAdicionales DECIMAL(18,2);
DECLARE @TotalPagar DECIMAL(18,2);
DECLARE @Estado NVARCHAR(50);
DECLARE @IdLiquidacion UNIQUEIDENTIFIER;
DECLARE @Contador INT = 0;

DECLARE liquidacion_cursor CURSOR FOR
SELECT IdProfesor, Mes, Año, TotalClases, TotalHoras, TotalBase, 
       TotalAdicionales, TotalPagar, Estado
FROM @Liquidaciones;

OPEN liquidacion_cursor;

FETCH NEXT FROM liquidacion_cursor 
INTO @IdProf, @Mes, @Año, @TotalClases, @TotalHoras, @TotalBase,
     @TotalAdicionales, @TotalPagar, @Estado;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @IdLiquidacion = NEWID();
    
    -- Insertar liquidación
    INSERT INTO LiquidacionesMensuales (
        IdLiquidacion,
        IdProfesor,
        Mes,
        Año,
        TotalClases,
        TotalHoras,
        TotalBase,
        TotalAdicionales,
        TotalPagar,
        Estado,
        FechaPago,
        Observaciones
    )
    VALUES (
        @IdLiquidacion,
        @IdProf,
        @Mes,
        @Año,
        @TotalClases,
        @TotalHoras,
        @TotalBase,
        @TotalAdicionales,
        @TotalPagar,
        @Estado,
        CASE WHEN @Estado = @EstadoPagada 
             THEN DATEFROMPARTS(@Año, @Mes, 28)  -- Fecha de pago ficticia al final del mes
             ELSE NULL 
        END,
        '[MKT] Liquidación ' + DATENAME(MONTH, DATEFROMPARTS(@Año, @Mes, 1)) + ' ' + CAST(@Año AS VARCHAR(4))
    );
    
    -- Actualizar ClasesProfesores para las clases de esta liquidación
    UPDATE cp
    SET cp.TotalPago = cp.TarifaProgramada,
        cp.EstadoPago = CASE WHEN @Estado = @EstadoPagada THEN 'Pagada' ELSE 'Pendiente' END,
        cp.IdLiquidacion = @IdLiquidacion
    FROM ClasesProfesores cp
    INNER JOIN Clases c ON c.IdClase = cp.IdClase
    WHERE cp.IdProfesor = @IdProf
      AND MONTH(c.Fecha) = @Mes
      AND YEAR(c.Fecha) = @Año
      AND c.Observaciones LIKE '%[MKT]%';
    
    SET @Contador = @Contador + 1;
    
    FETCH NEXT FROM liquidacion_cursor 
    INTO @IdProf, @Mes, @Año, @TotalClases, @TotalHoras, @TotalBase,
         @TotalAdicionales, @TotalPagar, @Estado;
END

CLOSE liquidacion_cursor;
DEALLOCATE liquidacion_cursor;

PRINT '';
PRINT '========================================';
PRINT 'Creadas: ' + CAST(@Contador AS VARCHAR(10)) + ' liquidaciones mensuales';
PRINT '========================================';

-- Mostrar resumen por profesor
PRINT '';
PRINT 'Resumen por profesor:';
SELECT 
    p.Nombre + ' ' + p.Apellido AS Profesor,
    COUNT(*) AS Liquidaciones,
    SUM(TotalClases) AS ClasesTotales,
    SUM(TotalPagar) AS PagoTotal
FROM LiquidacionesMensuales lm
INNER JOIN Profesores p ON p.IdProfesor = lm.IdProfesor
WHERE lm.Observaciones LIKE '%[MKT]%'
GROUP BY p.Nombre, p.Apellido
ORDER BY PagoTotal DESC;

GO

-- ============================================
-- MIGRACIÓN DESARROLLO: AgregarTarifaProfesorYPaqueteElenco
-- Base de Datos: ChetangoDB_Dev
-- Fecha: 2026-02-23
-- Descripción: 
--   1. Agregar campo TarifaProfesor a TiposPaquete
--   2. Crear paquete "Elenco Manizales" con tarifa especial
-- ============================================

USE [ChetangoDB_Dev];
GO

PRINT '============================================';
PRINT 'INICIO DE MIGRACIÓN - DESARROLLO';
PRINT 'Base de datos: ChetangoDB_Dev';
PRINT 'Fecha: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '============================================';
PRINT '';

-- ============================================
-- PASO 1: AGREGAR COLUMNA TarifaProfesor
-- ============================================

PRINT 'PASO 1: Agregando columna TarifaProfesor a TiposPaquete...';

IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.TiposPaquete') 
    AND name = 'TarifaProfesor'
)
BEGIN
    ALTER TABLE dbo.TiposPaquete
    ADD TarifaProfesor decimal(18,2) NULL;
    
    PRINT '✅ Columna TarifaProfesor agregada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠️  Columna TarifaProfesor ya existe - No se realizaron cambios';
END
GO

PRINT '';

-- ============================================
-- PASO 2: CREAR PAQUETE "ELENCO MANIZALES"
-- ============================================

PRINT 'PASO 2: Creando paquete Elenco Manizales...';

-- Verificar si ya existe
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Elenco Manizales')
BEGIN
    -- Crear el nuevo tipo de paquete (sin TarifaProfesor primero)
    INSERT INTO dbo.TiposPaquete (
        Id,
        Nombre,
        NumeroClases,
        Precio,
        DiasVigencia,
        Descripcion,
        Activo
    )
    VALUES (
        NEWID(),                                    -- Id único
        'Elenco Manizales',                        -- Nombre
        20,                                         -- 20 clases
        60000,                                      -- Precio para el alumno
        90,                                         -- 90 días de vigencia
        'Paquete especial para bailarines del Elenco de Manizales. Tarifa especial de profesor: $15,000/hora', -- Descripción
        1                                           -- Activo
    );
    
    -- Ahora actualizar la tarifa del profesor (la columna ya existe del PASO 1)
    UPDATE dbo.TiposPaquete
    SET TarifaProfesor = 15000
    WHERE Nombre = 'Elenco Manizales';
    
    PRINT '✅ Paquete "Elenco Manizales" creado exitosamente';
    PRINT '   - 20 clases';
    PRINT '   - Precio alumno: $60,000';
    PRINT '   - Vigencia: 90 días';
    PRINT '   - Tarifa profesor: $15,000/hora';
END
ELSE
BEGIN
    -- Si ya existe, solo actualizar la tarifa del profesor
    UPDATE dbo.TiposPaquete
    SET TarifaProfesor = 15000,
        Descripcion = 'Paquete especial para bailarines del Elenco de Manizales. Tarifa especial de profesor: $15,000/hora'
    WHERE Nombre = 'Elenco Manizales';
    
    PRINT '⚠️  Paquete "Elenco Manizales" ya existía';
    PRINT '✅ Tarifa profesor actualizada a $15,000/hora';
END

PRINT '';

-- ============================================
-- PASO 3: VERIFICACIÓN Y RESUMEN
-- ============================================

PRINT '============================================';
PRINT 'VERIFICACIÓN DE CAMBIOS';
PRINT '============================================';
PRINT '';

-- Verificar que la columna se agregó correctamente
PRINT 'Estructura de la tabla TiposPaquete:';
SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS Tipo,
    IS_NULLABLE AS Nullable,
    COLUMN_DEFAULT AS [Default]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'TiposPaquete'
AND COLUMN_NAME = 'TarifaProfesor';
PRINT '';

-- Mostrar todos los tipos de paquete con sus tarifas
PRINT 'Tipos de Paquete configurados:';
PRINT '--------------------------------------------';
SELECT 
    Nombre,
    NumeroClases AS Clases,
    FORMAT(Precio, 'C', 'es-CO') AS PrecioAlumno,
    CASE 
        WHEN TarifaProfesor IS NULL THEN 'Tarifa normal del profesor'
        ELSE FORMAT(TarifaProfesor, 'C', 'es-CO') + '/hora'
    END AS TarifaProfesor,
    CASE WHEN Activo = 1 THEN 'Sí' ELSE 'No' END AS Activo
FROM dbo.TiposPaquete
ORDER BY 
    CASE WHEN Nombre = 'Elenco Manizales' THEN 0 ELSE 1 END,
    Nombre;

PRINT '';
PRINT '============================================';
PRINT 'MIGRACIÓN COMPLETADA EXITOSAMENTE';
PRINT '============================================';
PRINT '';
PRINT 'RESUMEN:';
PRINT '- ✅ Campo TarifaProfesor agregado a TiposPaquete';
PRINT '- ✅ Paquete "Elenco Manizales" configurado';
PRINT '- ✅ Tarifa especial: $15,000/hora para clases del Elenco';
PRINT '';
PRINT 'COMPORTAMIENTO:';
PRINT '- Clases con alumnos del Elenco: $15,000/hora por profesor';
PRINT '- Clases regulares: Usa tarifa normal del profesor';
PRINT '';
PRINT 'Fecha finalización: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '============================================';

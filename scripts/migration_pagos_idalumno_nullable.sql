-- ============================================
-- MIGRACIÓN: Hacer IdAlumno nullable en Pagos
-- Fecha: 2026-02-07
-- Propósito: Soportar pagos compartidos (múltiples alumnos)
-- ============================================

USE ChetangoDB_Dev;
GO

-- Verificar que la columna existe y no es nullable
IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Pagos' 
    AND COLUMN_NAME = 'IdAlumno'
    AND IS_NULLABLE = 'NO'
)
BEGIN
    PRINT 'Modificando columna IdAlumno en tabla Pagos para permitir NULL...'
    
    -- Modificar la columna para permitir NULL
    ALTER TABLE Pagos
    ALTER COLUMN IdAlumno UNIQUEIDENTIFIER NULL;
    
    PRINT 'Columna modificada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La columna IdAlumno ya es nullable o no existe.'
END
GO

-- Verificar el cambio
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Pagos' AND COLUMN_NAME = 'IdAlumno';
GO

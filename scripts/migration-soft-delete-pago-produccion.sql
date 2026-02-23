-- ============================================
-- MIGRACIÓN: AddSoftDeleteToPago
-- Fecha: 2026-02-21
-- Descripción: Agregar columnas de soft delete a tabla Pagos
-- ============================================

-- Verificar si las columnas ya existen antes de agregarlas
BEGIN TRANSACTION;

-- Agregar columna Eliminado (bit, NOT NULL, default false)
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Pagos') 
    AND name = 'Eliminado'
)
BEGIN
    ALTER TABLE dbo.Pagos
    ADD Eliminado bit NOT NULL DEFAULT 0;
    
    PRINT 'Columna Eliminado agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'Columna Eliminado ya existe';
END

-- Agregar columna FechaEliminacion (datetime2, NULL)
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Pagos') 
    AND name = 'FechaEliminacion'
)
BEGIN
    ALTER TABLE dbo.Pagos
    ADD FechaEliminacion datetime2 NULL;
    
    PRINT 'Columna FechaEliminacion agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'Columna FechaEliminacion ya existe';
END

-- Agregar columna UsuarioEliminacion (nvarchar(256), NULL)
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.Pagos') 
    AND name = 'UsuarioEliminacion'
)
BEGIN
    ALTER TABLE dbo.Pagos
    ADD UsuarioEliminacion nvarchar(256) NULL;
    
    PRINT 'Columna UsuarioEliminacion agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'Columna UsuarioEliminacion ya existe';
END

COMMIT TRANSACTION;

-- Verificar que las columnas se agregaron correctamente
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Pagos'
AND COLUMN_NAME IN ('Eliminado', 'FechaEliminacion', 'UsuarioEliminacion')
ORDER BY COLUMN_NAME;

PRINT 'Migración completada exitosamente';

-- =====================================================
-- MIGRACIÓN MANUAL: Agregar columna Sede
-- Fecha: 2026-02-19
-- Descripción: Agregar columna Sede a tablas Usuarios, Pagos y LiquidacionesMensuales
-- =====================================================

-- Verificar si la columna ya existe antes de agregarla
-- Para evitar errores si el script se ejecuta múltiples veces

-- 1. Agregar columna Sede a Usuarios
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND name = 'Sede')
BEGIN
    ALTER TABLE [dbo].[Usuarios]
    ADD [Sede] int NOT NULL DEFAULT 1;
    
    PRINT 'Columna Sede agregada a tabla Usuarios con valor por defecto 1 (Medellín)';
END
ELSE
BEGIN
    PRINT 'Columna Sede ya existe en tabla Usuarios';
END
GO

-- 2. Agregar columna Sede a Pagos
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Pagos]') AND name = 'Sede')
BEGIN
    ALTER TABLE [dbo].[Pagos]
    ADD [Sede] int NOT NULL DEFAULT 1;
    
    PRINT 'Columna Sede agregada a tabla Pagos con valor por defecto 1 (Medellín)';
END
ELSE
BEGIN
    PRINT 'Columna Sede ya existe en tabla Pagos';
END
GO

-- 3. Agregar columna Sede a LiquidacionesMensuales
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[LiquidacionesMensuales]') AND name = 'Sede')
BEGIN
    ALTER TABLE [dbo].[LiquidacionesMensuales]
    ADD [Sede] int NOT NULL DEFAULT 1;
    
    PRINT 'Columna Sede agregada a tabla LiquidacionesMensuales con valor por defecto 1 (Medellín)';
END
ELSE
BEGIN
    PRINT 'Columna Sede ya existe en tabla LiquidacionesMensuales';
END
GO

-- 4. Registrar la migración en la tabla __EFMigrationsHistory
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260218150247_AgregarCampoSede')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260218150247_AgregarCampoSede', N'9.0.0');
    
    PRINT 'Migración registrada en __EFMigrationsHistory';
END
ELSE
BEGIN
    PRINT 'Migración ya estaba registrada en __EFMigrationsHistory';
END
GO

-- 5. Verificar que las columnas se agregaron correctamente
PRINT '==========================================';
PRINT 'VERIFICACIÓN DE MIGRACIÓN';
PRINT '==========================================';

SELECT 
    'Usuarios' as Tabla,
    COUNT(*) as TotalRegistros,
    SUM(CASE WHEN Sede = 1 THEN 1 ELSE 0 END) as Medellin,
    SUM(CASE WHEN Sede = 2 THEN 1 ELSE 0 END) as Manizales
FROM Usuarios;

SELECT 
    'Pagos' as Tabla,
    COUNT(*) as TotalRegistros,
    SUM(CASE WHEN Sede = 1 THEN 1 ELSE 0 END) as Medellin,
    SUM(CASE WHEN Sede = 2 THEN 1 ELSE 0 END) as Manizales
FROM Pagos;

SELECT 
    'LiquidacionesMensuales' as Tabla,
    COUNT(*) as TotalRegistros,
    SUM(CASE WHEN Sede = 1 THEN 1 ELSE 0 END) as Medellin,
    SUM(CASE WHEN Sede = 2 THEN 1 ELSE 0 END) as Manizales
FROM LiquidacionesMensuales;

PRINT 'Migración completada exitosamente';

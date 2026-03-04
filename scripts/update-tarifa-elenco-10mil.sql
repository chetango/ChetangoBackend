-- ============================================
-- ACTUALIZACIÓN: Tarifa Paquete Elenco Manizales
-- Base de Datos: chetango-db-prod
-- Fecha: 2026-02-23
-- Descripción: Cambiar tarifa profesor de $15,000 a $10,000/hora
-- ============================================

USE [chetango-db-prod];
GO

PRINT '============================================';
PRINT 'Actualizando tarifa profesor Elenco Manizales';
PRINT '============================================';
PRINT '';

-- Mostrar valor actual
SELECT 
    Nombre,
    TarifaProfesor AS TarifaActual
FROM dbo.TiposPaquete
WHERE Nombre = 'Elenco Manizales';

-- Actualizar a $10,000/hora
UPDATE dbo.TiposPaquete
SET TarifaProfesor = 10000,
    Descripcion = 'Paquete especial para bailarines del Elenco de Manizales. Tarifa especial de profesor: $10,000/hora'
WHERE Nombre = 'Elenco Manizales';

PRINT '';
PRINT '✅ Tarifa actualizada exitosamente';
PRINT '';

-- Verificar cambio
SELECT 
    Nombre,
    NumeroClases AS Clases,
    FORMAT(Precio, 'C', 'es-CO') AS PrecioAlumno,
    FORMAT(TarifaProfesor, 'C', 'es-CO') AS TarifaProfesor,
    Descripcion
FROM dbo.TiposPaquete
WHERE Nombre = 'Elenco Manizales';

PRINT '';
PRINT '============================================';
PRINT 'Actualización completada';
PRINT 'Nueva tarifa: $10,000/hora por profesor';
PRINT '============================================';

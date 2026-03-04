-- ============================================
-- ACTUALIZACIÓN: Nomenclatura de Paquetes + Eliminación
-- Base de Datos: chetango-db-prod
-- Fecha: 2026-02-23
-- ============================================

USE [chetango-db-prod];
GO

PRINT '============================================';
PRINT 'PASO 1: DESACTIVAR PAQUETES NO UTILIZADOS';
PRINT '============================================';
PRINT '';

-- Desactivar BonoClases (no se puede eliminar por FK)
UPDATE dbo.TiposPaquete 
SET Activo = 0, Nombre = '[INACTIVO] BonoClases'
WHERE Nombre = 'BonoClases';
PRINT '✅ Desactivado: BonoClases';

-- Desactivar Ilimitado
UPDATE dbo.TiposPaquete 
SET Activo = 0, Nombre = '[INACTIVO] Ilimitado'
WHERE Nombre = 'Ilimitado';
PRINT '✅ Desactivado: Ilimitado';

-- Desactivar Mensual
UPDATE dbo.TiposPaquete 
SET Activo = 0, Nombre = '[INACTIVO] Mensual'
WHERE Nombre = 'Mensual';
PRINT '✅ Desactivado: Mensual';

PRINT '';
PRINT '============================================';
PRINT 'PASO 2: ACTUALIZAR NOMENCLATURA';
PRINT '============================================';
PRINT '';

-- MANIZALES
UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal - 4 Clases - Manizales'
WHERE Nombre = '1 Manizales';
PRINT '✅ Actualizado: 1 Manizales → Grupal - 4 Clases - Manizales';

UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal - 8 Clases - Manizales'
WHERE Nombre = '2 Manizales';
PRINT '✅ Actualizado: 2 Manizales → Grupal - 8 Clases - Manizales';

UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal Pareja - 8 Clases - Manizales'
WHERE Nombre = '3 Manizales Pareja';
PRINT '✅ Actualizado: 3 Manizales Pareja → Grupal Pareja - 8 Clases - Manizales';

UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal Pareja - 4 Clases - Manizales'
WHERE Nombre = '4 Manizales pareja';
PRINT '✅ Actualizado: 4 Manizales pareja → Grupal Pareja - 4 Clases - Manizales';

UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal - 12 Clases - Manizales'
WHERE Nombre = '5 Manizales 12 clases';
PRINT '✅ Actualizado: 5 Manizales 12 clases → Grupal - 12 Clases - Manizales';

UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal - 1 Clase Suelta - Manizales'
WHERE Nombre = '6 Manizales clase suelta';
PRINT '✅ Actualizado: 6 Manizales clase suelta → Grupal - 1 Clase Suelta - Manizales';

UPDATE dbo.TiposPaquete
SET Nombre = 'Elenco - 8 Clases - Manizales'
WHERE Nombre = 'Elenco Manizales';
PRINT '✅ Actualizado: Elenco Manizales → Elenco - 8 Clases - Manizales';

-- MEDELLÍN (solo uno)
UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal - 4 Clases - Medellín'
WHERE Nombre = '1.Paquete 4 Clases 110,000 - 30 dias';
PRINT '✅ Actualizado: 1.Paquete 4 Clases 110,000 - 30 dias → Grupal - 4 Clases - Medellín';

PRINT '';
PRINT '============================================';
PRINT 'VERIFICACIÓN FINAL';
PRINT '============================================';
PRINT '';

-- Mostrar todos los paquetes activos con nueva nomenclatura
SELECT 
    Nombre,
    NumeroClases AS Clases,
    FORMAT(Precio, 'C', 'es-CO') AS Precio,
    CASE 
        WHEN TarifaProfesor IS NULL THEN 'Normal'
        ELSE FORMAT(TarifaProfesor, 'C', 'es-CO') + '/hora'
    END AS TarifaProfesor,
    CASE WHEN Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
FROM dbo.TiposPaquete
ORDER BY 
    CASE WHEN Nombre LIKE '%Manizales%' THEN 1 ELSE 2 END,
    NumeroClases;

PRINT '';
PRINT '============================================';
PRINT 'ACTUALIZACIÓN COMPLETADA';
PRINT '============================================';
PRINT '';
PRINT '✅ 3 paquetes desactivados (BonoClases, Ilimitado, Mensual)';
PRINT '✅ 8 paquetes renombrados con nueva nomenclatura';
PRINT '';

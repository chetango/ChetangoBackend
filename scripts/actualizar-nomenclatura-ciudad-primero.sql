-- ============================================
-- ACTUALIZACIÓN: Nomenclatura Paquetes - Ciudad Primero
-- Base de Datos: chetango-db-prod
-- Fecha: 2026-02-23
-- Descripción: Cambiar nomenclatura para que la ciudad aparezca primero
-- Formato: [Ciudad] - [Tipo] - [Cantidad]
-- ============================================

USE [chetango-db-prod];
GO

PRINT '============================================';
PRINT 'ACTUALIZACIÓN: CIUDAD PRIMERO EN NOMENCLATURA';
PRINT '============================================';
PRINT '';

-- ============================================
-- PAQUETES MANIZALES (7 activos)
-- ============================================

PRINT 'Actualizando paquetes de Manizales...';
PRINT '';

-- Grupal - 4 Clases - Manizales
UPDATE dbo.TiposPaquete
SET Nombre = 'Manizales - Grupal - 4 Clases'
WHERE Nombre = 'Grupal - 4 Clases - Manizales';
PRINT '✅ Grupal - 4 Clases - Manizales → Manizales - Grupal - 4 Clases';

-- Grupal - 8 Clases - Manizales
UPDATE dbo.TiposPaquete
SET Nombre = 'Manizales - Grupal - 8 Clases'
WHERE Nombre = 'Grupal - 8 Clases - Manizales';
PRINT '✅ Grupal - 8 Clases - Manizales → Manizales - Grupal - 8 Clases';

-- Grupal - 12 Clases - Manizales
UPDATE dbo.TiposPaquete
SET Nombre = 'Manizales - Grupal - 12 Clases'
WHERE Nombre = 'Grupal - 12 Clases - Manizales';
PRINT '✅ Grupal - 12 Clases - Manizales → Manizales - Grupal - 12 Clases';

-- Grupal - 1 Clase Suelta - Manizales
UPDATE dbo.TiposPaquete
SET Nombre = 'Manizales - Grupal - 1 Clase Suelta'
WHERE Nombre = 'Grupal - 1 Clase Suelta - Manizales';
PRINT '✅ Grupal - 1 Clase Suelta - Manizales → Manizales - Grupal - 1 Clase Suelta';

-- Grupal Pareja - 4 Clases - Manizales
UPDATE dbo.TiposPaquete
SET Nombre = 'Manizales - Grupal Pareja - 4 Clases'
WHERE Nombre = 'Grupal Pareja - 4 Clases - Manizales';
PRINT '✅ Grupal Pareja - 4 Clases - Manizales → Manizales - Grupal Pareja - 4 Clases';

-- Grupal Pareja - 8 Clases - Manizales
UPDATE dbo.TiposPaquete
SET Nombre = 'Manizales - Grupal Pareja - 8 Clases'
WHERE Nombre = 'Grupal Pareja - 8 Clases - Manizales';
PRINT '✅ Grupal Pareja - 8 Clases - Manizales → Manizales - Grupal Pareja - 8 Clases';

-- Elenco - 8 Clases - Manizales (con tarifa especial)
UPDATE dbo.TiposPaquete
SET Nombre = 'Manizales - Elenco - 8 Clases'
WHERE Nombre = 'Elenco - 8 Clases - Manizales';
PRINT '✅ Elenco - 8 Clases - Manizales → Manizales - Elenco - 8 Clases';

PRINT '';

-- ============================================
-- PAQUETES MEDELLÍN (11 paquetes)
-- ============================================

PRINT 'Actualizando paquetes de Medellín...';
PRINT '';

-- GRUPALES MEDELLÍN

-- Grupal - 4 Clases - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Grupal - 4 Clases'
WHERE Nombre = 'Grupal - 4 Clases - Medellín';
PRINT '✅ Grupal - 4 Clases - Medellín → Medellín - Grupal - 4 Clases';

-- Grupal - 8 Clases - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Grupal - 8 Clases'
WHERE Nombre = 'Grupal - 8 Clases - Medellín';
PRINT '✅ Grupal - 8 Clases - Medellín → Medellín - Grupal - 8 Clases';

-- Grupal - 12 Clases - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Grupal - 12 Clases'
WHERE Nombre = 'Grupal - 12 Clases - Medellín';
PRINT '✅ Grupal - 12 Clases - Medellín → Medellín - Grupal - 12 Clases';

PRINT '';

-- PRIVADAS 1 PERSONA MEDELLÍN

-- Privada 1P - 1 Clase Prueba - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Privada 1P - 1 Clase Prueba'
WHERE Nombre = 'Privada 1P - 1 Clase Prueba - Medellín';
PRINT '✅ Privada 1P - 1 Clase Prueba - Medellín → Medellín - Privada 1P - 1 Clase Prueba';

-- Privada 1P - 4 Clases - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Privada 1P - 4 Clases'
WHERE Nombre = 'Privada 1P - 4 Clases - Medellín';
PRINT '✅ Privada 1P - 4 Clases - Medellín → Medellín - Privada 1P - 4 Clases';

-- Privada 1P - 8 Clases - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Privada 1P - 8 Clases'
WHERE Nombre = 'Privada 1P - 8 Clases - Medellín';
PRINT '✅ Privada 1P - 8 Clases - Medellín → Medellín - Privada 1P - 8 Clases';

-- Privada 1P - 12 Clases - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Privada 1P - 12 Clases'
WHERE Nombre = 'Privada 1P - 12 Clases - Medellín';
PRINT '✅ Privada 1P - 12 Clases - Medellín → Medellín - Privada 1P - 12 Clases';

PRINT '';

-- PRIVADAS 2 PERSONAS MEDELLÍN

-- Privada 2P - 1 Clase - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Privada 2P - 1 Clase'
WHERE Nombre = 'Privada 2P - 1 Clase - Medellín';
PRINT '✅ Privada 2P - 1 Clase - Medellín → Medellín - Privada 2P - 1 Clase';

-- Privada 2P - 4 Clases - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Privada 2P - 4 Clases'
WHERE Nombre = 'Privada 2P - 4 Clases - Medellín';
PRINT '✅ Privada 2P - 4 Clases - Medellín → Medellín - Privada 2P - 4 Clases';

-- Privada 2P - 8 Clases - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Privada 2P - 8 Clases'
WHERE Nombre = 'Privada 2P - 8 Clases - Medellín';
PRINT '✅ Privada 2P - 8 Clases - Medellín → Medellín - Privada 2P - 8 Clases';

-- Privada 2P - 12 Clases - Medellín
UPDATE dbo.TiposPaquete
SET Nombre = 'Medellín - Privada 2P - 12 Clases'
WHERE Nombre = 'Privada 2P - 12 Clases - Medellín';
PRINT '✅ Privada 2P - 12 Clases - Medellín → Medellín - Privada 2P - 12 Clases';

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
    DiasVigencia AS Vigencia,
    CASE 
        WHEN TarifaProfesor IS NULL THEN 'Tarifa normal'
        ELSE FORMAT(TarifaProfesor, 'C', 'es-CO') + '/hora'
    END AS TarifaProfesor,
    CASE WHEN Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
FROM dbo.TiposPaquete
WHERE Activo = 1
ORDER BY 
    -- Agrupar por ciudad
    CASE 
        WHEN Nombre LIKE 'Manizales%' THEN 1
        WHEN Nombre LIKE 'Medellín%' THEN 2
        ELSE 3
    END,
    Nombre;

PRINT '';
PRINT '============================================';
PRINT 'ACTUALIZACIÓN COMPLETADA EXITOSAMENTE';
PRINT '============================================';
PRINT '';
PRINT '✅ 7 paquetes de Manizales actualizados';
PRINT '✅ 11 paquetes de Medellín actualizados';
PRINT '✅ Total: 18 paquetes con nueva nomenclatura';
PRINT '';
PRINT 'Nueva nomenclatura: [Ciudad] - [Tipo] - [Cantidad]';
PRINT 'Ejemplo: Manizales - Grupal - 4 Clases';
PRINT '         Medellín - Privada 1P - 8 Clases';
PRINT '';

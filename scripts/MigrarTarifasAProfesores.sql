-- ============================================
-- SCRIPT DE MIGRACIÓN: Tarifas Individuales por Profesor
-- ============================================
-- Este script migra las tarifas del sistema antiguo (TarifaProfesor basado en tipo)
-- al sistema nuevo (TarifaActual individual por profesor)
--
-- ESTRATEGIA:
-- 1. Para cada profesor, buscar la tarifa correspondiente a su tipo + rol "Principal"
-- 2. Si no existe tarifa para Principal, usar la de "Monitor"
-- 3. Si no existe ninguna, dejar en 0 (para revisión manual)
-- ============================================

-- Paso 1: Verificar estado actual
SELECT 
    'ANTES DE MIGRACIÓN' AS Estado,
    COUNT(*) AS TotalProfesores,
    SUM(CASE WHEN TarifaActual = 0 THEN 1 ELSE 0 END) AS ProfesoresSinTarifa,
    SUM(CASE WHEN TarifaActual > 0 THEN 1 ELSE 0 END) AS ProfesoresConTarifa
FROM Profesores;

-- Paso 2: Actualizar tarifas desde TarifaProfesor
-- Primero intentamos con rol "Principal"
UPDATE p
SET p.TarifaActual = tp.ValorPorClase
FROM Profesores p
INNER JOIN TiposProfesor tpp ON p.IdTipoProfesor = tpp.Id
INNER JOIN TarifasProfesor tp ON tp.IdTipoProfesor = tpp.Id
INNER JOIN RolesEnClase rc ON tp.IdRolEnClase = rc.Id
WHERE rc.Nombre = 'Principal'
  AND p.TarifaActual = 0; -- Solo actualizar los que aún no tienen tarifa

PRINT 'Actualizadas tarifas con rol Principal';

-- Si quedan profesores sin tarifa, intentar con rol "Monitor"
UPDATE p
SET p.TarifaActual = tp.ValorPorClase
FROM Profesores p
INNER JOIN TiposProfesor tpp ON p.IdTipoProfesor = tpp.Id
INNER JOIN TarifasProfesor tp ON tp.IdTipoProfesor = tpp.Id
INNER JOIN RolesEnClase rc ON tp.IdRolEnClase = rc.Id
WHERE rc.Nombre = 'Monitor'
  AND p.TarifaActual = 0;

PRINT 'Actualizadas tarifas con rol Monitor para profesores restantes';

-- Paso 3: Reportar profesores sin tarifa (requieren revisión manual)
SELECT 
    p.IdProfesor,
    u.NombreUsuario,
    u.Correo,
    tp.Nombre AS TipoProfesor,
    p.TarifaActual
FROM Profesores p
INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
INNER JOIN TiposProfesor tp ON p.IdTipoProfesor = tp.Id
WHERE p.TarifaActual = 0;

-- Paso 4: Verificar resultado final
SELECT 
    'DESPUÉS DE MIGRACIÓN' AS Estado,
    COUNT(*) AS TotalProfesores,
    SUM(CASE WHEN TarifaActual = 0 THEN 1 ELSE 0 END) AS ProfesoresSinTarifa,
    SUM(CASE WHEN TarifaActual > 0 THEN 1 ELSE 0 END) AS ProfesoresConTarifa,
    AVG(TarifaActual) AS TarifaPromedio,
    MIN(TarifaActual) AS TarifaMinima,
    MAX(TarifaActual) AS TarifaMaxima
FROM Profesores;

-- Paso 5: Detalles por tipo de profesor
SELECT 
    tp.Nombre AS TipoProfesor,
    COUNT(*) AS CantidadProfesores,
    AVG(p.TarifaActual) AS TarifaPromedio,
    MIN(p.TarifaActual) AS TarifaMinima,
    MAX(p.TarifaActual) AS TarifaMaxima
FROM Profesores p
INNER JOIN TiposProfesor tp ON p.IdTipoProfesor = tp.Id
GROUP BY tp.Nombre;

PRINT 'Migración completada. Revisar profesores sin tarifa si los hay.';

-- ============================================
-- ACTUALIZACIÓN: Nomenclatura de Paquetes
-- Base de Datos: chetango-db-prod
-- Fecha: 2026-02-23
-- Descripción: Estandarizar nombres de paquetes con formato [Tipo] - [Cantidad] - [Sede]
-- ============================================

USE [chetango-db-prod];
GO

PRINT '============================================';
PRINT 'CONSULTA DE PAQUETES ACTUALES';
PRINT '============================================';
PRINT '';

-- Ver todos los paquetes actuales
SELECT 
    Id,
    Nombre AS NombreActual,
    NumeroClases,
    FORMAT(Precio, 'C', 'es-CO') AS Precio,
    CASE WHEN Activo = 1 THEN 'Sí' ELSE 'No' END AS Activo
FROM dbo.TiposPaquete
ORDER BY Nombre;

PRINT '';
PRINT '============================================';
PRINT 'INICIANDO ACTUALIZACIÓN DE NOMENCLATURA';
PRINT '============================================';
PRINT '';

-- PAQUETES GRUPALES MEDELLÍN
UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal - 4 Clases - Medellín'
WHERE Nombre = 'Paquete 4 clases';

UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal - 8 Clases - Medellín'
WHERE Nombre = 'Paquete 8 clases';

UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal - 12 Clases - Medellín'
WHERE Nombre = 'Paquete 12 clases';

UPDATE dbo.TiposPaquete
SET Nombre = 'Grupal - Mensual Ilimitado - Medellín'
WHERE Nombre = 'Mensual (ilimitado)';

PRINT '✅ Paquetes grupales Medellín actualizados';

-- PAQUETES PRIVADOS 1 PERSONA MEDELLÍN
UPDATE dbo.TiposPaquete
SET Nombre = 'Privada 1P - 1 Clase Prueba - Medellín'
WHERE Nombre = 'Privada 1 Persona - 1 Clase prueba';

UPDATE dbo.TiposPaquete
SET Nombre = 'Privada 1P - 4 Clases - Medellín'
WHERE Nombre = 'Privada 1 Persona - 4 Clases';

UPDATE dbo.TiposPaquete
SET Nombre = 'Privada 1P - 8 Clases - Medellín'
WHERE Nombre = 'Privada 1 Persona - 8 Clases';

UPDATE dbo.TiposPaquete
SET Nombre = 'Privada 1P - 12 Clases - Medellín'
WHERE Nombre = 'Privada 1 Persona - 12 Clases';

PRINT '✅ Paquetes privados 1 persona Medellín actualizados';

-- PAQUETES PRIVADOS 2 PERSONAS MEDELLÍN
UPDATE dbo.TiposPaquete
SET Nombre = 'Privada 2P - 1 Clase - Medellín'
WHERE Nombre = 'Privada 2 Personas - 1 Clase';

UPDATE dbo.TiposPaquete
SET Nombre = 'Privada 2P - 4 Clases - Medellín'
WHERE Nombre = 'Privada 2 Personas - 4 Clases';

UPDATE dbo.TiposPaquete
SET Nombre = 'Privada 2P - 8 Clases - Medellín'
WHERE Nombre = 'Privada 2 Personas - 8 Clases';

UPDATE dbo.TiposPaquete
SET Nombre = 'Privada 2P - 12 Clases - Medellín'
WHERE Nombre = 'Privada 2 Personas - 12 Clases';

PRINT '✅ Paquetes privados 2 personas Medellín actualizados';

-- PAQUETE ESPECIAL MANIZALES (ya tiene buena nomenclatura, solo ajustar)
UPDATE dbo.TiposPaquete
SET Nombre = 'Elenco - 20 Clases - Manizales'
WHERE Nombre = 'Elenco Manizales';

PRINT '✅ Paquete Elenco Manizales actualizado';

PRINT '';
PRINT '============================================';
PRINT 'VERIFICACIÓN FINAL';
PRINT '============================================';
PRINT '';

-- Mostrar todos los paquetes con nueva nomenclatura
SELECT 
    Nombre AS NuevaNomenclatura,
    NumeroClases AS Clases,
    FORMAT(Precio, 'C', 'es-CO') AS Precio,
    CASE 
        WHEN TarifaProfesor IS NULL THEN 'Tarifa normal'
        ELSE FORMAT(TarifaProfesor, 'C', 'es-CO') + '/hora'
    END AS TarifaProfesor,
    CASE WHEN Activo = 1 THEN 'Sí' ELSE 'No' END AS Activo
FROM dbo.TiposPaquete
ORDER BY 
    -- Agrupar por tipo
    CASE 
        WHEN Nombre LIKE 'Grupal%' THEN 1
        WHEN Nombre LIKE 'Privada 1P%' THEN 2
        WHEN Nombre LIKE 'Privada 2P%' THEN 3
        WHEN Nombre LIKE 'Elenco%' THEN 4
        ELSE 5
    END,
    NumeroClases;

PRINT '';
PRINT '============================================';
PRINT 'ACTUALIZACIÓN COMPLETADA EXITOSAMENTE';
PRINT '============================================';
PRINT '';
PRINT 'Nomenclatura aplicada: [Tipo] - [Cantidad] - [Sede]';
PRINT '';
PRINT 'Tipos de paquetes:';
PRINT '  - Grupal: Clases grupales regulares';
PRINT '  - Privada 1P: Clases privadas 1 persona';
PRINT '  - Privada 2P: Clases privadas 2 personas';
PRINT '  - Elenco: Paquetes especiales para elenco';
PRINT '';

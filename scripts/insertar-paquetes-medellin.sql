-- ============================================
-- INSERCIÓN: Paquetes Medellín
-- Base de Datos: chetango-db-prod
-- Fecha: 2026-02-23
-- Descripción: Crear 10 paquetes nuevos para Medellín con nomenclatura estandarizada
-- ============================================

USE [chetango-db-prod];
GO

PRINT '============================================';
PRINT 'INSERCIÓN DE PAQUETES MEDELLÍN';
PRINT '============================================';
PRINT '';

-- PAQUETES GRUPALES MEDELLÍN

-- Grupal - 8 Clases - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Grupal - 8 Clases - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Grupal - 8 Clases - Medellín', 8, 140000, 60, 'Paquete grupal de 8 clases para Medellín', 1, NULL);
    PRINT '✅ Creado: Grupal - 8 Clases - Medellín';
END

-- Grupal - 12 Clases - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Grupal - 12 Clases - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Grupal - 12 Clases - Medellín', 12, 180000, 90, 'Paquete grupal de 12 clases para Medellín', 1, NULL);
    PRINT '✅ Creado: Grupal - 12 Clases - Medellín';
END

PRINT '';

-- PAQUETES PRIVADOS 1 PERSONA MEDELLÍN

-- Privada 1P - 1 Clase Prueba - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Privada 1P - 1 Clase Prueba - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Privada 1P - 1 Clase Prueba - Medellín', 1, 90000, 30, 'Clase privada de prueba para 1 persona en Medellín', 1, NULL);
    PRINT '✅ Creado: Privada 1P - 1 Clase Prueba - Medellín';
END

-- Privada 1P - 4 Clases - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Privada 1P - 4 Clases - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Privada 1P - 4 Clases - Medellín', 4, 360000, 60, 'Paquete privado de 4 clases para 1 persona en Medellín', 1, NULL);
    PRINT '✅ Creado: Privada 1P - 4 Clases - Medellín';
END

-- Privada 1P - 8 Clases - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Privada 1P - 8 Clases - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Privada 1P - 8 Clases - Medellín', 8, 720000, 90, 'Paquete privado de 8 clases para 1 persona en Medellín', 1, NULL);
    PRINT '✅ Creado: Privada 1P - 8 Clases - Medellín';
END

-- Privada 1P - 12 Clases - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Privada 1P - 12 Clases - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Privada 1P - 12 Clases - Medellín', 12, 1080000, 120, 'Paquete privado de 12 clases para 1 persona en Medellín', 1, NULL);
    PRINT '✅ Creado: Privada 1P - 12 Clases - Medellín';
END

PRINT '';

-- PAQUETES PRIVADOS 2 PERSONAS MEDELLÍN

-- Privada 2P - 1 Clase - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Privada 2P - 1 Clase - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Privada 2P - 1 Clase - Medellín', 1, 140000, 30, 'Clase privada para 2 personas en Medellín', 1, NULL);
    PRINT '✅ Creado: Privada 2P - 1 Clase - Medellín';
END

-- Privada 2P - 4 Clases - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Privada 2P - 4 Clases - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Privada 2P - 4 Clases - Medellín', 4, 560000, 60, 'Paquete privado de 4 clases para 2 personas en Medellín', 1, NULL);
    PRINT '✅ Creado: Privada 2P - 4 Clases - Medellín';
END

-- Privada 2P - 8 Clases - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Privada 2P - 8 Clases - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Privada 2P - 8 Clases - Medellín', 8, 1120000, 90, 'Paquete privado de 8 clases para 2 personas en Medellín', 1, NULL);
    PRINT '✅ Creado: Privada 2P - 8 Clases - Medellín';
END

-- Privada 2P - 12 Clases - Medellín
IF NOT EXISTS (SELECT 1 FROM dbo.TiposPaquete WHERE Nombre = 'Privada 2P - 12 Clases - Medellín')
BEGIN
    INSERT INTO dbo.TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo, TarifaProfesor)
    VALUES (NEWID(), 'Privada 2P - 12 Clases - Medellín', 12, 1680000, 120, 'Paquete privado de 12 clases para 2 personas en Medellín', 1, NULL);
    PRINT '✅ Creado: Privada 2P - 12 Clases - Medellín';
END

PRINT '';
PRINT '============================================';
PRINT 'VERIFICACIÓN FINAL';
PRINT '============================================';
PRINT '';

-- Mostrar todos los paquetes de Medellín
SELECT 
    Nombre,
    NumeroClases AS Clases,
    FORMAT(Precio, 'C', 'es-CO') AS Precio,
    DiasVigencia AS Vigencia,
    CASE WHEN Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
FROM dbo.TiposPaquete
WHERE Nombre LIKE '%Medellín%'
ORDER BY 
    CASE 
        WHEN Nombre LIKE 'Grupal%' THEN 1
        WHEN Nombre LIKE 'Privada 1P%' THEN 2
        WHEN Nombre LIKE 'Privada 2P%' THEN 3
        ELSE 4
    END,
    NumeroClases;

PRINT '';
PRINT '============================================';
PRINT 'INSERCIÓN COMPLETADA';
PRINT '============================================';
PRINT '';
PRINT '✅ 10 paquetes nuevos de Medellín creados';
PRINT '✅ Total: 2 grupales + 4 privados 1P + 4 privados 2P';
PRINT '';

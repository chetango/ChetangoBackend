-- ============================================
-- Script: Crear Tipos de Paquete para Clases Privadas
-- Fecha: 6 de febrero de 2026
-- Descripción: Agrega tipos de paquete específicos para clases privadas
--              con 1 o 2 personas, permitiendo precios flexibles
-- ============================================

USE ChetangoDB_Dev;
GO

BEGIN TRANSACTION;

PRINT '========================================';
PRINT 'Creando Tipos de Paquete PRIVADOS';
PRINT '========================================';

-- ============================================
-- TIPOS DE PAQUETE PARA 1 PERSONA
-- ============================================

DECLARE @TipoPrivada1P_1Clase UNIQUEIDENTIFIER = 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA';
DECLARE @TipoPrivada1P_4Clases UNIQUEIDENTIFIER = 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB';
DECLARE @TipoPrivada1P_8Clases UNIQUEIDENTIFIER = 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC';
DECLARE @TipoPrivada1P_12Clases UNIQUEIDENTIFIER = 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD';

-- Eliminar si existen (para re-ejecutar script)
DELETE FROM TiposPaquete WHERE Id IN (
    @TipoPrivada1P_1Clase, @TipoPrivada1P_4Clases, 
    @TipoPrivada1P_8Clases, @TipoPrivada1P_12Clases
);

INSERT INTO TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo)
VALUES
(
    @TipoPrivada1P_1Clase,
    'Privada 1 Persona - 1 Clase',
    1,
    90000,
    30,
    'Clase privada individual. Precio sugerido: $90,000 (editable por admin)',
    1
),
(
    @TipoPrivada1P_4Clases,
    'Privada 1 Persona - 4 Clases',
    4,
    360000,
    60,
    'Paquete de 4 clases privadas individuales. Precio sugerido: $360,000 ($90k c/u)',
    1
),
(
    @TipoPrivada1P_8Clases,
    'Privada 1 Persona - 8 Clases',
    8,
    720000,
    90,
    'Paquete de 8 clases privadas individuales. Precio sugerido: $720,000 ($90k c/u)',
    1
),
(
    @TipoPrivada1P_12Clases,
    'Privada 1 Persona - 12 Clases',
    12,
    1080000,
    120,
    'Paquete de 12 clases privadas individuales. Precio sugerido: $1,080,000 ($90k c/u)',
    1
);

PRINT '✓ Tipos de paquete 1 PERSONA creados (4 tipos)';

-- ============================================
-- TIPOS DE PAQUETE PARA 2 PERSONAS (PAREJAS)
-- ============================================

DECLARE @TipoPrivada2P_1Clase UNIQUEIDENTIFIER = 'EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE';
DECLARE @TipoPrivada2P_4Clases UNIQUEIDENTIFIER = 'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF';
DECLARE @TipoPrivada2P_8Clases UNIQUEIDENTIFIER = '11111111-2222-3333-4444-555555555555';
DECLARE @TipoPrivada2P_12Clases UNIQUEIDENTIFIER = '22222222-3333-4444-5555-666666666666';

-- Eliminar si existen
DELETE FROM TiposPaquete WHERE Id IN (
    @TipoPrivada2P_1Clase, @TipoPrivada2P_4Clases,
    @TipoPrivada2P_8Clases, @TipoPrivada2P_12Clases
);

INSERT INTO TiposPaquete (Id, Nombre, NumeroClases, Precio, DiasVigencia, Descripcion, Activo)
VALUES
(
    @TipoPrivada2P_1Clase,
    'Privada 2 Personas - 1 Clase',
    1,
    140000,
    30,
    'Clase privada para pareja (2 personas). Precio total: $140,000. Se crean 2 paquetes vinculados (1 por alumno)',
    1
),
(
    @TipoPrivada2P_4Clases,
    'Privada 2 Personas - 4 Clases',
    4,
    560000,
    60,
    'Paquete de 4 clases privadas para pareja. Precio total: $560,000 ($140k c/u). Se crean 2 paquetes vinculados',
    1
),
(
    @TipoPrivada2P_8Clases,
    'Privada 2 Personas - 8 Clases',
    8,
    1120000,
    90,
    'Paquete de 8 clases privadas para pareja. Precio total: $1,120,000 ($140k c/u). Se crean 2 paquetes vinculados',
    1
),
(
    @TipoPrivada2P_12Clases,
    'Privada 2 Personas - 12 Clases',
    12,
    1680000,
    120,
    'Paquete de 12 clases privadas para pareja. Precio total: $1,680,000 ($140k c/u). Se crean 2 paquetes vinculados',
    1
);

PRINT '✓ Tipos de paquete 2 PERSONAS creados (4 tipos)';

COMMIT TRANSACTION;

-- ============================================
-- VERIFICACIÓN
-- ============================================

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN - Tipos de Paquete Privados';
PRINT '========================================';

SELECT 
    Nombre,
    NumeroClases AS 'Clases',
    FORMAT(Precio, 'C0', 'es-CO') AS 'Precio',
    DiasVigencia AS 'Días',
    Activo,
    Descripcion
FROM TiposPaquete
WHERE Nombre LIKE 'Privada%'
ORDER BY 
    CASE WHEN Nombre LIKE '%1 Persona%' THEN 1 ELSE 2 END,
    NumeroClases;

PRINT '';
PRINT '========================================';
PRINT '✓ Script completado exitosamente';
PRINT '';
PRINT 'IMPORTANTE:';
PRINT '- Los tipos "2 Personas" requieren crear 2 paquetes';
PRINT '- Ambos paquetes se vinculan al mismo pago';
PRINT '- Cada alumno tiene su propio paquete independiente';
PRINT '========================================';

GO

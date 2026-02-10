-- =============================================
-- MIGRACIÓN: Agregar campos a TiposPaquete
-- Fecha: 28 Enero 2026
-- Descripción: Agrega NumeroClases, Precio, DiasVigencia, Descripcion, Activo
-- =============================================

USE ChetangoDB_Dev;
GO

-- 1. Agregar nuevas columnas
ALTER TABLE TiposPaquete ADD NumeroClases INT NOT NULL DEFAULT 0;
ALTER TABLE TiposPaquete ADD Precio DECIMAL(18,2) NOT NULL DEFAULT 0;
ALTER TABLE TiposPaquete ADD DiasVigencia INT NOT NULL DEFAULT 30;
ALTER TABLE TiposPaquete ADD Descripcion NVARCHAR(500) NULL;
ALTER TABLE TiposPaquete ADD Activo BIT NOT NULL DEFAULT 1;
GO

-- 2. Actualizar los datos existentes con los valores reales de CheTango
UPDATE TiposPaquete SET 
    NumeroClases = 4,
    Precio = 110000,
    DiasVigencia = 30,
    Descripcion = 'Paquete de 4 clases grupales con vigencia de 30 días',
    Activo = 1
WHERE Id = '11111111-1111-1111-1111-111111111111';

UPDATE TiposPaquete SET 
    NumeroClases = 8,
    Precio = 140000,
    DiasVigencia = 30,
    Descripcion = 'Paquete de 8 clases grupales con vigencia de 30 días',
    Activo = 1
WHERE Id = '22222222-2222-2222-2222-222222222222';

UPDATE TiposPaquete SET 
    NumeroClases = 12,
    Precio = 180000,
    DiasVigencia = 30,
    Descripcion = 'Paquete de 12 clases grupales con vigencia de 30 días',
    Activo = 1
WHERE Id = '33333333-3333-3333-3333-333333333333';

UPDATE TiposPaquete SET 
    NumeroClases = 0,
    Precio = 130000,
    DiasVigencia = 30,
    Descripcion = 'Mensualidad fija para Elenco Formativo (ilimitado)',
    Activo = 1
WHERE Id = '44444444-4444-4444-4444-444444444444';
GO

-- 3. Verificar resultado
SELECT Id, Nombre, NumeroClases, Precio, DiasVigencia, Activo
FROM TiposPaquete
ORDER BY Precio;
GO

PRINT 'Migración completada exitosamente';
GO

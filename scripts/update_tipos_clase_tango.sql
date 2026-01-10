-- Script para actualizar TipoClase con los tipos reales de la aplicación Chetango
-- Fecha: 2026-01-09

USE ChetangoDB_Dev;
GO

-- Limpiar tipos de clase existentes (si no tienen referencias)
DELETE FROM TiposClase;
GO

-- Insertar los tipos de clase reales de Chetango
INSERT INTO TiposClase (Id, Nombre) VALUES
(NEWID(), 'Tango Salon'),
(NEWID(), 'Tango Escenario'),
(NEWID(), 'Elencos Formativos'),
(NEWID(), 'Tango Salon Privado'),
(NEWID(), 'Tango Escenario Privado');
GO

-- Verificar los tipos insertados
SELECT Id, Nombre 
FROM TiposClase
ORDER BY Nombre;
GO

-- Script para actualizar TiposClase en PRODUCCIÓN
-- Actualiza los nombres de tipos existentes y agrega nuevos si es necesario
-- Base de datos: chetango-db-prod
-- Servidor: chetango-sql-prod.database.windows.net

USE [chetango-db-prod];
GO

-- Ver los tipos actuales antes de actualizar
PRINT 'Tipos actuales:';
SELECT Id, Nombre FROM TiposClase ORDER BY Nombre;
GO

-- OPCIÓN A: Actualizar nombres de los 6 primeros registros existentes
-- Ejecuta esto si quieres mantener los IDs existentes y solo cambiar nombres

DECLARE @TiposExistentes TABLE (Id UNIQUEIDENTIFIER, Nombre NVARCHAR(255), RowNum INT);

-- Obtener los IDs actuales ordenados
INSERT INTO @TiposExistentes (Id, Nombre, RowNum)
SELECT Id, Nombre, ROW_NUMBER() OVER (ORDER BY Nombre) as RowNum
FROM TiposClase;

-- Limpiar todos los registros si hay más de 6
DELETE FROM TiposClase WHERE Id NOT IN (SELECT TOP 6 Id FROM TiposClase ORDER BY Nombre);

-- Actualizar los 6 tipos principales
UPDATE tc SET Nombre = CASE RowNum
    WHEN 1 THEN 'Elenco Formativo'
    WHEN 2 THEN 'Tango Escenario'
    WHEN 3 THEN 'Tango Escenario Privado'
    WHEN 4 THEN 'Tango Salon Basico'
    WHEN 5 THEN 'Tango Salon Intermedio'
    WHEN 6 THEN 'Tango Salon Privado'
END
FROM TiposClase tc
INNER JOIN @TiposExistentes te ON tc.Id = te.Id
WHERE te.RowNum <= 6;

-- Si hay menos de 6, insertar los faltantes
DECLARE @Count INT;
SELECT @Count = COUNT(*) FROM TiposClase;

IF @Count < 1 INSERT INTO TiposClase (Id, Nombre) VALUES (NEWID(), 'Elenco Formativo');
IF @Count < 2 INSERT INTO TiposClase (Id, Nombre) VALUES (NEWID(), 'Tango Escenario');
IF @Count < 3 INSERT INTO TiposClase (Id, Nombre) VALUES (NEWID(), 'Tango Escenario Privado');
IF @Count < 4 INSERT INTO TiposClase (Id, Nombre) VALUES (NEWID(), 'Tango Salon Basico');
IF @Count < 5 INSERT INTO TiposClase (Id, Nombre) VALUES (NEWID(), 'Tango Salon Intermedio');
IF @Count < 6 INSERT INTO TiposClase (Id, Nombre) VALUES (NEWID(), 'Tango Salon Privado');

GO

-- Verificar resultados
SELECT Id, Nombre 
FROM TiposClase 
ORDER BY Nombre;
GO

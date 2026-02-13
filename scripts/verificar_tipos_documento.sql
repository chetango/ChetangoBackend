/********************************************************************************************************
 Script: verificar_tipos_documento.sql
 Objetivo: Verificar y crear si es necesario el tipo de documento "Cédula de Ciudadanía"
 
 Descripción:
   - Verifica qué tipos de documento existen en la base de datos
   - Crea "Cédula de Ciudadanía" si no existe
   - Verifica el GUID correcto que debe usar el sistema
*********************************************************************************************************/

USE [chetango-db-prod];
GO

PRINT '========================================';
PRINT 'Verificando tipos de documento...';
PRINT '========================================';
PRINT '';

-- Listar todos los tipos de documento existentes
PRINT 'Tipos de documento existentes:';
SELECT Id, Nombre, LEN(Nombre) AS Longitud
FROM TiposDocumento
ORDER BY Nombre;

PRINT '';
PRINT '========================================';

-- Verificar si existe "Cédula de Ciudadanía" exactamente
DECLARE @ExisteCedulaCiudadania INT;
SELECT @ExisteCedulaCiudadania = COUNT(*)
FROM TiposDocumento
WHERE Nombre = N'Cédula de Ciudadanía';

IF @ExisteCedulaCiudadania = 0
BEGIN
    PRINT '⚠️  NO existe "Cédula de Ciudadanía"';
    PRINT 'Creando tipo de documento...';
    
    -- Crear el tipo de documento con el GUID esperado
    IF NOT EXISTS (SELECT 1 FROM TiposDocumento WHERE Id = '11111111-1111-1111-1111-111111111111')
    BEGIN
        INSERT INTO TiposDocumento (Id, Nombre)
        VALUES ('11111111-1111-1111-1111-111111111111', N'Cédula de Ciudadanía');
        PRINT '✅ Tipo de documento "Cédula de Ciudadanía" creado con ID: 11111111-1111-1111-1111-111111111111';
    END
    ELSE
    BEGIN
        -- Si el GUID existe pero con otro nombre, actualizar
        UPDATE TiposDocumento
        SET Nombre = N'Cédula de Ciudadanía'
        WHERE Id = '11111111-1111-1111-1111-111111111111';
        PRINT '✅ Tipo de documento actualizado a "Cédula de Ciudadanía"';
    END
END
ELSE
BEGIN
    PRINT '✅ El tipo de documento "Cédula de Ciudadanía" YA EXISTE';
    
    -- Mostrar el GUID que tiene
    SELECT Id, Nombre
    FROM TiposDocumento
    WHERE Nombre = N'Cédula de Ciudadanía';
END

PRINT '';
PRINT '========================================';
PRINT 'Verificación completada';
PRINT '========================================';

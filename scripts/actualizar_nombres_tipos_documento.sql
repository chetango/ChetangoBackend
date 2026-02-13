/********************************************************************************************************
 Script: actualizar_nombres_tipos_documento.sql
 Objetivo: Actualizar nombres de tipos de documento de abreviaciones a nombres completos
 
 Descripción:
   - Actualiza "CC" a "Cédula de Ciudadanía"
   - Actualiza "CE" a "Cédula de Extranjería"
   - Actualiza "PAS" a "Pasaporte"
   - Mantiene "OID" para usuarios de Azure AD
*********************************************************************************************************/

USE [chetango-db-prod];
GO

PRINT '========================================';
PRINT 'Actualizando nombres de tipos de documento...';
PRINT '========================================';
PRINT '';

BEGIN TRANSACTION;

BEGIN TRY
    -- Actualizar CC a Cédula de Ciudadanía
    UPDATE TiposDocumento
    SET Nombre = N'Cédula de Ciudadanía'
    WHERE Id = '11111111-1111-1111-1111-111111111111' AND Nombre = 'CC';
    PRINT '✅ Actualizado: CC → Cédula de Ciudadanía';

    -- Actualizar CE a Cédula de Extranjería
    UPDATE TiposDocumento
    SET Nombre = N'Cédula de Extranjería'
    WHERE Id = '22222222-2222-2222-2222-222222222222' AND Nombre = 'CE';
    PRINT '✅ Actualizado: CE → Cédula de Extranjería';

    -- Actualizar PAS a Pasaporte
    UPDATE TiposDocumento
    SET Nombre = N'Pasaporte'
    WHERE Id = '33333333-3333-3333-3333-333333333333' AND Nombre = 'PAS';
    PRINT '✅ Actualizado: PAS → Pasaporte';

    -- OID se mantiene para usuarios de Azure AD (no se muestra en el frontend)
    PRINT 'ℹ️  OID se mantiene sin cambios (uso interno para Azure AD)';

    COMMIT TRANSACTION;
    
    PRINT '';
    PRINT '========================================';
    PRINT '✅ Actualización completada exitosamente';
    PRINT '========================================';
    PRINT '';
    
    -- Verificar cambios
    PRINT 'Tipos de documento después de la actualización:';
    SELECT Id, Nombre, LEN(Nombre) AS Longitud
    FROM TiposDocumento
    ORDER BY Nombre;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT '========================================';
    PRINT '❌ ERROR EN LA ACTUALIZACIÓN';
    PRINT '========================================';
    PRINT 'Error: ' + ERROR_MESSAGE();
    PRINT '';
    
    THROW;
END CATCH;

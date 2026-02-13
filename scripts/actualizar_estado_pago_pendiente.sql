/********************************************************************************************************
 Script: actualizar_estado_pago_pendiente.sql
 Objetivo: Actualizar el nombre del estado de pago de "Pendiente" a "Pendiente Verificación"
 
 Descripción:
   El código del sistema busca el estado "Pendiente Verificación" pero en la BD existe solo "Pendiente"
   Este script actualiza el nombre para que coincida con lo que espera el código
*********************************************************************************************************/

USE [chetango-db-prod];
GO

PRINT '========================================';
PRINT 'Actualizando estado de pago "Pendiente"...';
PRINT '========================================';
PRINT '';

BEGIN TRANSACTION;

BEGIN TRY
    -- Actualizar el nombre y descripción del estado
    UPDATE EstadosPago
    SET Nombre = 'Pendiente Verificación',
        Descripcion = 'Pago pendiente de verificación por administrador',
        FechaModificacion = SYSDATETIME(),
        UsuarioModificacion = 'SISTEMA'
    WHERE Id = '3aa86b45-6f16-435a-aa32-53d3b5d910e9' 
      AND Nombre = 'Pendiente';
    
    IF @@ROWCOUNT = 0
    BEGIN
        PRINT '⚠️  No se encontró el estado "Pendiente" con el ID especificado';
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT '✅ Estado actualizado: "Pendiente" → "Pendiente Verificación"';
    
    COMMIT TRANSACTION;
    
    PRINT '';
    PRINT '========================================';
    PRINT '✅ Actualización completada exitosamente';
    PRINT '========================================';
    PRINT '';
    
    -- Verificar el resultado
    PRINT 'Estados de pago actuales:';
    SELECT Id, Nombre, Descripcion, Activo
    FROM EstadosPago
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

-- ============================================
-- CORRECCIÓN DE CODIFICACIÓN - VERSIÓN SIMPLIFICADA
-- Solo corrige tabla Usuarios
-- ============================================

USE ChetangoDB_Dev;
GO

PRINT '=== CORRECCIÓN DE CARACTERES EN USUARIOS ===';
PRINT '';

BEGIN TRANSACTION;

BEGIN TRY

    PRINT '--- Estado ANTES de la corrección ---';
    SELECT NombreUsuario, Correo
    FROM Usuarios
    WHERE NombreUsuario LIKE '%Ã%'
    ORDER BY NombreUsuario;
    PRINT '';

    -- Correcciones en Usuarios.NombreUsuario
    UPDATE Usuarios SET NombreUsuario = REPLACE(NombreUsuario, 'Ã³', 'ó') WHERE NombreUsuario LIKE '%Ã³%';
    PRINT '✓ Ã³ → ó';
    
    UPDATE Usuarios SET NombreUsuario = REPLACE(NombreUsuario, 'Ã¡', 'á') WHERE NombreUsuario LIKE '%Ã¡%';
    PRINT '✓ Ã¡ → á';
    
    UPDATE Usuarios SET NombreUsuario = REPLACE(NombreUsuario, 'Ã©', 'é') WHERE NombreUsuario LIKE '%Ã©%';
    PRINT '✓ Ã© → é';
    
    UPDATE Usuarios SET NombreUsuario = REPLACE(NombreUsuario, 'Ã­', 'í') WHERE NombreUsuario LIKE '%Ã­%';
    PRINT '✓ Ã­ → í';
    
    UPDATE Usuarios SET NombreUsuario = REPLACE(NombreUsuario, 'Ãº', 'ú') WHERE NombreUsuario LIKE '%Ãº%';
    PRINT '✓ Ãº → ú';
    
    UPDATE Usuarios SET NombreUsuario = REPLACE(NombreUsuario, 'Ã±', 'ñ') WHERE NombreUsuario LIKE '%Ã±%';
    PRINT '✓ Ã± → ñ';
    
    UPDATE Usuarios SET NombreUsuario = REPLACE(NombreUsuario, 'Ã', 'Á') WHERE NombreUsuario LIKE '%Ã%';
    PRINT '✓ Mayúsculas corregidas';

    PRINT '';
    PRINT '--- Estado DESPUÉS de la corrección ---';
    SELECT NombreUsuario, Correo
    FROM Usuarios
    WHERE NombreUsuario IN (
        'Ana Zoraida Gómez',
        'María Alejandra López',
        'María Rodríguez',
        'Santiago Pérez'
    )
    ORDER BY NombreUsuario;

    COMMIT TRANSACTION;
    PRINT '';
    PRINT '✓✓✓ CORRECCIÓN COMPLETADA ✓✓✓';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT '❌ ERROR: ' + ERROR_MESSAGE();
    THROW;
END CATCH;

GO

-- ============================================
-- CORRECCIÓN DE CODIFICACIÓN DE CARACTERES
-- Script para corregir nombres con caracteres especiales corruptos
-- ============================================
-- PROBLEMA: SQL Server almacenó incorrectamente caracteres Unicode
-- porque los INSERTs originales no usaron el prefijo N'' para strings
-- 
-- CAUSA: Gómez → GÃ³mez, López → LÃ³pez, Pérez → PÃ©rez, etc.
-- SOLUCIÓN: Reemplazar secuencias corruptas con caracteres correctos
-- ============================================

USE ChetangoDB_Dev;
GO

PRINT '=== INICIANDO CORRECCIÓN DE CODIFICACIÓN DE CARACTERES ===';
PRINT '';

BEGIN TRANSACTION;

BEGIN TRY

    -- ============================================
    -- TABLA: Usuarios (NombreUsuario)
    -- ============================================
    PRINT '--- Corrigiendo tabla Usuarios ---';
    
    -- Ã³ → ó (Gómez, López, etc.)
    UPDATE Usuarios
    SET NombreUsuario = REPLACE(NombreUsuario, 'Ã³', 'ó')
    WHERE NombreUsuario LIKE '%Ã³%';
    PRINT '✓ Corregido: Ã³ → ó';

    -- Ã¡ → á (María, etc.)
    UPDATE Usuarios
    SET NombreUsuario = REPLACE(NombreUsuario, 'Ã¡', 'á')
    WHERE NombreUsuario LIKE '%Ã¡%';
    PRINT '✓ Corregido: Ã¡ → á';

    -- Ã© → é (Pérez, etc.)
    UPDATE Usuarios
    SET NombreUsuario = REPLACE(NombreUsuario, 'Ã©', 'é')
    WHERE NombreUsuario LIKE '%Ã©%';
    PRINT '✓ Corregido: Ã© → é';

    -- Ã­ → í (Rodríguez, etc.)
    UPDATE Usuarios
    SET NombreUsuario = REPLACE(NombreUsuario, 'Ã­', 'í')
    WHERE NombreUsuario LIKE '%Ã­%';
    PRINT '✓ Corregido: Ã­ → í';

    -- Ãº → ú (Raúl, etc.)
    UPDATE Usuarios
    SET NombreUsuario = REPLACE(NombreUsuario, 'Ãº', 'ú')
    WHERE NombreUsuario LIKE '%Ãº%';
    PRINT '✓ Corregido: Ãº → ú';

    -- Ã± → ñ (Muñoz, etc.)
    UPDATE Usuarios
    SET NombreUsuario = REPLACE(NombreUsuario, 'Ã±', 'ñ')
    WHERE NombreUsuario LIKE '%Ã±%';
    PRINT '✓ Corregido: Ã± → ñ';

    -- Ã‰ → É (ÉXITO, etc.)
    UPDATE Usuarios
    SET NombreUsuario = REPLACE(NombreUsuario, 'Ã‰', 'É')
    WHERE NombreUsuario LIKE '%Ã‰%';
    PRINT '✓ Corregido: Ã‰ → É';

    -- Ã" → Ó (LÓPEZ mayúscula)
    UPDATE Usuarios
    SET NombreUsuario = REPLACE(NombreUsuario, 'Ã"', 'Ó')
    WHERE NombreUsuario LIKE '%Ã"%';
    PRINT '✓ Corregido: Ã" → Ó';

    -- Ã → Á (MARÍA mayúscula)
    UPDATE Usuarios
    SET NombreUsuario = REPLACE(NombreUsuario, 'Ã', 'Á')
    WHERE NombreUsuario LIKE '%Ã%';
    PRINT '✓ Corregido: Ã → Á';

    PRINT '';
    
    -- ============================================
    -- TABLA: Profesores (si tiene campos de nombre)
    -- ============================================
    -- Nota: Si Profesores usa IdUsuario y no tiene campos propios de nombre,
    -- esto no es necesario. Ajustar según el modelo.
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Profesores') AND name = 'Nombre')
    BEGIN
        PRINT '--- Corrigiendo tabla Profesores ---';
        
        UPDATE Profesores
        SET Nombre = REPLACE(Nombre, 'Ã³', 'ó')
        WHERE Nombre LIKE '%Ã³%';
        
        UPDATE Profesores
        SET Nombre = REPLACE(Nombre, 'Ã¡', 'á')
        WHERE Nombre LIKE '%Ã¡%';
        
        UPDATE Profesores
        SET Nombre = REPLACE(Nombre, 'Ã©', 'é')
        WHERE Nombre LIKE '%Ã©%';
        
        UPDATE Profesores
        SET Nombre = REPLACE(Nombre, 'Ã­', 'í')
        WHERE Nombre LIKE '%Ã­%';
        
        UPDATE Profesores
        SET Nombre = REPLACE(Nombre, 'Ãº', 'ú')
        WHERE Nombre LIKE '%Ãº%';
        
        UPDATE Profesores
        SET Nombre = REPLACE(Nombre, 'Ã±', 'ñ')
        WHERE Nombre LIKE '%Ã±%';
        
        PRINT '✓ Tabla Profesores.Nombre corregida';
    END

    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Profesores') AND name = 'Apellido')
    BEGIN
        UPDATE Profesores
        SET Apellido = REPLACE(Apellido, 'Ã³', 'ó')
        WHERE Apellido LIKE '%Ã³%';
        
        UPDATE Profesores
        SET Apellido = REPLACE(Apellido, 'Ã¡', 'á')
        WHERE Apellido LIKE '%Ã¡%';
        
        UPDATE Profesores
        SET Apellido = REPLACE(Apellido, 'Ã©', 'é')
        WHERE Apellido LIKE '%Ã©%';
        
        UPDATE Profesores
        SET Apellido = REPLACE(Apellido, 'Ã­', 'í')
        WHERE Apellido LIKE '%Ã­%';
        
        UPDATE Profesores
        SET Apellido = REPLACE(Apellido, 'Ãº', 'ú')
        WHERE Apellido LIKE '%Ãº%';
        
        UPDATE Profesores
        SET Apellido = REPLACE(Apellido, 'Ã±', 'ñ')
        WHERE Apellido LIKE '%Ã±%';
        
        PRINT '✓ Tabla Profesores.Apellido corregida';
    END

    PRINT '';
    
    -- ============================================
    -- TABLA: Alumnos (si tiene campos de nombre)
    -- ============================================
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Alumnos') AND name = 'Nombre')
    BEGIN
        PRINT '--- Corrigiendo tabla Alumnos ---';
        
        UPDATE Alumnos
        SET Nombre = REPLACE(Nombre, 'Ã³', 'ó')
        WHERE Nombre LIKE '%Ã³%';
        
        UPDATE Alumnos
        SET Nombre = REPLACE(Nombre, 'Ã¡', 'á')
        WHERE Nombre LIKE '%Ã¡%';
        
        UPDATE Alumnos
        SET Nombre = REPLACE(Nombre, 'Ã©', 'é')
        WHERE Nombre LIKE '%Ã©%';
        
        UPDATE Alumnos
        SET Nombre = REPLACE(Nombre, 'Ã­', 'í')
        WHERE Nombre LIKE '%Ã­%';
        
        UPDATE Alumnos
        SET Nombre = REPLACE(Nombre, 'Ãº', 'ú')
        WHERE Nombre LIKE '%Ãº%';
        
        UPDATE Alumnos
        SET Nombre = REPLACE(Nombre, 'Ã±', 'ñ')
        WHERE Nombre LIKE '%Ã±%';
        
        PRINT '✓ Tabla Alumnos.Nombre corregida';
    END

    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Alumnos') AND name = 'Apellido')
    BEGIN
        UPDATE Alumnos
        SET Apellido = REPLACE(Apellido, 'Ã³', 'ó')
        WHERE Apellido LIKE '%Ã³%';
        
        UPDATE Alumnos
        SET Apellido = REPLACE(Apellido, 'Ã¡', 'á')
        WHERE Apellido LIKE '%Ã¡%';
        
        UPDATE Alumnos
        SET Apellido = REPLACE(Apellido, 'Ã©', 'é')
        WHERE Apellido LIKE '%Ã©%';
        
        UPDATE Alumnos
        SET Apellido = REPLACE(Apellido, 'Ã­', 'í')
        WHERE Apellido LIKE '%Ã­%';
        
        UPDATE Alumnos
        SET Apellido = REPLACE(Apellido, 'Ãº', 'ú')
        WHERE Apellido LIKE '%Ãº%';
        
        UPDATE Alumnos
        SET Apellido = REPLACE(Apellido, 'Ã±', 'ñ')
        WHERE Apellido LIKE '%Ã±%';
        
        PRINT '✓ Tabla Alumnos.Apellido corregida';
    END

    PRINT '';

    -- ============================================
    -- TABLA: Eventos (si tiene campos de texto)
    -- ============================================
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Eventos') AND name = 'Titulo')
    BEGIN
        PRINT '--- Corrigiendo tabla Eventos ---';
        
        UPDATE Eventos
        SET Titulo = REPLACE(Titulo, 'Ã³', 'ó')
        WHERE Titulo LIKE '%Ã³%';
        
        UPDATE Eventos
        SET Titulo = REPLACE(Titulo, 'Ã¡', 'á')
        WHERE Titulo LIKE '%Ã¡%';
        
        UPDATE Eventos
        SET Titulo = REPLACE(Titulo, 'Ã©', 'é')
        WHERE Titulo LIKE '%Ã©%';
        
        UPDATE Eventos
        SET Titulo = REPLACE(Titulo, 'Ã­', 'í')
        WHERE Titulo LIKE '%Ã­%';
        
        UPDATE Eventos
        SET Titulo = REPLACE(Titulo, 'Ãº', 'ú')
        WHERE Titulo LIKE '%Ãº%';
        
        UPDATE Eventos
        SET Titulo = REPLACE(Titulo, 'Ã±', 'ñ')
        WHERE Titulo LIKE '%Ã±%';
        
        PRINT '✓ Tabla Eventos.Titulo corregida';
    END

    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Eventos') AND name = 'Descripcion')
    BEGIN
        UPDATE Eventos
        SET Descripcion = REPLACE(Descripcion, 'Ã³', 'ó'),
            Descripcion = REPLACE(Descripcion, 'Ã¡', 'á'),
            Descripcion = REPLACE(Descripcion, 'Ã©', 'é'),
            Descripcion = REPLACE(Descripcion, 'Ã­', 'í'),
            Descripcion = REPLACE(Descripcion, 'Ãº', 'ú'),
            Descripcion = REPLACE(Descripcion, 'Ã±', 'ñ')
        WHERE Descripcion LIKE '%Ã%';
        
        PRINT '✓ Tabla Eventos.Descripcion corregida';
    END

    PRINT '';

    -- ============================================
    -- TABLA: Asistencias (si tiene observaciones)
    -- ============================================
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Asistencias') AND name = 'Observacion')
    BEGIN
        PRINT '--- Corrigiendo tabla Asistencias ---';
        
        UPDATE Asistencias
        SET Observacion = REPLACE(Observacion, 'Ã³', 'ó'),
            Observacion = REPLACE(Observacion, 'Ã¡', 'á'),
            Observacion = REPLACE(Observacion, 'Ã©', 'é'),
            Observacion = REPLACE(Observacion, 'Ã­', 'í'),
            Observacion = REPLACE(Observacion, 'Ãº', 'ú'),
            Observacion = REPLACE(Observacion, 'Ã±', 'ñ')
        WHERE Observacion LIKE '%Ã%';
        
        PRINT '✓ Tabla Asistencias.Observacion corregida';
    END

    PRINT '';

    -- ============================================
    -- VERIFICACIÓN DE RESULTADOS
    -- ============================================
    PRINT '--- Verificación de nombres corregidos ---';
    
    DECLARE @CountCorrupt INT;
    
    SELECT @CountCorrupt = COUNT(*)
    FROM Usuarios
    WHERE NombreUsuario LIKE '%Ã%';
    
    IF @CountCorrupt = 0
    BEGIN
        PRINT '✓ No se encontraron caracteres corruptos restantes en Usuarios.NombreUsuario';
    END
    ELSE
    BEGIN
        PRINT '⚠ Advertencia: Aún hay ' + CAST(@CountCorrupt AS NVARCHAR(10)) + ' registros con caracteres sospechosos';
        SELECT IdUsuario, NombreUsuario, Correo
        FROM Usuarios
        WHERE NombreUsuario LIKE '%Ã%'
        ORDER BY NombreUsuario;
    END

    PRINT '';
    PRINT '--- Muestra de nombres corregidos ---';
    SELECT TOP 10 
        IdUsuario, 
        NombreUsuario, 
        Correo
    FROM Usuarios
    WHERE NombreUsuario LIKE '%[óáéíúñ]%'
    ORDER BY NombreUsuario;

    PRINT '';
    PRINT '=== CORRECCIÓN COMPLETADA EXITOSAMENTE ===';
    
    COMMIT TRANSACTION;
    PRINT '✓ Transacción confirmada';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    PRINT '';
    PRINT '❌ ERROR DURANTE LA CORRECCIÓN:';
    PRINT 'Mensaje: ' + ERROR_MESSAGE();
    PRINT 'Línea: ' + CAST(ERROR_LINE() AS NVARCHAR(10));
    PRINT 'Procedimiento: ' + ISNULL(ERROR_PROCEDURE(), 'Script principal');
    PRINT '';
    PRINT '⚠ Transacción revertida, no se realizaron cambios';
    
    -- Re-lanzar el error para que sea visible
    THROW;
END CATCH;

GO

-- ============================================
-- CONSULTA FINAL: Ver nombres corregidos
-- ============================================
PRINT '';
PRINT '=== NOMBRES DE USUARIOS DESPUÉS DE LA CORRECCIÓN ===';
SELECT 
    NombreUsuario,
    Correo,
    Telefono
FROM Usuarios
WHERE NombreUsuario NOT IN ('Chetango Admin', 'Jorge Padilla', 'Juan David')
ORDER BY NombreUsuario;

-- =====================================================
-- ELIMINAR USUARIO COMPLETAMENTE (Sin registros asociados)
-- =====================================================
-- Usuario: james.idarraga@corporacionchetango.com
-- Fecha: 2026-02-21
-- Estado: Ya tiene borrado lógico, sin registros asociados
-- =====================================================

-- PASO 1: Verificar que el usuario existe
SELECT 
    IdUsuario,
    NombreUsuario,
    Correo,
    Sede,
    IdEstadoUsuario,
    FechaCreacion
FROM Usuarios
WHERE Correo = 'james.idarraga@corporacionchetango.com';

-- PASO 2: Verificar que NO tiene registros asociados (debe retornar todo en 0)
DECLARE @IdUsuario UNIQUEIDENTIFIER = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com');

SELECT 
    (SELECT COUNT(*) FROM Alumnos WHERE IdUsuario = @IdUsuario) AS Alumnos,
    (SELECT COUNT(*) FROM Profesores WHERE IdUsuario = @IdUsuario) AS Profesores,
    (SELECT COUNT(*) FROM Pagos WHERE IdUsuario = @IdUsuario) AS Pagos,
    (SELECT COUNT(*) FROM Notificaciones WHERE IdUsuario = @IdUsuario) AS Notificaciones;

-- PASO 3: ELIMINAR registro de Alumnos PRIMERO (respeta Foreign Key)
DELETE FROM Alumnos
WHERE IdUsuario = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com');

-- PASO 4: ELIMINAR registro de Profesores (si existe)
DELETE FROM Profesores
WHERE IdUsuario = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com');

-- PASO 5: AHORA SÍ, eliminar el usuario de la tabla Usuarios
DELETE FROM Usuarios
WHERE Correo = 'james.idarraga@corporacionchetango.com';

-- PASO 6: Verificar que el usuario fue eliminado (debe retornar 0)
SELECT COUNT(*) AS UsuarioExiste
FROM Usuarios
WHERE Correo = 'james.idarraga@corporacionchetango.com';

-- =====================================================
-- RESULTADO ESPERADO:
-- - PASO 1: Muestra 1 registro (el usuario existe)
-- - PASO 2: Muestra contadores de registros asociados
-- - PASO 3: Elimina registro de Alumnos
-- - PASO 4: Elimina registro de Profesores (si existe)
-- - PASO 5: Elimina el Usuario
-- - PASO 6: Retorna 0 (usuario ya no existe)
-- =====================================================
-- NOTA: Se debe eliminar en este orden por las Foreign Keys:
-- 1. Alumnos → referencia IdUsuario
-- 2. Profesores → referencia IdUsuario
-- 3. Usuarios → tabla principal
-- =====================================================

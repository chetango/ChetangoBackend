-- =====================================================
-- ELIMINAR USUARIO DE LA BASE DE DATOS
-- =====================================================
-- Usuario: james.idarraga@corporacionchetango.com
-- Fecha: 2026-02-21
-- =====================================================

-- PASO 1: Verificar que el usuario existe
SELECT 
    IdUsuario,
    NombreUsuario,
    Correo,
    Sede,
    IdEstadoUsuario AS EstadoActual,
    FechaCreacion
FROM Usuarios
WHERE Correo = 'james.idarraga@corporacionchetango.com';

-- PASO 2: Verificar registros relacionados (IMPORTANTE antes de eliminar)
DECLARE @IdUsuario UNIQUEIDENTIFIER = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com');

-- Ver si es alumno y tiene registros
SELECT 'Alumnos' AS Tabla, COUNT(*) AS Total
FROM Alumnos
WHERE IdUsuario = @IdUsuario
UNION ALL
-- Ver paquetes asociados
SELECT 'Paquetes', COUNT(*)
FROM Paquetes
WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos WHERE IdUsuario = @IdUsuario)
UNION ALL
-- Ver asistencias
SELECT 'Asistencias', COUNT(*)
FROM Asistencias
WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos WHERE IdUsuario = @IdUsuario)
UNION ALL
-- Ver pagos
SELECT 'Pagos', COUNT(*)
FROM Pagos
WHERE IdUsuario = @IdUsuario
UNION ALL
-- Ver si es profesor
SELECT 'Profesores', COUNT(*)
FROM Profesores
WHERE IdUsuario = @IdUsuario
UNION ALL
-- Ver clases como profesor
SELECT 'Clases', COUNT(*)
FROM Clases
WHERE IdProfesor IN (SELECT IdProfesor FROM Profesores WHERE IdUsuario = @IdUsuario)
UNION ALL
-- Ver notificaciones
SELECT 'Notificaciones', COUNT(*)
FROM Notificaciones
WHERE IdUsuario = @IdUsuario
UNION ALL
-- Ver auditorias
SELECT 'Auditorias', COUNT(*)
FROM Auditorias
WHERE IdUsuario = @IdUsuario;

-- =====================================================
-- OPCIÓN 1: DESACTIVAR USUARIO (RECOMENDADO)
-- =====================================================
-- Esta opción NO elimina datos, solo desactiva el acceso
-- Mantiene el historial completo

UPDATE Usuarios
SET IdEstadoUsuario = 2  -- Inactivo (verifica el ID correcto en tu tabla EstadoUsuario)
WHERE Correo = 'james.idarraga@corporacionchetango.com';

-- Verificar desactivación
SELECT 
    NombreUsuario,
    Correo,
    IdEstadoUsuario AS Estado,
    FechaCreacion
FROM Usuarios
WHERE Correo = 'james.idarraga@corporacionchetango.com';

-- =====================================================
-- OPCIÓN 2: ELIMINAR COMPLETAMENTE (PRECAUCIÓN)
-- =====================================================
-- ⚠️ ADVERTENCIA: Esta operación es IRREVERSIBLE
-- ⚠️ Solo ejecuta si estás SEGURO de que quieres perder todos los datos
-- ⚠️ Se eliminarán TODOS los registros relacionados

-- EJECUTAR EN ESTE ORDEN para respetar las Foreign Keys:

-- 1. Eliminar asistencias (si es alumno)
DELETE FROM Asistencias
WHERE IdAlumno IN (
    SELECT IdAlumno FROM Alumnos WHERE IdUsuario = 
    (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com')
);

-- 2. Eliminar paquetes (si es alumno)
DELETE FROM Paquetes
WHERE IdAlumno IN (
    SELECT IdAlumno FROM Alumnos WHERE IdUsuario = 
    (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com')
);

-- 3. Eliminar clases (si es profesor)
DELETE FROM Clases
WHERE IdProfesor IN (
    SELECT IdProfesor FROM Profesores WHERE IdUsuario = 
    (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com')
);

-- 4. Eliminar liquidaciones (si es profesor)
DELETE FROM LiquidacionesMensuales
WHERE IdProfesor IN (
    SELECT IdProfesor FROM Profesores WHERE IdUsuario = 
    (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com')
);

-- 5. Eliminar pagos
DELETE FROM Pagos
WHERE IdUsuario = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com');

-- 6. Eliminar notificaciones
DELETE FROM Notificaciones
WHERE IdUsuario = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com');

-- 7. Eliminar auditorías (opcional, considera mantenerlas)
-- DELETE FROM Auditorias
-- WHERE IdUsuario = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com');

-- 8. Eliminar registro de alumno
DELETE FROM Alumnos
WHERE IdUsuario = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com');

-- 9. Eliminar registro de profesor
DELETE FROM Profesores
WHERE IdUsuario = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'james.idarraga@corporacionchetango.com');

-- 10. FINALMENTE, eliminar el usuario
DELETE FROM Usuarios
WHERE Correo = 'james.idarraga@corporacionchetango.com';

-- =====================================================
-- PASO 3: Verificar que el usuario fue eliminado
SELECT COUNT(*) AS UsuarioExiste
FROM Usuarios
WHERE Correo = 'james.idarraga@corporacionchetango.com';
-- Debe retornar 0

-- =====================================================
-- RECOMENDACIÓN:
-- =====================================================
-- 👍 USA OPCIÓN 1 (Desactivar) si:
--    - El usuario tiene historial de pagos, clases o asistencias
--    - Necesitas mantener integridad de reportes financieros
--    - Puede que el usuario regrese en el futuro
--
-- ⚠️ USA OPCIÓN 2 (Eliminar) SOLO si:
--    - El usuario se creó por error recientemente
--    - NO tiene ningún registro asociado
--    - Estás 100% seguro de eliminar toda la información
--
-- 💡 Si el usuario tiene muchos registros, considera:
--    - Desactivar en lugar de eliminar
--    - Hacer backup de la base de datos antes de eliminar
--    - Documentar la razón de la eliminación
-- =====================================================

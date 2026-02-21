-- =====================================================
-- ACTUALIZAR SEDE DEL ADMINISTRADOR DE MANIZALES
-- =====================================================
-- Usuario: jhonathan.pachon@corporacionchetango.com
-- Sede: Manizales (Valor = 2)
-- Fecha: 2026-02-20
-- =====================================================

-- PASO 1: Verificar el usuario antes de actualizar
SELECT 
    IdUsuario,
    NombreUsuario,
    Correo,
    Sede AS SedeActual,
    CASE 
        WHEN Sede = 1 THEN 'Medellín'
        WHEN Sede = 2 THEN 'Manizales'
        ELSE 'Desconocido'
    END AS SedeNombre
FROM Usuarios
WHERE Correo = 'jhonathan.pachon@corporacionchetango.com';

-- PASO 2: Actualizar la sede a Manizales (2)
UPDATE Usuarios
SET Sede = 2  -- Manizales
WHERE Correo = 'jhonathan.pachon@corporacionchetango.com';

-- PASO 3: Verificar que la actualización se realizó correctamente
SELECT 
    IdUsuario,
    NombreUsuario,
    Correo,
    Sede AS SedeActual,
    CASE 
        WHEN Sede = 1 THEN 'Medellín'
        WHEN Sede = 2 THEN 'Manizales'
        ELSE 'Desconocido'
    END AS SedeNombre,
    FechaCreacion
FROM Usuarios
WHERE Correo = 'jhonathan.pachon@corporacionchetango.com';

-- PASO 4: Verificar si el usuario tiene registros asociados que hereden la sede
-- (Esto NO actualiza automáticamente registros existentes, solo muestra información)

-- Ver si tiene pagos registrados
SELECT COUNT(*) AS TotalPagos, Sede
FROM Pagos
WHERE IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo = 'jhonathan.pachon@corporacionchetango.com')
GROUP BY Sede;

-- Ver si es alumno
SELECT a.IdAlumno, a.IdUsuario, u.Correo, u.Sede
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
WHERE u.Correo = 'jhonathan.pachon@corporacionchetango.com';

-- Ver si es profesor
SELECT p.IdProfesor, p.IdUsuario, u.Correo, u.Sede
FROM Profesores p
INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
WHERE u.Correo = 'jhonathan.pachon@corporacionchetango.com';

-- =====================================================
-- NOTAS IMPORTANTES:
-- =====================================================
-- 1. Sede = 1 → Medellín
-- 2. Sede = 2 → Manizales
-- 3. Esta actualización solo cambia la sede del usuario
-- 4. Los registros FUTUROS que cree este usuario heredarán Sede = 2 (Manizales)
-- 5. Los registros EXISTENTES NO se actualizan automáticamente
-- 6. Si el usuario tiene pagos/clases creadas antes, esas mantienen su sede original
-- =====================================================

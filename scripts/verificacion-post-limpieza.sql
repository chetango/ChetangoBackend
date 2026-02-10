-- ============================================
-- VERIFICACIÓN POST-LIMPIEZA - CHETANGO
-- Ejecutar DESPUÉS de la limpieza para validar
-- ============================================

PRINT '🔍 INICIANDO VERIFICACIÓN DE BASE DE DATOS LIMPIA';
PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';

-- ============================================
-- CONTEO DE REGISTROS
-- ============================================

PRINT '';
PRINT '📊 CONTEO DE REGISTROS:';
PRINT '';

-- Crear tabla temporal para resultados
CREATE TABLE #Verificacion (
    Tabla VARCHAR(50),
    Cantidad INT,
    Esperado VARCHAR(20),
    Estado VARCHAR(10)
);

-- Usuarios
INSERT INTO #Verificacion
SELECT 'Usuarios', COUNT(*), '3', 
    CASE WHEN COUNT(*) = 3 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM Usuarios;

-- Profesores
INSERT INTO #Verificacion
SELECT 'Profesores', COUNT(*), '1', 
    CASE WHEN COUNT(*) = 1 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM Profesores;

-- Alumnos
INSERT INTO #Verificacion
SELECT 'Alumnos', COUNT(*), '1', 
    CASE WHEN COUNT(*) = 1 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM Alumnos;

-- Paquetes
INSERT INTO #Verificacion
SELECT 'Paquetes', COUNT(*), '0-1', 
    CASE WHEN COUNT(*) <= 1 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM Paquetes;

-- Clases (debe estar vacío)
INSERT INTO #Verificacion
SELECT 'Clases', COUNT(*), '0', 
    CASE WHEN COUNT(*) = 0 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM Clases;

-- Asistencias (debe estar vacío)
INSERT INTO #Verificacion
SELECT 'Asistencias', COUNT(*), '0', 
    CASE WHEN COUNT(*) = 0 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM Asistencias;

-- Liquidaciones (debe estar vacío)
INSERT INTO #Verificacion
SELECT 'Liquidaciones', COUNT(*), '0', 
    CASE WHEN COUNT(*) = 0 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM LiquidacionesMensuales;

-- Notificaciones (debe estar vacío)
INSERT INTO #Verificacion
SELECT 'Notificaciones', COUNT(*), '0', 
    CASE WHEN COUNT(*) = 0 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM Notificaciones;

-- Catálogos críticos
INSERT INTO #Verificacion
SELECT 'TiposProfesor', COUNT(*), '2', 
    CASE WHEN COUNT(*) = 2 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM TiposProfesor;

INSERT INTO #Verificacion
SELECT 'TarifasProfesor', COUNT(*), '4', 
    CASE WHEN COUNT(*) = 4 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM TarifasProfesor;

INSERT INTO #Verificacion
SELECT 'TiposAsistencia', COUNT(*), '4', 
    CASE WHEN COUNT(*) = 4 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM TiposAsistencia;

INSERT INTO #Verificacion
SELECT 'RolesEnClase', COUNT(*), '2', 
    CASE WHEN COUNT(*) = 2 THEN '✅ OK' ELSE '⚠️ ERROR' END
FROM RolesEnClase;

-- Mostrar resultados
SELECT * FROM #Verificacion ORDER BY Tabla;

-- Limpiar tabla temporal
DROP TABLE #Verificacion;

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';

-- ============================================
-- DETALLE DE USUARIOS MANTENIDOS
-- ============================================

PRINT '👤 USUARIOS MANTENIDOS (debe ser 3):';
PRINT '';

SELECT 
    IdUsuario,
    NombreUsuario,
    Correo,
    CASE 
        WHEN Correo LIKE '%Chetango@%' THEN '🔑 Admin'
        WHEN Correo LIKE '%Jorgepadilla@%' THEN '👨‍🏫 Profesor'
        WHEN Correo LIKE '%JuanDavid@%' THEN '👨‍🎓 Alumno'
        ELSE '❓ Desconocido'
    END AS Rol
FROM Usuarios
ORDER BY Correo;

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';

-- ============================================
-- DETALLE DE PROFESOR MANTENIDO
-- ============================================

PRINT '👨‍🏫 PROFESOR MANTENIDO (debe ser 1 - Jorge Padilla):';
PRINT '';

SELECT 
    p.IdProfesor,
    u.NombreUsuario,
    u.Correo,
    tp.Nombre AS TipoProfesor
FROM Profesores p
LEFT JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
LEFT JOIN TiposProfesor tp ON p.IdTipoProfesor = tp.IdTipoProfesor;

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';

-- ============================================
-- DETALLE DE ALUMNO MANTENIDO
-- ============================================

PRINT '👨‍🎓 ALUMNO MANTENIDO (debe ser 1 - Juan David):';
PRINT '';

SELECT 
    a.IdAlumno,
    u.NombreUsuario,
    u.Correo,
    ea.Nombre AS Estado,
    a.FechaInscripcion
FROM Alumnos a
LEFT JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
LEFT JOIN EstadosAlumno ea ON a.IdEstado = ea.IdEstado;

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';

-- ============================================
-- PAQUETES DEL ALUMNO (si existen)
-- ============================================

IF EXISTS (SELECT 1 FROM Paquetes)
BEGIN
    PRINT '📦 PAQUETE(S) DE JUAN DAVID:';
    PRINT '';
    
    SELECT 
        p.IdPaquete,
        tp.Nombre AS TipoPaquete,
        p.ClasesDisponibles,
        p.ClasesUsadas,
        (p.ClasesDisponibles - p.ClasesUsadas) AS ClasesRestantes,
        ep.Nombre AS Estado,
        p.FechaActivacion,
        p.FechaVencimiento
    FROM Paquetes p
    LEFT JOIN TiposPaquete tp ON p.IdTipoPaquete = tp.IdTipoPaquete
    LEFT JOIN EstadosPaquete ep ON p.IdEstado = ep.IdEstado
    WHERE p.IdAlumno = '295093d5-b36f-4737-b68a-ab40ca871b2e';
    
    PRINT '';
END
ELSE
BEGIN
    PRINT '📦 PAQUETES: Ninguno (tabla vacía)';
    PRINT '';
END

PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';

-- ============================================
-- VERIFICACIÓN DE CATÁLOGOS CRÍTICOS
-- ============================================

PRINT '📚 CATÁLOGOS CRÍTICOS (NO DEBEN ESTAR VACÍOS):';
PRINT '';

-- Tipos de Profesor
PRINT '  ↳ TIPOS DE PROFESOR:';
SELECT IdTipoProfesor, Nombre, Descripcion 
FROM TiposProfesor 
ORDER BY Nombre;

PRINT '';

-- Tarifas de Profesor
PRINT '  ↳ TARIFAS DE PROFESOR:';
SELECT 
    tp.Nombre AS TipoProfesor,
    r.Nombre AS RolEnClase,
    t.ValorHora
FROM TarifasProfesor t
LEFT JOIN TiposProfesor tp ON t.IdTipoProfesor = tp.IdTipoProfesor
LEFT JOIN RolesEnClase r ON t.IdRolEnClase = r.IdRolEnClase
ORDER BY tp.Nombre, r.Nombre;

PRINT '';

-- Tipos de Asistencia
PRINT '  ↳ TIPOS DE ASISTENCIA:';
SELECT 
    IdTipoAsistencia,
    Nombre,
    RequierePaquete,
    DescontarClase,
    Descripcion
FROM TiposAsistencia
ORDER BY Nombre;

PRINT '';

-- Roles en Clase
PRINT '  ↳ ROLES EN CLASE:';
SELECT IdRolEnClase, Nombre, Descripcion
FROM RolesEnClase
ORDER BY Nombre;

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';

-- ============================================
-- TIPOS DE CLASE Y PAQUETES (INFO)
-- ============================================

DECLARE @TiposClaseCount INT;
DECLARE @TiposPaqueteCount INT;

SELECT @TiposClaseCount = COUNT(*) FROM TiposClase;
SELECT @TiposPaqueteCount = COUNT(*) FROM TiposPaquete;

PRINT '📊 CATÁLOGOS CONFIGURABLES:';
PRINT '  ↳ Tipos de Clase: ' + CAST(@TiposClaseCount AS VARCHAR);
PRINT '  ↳ Tipos de Paquete: ' + CAST(@TiposPaqueteCount AS VARCHAR);

IF @TiposClaseCount > 0
BEGIN
    PRINT '';
    PRINT '  📋 TIPOS DE CLASE DISPONIBLES:';
    SELECT IdTipoClase, Nombre, Duracion, CupoMaximo 
    FROM TiposClase 
    WHERE Estado = 1
    ORDER BY Nombre;
END

IF @TiposPaqueteCount > 0
BEGIN
    PRINT '';
    PRINT '  📋 TIPOS DE PAQUETE DISPONIBLES:';
    SELECT IdTipoPaquete, Nombre, NumeroClases, Precio 
    FROM TiposPaquete 
    WHERE Estado = 1
    ORDER BY NumeroClases;
END

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';

-- ============================================
-- VALIDACIÓN FINAL
-- ============================================

DECLARE @ErrorCount INT = 0;

-- Verificar usuarios
IF (SELECT COUNT(*) FROM Usuarios) != 3
BEGIN
    PRINT '❌ ERROR: Usuarios incorrectos';
    SET @ErrorCount = @ErrorCount + 1;
END

-- Verificar profesor
IF (SELECT COUNT(*) FROM Profesores) != 1
BEGIN
    PRINT '❌ ERROR: Profesores incorrectos';
    SET @ErrorCount = @ErrorCount + 1;
END

-- Verificar alumno
IF (SELECT COUNT(*) FROM Alumnos) != 1
BEGIN
    PRINT '❌ ERROR: Alumnos incorrectos';
    SET @ErrorCount = @ErrorCount + 1;
END

-- Verificar clases vacías
IF (SELECT COUNT(*) FROM Clases) > 0
BEGIN
    PRINT '❌ ERROR: Aún hay clases en la BD';
    SET @ErrorCount = @ErrorCount + 1;
END

-- Verificar asistencias vacías
IF (SELECT COUNT(*) FROM Asistencias) > 0
BEGIN
    PRINT '❌ ERROR: Aún hay asistencias en la BD';
    SET @ErrorCount = @ErrorCount + 1;
END

-- Verificar catálogos críticos
IF (SELECT COUNT(*) FROM TiposProfesor) != 2
BEGIN
    PRINT '❌ ERROR: TiposProfesor no tiene 2 registros';
    SET @ErrorCount = @ErrorCount + 1;
END

IF (SELECT COUNT(*) FROM TarifasProfesor) != 4
BEGIN
    PRINT '❌ ERROR: TarifasProfesor no tiene 4 registros';
    SET @ErrorCount = @ErrorCount + 1;
END

IF (SELECT COUNT(*) FROM TiposAsistencia) != 4
BEGIN
    PRINT '❌ ERROR: TiposAsistencia no tiene 4 registros';
    SET @ErrorCount = @ErrorCount + 1;
END

IF @ErrorCount = 0
BEGIN
    PRINT '';
    PRINT '✅✅✅ VERIFICACIÓN EXITOSA ✅✅✅';
    PRINT '';
    PRINT 'La base de datos está correctamente limpia y lista para pruebas.';
    PRINT '';
    PRINT '🎯 Siguiente paso:';
    PRINT '   1. Crear tipos de clase (si no existen)';
    PRINT '   2. Crear tipos de paquete (si no existen)';
    PRINT '   3. Agregar más profesores si es necesario';
    PRINT '   4. Agregar más alumnos';
    PRINT '   5. Crear clases y programar horarios';
    PRINT '   6. Registrar asistencias';
    PRINT '   7. Procesar pagos de nómina';
END
ELSE
BEGIN
    PRINT '';
    PRINT '❌❌❌ VERIFICACIÓN FALLÓ ❌❌❌';
    PRINT '';
    PRINT 'Se encontraron ' + CAST(@ErrorCount AS VARCHAR) + ' errores.';
    PRINT 'Revisar el script de limpieza y ejecutar nuevamente.';
END

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';

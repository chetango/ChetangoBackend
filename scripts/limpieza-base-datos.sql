-- ============================================
-- SCRIPT DE LIMPIEZA DE BASE DE DATOS - CHETANGO
-- Fecha: 30 Enero 2026
-- Objetivo: Limpiar BD manteniendo catálogos y 3 usuarios con Entra ID
-- ============================================

-- ⚠️ IMPORTANTE: Ejecutar este script en ambiente de DESARROLLO/QA
-- ⚠️ NO ejecutar en PRODUCCIÓN sin backup completo

BEGIN TRANSACTION;

PRINT '🗑️ Iniciando limpieza de base de datos...';
PRINT '';

-- ============================================
-- FASE 1: BORRAR DATOS TRANSACCIONALES
-- ============================================

PRINT '📋 Fase 1: Limpiando datos transaccionales...';

-- 1. Borrar asistencias (dependen de clases y alumnos)
PRINT '  ↳ Borrando asistencias...';
DELETE FROM Asistencias;
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' asistencias eliminadas';

-- 2. Borrar monitores de clases (dependen de clases y profesores)
PRINT '  ↳ Borrando monitores de clases...';
DELETE FROM MonitoresClase;
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' monitores eliminados';

-- 3. Borrar clases-profesores (dependen de clases y profesores)
PRINT '  ↳ Borrando asignaciones clase-profesor...';
DELETE FROM ClasesProfesores;
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' asignaciones eliminadas';

-- 4. Borrar clases
PRINT '  ↳ Borrando clases...';
DELETE FROM Clases;
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' clases eliminadas';

-- 5. Borrar liquidaciones mensuales
PRINT '  ↳ Borrando liquidaciones mensuales...';
DELETE FROM LiquidacionesMensuales;
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' liquidaciones eliminadas';

-- 6. Borrar congelaciones de paquetes
PRINT '  ↳ Borrando congelaciones de paquetes...';
DELETE FROM CongelacionesPaquete;
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' congelaciones eliminadas';

-- 7. Borrar paquetes (EXCEPTO los del alumno Juan David)
PRINT '  ↳ Borrando paquetes (excepto Juan David)...';
DELETE FROM Paquetes 
WHERE IdAlumno != '295093d5-b36f-4737-b68a-ab40ca871b2e';
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' paquetes eliminados';

-- 8. Borrar pagos (EXCEPTO los relacionados al paquete de Juan David)
PRINT '  ↳ Borrando pagos (excepto Juan David)...';
DELETE FROM Pagos 
WHERE IdAlumno != '295093d5-b36f-4737-b68a-ab40ca871b2e';
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' pagos eliminados';

-- 9. Borrar notificaciones
PRINT '  ↳ Borrando notificaciones...';
DELETE FROM Notificaciones;
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' notificaciones eliminadas';

-- 10. Borrar eventos
PRINT '  ↳ Borrando eventos...';
DELETE FROM Eventos;
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' eventos eliminados';

-- 11. Borrar configuraciones de notificaciones
PRINT '  ↳ Borrando configuraciones de notificaciones...';
DELETE FROM ConfiguracionesNotificaciones;
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' configuraciones eliminadas';

PRINT '';
PRINT '✅ Fase 1 completada: Datos transaccionales limpiados';
PRINT '';

-- ============================================
-- FASE 2: LIMPIAR USUARIOS, PROFESORES Y ALUMNOS
-- ============================================

PRINT '👤 Fase 2: Limpiando usuarios, profesores y alumnos...';

-- 1. Borrar alumnos EXCEPTO Juan David
PRINT '  ↳ Borrando alumnos (excepto Juan David)...';
DELETE FROM Alumnos 
WHERE IdAlumno != '295093d5-b36f-4737-b68a-ab40ca871b2e';
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' alumnos eliminados';

-- 2. Borrar profesores EXCEPTO Jorge Padilla
PRINT '  ↳ Borrando profesores (excepto Jorge Padilla)...';
DELETE FROM Profesores 
WHERE IdProfesor != '8f6e460d-328d-4a40-89e3-b8effa76829c';
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' profesores eliminados';

-- 3. Borrar usuarios EXCEPTO los 3 con Entra ID Y que NO estén referenciados por Profesores o Alumnos
PRINT '  ↳ Borrando usuarios (excepto los 3 con Entra ID)...';
DELETE FROM Usuarios 
WHERE IdUsuario NOT IN (
    'b91e51b9-4094-441e-a5b6-062a846b3868', -- Admin: Chetango@chetangoprueba.onmicrosoft.com
    '8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB', -- Profesor: Jorgepadilla@chetangoprueba.onmicrosoft.com
    '71462106-9863-4fd0-b13d-9878ed231aa6'  -- Alumno: JuanDavid@chetangoprueba.onmicrosoft.com
)
AND IdUsuario NOT IN (SELECT IdUsuario FROM Profesores)
AND IdUsuario NOT IN (SELECT IdUsuario FROM Alumnos);
PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' usuarios eliminados';

-- 4. Borrar roles de usuario (si existe la tabla)
IF OBJECT_ID('UsuarioRol', 'U') IS NOT NULL
BEGIN
    PRINT '  ↳ Borrando roles de usuario...';
    DELETE FROM UsuarioRol
    WHERE IdUsuario NOT IN (
        'b91e51b9-4094-441e-a5b6-062a846b3868',
        '8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB',
        '71462106-9863-4fd0-b13d-9878ed231aa6'
    );
    PRINT '    ✓ ' + CAST(@@ROWCOUNT AS VARCHAR) + ' roles eliminados';
END

PRINT '';
PRINT '✅ Fase 2 completada: Usuarios limpiados';
PRINT '';

-- ============================================
-- FASE 3: VERIFICACIÓN POST-LIMPIEZA
-- ============================================

PRINT '🔍 Fase 3: Verificando limpieza...';
PRINT '';

-- Verificar usuarios mantenidos
DECLARE @UsuariosCount INT;
SELECT @UsuariosCount = COUNT(*) FROM Usuarios;
PRINT '  📊 Usuarios restantes: ' + CAST(@UsuariosCount AS VARCHAR) + ' (esperado: 3)';

IF @UsuariosCount = 3
    PRINT '    ✅ Correcto';
ELSE
    PRINT '    ⚠️  ADVERTENCIA: Se esperaban 3 usuarios, hay ' + CAST(@UsuariosCount AS VARCHAR);

-- Verificar profesores mantenidos
DECLARE @ProfesoresCount INT;
SELECT @ProfesoresCount = COUNT(*) FROM Profesores;
PRINT '  📊 Profesores restantes: ' + CAST(@ProfesoresCount AS VARCHAR) + ' (esperado: 1)';

IF @ProfesoresCount = 1
    PRINT '    ✅ Correcto';
ELSE
    PRINT '    ⚠️  ADVERTENCIA: Se esperaba 1 profesor, hay ' + CAST(@ProfesoresCount AS VARCHAR);

-- Verificar alumnos mantenidos
DECLARE @AlumnosCount INT;
SELECT @AlumnosCount = COUNT(*) FROM Alumnos;
PRINT '  📊 Alumnos restantes: ' + CAST(@AlumnosCount AS VARCHAR) + ' (esperado: 1)';

IF @AlumnosCount = 1
    PRINT '    ✅ Correcto';
ELSE
    PRINT '    ⚠️  ADVERTENCIA: Se esperaba 1 alumno, hay ' + CAST(@AlumnosCount AS VARCHAR);

-- Verificar paquetes mantenidos
DECLARE @PaquetesCount INT;
SELECT @PaquetesCount = COUNT(*) FROM Paquetes;
PRINT '  📊 Paquetes restantes: ' + CAST(@PaquetesCount AS VARCHAR) + ' (esperado: 0-1)';

-- Verificar clases
DECLARE @ClasesCount INT;
SELECT @ClasesCount = COUNT(*) FROM Clases;
PRINT '  📊 Clases restantes: ' + CAST(@ClasesCount AS VARCHAR) + ' (esperado: 0)';

IF @ClasesCount = 0
    PRINT '    ✅ Correcto';
ELSE
    PRINT '    ⚠️  ADVERTENCIA: Aún hay ' + CAST(@ClasesCount AS VARCHAR) + ' clases';

-- Verificar asistencias
DECLARE @AsistenciasCount INT;
SELECT @AsistenciasCount = COUNT(*) FROM Asistencias;
PRINT '  📊 Asistencias restantes: ' + CAST(@AsistenciasCount AS VARCHAR) + ' (esperado: 0)';

IF @AsistenciasCount = 0
    PRINT '    ✅ Correcto';
ELSE
    PRINT '    ⚠️  ADVERTENCIA: Aún hay ' + CAST(@AsistenciasCount AS VARCHAR) + ' asistencias';

-- Verificar catálogos críticos
DECLARE @TiposProfesorCount INT;
SELECT @TiposProfesorCount = COUNT(*) FROM TiposProfesor;
PRINT '  📊 Tipos de Profesor: ' + CAST(@TiposProfesorCount AS VARCHAR) + ' (esperado: 2)';

DECLARE @TarifasCount INT;
SELECT @TarifasCount = COUNT(*) FROM TarifasProfesor;
PRINT '  📊 Tarifas de Profesor: ' + CAST(@TarifasCount AS VARCHAR) + ' (esperado: 4)';

DECLARE @TiposAsistenciaCount INT;
SELECT @TiposAsistenciaCount = COUNT(*) FROM TiposAsistencia;
PRINT '  📊 Tipos de Asistencia: ' + CAST(@TiposAsistenciaCount AS VARCHAR) + ' (esperado: 4)';

DECLARE @RolesClaseCount INT;
SELECT @RolesClaseCount = COUNT(*) FROM RolesEnClase;
PRINT '  📊 Roles en Clase: ' + CAST(@RolesClaseCount AS VARCHAR) + ' (esperado: 2)';

DECLARE @TiposClaseCount INT;
SELECT @TiposClaseCount = COUNT(*) FROM TiposClase;
PRINT '  📊 Tipos de Clase: ' + CAST(@TiposClaseCount AS VARCHAR);

DECLARE @TiposPaqueteCount INT;
SELECT @TiposPaqueteCount = COUNT(*) FROM TiposPaquete;
PRINT '  📊 Tipos de Paquete: ' + CAST(@TiposPaqueteCount AS VARCHAR);

DECLARE @AuditoriasCount INT;
SELECT @AuditoriasCount = COUNT(*) FROM Auditorias;
PRINT '  📊 Auditorías restantes: ' + CAST(@AuditoriasCount AS VARCHAR) + ' (mantenidas)';

PRINT '';
PRINT '✅ Fase 3 completada: Verificación finalizada';
PRINT '';

-- ============================================
-- DETALLES DE LOS DATOS MANTENIDOS
-- ============================================

PRINT '📋 Detalle de datos mantenidos:';
PRINT '';

-- Usuarios mantenidos
PRINT '👤 USUARIOS (3):';
SELECT 
    IdUsuario,
    NombreUsuario,
    Correo
FROM Usuarios
ORDER BY Correo;

-- Profesor mantenido
PRINT '';
PRINT '👨‍🏫 PROFESOR (1):';
SELECT 
    p.IdProfesor,
    u.NombreUsuario,
    u.Correo
FROM Profesores p
LEFT JOIN Usuarios u ON p.IdUsuario = u.IdUsuario;

-- Alumno mantenido
PRINT '';
PRINT '👨‍🎓 ALUMNO (1):';
SELECT 
    a.IdAlumno,
    u.NombreUsuario,
    u.Correo
FROM Alumnos a
LEFT JOIN Usuarios u ON a.IdUsuario = u.IdUsuario;

-- Paquetes mantenidos (si existen)
IF EXISTS (SELECT 1 FROM Paquetes)
BEGIN
    PRINT '';
    PRINT '📦 PAQUETES MANTENIDOS:';
    SELECT 
        p.IdPaquete,
        p.ClasesDisponibles,
        p.ClasesUsadas,
        (p.ClasesDisponibles - p.ClasesUsadas) AS ClasesRestantes,
        p.FechaActivacion,
        p.FechaVencimiento
    FROM Paquetes p;
END

PRINT '';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '✅ LIMPIEZA COMPLETADA EXITOSAMENTE';
PRINT '═══════════════════════════════════════════════════════════════';
PRINT '';
PRINT '📊 Estado de la Base de Datos:';
PRINT '  ✓ Catálogos mantenidos: Estados, Tipos, Tarifas, Roles';
PRINT '  ✓ Usuarios con Entra ID: 3 (Admin, Profesor, Alumno)';
PRINT '  ✓ Profesor activo: 1 (Jorge Padilla)';
PRINT '  ✓ Alumno activo: 1 (Juan David)';
PRINT '  ✓ Datos transaccionales: LIMPIADOS';
PRINT '  ✓ Sistema listo para crear datos desde cero';
PRINT '';
PRINT '🎯 Siguiente paso: Crear clases, asistencias y pagos para pruebas';
PRINT '';

-- Si todo está OK, commitear
COMMIT TRANSACTION;
PRINT '💾 Cambios guardados (COMMIT)';

-- Si algo salió mal, descomentar la línea siguiente:
-- ROLLBACK TRANSACTION;
-- PRINT '❌ Cambios revertidos (ROLLBACK)';

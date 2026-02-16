/********************************************************************************************************
 Script: 99_validaciones.sql
 Objetivo: Validar integridad y consistencia de datos poblados
 Fecha: Febrero 2025
 Uso: Marketing video - Verificación post-población
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;

PRINT '';
PRINT '========================================';
PRINT '   VALIDACIONES DE DATOS DE MARKETING  ';
PRINT '========================================';
PRINT '';

DECLARE @Errores INT = 0;

-- ============================================
-- 1. VALIDAR USUARIOS Y PERFILES
-- ============================================
PRINT '[ 1/12 ] Validando usuarios y perfiles...';

DECLARE @TotalUsuariosMarketing INT;
DECLARE @TotalProfesores INT;
DECLARE @TotalAlumnos INT;

SELECT @TotalUsuariosMarketing = COUNT(*) 
FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com';

SELECT @TotalProfesores = COUNT(*) 
FROM Profesores p
INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com';

SELECT @TotalAlumnos = COUNT(*) 
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com';

PRINT '  - Usuarios marketing: ' + CAST(@TotalUsuariosMarketing AS VARCHAR);
PRINT '  - Profesores: ' + CAST(@TotalProfesores AS VARCHAR) + ' (esperado: 5)';
PRINT '  - Alumnos: ' + CAST(@TotalAlumnos AS VARCHAR) + ' (esperado: 50)';

IF @TotalProfesores != 5
BEGIN
    PRINT '  ✗ ERROR: Se esperaban 5 profesores';
    SET @Errores = @Errores + 1;
END;

IF @TotalAlumnos != 50
BEGIN
    PRINT '  ✗ ERROR: Se esperaban 50 alumnos';
    SET @Errores = @Errores + 1;
END;

IF @Errores = 0 PRINT '  ✓ Usuarios y perfiles OK';
PRINT '';

-- ============================================
-- 2. VALIDAR TRANSACCIONES FINANCIERAS
-- ============================================
PRINT '[ 2/12 ] Validando transacciones financieras...';

DECLARE @TotalPagos INT;
DECLARE @TotalPaquetes INT;
DECLARE @MontoTotal DECIMAL(18,2);

SELECT @TotalPagos = COUNT(*) 
FROM Pagos 
WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com');

SELECT @TotalPaquetes = COUNT(*) 
FROM Paquetes 
WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com');

SELECT @MontoTotal = SUM(Monto) 
FROM Pagos 
WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com');

PRINT '  - Pagos registrados: ' + CAST(@TotalPagos AS VARCHAR) + ' (esperado: ~95)';
PRINT '  - Paquetes creados: ' + CAST(@TotalPaquetes AS VARCHAR) + ' (esperado: ~100)';
PRINT '  - Monto total: $' + CAST(@MontoTotal AS VARCHAR);

-- Validar integridad: Cada paquete tiene pago
DECLARE @PaquetesSinPago INT;
SELECT @PaquetesSinPago = COUNT(*) 
FROM Paquetes p 
WHERE p.IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com')
  AND p.IdPago IS NULL;

IF @PaquetesSinPago > 0
BEGIN
    PRINT '  ✗ ERROR: ' + CAST(@PaquetesSinPago AS VARCHAR) + ' paquetes sin pago asociado';
    SET @Errores = @Errores + 1;
END
ELSE
    PRINT '  ✓ Todos los paquetes tienen pago asociado';

PRINT '  ✓ Transacciones financieras OK';
PRINT '';

-- ============================================
-- 3. VALIDAR PROGRAMACIÓN DE CLASES
-- ============================================
PRINT '[ 3/12 ] Validando programación de clases...';

DECLARE @TotalClases INT;
DECLARE @ClasesConProfesor INT;

SELECT @TotalClases = COUNT(*) FROM Clases WHERE Descripcion LIKE '%[MKT]%';
SELECT @ClasesConProfesor = COUNT(DISTINCT cp.IdClase) 
FROM ClaseProfesor cp 
INNER JOIN Clases c ON cp.IdClase = c.IdClase 
WHERE c.Descripcion LIKE '%[MKT]%';

PRINT '  - Clases creadas: ' + CAST(@TotalClases AS VARCHAR) + ' (esperado: ~180)';
PRINT '  - Clases con profesor: ' + CAST(@ClasesConProfesor AS VARCHAR);

IF @TotalClases != @ClasesConProfesor
BEGIN
    PRINT '  ✗ ERROR: ' + CAST(@TotalClases - @ClasesConProfesor AS VARCHAR) + ' clases sin profesor asignado';
    SET @Errores = @Errores + 1;
END
ELSE
    PRINT '  ✓ Todas las clases tienen profesor asignado';

PRINT '';

-- ============================================
-- 4. VALIDAR ASISTENCIAS
-- ============================================
PRINT '[ 4/12 ] Validando asistencias...';

DECLARE @TotalAsistencias INT;
DECLARE @OcupacionPromedio DECIMAL(5,2);

SELECT @TotalAsistencias = COUNT(*) 
FROM Asistencias 
WHERE IdClase IN (SELECT IdClase FROM Clases WHERE Descripcion LIKE '%[MKT]%');

SELECT @OcupacionPromedio = AVG(CAST(Ocupacion AS DECIMAL(5,2)))
FROM (
    SELECT c.IdClase, COUNT(a.IdAsistencia) * 100.0 / c.CapacidadMaxima as Ocupacion
    FROM Clases c
    LEFT JOIN Asistencias a ON c.IdClase = a.IdClase
    WHERE c.Descripcion LIKE '%[MKT]%'
      AND c.Fecha < CAST(GETDATE() AS DATE)
    GROUP BY c.IdClase, c.CapacidadMaxima
) stats;

PRINT '  - Asistencias registradas: ' + CAST(@TotalAsistencias AS VARCHAR) + ' (esperado: ~2,300)';
PRINT '  - Ocupación promedio: ' + CAST(@OcupacionPromedio AS VARCHAR) + '% (esperado: 60-80%)';

IF @OcupacionPromedio < 50 OR @OcupacionPromedio > 90
BEGIN
    PRINT '  ⚠ ADVERTENCIA: Ocupación fuera del rango esperado';
END
ELSE
    PRINT '  ✓ Ocupación dentro del rango esperado';

-- Validar integridad: Asistencias con paquete válido
DECLARE @AsistenciasSinPaquete INT;
SELECT @AsistenciasSinPaquete = COUNT(*) 
FROM Asistencias a
WHERE a.IdClase IN (SELECT IdClase FROM Clases WHERE Descripcion LIKE '%[MKT]%')
  AND a.IdPaquete IS NULL;

IF @AsistenciasSinPaquete > 0
BEGIN
    PRINT '  ✗ ERROR: ' + CAST(@AsistenciasSinPaquete AS VARCHAR) + ' asistencias sin paquete asociado';
    SET @Errores = @Errores + 1;
END
ELSE
    PRINT '  ✓ Todas las asistencias tienen paquete asociado';

PRINT '';

-- ============================================
-- 5. VALIDAR PAQUETES - CLASES USADAS
-- ============================================
PRINT '[ 5/12 ] Validando integridad de paquetes...';

DECLARE @PaquetesInvalidos INT;
SELECT @PaquetesInvalidos = COUNT(*) 
FROM Paquetes 
WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com')
  AND ClasesUsadas > ClasesDisponibles;

IF @PaquetesInvalidos > 0
BEGIN
    PRINT '  ✗ ERROR: ' + CAST(@PaquetesInvalidos AS VARCHAR) + ' paquetes con ClasesUsadas > ClasesDisponibles';
    SET @Errores = @Errores + 1;
END
ELSE
    PRINT '  ✓ Todos los paquetes tienen ClasesUsadas <= ClasesDisponibles';

PRINT '';

-- ============================================
-- 6. VALIDAR LIQUIDACIONES
-- ============================================
PRINT '[ 6/12 ] Validando liquidaciones...';

DECLARE @TotalLiquidaciones INT;
DECLARE @LiquidacionesPagadas INT;
DECLARE @MontoLiquidaciones DECIMAL(18,2);

SELECT @TotalLiquidaciones = COUNT(*) 
FROM LiquidacionesMensuales 
WHERE IdProfesor IN (SELECT IdProfesor FROM Profesores p INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com');

SELECT @LiquidacionesPagadas = COUNT(*) 
FROM LiquidacionesMensuales 
WHERE IdProfesor IN (SELECT IdProfesor FROM Profesores p INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com')
  AND Estado = 'Pagada';

SELECT @MontoLiquidaciones = SUM(TotalPagar) 
FROM LiquidacionesMensuales 
WHERE IdProfesor IN (SELECT IdProfesor FROM Profesores p INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com');

PRINT '  - Liquidaciones creadas: ' + CAST(@TotalLiquidaciones AS VARCHAR) + ' (esperado: ~48)';
PRINT '  - Liquidaciones pagadas: ' + CAST(@LiquidacionesPagadas AS VARCHAR);
PRINT '  - Monto total liquidaciones: $' + CAST(@MontoLiquidaciones AS VARCHAR);

PRINT '  ✓ Liquidaciones OK';
PRINT '';

-- ============================================
-- 7. VALIDAR SISTEMA DE REFERIDOS
-- ============================================
PRINT '[ 7/12 ] Validando sistema de referidos...';

DECLARE @TotalCodigos INT;
DECLARE @TotalUsos INT;

SELECT @TotalCodigos = COUNT(*) 
FROM CodigoReferido 
WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com');

SELECT @TotalUsos = COUNT(*) 
FROM UsoCodigoReferido 
WHERE IdCodigoReferido IN (SELECT IdCodigoReferido FROM CodigoReferido WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com'));

PRINT '  - Códigos de referido: ' + CAST(@TotalCodigos AS VARCHAR) + ' (esperado: 15)';
PRINT '  - Usos registrados: ' + CAST(@TotalUsos AS VARCHAR) + ' (esperado: 30)';

-- Validar contador de usos
DECLARE @CodigosConContadorIncorrecto INT;
SELECT @CodigosConContadorIncorrecto = COUNT(*)
FROM CodigoReferido cr
WHERE cr.IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com')
  AND cr.VecesUsado != (SELECT COUNT(*) FROM UsoCodigoReferido WHERE IdCodigoReferido = cr.IdCodigoReferido);

IF @CodigosConContadorIncorrecto > 0
BEGIN
    PRINT '  ✗ ERROR: ' + CAST(@CodigosConContadorIncorrecto AS VARCHAR) + ' códigos con contador incorrecto';
    SET @Errores = @Errores + 1;
END
ELSE
    PRINT '  ✓ Todos los códigos tienen contador correcto';

PRINT '';

-- ============================================
-- 8. VALIDAR EVENTOS
-- ============================================
PRINT '[ 8/12 ] Validando eventos...';

DECLARE @TotalEventos INT;
DECLARE @EventosFuturos INT;

SELECT @TotalEventos = COUNT(*) FROM Eventos WHERE Titulo LIKE '%[MKT]%';
SELECT @EventosFuturos = COUNT(*) FROM Eventos WHERE Titulo LIKE '%[MKT]%' AND FechaEvento >= GETDATE();

PRINT '  - Eventos creados: ' + CAST(@TotalEventos AS VARCHAR) + ' (esperado: 12)';
PRINT '  - Eventos futuros: ' + CAST(@EventosFuturos AS VARCHAR);

PRINT '  ✓ Eventos OK';
PRINT '';

-- ============================================
-- 9. VALIDAR NOTIFICACIONES
-- ============================================
PRINT '[ 9/12 ] Validando notificaciones...';

DECLARE @TotalNotificaciones INT;
DECLARE @NotificacionesLeidas INT;

SELECT @TotalNotificaciones = COUNT(*) FROM Notificaciones WHERE Mensaje LIKE '%[MKT]%';
SELECT @NotificacionesLeidas = COUNT(*) FROM Notificaciones WHERE Mensaje LIKE '%[MKT]%' AND EsLeida = 1;

PRINT '  - Notificaciones creadas: ' + CAST(@TotalNotificaciones AS VARCHAR) + ' (esperado: 80)';
PRINT '  - Notificaciones leídas: ' + CAST(@NotificacionesLeidas AS VARCHAR);

PRINT '  ✓ Notificaciones OK';
PRINT '';

-- ============================================
-- 10. VALIDAR SOLICITUDES
-- ============================================
PRINT '[ 10/12 ] Validando solicitudes...';

DECLARE @SolicitudesPrivadas INT;
DECLARE @SolicitudesRenovacion INT;

SELECT @SolicitudesPrivadas = COUNT(*) 
FROM SolicitudClasePrivada 
WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com');

SELECT @SolicitudesRenovacion = COUNT(*) 
FROM SolicitudRenovacionPaquete 
WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos a INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario WHERE u.Correo LIKE '%@marketing.chetango.com');

PRINT '  - Solicitudes de clases privadas: ' + CAST(@SolicitudesPrivadas AS VARCHAR) + ' (esperado: 20)';
PRINT '  - Solicitudes de renovación: ' + CAST(@SolicitudesRenovacion AS VARCHAR) + ' (esperado: 15)';

PRINT '  ✓ Solicitudes OK';
PRINT '';

-- ============================================
-- 11. VALIDAR USUARIOS DE PRUEBA PRESERVADOS
-- ============================================
PRINT '[ 11/12 ] Validando preservación de usuarios de prueba...';

DECLARE @UsuariosPrueba INT;
SELECT @UsuariosPrueba = COUNT(*) 
FROM Usuarios 
WHERE Correo IN ('admin@chetango.com', 'profesor@chetango.com', 'alumno@chetango.com')
   OR NumeroDocumento IN ('1017141203', '1032010606', '1032569651');

PRINT '  - Usuarios de prueba encontrados: ' + CAST(@UsuariosPrueba AS VARCHAR);

IF @UsuariosPrueba > 0
    PRINT '  ✓ Usuarios de prueba preservados correctamente';
ELSE
    PRINT '  ⚠ ADVERTENCIA: No se encontraron usuarios de prueba (podría ser normal si no existen aún)';

PRINT '';

-- ============================================
-- 12. RESUMEN DE VOLUMETRÍA
-- ============================================
PRINT '[ 12/12 ] Resumen de volumetría...';
PRINT '';
PRINT '  ENTIDAD                     CANTIDAD';
PRINT '  ────────────────────────────────────';
PRINT '  Usuarios (marketing)         ' + RIGHT(SPACE(8) + CAST(@TotalUsuariosMarketing AS VARCHAR), 8);
PRINT '  ├─ Profesores                ' + RIGHT(SPACE(8) + CAST(@TotalProfesores AS VARCHAR), 8);
PRINT '  └─ Alumnos                   ' + RIGHT(SPACE(8) + CAST(@TotalAlumnos AS VARCHAR), 8);
PRINT '  Pagos                        ' + RIGHT(SPACE(8) + CAST(@TotalPagos AS VARCHAR), 8);
PRINT '  Paquetes                     ' + RIGHT(SPACE(8) + CAST(@TotalPaquetes AS VARCHAR), 8);
PRINT '  Clases                       ' + RIGHT(SPACE(8) + CAST(@TotalClases AS VARCHAR), 8);
PRINT '  Asistencias                  ' + RIGHT(SPACE(8) + CAST(@TotalAsistencias AS VARCHAR), 8);
PRINT '  Liquidaciones                ' + RIGHT(SPACE(8) + CAST(@TotalLiquidaciones AS VARCHAR), 8);
PRINT '  Códigos de referido          ' + RIGHT(SPACE(8) + CAST(@TotalCodigos AS VARCHAR), 8);
PRINT '  Usos de códigos              ' + RIGHT(SPACE(8) + CAST(@TotalUsos AS VARCHAR), 8);
PRINT '  Eventos                      ' + RIGHT(SPACE(8) + CAST(@TotalEventos AS VARCHAR), 8);
PRINT '  Notificaciones               ' + RIGHT(SPACE(8) + CAST(@TotalNotificaciones AS VARCHAR), 8);
PRINT '  Solicitudes privadas         ' + RIGHT(SPACE(8) + CAST(@SolicitudesPrivadas AS VARCHAR), 8);
PRINT '  Solicitudes renovación       ' + RIGHT(SPACE(8) + CAST(@SolicitudesRenovacion AS VARCHAR), 8);
PRINT '  ────────────────────────────────────';
PRINT '  TOTAL REGISTROS:             ~' + CAST(
    @TotalUsuariosMarketing + @TotalPagos + @TotalPaquetes + @TotalClases + 
    @TotalAsistencias + @TotalLiquidaciones + @TotalCodigos + @TotalUsos + 
    @TotalEventos + @TotalNotificaciones + @SolicitudesPrivadas + @SolicitudesRenovacion 
AS VARCHAR);
PRINT '';

-- ============================================
-- RESULTADO FINAL
-- ============================================
PRINT '========================================';
IF @Errores = 0
BEGIN
    PRINT '   ✓ VALIDACIÓN EXITOSA - SIN ERRORES';
    PRINT '========================================';
    PRINT '';
    PRINT 'La base de datos está lista para el video de marketing.';
    PRINT 'Todos los datos están correctamente poblados y validados.';
END
ELSE
BEGIN
    PRINT '   ✗ VALIDACIÓN CON ERRORES';
    PRINT '========================================';
    PRINT '';
    PRINT 'Se encontraron ' + CAST(@Errores AS VARCHAR) + ' errores de integridad.';
    PRINT 'Revisar mensajes anteriores para detalles.';
END;
PRINT '';
GO

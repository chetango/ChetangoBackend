/********************************************************************************************************
 Script: 00_ejecutar_todo.sql
 Objetivo: Script maestro que ejecuta todos los scripts de población en orden
 Fecha: Febrero 2025
 Uso: Marketing video - Ejecución completa de toda la población de datos
 
 IMPORTANTE: 
 - Este script preserva los 3 usuarios de prueba existentes
 - Todos los datos de marketing tienen marcador [MKT] para identificación
 - Ejecutar solo en ChetangoDB_Dev
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT '';
PRINT '========================================';
PRINT '   POBLACIÓN DE DATOS DE MARKETING     ';
PRINT '   Preservando usuarios de prueba      ';
PRINT '========================================';
PRINT '';
PRINT 'Fecha ejecución: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT 'Base de datos: ChetangoDB_Dev';
PRINT '';

DECLARE @Inicio DATETIME = GETDATE();
DECLARE @ErrorOcurrido BIT = 0;

-- ============================================
-- FASE 1: CATÁLOGOS BASE
-- ============================================
BEGIN TRY
    PRINT '[ 1/9 ] Ejecutando: 01_catalogos_base.sql...';
    EXEC('EXEC(N''$(SQLCMDPATH)01_catalogos_base.sql'')');
    PRINT '✓ Catálogos base completados.';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '✗ ERROR en catálogos base: ' + ERROR_MESSAGE();
    SET @ErrorOcurrido = 1;
END CATCH;

IF @ErrorOcurrido = 1 GOTO Finalizar;

-- ============================================
-- FASE 2: USUARIOS Y PERFILES
-- ============================================
BEGIN TRY
    PRINT '[ 2/9 ] Ejecutando: 02_usuarios_y_perfiles.sql...';
    EXEC('EXEC(N''$(SQLCMDPATH)02_usuarios_y_perfiles.sql'')');
    PRINT '✓ Usuarios y perfiles completados.';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '✗ ERROR en usuarios y perfiles: ' + ERROR_MESSAGE();
    SET @ErrorOcurrido = 1;
END CATCH;

IF @ErrorOcurrido = 1 GOTO Finalizar;

-- ============================================
-- FASE 3: TRANSACCIONES FINANCIERAS
-- ============================================
BEGIN TRY
    PRINT '[ 3/9 ] Ejecutando: 03_transacciones_financieras.sql...';
    EXEC('EXEC(N''$(SQLCMDPATH)03_transacciones_financieras.sql'')');
    PRINT '✓ Transacciones financieras completadas.';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '✗ ERROR en transacciones financieras: ' + ERROR_MESSAGE();
    SET @ErrorOcurrido = 1;
END CATCH;

IF @ErrorOcurrido = 1 GOTO Finalizar;

-- ============================================
-- FASE 4: PROGRAMACIÓN DE CLASES
-- ============================================
BEGIN TRY
    PRINT '[ 4/9 ] Ejecutando: 04_programacion_clases.sql...';
    EXEC('EXEC(N''$(SQLCMDPATH)04_programacion_clases.sql'')');
    PRINT '✓ Programación de clases completada.';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '✗ ERROR en programación de clases: ' + ERROR_MESSAGE();
    SET @ErrorOcurrido = 1;
END CATCH;

IF @ErrorOcurrido = 1 GOTO Finalizar;

-- ============================================
-- FASE 5: ASISTENCIAS MASIVAS
-- ============================================
BEGIN TRY
    PRINT '[ 5/9 ] Ejecutando: 05_asistencias_masivas.sql...';
    EXEC('EXEC(N''$(SQLCMDPATH)05_asistencias_masivas.sql'')');
    PRINT '✓ Asistencias masivas completadas.';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '✗ ERROR en asistencias masivas: ' + ERROR_MESSAGE();
    SET @ErrorOcurrido = 1;
END CATCH;

IF @ErrorOcurrido = 1 GOTO Finalizar;

-- ============================================
-- FASE 6: LIQUIDACIONES MENSUALES
-- ============================================
BEGIN TRY
    PRINT '[ 6/9 ] Ejecutando: 06_liquidaciones_mensuales.sql...';
    EXEC('EXEC(N''$(SQLCMDPATH)06_liquidaciones_mensuales.sql'')');
    PRINT '✓ Liquidaciones mensuales completadas.';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '✗ ERROR en liquidaciones mensuales: ' + ERROR_MESSAGE();
    SET @ErrorOcurrido = 1;
END CATCH;

IF @ErrorOcurrido = 1 GOTO Finalizar;

-- ============================================
-- FASE 7: SISTEMA DE REFERIDOS
-- ============================================
BEGIN TRY
    PRINT '[ 7/9 ] Ejecutando: 07_sistema_referidos.sql...';
    EXEC('EXEC(N''$(SQLCMDPATH)07_sistema_referidos.sql'')');
    PRINT '✓ Sistema de referidos completado.';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '✗ ERROR en sistema de referidos: ' + ERROR_MESSAGE();
    SET @ErrorOcurrido = 1;
END CATCH;

IF @ErrorOcurrido = 1 GOTO Finalizar;

-- ============================================
-- FASE 8: EVENTOS Y NOTIFICACIONES
-- ============================================
BEGIN TRY
    PRINT '[ 8/9 ] Ejecutando: 08_eventos_y_notificaciones.sql...';
    EXEC('EXEC(N''$(SQLCMDPATH)08_eventos_y_notificaciones.sql'')');
    PRINT '✓ Eventos y notificaciones completados.';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '✗ ERROR en eventos y notificaciones: ' + ERROR_MESSAGE();
    SET @ErrorOcurrido = 1;
END CATCH;

IF @ErrorOcurrido = 1 GOTO Finalizar;

-- ============================================
-- FASE 9: SOLICITUDES
-- ============================================
BEGIN TRY
    PRINT '[ 9/9 ] Ejecutando: 09_solicitudes.sql...';
    EXEC('EXEC(N''$(SQLCMDPATH)09_solicitudes.sql'')');
    PRINT '✓ Solicitudes completadas.';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '✗ ERROR en solicitudes: ' + ERROR_MESSAGE();
    SET @ErrorOcurrido = 1;
END CATCH;

Finalizar:

DECLARE @Fin DATETIME = GETDATE();
DECLARE @Duracion INT = DATEDIFF(SECOND, @Inicio, @Fin);

PRINT '';
PRINT '========================================';
IF @ErrorOcurrido = 0
BEGIN
    PRINT '   ✓ POBLACIÓN COMPLETADA EXITOSAMENTE';
    PRINT '========================================';
    PRINT '';
    PRINT 'Tiempo total: ' + CAST(@Duracion AS VARCHAR) + ' segundos';
    PRINT '';
    PRINT 'Próximo paso: Ejecutar 99_validaciones.sql para verificar integridad';
END
ELSE
BEGIN
    PRINT '   ✗ POBLACIÓN INCOMPLETA CON ERRORES';
    PRINT '========================================';
    PRINT '';
    PRINT 'Revisar mensajes de error anteriores.';
END;
PRINT '';
GO

/********************************************************************************************************
 INSTRUCCIONES DE USO:
 
 OPCIÓN 1 - Ejecutar scripts individualmente en SSMS:
 1. Abrir cada script (01 a 09) en orden
 2. Ejecutar uno por uno
 3. Verificar mensajes de éxito
 
 OPCIÓN 2 - Usar sqlcmd (línea de comandos):
 sqlcmd -S localhost -d ChetangoDB_Dev -E -i "01_catalogos_base.sql"
 sqlcmd -S localhost -d ChetangoDB_Dev -E -i "02_usuarios_y_perfiles.sql"
 ... continuar con los demás
 
 OPCIÓN 3 - Ejecutar este script maestro modificado:
 Como EXEC() no puede ejecutar archivos externos, ejecutar manualmente cada script en SSMS
 
 VERIFICACIÓN:
 Después de completar, ejecutar: 99_validaciones.sql
*********************************************************************************************************/

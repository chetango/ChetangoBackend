-- ===========================================================================
-- SCRIPT DE REPARACIÓN MULTI-TENANCY - PRODUCCIÓN
-- Fecha: 2026-03-05
-- Problema: Registros creados con TenantId = NULL no son visibles
-- Causa: Los siguientes handlers NO asignaban TenantId al crear entidades:
--   - CrearClaseCommandHandler
--   - CrearPaqueteCommandHandler
--   - CrearOtroGastoCommandHandler
--   - CrearOtroIngresoCommandHandler
--   - RegistrarPagoCommandHandler  (Pago + Paquete)
--   - RegistrarAsistenciaCommandHandler
--   - LiquidarMesHandler
--   - CreateEventoHandler
--   - SolicitarClasePrivadaHandler
--   - SolicitarRenovacionPaqueteHandler
-- ===========================================================================
-- PREREQUISITO: La migración migration-prod-20260304.sql debe estar aplicada
-- antes de ejecutar este script (columnas TenantId deben existir en todas las tablas)
-- ===========================================================================

DECLARE @TenantId uniqueidentifier = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890';

BEGIN TRANSACTION;

-- ===========================================================================
-- PASO 1: DIAGNÓSTICO - Ver cuántos registros tienen TenantId NULL
-- ===========================================================================
SELECT 'Usuarios con TenantId NULL'                  AS Tabla, COUNT(*) AS Cantidad FROM Usuarios                  WHERE TenantId IS NULL
UNION ALL
SELECT 'Alumnos con TenantId NULL'                   AS Tabla, COUNT(*) AS Cantidad FROM Alumnos                   WHERE TenantId IS NULL
UNION ALL
SELECT 'Profesores con TenantId NULL'                AS Tabla, COUNT(*) AS Cantidad FROM Profesores                WHERE TenantId IS NULL
UNION ALL
SELECT 'Clases con TenantId NULL'                    AS Tabla, COUNT(*) AS Cantidad FROM Clases                    WHERE TenantId IS NULL
UNION ALL
SELECT 'Paquetes con TenantId NULL'                  AS Tabla, COUNT(*) AS Cantidad FROM Paquetes                  WHERE TenantId IS NULL
UNION ALL
SELECT 'Pagos con TenantId NULL'                     AS Tabla, COUNT(*) AS Cantidad FROM Pagos                     WHERE TenantId IS NULL
UNION ALL
SELECT 'Asistencias con TenantId NULL'               AS Tabla, COUNT(*) AS Cantidad FROM Asistencias               WHERE TenantId IS NULL
UNION ALL
SELECT 'OtrosGastos con TenantId NULL'               AS Tabla, COUNT(*) AS Cantidad FROM OtrosGastos               WHERE TenantId IS NULL
UNION ALL
SELECT 'OtrosIngresos con TenantId NULL'             AS Tabla, COUNT(*) AS Cantidad FROM OtrosIngresos             WHERE TenantId IS NULL
UNION ALL
SELECT 'LiquidacionesMensuales con TenantId NULL'    AS Tabla, COUNT(*) AS Cantidad FROM LiquidacionesMensuales    WHERE TenantId IS NULL
UNION ALL
SELECT 'Eventos con TenantId NULL'                   AS Tabla, COUNT(*) AS Cantidad FROM Eventos                   WHERE TenantId IS NULL
UNION ALL
SELECT 'SolicitudesClasePrivada con TenantId NULL'   AS Tabla, COUNT(*) AS Cantidad FROM SolicitudesClasePrivada   WHERE TenantId IS NULL
UNION ALL
SELECT 'SolicitudesRenovacionPaquete con TenantId NULL' AS Tabla, COUNT(*) AS Cantidad FROM SolicitudesRenovacionPaquete WHERE TenantId IS NULL;

-- ===========================================================================
-- PASO 2: VERIFICAR EL USUARIO AFECTADO
-- ===========================================================================
SELECT 
    u.IdUsuario,
    u.NombreUsuario,
    u.Correo,
    u.TenantId AS TenantId_Usuario,
    tu.Id AS TenantUserId,
    tu.TenantId AS TenantId_TenantUsers,
    tu.Activo
FROM Usuarios u
LEFT JOIN TenantUsers tu ON tu.IdUsuario = u.IdUsuario AND tu.TenantId = @TenantId
WHERE u.Correo = 'jhonathan.pachon@corporacionchetango.com';

-- ===========================================================================
-- PASO 3: CORREGIR TenantId NULL en todas las tablas (solo registros de este tenant)
-- Seguro: solo afecta registros que ya deberían pertenecer a Corporación Chetango
-- ===========================================================================
UPDATE Clases                    SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE Paquetes                  SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE Pagos                     SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE Asistencias               SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE Alumnos                   SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE Usuarios                  SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE Profesores                SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE OtrosGastos               SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE OtrosIngresos             SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE LiquidacionesMensuales    SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE Eventos                   SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE SolicitudesClasePrivada   SET TenantId = @TenantId WHERE TenantId IS NULL;
UPDATE SolicitudesRenovacionPaquete SET TenantId = @TenantId WHERE TenantId IS NULL;

SELECT 'Registros corregidos en todas las tablas' AS Resultado;

-- ===========================================================================
-- PASO 4: POBLAR TenantUsers para todos los usuarios que no tienen registro
-- Necesario para el fallback de resolución de tenant en OnTokenValidated
-- ===========================================================================
INSERT INTO TenantUsers (Id, TenantId, IdUsuario, FechaAsignacion, Activo)
SELECT 
    NEWID(),
    @TenantId,
    u.IdUsuario,
    GETDATE(),
    1
FROM Usuarios u
WHERE u.TenantId = @TenantId
  AND NOT EXISTS (
      SELECT 1 FROM TenantUsers tu 
      WHERE tu.IdUsuario = u.IdUsuario 
        AND tu.TenantId = @TenantId
  );

SELECT 
    'TenantUsers insertados: ' + CAST(@@ROWCOUNT AS nvarchar) AS Resultado;

-- ===========================================================================
-- PASO 5: VERIFICACIÓN FINAL
-- ===========================================================================
SELECT 
    u.Correo,
    u.TenantId AS TenantId_OK,
    tu.Activo AS TenantUser_Activo
FROM Usuarios u
INNER JOIN TenantUsers tu ON tu.IdUsuario = u.IdUsuario AND tu.TenantId = @TenantId
WHERE u.Correo = 'jhonathan.pachon@corporacionchetango.com';

SELECT 'TenantUsers total' AS Tabla, COUNT(*) AS Cantidad FROM TenantUsers WHERE TenantId = @TenantId;

COMMIT;
GO

-- =====================================================
-- Script de Sincronización: Producción → Desarrollo
-- Fecha: 2026-02-11
-- Objetivo: Actualizar GUIDs de EstadosPago para que coincidan con desarrollo
-- =====================================================

BEGIN TRANSACTION;
GO

-- 1. Verificar estado actual antes de cambios
PRINT '=== ESTADO ACTUAL EN PRODUCCIÓN ===';
SELECT Id, Nombre FROM EstadosPago ORDER BY Nombre;
SELECT COUNT(*) as TotalPagos, IdEstadoPago 
FROM Pagos 
GROUP BY IdEstadoPago;
GO

-- 2. Deshabilitar FK constraint temporalmente
ALTER TABLE Pagos NOCHECK CONSTRAINT ALL;
GO

-- 3. Actualizar FKs en Pagos para apuntar a nuevos GUIDs de desarrollo
PRINT '=== ACTUALIZANDO FKs EN PAGOS ===';

-- Mapeo: Rechazado
UPDATE Pagos 
SET IdEstadoPago = 'C35AF436-CC70-4B29-8FDA-0D74C477B12B'
WHERE IdEstadoPago = '3dba304c-4bb1-4456-9836-3b2d6661a508';
PRINT 'Pagos con estado Rechazado actualizados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- Mapeo: Pendiente → Pendiente Verificación
UPDATE Pagos 
SET IdEstadoPago = '08EDFDA2-A022-4C45-89BD-CA7AB20668F0'
WHERE IdEstadoPago = '3aa86b45-6f16-435a-aa32-53d3b5d910e9';
PRINT 'Pagos con estado Pendiente actualizados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- Mapeo: Verificado
UPDATE Pagos 
SET IdEstadoPago = '60D317B2-6C9F-4D16-9201-07D204F78DFC'
WHERE IdEstadoPago = '5e35cae1-0990-49d8-8760-a3e593738bce';
PRINT 'Pagos con estado Verificado actualizados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

GO

-- 4. Eliminar registros antiguos de EstadosPago
PRINT '=== ELIMINANDO REGISTROS ANTIGUOS DE ESTADOSPAGO ===';
DELETE FROM EstadosPago WHERE Id = '3dba304c-4bb1-4456-9836-3b2d6661a508'; -- Rechazado (antiguo)
DELETE FROM EstadosPago WHERE Id = '3aa86b45-6f16-435a-aa32-53d3b5d910e9'; -- Pendiente (antiguo)
DELETE FROM EstadosPago WHERE Id = '5e35cae1-0990-49d8-8760-a3e593738bce'; -- Verificado (antiguo)
PRINT 'Registros antiguos eliminados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
GO

-- 5. Insertar registros con GUIDs de desarrollo
PRINT '=== INSERTANDO REGISTROS CON GUIDs DE DESARROLLO ===';

-- Verificar si ya existen (por si se ejecuta dos veces)
IF NOT EXISTS (SELECT 1 FROM EstadosPago WHERE Id = '08EDFDA2-A022-4C45-89BD-CA7AB20668F0')
BEGIN
    INSERT INTO EstadosPago (Id, Nombre, Descripcion) 
    VALUES ('08EDFDA2-A022-4C45-89BD-CA7AB20668F0', 'Pendiente Verificación', 'Pago pendiente de verificación');
    PRINT 'Estado Pendiente Verificación insertado';
END

IF NOT EXISTS (SELECT 1 FROM EstadosPago WHERE Id = 'C35AF436-CC70-4B29-8FDA-0D74C477B12B')
BEGIN
    INSERT INTO EstadosPago (Id, Nombre, Descripcion) 
    VALUES ('C35AF436-CC70-4B29-8FDA-0D74C477B12B', 'Rechazado', 'Pago rechazado');
    PRINT 'Estado Rechazado insertado';
END

IF NOT EXISTS (SELECT 1 FROM EstadosPago WHERE Id = '60D317B2-6C9F-4D16-9201-07D204F78DFC')
BEGIN
    INSERT INTO EstadosPago (Id, Nombre, Descripcion) 
    VALUES ('60D317B2-6C9F-4D16-9201-07D204F78DFC', 'Verificado', 'Pago verificado');
    PRINT 'Estado Verificado insertado';
END
GO

-- 6. Rehabilitar FK constraints
ALTER TABLE Pagos WITH CHECK CHECK CONSTRAINT ALL;
PRINT 'Constraints FK rehabilitados';
GO

-- 7. Agregar migración a historial de EF Core
PRINT '=== ACTUALIZANDO __EFMigrationsHistory ===';

IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260211070714_AgregarColumnasTiposPaqueteYPagos_Safe')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260211070714_AgregarColumnasTiposPaqueteYPagos_Safe', '9.0.1');
    PRINT 'Migración 20260211070714_AgregarColumnasTiposPaqueteYPagos_Safe registrada';
END
ELSE
BEGIN
    PRINT 'Migración ya existe en historial';
END
GO

-- 8. Verificar estado final
PRINT '=== ESTADO FINAL EN PRODUCCIÓN ===';
SELECT Id, Nombre FROM EstadosPago ORDER BY Nombre;
SELECT COUNT(*) as TotalPagos, IdEstadoPago 
FROM Pagos 
GROUP BY IdEstadoPago;
GO

-- 9. Commit si todo está OK
PRINT '=== SINCRONIZACIÓN COMPLETADA ===';
PRINT 'Revisa los resultados. Si todo está correcto, ejecuta: COMMIT TRANSACTION';
PRINT 'Si algo salió mal, ejecuta: ROLLBACK TRANSACTION';

-- Descomentar la línea siguiente para auto-commit (NO RECOMENDADO en primera ejecución)
-- COMMIT TRANSACTION;

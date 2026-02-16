/********************************************************************************************************
 Script: 01_catalogos_base.sql
 Objetivo: Poblar catálogos base del sistema sin afectar usuarios de prueba existentes
 Fecha: Febrero 2025
 Uso: Marketing video - Datos realistas para demostración
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT 'Iniciando población de catálogos base...';

BEGIN TRANSACTION;

-- ============================================
-- 1. TiposDocumento (si no existen)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM TiposDocumento WHERE Id = '11111111-1111-1111-1111-111111111111')
    INSERT INTO TiposDocumento (Id, Nombre) VALUES ('11111111-1111-1111-1111-111111111111', 'Cédula de Ciudadanía');

IF NOT EXISTS (SELECT 1 FROM TiposDocumento WHERE Id = '22222222-2222-2222-2222-222222222222')
    INSERT INTO TiposDocumento (Id, Nombre) VALUES ('22222222-2222-2222-2222-222222222222', 'Cédula de Extranjería');

IF NOT EXISTS (SELECT 1 FROM TiposDocumento WHERE Id = '33333333-3333-3333-3333-333333333333')
    INSERT INTO TiposDocumento (Id, Nombre) VALUES ('33333333-3333-3333-3333-333333333333', 'Pasaporte');

IF NOT EXISTS (SELECT 1 FROM TiposDocumento WHERE Id = '44444444-4444-4444-4444-444444444444')
    INSERT INTO TiposDocumento (Id, Nombre) VALUES ('44444444-4444-4444-4444-444444444444', 'Tarjeta de Identidad');

PRINT 'TiposDocumento: OK';

-- ============================================
-- 2. Estados (verificar que existan - no crear duplicados)
-- ============================================
-- Los estados ya existen en las tablas: EstadosAlumno, EstadosAsistencia, EstadosUsuario
-- No se modifican para preservar integridad

PRINT 'Estados existentes: OK';

-- ============================================
-- 3. EstadosAsistencia (verificar existentes)
-- ============================================
-- Ya existen: Presente (1), Ausente (2), Justificada (3)
IF NOT EXISTS (SELECT 1 FROM EstadosAsistencia WHERE Id = 4)
    INSERT INTO EstadosAsistencia (Id, Nombre) VALUES (4, 'Tarde');

PRINT 'EstadosAsistencia: OK';

-- ============================================
-- 4. MetodosPago
-- ============================================
IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Id = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')
    INSERT INTO MetodosPago (Id, Nombre) VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Efectivo');

IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Id = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb')
    INSERT INTO MetodosPago (Id, Nombre) VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Transferencia');

IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Id = 'cccccccc-cccc-cccc-cccc-cccccccccccc')
    INSERT INTO MetodosPago (Id, Nombre) VALUES ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Tarjeta');

IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Id = 'dddddddd-dddd-dddd-dddd-dddddddddddd')
    INSERT INTO MetodosPago (Id, Nombre) VALUES ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Nequi');

IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Id = 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee')
    INSERT INTO MetodosPago (Id, Nombre) VALUES ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Daviplata');

PRINT 'MetodosPago: OK';

-- ============================================
-- 5. TiposClase
-- ============================================
IF NOT EXISTS (SELECT 1 FROM TiposClase WHERE Id = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')
    INSERT INTO TiposClase (Id, Nombre) VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Grupal');

IF NOT EXISTS (SELECT 1 FROM TiposClase WHERE Id = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb')
    INSERT INTO TiposClase (Id, Nombre) VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Privada');

IF NOT EXISTS (SELECT 1 FROM TiposClase WHERE Id = 'cccccccc-cccc-cccc-cccc-cccccccccccc')
    INSERT INTO TiposClase (Id, Nombre) VALUES ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Semi-privada');

PRINT 'TiposClase: OK';

-- ============================================
-- 6. TiposPaquete
-- ============================================
IF NOT EXISTS (SELECT 1 FROM TiposPaquete WHERE Id = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')
    INSERT INTO TiposPaquete (Id, Nombre, Descripcion) VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Básico', 'Paquete básico de clases grupales');

IF NOT EXISTS (SELECT 1 FROM TiposPaquete WHERE Id = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb')
    INSERT INTO TiposPaquete (Id, Nombre, Descripcion) VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Premium', 'Paquete premium con beneficios');

IF NOT EXISTS (SELECT 1 FROM TiposPaquete WHERE Id = 'cccccccc-cccc-cccc-cccc-cccccccccccc')
    INSERT INTO TiposPaquete (Id, Nombre, Descripcion) VALUES ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Privado', 'Clases privadas personalizadas');

PRINT 'TiposPaquete: OK';

-- ============================================
-- 7. TiposProfesor
-- ============================================
IF NOT EXISTS (SELECT 1 FROM TiposProfesor WHERE Id = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')
    INSERT INTO TiposProfesor (Id, Nombre) VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Principal');

IF NOT EXISTS (SELECT 1 FROM TiposProfesor WHERE Id = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb')
    INSERT INTO TiposProfesor (Id, Nombre) VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Auxiliar');

PRINT 'TiposProfesor: OK';

-- ============================================
-- 8. TiposAsistencia
-- ============================================
IF NOT EXISTS (SELECT 1 FROM TiposAsistencia WHERE Id = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa')
    INSERT INTO TiposAsistencia (Id, Nombre) VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Normal');

IF NOT EXISTS (SELECT 1 FROM TiposAsistencia WHERE Id = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb')
    INSERT INTO TiposAsistencia (Id, Nombre) VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Cortesía');

IF NOT EXISTS (SELECT 1 FROM TiposAsistencia WHERE Id = 'cccccccc-cccc-cccc-cccc-cccccccccccc')
    INSERT INTO TiposAsistencia (Id, Nombre) VALUES ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Clase de Prueba');

PRINT 'TiposAsistencia: OK';

-- NOTE: Roles table doesn't exist - skipping

PRINT 'Catálogos validados: OK';

COMMIT TRANSACTION;

PRINT 'Catálogos base poblados exitosamente.';
PRINT '========================================';
GO

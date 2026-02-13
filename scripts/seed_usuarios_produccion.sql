/********************************************************************************************************
 Script: seed_usuarios_produccion.sql
 Objetivo: Crear usuarios de producción - Administradores y Profesores
 
 Crea:
   - 3 Administradores (Jorge Padilla, Jhonathan Pachon, Chetango Corporacion)
   - 13 Profesores
   - NO crea alumnos (se crearán mediante la funcionalidad del sistema)
   
 Prerequisitos:
   - Base de datos chetango-db-prod existente con tablas creadas
   - Usuarios ya creados en Azure AD con emails @corporacionchetango.com
   
 Instrucciones:
   1. Conectarse a la base de datos chetango-db-prod en Azure
   2. Ejecutar el script completo
   3. Verificar que los usuarios fueron creados correctamente
*********************************************************************************************************/

USE [chetango-db-prod];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT '========================================';
PRINT 'Iniciando creación de usuarios de producción';
PRINT '========================================';
PRINT '';

DECLARE @Ahora DATETIME2(0) = SYSDATETIME();

-- =====================================================================
-- IDs de Catálogos
-- =====================================================================
DECLARE @IdTipoDocCC UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111'; -- Cédula de Ciudadanía
DECLARE @IdEstadoUsuarioActivo INT = 1;
DECLARE @IdTipoProfesorPrincipal UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'; -- Principal

-- =====================================================================
-- GUIDs para Usuarios
-- =====================================================================

-- ADMINISTRADORES
DECLARE @IdUsuarioJorgePadilla UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioJhonathanPachon UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioChetangoCorp UNIQUEIDENTIFIER = NEWID();

-- PROFESORES
DECLARE @IdUsuarioJuanHerrera UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioAnaGomez UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioSantiagoSalazar UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioMariaOspina UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioSusanaAlzate UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioBranndonCardenas UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioSulyPachon UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioCamilaGarzon UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioSamuelSanchez UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioLauraMachado UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioYeisonGomez UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioCarolinaMolina UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioDanielCano UNIQUEIDENTIFIER = NEWID();

-- GUIDs para Profesores
DECLARE @IdProfesorJuanHerrera UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorAnaGomez UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorSantiagoSalazar UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorMariaOspina UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorSusanaAlzate UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorBranndonCardenas UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorSulyPachon UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorCamilaGarzon UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorSamuelSanchez UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorLauraMachado UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorYeisonGomez UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorCarolinaMolina UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorDanielCano UNIQUEIDENTIFIER = NEWID();

BEGIN TRANSACTION;

BEGIN TRY

    PRINT 'Paso 1: Verificando e insertando Administradores...';
    PRINT '';
    
    -- =====================================================================
    -- INSERTAR ADMINISTRADORES (3) - Solo si no existen
    -- =====================================================================
    
    -- Jorge Padilla
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'jorge.padilla@corporacionchetango.com' OR NombreUsuario = 'Jorge Padilla')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioJorgePadilla, 'Jorge Padilla', @IdTipoDocCC, 'ADMIN-001', 'jorge.padilla@corporacionchetango.com', '3000000001', @IdEstadoUsuarioActivo, @Ahora);
        PRINT '  ✅ Jorge Padilla creado';
    END
    ELSE
    BEGIN
        SET @IdUsuarioJorgePadilla = (SELECT TOP 1 IdUsuario FROM Usuarios WHERE Correo = 'jorge.padilla@corporacionchetango.com' OR NombreUsuario = 'Jorge Padilla');
        PRINT '  ⚠️  Jorge Padilla ya existe (nombre o correo duplicado), omitiendo creación';
    END

    -- Jhonathan Pachon
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'jhonathan.pachon@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioJhonathanPachon, 'Jhonathan Pachon', @IdTipoDocCC, 'ADMIN-002', 'jhonathan.pachon@corporacionchetango.com', '3000000002', @IdEstadoUsuarioActivo, @Ahora);
        PRINT '  ✅ Jhonathan Pachon creado';
    END
    ELSE
    BEGIN
        SET @IdUsuarioJhonathanPachon = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'jhonathan.pachon@corporacionchetango.com');
        PRINT '  ⚠️  Jhonathan Pachon ya existe, usando ID existente';
    END

    -- Chetango Corporacion
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'chetango.corporacion@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioChetangoCorp, 'Chetango Corporacion', @IdTipoDocCC, 'ADMIN-003', 'chetango.corporacion@corporacionchetango.com', '3000000003', @IdEstadoUsuarioActivo, @Ahora);
        PRINT '  ✅ Chetango Corporacion creado';
    END
    ELSE
    BEGIN
        SET @IdUsuarioChetangoCorp = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'chetango.corporacion@corporacionchetango.com');
        PRINT '  ⚠️  Chetango Corporacion ya existe, usando ID existente';
    END
    
    PRINT '';

    PRINT 'Paso 2: Verificando e insertando Profesores...';
    PRINT '';
    
    -- =====================================================================
    -- INSERTAR USUARIOS DE PROFESORES (13) - Solo si no existen
    -- =====================================================================
    
    -- Macro para insertar profesores solo si no existen
    DECLARE @ContadorProfesores INT = 0;
    
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'juan.herrera@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioJuanHerrera, 'Juan Herrera', @IdTipoDocCC, 'PROF-001', 'juan.herrera@corporacionchetango.com', '3100000001', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioJuanHerrera = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'juan.herrera@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'ana.gomez@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioAnaGomez, 'Ana Gomez', @IdTipoDocCC, 'PROF-002', 'ana.gomez@corporacionchetango.com', '3100000002', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioAnaGomez = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'ana.gomez@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'santiago.salazar@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioSantiagoSalazar, 'Santiago Salazar', @IdTipoDocCC, 'PROF-003', 'santiago.salazar@corporacionchetango.com', '3100000003', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioSantiagoSalazar = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'santiago.salazar@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'maria.ospina@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioMariaOspina, 'Maria Ospina', @IdTipoDocCC, 'PROF-004', 'maria.ospina@corporacionchetango.com', '3100000004', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioMariaOspina = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'maria.ospina@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'susana.alzate@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioSusanaAlzate, 'Susana Alzate', @IdTipoDocCC, 'PROF-005', 'susana.alzate@corporacionchetango.com', '3100000005', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioSusanaAlzate = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'susana.alzate@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'branndon.cardenas@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioBranndonCardenas, 'Branndon Cardenas', @IdTipoDocCC, 'PROF-006', 'branndon.cardenas@corporacionchetango.com', '3100000006', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioBranndonCardenas = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'branndon.cardenas@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'suly.pachon@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioSulyPachon, 'Suly Pachon', @IdTipoDocCC, 'PROF-007', 'suly.pachon@corporacionchetango.com', '3100000007', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioSulyPachon = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'suly.pachon@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'camila.garzon@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioCamilaGarzon, 'Camila Garzon', @IdTipoDocCC, 'PROF-008', 'camila.garzon@corporacionchetango.com', '3100000008', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioCamilaGarzon = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'camila.garzon@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'samuel.sanchez@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioSamuelSanchez, 'Samuel Sanchez', @IdTipoDocCC, 'PROF-009', 'samuel.sanchez@corporacionchetango.com', '3100000009', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioSamuelSanchez = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'samuel.sanchez@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'laura.machado@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioLauraMachado, 'Laura Machado', @IdTipoDocCC, 'PROF-010', 'laura.machado@corporacionchetango.com', '3100000010', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioLauraMachado = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'laura.machado@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'yeison.gomez@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioYeisonGomez, 'Yeison Gomez', @IdTipoDocCC, 'PROF-011', 'yeison.gomez@corporacionchetango.com', '3100000011', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioYeisonGomez = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'yeison.gomez@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'carolina.molina@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioCarolinaMolina, 'Carolina Molina', @IdTipoDocCC, 'PROF-012', 'carolina.molina@corporacionchetango.com', '3100000012', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioCarolinaMolina = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'carolina.molina@corporacionchetango.com');

    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'daniel.cano@corporacionchetango.com')
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
        VALUES (@IdUsuarioDanielCano, 'Daniel Cano', @IdTipoDocCC, 'PROF-013', 'daniel.cano@corporacionchetango.com', '3100000013', @IdEstadoUsuarioActivo, @Ahora);
        SET @ContadorProfesores = @ContadorProfesores + 1;
    END
    ELSE SET @IdUsuarioDanielCano = (SELECT IdUsuario FROM Usuarios WHERE Correo = 'daniel.cano@corporacionchetango.com');
    
    PRINT '  ✅ ' + CAST(@ContadorProfesores AS VARCHAR) + ' nuevos profesores (usuarios) creados';
    PRINT '  ⚠️  ' + CAST((13 - @ContadorProfesores) AS VARCHAR) + ' profesores ya existían';
    PRINT '';

    PRINT 'Paso 3: Insertando registros en tabla Profesores...';
    PRINT '';
    
    -- =====================================================================
    -- INSERTAR EN TABLA PROFESORES (13) - Solo si no existen
    -- =====================================================================
    
    DECLARE @ContadorNuevosProfesores INT = 0;
    
    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioJuanHerrera)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorJuanHerrera, @IdUsuarioJuanHerrera, @IdTipoProfesorPrincipal, 'Profesor de danza', '["Salón","Escenario"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioAnaGomez)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorAnaGomez, @IdUsuarioAnaGomez, @IdTipoProfesorPrincipal, 'Profesora de danza', '["Salón","Privadas"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioSantiagoSalazar)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorSantiagoSalazar, @IdUsuarioSantiagoSalazar, @IdTipoProfesorPrincipal, 'Profesor de danza', '["Salón","Escenario"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioMariaOspina)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorMariaOspina, @IdUsuarioMariaOspina, @IdTipoProfesorPrincipal, 'Profesora de danza', '["Salón"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioSusanaAlzate)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorSusanaAlzate, @IdUsuarioSusanaAlzate, @IdTipoProfesorPrincipal, 'Profesora de danza', '["Salón","Privadas"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioBranndonCardenas)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorBranndonCardenas, @IdUsuarioBranndonCardenas, @IdTipoProfesorPrincipal, 'Profesor de danza', '["Salón","Escenario"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioSulyPachon)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorSulyPachon, @IdUsuarioSulyPachon, @IdTipoProfesorPrincipal, 'Profesora de danza', '["Salón"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioCamilaGarzon)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorCamilaGarzon, @IdUsuarioCamilaGarzon, @IdTipoProfesorPrincipal, 'Profesora de danza', '["Salón","Privadas"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioSamuelSanchez)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorSamuelSanchez, @IdUsuarioSamuelSanchez, @IdTipoProfesorPrincipal, 'Profesor de danza', '["Salón","Escenario"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioLauraMachado)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorLauraMachado, @IdUsuarioLauraMachado, @IdTipoProfesorPrincipal, 'Profesora de danza', '["Salón"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioYeisonGomez)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorYeisonGomez, @IdUsuarioYeisonGomez, @IdTipoProfesorPrincipal, 'Profesor de danza', '["Salón","Escenario"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioCarolinaMolina)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorCarolinaMolina, @IdUsuarioCarolinaMolina, @IdTipoProfesorPrincipal, 'Profesora de danza', '["Salón","Privadas"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdUsuario = @IdUsuarioDanielCano)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, Biografia, Especialidades, TarifaActual, NotificacionesEmail, RecordatoriosClase, AlertasCambios)
        VALUES (@IdProfesorDanielCano, @IdUsuarioDanielCano, @IdTipoProfesorPrincipal, 'Profesor de danza', '["Salón","Escenario"]', 30000, 1, 1, 1);
        SET @ContadorNuevosProfesores = @ContadorNuevosProfesores + 1;
    END
    
    PRINT '  ✅ ' + CAST(@ContadorNuevosProfesores AS VARCHAR) + ' nuevos registros de Profesores creados';
    PRINT '  ⚠️  ' + CAST((13 - @ContadorNuevosProfesores) AS VARCHAR) + ' profesores ya existían en la tabla';
    PRINT '';

    COMMIT TRANSACTION;
    
    PRINT '========================================';
    PRINT '✅ CREACIÓN COMPLETADA EXITOSAMENTE';
    PRINT '========================================';
    PRINT '';
    PRINT 'Resumen:';
    PRINT '  - 3 Administradores creados';
    PRINT '  - 13 Profesores creados';
    PRINT '  - Total: 16 usuarios';
    PRINT '';
    PRINT '📋 Los alumnos deben ser creados por los administradores';
    PRINT '   usando la funcionalidad del sistema en producción.';
    PRINT '';
    PRINT 'Usuarios administradores:';
    PRINT '  - jorge.padilla@corporacionchetango.com';
    PRINT '  - jhonathan.pachon@corporacionchetango.com';
    PRINT '  - chetango.corporacion@corporacionchetango.com';
    PRINT '';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT '========================================';
    PRINT '❌ ERROR EN LA CREACIÓN';
    PRINT '========================================';
    PRINT 'Error: ' + ERROR_MESSAGE();
    PRINT 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR);
    PRINT '';
    
    THROW;
END CATCH;

-- =====================================================================
-- VERIFICACIÓN
-- =====================================================================

PRINT 'Verificando usuarios creados...';
PRINT '';

SELECT 
    u.NombreUsuario,
    u.Correo,
    CASE 
        WHEN p.IdProfesor IS NOT NULL THEN 'Profesor'
        ELSE 'Administrador'
    END AS Rol,
    u.Telefono,
    CASE u.IdEstadoUsuario WHEN 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
FROM Usuarios u
LEFT JOIN Profesores p ON u.IdUsuario = p.IdUsuario
WHERE u.Correo LIKE '%@corporacionchetango.com'
ORDER BY 
    CASE 
        WHEN p.IdProfesor IS NOT NULL THEN 2
        ELSE 1
    END,
    u.NombreUsuario;

PRINT '';
PRINT 'Script completado.';

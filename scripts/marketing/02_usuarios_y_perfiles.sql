/********************************************************************************************************
 Script: 02_usuarios_y_perfiles.sql
 Objetivo: Crear 5 profesores y 50 alumnos con datos realistas
           PRESERVA los 3 usuarios de prueba existentes (admin, profesor, alumno)
 Fecha: Febrero 2025
 Uso: Marketing video - Diversidad demográfica y temporal
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT 'Iniciando población de usuarios y perfiles...';

-- Obtener IDs reales de catálogos
DECLARE @TipoDocCC UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TiposDocumento WHERE Nombre LIKE '%C%dula%');
DECLARE @TipoProfesorPrincipal UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TiposProfesor WHERE Nombre = 'Principal');
DECLARE @EstadoUsuarioActivo INT = 1; -- Activo
DECLARE @EstadoAlumnoActivo INT = 1; -- Activo
DECLARE @RolAlumno UNIQUEIDENTIFIER = (SELECT IdRol FROM Roles WHERE Nombre = 'Alumno');
DECLARE @RolProfesor UNIQUEIDENTIFIER = (SELECT IdRol FROM Roles WHERE Nombre = 'Profesor');

BEGIN TRANSACTION;

-- ============================================
-- LIMPIAR SOLO DATOS DE MARKETING (NO USUARIOS DE PRUEBA)
-- ============================================
PRINT 'Limpiando datos de marketing previos...';

-- Identificar usuarios de prueba por correo/documento conocidos
DECLARE @UsuariosPrueba TABLE (IdUsuario UNIQUEIDENTIFIER);

INSERT INTO @UsuariosPrueba (IdUsuario)
SELECT IdUsuario FROM Usuarios 
WHERE Correo IN ('admin@chetango.com', 'profesor@chetango.com', 'alumno@chetango.com')
   OR NumeroDocumento IN ('1017141203', '1032010606', '1032569651'); -- Documentos de prueba si existen

-- Eliminar datos vinculados a usuarios de marketing (NO los de prueba)
DELETE FROM UsuariosRoles 
WHERE IdUsuario NOT IN (SELECT IdUsuario FROM @UsuariosPrueba)
  AND IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com');

DELETE FROM Profesores 
WHERE IdUsuario NOT IN (SELECT IdUsuario FROM @UsuariosPrueba)
  AND IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com');

DELETE FROM Alumnos 
WHERE IdUsuario NOT IN (SELECT IdUsuario FROM @UsuariosPrueba)
  AND IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com');

DELETE FROM Usuarios 
WHERE IdUsuario NOT IN (SELECT IdUsuario FROM @UsuariosPrueba)
  AND Correo LIKE '%@marketing.chetango.com';

PRINT 'Limpieza completada (usuarios de prueba preservados).';

-- ============================================
-- PROFESORES (5 profesores con fechas escalonadas)
-- ============================================
PRINT 'Creando 5 profesores...';

-- Profesor 1: Antiguo (inicio sistema)
DECLARE @IdUsuarioProf1 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProf1 UNIQUEIDENTIFIER = NEWID();

    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES (@IdUsuarioProf1, 'Profesor 1', @TipoDocCC, '1000000001', 'profesor1@marketing.chetango.com', '+57 3001234501', @EstadoUsuarioActivo, '2024-08-01 08:00:00');

INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor)
VALUES (@IdProf1, @IdUsuarioProf1, @TipoProfesorPrincipal);

INSERT INTO UsuariosRoles (IdUsuario, IdRol)
VALUES (@IdUsuarioProf1, @RolProfesor);

-- Profesor 2: Antiguo
DECLARE @IdUsuarioProf2 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProf2 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
VALUES (@IdUsuarioProf2, 'Profesor 2', @TipoDocCC, '1000000002', 'profesor2@marketing.chetango.com', '+57 3001234502', @EstadoUsuarioActivo, '2024-08-15 10:00:00');

INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor)
VALUES (@IdProf2, @IdUsuarioProf2, @TipoProfesorPrincipal);

INSERT INTO UsuariosRoles (IdUsuario, IdRol)
VALUES (@IdUsuarioProf2, @RolProfesor);

-- Profesor 3: Intermedio
DECLARE @IdUsuarioProf3 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProf3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
VALUES (@IdUsuarioProf3, 'Profesor 3', @TipoDocCC, '1000000003', 'profesor3@marketing.chetango.com', '+57 3001234503', @EstadoUsuarioActivo, '2024-10-01 09:00:00');

INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor)
VALUES (@IdProf3, @IdUsuarioProf3, @TipoProfesorPrincipal);

INSERT INTO UsuariosRoles (IdUsuario, IdRol)
VALUES (@IdUsuarioProf3, @RolProfesor);

-- Profesor 4: Reciente
DECLARE @IdUsuarioProf4 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProf4 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
VALUES (@IdUsuarioProf4, 'Profesor 4', @TipoDocCC, '1000000004', 'profesor4@marketing.chetango.com', '+57 3001234504', @EstadoUsuarioActivo, '2025-01-10 11:00:00');

INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor)
VALUES (@IdProf4, @IdUsuarioProf4, @TipoProfesorPrincipal);

INSERT INTO UsuariosRoles (IdUsuario, IdRol)
VALUES (@IdUsuarioProf4, @RolProfesor);

-- Profesor 5: Muy reciente
DECLARE @IdUsuarioProf5 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProf5 UNIQUEIDENTIFIER = NEWID();

INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
VALUES (@IdUsuarioProf5, 'Profesor 5', @TipoDocCC, '1000000005', 'profesor5@marketing.chetango.com', '+57 3001234505', @EstadoUsuarioActivo, '2025-02-01 14:00:00');

INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor)
VALUES (@IdProf5, @IdUsuarioProf5, @TipoProfesorPrincipal);

INSERT INTO UsuariosRoles (IdUsuario, IdRol)
VALUES (@IdUsuarioProf5, @RolProfesor);

PRINT '5 profesores creados: Documentos 1000000001-1000000005';

-- ============================================
-- ALUMNOS (50 alumnos distribuidos en el tiempo)
-- ============================================
PRINT 'Creando 50 alumnos distribuidos temporalmente...';

-- Agosto 2024: 10 alumnos (fundadores)
DECLARE @i INT = 1;
DECLARE @IdUsuarioAlum UNIQUEIDENTIFIER;
DECLARE @IdAlum UNIQUEIDENTIFIER;
DECLARE @Documento VARCHAR(20);
DECLARE @Correo NVARCHAR(150);
DECLARE @Telefono NVARCHAR(30);
DECLARE @FechaInscripcion DATETIME2;

-- Batch 1: Agosto 2024 (10 alumnos)
SET @i = 1;
WHILE @i <= 10
BEGIN
    SET @IdUsuarioAlum = NEWID();
    SET @IdAlum = NEWID();
    SET @Documento = '200000' + RIGHT('000' + CAST(@i AS VARCHAR), 3);
    SET @Correo = 'alumno' + CAST(@i AS VARCHAR) + '@marketing.chetango.com';
    SET @Telefono = '+57 310' + RIGHT('0000000' + CAST(1000000 + @i AS VARCHAR), 7);
    SET @FechaInscripcion = DATEADD(DAY, (@i - 1) * 2, '2024-08-05 09:00:00');

    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES (@IdUsuarioAlum, 'Alumno ' + CAST(@i AS VARCHAR), @TipoDocCC, @Documento, @Correo, @Telefono, @EstadoUsuarioActivo, @FechaInscripcion);

    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
    VALUES (@IdAlum, @IdUsuarioAlum, @FechaInscripcion, @EstadoUsuarioActivo);

    INSERT INTO UsuariosRoles (IdUsuario, IdRol)
    VALUES (@IdUsuarioAlum, @RolAlumno);

    SET @i = @i + 1;
END;

-- Batch 2: Septiembre 2024 (8 alumnos)
SET @i = 11;
WHILE @i <= 18
BEGIN
    SET @IdUsuarioAlum = NEWID();
    SET @IdAlum = NEWID();
    SET @Documento = '200000' + RIGHT('000' + CAST(@i AS VARCHAR), 3);
    SET @Correo = 'alumno' + CAST(@i AS VARCHAR) + '@marketing.chetango.com';
    SET @Telefono = '+57 310' + RIGHT('0000000' + CAST(1000000 + @i AS VARCHAR), 7);
    SET @FechaInscripcion = DATEADD(DAY, (@i - 11) * 3, '2024-09-02 10:00:00');

    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES (@IdUsuarioAlum, 'Alumno ' + CAST(@i AS VARCHAR), @TipoDocCC, @Documento, @Correo, @Telefono, @EstadoUsuarioActivo, @FechaInscripcion);

    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
    VALUES (@IdAlum, @IdUsuarioAlum, @FechaInscripcion, @EstadoUsuarioActivo);

    INSERT INTO UsuariosRoles (IdUsuario, IdRol)
    VALUES (@IdUsuarioAlum, @RolAlumno);

    SET @i = @i + 1;
END;

-- Batch 3: Octubre 2024 (7 alumnos)
SET @i = 19;
WHILE @i <= 25
BEGIN
    SET @IdUsuarioAlum = NEWID();
    SET @IdAlum = NEWID();
    SET @Documento = '200000' + RIGHT('000' + CAST(@i AS VARCHAR), 3);
    SET @Correo = 'alumno' + CAST(@i AS VARCHAR) + '@marketing.chetango.com';
    SET @Telefono = '+57 310' + RIGHT('0000000' + CAST(1000000 + @i AS VARCHAR), 7);
    SET @FechaInscripcion = DATEADD(DAY, (@i - 19) * 4, '2024-10-01 11:00:00');

    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES (@IdUsuarioAlum, 'Alumno ' + CAST(@i AS VARCHAR), @TipoDocCC, @Documento, @Correo, @Telefono, @EstadoUsuarioActivo, @FechaInscripcion);

    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
    VALUES (@IdAlum, @IdUsuarioAlum, @FechaInscripcion, @EstadoUsuarioActivo);

    INSERT INTO UsuariosRoles (IdUsuario, IdRol)
    VALUES (@IdUsuarioAlum, @RolAlumno);

    SET @i = @i + 1;
END;

-- Batch 4: Noviembre 2024 (6 alumnos)
SET @i = 26;
WHILE @i <= 31
BEGIN
    SET @IdUsuarioAlum = NEWID();
    SET @IdAlum = NEWID();
    SET @Documento = '200000' + RIGHT('000' + CAST(@i AS VARCHAR), 3);
    SET @Correo = 'alumno' + CAST(@i AS VARCHAR) + '@marketing.chetango.com';
    SET @Telefono = '+57 310' + RIGHT('0000000' + CAST(1000000 + @i AS VARCHAR), 7);
    SET @FechaInscripcion = DATEADD(DAY, (@i - 26) * 4, '2024-11-03 09:00:00');

    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES (@IdUsuarioAlum, 'Alumno ' + CAST(@i AS VARCHAR), @TipoDocCC, @Documento, @Correo, @Telefono, @EstadoUsuarioActivo, @FechaInscripcion);

    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
    VALUES (@IdAlum, @IdUsuarioAlum, @FechaInscripcion, @EstadoUsuarioActivo);

    INSERT INTO UsuariosRoles (IdUsuario, IdRol)
    VALUES (@IdUsuarioAlum, @RolAlumno);

    SET @i = @i + 1;
END;

-- Batch 5: Diciembre 2024 (6 alumnos)
SET @i = 32;
WHILE @i <= 37
BEGIN
    SET @IdUsuarioAlum = NEWID();
    SET @IdAlum = NEWID();
    SET @Documento = '200000' + RIGHT('000' + CAST(@i AS VARCHAR), 3);
    SET @Correo = 'alumno' + CAST(@i AS VARCHAR) + '@marketing.chetango.com';
    SET @Telefono = '+57 310' + RIGHT('0000000' + CAST(1000000 + @i AS VARCHAR), 7);
    SET @FechaInscripcion = DATEADD(DAY, (@i - 32) * 4, '2024-12-02 10:00:00');

    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES (@IdUsuarioAlum, 'Alumno ' + CAST(@i AS VARCHAR), @TipoDocCC, @Documento, @Correo, @Telefono, @EstadoUsuarioActivo, @FechaInscripcion);

    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
    VALUES (@IdAlum, @IdUsuarioAlum, @FechaInscripcion, @EstadoUsuarioActivo);

    INSERT INTO UsuariosRoles (IdUsuario, IdRol)
    VALUES (@IdUsuarioAlum, @RolAlumno);

    SET @i = @i + 1;
END;

-- Batch 6: Enero 2025 (7 alumnos)
SET @i = 38;
WHILE @i <= 44
BEGIN
    SET @IdUsuarioAlum = NEWID();
    SET @IdAlum = NEWID();
    SET @Documento = '200000' + RIGHT('000' + CAST(@i AS VARCHAR), 3);
    SET @Correo = 'alumno' + CAST(@i AS VARCHAR) + '@marketing.chetango.com';
    SET @Telefono = '+57 310' + RIGHT('0000000' + CAST(1000000 + @i AS VARCHAR), 7);
    SET @FechaInscripcion = DATEADD(DAY, (@i - 38) * 4, '2025-01-05 11:00:00');

    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES (@IdUsuarioAlum, 'Alumno ' + CAST(@i AS VARCHAR), @TipoDocCC, @Documento, @Correo, @Telefono, @EstadoUsuarioActivo, @FechaInscripcion);

    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
    VALUES (@IdAlum, @IdUsuarioAlum, @FechaInscripcion, @EstadoUsuarioActivo);

    INSERT INTO UsuariosRoles (IdUsuario, IdRol)
    VALUES (@IdUsuarioAlum, @RolAlumno);

    SET @i = @i + 1;
END;

-- Batch 7: Febrero 2025 (6 alumnos - más recientes)
SET @i = 45;
WHILE @i <= 50
BEGIN
    SET @IdUsuarioAlum = NEWID();
    SET @IdAlum = NEWID();
    SET @Documento = '200000' + RIGHT('000' + CAST(@i AS VARCHAR), 3);
    SET @Correo = 'alumno' + CAST(@i AS VARCHAR) + '@marketing.chetango.com';
    SET @Telefono = '+57 310' + RIGHT('0000000' + CAST(1000000 + @i AS VARCHAR), 7);
    SET @FechaInscripcion = DATEADD(DAY, (@i - 45) * 2, '2025-02-01 09:00:00');

    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES (@IdUsuarioAlum, 'Alumno ' + CAST(@i AS VARCHAR), @TipoDocCC, @Documento, @Correo, @Telefono, @EstadoUsuarioActivo, @FechaInscripcion);

    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
    VALUES (@IdAlum, @IdUsuarioAlum, @FechaInscripcion, @EstadoUsuarioActivo);

    INSERT INTO UsuariosRoles (IdUsuario, IdRol)
    VALUES (@IdUsuarioAlum, @RolAlumno);

    SET @i = @i + 1;
END;

PRINT '50 alumnos creados: Documentos 20000001-20000050';
PRINT 'Distribución temporal:';
PRINT '  - Ago 2024: 10 alumnos';
PRINT '  - Sep 2024: 8 alumnos';
PRINT '  - Oct 2024: 7 alumnos';
PRINT '  - Nov 2024: 6 alumnos';
PRINT '  - Dic 2024: 6 alumnos';
PRINT '  - Ene 2025: 7 alumnos';
PRINT '  - Feb 2025: 6 alumnos';

COMMIT TRANSACTION;

PRINT 'Usuarios y perfiles creados exitosamente.';
PRINT '========================================';
GO

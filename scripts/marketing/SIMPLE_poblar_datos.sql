USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
PRINT 'Iniciando población simplificada de datos de marketing...';

-- Obtener IDs de catálogos
DECLARE @TipoDocCC UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TiposDocumento);
DECLARE @TipoProfesorPrincipal UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TiposProfesor WHERE Nombre = 'Principal');
DECLARE @TipoPaqueteBasico UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TiposPaquete WHERE NumeroClases = 12);
DECLARE @TipoPaquetePremium UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TiposPaquete WHERE NumeroClases > 12);
DECLARE @TipoClaseGrupal UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TiposClase WHERE Nombre LIKE '%Grupal%');
DECLARE @MetodoEfectivo UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM MetodosPago WHERE Nombre LIKE '%Efectivo%');
DECLARE @EstadoPagoCompletado UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM EstadosPago WHERE Nombre LIKE '%Completado%' OR Nombre LIKE '%Pagado%');

BEGIN TRANSACTION;

-- LIMPIAR DATOS PREVIOS DE MARKETING
PRINT '1. Limpiando datos de marketing previos...';
DELETE FROM Asistencias WHERE IdClase IN (SELECT IdClase FROM Clases WHERE Observaciones LIKE '%[MKT]%');
DELETE FROM ClasesProfesores WHERE IdClase IN (SELECT IdClase FROM Clases WHERE Observaciones LIKE '%[MKT]%');
DELETE FROM Clases WHERE Observaciones LIKE '%[MKT]%';
DELETE FROM Paquetes WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos WHERE IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com'));
DELETE FROM Pagos WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos WHERE IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com'));
DELETE FROM Alumnos WHERE IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com');
DELETE FROM Profesores WHERE IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com');
DELETE FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com';
PRINT '   ✓ Limpieza completada';

-- CREAR 5 PROFESORES
PRINT '2. Creando 5 profesores...';
DECLARE @Prof1User UNIQUEIDENTIFIER = NEWID();
DECLARE @Prof1 UNIQUEIDENTIFIER = NEWID();
INSERT INTO Usuarios VALUES (@Prof1User, 'Profesor 1', @TipoDocCC, '1000000001', 'profesor1@marketing.chetango.com', '+57 3001234501', 1, GETDATE());
INSERT INTO Profesores VALUES (@Prof1, @Prof1User, @TipoProfesorPrincipal, 0, NULL, NULL, 1, 1, 30000);

DECLARE @Prof2User UNIQUEIDENTIFIER = NEWID();
DECLARE @Prof2 UNIQUEIDENTIFIER = NEWID();
INSERT INTO Usuarios VALUES (@Prof2User, 'Profesor 2', @TipoDocCC, '1000000002', 'profesor2@marketing.chetango.com', '+57 3001234502', 1, GETDATE());
INSERT INTO Profesores VALUES (@Prof2, @Prof2User, @TipoProfesorPrincipal, 0, NULL, NULL, 1, 1, 30000);

DECLARE @Prof3User UNIQUEIDENTIFIER = NEWID();
DECLARE @Prof3 UNIQUEIDENTIFIER = NEWID();
INSERT INTO Usuarios VALUES (@Prof3User, 'Profesor 3', @TipoDocCC, '1000000003', 'profesor3@marketing.chetango.com', '+57 3001234503', 1, GETDATE());
INSERT INTO Profesores VALUES (@Prof3, @Prof3User, @TipoProfesorPrincipal, 0, NULL, NULL, 1, 1, 30000);

DECLARE @Prof4User UNIQUEIDENTIFIER = NEWID();
DECLARE @Prof4 UNIQUEIDENTIFIER = NEWID();
INSERT INTO Usuarios VALUES (@Prof4User, 'Profesor 4', @TipoDocCC, '1000000004', 'profesor4@marketing.chetango.com', '+57 3001234504', 1, GETDATE());
INSERT INTO Profesores VALUES (@Prof4, @Prof4User, @TipoProfesorPrincipal, 0, NULL, NULL, 1, 1, 30000);

DECLARE @Prof5User UNIQUEIDENTIFIER = NEWID();
DECLARE @Prof5 UNIQUEIDENTIFIER = NEWID();
INSERT INTO Usuarios VALUES (@Prof5User, 'Profesor 5', @TipoDocCC, '1000000005', 'profesor5@marketing.chetango.com', '+57 3001234505', 1, GETDATE());
INSERT INTO Profesores VALUES (@Prof5, @Prof5User, @TipoProfesorPrincipal, 0, NULL, NULL, 1, 1, 30000);

PRINT '   ✓ 5 profesores creados';

-- CREAR 10 ALUMNOS PARA PRUEBA
PRINT '3. Creando 10 alumnos de prueba...';
DECLARE @i INT = 1;
WHILE @i <= 10
BEGIN
    DECLARE @UserID UNIQUEIDENTIFIER = NEWID();
    DECLARE @AlumID UNIQUEIDENTIFIER = NEWID();
    
    INSERT INTO Usuarios VALUES (
        @UserID, 
        'Alumno ' + CAST(@i AS VARCHAR), 
        @TipoDocCC, 
        '20000' + RIGHT('000' + CAST(@i AS VARCHAR), 3),
        'alumno' + CAST(@i AS VARCHAR) + '@marketing.chetango.com',
        '+57 310' + RIGHT('0000000' + CAST(1000000 + @i AS VARCHAR), 7),
        1,
        DATEADD(DAY, -180 + (@i * 15), GETDATE())
    );
    
    INSERT INTO Alumnos VALUES (
        @AlumID,
        @UserID,
        DATEADD(DAY, -180 + (@i * 15), GETDATE()),
        1, 1, NULL, NULL, NULL, NULL, 1, 1
    );
    
    SET @i = @i + 1;
END;

PRINT '   ✓ 10 alumnos creados';

COMMIT TRANSACTION;

PRINT '========================================';
PRINT 'Población simplificada completada';
PRINT '- 5 Profesores';
PRINT '- 10 Alumnos (de 50 planificados)';
PRINT '========================================';
GO

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT '========================================';
PRINT 'Completando alumnos (11-50)';
PRINT '========================================';

DECLARE @TipoDocCC UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TiposDocumento);

BEGIN TRANSACTION;

-- Crear alumnos 11-50
DECLARE @i INT = 11;
WHILE @i <= 50
BEGIN
    DECLARE @UserID UNIQUEIDENTIFIER = NEWID();
    DECLARE @AlumID UNIQUEIDENTIFIER = NEWID();
    DECLARE @FechaInscripcion DATETIME2;
    
    -- Distribuir en el tiempo (últimos 6 meses)
    SET @FechaInscripcion = DATEADD(DAY, -180 + (@i * 3), GETDATE());
    
    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
    VALUES (
        @UserID, 
        'Alumno ' + CAST(@i AS VARCHAR), 
        @TipoDocCC, 
        '20000' + RIGHT('000' + CAST(@i AS VARCHAR), 3),
        'alumno' + CAST(@i AS VARCHAR) + '@marketing.chetango.com',
        '+57 310' + RIGHT('0000000' + CAST(1000000 + @i AS VARCHAR), 7),
        1,
        @FechaInscripcion
    );
    
    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado, AlertasPaquete, AvatarUrl, ContactoEmergenciaNombre, ContactoEmergenciaRelacion, ContactoEmergenciaTelefono, NotificacionesEmail, RecordatoriosClase)
    VALUES (
        @AlumID,
        @UserID,
        @FechaInscripcion,
        1, -- Activo
        1, -- AlertasPaquete
        NULL,
        NULL,
        NULL,
        NULL,
        1, -- NotificacionesEmail
        1  -- RecordatoriosClase
    );
    
    IF @i % 10 = 0
        PRINT '   ✓ ' + CAST(@i AS VARCHAR) + ' alumnos creados...';
    
    SET @i = @i + 1;
END;

COMMIT TRANSACTION;

-- Verificar
DECLARE @TotalAlumnos INT;
SELECT @TotalAlumnos = COUNT(*) FROM Alumnos WHERE IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com');

PRINT '';
PRINT '========================================';
PRINT 'Completado: ' + CAST(@TotalAlumnos AS VARCHAR) + ' alumnos en total';
PRINT '========================================';
GO

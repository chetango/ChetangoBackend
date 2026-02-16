USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT '========================================';
PRINT 'Creando Clases (180 clases)';
PRINT '========================================';

-- Obtener IDs (usando IDs reales de la base de datos)
DECLARE @TipoClaseGrupal UNIQUEIDENTIFIER = '3F53914A-56DF-4F5F-9E51-053C993C2125'; -- Salsa
DECLARE @RolPrincipal UNIQUEIDENTIFIER = 'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF'; -- Principal

-- Obtener profesores
DECLARE @Profesores TABLE (IdProfesor UNIQUEIDENTIFIER, RowNum INT);
INSERT INTO @Profesores
SELECT IdProfesor, ROW_NUMBER() OVER (ORDER BY IdProfesor)
FROM Profesores WHERE IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com');

DECLARE @Prof1 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE RowNum = 1);
DECLARE @Prof2 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE RowNum = 2);
DECLARE @Prof3 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE RowNum = 3);
DECLARE @Prof4 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE RowNum = 4);
DECLARE @Prof5 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE RowNum = 5);

PRINT 'Tipo de clase: ' + CAST(@TipoClaseGrupal AS VARCHAR(50));
PRINT 'Rol Principal: ' + CAST(@RolPrincipal AS VARCHAR(50));
PRINT '5 Profesores obtenidos';
PRINT '';

BEGIN TRANSACTION;

-- Crear clases: 3-4 por semana durante 6 meses (~180 clases)
DECLARE @FechaInicio DATE = DATEADD(DAY, -180, GETDATE());
DECLARE @FechaActual DATE = @FechaInicio;
DECLARE @DiaSemana INT;
DECLARE @IdClase UNIQUEIDENTIFIER;
DECLARE @IdClaseProfesor UNIQUEIDENTIFIER;
DECLARE @ProfesorAsignado UNIQUEIDENTIFIER;
DECLARE @ClasesCreadas INT = 0;

WHILE @FechaActual <= GETDATE()
BEGIN
    SET @DiaSemana = DATEPART(WEEKDAY, @FechaActual);
    
    -- Lunes (2), Miércoles (4), Viernes (6): Clases 18:00
    IF @DiaSemana IN (2, 4, 6)
    BEGIN
        SET @IdClase = NEWID();
        
        -- Rotar profesores
        SET @ProfesorAsignado = CASE (@ClasesCreadas % 5)
            WHEN 0 THEN @Prof1
            WHEN 1 THEN @Prof2
            WHEN 2 THEN @Prof3
            WHEN 3 THEN @Prof4
            ELSE @Prof5
        END;
        
        INSERT INTO Clases (IdClase, Fecha, IdTipoClase, HoraInicio, HoraFin, IdProfesorPrincipal, CupoMaximo, Observaciones, Estado)
        VALUES (
            @IdClase,
            @FechaActual,
            @TipoClaseGrupal,
            '18:00:00',
            '19:30:00',
            @ProfesorAsignado,
            25,
            'Clase grupal salsa [MKT]',
            'Activa'
        );
        
        -- Crear relación ClasesProfesores
        SET @IdClaseProfesor = NEWID();
        INSERT INTO ClasesProfesores (IdClaseProfesor, IdClase, IdProfesor, IdRolEnClase, TarifaProgramada, ValorAdicional, ConceptoAdicional, TotalPago, EstadoPago, FechaAprobacion, FechaPago, AprobadoPorIdUsuario, FechaCreacion, FechaModificacion)
        VALUES (
            @IdClaseProfesor,
            @IdClase,
            @ProfesorAsignado,
            @RolPrincipal,
            30000, -- Tarifa base
            0,
            NULL,
            30000,
            'Pendiente',
            NULL,
            NULL,
            NULL,
            GETDATE(),
            NULL
        );
        
        SET @ClasesCreadas = @ClasesCreadas + 1;
        
        IF @ClasesCreadas % 30 = 0
            PRINT '   ✓ ' + CAST(@ClasesCreadas AS VARCHAR) + ' clases creadas...';
    END;
    
    -- Martes (3) y Jueves (5): Clase nocturna 20:00
    IF @DiaSemana IN (3, 5) AND @ClasesCreadas < 180
    BEGIN
        SET @IdClase = NEWID();
        
        SET @ProfesorAsignado = CASE (@ClasesCreadas % 5)
            WHEN 0 THEN @Prof1
            WHEN 1 THEN @Prof2
            WHEN 2 THEN @Prof3
            WHEN 3 THEN @Prof4
            ELSE @Prof5
        END;
        
        INSERT INTO Clases (IdClase, Fecha, IdTipoClase, HoraInicio, HoraFin, IdProfesorPrincipal, CupoMaximo, Observaciones, Estado)
        VALUES (
            @IdClase,
            @FechaActual,
            @TipoClaseGrupal,
            '20:00:00',
            '21:30:00',
            @ProfesorAsignado,
            20,
            'Clase nocturna salsa [MKT]',
            'Activa'
        );
        
        SET @IdClaseProfesor = NEWID();
        INSERT INTO ClasesProfesores (IdClaseProfesor, IdClase, IdProfesor, IdRolEnClase, TarifaProgramada, ValorAdicional, ConceptoAdicional, TotalPago, EstadoPago, FechaAprobacion, FechaPago, AprobadoPorIdUsuario, FechaCreacion, FechaModificacion)
        VALUES (
            @IdClaseProfesor,
            @IdClase,
            @ProfesorAsignado,
            @RolPrincipal,
            30000,
            0,
            NULL,
            30000,
            'Pendiente',
            NULL,
            NULL,
            NULL,
            GETDATE(),
            NULL
        );
        
        SET @ClasesCreadas = @ClasesCreadas + 1;
        
        IF @ClasesCreadas % 30 = 0
            PRINT '   ✓ ' + CAST(@ClasesCreadas AS VARCHAR) + ' clases creadas...';
    END;
    
    SET @FechaActual = DATEADD(DAY, 1, @FechaActual);
END;

COMMIT TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT 'Creadas: ' + CAST(@ClasesCreadas AS VARCHAR) + ' clases';
PRINT '========================================';
GO

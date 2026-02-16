/********************************************************************************************************
 Script: 04_programacion_clases.sql
 Objetivo: Crear ~180 clases distribuidas desde Agosto 2024 hasta Febrero 2026
           Incluye asignación de profesores y horarios realistas
 Fecha: Febrero 2025
 Uso: Marketing video - Calendario, reportes, programación
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT 'Iniciando programación de clases...';

DECLARE @TipoClaseGrupal UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @EstadoActivo INT = 1;

BEGIN TRANSACTION;

-- ============================================
-- LIMPIAR CLASES DE MARKETING
-- ============================================
PRINT 'Limpiando clases previas...';

DELETE FROM ClaseProfesor WHERE IdClase IN (
    SELECT IdClase FROM Clases WHERE Descripcion LIKE '%[MKT]%'
);

DELETE FROM Clases WHERE Descripcion LIKE '%[MKT]%';

PRINT 'Limpieza completada.';

-- ============================================
-- OBTENER PROFESORES
-- ============================================
DECLARE @Profesores TABLE (IdProfesor UNIQUEIDENTIFIER, NumProf INT);

INSERT INTO @Profesores (IdProfesor, NumProf)
SELECT p.IdProfesor, ROW_NUMBER() OVER (ORDER BY u.FechaCreacion)
FROM Profesores p
INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com'
ORDER BY u.FechaCreacion;

DECLARE @Prof1 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE NumProf = 1);
DECLARE @Prof2 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE NumProf = 2);
DECLARE @Prof3 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE NumProf = 3);
DECLARE @Prof4 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE NumProf = 4);
DECLARE @Prof5 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE NumProf = 5);

-- ============================================
-- GENERAR CLASES (3-5 por semana desde Ago 2024)
-- ============================================
PRINT 'Generando clases distribuidas...';

DECLARE @FechaInicio DATE = '2024-08-05';
DECLARE @FechaFin DATE = '2026-02-28';
DECLARE @FechaActual DATE = @FechaInicio;
DECLARE @DiaSemana INT;
DECLARE @HoraClase TIME;
DECLARE @IdClase UNIQUEIDENTIFIER;
DECLARE @IdClaseProfesor UNIQUEIDENTIFIER;
DECLARE @ProfesorAsignado UNIQUEIDENTIFIER;
DECLARE @ClasesCreadas INT = 0;
DECLARE @Titulo NVARCHAR(200);

WHILE @FechaActual <= @FechaFin
BEGIN
    SET @DiaSemana = DATEPART(WEEKDAY, @FechaActual); -- 1=Domingo, 2=Lunes, ..., 7=Sábado
    
    -- Lunes (2), Miércoles (4), Viernes (6): Clases en horarios específicos
    IF @DiaSemana IN (2, 4, 6)
    BEGIN
        -- Clase matutina: 08:00
        SET @IdClase = NEWID();
        SET @HoraClase = '08:00:00';
        SET @Titulo = 'Clase Matutina [MKT]';
        
        -- Rotación de profesores (Prof1 y Prof2 en mañanas)
        SET @ProfesorAsignado = CASE 
            WHEN (@ClasesCreadas % 2) = 0 THEN @Prof1
            ELSE @Prof2
        END;
        
        INSERT INTO Clases (IdClase, IdTipoClase, Titulo, Descripcion, Fecha, Hora, Duracion, CapacidadMaxima, Estado)
        VALUES (@IdClase, @TipoClaseGrupal, @Titulo, 'Clase grupal de salsa nivel básico-intermedio [MKT]', 
                @FechaActual, @HoraClase, 90, 25, @EstadoActivo);
        
        SET @IdClaseProfesor = NEWID();
        INSERT INTO ClaseProfesor (IdClaseProfesor, IdClase, IdProfesor, EsPrincipal, HorasTrabajadas, EstadoPago)
        VALUES (@IdClaseProfesor, @IdClase, @ProfesorAsignado, 1, 1.5, 'Pendiente');
        
        SET @ClasesCreadas = @ClasesCreadas + 1;
        
        -- Clase vespertina: 18:00 (solo si hay suficientes alumnos - después Sep 2024)
        IF @FechaActual >= '2024-09-01'
        BEGIN
            SET @IdClase = NEWID();
            SET @HoraClase = '18:00:00';
            SET @Titulo = 'Clase Vespertina [MKT]';
            
            -- Prof3, Prof4, Prof5 en tardes
            SET @ProfesorAsignado = CASE 
                WHEN (@ClasesCreadas % 3) = 0 THEN @Prof3
                WHEN (@ClasesCreadas % 3) = 1 THEN @Prof4
                ELSE @Prof5
            END;
            
            INSERT INTO Clases (IdClase, IdTipoClase, Titulo, Descripcion, Fecha, Hora, Duracion, CapacidadMaxima, Estado)
            VALUES (@IdClase, @TipoClaseGrupal, @Titulo, 'Clase grupal de salsa nivel intermedio-avanzado [MKT]', 
                    @FechaActual, @HoraClase, 90, 20, @EstadoActivo);
            
            SET @IdClaseProfesor = NEWID();
            INSERT INTO ClaseProfesor (IdClaseProfesor, IdClase, IdProfesor, EsPrincipal, HorasTrabajadas, EstadoPago)
            VALUES (@IdClaseProfesor, @IdClase, @ProfesorAsignado, 1, 1.5, 'Pendiente');
            
            SET @ClasesCreadas = @ClasesCreadas + 1;
        END;
    END;
    
    -- Martes (3) y Jueves (5): Clase nocturna desde Oct 2024
    IF @DiaSemana IN (3, 5) AND @FechaActual >= '2024-10-01'
    BEGIN
        SET @IdClase = NEWID();
        SET @HoraClase = '20:00:00';
        SET @Titulo = 'Clase Nocturna [MKT]';
        
        SET @ProfesorAsignado = CASE 
            WHEN (@ClasesCreadas % 5) = 0 THEN @Prof1
            WHEN (@ClasesCreadas % 5) = 1 THEN @Prof2
            WHEN (@ClasesCreadas % 5) = 2 THEN @Prof3
            WHEN (@ClasesCreadas % 5) = 3 THEN @Prof4
            ELSE @Prof5
        END;
        
        INSERT INTO Clases (IdClase, IdTipoClase, Titulo, Descripcion, Fecha, Hora, Duracion, CapacidadMaxima, Estado)
        VALUES (@IdClase, @TipoClaseGrupal, @Titulo, 'Clase grupal de salsa nivel mixto [MKT]', 
                @FechaActual, @HoraClase, 90, 18, @EstadoActivo);
        
        SET @IdClaseProfesor = NEWID();
        INSERT INTO ClaseProfesor (IdClaseProfesor, IdClase, IdProfesor, EsPrincipal, HorasTrabajadas, EstadoPago)
        VALUES (@IdClaseProfesor, @IdClase, @ProfesorAsignado, 1, 1.5, 'Pendiente');
        
        SET @ClasesCreadas = @ClasesCreadas + 1;
    END;
    
    -- Sábado (7): Dos clases desde Nov 2024
    IF @DiaSemana = 7 AND @FechaActual >= '2024-11-01'
    BEGIN
        -- Clase mañana
        SET @IdClase = NEWID();
        SET @HoraClase = '10:00:00';
        SET @Titulo = 'Clase Sábado Mañana [MKT]';
        
        SET @ProfesorAsignado = CASE 
            WHEN (@ClasesCreadas % 2) = 0 THEN @Prof1
            ELSE @Prof3
        END;
        
        INSERT INTO Clases (IdClase, IdTipoClase, Titulo, Descripcion, Fecha, Hora, Duracion, CapacidadMaxima, Estado)
        VALUES (@IdClase, @TipoClaseGrupal, @Titulo, 'Clase especial fin de semana [MKT]', 
                @FechaActual, @HoraClase, 120, 30, @EstadoActivo);
        
        SET @IdClaseProfesor = NEWID();
        INSERT INTO ClaseProfesor (IdClaseProfesor, IdClase, IdProfesor, EsPrincipal, HorasTrabajadas, EstadoPago)
        VALUES (@IdClaseProfesor, @IdClase, @ProfesorAsignado, 1, 2.0, 'Pendiente');
        
        SET @ClasesCreadas = @ClasesCreadas + 1;
        
        -- Clase tarde
        SET @IdClase = NEWID();
        SET @HoraClase = '15:00:00';
        SET @Titulo = 'Clase Sábado Tarde [MKT]';
        
        SET @ProfesorAsignado = CASE 
            WHEN (@ClasesCreadas % 2) = 0 THEN @Prof2
            ELSE @Prof4
        END;
        
        INSERT INTO Clases (IdClase, IdTipoClase, Titulo, Descripcion, Fecha, Hora, Duracion, CapacidadMaxima, Estado)
        VALUES (@IdClase, @TipoClaseGrupal, @Titulo, 'Clase práctica intensiva [MKT]', 
                @FechaActual, @HoraClase, 120, 25, @EstadoActivo);
        
        SET @IdClaseProfesor = NEWID();
        INSERT INTO ClaseProfesor (IdClaseProfesor, IdClase, IdProfesor, EsPrincipal, HorasTrabajadas, EstadoPago)
        VALUES (@IdClaseProfesor, @IdClase, @ProfesorAsignado, 1, 2.0, 'Pendiente');
        
        SET @ClasesCreadas = @ClasesCreadas + 1;
    END;
    
    SET @FechaActual = DATEADD(DAY, 1, @FechaActual);
END;

PRINT 'Clases creadas: ' + CAST(@ClasesCreadas AS VARCHAR);
PRINT 'Distribución:';
PRINT '  - Lun/Mié/Vie: Clases matutinas y vespertinas';
PRINT '  - Mar/Jue: Clases nocturnas';
PRINT '  - Sábados: Clases especiales dobles';

COMMIT TRANSACTION;

PRINT 'Programación de clases completada exitosamente.';
PRINT '========================================';
GO

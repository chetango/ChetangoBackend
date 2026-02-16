/********************************************************************************************************
 Script: 09_solicitudes.sql
 Objetivo: Crear solicitudes de clases privadas y renovaciones de paquetes
           20 solicitudes privadas + 15 renovaciones en diferentes estados
 Fecha: Febrero 2025
 Uso: Marketing video - Gestión de solicitudes, workflow aprobaciones
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT 'Iniciando población de solicitudes...';

BEGIN TRANSACTION;

-- ============================================
-- LIMPIAR SOLICITUDES DE MARKETING
-- ============================================
PRINT 'Limpiando solicitudes previas...';

DELETE FROM SolicitudRenovacionPaquete WHERE IdAlumno IN (
    SELECT a.IdAlumno FROM Alumnos a
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
    WHERE u.Correo LIKE '%@marketing.chetango.com'
);

DELETE FROM SolicitudClasePrivada WHERE IdAlumno IN (
    SELECT a.IdAlumno FROM Alumnos a
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
    WHERE u.Correo LIKE '%@marketing.chetango.com'
);

PRINT 'Limpieza completada.';

-- ============================================
-- CREAR SOLICITUDES DE CLASES PRIVADAS (20)
-- ============================================
PRINT 'Creando 20 solicitudes de clases privadas...';

DECLARE @AlumnosMarketing TABLE (IdAlumno UNIQUEIDENTIFIER, FechaInscripcion DATETIME2);

INSERT INTO @AlumnosMarketing (IdAlumno, FechaInscripcion)
SELECT a.IdAlumno, a.FechaInscripcion
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com';

DECLARE @IdAlumno UNIQUEIDENTIFIER;
DECLARE @IdSolicitud UNIQUEIDENTIFIER;
DECLARE @FechaSolicitud DATETIME2;
DECLARE @EstadoSolicitud VARCHAR(20);
DECLARE @FechaPreferida DATE;
DECLARE @HoraPreferida TIME;
DECLARE @Observaciones NVARCHAR(500);
DECLARE @SolicitudesPrivadasCreadas INT = 0;

DECLARE @i INT = 1;
WHILE @i <= 20
BEGIN
    -- Seleccionar alumno aleatorio
    SELECT TOP 1 @IdAlumno = IdAlumno
    FROM @AlumnosMarketing
    ORDER BY NEWID();
    
    SET @IdSolicitud = NEWID();
    
    -- Fecha entre Nov 2024 y Feb 2025
    SET @FechaSolicitud = DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 100), '2024-11-01');
    
    -- Estados: 50% Pendiente, 30% Aprobada, 20% Rechazada
    SET @EstadoSolicitud = CASE (ABS(CHECKSUM(NEWID())) % 10)
        WHEN 0 THEN 'Rechazada'
        WHEN 1 THEN 'Rechazada'
        WHEN 2 THEN 'Aprobada'
        WHEN 3 THEN 'Aprobada'
        WHEN 4 THEN 'Aprobada'
        ELSE 'Pendiente'
    END;
    
    -- Fecha preferida futura
    SET @FechaPreferida = DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 30) + 5, @FechaSolicitud);
    
    -- Hora preferida
    SET @HoraPreferida = CASE (ABS(CHECKSUM(NEWID())) % 5)
        WHEN 0 THEN '08:00:00'
        WHEN 1 THEN '10:00:00'
        WHEN 2 THEN '14:00:00'
        WHEN 3 THEN '16:00:00'
        ELSE '18:00:00'
    END;
    
    -- Observaciones variadas
    SET @Observaciones = CASE (ABS(CHECKSUM(NEWID())) % 5)
        WHEN 0 THEN 'Me gustaría enfocarme en técnica de giros'
        WHEN 1 THEN 'Necesito preparación para competencia regional'
        WHEN 2 THEN 'Quiero mejorar mi footwork y shines'
        WHEN 3 THEN 'Clase privada para pareja (2 personas)'
        ELSE 'Preparación para presentación especial'
    END;
    
    INSERT INTO SolicitudClasePrivada (
        IdSolicitud, IdAlumno, FechaSolicitud, FechaPreferida, HoraPreferida,
        Observaciones, Estado, FechaRespuesta
    )
    VALUES (
        @IdSolicitud, @IdAlumno, @FechaSolicitud, @FechaPreferida, @HoraPreferida,
        @Observaciones, @EstadoSolicitud, 
        CASE WHEN @EstadoSolicitud != 'Pendiente' THEN DATEADD(DAY, 2, @FechaSolicitud) ELSE NULL END
    );
    
    SET @SolicitudesPrivadasCreadas = @SolicitudesPrivadasCreadas + 1;
    SET @i = @i + 1;
END;

PRINT 'Solicitudes de clases privadas creadas: ' + CAST(@SolicitudesPrivadasCreadas AS VARCHAR);

-- ============================================
-- CREAR SOLICITUDES DE RENOVACIÓN (15)
-- ============================================
PRINT 'Creando 15 solicitudes de renovación de paquetes...';

DECLARE @IdPaquete UNIQUEIDENTIFIER;
DECLARE @TipoPaqueteNuevo UNIQUEIDENTIFIER;
DECLARE @MontoEstimado DECIMAL(18,2);
DECLARE @SolicitudesRenovacionCreadas INT = 0;

DECLARE @TipoPaqueteBasico UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @TipoPaquetePremium UNIQUEIDENTIFIER = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb';

SET @i = 1;
WHILE @i <= 15
BEGIN
    -- Seleccionar alumno aleatorio
    SELECT TOP 1 @IdAlumno = IdAlumno
    FROM @AlumnosMarketing
    ORDER BY NEWID();
    
    -- Obtener paquete casi agotado del alumno
    SELECT TOP 1 @IdPaquete = IdPaquete
    FROM Paquetes
    WHERE IdAlumno = @IdAlumno
      AND ClasesUsadas >= (ClasesDisponibles * 0.8) -- 80% usado
    ORDER BY FechaCompra DESC;
    
    IF @IdPaquete IS NOT NULL
    BEGIN
        SET @IdSolicitud = NEWID();
        
        -- Fecha entre Dic 2024 y Feb 2025
        SET @FechaSolicitud = DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 80), '2024-12-01');
        
        -- 60% mismo nivel, 40% upgrade a Premium
        IF (ABS(CHECKSUM(NEWID())) % 10) < 6
        BEGIN
            SELECT @TipoPaqueteNuevo = IdTipoPaquete FROM Paquetes WHERE IdPaquete = @IdPaquete;
        END
        ELSE
        BEGIN
            SET @TipoPaqueteNuevo = @TipoPaquetePremium;
        END;
        
        -- Monto según tipo
        SET @MontoEstimado = CASE 
            WHEN @TipoPaqueteNuevo = @TipoPaqueteBasico THEN 200000
            ELSE 350000
        END;
        
        -- Estados: 40% Pendiente, 40% Aprobada, 20% Rechazada
        SET @EstadoSolicitud = CASE (ABS(CHECKSUM(NEWID())) % 10)
            WHEN 0 THEN 'Rechazada'
            WHEN 1 THEN 'Rechazada'
            WHEN 2 THEN 'Aprobada'
            WHEN 3 THEN 'Aprobada'
            WHEN 4 THEN 'Aprobada'
            WHEN 5 THEN 'Aprobada'
            ELSE 'Pendiente'
        END;
        
        SET @Observaciones = CASE (ABS(CHECKSUM(NEWID())) % 4)
            WHEN 0 THEN 'Solicito descuento por renovación anticipada'
            WHEN 1 THEN 'Me gustaría mantener mi horario actual'
            WHEN 2 THEN 'Tengo código de referido para aplicar'
            ELSE 'Renovación estándar'
        END;
        
        INSERT INTO SolicitudRenovacionPaquete (
            IdSolicitud, IdAlumno, IdPaqueteActual, IdTipoPaqueteNuevo,
            FechaSolicitud, MontoEstimado, Observaciones, Estado, FechaRespuesta
        )
        VALUES (
            @IdSolicitud, @IdAlumno, @IdPaquete, @TipoPaqueteNuevo,
            @FechaSolicitud, @MontoEstimado, @Observaciones, @EstadoSolicitud,
            CASE WHEN @EstadoSolicitud != 'Pendiente' THEN DATEADD(DAY, 1, @FechaSolicitud) ELSE NULL END
        );
        
        SET @SolicitudesRenovacionCreadas = @SolicitudesRenovacionCreadas + 1;
    END;
    
    SET @i = @i + 1;
END;

PRINT 'Solicitudes de renovación creadas: ' + CAST(@SolicitudesRenovacionCreadas AS VARCHAR);

-- ============================================
-- ESTADÍSTICAS
-- ============================================
DECLARE @TotalSolicitudes INT;
DECLARE @SolicitudesPendientes INT;
DECLARE @SolicitudesAprobadas INT;
DECLARE @SolicitudesRechazadas INT;

SELECT @TotalSolicitudes = 
    (SELECT COUNT(*) FROM SolicitudClasePrivada WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing)) +
    (SELECT COUNT(*) FROM SolicitudRenovacionPaquete WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing));

SELECT @SolicitudesPendientes = 
    (SELECT COUNT(*) FROM SolicitudClasePrivada WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing) AND Estado = 'Pendiente') +
    (SELECT COUNT(*) FROM SolicitudRenovacionPaquete WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing) AND Estado = 'Pendiente');

SELECT @SolicitudesAprobadas = 
    (SELECT COUNT(*) FROM SolicitudClasePrivada WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing) AND Estado = 'Aprobada') +
    (SELECT COUNT(*) FROM SolicitudRenovacionPaquete WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing) AND Estado = 'Aprobada');

SELECT @SolicitudesRechazadas = 
    (SELECT COUNT(*) FROM SolicitudClasePrivada WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing) AND Estado = 'Rechazada') +
    (SELECT COUNT(*) FROM SolicitudRenovacionPaquete WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosMarketing) AND Estado = 'Rechazada');

PRINT 'Estadísticas de solicitudes:';
PRINT '  - Total solicitudes: ' + CAST(@TotalSolicitudes AS VARCHAR);
PRINT '    * Clases privadas: ' + CAST(@SolicitudesPrivadasCreadas AS VARCHAR);
PRINT '    * Renovaciones: ' + CAST(@SolicitudesRenovacionCreadas AS VARCHAR);
PRINT '  - Por estado:';
PRINT '    * Pendientes: ' + CAST(@SolicitudesPendientes AS VARCHAR);
PRINT '    * Aprobadas: ' + CAST(@SolicitudesAprobadas AS VARCHAR);
PRINT '    * Rechazadas: ' + CAST(@SolicitudesRechazadas AS VARCHAR);

COMMIT TRANSACTION;

PRINT 'Solicitudes pobladas exitosamente.';
PRINT '========================================';
GO

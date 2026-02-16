/********************************************************************************************************
 Script: 05_asistencias_masivas.sql
 Objetivo: Crear ~2,300 asistencias con ocupación realista (60-80%)
           Solo para clases pasadas (no futuras)
 Fecha: Febrero 2025
 Uso: Marketing video - Dashboard asistencias, reportes, gráficos ocupación
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT 'Iniciando generación de asistencias masivas...';

DECLARE @TipoAsistenciaNormal UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @EstadoPresente INT = 1;
DECLARE @EstadoAusente INT = 2;
DECLARE @EstadoTarde INT = 4;

BEGIN TRANSACTION;

-- ============================================
-- LIMPIAR ASISTENCIAS DE MARKETING
-- ============================================
PRINT 'Limpiando asistencias previas...';

DELETE FROM Asistencias WHERE IdClase IN (
    SELECT IdClase FROM Clases WHERE Descripcion LIKE '%[MKT]%'
);

PRINT 'Limpieza completada.';

-- ============================================
-- GENERAR ASISTENCIAS
-- ============================================
PRINT 'Generando asistencias para clases pasadas...';

-- Obtener clases pasadas solamente
DECLARE @ClasesPasadas TABLE (IdClase UNIQUEIDENTIFIER, Fecha DATE, CapacidadMaxima INT);

INSERT INTO @ClasesPasadas (IdClase, Fecha, CapacidadMaxima)
SELECT IdClase, Fecha, CapacidadMaxima
FROM Clases
WHERE Descripcion LIKE '%[MKT]%'
  AND Fecha < CAST(GETDATE() AS DATE)
ORDER BY Fecha;

-- Obtener alumnos activos por mes
DECLARE @AlumnosMarketing TABLE (IdAlumno UNIQUEIDENTIFIER, FechaInscripcion DATETIME2);

INSERT INTO @AlumnosMarketing (IdAlumno, FechaInscripcion)
SELECT a.IdAlumno, a.FechaInscripcion
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com';

DECLARE @IdClase UNIQUEIDENTIFIER;
DECLARE @FechaClase DATE;
DECLARE @CapacidadMaxima INT;
DECLARE @OcupacionObjetivo INT;
DECLARE @IdAlumno UNIQUEIDENTIFIER;
DECLARE @FechaInscripcion DATETIME2;
DECLARE @IdAsistencia UNIQUEIDENTIFIER;
DECLARE @IdPaquete UNIQUEIDENTIFIER;
DECLARE @EstadoAsistencia INT;
DECLARE @AsistenciasCreadas INT = 0;
DECLARE @AlumnosRegistrados INT;

DECLARE cursorClases CURSOR FAST_FORWARD FOR
SELECT IdClase, Fecha, CapacidadMaxima FROM @ClasesPasadas;

OPEN cursorClases;
FETCH NEXT FROM cursorClases INTO @IdClase, @FechaClase, @CapacidadMaxima;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Ocupación realista: 60-80% de capacidad
    SET @OcupacionObjetivo = (@CapacidadMaxima * (60 + (ABS(CHECKSUM(NEWID())) % 21))) / 100;
    SET @AlumnosRegistrados = 0;
    
    -- Seleccionar alumnos que ya estaban inscritos en esa fecha
    DECLARE cursorAlumnos CURSOR FAST_FORWARD FOR
    SELECT IdAlumno, FechaInscripcion
    FROM @AlumnosMarketing
    WHERE CAST(FechaInscripcion AS DATE) <= @FechaClase
    ORDER BY NEWID(); -- Aleatoriedad
    
    OPEN cursorAlumnos;
    FETCH NEXT FROM cursorAlumnos INTO @IdAlumno, @FechaInscripcion;
    
    WHILE @@FETCH_STATUS = 0 AND @AlumnosRegistrados < @OcupacionObjetivo
    BEGIN
        -- Obtener un paquete activo del alumno en esa fecha
        SELECT TOP 1 @IdPaquete = IdPaquete
        FROM Paquetes
        WHERE IdAlumno = @IdAlumno
          AND FechaCompra <= @FechaClase
          AND ClasesUsadas < ClasesDisponibles
        ORDER BY FechaCompra DESC;
        
        IF @IdPaquete IS NOT NULL
        BEGIN
            SET @IdAsistencia = NEWID();
            
            -- 85% Presente, 10% Ausente, 5% Tarde
            SET @EstadoAsistencia = CASE (ABS(CHECKSUM(NEWID())) % 100)
                WHEN 0 THEN @EstadoAusente
                WHEN 1 THEN @EstadoTarde
                WHEN 2 THEN @EstadoTarde
                WHEN 3 THEN @EstadoAusente
                WHEN 4 THEN @EstadoAusente
                WHEN 5 THEN @EstadoAusente
                WHEN 6 THEN @EstadoAusente
                WHEN 7 THEN @EstadoAusente
                WHEN 8 THEN @EstadoAusente
                WHEN 9 THEN @EstadoAusente
                WHEN 10 THEN @EstadoAusente
                WHEN 11 THEN @EstadoAusente
                WHEN 12 THEN @EstadoTarde
                WHEN 13 THEN @EstadoTarde
                WHEN 14 THEN @EstadoTarde
                ELSE @EstadoPresente
            END;
            
            INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaquete, IdTipoAsistencia, IdEstado, FechaRegistro)
            VALUES (@IdAsistencia, @IdClase, @IdAlumno, @IdPaquete, @TipoAsistenciaNormal, @EstadoAsistencia, 
                    DATEADD(HOUR, -1, @FechaClase)); -- Registrada 1 hora antes
            
            -- Descontar clase del paquete si asistió
            IF @EstadoAsistencia = @EstadoPresente OR @EstadoAsistencia = @EstadoTarde
            BEGIN
                UPDATE Paquetes SET ClasesUsadas = ClasesUsadas + 1 WHERE IdPaquete = @IdPaquete;
            END;
            
            SET @AlumnosRegistrados = @AlumnosRegistrados + 1;
            SET @AsistenciasCreadas = @AsistenciasCreadas + 1;
        END;
        
        FETCH NEXT FROM cursorAlumnos INTO @IdAlumno, @FechaInscripcion;
    END;
    
    CLOSE cursorAlumnos;
    DEALLOCATE cursorAlumnos;
    
    FETCH NEXT FROM cursorClases INTO @IdClase, @FechaClase, @CapacidadMaxima;
END;

CLOSE cursorClases;
DEALLOCATE cursorClases;

-- ============================================
-- ESTADÍSTICAS
-- ============================================
DECLARE @TotalClasesPasadas INT;
DECLARE @OcupacionPromedio DECIMAL(5,2);

SELECT @TotalClasesPasadas = COUNT(*) FROM @ClasesPasadas;

SELECT @OcupacionPromedio = AVG(CAST(AsistenciasPorClase AS DECIMAL(5,2)) / CapacidadMaxima) * 100
FROM (
    SELECT c.IdClase, c.CapacidadMaxima, COUNT(a.IdAsistencia) as AsistenciasPorClase
    FROM @ClasesPasadas c
    LEFT JOIN Asistencias a ON c.IdClase = a.IdClase
    GROUP BY c.IdClase, c.CapacidadMaxima
) stats;

PRINT 'Asistencias generadas: ' + CAST(@AsistenciasCreadas AS VARCHAR);
PRINT 'Clases pasadas procesadas: ' + CAST(@TotalClasesPasadas AS VARCHAR);
PRINT 'Ocupación promedio: ' + CAST(@OcupacionPromedio AS VARCHAR) + '%';

COMMIT TRANSACTION;

PRINT 'Asistencias masivas generadas exitosamente.';
PRINT '========================================';
GO

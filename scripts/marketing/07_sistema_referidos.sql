/********************************************************************************************************
 Script: 07_sistema_referidos.sql
 Objetivo: Crear códigos de referido y usos realistas
           15 alumnos con códigos activos, 30 usos distribuidos
 Fecha: Febrero 2025
 Uso: Marketing video - Sistema de referidos, descuentos, tracking
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT 'Iniciando población de sistema de referidos...';

BEGIN TRANSACTION;

-- ============================================
-- LIMPIAR SISTEMA REFERIDOS DE MARKETING
-- ============================================
PRINT 'Limpiando referidos previos...';

DELETE FROM UsoCodigoReferido WHERE IdCodigoReferido IN (
    SELECT cr.IdCodigoReferido FROM CodigoReferido cr
    INNER JOIN Alumnos a ON cr.IdAlumno = a.IdAlumno
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
    WHERE u.Correo LIKE '%@marketing.chetango.com'
);

DELETE FROM CodigoReferido WHERE IdAlumno IN (
    SELECT a.IdAlumno FROM Alumnos a
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
    WHERE u.Correo LIKE '%@marketing.chetango.com'
);

PRINT 'Limpieza completada.';

-- ============================================
-- CREAR CÓDIGOS DE REFERIDO (15 alumnos)
-- ============================================
PRINT 'Creando códigos de referido para 15 alumnos...';

-- Seleccionar 15 alumnos antiguos (más probabilidad de referir)
DECLARE @AlumnosConCodigo TABLE (IdAlumno UNIQUEIDENTIFIER, NumAlumno INT, FechaInscripcion DATETIME2);

INSERT INTO @AlumnosConCodigo (IdAlumno, NumAlumno, FechaInscripcion)
SELECT TOP 15 a.IdAlumno, 
       CAST(REPLACE(u.NumeroDocumento, '20000', '') AS INT),
       a.FechaInscripcion
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com'
ORDER BY a.FechaInscripcion; -- Los más antiguos

DECLARE @IdAlumno UNIQUEIDENTIFIER;
DECLARE @NumAlumno INT;
DECLARE @FechaInscripcion DATETIME2;
DECLARE @IdCodigoReferido UNIQUEIDENTIFIER;
DECLARE @Codigo VARCHAR(10);
DECLARE @FechaCreacion DATETIME2;
DECLARE @DescuentoReferidor DECIMAL(18,2) = 20000; -- $20k descuento para quien refiere
DECLARE @DescuentoReferido DECIMAL(18,2) = 30000; -- $30k descuento para referido
DECLARE @CodigosCreados INT = 0;

DECLARE cursorAlumnos CURSOR FAST_FORWARD FOR
SELECT IdAlumno, NumAlumno, FechaInscripcion FROM @AlumnosConCodigo;

OPEN cursorAlumnos;
FETCH NEXT FROM cursorAlumnos INTO @IdAlumno, @NumAlumno, @FechaInscripcion;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @IdCodigoReferido = NEWID();
    SET @Codigo = 'REF' + RIGHT('000' + CAST(@NumAlumno AS VARCHAR), 3); -- REF001, REF002...
    SET @FechaCreacion = DATEADD(DAY, 30, @FechaInscripcion); -- Código creado 1 mes después de inscripción
    
    INSERT INTO CodigoReferido (
        IdCodigoReferido, IdAlumno, Codigo, FechaCreacion, 
        EsActivo, VecesUsado, DescuentoReferidor, DescuentoReferido
    )
    VALUES (
        @IdCodigoReferido, @IdAlumno, @Codigo, @FechaCreacion,
        1, 0, @DescuentoReferidor, @DescuentoReferido
    );
    
    SET @CodigosCreados = @CodigosCreados + 1;
    
    FETCH NEXT FROM cursorAlumnos INTO @IdAlumno, @NumAlumno, @FechaInscripcion;
END;

CLOSE cursorAlumnos;
DEALLOCATE cursorAlumnos;

PRINT 'Códigos de referido creados: ' + CAST(@CodigosCreados AS VARCHAR);

-- ============================================
-- CREAR USOS DE CÓDIGOS (30 usos)
-- ============================================
PRINT 'Generando usos de códigos de referido (30 usos)...';

-- Alumnos que NO tienen código pero pueden ser referidos
DECLARE @AlumnosReferidos TABLE (IdAlumno UNIQUEIDENTIFIER, FechaInscripcion DATETIME2);

INSERT INTO @AlumnosReferidos (IdAlumno, FechaInscripcion)
SELECT a.IdAlumno, a.FechaInscripcion
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com'
  AND a.IdAlumno NOT IN (SELECT IdAlumno FROM @AlumnosConCodigo)
  AND a.FechaInscripcion > '2024-09-01' -- Solo referidos después de Sep 2024
ORDER BY a.FechaInscripcion;

DECLARE @CodigosDisponibles TABLE (IdCodigoReferido UNIQUEIDENTIFIER, Codigo VARCHAR(10));

INSERT INTO @CodigosDisponibles (IdCodigoReferido, Codigo)
SELECT IdCodigoReferido, Codigo FROM CodigoReferido 
WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosConCodigo);

DECLARE @IdAlumnoReferido UNIQUEIDENTIFIER;
DECLARE @FechaInscripcionReferido DATETIME2;
DECLARE @IdCodigoUsado UNIQUEIDENTIFIER;
DECLARE @FechaUso DATETIME2;
DECLARE @IdUso UNIQUEIDENTIFIER;
DECLARE @UsosCreados INT = 0;

-- Crear 30 usos (o menos si no hay suficientes alumnos)
DECLARE cursorReferidos CURSOR FAST_FORWARD FOR
SELECT TOP 30 IdAlumno, FechaInscripcion FROM @AlumnosReferidos ORDER BY FechaInscripcion;

OPEN cursorReferidos;
FETCH NEXT FROM cursorReferidos INTO @IdAlumnoReferido, @FechaInscripcionReferido;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Seleccionar código aleatorio
    SELECT TOP 1 @IdCodigoUsado = IdCodigoReferido
    FROM @CodigosDisponibles
    ORDER BY NEWID();
    
    IF @IdCodigoUsado IS NOT NULL
    BEGIN
        SET @IdUso = NEWID();
        SET @FechaUso = DATEADD(DAY, -1, @FechaInscripcionReferido); -- Usado 1 día antes de inscripción
        
        INSERT INTO UsoCodigoReferido (IdUso, IdCodigoReferido, IdAlumnoReferido, FechaUso)
        VALUES (@IdUso, @IdCodigoUsado, @IdAlumnoReferido, @FechaUso);
        
        -- Actualizar contador en CodigoReferido
        UPDATE CodigoReferido 
        SET VecesUsado = VecesUsado + 1
        WHERE IdCodigoReferido = @IdCodigoUsado;
        
        SET @UsosCreados = @UsosCreados + 1;
    END;
    
    FETCH NEXT FROM cursorReferidos INTO @IdAlumnoReferido, @FechaInscripcionReferido;
END;

CLOSE cursorReferidos;
DEALLOCATE cursorReferidos;

PRINT 'Usos de códigos generados: ' + CAST(@UsosCreados AS VARCHAR);

-- ============================================
-- ESTADÍSTICAS
-- ============================================
DECLARE @CodigosActivos INT;
DECLARE @PromedioUsosPorCodigo DECIMAL(5,2);

SELECT @CodigosActivos = COUNT(*) 
FROM CodigoReferido 
WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosConCodigo);

SELECT @PromedioUsosPorCodigo = AVG(CAST(VecesUsado AS DECIMAL(5,2)))
FROM CodigoReferido 
WHERE IdAlumno IN (SELECT IdAlumno FROM @AlumnosConCodigo);

PRINT 'Estadísticas del sistema:';
PRINT '  - Códigos activos: ' + CAST(@CodigosActivos AS VARCHAR);
PRINT '  - Usos totales: ' + CAST(@UsosCreados AS VARCHAR);
PRINT '  - Promedio usos/código: ' + CAST(@PromedioUsosPorCodigo AS VARCHAR);

COMMIT TRANSACTION;

PRINT 'Sistema de referidos poblado exitosamente.';
PRINT '========================================';
GO

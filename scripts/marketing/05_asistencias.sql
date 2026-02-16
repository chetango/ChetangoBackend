USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT '========================================';
PRINT 'Creando Asistencias (~2,000 registros)';
PRINT '========================================';

-- Obtener estados de asistencia
DECLARE @EstadoPresente INT = 1;  -- Asumiendo que 1 es Presente
DECLARE @EstadoAusente INT = 2;   -- Asumiendo que 2 es Ausente
DECLARE @EstadoTarde INT = 3;     -- Asumiendo que 3 es Tarde

-- Verificar estados reales
SELECT @EstadoPresente = Id FROM EstadosAsistencia WHERE Nombre = 'Presente';
SELECT @EstadoAusente = Id FROM EstadosAsistencia WHERE Nombre = 'Ausente';
SELECT @EstadoTarde = Id FROM EstadosAsistencia WHERE Nombre = 'Tarde';

PRINT 'Estados encontrados:';
PRINT '  - Presente: ' + CAST(@EstadoPresente AS VARCHAR(10));
PRINT '  - Ausente: ' + CAST(@EstadoAusente AS VARCHAR(10));
PRINT '  - Tarde: ' + CAST(@EstadoTarde AS VARCHAR(10));

-- Tabla temporal para almacenar clases pasadas
DECLARE @ClasesPasadas TABLE (
    IdClase UNIQUEIDENTIFIER,
    Fecha DATE,
    CupoMaximo INT,
    RowNum INT
);

INSERT INTO @ClasesPasadas
SELECT IdClase, Fecha, CupoMaximo, 
       ROW_NUMBER() OVER (ORDER BY Fecha) AS RowNum
FROM Clases 
WHERE Observaciones LIKE '%[MKT]%'
  AND Fecha < GETDATE()  -- Solo clases pasadas
ORDER BY Fecha;

DECLARE @TotalClases INT = (SELECT COUNT(*) FROM @ClasesPasadas);
PRINT 'Clases pasadas encontradas: ' + CAST(@TotalClases AS VARCHAR(10));

-- Tabla temporal para alumnos con paquetes activos
DECLARE @AlumnosConPaquetes TABLE (
    IdAlumno UNIQUEIDENTIFIER,
    IdPaquete UNIQUEIDENTIFIER,
    FechaInscripcion DATE,
    ClasesDisponiblesReal INT,
    RowNum INT
);

INSERT INTO @AlumnosConPaquetes
SELECT 
    a.IdAlumno,
    p.IdPaquete,
    a.FechaInscripcion,
    (p.ClasesDisponibles - ISNULL(p.ClasesUsadas, 0)) AS ClasesDisponiblesReal,
    ROW_NUMBER() OVER (ORDER BY a.FechaInscripcion) AS RowNum
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
INNER JOIN Paquetes p ON p.IdAlumno = a.IdAlumno
WHERE u.Correo LIKE '%@marketing.chetango.com'
  AND (p.ClasesDisponibles - ISNULL(p.ClasesUsadas, 0)) > 0  -- Tiene clases disponibles
ORDER BY a.FechaInscripcion;

DECLARE @TotalAlumnos INT = (SELECT COUNT(*) FROM @AlumnosConPaquetes);
PRINT 'Alumnos con paquetes activos: ' + CAST(@TotalAlumnos AS VARCHAR(10));

-- Crear asistencias con ocupación del 60-80%
DECLARE @ClaseActual UNIQUEIDENTIFIER;
DECLARE @FechaClase DATE;
DECLARE @CupoClase INT;
DECLARE @OcupacionObjetivo INT;
DECLARE @AsistentesCreados INT;
DECLARE @ClaseNum INT = 1;
DECLARE @TotalAsistencias INT = 0;

-- Cursor para recorrer clases
DECLARE @i INT = 1;
WHILE @i <= @TotalClases
BEGIN
    SELECT @ClaseActual = IdClase, @FechaClase = Fecha, @CupoClase = CupoMaximo
    FROM @ClasesPasadas WHERE RowNum = @i;
    
    -- Ocupación aleatoria entre 60% y 80% del cupo
    -- Usamos el número de clase para generar variación determinística
    SET @OcupacionObjetivo = (@CupoClase * (60 + (@i % 21))) / 100;  -- 60-80%
    
    SET @AsistentesCreados = 0;
    
    -- Seleccionar alumnos aleatorios que estaban inscritos en esa fecha
    DECLARE @AlumnosElegibles TABLE (
        IdAlumno UNIQUEIDENTIFIER,
        IdPaquete UNIQUEIDENTIFIER,
        Prioridad INT
    );
    
    INSERT INTO @AlumnosElegibles
    SELECT TOP (@OcupacionObjetivo)
        IdAlumno, 
        IdPaquete,
        ROW_NUMBER() OVER (ORDER BY NEWID()) AS Prioridad
    FROM @AlumnosConPaquetes
    WHERE FechaInscripcion <= @FechaClase  -- Solo alumnos ya inscritos
      AND ClasesDisponiblesReal > 0
    ORDER BY NEWID();  -- Selección aleatoria
    
    -- Crear asistencias para los alumnos seleccionados
    DECLARE @IdAlumnoActual UNIQUEIDENTIFIER;
    DECLARE @IdPaqueteActual UNIQUEIDENTIFIER;
    DECLARE @PrioridadActual INT;
    DECLARE @EstadoAsistencia INT;
    DECLARE @j INT = 1;
    DECLARE @MaxAsistentes INT = (SELECT COUNT(*) FROM @AlumnosElegibles);
    
    WHILE @j <= @MaxAsistentes
    BEGIN
        SELECT @IdAlumnoActual = IdAlumno, 
               @IdPaqueteActual = IdPaquete,
               @PrioridadActual = Prioridad
        FROM (
            SELECT IdAlumno, IdPaquete, Prioridad,
                   ROW_NUMBER() OVER (ORDER BY Prioridad) AS RN
            FROM @AlumnosElegibles
        ) AS T
        WHERE RN = @j;
        
        -- Determinar estado: 85% Presente, 10% Ausente, 5% Tarde
        SET @EstadoAsistencia = CASE 
            WHEN (@j % 20) = 0 THEN @EstadoAusente  -- 5% Ausente
            WHEN (@j % 10) = 0 THEN @EstadoTarde    -- 5% Tarde (cada 10, excepto 20)
            ELSE @EstadoPresente                      -- 85% Presente
        END;
        
        -- Insertar asistencia
        INSERT INTO Asistencias (
            IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado,
            IdEstado, IdTipoAsistencia, FechaRegistro, Observacion
        )
        VALUES (
            NEWID(),
            @ClaseActual,
            @IdAlumnoActual,
            @IdPaqueteActual,
            @EstadoAsistencia,
            1,  -- Tipo Normal (asistencia normal con paquete activo)
            @FechaClase,
            CASE @EstadoAsistencia
                WHEN @EstadoPresente THEN '[MKT] Asistencia registrada'
                WHEN @EstadoAusente THEN '[MKT] Ausencia justificada'
                ELSE '[MKT] Llegada tarde'
            END
        );
        
        -- Actualizar clases usadas del paquete solo si asistió
        IF @EstadoAsistencia = @EstadoPresente OR @EstadoAsistencia = @EstadoTarde
        BEGIN
            UPDATE Paquetes 
            SET ClasesUsadas = ISNULL(ClasesUsadas, 0) + 1
            WHERE IdPaquete = @IdPaqueteActual;
            
            -- Actualizar disponibilidad en tabla temporal
            UPDATE @AlumnosConPaquetes
            SET ClasesDisponiblesReal = ClasesDisponiblesReal - 1
            WHERE IdAlumno = @IdAlumnoActual AND IdPaquete = @IdPaqueteActual;
        END
        
        SET @AsistentesCreados = @AsistentesCreados + 1;
        SET @j = @j + 1;
    END
    
    SET @TotalAsistencias = @TotalAsistencias + @AsistentesCreados;
    
    -- Limpiar tabla temporal para siguiente clase
    DELETE FROM @AlumnosElegibles;
    
    -- Mostrar progreso cada 20 clases
    IF (@i % 20) = 0
    BEGIN
        PRINT '   ✓ ' + CAST(@i AS VARCHAR(10)) + ' clases procesadas... (' 
              + CAST(@TotalAsistencias AS VARCHAR(10)) + ' asistencias)';
    END
    
    SET @i = @i + 1;
END

PRINT '';
PRINT '========================================';
PRINT 'Creadas: ' + CAST(@TotalAsistencias AS VARCHAR(10)) + ' asistencias';
PRINT '  - Para ' + CAST(@TotalClases AS VARCHAR(10)) + ' clases';
PRINT '  - Con ' + CAST(@TotalAlumnos AS VARCHAR(10)) + ' alumnos';
PRINT '  - Ocupación promedio: ' + CAST((@TotalAsistencias * 100 / @TotalClases) AS VARCHAR(10)) + '%';
PRINT '========================================';

GO

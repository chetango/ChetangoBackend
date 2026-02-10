-- =============================================
-- SEED: EVENTOS PARA CARRUSEL DEL DASHBOARD
-- =============================================
-- Descripción: Crea dos eventos de ejemplo para mostrar en el carrusel del dashboard del alumno
-- Fecha: 6 de Febrero 2026
-- =============================================

-- Obtener el ID del primer usuario (usualmente el admin)
-- Los roles se manejan en Azure Entra ID, no en la BD
DECLARE @IdUsuarioCreador UNIQUEIDENTIFIER = (
    SELECT TOP 1 IdUsuario 
    FROM Usuarios
    WHERE Correo LIKE '%admin%' OR Correo LIKE '%chetango%'
    ORDER BY FechaCreacion
);

-- Si no encuentra admin, usar el primer usuario disponible
IF @IdUsuarioCreador IS NULL
BEGIN
    SET @IdUsuarioCreador = (SELECT TOP 1 IdUsuario FROM Usuarios ORDER BY FechaCreacion);
END

-- Verificar que existe al menos un usuario
IF @IdUsuarioCreador IS NULL
BEGIN
    PRINT 'ERROR: No se encontró ningún usuario en la base de datos.';
    RETURN;
END

PRINT '📌 Usuario creador: ' + CAST(@IdUsuarioCreador AS NVARCHAR(50));

-- =============================================
-- EVENTO 1: Seminario Especial de Tango
-- =============================================
DECLARE @IdEvento1 UNIQUEIDENTIFIER = NEWID();

-- Verificar si ya existe un evento similar
IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo LIKE '%Seminario%Jorge Padilla%Ana Gomez%')
BEGIN
    INSERT INTO Eventos (
        IdEvento,
        Titulo,
        Descripcion,
        Fecha,
        Hora,
        Precio,
        Destacado,
        ImagenUrl,
        ImagenNombre,
        Activo,
        FechaCreacion,
        FechaModificacion,
        IdUsuarioCreador
    )
    VALUES (
        @IdEvento1,
        N'Seminario Especial de Tango',
        N'Únete a un seminario único con los reconocidos maestros Jorge Padilla y Ana Gómez. Explora técnicas avanzadas de tango, musicalidad y conexión en pareja. ¡Cupos limitados!',
        '2026-02-22',  -- 22 de febrero 2026 (sábado)
        '15:00:00',    -- 3:00 PM
        35000.00,      -- $35,000 pesos
        1,             -- Destacado = true
        '/uploads/eventos/seminario-tango-padilla-gomez.jpeg',
        'seminario-tango-padilla-gomez.jpeg',
        1,             -- Activo = true
        GETDATE(),
        NULL,
        @IdUsuarioCreador
    );
    
    PRINT '✅ Evento 1 creado: Seminario Especial de Tango con Jorge Padilla y Ana Gómez';
END
ELSE
BEGIN
    PRINT '⚠️ Ya existe un evento similar al Seminario de Tango';
END

-- =============================================
-- EVENTO 2: Taller de Técnica Masculina
-- =============================================
DECLARE @IdEvento2 UNIQUEIDENTIFIER = NEWID();

-- Verificar si ya existe un evento similar
IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo LIKE '%Taller%Técnica Masculina%')
BEGIN
    INSERT INTO Eventos (
        IdEvento,
        Titulo,
        Descripcion,
        Fecha,
        Hora,
        Precio,
        Destacado,
        ImagenUrl,
        ImagenNombre,
        Activo,
        FechaCreacion,
        FechaModificacion,
        IdUsuarioCreador
    )
    VALUES (
        @IdEvento2,
        N'Taller de Técnica Masculina',
        N'Taller especializado para el rol masculino en el tango. El maestro Jorge Padilla te enseñará técnicas de liderazgo, marcación y disociación para llevar tu baile al siguiente nivel.',
        '2026-02-15',  -- 15 de febrero 2026 (domingo)
        '17:00:00',    -- 5:00 PM
        25000.00,      -- $25,000 pesos
        0,             -- Destacado = false
        '/uploads/eventos/taller-tecnica-masculina.jpeg',
        'taller-tecnica-masculina.jpeg',
        1,             -- Activo = true
        GETDATE(),
        NULL,
        @IdUsuarioCreador
    );
    
    PRINT '✅ Evento 2 creado: Taller de Técnica Masculina con Jorge Padilla';
END
ELSE
BEGIN
    PRINT '⚠️ Ya existe un evento similar al Taller de Técnica Masculina';
END

-- =============================================
-- VERIFICACIÓN FINAL
-- =============================================
PRINT '';
PRINT '================================================';
PRINT 'RESUMEN DE EVENTOS ACTIVOS Y FUTUROS:';
PRINT '================================================';

SELECT 
    Titulo,
    Fecha,
    Hora,
    Precio,
    CASE WHEN Destacado = 1 THEN 'Sí' ELSE 'No' END AS Destacado,
    ImagenUrl
FROM Eventos
WHERE Activo = 1 AND Fecha >= CAST(GETDATE() AS DATE)
ORDER BY Fecha, Hora;

PRINT '';
PRINT '✅ Script completado exitosamente';
PRINT '📌 Los eventos aparecerán en el dashboard del alumno automáticamente';

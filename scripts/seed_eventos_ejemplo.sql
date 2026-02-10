-- ================================================
-- SEED DE EVENTOS DE EJEMPLO - CHETANGO
-- ================================================
-- Este script inserta 3 eventos de ejemplo para probar el dashboard del alumno
-- Ejecutar después de crear la tabla Eventos

USE ChetangoDB_Dev;
GO

-- Obtener un IdUsuario admin existente para usar como creador
DECLARE @IdUsuarioAdmin UNIQUEIDENTIFIER;
SELECT TOP 1 @IdUsuarioAdmin = IdUsuario 
FROM Usuarios 
WHERE Correo = 'Chetango@chetangoprueba.onmicrosoft.com';

-- Si no existe el admin, usar el primer usuario disponible
IF @IdUsuarioAdmin IS NULL
BEGIN
    SELECT TOP 1 @IdUsuarioAdmin = IdUsuario FROM Usuarios;
END

-- Verificar que tenemos un usuario
IF @IdUsuarioAdmin IS NOT NULL
BEGIN
    PRINT 'Insertando eventos de ejemplo...';
    PRINT 'Usuario creador: ' + CAST(@IdUsuarioAdmin AS NVARCHAR(50));

    -- Evento 1: Taller de Musicalidad (Destacado)
    IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Taller de Musicalidad')
    BEGIN
        INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, Activo, FechaCreacion, IdUsuarioCreador)
        VALUES (
            NEWID(),
            'Taller de Musicalidad',
            'Mejora tu interpretación musical en el tango. Aprende a escuchar y expresar la música a través del movimiento.',
            DATEADD(DAY, 9, GETDATE()), -- 9 días desde hoy
            '15:00:00',
            25000,
            1, -- Destacado
            'https://images.unsplash.com/photo-1504609813442-a8924e83f76e?w=400&q=80',
            1,
            GETDATE(),
            @IdUsuarioAdmin
        );
        PRINT '✓ Evento 1: Taller de Musicalidad insertado';
    END

    -- Evento 2: Milonga de Práctica (Gratis)
    IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Milonga de Práctica')
    BEGIN
        INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, Activo, FechaCreacion, IdUsuarioCreador)
        VALUES (
            NEWID(),
            'Milonga de Práctica',
            'Noche de baile social para practicar lo aprendido en clases. Ambiente relajado y amigable para todos los niveles.',
            DATEADD(DAY, 4, GETDATE()), -- 4 días desde hoy
            '21:00:00',
            NULL, -- Gratis
            0,
            'https://images.unsplash.com/photo-1571156230214-50e0b9d14d45?w=400&q=80',
            1,
            GETDATE(),
            @IdUsuarioAdmin
        );
        PRINT '✓ Evento 2: Milonga de Práctica insertado';
    END

    -- Evento 3: Workshop Avanzado
    IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Workshop Avanzado de Tango')
    BEGIN
        INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, Activo, FechaCreacion, IdUsuarioCreador)
        VALUES (
            NEWID(),
            'Workshop Avanzado de Tango',
            'Técnicas avanzadas de tango para bailarines experimentados. Incluye boleos, ganchos y sacadas complejas.',
            DATEADD(DAY, 16, GETDATE()), -- 16 días desde hoy
            '16:00:00',
            40000,
            0,
            'https://images.unsplash.com/photo-1508700929628-666bc8bd84ea?w=400&q=80',
            1,
            GETDATE(),
            @IdUsuarioAdmin
        );
        PRINT '✓ Evento 3: Workshop Avanzado insertado';
    END

    PRINT '';
    PRINT 'Seed de eventos completado exitosamente.';
    PRINT 'Total de eventos activos: ' + CAST((SELECT COUNT(*) FROM Eventos WHERE Activo = 1) AS NVARCHAR(10));
END
ELSE
BEGIN
    PRINT 'ERROR: No se encontró ningún usuario en la tabla Usuarios.';
    PRINT 'Ejecuta primero el script de seed de usuarios.';
END
GO

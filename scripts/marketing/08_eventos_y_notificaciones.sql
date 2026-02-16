/********************************************************************************************************
 Script: 08_eventos_y_notificaciones.sql
 Objetivo: Crear eventos pasados/futuros y notificaciones para engagement
           12 eventos distribuidos, 80 notificaciones enviadas
 Fecha: Febrero 2025
 Uso: Marketing video - Carrusel eventos, comunicación con alumnos
*********************************************************************************************************/

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

PRINT 'Iniciando población de eventos y notificaciones...';

DECLARE @TipoEventoWorkshop UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @TipoEventoCompetencia UNIQUEIDENTIFIER = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @TipoEventoSocial UNIQUEIDENTIFIER = 'cccccccc-cccc-cccc-cccc-cccccccccccc';
DECLARE @TipoEventoMasterclass UNIQUEIDENTIFIER = 'dddddddd-dddd-dddd-dddd-dddddddddddd';

BEGIN TRANSACTION;

-- ============================================
-- LIMPIAR EVENTOS Y NOTIFICACIONES
-- ============================================
PRINT 'Limpiando eventos y notificaciones previas...';

DELETE FROM Notificaciones WHERE Mensaje LIKE '%[MKT]%';
DELETE FROM Eventos WHERE Titulo LIKE '%[MKT]%';

PRINT 'Limpieza completada.';

-- ============================================
-- CREAR EVENTOS (12 eventos)
-- ============================================
PRINT 'Creando 12 eventos distribuidos...';

-- Evento 1: Workshop pasado (Oct 2024)
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(), 
    'Workshop de Giros Avanzados [MKT]',
    'Taller especializado en técnicas de giros para nivel intermedio-avanzado',
    '2024-10-15 14:00:00',
    'Studio Principal - Chetango',
    @TipoEventoWorkshop,
    30,
    '/uploads/eventos/workshop-giros.jpg',
    1
);

-- Evento 2: Social (Nov 2024)
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Rumba Social Noviembre [MKT]',
    'Noche de baile social con música en vivo y competencias amistosas',
    '2024-11-20 19:00:00',
    'Salón de Eventos Chetango',
    @TipoEventoSocial,
    80,
    '/uploads/eventos/social-nov.jpg',
    1
);

-- Evento 3: Masterclass (Dic 2024)
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Masterclass con Campeones Nacionales [MKT]',
    'Clase magistral con los campeones nacionales de salsa categoría profesional',
    '2024-12-05 16:00:00',
    'Studio Principal - Chetango',
    @TipoEventoMasterclass,
    40,
    '/uploads/eventos/masterclass-dic.jpg',
    1
);

-- Evento 4: Competencia (Dic 2024)
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Competencia Interna de Fin de Año [MKT]',
    'Competencia amistosa entre alumnos con premios y reconocimientos',
    '2024-12-20 18:00:00',
    'Salón de Eventos Chetango',
    @TipoEventoCompetencia,
    100,
    '/uploads/eventos/competencia-dic.jpg',
    1
);

-- Evento 5: Workshop (Ene 2025)
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Workshop de Shines y Footwork [MKT]',
    'Taller enfocado en pasos individuales y trabajo de pies',
    '2025-01-10 15:00:00',
    'Studio 2 - Chetango',
    @TipoEventoWorkshop,
    25,
    '/uploads/eventos/workshop-shines.jpg',
    1
);

-- Evento 6: Social (Ene 2025)
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Social de Año Nuevo [MKT]',
    'Celebración de inicio de año con todos los alumnos y profesores',
    '2025-01-25 20:00:00',
    'Salón de Eventos Chetango',
    @TipoEventoSocial,
    90,
    '/uploads/eventos/social-ene.jpg',
    1
);

-- Evento 7: Masterclass pasada (Feb 2025)
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Masterclass de Bachata Fusion [MKT]',
    'Clase especial de fusión de bachata con estilos modernos',
    '2025-02-01 17:00:00',
    'Studio Principal - Chetango',
    @TipoEventoMasterclass,
    35,
    '/uploads/eventos/masterclass-bachata.jpg',
    1
);

-- Eventos FUTUROS para el video

-- Evento 8: Workshop próximo
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Workshop de Coreografía Avanzada [MKT]',
    'Creación de coreografías grupales para presentaciones',
    '2025-02-20 16:00:00',
    'Studio Principal - Chetango',
    @TipoEventoWorkshop,
    30,
    '/uploads/eventos/workshop-choreo.jpg',
    1
);

-- Evento 9: Social próxima
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Rumba Social Febrero [MKT]',
    'Noche de baile con DJ invitado y concursos',
    '2025-02-28 19:30:00',
    'Salón de Eventos Chetango',
    @TipoEventoSocial,
    85,
    '/uploads/eventos/social-feb.jpg',
    1
);

-- Evento 10: Masterclass próxima
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Masterclass de Son Cubano [MKT]',
    'Clase especializada en técnicas tradicionales del son',
    '2025-03-05 15:00:00',
    'Studio Principal - Chetango',
    @TipoEventoMasterclass,
    40,
    '/uploads/eventos/masterclass-son.jpg',
    1
);

-- Evento 11: Competencia próxima
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Competencia Regional de Salsa [MKT]',
    'Competencia abierta con categorías amateur y profesional',
    '2025-03-15 18:00:00',
    'Centro de Convenciones',
    @TipoEventoCompetencia,
    150,
    '/uploads/eventos/competencia-regional.jpg',
    1
);

-- Evento 12: Workshop lejano
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, FechaEvento, Ubicacion, IdTipoEvento, CapacidadMaxima, ImagenUrl, EsActivo)
VALUES (
    NEWID(),
    'Workshop de Estilos de Salsa del Mundo [MKT]',
    'Exploración de diferentes estilos: Caleña, On2, Cubana, LA Style',
    '2025-03-25 14:00:00',
    'Studio Principal - Chetango',
    @TipoEventoWorkshop,
    35,
    '/uploads/eventos/workshop-estilos.jpg',
    1
);

PRINT '12 eventos creados (7 pasados, 5 futuros).';

-- ============================================
-- CREAR NOTIFICACIONES (80 notificaciones)
-- ============================================
PRINT 'Generando 80 notificaciones para alumnos...';

DECLARE @AlumnosMarketing TABLE (IdUsuario UNIQUEIDENTIFIER, NumAlumno INT);

INSERT INTO @AlumnosMarketing (IdUsuario, NumAlumno)
SELECT u.IdUsuario, ROW_NUMBER() OVER (ORDER BY a.FechaInscripcion)
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com';

-- Notificaciones variadas: pagos, clases, eventos, referidos
DECLARE @TiposNotificacion TABLE (Tipo NVARCHAR(100), Mensaje NVARCHAR(500));

INSERT INTO @TiposNotificacion (Tipo, Mensaje) VALUES
('Pago Confirmado', 'Tu pago de $XXX ha sido confirmado exitosamente [MKT]'),
('Clase Próxima', 'Recordatorio: Tienes clase mañana a las XX:XX hrs [MKT]'),
('Evento Nuevo', 'Nuevo evento disponible: Workshop de Giros Avanzados [MKT]'),
('Paquete Bajo', 'Te quedan solo 3 clases en tu paquete actual [MKT]'),
('Referido Exitoso', 'Tu código de referido fue usado. Ganaste descuento! [MKT]'),
('Bienvenida', 'Bienvenido a Chetango! Prepárate para tu primera clase [MKT]'),
('Ausencia', 'Notamos tu ausencia en la clase de hoy. Todo bien? [MKT]'),
('Felicitación', 'Felicitaciones por completar 10 clases! Sigue así [MKT]');

DECLARE @IdUsuario UNIQUEIDENTIFIER;
DECLARE @NumAlumno INT;
DECLARE @TipoNotif NVARCHAR(100);
DECLARE @MensajeNotif NVARCHAR(500);
DECLARE @FechaNotif DATETIME2;
DECLARE @NotifCreadas INT = 0;

-- Generar 80 notificaciones distribuidas
DECLARE @i INT = 1;
WHILE @i <= 80
BEGIN
    -- Seleccionar alumno aleatorio
    SELECT TOP 1 @IdUsuario = IdUsuario
    FROM @AlumnosMarketing
    ORDER BY NEWID();
    
    -- Seleccionar tipo aleatorio
    SELECT TOP 1 @TipoNotif = Tipo, @MensajeNotif = Mensaje
    FROM @TiposNotificacion
    ORDER BY NEWID();
    
    -- Fecha entre Ago 2024 y Feb 2025
    SET @FechaNotif = DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 180), '2024-08-01');
    
    INSERT INTO Notificaciones (IdNotificacion, IdUsuario, Titulo, Mensaje, FechaEnvio, EsLeida)
    VALUES (
        NEWID(),
        @IdUsuario,
        @TipoNotif,
        @MensajeNotif,
        @FechaNotif,
        CASE WHEN @FechaNotif < DATEADD(DAY, -7, GETDATE()) THEN 1 ELSE 0 END -- Leídas si son viejas
    );
    
    SET @NotifCreadas = @NotifCreadas + 1;
    SET @i = @i + 1;
END;

PRINT 'Notificaciones creadas: ' + CAST(@NotifCreadas AS VARCHAR);

-- ============================================
-- ESTADÍSTICAS
-- ============================================
DECLARE @TotalEventos INT;
DECLARE @EventosPasados INT;
DECLARE @EventosFuturos INT;
DECLARE @NotifLeidas INT;
DECLARE @NotifNoLeidas INT;

SELECT @TotalEventos = COUNT(*),
       @EventosPasados = SUM(CASE WHEN FechaEvento < GETDATE() THEN 1 ELSE 0 END),
       @EventosFuturos = SUM(CASE WHEN FechaEvento >= GETDATE() THEN 1 ELSE 0 END)
FROM Eventos WHERE Titulo LIKE '%[MKT]%';

SELECT @NotifLeidas = SUM(CASE WHEN EsLeida = 1 THEN 1 ELSE 0 END),
       @NotifNoLeidas = SUM(CASE WHEN EsLeida = 0 THEN 1 ELSE 0 END)
FROM Notificaciones WHERE Mensaje LIKE '%[MKT]%';

PRINT 'Estadísticas:';
PRINT '  - Eventos totales: ' + CAST(@TotalEventos AS VARCHAR);
PRINT '    * Pasados: ' + CAST(@EventosPasados AS VARCHAR);
PRINT '    * Futuros: ' + CAST(@EventosFuturos AS VARCHAR);
PRINT '  - Notificaciones: ' + CAST(@NotifCreadas AS VARCHAR);
PRINT '    * Leídas: ' + CAST(@NotifLeidas AS VARCHAR);
PRINT '    * No leídas: ' + CAST(@NotifNoLeidas AS VARCHAR);

COMMIT TRANSACTION;

PRINT 'Eventos y notificaciones poblados exitosamente.';
PRINT '========================================';
GO

-- =============================================
-- FIX: Corregir codificación UTF-8 en Eventos
-- =============================================
-- Problema: Los caracteres especiales (tildes, ñ) se guardaron incorrectamente
-- Solución: Actualizar con prefijo N'' para Unicode
-- =============================================

-- Evento 1: Taller de Técnica Masculina
UPDATE Eventos
SET 
    Titulo = N'Taller de Técnica Masculina',
    Descripcion = N'Taller especializado para el rol masculino en el tango. El maestro Jorge Padilla te enseñará técnicas de liderazgo, marcación y disociación para llevar tu baile al siguiente nivel.'
WHERE Titulo LIKE '%Taller%Masculina%' OR Titulo LIKE '%T_cnica%';

PRINT '✅ Evento 1 actualizado: Taller de Técnica Masculina';

-- Evento 2: Seminario Especial de Tango
UPDATE Eventos
SET 
    Titulo = N'Seminario Especial de Tango',
    Descripcion = N'Únete a un seminario único con los reconocidos maestros Jorge Padilla y Ana Gómez. Explora técnicas avanzadas de tango, musicalidad y conexión en pareja. ¡Cupos limitados!'
WHERE Titulo LIKE '%Seminario%Tango%';

PRINT '✅ Evento 2 actualizado: Seminario Especial de Tango';

-- Verificar resultados
PRINT '';
PRINT '================================================';
PRINT 'EVENTOS ACTUALIZADOS:';
PRINT '================================================';

SELECT 
    Titulo,
    LEFT(Descripcion, 50) + '...' AS Descripcion,
    Fecha,
    Precio
FROM Eventos
WHERE Activo = 1 AND Fecha >= CAST(GETDATE() AS DATE)
ORDER BY Fecha;

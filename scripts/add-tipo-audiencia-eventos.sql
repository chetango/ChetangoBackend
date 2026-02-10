-- =============================================
-- MIGRATION: Agregar TipoAudiencia a Eventos
-- =============================================
-- Fecha: 6 de Febrero 2026
-- Descripción: Permite filtrar eventos por audiencia (Alumno, Profesor, Todos)
-- =============================================

-- Agregar columna TipoAudiencia
ALTER TABLE Eventos
ADD TipoAudiencia NVARCHAR(50) NOT NULL DEFAULT 'Todos';

PRINT '✅ Columna TipoAudiencia agregada a tabla Eventos';

-- Actualizar eventos existentes según su contenido
UPDATE Eventos SET TipoAudiencia = 'Alumno' 
WHERE Titulo IN (N'Beneficios del Tango para tu Salud', N'Códigos de la Milonga');

UPDATE Eventos SET TipoAudiencia = 'Profesor' 
WHERE Titulo IN (N'Técnicas de Enseñanza Efectiva', N'Motivación de Alumnos');

PRINT '✅ Eventos clasificados por audiencia';

-- Verificar resultado
PRINT '';
PRINT '================================================';
PRINT 'EVENTOS POR AUDIENCIA:';
PRINT '================================================';

SELECT 
    TipoAudiencia,
    Titulo,
    Fecha,
    CASE WHEN Destacado = 1 THEN 'Sí' ELSE 'No' END AS Destacado
FROM Eventos
WHERE Activo = 1
ORDER BY TipoAudiencia, Fecha;

PRINT '';
PRINT '✅ Migración completada exitosamente';

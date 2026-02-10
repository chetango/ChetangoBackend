-- ============================================
-- AGREGAR COLUMNAS PARA CONFIRMACIÓN DE ASISTENCIA
-- Permite que alumnos confirmen su asistencia
-- ============================================

USE ChetangoDB_Dev;
GO

-- Agregar columnas a la tabla Asistencias
ALTER TABLE Asistencias
ADD Confirmado BIT NOT NULL DEFAULT 0,
    FechaConfirmacion DATETIME2 NULL;
GO

-- Índice para consultas de asistencias pendientes de confirmar
SET QUOTED_IDENTIFIER ON;
GO

CREATE NONCLUSTERED INDEX IX_Asistencias_Confirmado
ON Asistencias(IdAlumno, Confirmado)
WHERE Confirmado = 0;
GO

PRINT 'Columnas de confirmación agregadas exitosamente a la tabla Asistencias';
GO

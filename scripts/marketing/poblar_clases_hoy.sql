-- ================================================
-- POBLAR CLASES Y ASISTENCIAS PARA HOY
-- Fecha: 18 de Febrero de 2026
-- Para dashboard: Asistencias Registradas HOY
-- ================================================

USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;

DECLARE @FechaHoy DATE = '2026-02-18';
DECLARE @IdTipoClase UNIQUEIDENTIFIER = '3F53914A-56DF-4F5F-9E51-053C993C2125'; -- Salsa
DECLARE @IdRol UNIQUEIDENTIFIER = '12121212-1212-1212-1212-121212121212'; -- Rol principal

-- Seleccionar profesores de marketing
DECLARE @Profesores TABLE (Seq INT IDENTITY(1,1), IdProfesor UNIQUEIDENTIFIER);
INSERT INTO @Profesores (IdProfesor)
SELECT TOP 5 p.IdProfesor 
FROM Profesores p 
INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
WHERE u.Correo LIKE '%@marketing.chetango.com'
ORDER BY NEWID();

-- Seleccionar alumnos de marketing con paquetes activos
DECLARE @Alumnos TABLE (Seq INT IDENTITY(1,1), IdAlumno UNIQUEIDENTIFIER, IdPaqueteUsado UNIQUEIDENTIFIER);
INSERT INTO @Alumnos (IdAlumno, IdPaqueteUsado)
SELECT TOP 40 a.IdAlumno, paq.IdPaquete
FROM Alumnos a
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
INNER JOIN Paquetes paq ON paq.IdAlumno = a.IdAlumno
WHERE u.Correo LIKE '%@marketing.chetango.com'
  AND paq.IdEstado = 1
  AND paq.ClasesUsadas < paq.ClasesDisponibles
ORDER BY NEWID();

PRINT '=== CREANDO 5 CLASES PARA HOY (18 FEB 2026) ===';
PRINT '';

-- Clase 1: 09:00-10:30
DECLARE @IdC1 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdP1 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE Seq = 1);
INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo, Estado)
VALUES (@IdC1, @FechaHoy, '09:00:00', '10:30:00', @IdTipoClase, @IdP1, 15, 'Programada');

INSERT INTO ClasesProfesores (IdClaseProfesor, IdClase, IdProfesor, IdRolEnClase, TarifaProgramada, ValorAdicional, TotalPago, EstadoPago)
VALUES (NEWID(), @IdC1, @IdP1, @IdRol, 30000, 0, 30000, 'Pendiente');

-- 8 asistencias para clase 1
INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, IdTipoAsistencia, Observacion)
SELECT NEWID(), @IdC1, IdAlumno, IdPaqueteUsado, 1, 1, 'Asistencia HOY [MKT]'
FROM (SELECT TOP 8 * FROM @Alumnos ORDER BY Seq) AS T;

-- Clase 2: 11:00-12:30
DECLARE @IdC2 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdP2 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE Seq = 2);
INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo, Estado)
VALUES (@IdC2, @FechaHoy, '11:00:00', '12:30:00', @IdTipoClase, @IdP2, 15, 'Programada');

INSERT INTO ClasesProfesores (IdClaseProfesor, IdClase, IdProfesor, IdRolEnClase, TarifaProgramada, ValorAdicional, TotalPago, EstadoPago)
VALUES (NEWID(), @IdC2, @IdP2, @IdRol, 30000, 0, 30000, 'Pendiente');

INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, IdTipoAsistencia, Observacion)
SELECT NEWID(), @IdC2, IdAlumno, IdPaqueteUsado, 1, 1, 'Asistencia HOY [MKT]'
FROM (SELECT * FROM @Alumnos WHERE Seq BETWEEN 9 AND 16) AS T;

-- Clase 3: 15:00-16:30
DECLARE @IdC3 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdP3 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE Seq = 3);
INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo, Estado)
VALUES (@IdC3, @FechaHoy, '15:00:00', '16:30:00', @IdTipoClase, @IdP3, 15, 'Programada');

INSERT INTO ClasesProfesores (IdClaseProfesor, IdClase, IdProfesor, IdRolEnClase, TarifaProgramada, ValorAdicional, TotalPago, EstadoPago)
VALUES (NEWID(), @IdC3, @IdP3, @IdRol, 30000, 0, 30000, 'Pendiente');

INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, IdTipoAsistencia, Observacion)
SELECT NEWID(), @IdC3, IdAlumno, IdPaqueteUsado, 1, 1, 'Asistencia HOY [MKT]'
FROM (SELECT * FROM @Alumnos WHERE Seq BETWEEN 17 AND 24) AS T;

-- Clase 4: 17:00-18:30
DECLARE @IdC4 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdP4 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE Seq = 4);
INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo, Estado)
VALUES (@IdC4, @FechaHoy, '17:00:00', '18:30:00', @IdTipoClase, @IdP4, 15, 'Programada');

INSERT INTO ClasesProfesores (IdClaseProfesor, IdClase, IdProfesor, IdRolEnClase, TarifaProgramada, ValorAdicional, TotalPago, EstadoPago)
VALUES (NEWID(), @IdC4, @IdP4, @IdRol, 30000, 0, 30000, 'Pendiente');

INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, IdTipoAsistencia, Observacion)
SELECT NEWID(), @IdC4, IdAlumno, IdPaqueteUsado, 1, 1, 'Asistencia HOY [MKT]'
FROM (SELECT * FROM @Alumnos WHERE Seq BETWEEN 25 AND 32) AS T;

-- Clase 5: 19:00-20:30
DECLARE @IdC5 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdP5 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @Profesores WHERE Seq = 5);
INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal, CupoMaximo, Estado)
VALUES (@IdC5, @FechaHoy, '19:00:00', '20:30:00', @IdTipoClase, @IdP5, 15, 'Programada');

INSERT INTO ClasesProfesores (IdClaseProfesor, IdClase, IdProfesor, IdRolEnClase, TarifaProgramada, ValorAdicional, TotalPago, EstadoPago)
VALUES (NEWID(), @IdC5, @IdP5, @IdRol, 30000, 0, 30000, 'Pendiente');

INSERT INTO Asistencias (IdAsistencia, IdClase, IdAlumno, IdPaqueteUsado, IdEstado, IdTipoAsistencia, Observacion)
SELECT NEWID(), @IdC5, IdAlumno, IdPaqueteUsado, 1, 1, 'Asistencia HOY [MKT]'
FROM (SELECT * FROM @Alumnos WHERE Seq BETWEEN 33 AND 40) AS T;

PRINT '5 clases creadas con 40 asistencias totales';
PRINT '';

-- Resumen
PRINT '=== RESUMEN FINAL ===';
SELECT 'Clases HOY (18 Feb 2026)' AS Concepto, COUNT(DISTINCT c.IdClase) AS Cantidad
FROM Clases c
WHERE CAST(c.Fecha AS DATE) = @FechaHoy;

SELECT 'Asistencias Registradas HOY' AS Concepto, COUNT(*) AS Cantidad
FROM Asistencias a
INNER JOIN Clases c ON a.IdClase = c.IdClase
WHERE CAST(c.Fecha AS DATE) = @FechaHoy;

PRINT '';
PRINT 'Dashboard: Tarjeta "Asistencias Registradas HOY" actualizada';

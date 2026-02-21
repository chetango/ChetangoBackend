-- Script para verificar estados de pagos
-- Base de datos: ChetangoDB_Dev

-- Ver todos los estados de pago disponibles
PRINT '=== ESTADOS DE PAGO DISPONIBLES ===';
SELECT Id, Nombre, Descripcion, Activo FROM EstadosPago;

-- Ver columnas de la tabla Pagos
PRINT '';
PRINT '=== ESTRUCTURA TABLA PAGOS ===';
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Pagos' 
ORDER BY ORDINAL_POSITION;

-- Ver los pagos con su estado (sin métodos de pago por ahora)
PRINT '';
PRINT '=== PAGOS CON ESTADOS ===';
SELECT 
    p.IdPago,
    p.FechaPago,
    p.MontoTotal,
    u.NombreUsuario AS Alumno,
    e.Nombre AS Estado,
    p.IdEstadoPago,
    p.Eliminado
FROM Pagos p
INNER JOIN Alumnos a ON p.IdAlumno = a.IdAlumno
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
INNER JOIN EstadosPago e ON p.IdEstadoPago = e.Id
WHERE p.Eliminado = 0
ORDER BY p.FechaPago DESC;

-- Contar pagos por estado
PRINT '';
PRINT '=== CONTEO POR ESTADO ===';
SELECT 
    e.Nombre AS Estado,
    COUNT(*) AS CantidadPagos
FROM Pagos p
INNER JOIN EstadosPago e ON p.IdEstadoPago = e.Id
WHERE p.Eliminado = 0
GROUP BY e.Nombre;

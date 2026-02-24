-- Descubrir nombres exactos de columnas en Paquetes
SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Paquetes'
ORDER BY ORDINAL_POSITION;

-- Ver todos los estados disponibles
SELECT * FROM EstadosPaquete;

-- Ver paquete de Isabella sin usar IdEstado (aún no sabemos el nombre real)
SELECT TOP 5 *
FROM Paquetes p
INNER JOIN Alumnos al ON p.IdAlumno = al.IdAlumno
INNER JOIN Usuarios u ON al.IdUsuario = u.IdUsuario
WHERE u.NumeroDocumento = '573212847256';

-- CONFIRMADO: IdEstado = 4 es "Agotado" en EstadosPaquete
-- Estados: 1=Activo, 2=Vencido, 3=Congelado, 4=Agotado

-- Paquetes con estado Agotado (IdEstado = 4) - SIN JOIN a EstadosPaquete por ahora
SELECT 
    p.IdPaquete,
    u.NombreUsuario,
    p.ClasesDisponibles,
    p.ClasesUsadas,
    (p.ClasesDisponibles - p.ClasesUsadas) as ClasesRestantes
FROM Paquetes p
INNER JOIN Alumnos al ON p.IdAlumno = al.IdAlumno
INNER JOIN Usuarios u ON al.IdUsuario = u.IdUsuario
WHERE p.IdEstado = 4  -- Estado Agotado
ORDER BY u.NombreUsuario;

-- Resumen por sede de paquetes agotados (ENUM: Medellin=1, Manizales=2)
SELECT 
    CASE 
        WHEN pg.Sede = 1 THEN 'Medellin'
        WHEN pg.Sede = 2 THEN 'Manizales'
        WHEN pg.Sede IS NULL THEN 'Sin Pago'
        ELSE 'Desconocido'
    END as Sede,
    COUNT(*) as PaquetesAgotados
FROM Paquetes p
LEFT JOIN Pagos pg ON p.IdPago = pg.IdPago
WHERE p.IdEstado = 4  -- Estado Agotado
GROUP BY pg.Sede;

-- Total general de paquetes agotados
SELECT 
    COUNT(*) as TotalPaquetesAgotados
FROM Paquetes p
WHERE p.IdEstado = 4;  -- Estado Agotado

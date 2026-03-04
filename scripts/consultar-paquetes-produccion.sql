-- Consultar nombres de paquetes en producción
USE [chetango-db-prod];
GO

SELECT 
    Nombre,
    NumeroClases,
    Precio,
    Activo
FROM dbo.TiposPaquete
ORDER BY Nombre;

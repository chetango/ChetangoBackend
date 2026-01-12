-- Script para insertar los métodos de pago en la base de datos
-- Base de datos: ChetangoDB_Dev
-- Módulo: Pagos

USE ChetangoDB_Dev;
GO

-- Insertar métodos de pago si no existen
IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Nombre = 'Efectivo')
BEGIN
    INSERT INTO MetodosPago (Id, Nombre)
    VALUES (NEWID(), 'Efectivo');
END

IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Nombre = 'Transferencia Bancaria')
BEGIN
    INSERT INTO MetodosPago (Id, Nombre)
    VALUES (NEWID(), 'Transferencia Bancaria');
END

IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Nombre = 'Tarjeta Débito')
BEGIN
    INSERT INTO MetodosPago (Id, Nombre)
    VALUES (NEWID(), 'Tarjeta Débito');
END

IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Nombre = 'Tarjeta Crédito')
BEGIN
    INSERT INTO MetodosPago (Id, Nombre)
    VALUES (NEWID(), 'Tarjeta Crédito');
END

IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Nombre = 'Nequi')
BEGIN
    INSERT INTO MetodosPago (Id, Nombre)
    VALUES (NEWID(), 'Nequi');
END

IF NOT EXISTS (SELECT 1 FROM MetodosPago WHERE Nombre = 'Daviplata')
BEGIN
    INSERT INTO MetodosPago (Id, Nombre)
    VALUES (NEWID(), 'Daviplata');
END

-- Verificar los métodos de pago insertados
SELECT * FROM MetodosPago;
GO

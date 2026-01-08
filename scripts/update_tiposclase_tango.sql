/********************************************************************************************************
 Script: update_tiposclase_tango.sql
 Objetivo: Renombrar/crear tipos de clase para mostrar nombres específicos de Tango y dejar una opción para
           clases privadas. Debe ejecutarse en QA antes de correr las pruebas de admin asistencias.
*********************************************************************************************************/

-- USE [ChetangoDB_QA]; -- << reemplaza por tu base antes de ejecutar

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @TipoClaseRegular UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';
DECLARE @TipoClaseTaller  UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555555';
DECLARE @TipoClaseEvento  UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666666';
DECLARE @TipoClasePrivado UNIQUEIDENTIFIER = '77777777-aaaa-bbbb-cccc-111111111111'; -- nuevo registro

BEGIN TRAN;
    UPDATE TiposClase
        SET Nombre = 'Tango Salón'
        WHERE Id = @TipoClaseRegular;

    UPDATE TiposClase
        SET Nombre = 'Tango Escenario'
        WHERE Id = @TipoClaseTaller;

    UPDATE TiposClase
        SET Nombre = 'Tango Salón Privada'
        WHERE Id = @TipoClaseEvento;

    IF NOT EXISTS (SELECT 1 FROM TiposClase WHERE Id = @TipoClasePrivado)
    BEGIN
        INSERT INTO TiposClase (Id, Nombre)
        VALUES (@TipoClasePrivado, 'Tango Escenario Privada');
    END

    -- (Opcional) reasignar las clases seed a los nuevos tipos si se desea mezclar nombres.
    -- UPDATE Clases SET IdTipoClase = @TipoClasePrivado WHERE IdClase = '...';
COMMIT;

PRINT 'Tipos de clase actualizados con nombres de Tango.';

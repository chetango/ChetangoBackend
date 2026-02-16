USE ChetangoDB_Dev;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT '========================================';
PRINT 'Creando Pagos y Paquetes';
PRINT '========================================';

-- Obtener IDs de catálogos (usar IDs fijos conocidos)
DECLARE @TipoPaquete4Clases UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111'; -- 4 clases
DECLARE @TipoPaquete8Clases UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222'; -- 8 clases  
DECLARE @TipoPaquete12Clases UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333'; -- 12 clases
DECLARE @MetodoEfectivo UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM MetodosPago WHERE Nombre LIKE '%Efectivo%');
DECLARE @MetodoTransferencia UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM MetodosPago WHERE Nombre LIKE '%Transferencia%');
DECLARE @MetodoTarjeta UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM MetodosPago WHERE Nombre LIKE '%Tarjeta%');
DECLARE @EstadoPagoCompletado UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM EstadosPago);

PRINT 'Catálogos obtenidos:';
PRINT '  - Paquete 4: ' + CAST(@TipoPaquete4Clases AS VARCHAR(50));
PRINT '  - Paquete 8: ' + CAST(@TipoPaquete8Clases AS VARCHAR(50));
PRINT '  - Paquete 12: ' + CAST(@TipoPaquete12Clases AS VARCHAR(50));
PRINT '';

BEGIN TRANSACTION;

-- Obtener todos los alumnos de marketing
DECLARE @AlumnosMarketing TABLE (IdAlumno UNIQUEIDENTIFIER, FechaInscripcion DATETIME2, RowNum INT);
INSERT INTO @AlumnosMarketing
SELECT IdAlumno, FechaInscripcion, ROW_NUMBER() OVER (ORDER BY FechaInscripcion)
FROM Alumnos WHERE IdUsuario IN (SELECT IdUsuario FROM Usuarios WHERE Correo LIKE '%@marketing.chetango.com');

DECLARE @IdAlumno UNIQUEIDENTIFIER;
DECLARE @FechaInscripcion DATETIME2;
DECLARE @RowNum INT;
DECLARE @IdPago UNIQUEIDENTIFIER;
DECLARE @IdPaquete UNIQUEIDENTIFIER;
DECLARE @TipoPaquete UNIQUEIDENTIFIER;
DECLARE @Monto DECIMAL(18,2);
DECLARE @MetodoPago UNIQUEIDENTIFIER;
DECLARE @NumClases INT;
DECLARE @PaquetesCreados INT = 0;
DECLARE @PagosCreados INT = 0;

-- Crear paquete inicial para cada alumno
DECLARE cursorAlumnos CURSOR FOR
SELECT IdAlumno, FechaInscripcion, RowNum FROM @AlumnosMarketing;

OPEN cursorAlumnos;
FETCH NEXT FROM cursorAlumnos INTO @IdAlumno, @FechaInscripcion, @RowNum;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- 33% Paquete 4, 33% Paquete 8, 33% Paquete 12
    IF (@RowNum % 3) = 0
    BEGIN
        SET @TipoPaquete = @TipoPaquete4Clases;
        SET @NumClases = 4;
        SET @Monto = 110000;
    END
    ELSE IF (@RowNum % 3) = 1
    BEGIN
        SET @TipoPaquete = @TipoPaquete8Clases;
        SET @NumClases = 8;
        SET @Monto = 140000;
    END
    ELSE
    BEGIN
        SET @TipoPaquete = @TipoPaquete12Clases;
        SET @NumClases = 12;
        SET @Monto = 180000;
    END;
    
    -- Rotar métodos de pago
    SET @MetodoPago = CASE (@RowNum % 3)
        WHEN 0 THEN @MetodoEfectivo
        WHEN 1 THEN @MetodoTransferencia
        ELSE @MetodoTarjeta
    END;
    
    -- Crear Pago
    SET @IdPago = NEWID();
    INSERT INTO Pagos (IdPago, IdAlumno, FechaPago, MontoTotal, IdMetodoPago, Nota, FechaCreacion, FechaModificacion, UsuarioCreacion, UsuarioModificacion, IdEstadoPago, UrlComprobante, ReferenciaTransferencia, NotasVerificacion, FechaVerificacion, UsuarioVerificacion)
    VALUES (
        @IdPago,
        @IdAlumno,
        @FechaInscripcion,
        @Monto,
        @MetodoPago,
        'Pago inicial - Marketing',
        @FechaInscripcion,
        NULL,
        'SYSTEM',
        NULL,
        @EstadoPagoCompletado,
        NULL,
        NULL,
        NULL,
        NULL,
        NULL
    );
    SET @PagosCreados = @PagosCreados + 1;
    
    -- Crear Paquete
    SET @IdPaquete = NEWID();
    INSERT INTO Paquetes (IdPaquete, IdAlumno, IdPago, ClasesDisponibles, ClasesUsadas, FechaActivacion, FechaVencimiento, IdEstado, IdTipoPaquete, ValorPaquete, FechaCreacion, FechaModificacion, UsuarioCreacion, UsuarioModificacion)
    VALUES (
        @IdPaquete,
        @IdAlumno,
        @IdPago,
        @NumClases,
        CASE WHEN DATEDIFF(DAY, @FechaInscripcion, GETDATE()) > 60 THEN (@NumClases * 50) / 100 ELSE 0 END, -- 50% usado si tiene más de 2 meses
        @FechaInscripcion,
        DATEADD(DAY, 90, @FechaInscripcion), -- 90 días de vigencia
        1, -- Activo
        @TipoPaquete,
        @Monto,
        @FechaInscripcion,
        NULL,
        'SYSTEM',
        NULL
    );
    SET @PaquetesCreados = @PaquetesCreados + 1;
    
    IF @RowNum % 10 = 0
        PRINT '   ✓ ' + CAST(@PaquetesCreados AS VARCHAR) + ' paquetes creados...';
    
    FETCH NEXT FROM cursorAlumnos INTO @IdAlumno, @FechaInscripcion, @RowNum;
END;

CLOSE cursorAlumnos;
DEALLOCATE cursorAlumnos;

COMMIT TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT 'Creados:';
PRINT '  - ' + CAST(@PagosCreados AS VARCHAR) + ' Pagos';
PRINT '  - ' + CAST(@PaquetesCreados AS VARCHAR) + ' Paquetes';
PRINT '========================================';
GO

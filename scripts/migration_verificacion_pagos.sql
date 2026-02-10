-- ============================================
-- MIGRACIÓN: SISTEMA DE VERIFICACIÓN DE PAGOS
-- Fecha: 2026-01-28
-- ============================================

-- 1. CREAR TABLA EstadosPago
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EstadosPago')
BEGIN
    CREATE TABLE EstadosPago (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Nombre NVARCHAR(100) NOT NULL,
        Descripcion NVARCHAR(500) NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
        FechaModificacion DATETIME2 NULL,
        UsuarioCreacion NVARCHAR(100) NOT NULL,
        UsuarioModificacion NVARCHAR(100) NULL,
        CONSTRAINT UK_EstadoPago_Nombre UNIQUE (Nombre)
    );
    PRINT '✅ Tabla EstadosPago creada';
END
ELSE
BEGIN
    PRINT '⚠️ Tabla EstadosPago ya existe';
END
GO

-- 2. INSERTAR ESTADOS DE PAGO
DECLARE @EstadoPendiente UNIQUEIDENTIFIER = NEWID();
DECLARE @EstadoVerificado UNIQUEIDENTIFIER = NEWID();
DECLARE @EstadoRechazado UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT * FROM EstadosPago WHERE Nombre = 'Pendiente Verificación')
BEGIN
    SET @EstadoPendiente = NEWID();
    INSERT INTO EstadosPago (Id, Nombre, Descripcion, Activo, FechaCreacion, UsuarioCreacion)
    VALUES (
        @EstadoPendiente,
        'Pendiente Verificación',
        'Pago registrado, esperando verificación del comprobante',
        1,
        GETDATE(),
        'Sistema'
    );
    PRINT '✅ Estado "Pendiente Verificación" insertado';
END
ELSE
BEGIN
    SELECT @EstadoPendiente = Id FROM EstadosPago WHERE Nombre = 'Pendiente Verificación';
    PRINT '⚠️ Estado "Pendiente Verificación" ya existe';
END

IF NOT EXISTS (SELECT * FROM EstadosPago WHERE Nombre = 'Verificado')
BEGIN
    SET @EstadoVerificado = NEWID();
    INSERT INTO EstadosPago (Id, Nombre, Descripcion, Activo, FechaCreacion, UsuarioCreacion)
    VALUES (
        @EstadoVerificado,
        'Verificado',
        'Pago verificado y aprobado correctamente',
        1,
        GETDATE(),
        'Sistema'
    );
    PRINT '✅ Estado "Verificado" insertado';
END
ELSE
BEGIN
    SELECT @EstadoVerificado = Id FROM EstadosPago WHERE Nombre = 'Verificado';
    PRINT '⚠️ Estado "Verificado" ya existe';
END

IF NOT EXISTS (SELECT * FROM EstadosPago WHERE Nombre = 'Rechazado')
BEGIN
    SET @EstadoRechazado = NEWID();
    INSERT INTO EstadosPago (Id, Nombre, Descripcion, Activo, FechaCreacion, UsuarioCreacion)
    VALUES (
        @EstadoRechazado,
        'Rechazado',
        'Pago rechazado por discrepancias en el comprobante',
        1,
        GETDATE(),
        'Sistema'
    );
    PRINT '✅ Estado "Rechazado" insertado';
END
ELSE
BEGIN
    SELECT @EstadoRechazado = Id FROM EstadosPago WHERE Nombre = 'Rechazado';
    PRINT '⚠️ Estado "Rechazado" ya existe';
END
GO

-- 3. AGREGAR COLUMNAS A TABLA Pagos
PRINT '📝 Agregando columnas a tabla Pagos...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Pagos') AND name = 'IdEstadoPago')
BEGIN
    ALTER TABLE Pagos ADD IdEstadoPago UNIQUEIDENTIFIER NULL;
    PRINT '✅ Columna IdEstadoPago agregada';
END
ELSE
BEGIN
    PRINT '⚠️ Columna IdEstadoPago ya existe';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Pagos') AND name = 'UrlComprobante')
BEGIN
    ALTER TABLE Pagos ADD UrlComprobante NVARCHAR(500) NULL;
    PRINT '✅ Columna UrlComprobante agregada';
END
ELSE
BEGIN
    PRINT '⚠️ Columna UrlComprobante ya existe';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Pagos') AND name = 'ReferenciaTransferencia')
BEGIN
    ALTER TABLE Pagos ADD ReferenciaTransferencia NVARCHAR(100) NULL;
    PRINT '✅ Columna ReferenciaTransferencia agregada';
END
ELSE
BEGIN
    PRINT '⚠️ Columna ReferenciaTransferencia ya existe';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Pagos') AND name = 'NotasVerificacion')
BEGIN
    ALTER TABLE Pagos ADD NotasVerificacion NVARCHAR(1000) NULL;
    PRINT '✅ Columna NotasVerificacion agregada';
END
ELSE
BEGIN
    PRINT '⚠️ Columna NotasVerificacion ya existe';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Pagos') AND name = 'FechaVerificacion')
BEGIN
    ALTER TABLE Pagos ADD FechaVerificacion DATETIME2 NULL;
    PRINT '✅ Columna FechaVerificacion agregada';
END
ELSE
BEGIN
    PRINT '⚠️ Columna FechaVerificacion ya existe';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Pagos') AND name = 'UsuarioVerificacion')
BEGIN
    ALTER TABLE Pagos ADD UsuarioVerificacion NVARCHAR(100) NULL;
    PRINT '✅ Columna UsuarioVerificacion agregada';
END
ELSE
BEGIN
    PRINT '⚠️ Columna UsuarioVerificacion ya existe';
END
GO

-- 4. ESTABLECER ESTADO POR DEFECTO PARA PAGOS EXISTENTES
PRINT '📝 Actualizando pagos existentes con estado "Verificado"...';

DECLARE @EstadoVerificadoId UNIQUEIDENTIFIER;
SELECT @EstadoVerificadoId = Id FROM EstadosPago WHERE Nombre = 'Verificado';

UPDATE Pagos 
SET IdEstadoPago = @EstadoVerificadoId
WHERE IdEstadoPago IS NULL;

DECLARE @PagosActualizados INT = @@ROWCOUNT;
PRINT CONCAT('✅ ', @PagosActualizados, ' pago(s) actualizado(s) con estado "Verificado"');
GO

-- 5. HACER COLUMNA IdEstadoPago OBLIGATORIA Y AGREGAR FK
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Pagos') AND name = 'IdEstadoPago' AND is_nullable = 1)
BEGIN
    ALTER TABLE Pagos ALTER COLUMN IdEstadoPago UNIQUEIDENTIFIER NOT NULL;
    PRINT '✅ Columna IdEstadoPago configurada como NOT NULL';
END
ELSE
BEGIN
    PRINT '⚠️ Columna IdEstadoPago ya es NOT NULL';
END
GO

-- 6. AGREGAR FOREIGN KEY
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Pagos_EstadoPago')
BEGIN
    ALTER TABLE Pagos 
    ADD CONSTRAINT FK_Pagos_EstadoPago 
    FOREIGN KEY (IdEstadoPago) REFERENCES EstadosPago(Id);
    PRINT '✅ Foreign Key FK_Pagos_EstadoPago creada';
END
ELSE
BEGIN
    PRINT '⚠️ Foreign Key FK_Pagos_EstadoPago ya existe';
END
GO

-- 7. CREAR ÍNDICES PARA MEJOR PERFORMANCE
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pagos_EstadoPago' AND object_id = OBJECT_ID('Pagos'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Pagos_EstadoPago 
    ON Pagos(IdEstadoPago) 
    INCLUDE (FechaPago, MontoTotal);
    PRINT '✅ Índice IX_Pagos_EstadoPago creado';
END
ELSE
BEGIN
    PRINT '⚠️ Índice IX_Pagos_EstadoPago ya existe';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pagos_FechaVerificacion' AND object_id = OBJECT_ID('Pagos'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Pagos_FechaVerificacion 
    ON Pagos(FechaVerificacion) 
    WHERE FechaVerificacion IS NOT NULL;
    PRINT '✅ Índice IX_Pagos_FechaVerificacion creado';
END
ELSE
BEGIN
    PRINT '⚠️ Índice IX_Pagos_FechaVerificacion ya existe';
END
GO

-- ============================================
-- VERIFICACIÓN FINAL
-- ============================================
PRINT '';
PRINT '╔═══════════════════════════════════════════════════════════╗';
PRINT '║         MIGRACIÓN COMPLETADA EXITOSAMENTE ✅              ║';
PRINT '╚═══════════════════════════════════════════════════════════╝';
PRINT '';

PRINT '📊 RESUMEN:';
SELECT 
    'Estados de Pago' AS Tabla,
    COUNT(*) AS Registros
FROM EstadosPago
UNION ALL
SELECT 
    'Pagos Total' AS Tabla,
    COUNT(*) AS Registros
FROM Pagos
UNION ALL
SELECT 
    'Pagos Verificados' AS Tabla,
    COUNT(*) AS Registros
FROM Pagos p
INNER JOIN EstadosPago e ON p.IdEstadoPago = e.Id
WHERE e.Nombre = 'Verificado'
UNION ALL
SELECT 
    'Pagos Pendientes' AS Tabla,
    COUNT(*) AS Registros
FROM Pagos p
INNER JOIN EstadosPago e ON p.IdEstadoPago = e.Id
WHERE e.Nombre = 'Pendiente Verificación';

PRINT '';
PRINT '✅ Estados disponibles:';
SELECT Nombre, Descripcion FROM EstadosPago WHERE Activo = 1;

PRINT '';
PRINT '🎯 Sistema de verificación de pagos listo para usar!';
PRINT '';

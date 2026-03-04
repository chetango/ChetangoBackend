-- ============================================================
-- Migración manual: 20260225063042_AgregarOtrosIngresosGastos
-- Base de datos destino: chetango-db-prod
-- Fecha: 2026-02-25
-- ============================================================

-- 1. Tabla CategoriasGasto
CREATE TABLE [dbo].[CategoriasGasto] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Nombre]      NVARCHAR(100)    NOT NULL,
    [Descripcion] NVARCHAR(500)    NULL,
    CONSTRAINT [PK_CategoriasGasto] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_CategoriasGasto_Nombre] ON [dbo].[CategoriasGasto] ([Nombre]);

-- 2. Tabla CategoriasIngreso
CREATE TABLE [dbo].[CategoriasIngreso] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Nombre]      NVARCHAR(100)    NOT NULL,
    [Descripcion] NVARCHAR(500)    NULL,
    CONSTRAINT [PK_CategoriasIngreso] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_CategoriasIngreso_Nombre] ON [dbo].[CategoriasIngreso] ([Nombre]);

-- 3. Tabla OtrosGastos
CREATE TABLE [dbo].[OtrosGastos] (
    [IdOtroGasto]          UNIQUEIDENTIFIER NOT NULL,
    [Concepto]             NVARCHAR(200)    NOT NULL,
    [Monto]                DECIMAL(18,2)    NOT NULL,
    [Fecha]                DATETIME2        NOT NULL,
    [Sede]                 INT              NOT NULL,
    [IdCategoriaGasto]     UNIQUEIDENTIFIER NULL,
    [Proveedor]            NVARCHAR(200)    NULL,
    [Descripcion]          NVARCHAR(1000)   NULL,
    [UrlFactura]           NVARCHAR(500)    NULL,
    [NumeroFactura]        NVARCHAR(100)    NULL,
    [FechaCreacion]        DATETIME2        NOT NULL CONSTRAINT [DF_OtrosGastos_FechaCreacion] DEFAULT (GETDATE()),
    [FechaModificacion]    DATETIME2        NULL,
    [UsuarioCreacion]      NVARCHAR(256)    NOT NULL CONSTRAINT [DF_OtrosGastos_UsuarioCreacion] DEFAULT (SUSER_SNAME()),
    [UsuarioModificacion]  NVARCHAR(256)    NULL,
    [Eliminado]            BIT              NOT NULL CONSTRAINT [DF_OtrosGastos_Eliminado] DEFAULT (0),
    [FechaEliminacion]     DATETIME2        NULL,
    [UsuarioEliminacion]   NVARCHAR(256)    NULL,
    CONSTRAINT [PK_OtrosGastos] PRIMARY KEY ([IdOtroGasto]),
    CONSTRAINT [FK_OtrosGastos_CategoriasGasto_IdCategoriaGasto]
        FOREIGN KEY ([IdCategoriaGasto]) REFERENCES [dbo].[CategoriasGasto] ([Id])
        ON DELETE NO ACTION
);

CREATE INDEX [IX_OtrosGastos_Fecha]      ON [dbo].[OtrosGastos] ([Fecha]);
CREATE INDEX [IX_OtrosGastos_Sede]       ON [dbo].[OtrosGastos] ([Sede]);
CREATE INDEX [IX_OtrosGastos_Fecha_Sede] ON [dbo].[OtrosGastos] ([Fecha], [Sede]);
CREATE INDEX [IX_OtrosGastos_IdCategoriaGasto] ON [dbo].[OtrosGastos] ([IdCategoriaGasto]);

-- 4. Tabla OtrosIngresos
CREATE TABLE [dbo].[OtrosIngresos] (
    [IdOtroIngreso]        UNIQUEIDENTIFIER NOT NULL,
    [Concepto]             NVARCHAR(200)    NOT NULL,
    [Monto]                DECIMAL(18,2)    NOT NULL,
    [Fecha]                DATETIME2        NOT NULL,
    [Sede]                 INT              NOT NULL,
    [IdCategoriaIngreso]   UNIQUEIDENTIFIER NULL,
    [Descripcion]          NVARCHAR(1000)   NULL,
    [UrlComprobante]       NVARCHAR(500)    NULL,
    [FechaCreacion]        DATETIME2        NOT NULL CONSTRAINT [DF_OtrosIngresos_FechaCreacion] DEFAULT (GETDATE()),
    [FechaModificacion]    DATETIME2        NULL,
    [UsuarioCreacion]      NVARCHAR(256)    NOT NULL CONSTRAINT [DF_OtrosIngresos_UsuarioCreacion] DEFAULT (SUSER_SNAME()),
    [UsuarioModificacion]  NVARCHAR(256)    NULL,
    [Eliminado]            BIT              NOT NULL CONSTRAINT [DF_OtrosIngresos_Eliminado] DEFAULT (0),
    [FechaEliminacion]     DATETIME2        NULL,
    [UsuarioEliminacion]   NVARCHAR(256)    NULL,
    CONSTRAINT [PK_OtrosIngresos] PRIMARY KEY ([IdOtroIngreso]),
    CONSTRAINT [FK_OtrosIngresos_CategoriasIngreso_IdCategoriaIngreso]
        FOREIGN KEY ([IdCategoriaIngreso]) REFERENCES [dbo].[CategoriasIngreso] ([Id])
        ON DELETE NO ACTION
);

CREATE INDEX [IX_OtrosIngresos_Fecha]      ON [dbo].[OtrosIngresos] ([Fecha]);
CREATE INDEX [IX_OtrosIngresos_Sede]       ON [dbo].[OtrosIngresos] ([Sede]);
CREATE INDEX [IX_OtrosIngresos_Fecha_Sede] ON [dbo].[OtrosIngresos] ([Fecha], [Sede]);
CREATE INDEX [IX_OtrosIngresos_IdCategoriaIngreso] ON [dbo].[OtrosIngresos] ([IdCategoriaIngreso]);

-- 5. Seed: Categorías de Gasto
INSERT INTO [dbo].[CategoriasGasto] ([Id], [Nombre], [Descripcion]) VALUES
('F1111111-0000-0000-0000-000000000001', 'Arriendo',           'Pago mensual del local o salón'),
('F1111111-0000-0000-0000-000000000002', 'Servicios Públicos', 'Luz, agua, internet, teléfono'),
('F1111111-0000-0000-0000-000000000003', 'Mantenimiento',      'Reparaciones, mantenimiento de equipos e instalaciones'),
('F1111111-0000-0000-0000-000000000004', 'Marketing',          'Publicidad, redes sociales, diseño gráfico'),
('F1111111-0000-0000-0000-000000000005', 'Suministros',        'Material de oficina, limpieza, consumibles'),
('F1111111-0000-0000-0000-000000000006', 'Equipamiento',       'Espejos, barras, sonido, iluminación'),
('F1111111-0000-0000-0000-000000000007', 'Impuestos y Seguros','Impuestos, seguros, trámites legales'),
('F1111111-0000-0000-0000-000000000008', 'Transporte',         'Transporte de profesores o materiales'),
('F1111111-0000-0000-0000-000000000009', 'Otros',              'Otros gastos no clasificados');

-- 6. Seed: Categorías de Ingreso
INSERT INTO [dbo].[CategoriasIngreso] ([Id], [Nombre], [Descripcion]) VALUES
('E1111111-0000-0000-0000-000000000001', 'Eventos',           'Ingresos por eventos especiales, shows, presentaciones'),
('E1111111-0000-0000-0000-000000000002', 'Alquiler de Espacio','Ingresos por alquiler del salón para eventos externos'),
('E1111111-0000-0000-0000-000000000003', 'Mercancía',         'Venta de camisetas, zapatos, accesorios de danza'),
('E1111111-0000-0000-0000-000000000004', 'Shows Privados',    'Presentaciones privadas contratadas'),
('E1111111-0000-0000-0000-000000000005', 'Talleres Externos', 'Talleres impartidos fuera de la academia'),
('E1111111-0000-0000-0000-000000000006', 'Patrocinios',       'Ingresos por patrocinios de marcas o empresas'),
('E1111111-0000-0000-0000-000000000007', 'Otros',             'Otros ingresos no clasificados');

-- 7. Registrar la migración en EF Core para que no intente aplicarla de nuevo
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20260225063042_AgregarOtrosIngresosGastos', '9.0.0');

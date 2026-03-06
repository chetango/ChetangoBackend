BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    ALTER TABLE [Tenants] ADD [FechaActivacion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    ALTER TABLE [Tenants] ADD [OnboardingCompletado] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    ALTER TABLE [Tenants] ADD [RequiereReaceptacion] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    CREATE TABLE [DocumentosLegales] (
        [Id] uniqueidentifier NOT NULL,
        [Codigo] nvarchar(50) NOT NULL,
        [Nombre] nvarchar(200) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [Destinatario] nvarchar(20) NOT NULL DEFAULT N'Admin',
        [EsObligatorio] bit NOT NULL DEFAULT CAST(1 AS bit),
        [RequiereReaceptacion] bit NOT NULL DEFAULT CAST(1 AS bit),
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [CreadoPor] nvarchar(256) NOT NULL,
        CONSTRAINT [PK_DocumentosLegales] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    CREATE TABLE [VersionesDocumentoLegal] (
        [Id] uniqueidentifier NOT NULL,
        [DocumentoLegalId] uniqueidentifier NOT NULL,
        [NumeroVersion] nvarchar(10) NOT NULL,
        [UrlDocumento] nvarchar(1000) NOT NULL,
        [ResumenCambios] nvarchar(1000) NULL,
        [FechaVigencia] datetime2 NOT NULL,
        [EsCambioSignificativo] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Activa] bit NOT NULL DEFAULT CAST(1 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [CreadoPor] nvarchar(256) NOT NULL,
        CONSTRAINT [PK_VersionesDocumentoLegal] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VersionesDocumentoLegal_DocumentosLegales_DocumentoLegalId] FOREIGN KEY ([DocumentoLegalId]) REFERENCES [DocumentosLegales] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    CREATE TABLE [AceptacionesDocumento] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [IdUsuario] uniqueidentifier NOT NULL,
        [VersionDocumentoLegalId] uniqueidentifier NOT NULL,
        [FechaAceptacion] datetime2 NOT NULL,
        [IpOrigen] nvarchar(45) NOT NULL,
        [UserAgent] nvarchar(500) NULL,
        [Contexto] nvarchar(20) NOT NULL DEFAULT N'Onboarding',
        CONSTRAINT [PK_AceptacionesDocumento] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AceptacionesDocumento_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AceptacionesDocumento_Usuarios_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AceptacionesDocumento_VersionesDocumentoLegal_VersionDocumentoLegalId] FOREIGN KEY ([VersionDocumentoLegalId]) REFERENCES [VersionesDocumentoLegal] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'Codigo', N'CreadoPor', N'Descripcion', N'Destinatario', N'EsObligatorio', N'FechaCreacion', N'Nombre', N'RequiereReaceptacion') AND [object_id] = OBJECT_ID(N'[DocumentosLegales]'))
        SET IDENTITY_INSERT [DocumentosLegales] ON;
    EXEC(N'INSERT INTO [DocumentosLegales] ([Id], [Activo], [Codigo], [CreadoPor], [Descripcion], [Destinatario], [EsObligatorio], [FechaCreacion], [Nombre], [RequiereReaceptacion])
    VALUES (''aa000001-0000-0000-0000-000000000001'', CAST(1 AS bit), N''TERMINOS'', N''SISTEMA'', N''Contrato SaaS entre Aphellion y la academia cliente.'', N''Admin'', CAST(1 AS bit), ''2026-03-06T00:00:00.0000000Z'', N''Términos y Condiciones del Servicio'', CAST(1 AS bit)),
    (''aa000002-0000-0000-0000-000000000002'', CAST(1 AS bit), N''DPA'', N''SISTEMA'', N''Define las responsabilidades de Aphellion como encargado y de la academia como responsable del tratamiento.'', N''Admin'', CAST(1 AS bit), ''2026-03-06T00:00:00.0000000Z'', N''Acuerdo de Tratamiento de Datos (DPA)'', CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'Codigo', N'CreadoPor', N'Descripcion', N'Destinatario', N'EsObligatorio', N'FechaCreacion', N'Nombre', N'RequiereReaceptacion') AND [object_id] = OBJECT_ID(N'[DocumentosLegales]'))
        SET IDENTITY_INSERT [DocumentosLegales] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'Codigo', N'CreadoPor', N'Descripcion', N'Destinatario', N'EsObligatorio', N'FechaCreacion', N'Nombre') AND [object_id] = OBJECT_ID(N'[DocumentosLegales]'))
        SET IDENTITY_INSERT [DocumentosLegales] ON;
    EXEC(N'INSERT INTO [DocumentosLegales] ([Id], [Activo], [Codigo], [CreadoPor], [Descripcion], [Destinatario], [EsObligatorio], [FechaCreacion], [Nombre])
    VALUES (''aa000003-0000-0000-0000-000000000003'', CAST(1 AS bit), N''POLITICA_PRIVACIDAD'', N''SISTEMA'', N''Cómo Aphellion maneja los datos comerciales del cliente.'', N''Admin'', CAST(1 AS bit), ''2026-03-06T00:00:00.0000000Z'', N''Política de Privacidad''),
    (''aa000004-0000-0000-0000-000000000004'', CAST(1 AS bit), N''AVISO_PRIVACIDAD'', N''SISTEMA'', N''Aviso corto para usuarios finales (profesores y alumnos) sobre el tratamiento de sus datos.'', N''Todos'', CAST(1 AS bit), ''2026-03-06T00:00:00.0000000Z'', N''Aviso de Privacidad'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activo', N'Codigo', N'CreadoPor', N'Descripcion', N'Destinatario', N'EsObligatorio', N'FechaCreacion', N'Nombre') AND [object_id] = OBJECT_ID(N'[DocumentosLegales]'))
        SET IDENTITY_INSERT [DocumentosLegales] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activa', N'CreadoPor', N'DocumentoLegalId', N'FechaCreacion', N'FechaVigencia', N'NumeroVersion', N'ResumenCambios', N'UrlDocumento') AND [object_id] = OBJECT_ID(N'[VersionesDocumentoLegal]'))
        SET IDENTITY_INSERT [VersionesDocumentoLegal] ON;
    EXEC(N'INSERT INTO [VersionesDocumentoLegal] ([Id], [Activa], [CreadoPor], [DocumentoLegalId], [FechaCreacion], [FechaVigencia], [NumeroVersion], [ResumenCambios], [UrlDocumento])
    VALUES (''bb000001-0000-0000-0000-000000000001'', CAST(1 AS bit), N''SISTEMA'', ''aa000001-0000-0000-0000-000000000001'', ''2026-03-06T00:00:00.0000000Z'', ''2026-03-06T00:00:00.0000000Z'', N''1.0'', N''Versión inicial.'', N''/docs/terminos-v1.0.pdf''),
    (''bb000002-0000-0000-0000-000000000002'', CAST(1 AS bit), N''SISTEMA'', ''aa000002-0000-0000-0000-000000000002'', ''2026-03-06T00:00:00.0000000Z'', ''2026-03-06T00:00:00.0000000Z'', N''1.0'', N''Versión inicial.'', N''/docs/dpa-v1.0.pdf''),
    (''bb000003-0000-0000-0000-000000000003'', CAST(1 AS bit), N''SISTEMA'', ''aa000003-0000-0000-0000-000000000003'', ''2026-03-06T00:00:00.0000000Z'', ''2026-03-06T00:00:00.0000000Z'', N''1.0'', N''Versión inicial.'', N''/docs/politica-privacidad-v1.0.pdf''),
    (''bb000004-0000-0000-0000-000000000004'', CAST(1 AS bit), N''SISTEMA'', ''aa000004-0000-0000-0000-000000000004'', ''2026-03-06T00:00:00.0000000Z'', ''2026-03-06T00:00:00.0000000Z'', N''1.0'', N''Versión inicial.'', N''/docs/aviso-privacidad-v1.0.pdf'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Activa', N'CreadoPor', N'DocumentoLegalId', N'FechaCreacion', N'FechaVigencia', N'NumeroVersion', N'ResumenCambios', N'UrlDocumento') AND [object_id] = OBJECT_ID(N'[VersionesDocumentoLegal]'))
        SET IDENTITY_INSERT [VersionesDocumentoLegal] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    CREATE INDEX [IX_AceptacionesDocumento_IdUsuario] ON [AceptacionesDocumento] ([IdUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    CREATE INDEX [IX_AceptacionesDocumento_TenantId] ON [AceptacionesDocumento] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AceptacionesDocumento_TenantId_VersionDocumentoLegalId] ON [AceptacionesDocumento] ([TenantId], [VersionDocumentoLegalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    CREATE INDEX [IX_AceptacionesDocumento_VersionDocumentoLegalId] ON [AceptacionesDocumento] ([VersionDocumentoLegalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DocumentosLegales_Codigo] ON [DocumentosLegales] ([Codigo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    CREATE UNIQUE INDEX [IX_VersionesDocumentoLegal_DocumentoLegalId_NumeroVersion] ON [VersionesDocumentoLegal] ([DocumentoLegalId], [NumeroVersion]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306204346_AgregarModuloCompliance'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260306204346_AgregarModuloCompliance', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306211845_ActualizarUrlsDocumentosHtml'
)
BEGIN
    EXEC(N'UPDATE [VersionesDocumentoLegal] SET [UrlDocumento] = N''/docs/terminos-v1.0.html''
    WHERE [Id] = ''bb000001-0000-0000-0000-000000000001'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306211845_ActualizarUrlsDocumentosHtml'
)
BEGIN
    EXEC(N'UPDATE [VersionesDocumentoLegal] SET [UrlDocumento] = N''/docs/dpa-v1.0.html''
    WHERE [Id] = ''bb000002-0000-0000-0000-000000000002'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306211845_ActualizarUrlsDocumentosHtml'
)
BEGIN
    EXEC(N'UPDATE [VersionesDocumentoLegal] SET [UrlDocumento] = N''/docs/politica-privacidad-v1.0.html''
    WHERE [Id] = ''bb000003-0000-0000-0000-000000000003'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306211845_ActualizarUrlsDocumentosHtml'
)
BEGIN
    EXEC(N'UPDATE [VersionesDocumentoLegal] SET [UrlDocumento] = N''/docs/aviso-privacidad-v1.0.html''
    WHERE [Id] = ''bb000004-0000-0000-0000-000000000004'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306211845_ActualizarUrlsDocumentosHtml'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260306211845_ActualizarUrlsDocumentosHtml', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306212347_ActualizarUrlsDocumentosHtml2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260306212347_ActualizarUrlsDocumentosHtml2', N'9.0.9');
END;

COMMIT;
GO


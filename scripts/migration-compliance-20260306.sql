IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [ConfiguracionesNotificaciones] (
        [IdConfig] uniqueidentifier NOT NULL,
        [AnticipacionAlerta] int NOT NULL,
        [TextoVencimiento] nvarchar(300) NOT NULL,
        [TextoAgotamiento] nvarchar(300) NOT NULL,
        CONSTRAINT [PK_ConfiguracionesNotificaciones] PRIMARY KEY ([IdConfig])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [EstadosAsistencia] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_EstadosAsistencia] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [EstadosNotificacion] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_EstadosNotificacion] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [EstadosPaquete] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_EstadosPaquete] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [EstadosUsuario] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_EstadosUsuario] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [MetodosPago] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_MetodosPago] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [RolesEnClase] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_RolesEnClase] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [TiposClase] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_TiposClase] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [TiposDocumento] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_TiposDocumento] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [TiposPaquete] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_TiposPaquete] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [TiposProfesor] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_TiposProfesor] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [IdUsuario] uniqueidentifier NOT NULL,
        [NombreUsuario] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(255) NOT NULL,
        [IdTipoDocumento] uniqueidentifier NOT NULL,
        [NumeroDocumento] nvarchar(50) NOT NULL,
        [Correo] nvarchar(150) NOT NULL,
        [Telefono] nvarchar(30) NOT NULL,
        [IdEstadoUsuario] int NOT NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([IdUsuario]),
        CONSTRAINT [FK_Usuarios_EstadosUsuario_IdEstadoUsuario] FOREIGN KEY ([IdEstadoUsuario]) REFERENCES [EstadosUsuario] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Usuarios_TiposDocumento_IdTipoDocumento] FOREIGN KEY ([IdTipoDocumento]) REFERENCES [TiposDocumento] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Alumnos] (
        [IdAlumno] uniqueidentifier NOT NULL,
        [IdUsuario] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Alumnos] PRIMARY KEY ([IdAlumno]),
        CONSTRAINT [FK_Alumnos_Usuarios_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Auditorias] (
        [IdAuditoria] uniqueidentifier NOT NULL,
        [IdUsuario] uniqueidentifier NOT NULL,
        [Modulo] nvarchar(100) NOT NULL,
        [Accion] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(1000) NOT NULL,
        [FechaHora] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_Auditorias] PRIMARY KEY ([IdAuditoria]),
        CONSTRAINT [FK_Auditorias_Usuarios_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Notificaciones] (
        [IdNotificacion] uniqueidentifier NOT NULL,
        [IdUsuario] uniqueidentifier NOT NULL,
        [IdEstado] int NOT NULL,
        [Mensaje] nvarchar(500) NOT NULL,
        [FechaEnvio] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [Leida] bit NOT NULL,
        CONSTRAINT [PK_Notificaciones] PRIMARY KEY ([IdNotificacion]),
        CONSTRAINT [FK_Notificaciones_EstadosNotificacion_IdEstado] FOREIGN KEY ([IdEstado]) REFERENCES [EstadosNotificacion] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Notificaciones_Usuarios_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Profesores] (
        [IdProfesor] uniqueidentifier NOT NULL,
        [IdUsuario] uniqueidentifier NOT NULL,
        [IdTipoProfesor] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Profesores] PRIMARY KEY ([IdProfesor]),
        CONSTRAINT [FK_Profesores_TiposProfesor_IdTipoProfesor] FOREIGN KEY ([IdTipoProfesor]) REFERENCES [TiposProfesor] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Profesores_Usuarios_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [UsuariosRoles] (
        [IdUsuario] uniqueidentifier NOT NULL,
        [IdRol] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UsuariosRoles] PRIMARY KEY ([IdUsuario], [IdRol]),
        CONSTRAINT [FK_UsuariosRoles_Roles_IdRol] FOREIGN KEY ([IdRol]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UsuariosRoles_Usuarios_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Pagos] (
        [IdPago] uniqueidentifier NOT NULL,
        [IdAlumno] uniqueidentifier NOT NULL,
        [FechaPago] datetime2 NOT NULL,
        [MontoTotal] decimal(18,2) NOT NULL,
        [IdMetodoPago] uniqueidentifier NOT NULL,
        [Nota] nvarchar(max) NULL,
        CONSTRAINT [PK_Pagos] PRIMARY KEY ([IdPago]),
        CONSTRAINT [FK_Pagos_Alumnos_IdAlumno] FOREIGN KEY ([IdAlumno]) REFERENCES [Alumnos] ([IdAlumno]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Pagos_MetodosPago_IdMetodoPago] FOREIGN KEY ([IdMetodoPago]) REFERENCES [MetodosPago] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Clases] (
        [IdClase] uniqueidentifier NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [IdTipoClase] uniqueidentifier NOT NULL,
        [HoraInicio] time NOT NULL,
        [HoraFin] time NOT NULL,
        [IdProfesorPrincipal] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Clases] PRIMARY KEY ([IdClase]),
        CONSTRAINT [FK_Clases_Profesores_IdProfesorPrincipal] FOREIGN KEY ([IdProfesorPrincipal]) REFERENCES [Profesores] ([IdProfesor]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Clases_TiposClase_IdTipoClase] FOREIGN KEY ([IdTipoClase]) REFERENCES [TiposClase] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [TarifasProfesor] (
        [IdTarifa] uniqueidentifier NOT NULL,
        [IdTipoProfesor] uniqueidentifier NOT NULL,
        [IdRolEnClase] uniqueidentifier NOT NULL,
        [ValorPorClase] decimal(18,2) NOT NULL,
        [ProfesorIdProfesor] uniqueidentifier NULL,
        CONSTRAINT [PK_TarifasProfesor] PRIMARY KEY ([IdTarifa]),
        CONSTRAINT [FK_TarifasProfesor_Profesores_ProfesorIdProfesor] FOREIGN KEY ([ProfesorIdProfesor]) REFERENCES [Profesores] ([IdProfesor]),
        CONSTRAINT [FK_TarifasProfesor_RolesEnClase_IdRolEnClase] FOREIGN KEY ([IdRolEnClase]) REFERENCES [RolesEnClase] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TarifasProfesor_TiposProfesor_IdTipoProfesor] FOREIGN KEY ([IdTipoProfesor]) REFERENCES [TiposProfesor] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Paquetes] (
        [IdPaquete] uniqueidentifier NOT NULL,
        [IdAlumno] uniqueidentifier NOT NULL,
        [IdPago] uniqueidentifier NULL,
        [ClasesDisponibles] int NOT NULL,
        [ClasesUsadas] int NOT NULL,
        [FechaActivacion] datetime2 NOT NULL,
        [FechaVencimiento] datetime2 NOT NULL,
        [IdEstado] int NOT NULL,
        [IdTipoPaquete] uniqueidentifier NOT NULL,
        [ValorPaquete] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Paquetes] PRIMARY KEY ([IdPaquete]),
        CONSTRAINT [FK_Paquetes_Alumnos_IdAlumno] FOREIGN KEY ([IdAlumno]) REFERENCES [Alumnos] ([IdAlumno]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Paquetes_EstadosPaquete_IdEstado] FOREIGN KEY ([IdEstado]) REFERENCES [EstadosPaquete] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Paquetes_Pagos_IdPago] FOREIGN KEY ([IdPago]) REFERENCES [Pagos] ([IdPago]) ON DELETE SET NULL,
        CONSTRAINT [FK_Paquetes_TiposPaquete_IdTipoPaquete] FOREIGN KEY ([IdTipoPaquete]) REFERENCES [TiposPaquete] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [MonitoresClase] (
        [IdMonitorClase] uniqueidentifier NOT NULL,
        [IdClase] uniqueidentifier NOT NULL,
        [IdProfesor] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_MonitoresClase] PRIMARY KEY ([IdMonitorClase]),
        CONSTRAINT [FK_MonitoresClase_Clases_IdClase] FOREIGN KEY ([IdClase]) REFERENCES [Clases] ([IdClase]) ON DELETE CASCADE,
        CONSTRAINT [FK_MonitoresClase_Profesores_IdProfesor] FOREIGN KEY ([IdProfesor]) REFERENCES [Profesores] ([IdProfesor]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [Asistencias] (
        [IdAsistencia] uniqueidentifier NOT NULL,
        [IdClase] uniqueidentifier NOT NULL,
        [IdAlumno] uniqueidentifier NOT NULL,
        [IdPaqueteUsado] uniqueidentifier NOT NULL,
        [IdEstado] int NOT NULL,
        [Observacion] nvarchar(500) NULL,
        CONSTRAINT [PK_Asistencias] PRIMARY KEY ([IdAsistencia]),
        CONSTRAINT [FK_Asistencias_Alumnos_IdAlumno] FOREIGN KEY ([IdAlumno]) REFERENCES [Alumnos] ([IdAlumno]) ON DELETE CASCADE,
        CONSTRAINT [FK_Asistencias_Clases_IdClase] FOREIGN KEY ([IdClase]) REFERENCES [Clases] ([IdClase]) ON DELETE CASCADE,
        CONSTRAINT [FK_Asistencias_EstadosAsistencia_IdEstado] FOREIGN KEY ([IdEstado]) REFERENCES [EstadosAsistencia] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Asistencias_Paquetes_IdPaqueteUsado] FOREIGN KEY ([IdPaqueteUsado]) REFERENCES [Paquetes] ([IdPaquete]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE TABLE [CongelacionesPaquete] (
        [IdCongelacion] uniqueidentifier NOT NULL,
        [IdPaquete] uniqueidentifier NOT NULL,
        [FechaInicio] datetime2 NOT NULL,
        [FechaFin] datetime2 NOT NULL,
        CONSTRAINT [PK_CongelacionesPaquete] PRIMARY KEY ([IdCongelacion]),
        CONSTRAINT [FK_CongelacionesPaquete_Paquetes_IdPaquete] FOREIGN KEY ([IdPaquete]) REFERENCES [Paquetes] ([IdPaquete]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosAsistencia]'))
        SET IDENTITY_INSERT [EstadosAsistencia] ON;
    EXEC(N'INSERT INTO [EstadosAsistencia] ([Id], [Nombre])
    VALUES (1, N''Presente''),
    (2, N''Ausente''),
    (3, N''Justificada'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosAsistencia]'))
        SET IDENTITY_INSERT [EstadosAsistencia] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosNotificacion]'))
        SET IDENTITY_INSERT [EstadosNotificacion] ON;
    EXEC(N'INSERT INTO [EstadosNotificacion] ([Id], [Nombre])
    VALUES (1, N''Pendiente''),
    (2, N''Enviada''),
    (3, N''Leida'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosNotificacion]'))
        SET IDENTITY_INSERT [EstadosNotificacion] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosPaquete]'))
        SET IDENTITY_INSERT [EstadosPaquete] ON;
    EXEC(N'INSERT INTO [EstadosPaquete] ([Id], [Nombre])
    VALUES (1, N''Activo''),
    (2, N''Vencido''),
    (3, N''Congelado''),
    (4, N''Agotado'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosPaquete]'))
        SET IDENTITY_INSERT [EstadosPaquete] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosUsuario]'))
        SET IDENTITY_INSERT [EstadosUsuario] ON;
    EXEC(N'INSERT INTO [EstadosUsuario] ([Id], [Nombre])
    VALUES (1, N''Activo''),
    (2, N''Inactivo''),
    (3, N''Bloqueado'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosUsuario]'))
        SET IDENTITY_INSERT [EstadosUsuario] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[MetodosPago]'))
        SET IDENTITY_INSERT [MetodosPago] ON;
    EXEC(N'INSERT INTO [MetodosPago] ([Id], [Nombre])
    VALUES (''10101010-1010-1010-1010-101010101010'', N''Efectivo''),
    (''20202020-2020-2020-2020-202020202020'', N''Transferencia''),
    (''30303030-3030-3030-3030-303030303030'', N''Tarjeta''),
    (''40404040-4040-4040-4040-404040404040'', N''Bono''),
    (''50505050-5050-5050-5050-505050505050'', N''Cortesia'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[MetodosPago]'))
        SET IDENTITY_INSERT [MetodosPago] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([Id], [Nombre])
    VALUES (''cccccccc-cccc-cccc-cccc-cccccccccccc'', N''Administrador''),
    (''dddddddd-dddd-dddd-dddd-dddddddddddd'', N''Alumno''),
    (''eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee'', N''Profesor'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[RolesEnClase]'))
        SET IDENTITY_INSERT [RolesEnClase] ON;
    EXEC(N'INSERT INTO [RolesEnClase] ([Id], [Nombre])
    VALUES (''12121212-1212-1212-1212-121212121212'', N''Monitor''),
    (''ffffffff-ffff-ffff-ffff-ffffffffffff'', N''Principal'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[RolesEnClase]'))
        SET IDENTITY_INSERT [RolesEnClase] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[TiposClase]'))
        SET IDENTITY_INSERT [TiposClase] ON;
    EXEC(N'INSERT INTO [TiposClase] ([Id], [Nombre])
    VALUES (''44444444-4444-4444-4444-444444444444'', N''Regular''),
    (''55555555-5555-5555-5555-555555555555'', N''Taller''),
    (''66666666-6666-6666-6666-666666666666'', N''Evento'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[TiposClase]'))
        SET IDENTITY_INSERT [TiposClase] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[TiposDocumento]'))
        SET IDENTITY_INSERT [TiposDocumento] ON;
    EXEC(N'INSERT INTO [TiposDocumento] ([Id], [Nombre])
    VALUES (''11111111-1111-1111-1111-111111111111'', N''CC''),
    (''22222222-2222-2222-2222-222222222222'', N''CE''),
    (''33333333-3333-3333-3333-333333333333'', N''PAS'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[TiposDocumento]'))
        SET IDENTITY_INSERT [TiposDocumento] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[TiposPaquete]'))
        SET IDENTITY_INSERT [TiposPaquete] ON;
    EXEC(N'INSERT INTO [TiposPaquete] ([Id], [Nombre])
    VALUES (''77777777-7777-7777-7777-777777777777'', N''Mensual''),
    (''88888888-8888-8888-8888-888888888888'', N''BonoClases''),
    (''99999999-9999-9999-9999-999999999999'', N''Ilimitado'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[TiposPaquete]'))
        SET IDENTITY_INSERT [TiposPaquete] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[TiposProfesor]'))
        SET IDENTITY_INSERT [TiposProfesor] ON;
    EXEC(N'INSERT INTO [TiposProfesor] ([Id], [Nombre])
    VALUES (''aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'', N''Principal''),
    (''bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'', N''Monitor'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre') AND [object_id] = OBJECT_ID(N'[TiposProfesor]'))
        SET IDENTITY_INSERT [TiposProfesor] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Alumnos_IdUsuario] ON [Alumnos] ([IdUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Asistencias_IdAlumno] ON [Asistencias] ([IdAlumno]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Asistencias_IdClase_IdAlumno] ON [Asistencias] ([IdClase], [IdAlumno]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Asistencias_IdEstado] ON [Asistencias] ([IdEstado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Asistencias_IdPaqueteUsado] ON [Asistencias] ([IdPaqueteUsado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Auditorias_FechaHora] ON [Auditorias] ([FechaHora]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Auditorias_IdUsuario] ON [Auditorias] ([IdUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Clases_Fecha_IdTipoClase] ON [Clases] ([Fecha], [IdTipoClase]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Clases_IdProfesorPrincipal] ON [Clases] ([IdProfesorPrincipal]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Clases_IdTipoClase] ON [Clases] ([IdTipoClase]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CongelacionesPaquete_IdPaquete_FechaInicio_FechaFin] ON [CongelacionesPaquete] ([IdPaquete], [FechaInicio], [FechaFin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EstadosAsistencia_Nombre] ON [EstadosAsistencia] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EstadosNotificacion_Nombre] ON [EstadosNotificacion] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EstadosPaquete_Nombre] ON [EstadosPaquete] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EstadosUsuario_Nombre] ON [EstadosUsuario] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MetodosPago_Nombre] ON [MetodosPago] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_MonitoresClase_IdClase_IdProfesor] ON [MonitoresClase] ([IdClase], [IdProfesor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_MonitoresClase_IdProfesor] ON [MonitoresClase] ([IdProfesor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_IdEstado] ON [Notificaciones] ([IdEstado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_IdUsuario_Leida] ON [Notificaciones] ([IdUsuario], [Leida]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Pagos_IdAlumno] ON [Pagos] ([IdAlumno]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Pagos_IdMetodoPago] ON [Pagos] ([IdMetodoPago]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Paquetes_IdAlumno_FechaVencimiento] ON [Paquetes] ([IdAlumno], [FechaVencimiento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Paquetes_IdEstado] ON [Paquetes] ([IdEstado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Paquetes_IdPago] ON [Paquetes] ([IdPago]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Paquetes_IdTipoPaquete] ON [Paquetes] ([IdTipoPaquete]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Profesores_IdTipoProfesor] ON [Profesores] ([IdTipoProfesor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Profesores_IdUsuario] ON [Profesores] ([IdUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_Nombre] ON [Roles] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RolesEnClase_Nombre] ON [RolesEnClase] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TarifasProfesor_IdRolEnClase] ON [TarifasProfesor] ([IdRolEnClase]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TarifasProfesor_IdTipoProfesor_IdRolEnClase] ON [TarifasProfesor] ([IdTipoProfesor], [IdRolEnClase]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TarifasProfesor_ProfesorIdProfesor] ON [TarifasProfesor] ([ProfesorIdProfesor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TiposClase_Nombre] ON [TiposClase] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TiposDocumento_Nombre] ON [TiposDocumento] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TiposPaquete_Nombre] ON [TiposPaquete] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TiposProfesor_Nombre] ON [TiposProfesor] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Correo] ON [Usuarios] ([Correo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Usuarios_IdEstadoUsuario] ON [Usuarios] ([IdEstadoUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Usuarios_IdTipoDocumento] ON [Usuarios] ([IdTipoDocumento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_NombreUsuario] ON [Usuarios] ([NombreUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_NumeroDocumento] ON [Usuarios] ([NumeroDocumento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UsuariosRoles_IdRol] ON [UsuariosRoles] ([IdRol]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250914232847_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250914232847_InitialCreate', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250915003312_RemovePasswordHashFromUsuario'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'PasswordHash');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [Usuarios] DROP COLUMN [PasswordHash];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250915003312_RemovePasswordHashFromUsuario'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250915003312_RemovePasswordHashFromUsuario', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250917203135_SeedOIDTipoDocumento'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [TiposDocumento] WHERE [Nombre] = N'OID')
    BEGIN
        INSERT INTO [TiposDocumento] ([Id], [Nombre]) VALUES ('44444444-1111-2222-3333-555555555555', N'OID');
    END

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250917203135_SeedOIDTipoDocumento'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250917203135_SeedOIDTipoDocumento', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250917211543_AlignSnapshot_SeedOID'
)
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [TiposDocumento] WHERE [Nombre] = N'OID')
    BEGIN
        INSERT INTO [TiposDocumento] ([Id], [Nombre]) VALUES ('44444444-1111-2222-3333-555555555555', N'OID');
    END

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250917211543_AlignSnapshot_SeedOID'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250917211543_AlignSnapshot_SeedOID', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250917212328_SeedMinimalDataForTests'
)
BEGIN

    -- Usuario de prueba
    IF NOT EXISTS (SELECT 1 FROM [Usuarios] WHERE [Correo] = N'test.user@local')
    BEGIN
        DECLARE @TipoOID uniqueidentifier = (SELECT TOP 1 [Id] FROM [TiposDocumento] WHERE [Nombre] = N'OID');
        IF @TipoOID IS NULL BEGIN
            -- fallback a cualquier tipo
            SET @TipoOID = (SELECT TOP 1 [Id] FROM [TiposDocumento]);
        END
        INSERT INTO [Usuarios] ([IdUsuario],[NombreUsuario],[IdTipoDocumento],[NumeroDocumento],[Correo],[Telefono],[IdEstadoUsuario])
        VALUES ('aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee', N'Test User', @TipoOID, N'oid-test', N'test.user@local', N'0000000000', 1);
    END

    -- Alumno ligado al usuario de prueba
    IF NOT EXISTS (SELECT 1 FROM [Alumnos] WHERE [IdUsuario] = 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee')
    BEGIN
        INSERT INTO [Alumnos] ([IdAlumno],[IdUsuario])
        VALUES ('bbbbbbbb-cccc-dddd-eeee-ffffffffffff', 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee');
    END

    -- Paquete para el alumno de prueba (requerido por FK en Asistencias)
    DECLARE @PaqueteId uniqueidentifier = 'f0f0f0f0-0000-0000-0000-000000000000';
    IF NOT EXISTS (SELECT 1 FROM [Paquetes] WHERE [IdPaquete] = @PaqueteId)
    BEGIN
        DECLARE @TipoPaquete uniqueidentifier = (SELECT TOP 1 [Id] FROM [TiposPaquete] WHERE [Nombre] = N'Mensual');
        IF @TipoPaquete IS NULL SET @TipoPaquete = (SELECT TOP 1 [Id] FROM [TiposPaquete]);
        INSERT INTO [Paquetes] ([IdPaquete],[ClasesDisponibles],[ClasesUsadas],[FechaActivacion],[FechaVencimiento],[IdAlumno],[IdEstado],[IdPago],[IdTipoPaquete],[ValorPaquete])
        VALUES (@PaqueteId, 4, 0, CAST(GETDATE() AS date), DATEADD(day,30,CAST(GETDATE() AS date)), 'bbbbbbbb-cccc-dddd-eeee-ffffffffffff', 1, NULL, @TipoPaquete, 100000);
    END

    -- TipoProfesor ya existe (Principal) y TipoClase Regular ya existe
    -- Profesor para el usuario de prueba
    IF NOT EXISTS (SELECT 1 FROM [Profesores] WHERE [IdUsuario] = 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee')
    BEGIN
        DECLARE @TipoProfesor uniqueidentifier = (SELECT TOP 1 [Id] FROM [TiposProfesor] WHERE [Nombre] = N'Principal');
        INSERT INTO [Profesores] ([IdProfesor],[IdTipoProfesor],[IdUsuario])
        VALUES ('cccccccc-dddd-eeee-ffff-000000000000', @TipoProfesor, 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee');
    END

    -- Clase de prueba para hoy
    IF NOT EXISTS (SELECT 1 FROM [Clases] WHERE [IdClase] = 'dddddddd-eeee-ffff-0000-111111111111')
    BEGIN
        DECLARE @TipoClase uniqueidentifier = (SELECT TOP 1 [Id] FROM [TiposClase] WHERE [Nombre] = N'Regular');
        INSERT INTO [Clases] ([IdClase],[Fecha],[HoraInicio],[HoraFin],[IdTipoClase],[IdProfesorPrincipal])
        VALUES ('dddddddd-eeee-ffff-0000-111111111111', CAST(GETDATE() AS date), '10:00:00', '11:00:00', @TipoClase, 'cccccccc-dddd-eeee-ffff-000000000000');
    END

    -- Asistencia del alumno a esa clase (usa paquete creado)
    IF NOT EXISTS (SELECT 1 FROM [Asistencias] WHERE [IdClase] = 'dddddddd-eeee-ffff-0000-111111111111' AND [IdAlumno] = 'bbbbbbbb-cccc-dddd-eeee-ffffffffffff')
    BEGIN
        INSERT INTO [Asistencias] ([IdAsistencia],[IdClase],[IdAlumno],[IdEstado],[IdPaqueteUsado],[Observacion])
        VALUES ('eeeeeeee-ffff-0000-1111-222222222222', 'dddddddd-eeee-ffff-0000-111111111111', 'bbbbbbbb-cccc-dddd-eeee-ffffffffffff', 1, @PaqueteId, N'Asistencia de prueba');
    END

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250917212328_SeedMinimalDataForTests'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250917212328_SeedMinimalDataForTests', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Paquetes] ADD [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Paquetes] ADD [FechaModificacion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Paquetes] ADD [UsuarioCreacion] nvarchar(256) NOT NULL DEFAULT (SUSER_SNAME());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Paquetes] ADD [UsuarioModificacion] nvarchar(256) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Pagos] ADD [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Pagos] ADD [FechaModificacion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Pagos] ADD [UsuarioCreacion] nvarchar(256) NOT NULL DEFAULT (SUSER_SNAME());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Pagos] ADD [UsuarioModificacion] nvarchar(256) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Asistencias] ADD [FechaModificacion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Asistencias] ADD [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Asistencias] ADD [UsuarioCreacion] nvarchar(256) NOT NULL DEFAULT (SUSER_SNAME());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Asistencias] ADD [UsuarioModificacion] nvarchar(256) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [FechaInscripcion] datetime2 NOT NULL DEFAULT (GETDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [IdEstado] int NOT NULL DEFAULT 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    CREATE TABLE [EstadosAlumno] (
        [IdEstado] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        [Descripcion] nvarchar(200) NULL,
        CONSTRAINT [PK_EstadosAlumno] PRIMARY KEY ([IdEstado])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdEstado', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosAlumno]'))
        SET IDENTITY_INSERT [EstadosAlumno] ON;
    EXEC(N'INSERT INTO [EstadosAlumno] ([IdEstado], [Descripcion], [Nombre])
    VALUES (1, N''Alumno activo asistiendo a clases'', N''Activo''),
    (2, N''Alumno que dejó de asistir temporalmente'', N''Inactivo''),
    (3, N''Alumno suspendido por razones administrativas'', N''Suspendido''),
    (4, N''Alumno que se retiró definitivamente'', N''Retirado'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdEstado', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[EstadosAlumno]'))
        SET IDENTITY_INSERT [EstadosAlumno] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    CREATE INDEX [IX_Alumnos_IdEstado] ON [Alumnos] ([IdEstado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EstadosAlumno_Nombre] ON [EstadosAlumno] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    ALTER TABLE [Alumnos] ADD CONSTRAINT [FK_Alumnos_EstadosAlumno_IdEstado] FOREIGN KEY ([IdEstado]) REFERENCES [EstadosAlumno] ([IdEstado]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251124051035_SyncModelWithManualDatabaseChanges'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251124051035_SyncModelWithManualDatabaseChanges', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251230062749_RemoveRolesAndUsuarioRoles'
)
BEGIN
    DROP TABLE [UsuariosRoles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251230062749_RemoveRolesAndUsuarioRoles'
)
BEGIN
    DROP TABLE [Roles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251230062749_RemoveRolesAndUsuarioRoles'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251230062749_RemoveRolesAndUsuarioRoles', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110014008_AgregarCupoMaximoYObservacionesAClase'
)
BEGIN
    ALTER TABLE [Clases] ADD [CupoMaximo] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110014008_AgregarCupoMaximoYObservacionesAClase'
)
BEGIN
    ALTER TABLE [Clases] ADD [Observaciones] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110014008_AgregarCupoMaximoYObservacionesAClase'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260110014008_AgregarCupoMaximoYObservacionesAClase', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Asistencias]') AND [c].[name] = N'IdPaqueteUsado');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Asistencias] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Asistencias] ALTER COLUMN [IdPaqueteUsado] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos'
)
BEGIN
    ALTER TABLE [Asistencias] ADD [IdTipoAsistencia] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos'
)
BEGIN
    CREATE TABLE [TiposAsistencia] (
        [IdTipoAsistencia] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        [Descripcion] nvarchar(200) NOT NULL,
        [RequierePaquete] bit NOT NULL,
        [DescontarClase] bit NOT NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_TiposAsistencia] PRIMARY KEY ([IdTipoAsistencia])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdTipoAsistencia', N'Activo', N'DescontarClase', N'Descripcion', N'Nombre', N'RequierePaquete') AND [object_id] = OBJECT_ID(N'[TiposAsistencia]'))
        SET IDENTITY_INSERT [TiposAsistencia] ON;
    EXEC(N'INSERT INTO [TiposAsistencia] ([IdTipoAsistencia], [Activo], [DescontarClase], [Descripcion], [Nombre], [RequierePaquete])
    VALUES (1, CAST(1 AS bit), CAST(1 AS bit), N''Asistencia normal con paquete activo'', N''Normal'', CAST(1 AS bit)),
    (2, CAST(1 AS bit), CAST(0 AS bit), N''Clase de cortesía sin descuento de paquete'', N''Cortesía'', CAST(0 AS bit)),
    (3, CAST(1 AS bit), CAST(0 AS bit), N''Clase de prueba para nuevos alumnos'', N''Clase de Prueba'', CAST(0 AS bit)),
    (4, CAST(1 AS bit), CAST(0 AS bit), N''Clase de recuperación por inasistencia justificada'', N''Recuperación'', CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdTipoAsistencia', N'Activo', N'DescontarClase', N'Descripcion', N'Nombre', N'RequierePaquete') AND [object_id] = OBJECT_ID(N'[TiposAsistencia]'))
        SET IDENTITY_INSERT [TiposAsistencia] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos'
)
BEGIN

                    UPDATE Asistencias 
                    SET IdTipoAsistencia = 1 
                    WHERE IdTipoAsistencia = 0 OR IdTipoAsistencia IS NULL;
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos'
)
BEGIN
    CREATE INDEX [IX_Asistencias_IdTipoAsistencia] ON [Asistencias] ([IdTipoAsistencia]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TiposAsistencia_Nombre] ON [TiposAsistencia] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos'
)
BEGIN
    ALTER TABLE [Asistencias] ADD CONSTRAINT [FK_Asistencias_TiposAsistencia_IdTipoAsistencia] FOREIGN KEY ([IdTipoAsistencia]) REFERENCES [TiposAsistencia] ([IdTipoAsistencia]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260124010244_AgregarCatalogoTipoAsistenciaConDatos', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127082030_AddEventosTable'
)
BEGIN
    CREATE TABLE [Eventos] (
        [IdEvento] uniqueidentifier NOT NULL,
        [Titulo] nvarchar(200) NOT NULL,
        [Descripcion] nvarchar(1000) NULL,
        [Fecha] datetime2 NOT NULL,
        [Hora] time NULL,
        [Precio] decimal(10,2) NULL,
        [Destacado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [ImagenUrl] nvarchar(500) NULL,
        [ImagenNombre] nvarchar(200) NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaModificacion] datetime2 NULL,
        [IdUsuarioCreador] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Eventos] PRIMARY KEY ([IdEvento]),
        CONSTRAINT [FK_Eventos_Usuarios_IdUsuarioCreador] FOREIGN KEY ([IdUsuarioCreador]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127082030_AddEventosTable'
)
BEGIN
    CREATE INDEX [IX_Eventos_Activo] ON [Eventos] ([Activo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127082030_AddEventosTable'
)
BEGIN
    CREATE INDEX [IX_Eventos_Destacado] ON [Eventos] ([Destacado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127082030_AddEventosTable'
)
BEGIN
    CREATE INDEX [IX_Eventos_Fecha] ON [Eventos] ([Fecha]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127082030_AddEventosTable'
)
BEGIN
    CREATE INDEX [IX_Eventos_IdUsuarioCreador] ON [Eventos] ([IdUsuarioCreador]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127082030_AddEventosTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260127082030_AddEventosTable', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127085435_SeedEventosEjemplo'
)
BEGIN

                    DECLARE @IdUsuarioAdmin UNIQUEIDENTIFIER;
                    SELECT TOP 1 @IdUsuarioAdmin = IdUsuario 
                    FROM Usuarios 
                    WHERE Correo = 'Chetango@chetangoprueba.onmicrosoft.com';

                    -- Si no existe el admin, usar el primer usuario disponible
                    IF @IdUsuarioAdmin IS NULL
                    BEGIN
                        SELECT TOP 1 @IdUsuarioAdmin = IdUsuario FROM Usuarios;
                    END

                    -- Evento 1: Taller de Musicalidad (Destacado)
                    IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Taller de Musicalidad')
                    BEGIN
                        INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, ImagenNombre, Activo, FechaCreacion, FechaModificacion, IdUsuarioCreador)
                        VALUES (
                            NEWID(),
                            'Taller de Musicalidad',
                            'Mejora tu interpretación musical en el tango. Aprende a escuchar y expresar la música a través del movimiento.',
                            DATEADD(DAY, 9, GETDATE()),
                            '15:00:00',
                            25000,
                            1,
                            'https://images.unsplash.com/photo-1504609813442-a8924e83f76e?w=800',
                            'taller-musicalidad.jpg',
                            1,
                            GETDATE(),
                            GETDATE(),
                            @IdUsuarioAdmin
                        );
                    END

                    -- Evento 2: Milonga de Práctica (Gratis)
                    IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Milonga de Práctica')
                    BEGIN
                        INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, ImagenNombre, Activo, FechaCreacion, FechaModificacion, IdUsuarioCreador)
                        VALUES (
                            NEWID(),
                            'Milonga de Práctica',
                            'Noche de baile social para practicar lo aprendido en clases. Ambiente relajado y amigable para todos los niveles.',
                            DATEADD(DAY, 4, GETDATE()),
                            '21:00:00',
                            NULL,
                            0,
                            'https://images.unsplash.com/photo-1571156230214-50e0b9d14d45?w=800',
                            'milonga-practica.jpg',
                            1,
                            GETDATE(),
                            GETDATE(),
                            @IdUsuarioAdmin
                        );
                    END

                    -- Evento 3: Workshop Avanzado
                    IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Workshop Avanzado de Tango')
                    BEGIN
                        INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, ImagenNombre, Activo, FechaCreacion, FechaModificacion, IdUsuarioCreador)
                        VALUES (
                            NEWID(),
                            'Workshop Avanzado de Tango',
                            'Técnicas avanzadas de tango para bailarines experimentados. Incluye boleos, ganchos y sacadas complejas.',
                            DATEADD(DAY, 16, GETDATE()),
                            '16:00:00',
                            40000,
                            1,
                            'https://images.unsplash.com/photo-1508700929628-666bc8bd84ea?w=800',
                            'workshop-avanzado.jpg',
                            1,
                            GETDATE(),
                            GETDATE(),
                            @IdUsuarioAdmin
                        );
                    END
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127085435_SeedEventosEjemplo'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260127085435_SeedEventosEjemplo', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127160059_AddPerfilAlumnoFields'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [AlertasPaquete] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127160059_AddPerfilAlumnoFields'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [AvatarUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127160059_AddPerfilAlumnoFields'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [ContactoEmergenciaNombre] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127160059_AddPerfilAlumnoFields'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [ContactoEmergenciaRelacion] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127160059_AddPerfilAlumnoFields'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [ContactoEmergenciaTelefono] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127160059_AddPerfilAlumnoFields'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [NotificacionesEmail] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127160059_AddPerfilAlumnoFields'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [RecordatoriosClase] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127160059_AddPerfilAlumnoFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260127160059_AddPerfilAlumnoFields', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127163636_AgregarPerfilProfesor'
)
BEGIN
    ALTER TABLE [Profesores] ADD [AlertasCambios] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127163636_AgregarPerfilProfesor'
)
BEGIN
    ALTER TABLE [Profesores] ADD [Biografia] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127163636_AgregarPerfilProfesor'
)
BEGIN
    ALTER TABLE [Profesores] ADD [Especialidades] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127163636_AgregarPerfilProfesor'
)
BEGIN
    ALTER TABLE [Profesores] ADD [NotificacionesEmail] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127163636_AgregarPerfilProfesor'
)
BEGIN
    ALTER TABLE [Profesores] ADD [RecordatoriosClase] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127163636_AgregarPerfilProfesor'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260127163636_AgregarPerfilProfesor', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE TABLE [ClasesProfesores] (
        [IdClaseProfesor] uniqueidentifier NOT NULL,
        [IdClase] uniqueidentifier NOT NULL,
        [IdProfesor] uniqueidentifier NOT NULL,
        [IdRolEnClase] uniqueidentifier NOT NULL,
        [TarifaProgramada] decimal(18,2) NOT NULL,
        [ValorAdicional] decimal(18,2) NOT NULL DEFAULT 0.0,
        [ConceptoAdicional] nvarchar(500) NULL,
        [TotalPago] decimal(18,2) NOT NULL,
        [EstadoPago] nvarchar(50) NOT NULL DEFAULT N'Pendiente',
        [FechaAprobacion] datetime2 NULL,
        [FechaPago] datetime2 NULL,
        [AprobadoPorIdUsuario] uniqueidentifier NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaModificacion] datetime2 NULL,
        CONSTRAINT [PK_ClasesProfesores] PRIMARY KEY ([IdClaseProfesor]),
        CONSTRAINT [FK_ClasesProfesores_Clases_IdClase] FOREIGN KEY ([IdClase]) REFERENCES [Clases] ([IdClase]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ClasesProfesores_Profesores_IdProfesor] FOREIGN KEY ([IdProfesor]) REFERENCES [Profesores] ([IdProfesor]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ClasesProfesores_RolesEnClase_IdRolEnClase] FOREIGN KEY ([IdRolEnClase]) REFERENCES [RolesEnClase] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ClasesProfesores_Usuarios_AprobadoPorIdUsuario] FOREIGN KEY ([AprobadoPorIdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE TABLE [LiquidacionesMensuales] (
        [IdLiquidacion] uniqueidentifier NOT NULL,
        [IdProfesor] uniqueidentifier NOT NULL,
        [Mes] int NOT NULL,
        [Año] int NOT NULL,
        [TotalClases] int NOT NULL,
        [TotalHoras] decimal(10,2) NOT NULL,
        [TotalBase] decimal(18,2) NOT NULL,
        [TotalAdicionales] decimal(18,2) NOT NULL DEFAULT 0.0,
        [TotalPagar] decimal(18,2) NOT NULL,
        [Estado] nvarchar(50) NOT NULL DEFAULT N'EnProceso',
        [FechaCierre] datetime2 NULL,
        [FechaPago] datetime2 NULL,
        [Observaciones] nvarchar(1000) NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [CreadoPorIdUsuario] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_LiquidacionesMensuales] PRIMARY KEY ([IdLiquidacion]),
        CONSTRAINT [FK_LiquidacionesMensuales_Profesores_IdProfesor] FOREIGN KEY ([IdProfesor]) REFERENCES [Profesores] ([IdProfesor]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LiquidacionesMensuales_Usuarios_CreadoPorIdUsuario] FOREIGN KEY ([CreadoPorIdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE INDEX [IX_ClasesProfesores_AprobadoPorIdUsuario] ON [ClasesProfesores] ([AprobadoPorIdUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE INDEX [IX_ClasesProfesores_EstadoPago] ON [ClasesProfesores] ([EstadoPago]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE INDEX [IX_ClasesProfesores_FechaAprobacion] ON [ClasesProfesores] ([FechaAprobacion]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE INDEX [IX_ClasesProfesores_IdClase_IdProfesor] ON [ClasesProfesores] ([IdClase], [IdProfesor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE INDEX [IX_ClasesProfesores_IdProfesor] ON [ClasesProfesores] ([IdProfesor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE INDEX [IX_ClasesProfesores_IdRolEnClase] ON [ClasesProfesores] ([IdRolEnClase]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE INDEX [IX_LiquidacionesMensuales_CreadoPorIdUsuario] ON [LiquidacionesMensuales] ([CreadoPorIdUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE INDEX [IX_LiquidacionesMensuales_Estado] ON [LiquidacionesMensuales] ([Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LiquidacionesMensuales_IdProfesor_Mes_Año] ON [LiquidacionesMensuales] ([IdProfesor], [Mes], [Año]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129063224_AgregarSistemaNomina'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260129063224_AgregarSistemaNomina', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129073842_AgregarEstadoAClases'
)
BEGIN
    ALTER TABLE [Clases] ADD [Estado] nvarchar(50) NOT NULL DEFAULT N'Programada';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129073842_AgregarEstadoAClases'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260129073842_AgregarEstadoAClases', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203163138_AgregarTarifaActualProfesor'
)
BEGIN
    ALTER TABLE [Profesores] ADD [TarifaActual] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203163138_AgregarTarifaActualProfesor'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260203163138_AgregarTarifaActualProfesor', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203172354_SistemaMultiplesProfesoresYTarifasIndividuales'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Clases]') AND [c].[name] = N'IdProfesorPrincipal');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Clases] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Clases] ALTER COLUMN [IdProfesorPrincipal] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203172354_SistemaMultiplesProfesoresYTarifasIndividuales'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260203172354_SistemaMultiplesProfesoresYTarifasIndividuales', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE TABLE [CodigosReferido] (
        [IdCodigo] uniqueidentifier NOT NULL,
        [IdAlumno] uniqueidentifier NOT NULL,
        [Codigo] nvarchar(20) NOT NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [VecesUsado] int NOT NULL DEFAULT 0,
        [BeneficioReferidor] nvarchar(500) NULL,
        [BeneficioNuevoAlumno] nvarchar(500) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [FechaModificacion] datetime2 NULL,
        CONSTRAINT [PK_CodigosReferido] PRIMARY KEY ([IdCodigo]),
        CONSTRAINT [FK_CodigosReferido_Alumnos_IdAlumno] FOREIGN KEY ([IdAlumno]) REFERENCES [Alumnos] ([IdAlumno]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE TABLE [SolicitudesClasePrivada] (
        [IdSolicitud] uniqueidentifier NOT NULL,
        [IdAlumno] uniqueidentifier NOT NULL,
        [IdTipoClaseDeseado] uniqueidentifier NULL,
        [TipoClaseDeseado] nvarchar(200) NOT NULL,
        [FechaPreferida] datetime2 NULL,
        [HoraPreferida] time NULL,
        [ObservacionesAlumno] nvarchar(1000) NULL,
        [Estado] nvarchar(50) NOT NULL DEFAULT N'Pendiente',
        [FechaSolicitud] datetime2 NOT NULL,
        [FechaRespuesta] datetime2 NULL,
        [IdUsuarioRespondio] uniqueidentifier NULL,
        [MensajeRespuesta] nvarchar(1000) NULL,
        [IdClaseCreada] uniqueidentifier NULL,
        CONSTRAINT [PK_SolicitudesClasePrivada] PRIMARY KEY ([IdSolicitud]),
        CONSTRAINT [FK_SolicitudesClasePrivada_Alumnos_IdAlumno] FOREIGN KEY ([IdAlumno]) REFERENCES [Alumnos] ([IdAlumno]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE TABLE [SolicitudesRenovacionPaquete] (
        [IdSolicitud] uniqueidentifier NOT NULL,
        [IdAlumno] uniqueidentifier NOT NULL,
        [IdPaqueteActual] uniqueidentifier NULL,
        [IdTipoPaqueteDeseado] uniqueidentifier NULL,
        [TipoPaqueteDeseado] nvarchar(200) NOT NULL,
        [MensajeAlumno] nvarchar(1000) NULL,
        [Estado] nvarchar(50) NOT NULL DEFAULT N'Pendiente',
        [FechaSolicitud] datetime2 NOT NULL,
        [FechaRespuesta] datetime2 NULL,
        [IdUsuarioRespondio] uniqueidentifier NULL,
        [MensajeRespuesta] nvarchar(1000) NULL,
        [IdPaqueteCreado] uniqueidentifier NULL,
        CONSTRAINT [PK_SolicitudesRenovacionPaquete] PRIMARY KEY ([IdSolicitud]),
        CONSTRAINT [FK_SolicitudesRenovacionPaquete_Alumnos_IdAlumno] FOREIGN KEY ([IdAlumno]) REFERENCES [Alumnos] ([IdAlumno]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE TABLE [UsosCodigoReferido] (
        [IdUso] uniqueidentifier NOT NULL,
        [IdCodigoReferido] uniqueidentifier NOT NULL,
        [IdAlumnoReferidor] uniqueidentifier NOT NULL,
        [IdAlumnoNuevo] uniqueidentifier NOT NULL,
        [FechaUso] datetime2 NOT NULL,
        [Estado] nvarchar(50) NOT NULL DEFAULT N'Pendiente',
        [BeneficioAplicadoReferidor] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaBeneficioReferidor] datetime2 NULL,
        [BeneficioAplicadoNuevo] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaBeneficioNuevo] datetime2 NULL,
        [Observaciones] nvarchar(1000) NULL,
        CONSTRAINT [PK_UsosCodigoReferido] PRIMARY KEY ([IdUso]),
        CONSTRAINT [FK_UsosCodigoReferido_Alumnos_IdAlumnoNuevo] FOREIGN KEY ([IdAlumnoNuevo]) REFERENCES [Alumnos] ([IdAlumno]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UsosCodigoReferido_Alumnos_IdAlumnoReferidor] FOREIGN KEY ([IdAlumnoReferidor]) REFERENCES [Alumnos] ([IdAlumno]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UsosCodigoReferido_CodigosReferido_IdCodigoReferido] FOREIGN KEY ([IdCodigoReferido]) REFERENCES [CodigosReferido] ([IdCodigo]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_CodigosReferido_Activo] ON [CodigosReferido] ([Activo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CodigosReferido_Codigo] ON [CodigosReferido] ([Codigo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_CodigosReferido_IdAlumno] ON [CodigosReferido] ([IdAlumno]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_SolicitudesClasePrivada_Estado] ON [SolicitudesClasePrivada] ([Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_SolicitudesClasePrivada_FechaSolicitud] ON [SolicitudesClasePrivada] ([FechaSolicitud]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_SolicitudesClasePrivada_IdAlumno] ON [SolicitudesClasePrivada] ([IdAlumno]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_SolicitudesRenovacionPaquete_Estado] ON [SolicitudesRenovacionPaquete] ([Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_SolicitudesRenovacionPaquete_FechaSolicitud] ON [SolicitudesRenovacionPaquete] ([FechaSolicitud]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_SolicitudesRenovacionPaquete_IdAlumno] ON [SolicitudesRenovacionPaquete] ([IdAlumno]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_UsosCodigoReferido_Estado] ON [UsosCodigoReferido] ([Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_UsosCodigoReferido_FechaUso] ON [UsosCodigoReferido] ([FechaUso]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_UsosCodigoReferido_IdAlumnoNuevo] ON [UsosCodigoReferido] ([IdAlumnoNuevo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_UsosCodigoReferido_IdAlumnoReferidor] ON [UsosCodigoReferido] ([IdAlumnoReferidor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    CREATE INDEX [IX_UsosCodigoReferido_IdCodigoReferido] ON [UsosCodigoReferido] ([IdCodigoReferido]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260209034107_SincronizarModeloCompleto'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260209034107_SincronizarModeloCompleto', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260211070714_AgregarColumnasTiposPaqueteYPagos_Safe'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260211070714_AgregarColumnasTiposPaqueteYPagos_Safe', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260218150247_AgregarCampoSede'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [Sede] int NOT NULL DEFAULT 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260218150247_AgregarCampoSede'
)
BEGIN
    ALTER TABLE [Pagos] ADD [Sede] int NOT NULL DEFAULT 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260218150247_AgregarCampoSede'
)
BEGIN
    ALTER TABLE [LiquidacionesMensuales] ADD [Sede] int NOT NULL DEFAULT 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260218150247_AgregarCampoSede'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260218150247_AgregarCampoSede', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221035857_AddSoftDeleteToPago'
)
BEGIN
    ALTER TABLE [Pagos] ADD [Eliminado] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221035857_AddSoftDeleteToPago'
)
BEGIN
    ALTER TABLE [Pagos] ADD [FechaEliminacion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221035857_AddSoftDeleteToPago'
)
BEGIN
    ALTER TABLE [Pagos] ADD [UsuarioEliminacion] nvarchar(256) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221035857_AddSoftDeleteToPago'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260221035857_AddSoftDeleteToPago', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE TABLE [CategoriasGasto] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        CONSTRAINT [PK_CategoriasGasto] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE TABLE [CategoriasIngreso] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        CONSTRAINT [PK_CategoriasIngreso] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE TABLE [OtrosGastos] (
        [IdOtroGasto] uniqueidentifier NOT NULL,
        [Concepto] nvarchar(200) NOT NULL,
        [Monto] decimal(18,2) NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Sede] int NOT NULL,
        [IdCategoriaGasto] uniqueidentifier NULL,
        [Proveedor] nvarchar(200) NULL,
        [Descripcion] nvarchar(1000) NULL,
        [UrlFactura] nvarchar(500) NULL,
        [NumeroFactura] nvarchar(100) NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaModificacion] datetime2 NULL,
        [UsuarioCreacion] nvarchar(256) NOT NULL DEFAULT (SUSER_SNAME()),
        [UsuarioModificacion] nvarchar(256) NULL,
        [Eliminado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaEliminacion] datetime2 NULL,
        [UsuarioEliminacion] nvarchar(256) NULL,
        CONSTRAINT [PK_OtrosGastos] PRIMARY KEY ([IdOtroGasto]),
        CONSTRAINT [FK_OtrosGastos_CategoriasGasto_IdCategoriaGasto] FOREIGN KEY ([IdCategoriaGasto]) REFERENCES [CategoriasGasto] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE TABLE [OtrosIngresos] (
        [IdOtroIngreso] uniqueidentifier NOT NULL,
        [Concepto] nvarchar(200) NOT NULL,
        [Monto] decimal(18,2) NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Sede] int NOT NULL,
        [IdCategoriaIngreso] uniqueidentifier NULL,
        [Descripcion] nvarchar(1000) NULL,
        [UrlComprobante] nvarchar(500) NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaModificacion] datetime2 NULL,
        [UsuarioCreacion] nvarchar(256) NOT NULL DEFAULT (SUSER_SNAME()),
        [UsuarioModificacion] nvarchar(256) NULL,
        [Eliminado] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaEliminacion] datetime2 NULL,
        [UsuarioEliminacion] nvarchar(256) NULL,
        CONSTRAINT [PK_OtrosIngresos] PRIMARY KEY ([IdOtroIngreso]),
        CONSTRAINT [FK_OtrosIngresos_CategoriasIngreso_IdCategoriaIngreso] FOREIGN KEY ([IdCategoriaIngreso]) REFERENCES [CategoriasIngreso] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[CategoriasGasto]'))
        SET IDENTITY_INSERT [CategoriasGasto] ON;
    EXEC(N'INSERT INTO [CategoriasGasto] ([Id], [Descripcion], [Nombre])
    VALUES (''f1111111-0000-0000-0000-000000000001'', N''Pago mensual del local o salón'', N''Arriendo''),
    (''f1111111-0000-0000-0000-000000000002'', N''Luz, agua, internet, teléfono'', N''Servicios Públicos''),
    (''f1111111-0000-0000-0000-000000000003'', N''Reparaciones, mantenimiento de equipos e instalaciones'', N''Mantenimiento''),
    (''f1111111-0000-0000-0000-000000000004'', N''Publicidad, redes sociales, diseño gráfico'', N''Marketing''),
    (''f1111111-0000-0000-0000-000000000005'', N''Material de oficina, limpieza, consumibles'', N''Suministros''),
    (''f1111111-0000-0000-0000-000000000006'', N''Espejos, barras, sonido, iluminación'', N''Equipamiento''),
    (''f1111111-0000-0000-0000-000000000007'', N''Impuestos, seguros, trámites legales'', N''Impuestos y Seguros''),
    (''f1111111-0000-0000-0000-000000000008'', N''Transporte de profesores o materiales'', N''Transporte''),
    (''f1111111-0000-0000-0000-000000000009'', N''Otros gastos no clasificados'', N''Otros'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[CategoriasGasto]'))
        SET IDENTITY_INSERT [CategoriasGasto] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[CategoriasIngreso]'))
        SET IDENTITY_INSERT [CategoriasIngreso] ON;
    EXEC(N'INSERT INTO [CategoriasIngreso] ([Id], [Descripcion], [Nombre])
    VALUES (''e1111111-0000-0000-0000-000000000001'', N''Ingresos por eventos especiales, shows, presentaciones'', N''Eventos''),
    (''e1111111-0000-0000-0000-000000000002'', N''Ingresos por alquiler del salón para eventos externos'', N''Alquiler de Espacio''),
    (''e1111111-0000-0000-0000-000000000003'', N''Venta de camisetas, zapatos, accesorios de danza'', N''Mercancía''),
    (''e1111111-0000-0000-0000-000000000004'', N''Presentaciones privadas contratadas'', N''Shows Privados''),
    (''e1111111-0000-0000-0000-000000000005'', N''Talleres impartidos fuera de la academia'', N''Talleres Externos''),
    (''e1111111-0000-0000-0000-000000000006'', N''Ingresos por patrocinios de marcas o empresas'', N''Patrocinios''),
    (''e1111111-0000-0000-0000-000000000007'', N''Otros ingresos no clasificados'', N''Otros'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Descripcion', N'Nombre') AND [object_id] = OBJECT_ID(N'[CategoriasIngreso]'))
        SET IDENTITY_INSERT [CategoriasIngreso] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CategoriasGasto_Nombre] ON [CategoriasGasto] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CategoriasIngreso_Nombre] ON [CategoriasIngreso] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE INDEX [IX_OtrosGastos_Fecha] ON [OtrosGastos] ([Fecha]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE INDEX [IX_OtrosGastos_Fecha_Sede] ON [OtrosGastos] ([Fecha], [Sede]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE INDEX [IX_OtrosGastos_IdCategoriaGasto] ON [OtrosGastos] ([IdCategoriaGasto]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE INDEX [IX_OtrosGastos_Sede] ON [OtrosGastos] ([Sede]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE INDEX [IX_OtrosIngresos_Fecha] ON [OtrosIngresos] ([Fecha]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE INDEX [IX_OtrosIngresos_Fecha_Sede] ON [OtrosIngresos] ([Fecha], [Sede]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE INDEX [IX_OtrosIngresos_IdCategoriaIngreso] ON [OtrosIngresos] ([IdCategoriaIngreso]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    CREATE INDEX [IX_OtrosIngresos_Sede] ON [OtrosIngresos] ([Sede]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260225063042_AgregarOtrosIngresosGastos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260225063042_AgregarOtrosIngresosGastos', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE TABLE [ConfiguracionPagos] (
        [Id] int NOT NULL IDENTITY,
        [Banco] nvarchar(100) NOT NULL,
        [TipoCuenta] nvarchar(50) NOT NULL,
        [NumeroCuenta] nvarchar(50) NOT NULL,
        [Titular] nvarchar(200) NOT NULL,
        [NIT] nvarchar(50) NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [MostrarEnPortal] bit NOT NULL DEFAULT CAST(1 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaModificacion] datetime2 NULL,
        [CreadoPor] nvarchar(256) NULL DEFAULT (SUSER_SNAME()),
        [ModificadoPor] nvarchar(256) NULL,
        CONSTRAINT [PK_ConfiguracionPagos] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE TABLE [Tenants] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(200) NOT NULL,
        [Subdomain] nvarchar(50) NOT NULL,
        [Plan] nvarchar(20) NOT NULL,
        [Estado] nvarchar(20) NOT NULL DEFAULT N'Activo',
        [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaVencimientoPlan] datetime2 NULL,
        [MaxSedes] int NOT NULL,
        [MaxAlumnos] int NOT NULL,
        [MaxProfesores] int NOT NULL,
        [MaxStorageMB] int NOT NULL,
        [EmailContacto] nvarchar(100) NOT NULL,
        [TelefonoContacto] nvarchar(20) NULL,
        [LogoUrl] nvarchar(500) NULL,
        [ColorPrimario] nvarchar(7) NULL,
        [ColorSecundario] nvarchar(7) NULL,
        [ColorAccent] nvarchar(7) NULL,
        [NombreComercial] nvarchar(200) NULL,
        [FaviconUrl] nvarchar(500) NULL,
        [WompiSubscriptionId] nvarchar(100) NULL,
        [StripeCustomerId] nvarchar(100) NULL,
        [MetodoPagoPreferido] nvarchar(50) NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaActualizacion] datetime2 NULL,
        [CreadoPor] nvarchar(256) NULL,
        [ActualizadoPor] nvarchar(256) NULL,
        CONSTRAINT [PK_Tenants] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE TABLE [PagosSuscripcion] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [FechaPago] datetime2 NOT NULL,
        [Monto] decimal(18,2) NOT NULL,
        [Referencia] nvarchar(50) NOT NULL,
        [MetodoPago] nvarchar(50) NOT NULL,
        [ComprobanteUrl] nvarchar(500) NULL,
        [NombreArchivo] nvarchar(200) NULL,
        [TamanoArchivo] int NULL,
        [Estado] nvarchar(20) NOT NULL DEFAULT N'Pendiente',
        [AprobadoPor] nvarchar(100) NULL,
        [FechaAprobacion] datetime2 NULL,
        [Observaciones] nvarchar(500) NULL,
        [TransaccionId] nvarchar(100) NULL,
        [EstadoTransaccion] nvarchar(50) NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaModificacion] datetime2 NULL,
        [CreadoPor] nvarchar(256) NULL DEFAULT (SUSER_SNAME()),
        [ModificadoPor] nvarchar(256) NULL,
        CONSTRAINT [PK_PagosSuscripcion] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PagosSuscripcion_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE INDEX [IX_PagosSuscripcion_Estado] ON [PagosSuscripcion] ([Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE INDEX [IX_PagosSuscripcion_FechaPago] ON [PagosSuscripcion] ([FechaPago]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PagosSuscripcion_Referencia] ON [PagosSuscripcion] ([Referencia]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE INDEX [IX_PagosSuscripcion_TenantId] ON [PagosSuscripcion] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE INDEX [IX_PagosSuscripcion_TenantId_FechaPago] ON [PagosSuscripcion] ([TenantId], [FechaPago]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Tenants_EmailContacto] ON [Tenants] ([EmailContacto]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Tenants_Subdomain] ON [Tenants] ([Subdomain]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031050_AgregarSistemaSuscripciones'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260228031050_AgregarSistemaSuscripciones', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031940_SeedDatosSuscripciones'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre', N'Subdomain', N'Plan', N'Estado', N'FechaRegistro', N'FechaVencimientoPlan', N'MaxSedes', N'MaxAlumnos', N'MaxProfesores', N'MaxStorageMB', N'EmailContacto', N'TelefonoContacto', N'CreadoPor') AND [object_id] = OBJECT_ID(N'[Tenants]'))
        SET IDENTITY_INSERT [Tenants] ON;
    EXEC(N'INSERT INTO [Tenants] ([Id], [Nombre], [Subdomain], [Plan], [Estado], [FechaRegistro], [FechaVencimientoPlan], [MaxSedes], [MaxAlumnos], [MaxProfesores], [MaxStorageMB], [EmailContacto], [TelefonoContacto], [CreadoPor])
    VALUES (''a1b2c3d4-e5f6-7890-abcd-ef1234567890'', N''Corporación Chetango'', N''corporacionchetango'', N''Enterprise'', N''Activo'', ''2024-01-01T00:00:00.0000000'', ''2026-12-31T00:00:00.0000000'', 99999, 99999, 99999, 999999, N''chetango.corporacion@corporacionchetango.com'', N''+57 300 123 4567'', N''MIGRATION_SEED'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nombre', N'Subdomain', N'Plan', N'Estado', N'FechaRegistro', N'FechaVencimientoPlan', N'MaxSedes', N'MaxAlumnos', N'MaxProfesores', N'MaxStorageMB', N'EmailContacto', N'TelefonoContacto', N'CreadoPor') AND [object_id] = OBJECT_ID(N'[Tenants]'))
        SET IDENTITY_INSERT [Tenants] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031940_SeedDatosSuscripciones'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Banco', N'TipoCuenta', N'NumeroCuenta', N'Titular', N'NIT', N'Activo', N'MostrarEnPortal', N'CreadoPor') AND [object_id] = OBJECT_ID(N'[ConfiguracionPagos]'))
        SET IDENTITY_INSERT [ConfiguracionPagos] ON;
    EXEC(N'INSERT INTO [ConfiguracionPagos] ([Banco], [TipoCuenta], [NumeroCuenta], [Titular], [NIT], [Activo], [MostrarEnPortal], [CreadoPor])
    VALUES (N''Bancolombia'', N''Ahorros'', N''123-456-789'', N''Aphellion SAS'', N''900.123.456-7'', CAST(1 AS bit), CAST(1 AS bit), N''MIGRATION_SEED'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Banco', N'TipoCuenta', N'NumeroCuenta', N'Titular', N'NIT', N'Activo', N'MostrarEnPortal', N'CreadoPor') AND [object_id] = OBJECT_ID(N'[ConfiguracionPagos]'))
        SET IDENTITY_INSERT [ConfiguracionPagos] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228031940_SeedDatosSuscripciones'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260228031940_SeedDatosSuscripciones', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [SolicitudesRenovacionPaquete] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [SolicitudesClasePrivada] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Paquetes] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Pagos] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Eventos] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Clases] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Asistencias] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    CREATE INDEX [IX_Usuarios_TenantId] ON [Usuarios] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    CREATE INDEX [IX_SolicitudesRenovacionPaquete_TenantId] ON [SolicitudesRenovacionPaquete] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    CREATE INDEX [IX_SolicitudesClasePrivada_TenantId] ON [SolicitudesClasePrivada] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    CREATE INDEX [IX_Paquetes_TenantId] ON [Paquetes] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    CREATE INDEX [IX_Pagos_TenantId] ON [Pagos] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    CREATE INDEX [IX_Eventos_TenantId] ON [Eventos] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    CREATE INDEX [IX_Clases_TenantId] ON [Clases] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    CREATE INDEX [IX_Asistencias_TenantId] ON [Asistencias] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Asistencias] ADD CONSTRAINT [FK_Asistencias_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Clases] ADD CONSTRAINT [FK_Clases_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Eventos] ADD CONSTRAINT [FK_Eventos_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Pagos] ADD CONSTRAINT [FK_Pagos_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Paquetes] ADD CONSTRAINT [FK_Paquetes_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [SolicitudesClasePrivada] ADD CONSTRAINT [FK_SolicitudesClasePrivada_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [SolicitudesRenovacionPaquete] ADD CONSTRAINT [FK_SolicitudesRenovacionPaquete_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    ALTER TABLE [Usuarios] ADD CONSTRAINT [FK_Usuarios_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN

                    -- Asignar TenantId a Usuarios existentes
                    UPDATE Usuarios 
                    SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' 
                    WHERE TenantId IS NULL;

                    -- Asignar TenantId a Clases existentes
                    UPDATE Clases 
                    SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' 
                    WHERE TenantId IS NULL;

                    -- Asignar TenantId a Pagos existentes
                    UPDATE Pagos 
                    SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' 
                    WHERE TenantId IS NULL;

                    -- Asignar TenantId a Paquetes existentes
                    UPDATE Paquetes 
                    SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' 
                    WHERE TenantId IS NULL;

                    -- Asignar TenantId a Asistencias existentes
                    UPDATE Asistencias 
                    SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' 
                    WHERE TenantId IS NULL;

                    -- Asignar TenantId a Eventos existentes
                    UPDATE Eventos 
                    SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' 
                    WHERE TenantId IS NULL;

                    -- Asignar TenantId a Solicitudes de Clase Privada existentes
                    UPDATE SolicitudesClasePrivada 
                    SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' 
                    WHERE TenantId IS NULL;

                    -- Asignar TenantId a Solicitudes de Renovación existentes
                    UPDATE SolicitudesRenovacionPaquete 
                    SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' 
                    WHERE TenantId IS NULL;
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228055648_AgregarMultiTenancy'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260228055648_AgregarMultiTenancy', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228071433_CambiarMultiTenancyADominioCompleto'
)
BEGIN
    ALTER TABLE [Usuarios] DROP CONSTRAINT [FK_Usuarios_Tenants_TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228071433_CambiarMultiTenancyADominioCompleto'
)
BEGIN
    DROP INDEX [IX_Usuarios_TenantId] ON [Usuarios];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228071433_CambiarMultiTenancyADominioCompleto'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuarios]') AND [c].[name] = N'TenantId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Usuarios] DROP COLUMN [TenantId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228071433_CambiarMultiTenancyADominioCompleto'
)
BEGIN
    ALTER TABLE [Tenants] ADD [Dominio] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228071433_CambiarMultiTenancyADominioCompleto'
)
BEGIN

                    UPDATE Tenants 
                    SET Dominio = 'corporacionchetango.aphelion.com'
                    WHERE Nombre = 'Corporación Chetango';
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260228071433_CambiarMultiTenancyADominioCompleto'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260228071433_CambiarMultiTenancyADominioCompleto', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304044601_AgregarTenantUsersYCorregirMultiTenancy'
)
BEGIN
    CREATE TABLE [TenantUsers] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [IdUsuario] uniqueidentifier NOT NULL,
        [FechaAsignacion] datetime2 NOT NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_TenantUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TenantUsers_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TenantUsers_Usuarios_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304044601_AgregarTenantUsersYCorregirMultiTenancy'
)
BEGIN
    CREATE INDEX [IX_TenantUsers_IdUsuario] ON [TenantUsers] ([IdUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304044601_AgregarTenantUsersYCorregirMultiTenancy'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TenantUsers_TenantId_IdUsuario] ON [TenantUsers] ([TenantId], [IdUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304044601_AgregarTenantUsersYCorregirMultiTenancy'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260304044601_AgregarTenantUsersYCorregirMultiTenancy', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [Usuarios] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [Profesores] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [OtrosIngresos] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [OtrosGastos] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [LiquidacionesMensuales] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [Alumnos] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    CREATE INDEX [IX_Usuarios_TenantId] ON [Usuarios] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    CREATE INDEX [IX_Profesores_TenantId] ON [Profesores] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    CREATE INDEX [IX_OtrosIngresos_TenantId] ON [OtrosIngresos] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    CREATE INDEX [IX_OtrosGastos_TenantId] ON [OtrosGastos] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    CREATE INDEX [IX_LiquidacionesMensuales_TenantId] ON [LiquidacionesMensuales] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    CREATE INDEX [IX_Alumnos_TenantId] ON [Alumnos] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [Alumnos] ADD CONSTRAINT [FK_Alumnos_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [LiquidacionesMensuales] ADD CONSTRAINT [FK_LiquidacionesMensuales_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [OtrosGastos] ADD CONSTRAINT [FK_OtrosGastos_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [OtrosIngresos] ADD CONSTRAINT [FK_OtrosIngresos_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [Profesores] ADD CONSTRAINT [FK_Profesores_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    ALTER TABLE [Usuarios] ADD CONSTRAINT [FK_Usuarios_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    UPDATE Usuarios SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' WHERE TenantId IS NULL
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    UPDATE Alumnos SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' WHERE TenantId IS NULL
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    UPDATE Profesores SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' WHERE TenantId IS NULL
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    UPDATE OtrosIngresos SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' WHERE TenantId IS NULL
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    UPDATE OtrosGastos SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' WHERE TenantId IS NULL
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    UPDATE LiquidacionesMensuales SET TenantId = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890' WHERE TenantId IS NULL
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304072710_AgregarTenantIdATodasLasEntidades'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260304072710_AgregarTenantIdATodasLasEntidades', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306041112_AgregarSedeConfig'
)
BEGIN
    CREATE TABLE [SedeConfigs] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NOT NULL,
        [SedeValor] int NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Activa] bit NOT NULL DEFAULT CAST(1 AS bit),
        [EsDefault] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Orden] int NOT NULL DEFAULT 1,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_SedeConfigs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306041112_AgregarSedeConfig'
)
BEGIN
    CREATE INDEX [IX_SedeConfigs_TenantId] ON [SedeConfigs] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306041112_AgregarSedeConfig'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SedeConfigs_TenantId_SedeValor] ON [SedeConfigs] ([TenantId], [SedeValor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306041112_AgregarSedeConfig'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'TenantId', N'SedeValor', N'Nombre', N'Activa', N'EsDefault', N'Orden') AND [object_id] = OBJECT_ID(N'[SedeConfigs]'))
        SET IDENTITY_INSERT [SedeConfigs] ON;
    EXEC(N'INSERT INTO [SedeConfigs] ([Id], [TenantId], [SedeValor], [Nombre], [Activa], [EsDefault], [Orden])
    VALUES (''9d226435-998e-4ab2-9d35-18c2313f2f08'', ''a1b2c3d4-e5f6-7890-abcd-ef1234567890'', 1, N''Medellín'', CAST(1 AS bit), CAST(1 AS bit), 1),
    (''3d2770cb-2dd1-4fc9-adfd-6436ffec0067'', ''a1b2c3d4-e5f6-7890-abcd-ef1234567890'', 2, N''Manizales'', CAST(1 AS bit), CAST(0 AS bit), 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'TenantId', N'SedeValor', N'Nombre', N'Activa', N'EsDefault', N'Orden') AND [object_id] = OBJECT_ID(N'[SedeConfigs]'))
        SET IDENTITY_INSERT [SedeConfigs] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306041112_AgregarSedeConfig'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260306041112_AgregarSedeConfig', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    DROP INDEX [IX_TiposPaquete_Nombre] ON [TiposPaquete];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    DROP INDEX [IX_TiposClase_Nombre] ON [TiposClase];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    ALTER TABLE [TiposPaquete] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    ALTER TABLE [TiposClase] ADD [TenantId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    EXEC(N'UPDATE [TiposClase] SET [TenantId] = NULL
    WHERE [Id] = ''44444444-4444-4444-4444-444444444444'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    EXEC(N'UPDATE [TiposClase] SET [TenantId] = NULL
    WHERE [Id] = ''55555555-5555-5555-5555-555555555555'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    EXEC(N'UPDATE [TiposClase] SET [TenantId] = NULL
    WHERE [Id] = ''66666666-6666-6666-6666-666666666666'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    EXEC(N'UPDATE [TiposPaquete] SET [TenantId] = NULL
    WHERE [Id] = ''77777777-7777-7777-7777-777777777777'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    EXEC(N'UPDATE [TiposPaquete] SET [TenantId] = NULL
    WHERE [Id] = ''88888888-8888-8888-8888-888888888888'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    EXEC(N'UPDATE [TiposPaquete] SET [TenantId] = NULL
    WHERE [Id] = ''99999999-9999-9999-9999-999999999999'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_TiposPaquete_TenantId_Nombre] ON [TiposPaquete] ([TenantId], [Nombre]) WHERE [TenantId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_TiposClase_TenantId_Nombre] ON [TiposClase] ([TenantId], [Nombre]) WHERE [TenantId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260306053737_AgregarTenantIdATiposClaseYTipoPaquete', N'9.0.9');
END;

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

COMMIT;
GO


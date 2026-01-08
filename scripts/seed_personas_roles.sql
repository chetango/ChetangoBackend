/********************************************************************************************************
 Script: seed_personas_roles.sql
 Objetivo: Crear/actualizar usuarios, roles, profesores y alumnos de referencia solicitados para QA.
          - Jorge Padilla y Juan Herrera: Administradores + Docentes + Alumnos.
          - Santiago Salazar y María Alejandra: Docentes.
          - Jhoan Cadavid, María Isabel y Juan Pablo: Solo alumnos.
 El script es idempotente; elimina los registros previos de esas personas antes de insertarlos.
*********************************************************************************************************/

-- USE [ChetangoDB_QA]; -- << reemplaza por tu base antes de ejecutar

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @TipoDocCC UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @TipoProfesorPrincipal UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @EstadoUsuarioActivo INT = 1;
DECLARE @EstadoAlumnoActivo INT = 1;
DECLARE @RolAdministrador UNIQUEIDENTIFIER = 'cccccccc-cccc-cccc-cccc-cccccccccccc';
DECLARE @RolAlumno UNIQUEIDENTIFIER        = 'dddddddd-dddd-dddd-dddd-dddddddddddd';
DECLARE @RolProfesor UNIQUEIDENTIFIER      = 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee';

DECLARE @Ahora DATETIME2(0) = SYSDATETIME();
DECLARE @Hoy DATE = CAST(@Ahora AS DATE);

DECLARE @Personas TABLE (
    Nombre NVARCHAR(150),
    Documento VARCHAR(20),
    Correo NVARCHAR(150),
    Telefono NVARCHAR(30),
    EsAdmin BIT,
    EsProfesor BIT,
    EsAlumno BIT
);

INSERT INTO @Personas (Nombre, Documento, Correo, Telefono, EsAdmin, EsProfesor, EsAlumno)
VALUES
    ('Jorge Padilla',    '1017141203', 'jorge.padilla@demo.chetango.com',    '+57 3101714120', 1, 1, 1),
    ('Juan Herrera',     '1032010606', 'juan.herrera@demo.chetango.com',     '+57 3103201060', 1, 1, 1),
    ('Santiago Salazar', '1032569651', 'santiago.salazar@demo.chetango.com', '+57 3103259651', 0, 1, 0),
    ('Maria Alejandra',  '1437840858', 'maria.alejandra@demo.chetango.com',  '+57 3143784085', 0, 1, 0),
    ('Jhoan Cadavid',    '1084576038', 'jhoan.cadavid@demo.chetango.com',    '+57 3108457603', 0, 0, 1),
    ('Maria Isabel',     '1784508757', 'maria.isabel@demo.chetango.com',     '+57 3178450875', 0, 0, 1),
    ('Juan Pablo',       '1674650987', 'juan.pablo@demo.chetango.com',       '+57 3167465098', 0, 0, 1);

BEGIN TRAN;
        DECLARE @ProfesorMap TABLE (Documento VARCHAR(20), IdProfesor UNIQUEIDENTIFIER);
        DECLARE @AlumnoMap   TABLE (Documento VARCHAR(20), IdAlumno UNIQUEIDENTIFIER);

    DECLARE @Nombre NVARCHAR(150);
    DECLARE @Documento VARCHAR(20);
    DECLARE @Correo NVARCHAR(150);
    DECLARE @Telefono NVARCHAR(30);
    DECLARE @EsAdmin BIT;
    DECLARE @EsProfesor BIT;
    DECLARE @EsAlumno BIT;

    DECLARE @IdUsuario UNIQUEIDENTIFIER;
    DECLARE @IdProfesor UNIQUEIDENTIFIER;
    DECLARE @IdAlumno UNIQUEIDENTIFIER;
    DECLARE @DiasOffset INT = 0;

    DECLARE personas_cursor CURSOR FAST_FORWARD FOR
        SELECT Nombre, Documento, Correo, Telefono, EsAdmin, EsProfesor, EsAlumno
        FROM @Personas
        ORDER BY Nombre;

    OPEN personas_cursor;
    FETCH NEXT FROM personas_cursor INTO @Nombre, @Documento, @Correo, @Telefono, @EsAdmin, @EsProfesor, @EsAlumno;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @IdUsuario = NULL;
        SET @IdProfesor = NULL;
        SET @IdAlumno = NULL;

        SELECT @IdUsuario = IdUsuario
        FROM Usuarios
        WHERE NumeroDocumento = @Documento;

        IF @IdUsuario IS NULL
        BEGIN
            SELECT @IdUsuario = IdUsuario
            FROM Usuarios
            WHERE NombreUsuario = @Nombre;
        END

        IF @IdUsuario IS NULL
        BEGIN
            SET @IdUsuario = NEWID();
            INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario, FechaCreacion)
            VALUES (@IdUsuario, @Nombre, @TipoDocCC, @Documento, @Correo, @Telefono, @EstadoUsuarioActivo, @Ahora);
        END
        ELSE
        BEGIN
            UPDATE Usuarios
            SET NombreUsuario = @Nombre,
                IdTipoDocumento = @TipoDocCC,
                Correo = @Correo,
                Telefono = @Telefono,
                IdEstadoUsuario = @EstadoUsuarioActivo
            WHERE IdUsuario = @IdUsuario;

            DELETE FROM UsuariosRoles
            WHERE IdUsuario = @IdUsuario
              AND IdRol IN (@RolAdministrador, @RolAlumno, @RolProfesor);
        END

        IF (@EsProfesor = 1)
        BEGIN
            SELECT @IdProfesor = IdProfesor
            FROM Profesores
            WHERE IdUsuario = @IdUsuario;

            IF @IdProfesor IS NULL
            BEGIN
                SET @IdProfesor = NEWID();
                INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor)
                VALUES (@IdProfesor, @IdUsuario, @TipoProfesorPrincipal);
            END
            ELSE
            BEGIN
                UPDATE Profesores
                SET IdTipoProfesor = @TipoProfesorPrincipal
                WHERE IdProfesor = @IdProfesor;
            END

            IF NOT EXISTS (SELECT 1 FROM UsuariosRoles WHERE IdUsuario = @IdUsuario AND IdRol = @RolProfesor)
                INSERT INTO UsuariosRoles (IdUsuario, IdRol)
                VALUES (@IdUsuario, @RolProfesor);

            INSERT INTO @ProfesorMap (Documento, IdProfesor)
            VALUES (@Documento, @IdProfesor);
        END
        ELSE
        BEGIN
            DELETE FROM UsuariosRoles
            WHERE IdUsuario = @IdUsuario AND IdRol = @RolProfesor;
        END

        IF (@EsAlumno = 1)
        BEGIN
            SELECT @IdAlumno = IdAlumno
            FROM Alumnos
            WHERE IdUsuario = @IdUsuario;

            SET @DiasOffset += 5;

            IF @IdAlumno IS NULL
            BEGIN
                SET @IdAlumno = NEWID();
                INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
                VALUES (@IdAlumno, @IdUsuario, DATEADD(day, -@DiasOffset, @Hoy), @EstadoAlumnoActivo);
            END
            ELSE
            BEGIN
                UPDATE Alumnos
                SET IdEstado = @EstadoAlumnoActivo
                WHERE IdAlumno = @IdAlumno;
            END

            IF NOT EXISTS (SELECT 1 FROM UsuariosRoles WHERE IdUsuario = @IdUsuario AND IdRol = @RolAlumno)
                INSERT INTO UsuariosRoles (IdUsuario, IdRol)
                VALUES (@IdUsuario, @RolAlumno);

            INSERT INTO @AlumnoMap (Documento, IdAlumno)
            VALUES (@Documento, @IdAlumno);
        END
        ELSE
        BEGIN
            DELETE FROM UsuariosRoles
            WHERE IdUsuario = @IdUsuario AND IdRol = @RolAlumno;
        END

        IF (@EsAdmin = 1)
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM UsuariosRoles WHERE IdUsuario = @IdUsuario AND IdRol = @RolAdministrador)
                INSERT INTO UsuariosRoles (IdUsuario, IdRol)
                VALUES (@IdUsuario, @RolAdministrador);
        END
        ELSE
        BEGIN
            DELETE FROM UsuariosRoles
            WHERE IdUsuario = @IdUsuario AND IdRol = @RolAdministrador;
        END

        FETCH NEXT FROM personas_cursor INTO @Nombre, @Documento, @Correo, @Telefono, @EsAdmin, @EsProfesor, @EsAlumno;
    END

    CLOSE personas_cursor;
    DEALLOCATE personas_cursor;

    /* Actualizar clases seed para que usen los nuevos profesores */
    DECLARE @ProfesorClase1 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @ProfesorMap WHERE Documento = '1017141203'); -- Jorge Padilla
    DECLARE @ProfesorClase2 UNIQUEIDENTIFIER = (SELECT IdProfesor FROM @ProfesorMap WHERE Documento = '1032569651'); -- Santiago Salazar

    IF @ProfesorClase1 IS NOT NULL
        UPDATE Clases SET IdProfesorPrincipal = @ProfesorClase1 WHERE IdClase = 'c9d5af98-2b1a-4cb4-a1a8-13f3381c93ef';

    IF @ProfesorClase2 IS NOT NULL
        UPDATE Clases SET IdProfesorPrincipal = @ProfesorClase2 WHERE IdClase = 'c6a8a2a0-1b5f-44e1-9bcd-9f1197f4a3ed';
COMMIT;

PRINT 'Usuarios, roles, profesores y alumnos actualizados correctamente.';

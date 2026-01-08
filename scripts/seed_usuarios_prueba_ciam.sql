-- =====================================================================
-- Script de Datos de Prueba para Autenticación con Microsoft Entra CIAM
-- =====================================================================
-- Este script crea 3 usuarios de prueba con sus datos relacionados:
--   1. Chetango (Admin)
--   2. Jorge Padilla (Profesor) 
--   3. Juan David (Alumno)
--
-- IMPORTANTE: 
-- - Estos usuarios YA EXISTEN en Microsoft Entra External ID (CIAM)
-- - Este script solo crea los registros en la BD para vincularlos
-- - Los usuarios pueden iniciar sesión con OAuth 2.0 usando las credenciales
--   documentadas en docs/API-CONTRACT-FRONTEND.md
-- =====================================================================

USE ChetangoDB_Dev;
GO

-- =====================================================================
-- VARIABLES DE IDS (para referencia y consistencia)
-- =====================================================================
DECLARE @IdUsuarioChetango UNIQUEIDENTIFIER = 'b91e51b9-4094-441e-a5b6-062a846b3868';
DECLARE @IdUsuarioJorge UNIQUEIDENTIFIER = '8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB';
DECLARE @IdUsuarioJuanDavid UNIQUEIDENTIFIER = '71462106-9863-4fd0-b13d-9878ed231aa6';

DECLARE @IdProfesorJorge UNIQUEIDENTIFIER = '8f6e460d-328d-4a40-89e3-b8effa76829c';
DECLARE @IdAlumnoJuanDavid UNIQUEIDENTIFIER = '295093d5-b36f-4737-b68a-ab40ca871b2e';

DECLARE @IdClaseJorge UNIQUEIDENTIFIER = '6a50c2cb-461e-4ee1-a50f-03f938bc5b4c';
DECLARE @IdPaqueteJuanDavid UNIQUEIDENTIFIER = 'aabbccdd-1234-5678-90ab-cdef12345678';

-- IDs de tipos (obtener de tablas existentes)
DECLARE @IdTipoDocOID UNIQUEIDENTIFIER;
DECLARE @IdTipoProfesorPrincipal UNIQUEIDENTIFIER;
DECLARE @IdTipoClaseTango UNIQUEIDENTIFIER;
DECLARE @IdTipoPaqueteMensual UNIQUEIDENTIFIER;
DECLARE @IdEstadoActivo INT = 1;
DECLARE @IdEstadoPaqueteActivo INT = 1;
DECLARE @IdEstadoAsistenciaPresente INT = 1;

-- Obtener IDs de tipos desde tablas de catálogo
SELECT @IdTipoDocOID = Id FROM TiposDocumento WHERE Nombre = 'OID';
SELECT @IdTipoProfesorPrincipal = Id FROM TiposProfesor WHERE Nombre = 'Principal';
SELECT @IdTipoClaseTango = Id FROM TiposClase WHERE Nombre = 'Tango';
SELECT @IdTipoPaqueteMensual = Id FROM TiposPaquete WHERE Nombre = 'Mensual';

-- Validar que existen los tipos necesarios
IF @IdTipoDocOID IS NULL OR @IdTipoProfesorPrincipal IS NULL OR @IdTipoClaseTango IS NULL OR @IdTipoPaqueteMensual IS NULL
BEGIN
    RAISERROR('ERROR: Faltan tipos en las tablas de catálogo. Asegúrate de que las migraciones de EF Core se ejecutaron correctamente.', 16, 1);
    RETURN;
END

BEGIN TRANSACTION;

BEGIN TRY
    -- =====================================================================
    -- 1. USUARIO ADMIN: Chetango
    -- =====================================================================
    -- Correo: Chetango@chetangoprueba.onmicrosoft.com
    -- Contraseña: Chet4ngo20#
    -- Rol en Entra: admin
    
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioChetango)
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
        VALUES (
            @IdUsuarioChetango,
            'Chetango Admin',
            @IdTipoDocOID,
            'admin-oid-001',
            'Chetango@chetangoprueba.onmicrosoft.com',
            '0000000000',
            @IdEstadoActivo
        );
        PRINT '✓ Usuario Admin Chetango creado';
    END
    ELSE
    BEGIN
        PRINT '→ Usuario Admin Chetango ya existe, actualizando datos...';
        UPDATE Usuarios 
        SET NombreUsuario = 'Chetango Admin',
            Correo = 'Chetango@chetangoprueba.onmicrosoft.com'
        WHERE IdUsuario = @IdUsuarioChetango;
    END

    -- =====================================================================
    -- 2. USUARIO PROFESOR: Jorge Padilla
    -- =====================================================================
    -- Correo: Jorgepadilla@chetangoprueba.onmicrosoft.com
    -- Contraseña: Jorge2026
    -- Rol en Entra: profesor
    
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioJorge)
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
        VALUES (
            @IdUsuarioJorge,
            'Jorge Padilla',
            @IdTipoDocOID,
            'jorge-oid-002',
            'Jorgepadilla@chetangoprueba.onmicrosoft.com',
            '1111111111',
            @IdEstadoActivo
        );
        PRINT '✓ Usuario Profesor Jorge creado';
    END
    ELSE
    BEGIN
        PRINT '→ Usuario Profesor Jorge ya existe, actualizando datos...';
        UPDATE Usuarios 
        SET NombreUsuario = 'Jorge Padilla',
            Correo = 'Jorgepadilla@chetangoprueba.onmicrosoft.com'
        WHERE IdUsuario = @IdUsuarioJorge;
    END

    -- Crear relación Profesor para Jorge
    IF NOT EXISTS (SELECT 1 FROM Profesores WHERE IdProfesor = @IdProfesorJorge)
    BEGIN
        INSERT INTO Profesores (IdProfesor, IdTipoProfesor, IdUsuario)
        VALUES (
            @IdProfesorJorge,
            @IdTipoProfesorPrincipal,
            @IdUsuarioJorge
        );
        PRINT '✓ Relación Profesor Jorge creada';
    END
    ELSE
    BEGIN
        PRINT '→ Relación Profesor Jorge ya existe';
    END

    -- =====================================================================
    -- 3. USUARIO ALUMNO: Juan David
    -- =====================================================================
    -- Correo: JuanDavid@chetangoprueba.onmicrosoft.com
    -- Contraseña: Juaj0rge20#
    -- Rol en Entra: alumno
    
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioJuanDavid)
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
        VALUES (
            @IdUsuarioJuanDavid,
            'Juan David',
            @IdTipoDocOID,
            'juandavid-oid-003',
            'JuanDavid@chetangoprueba.onmicrosoft.com',
            '2222222222',
            @IdEstadoActivo
        );
        PRINT '✓ Usuario Alumno Juan David creado';
    END
    ELSE
    BEGIN
        PRINT '→ Usuario Alumno Juan David ya existe, actualizando datos...';
        UPDATE Usuarios 
        SET NombreUsuario = 'Juan David',
            Correo = 'JuanDavid@chetangoprueba.onmicrosoft.com'
        WHERE IdUsuario = @IdUsuarioJuanDavid;
    END

    -- Crear relación Alumno para Juan David
    IF NOT EXISTS (SELECT 1 FROM Alumnos WHERE IdAlumno = @IdAlumnoJuanDavid)
    BEGIN
        INSERT INTO Alumnos (IdAlumno, IdUsuario)
        VALUES (
            @IdAlumnoJuanDavid,
            @IdUsuarioJuanDavid
        );
        PRINT '✓ Relación Alumno Juan David creada';
    END
    ELSE
    BEGIN
        PRINT '→ Relación Alumno Juan David ya existe';
    END

    -- =====================================================================
    -- 4. DATOS PARA PRUEBAS: Clase de Jorge
    -- =====================================================================
    
    IF NOT EXISTS (SELECT 1 FROM Clases WHERE IdClase = @IdClaseJorge)
    BEGIN
        INSERT INTO Clases (IdClase, Fecha, HoraInicio, HoraFin, IdTipoClase, IdProfesorPrincipal)
        VALUES (
            @IdClaseJorge,
            CAST(GETDATE() AS DATE), -- Fecha de hoy
            '19:00:00',
            '20:30:00',
            @IdTipoClaseTango,
            @IdProfesorJorge
        );
        PRINT '✓ Clase de Jorge creada para pruebas';
    END
    ELSE
    BEGIN
        PRINT '→ Clase de Jorge ya existe';
    END

    -- =====================================================================
    -- 5. PAQUETE PARA JUAN DAVID (necesario para asistencias)
    -- =====================================================================
    
    IF NOT EXISTS (SELECT 1 FROM Paquetes WHERE IdPaquete = @IdPaqueteJuanDavid)
    BEGIN
        INSERT INTO Paquetes (
            IdPaquete, 
            ClasesDisponibles, 
            ClasesUsadas, 
            FechaActivacion, 
            FechaVencimiento, 
            IdAlumno, 
            IdEstado, 
            IdPago, 
            IdTipoPaquete, 
            ValorPaquete
        )
        VALUES (
            @IdPaqueteJuanDavid,
            8,  -- 8 clases disponibles
            0,  -- 0 usadas
            CAST(GETDATE() AS DATE),
            DATEADD(DAY, 30, CAST(GETDATE() AS DATE)), -- Válido por 30 días
            @IdAlumnoJuanDavid,
            @IdEstadoPaqueteActivo,
            NULL, -- Sin pago asociado (datos de prueba)
            @IdTipoPaqueteMensual,
            150000 -- Valor del paquete
        );
        PRINT '✓ Paquete de Juan David creado';
    END
    ELSE
    BEGIN
        PRINT '→ Paquete de Juan David ya existe';
    END

    -- =====================================================================
    -- 6. ASISTENCIA DE PRUEBA: Juan David en clase de Jorge
    -- =====================================================================
    
    DECLARE @IdAsistenciaPrueba UNIQUEIDENTIFIER = NEWID();
    
    IF NOT EXISTS (SELECT 1 FROM Asistencias WHERE IdClase = @IdClaseJorge AND IdAlumno = @IdAlumnoJuanDavid)
    BEGIN
        INSERT INTO Asistencias (
            IdAsistencia,
            IdClase,
            IdAlumno,
            IdEstado,
            IdPaqueteUsado,
            Observacion
        )
        VALUES (
            @IdAsistenciaPrueba,
            @IdClaseJorge,
            @IdAlumnoJuanDavid,
            @IdEstadoAsistenciaPresente,
            @IdPaqueteJuanDavid,
            'Asistencia de prueba creada por script'
        );
        PRINT '✓ Asistencia de prueba Juan David → Clase Jorge creada';
    END
    ELSE
    BEGIN
        PRINT '→ Asistencia de prueba ya existe';
    END

    COMMIT TRANSACTION;
    
    PRINT '';
    PRINT '=====================================================================';
    PRINT '✓✓✓ DATOS DE PRUEBA CREADOS EXITOSAMENTE ✓✓✓';
    PRINT '=====================================================================';
    PRINT '';
    PRINT 'Usuarios creados:';
    PRINT '  1. Admin:    Chetango@chetangoprueba.onmicrosoft.com / Chet4ngo20#';
    PRINT '  2. Profesor: Jorgepadilla@chetangoprueba.onmicrosoft.com / Jorge2026';
    PRINT '  3. Alumno:   JuanDavid@chetangoprueba.onmicrosoft.com / Juaj0rge20#';
    PRINT '';
    PRINT 'Datos adicionales:';
    PRINT '  - 1 Clase de Jorge (hoy 19:00-20:30)';
    PRINT '  - 1 Paquete activo para Juan David (8 clases, 30 días)';
    PRINT '  - 1 Asistencia de prueba (Juan David en clase de Jorge)';
    PRINT '';
    PRINT 'Para más información, consulta: docs/API-CONTRACT-FRONTEND.md';
    PRINT '=====================================================================';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    PRINT '';
    PRINT '=====================================================================';
    PRINT '✗✗✗ ERROR AL CREAR DATOS DE PRUEBA ✗✗✗';
    PRINT '=====================================================================';
    PRINT 'Mensaje: ' + @ErrorMessage;
    PRINT 'Severidad: ' + CAST(@ErrorSeverity AS VARCHAR);
    PRINT 'Estado: ' + CAST(@ErrorState AS VARCHAR);
    PRINT '=====================================================================';
    
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO

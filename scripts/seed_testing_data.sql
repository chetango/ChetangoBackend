-- ============================================
-- DATOS ADICIONALES PARA TESTING AUTOMATIZADO
-- Ejecutar DESPUÉS de seed_usuarios_prueba_ciam.sql
-- ============================================

USE ChetangoDB_Dev;
GO

DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
DECLARE @Ayer DATE = DATEADD(DAY, -1, @Hoy);
DECLARE @Manana DATE = DATEADD(DAY, 1, @Hoy);

-- IDs existentes
DECLARE @IdUsuarioJorge UNIQUEIDENTIFIER = '8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB';
DECLARE @IdProfesorJorge UNIQUEIDENTIFIER = '8f6e460d-328d-4a40-89e3-b8effa76829c';
DECLARE @IdAlumnoJuan UNIQUEIDENTIFIER = '295093d5-b36f-4737-b68a-ab40ca871b2e';

-- Nuevos IDs para testing
DECLARE @IdUsuarioAna UNIQUEIDENTIFIER = 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA';
DECLARE @IdProfesorAna UNIQUEIDENTIFIER = 'AAAAAAAA-BBBB-BBBB-BBBB-BBBBBBBBBBBB';
DECLARE @IdUsuarioSanti UNIQUEIDENTIFIER = 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC';
DECLARE @IdProfesorSanti UNIQUEIDENTIFIER = 'CCCCCCCC-DDDD-DDDD-DDDD-DDDDDDDDDDDD';
DECLARE @IdUsuarioMaria UNIQUEIDENTIFIER = 'EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE';
DECLARE @IdProfesorMaria UNIQUEIDENTIFIER = 'EEEEEEEE-FFFF-FFFF-FFFF-FFFFFFFFFFFF';
DECLARE @IdUsuarioAlumno2 UNIQUEIDENTIFIER = 'AAAABBBB-CCCC-DDDD-EEEE-111111111111';
DECLARE @IdAlumno2 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

-- Tipos y estados
DECLARE @IdTipoDocCC UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @IdTipoProfesorPrincipal UNIQUEIDENTIFIER;
DECLARE @IdTipoClaseTango UNIQUEIDENTIFIER;
DECLARE @IdEstadoActivo INT = 1;
DECLARE @IdEstadoPaqueteVencido INT = 2;
DECLARE @IdEstadoPaqueteCongelado INT = 3;

SELECT @IdTipoProfesorPrincipal = Id FROM TiposProfesor WHERE Nombre = 'Principal';
SELECT @IdTipoClaseTango = Id FROM TiposClase WHERE Nombre = 'Tango';

BEGIN TRANSACTION;

BEGIN TRY

    -- ============================================
    -- 1. ACTUALIZAR TARIFAS DE JORGE
    -- ============================================
    UPDATE Profesores
    SET TarifaActual = 40000
    WHERE IdProfesor = @IdProfesorJorge;

    PRINT '✓ Tarifa de Jorge actualizada (40k)';

    -- ============================================
    -- 2. CREAR PROFESORES ADICIONALES
    -- ============================================

    -- Ana Zoraida Gómez
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioAna)
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
        VALUES (@IdUsuarioAna, N'Ana Zoraida Gómez', @IdTipoDocCC, N'test-ana-001', N'ana.zoraida@chetango.test', N'3001111111', @IdEstadoActivo);
        
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, TarifaActual)
        VALUES (@IdProfesorAna, @IdUsuarioAna, @IdTipoProfesorPrincipal, 40000);
        
        PRINT '✓ Profesora Ana creada (Tarifa: 40k)';
    END
    ELSE
    BEGIN
        UPDATE Profesores 
        SET TarifaActual = 40000
        WHERE IdProfesor = @IdProfesorAna;
        PRINT '→ Profesora Ana ya existe, tarifa actualizada';
    END

    -- Santiago (Santi)
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioSanti)
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
        VALUES (@IdUsuarioSanti, N'Santiago Pérez', @IdTipoDocCC, N'test-santi-001', N'santiago.perez@chetango.test', N'3002222222', @IdEstadoActivo);
        
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, TarifaActual)
        VALUES (@IdProfesorSanti, @IdUsuarioSanti, @IdTipoProfesorPrincipal, 30000);
        
        PRINT '✓ Profesor Santi creado (Tarifa: 30k)';
    END
    ELSE
    BEGIN
        UPDATE Profesores 
        SET TarifaActual = 30000
        WHERE IdProfesor = @IdProfesorSanti;
        PRINT '→ Profesor Santi ya existe, tarifa actualizada';
    END

    -- María Alejandra
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioMaria)
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
        VALUES (@IdUsuarioMaria, N'María Alejandra López', @IdTipoDocCC, N'test-maria-001', N'maria.lopez@chetango.test', N'3003333333', @IdEstadoActivo);
        
        INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, TarifaActual)
        VALUES (@IdProfesorMaria, @IdUsuarioMaria, @IdTipoProfesorPrincipal, 30000);
        
        PRINT '✓ Profesora María creada (Tarifa: 30k)';
    END
    ELSE
    BEGIN
        UPDATE Profesores 
        SET TarifaActual = 30000
        WHERE IdProfesor = @IdProfesorMaria;
        PRINT '→ Profesora María ya existe, tarifa actualizada';
    END

    -- ============================================
    -- 3. CREAR SEGUNDO ALUMNO (para pruebas de ownership)
    -- ============================================
    IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioAlumno2)
    BEGIN
        INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
        VALUES (@IdUsuarioAlumno2, N'María Rodríguez', @IdTipoDocCC, N'test-alumno2-001', N'maria.rodriguez@chetango.test', N'3004444444', @IdEstadoActivo);
        
        INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstado)
        VALUES (@IdAlumno2, @IdUsuarioAlumno2, GETDATE(), @IdEstadoActivo);
        
        PRINT '✓ Alumna María (alumno2) creada';
    END
    ELSE
    BEGIN
        PRINT '→ Alumna María (alumno2) ya existe';
    END

    -- ============================================
    -- 4. CATÁLOGO: RolesEnClase
    -- ============================================
    DECLARE @IdRolPrincipal UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
    DECLARE @IdRolMonitor UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';

    IF NOT EXISTS (SELECT 1 FROM RolesEnClase WHERE Nombre = 'Principal')
    BEGIN
        INSERT INTO RolesEnClase (Id, Nombre) VALUES (@IdRolPrincipal, 'Principal');
        PRINT '✓ Rol Principal creado';
    END
    ELSE
    BEGIN
        SELECT @IdRolPrincipal = Id FROM RolesEnClase WHERE Nombre = 'Principal';
        PRINT '→ Rol Principal ya existe';
    END

    IF NOT EXISTS (SELECT 1 FROM RolesEnClase WHERE Nombre = 'Monitor')
    BEGIN
        INSERT INTO RolesEnClase (Id, Nombre) VALUES (@IdRolMonitor, 'Monitor');
        PRINT '✓ Rol Monitor creado';
    END
    ELSE
    BEGIN
        SELECT @IdRolMonitor = Id FROM RolesEnClase WHERE Nombre = 'Monitor';
        PRINT '→ Rol Monitor ya existe';
    END

    -- ============================================
    -- 5. PAQUETES ESPECIALES PARA PRUEBAS
    -- ============================================
    DECLARE @IdTipoPaquete8 UNIQUEIDENTIFIER;
    DECLARE @IdTipoPaquete12 UNIQUEIDENTIFIER;
    
    SELECT TOP 1 @IdTipoPaquete8 = Id FROM TiposPaquete WHERE Nombre LIKE '%8%' AND Activo = 1;
    SELECT TOP 1 @IdTipoPaquete12 = Id FROM TiposPaquete WHERE Nombre LIKE '%12%' AND Activo = 1;

    -- Paquete VENCIDO (para CP-ASI-004)
    DECLARE @IdPaqueteVencido UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555555';
    IF NOT EXISTS (SELECT 1 FROM Paquetes WHERE IdPaquete = @IdPaqueteVencido)
    BEGIN
        INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, 
                              FechaActivacion, FechaVencimiento, IdEstado, ValorPaquete, 
                              FechaCreacion, UsuarioCreacion)
        VALUES (@IdPaqueteVencido, @IdAlumnoJuan, @IdTipoPaquete8, 8, 2, 
                DATEADD(MONTH, -2, @Hoy), DATEADD(DAY, -5, @Hoy), @IdEstadoPaqueteVencido, 150000,
                GETDATE(), 'TestingSetup');
        PRINT '✓ Paquete vencido creado';
    END

    -- Paquete con 1 clase restante (para CP-PAQ-003)
    DECLARE @IdPaqueteCasiAgotado UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666666';
    IF NOT EXISTS (SELECT 1 FROM Paquetes WHERE IdPaquete = @IdPaqueteCasiAgotado)
    BEGIN
        INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, 
                              FechaActivacion, FechaVencimiento, IdEstado, ValorPaquete, 
                              FechaCreacion, UsuarioCreacion)
        VALUES (@IdPaqueteCasiAgotado, @IdAlumnoJuan, @IdTipoPaquete8, 8, 7, 
                @Hoy, DATEADD(DAY, 30, @Hoy), @IdEstadoActivo, 150000,
                GETDATE(), 'TestingSetup');
        PRINT '✓ Paquete casi agotado creado (7/8 usado)';
    END

    -- Paquete para alumno2 (ownership tests)
    DECLARE @IdPaqueteAlumno2 UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777777';
    IF NOT EXISTS (SELECT 1 FROM Paquetes WHERE IdPaquete = @IdPaqueteAlumno2)
    BEGIN
        INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, 
                              FechaActivacion, FechaVencimiento, IdEstado, ValorPaquete, 
                              FechaCreacion, UsuarioCreacion)
        VALUES (@IdPaqueteAlumno2, @IdAlumno2, @IdTipoPaquete12, 12, 0, 
                @Hoy, DATEADD(DAY, 45, @Hoy), @IdEstadoActivo, 200000,
                GETDATE(), 'TestingSetup');
        PRINT '✓ Paquete para alumno2 creado';
    END

    -- ============================================
    -- 6. CLASES PARA PRUEBAS
    -- ============================================

    -- Clase de AYER (completada, para asistencias)
    DECLARE @IdClaseAyer UNIQUEIDENTIFIER = '88888888-8888-8888-8888-888888888888';
    IF NOT EXISTS (SELECT 1 FROM Clases WHERE IdClase = @IdClaseAyer)
    BEGIN
        INSERT INTO Clases (IdClase, Fecha, IdTipoClase, HoraInicio, HoraFin, CupoMaximo, 
                            IdProfesorPrincipal, Estado)
        VALUES (@IdClaseAyer, @Ayer, @IdTipoClaseTango, '18:00:00', '19:30:00', 20,
                @IdProfesorJorge, 'Completada');
        PRINT '✓ Clase de ayer creada';
    END

    -- Clase FUTURA (para validar que no se puede registrar asistencia)
    DECLARE @IdClaseFutura UNIQUEIDENTIFIER = '99999999-9999-9999-9999-999999999999';
    IF NOT EXISTS (SELECT 1 FROM Clases WHERE IdClase = @IdClaseFutura)
    BEGIN
        INSERT INTO Clases (IdClase, Fecha, IdTipoClase, HoraInicio, HoraFin, CupoMaximo, 
                            IdProfesorPrincipal, Estado)
        VALUES (@IdClaseFutura, @Manana, @IdTipoClaseTango, '19:00:00', '20:00:00', 20,
                @IdProfesorJorge, 'Programada');
        PRINT '✓ Clase futura creada';
    END

    -- ============================================
    -- COMMIT
    -- ============================================
    COMMIT TRANSACTION;

    PRINT '';
    PRINT '================================================';
    PRINT '✅ DATOS DE TESTING CREADOS EXITOSAMENTE';
    PRINT '================================================';
    PRINT '';
    PRINT 'Profesores con tarifas:';
    PRINT '';
    PRINT 'Paquetes de prueba:';
    SELECT p.IdPaquete, u.NombreUsuario, p.ClasesDisponibles, p.ClasesUsadas, 
           CASE p.IdEstado WHEN 1 THEN 'Activo' WHEN 2 THEN 'Vencido' WHEN 3 THEN 'Congelado' WHEN 4 THEN 'Agotado' ELSE 'Otro' END AS Estado
    FROM Paquetes p
    INNER JOIN Alumnos a ON p.IdAlumno = a.IdAlumno
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario;

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'ERROR: ' + ERROR_MESSAGE();
    THROW;
END CATCH

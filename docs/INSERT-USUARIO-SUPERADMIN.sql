-- ================================================
-- SCRIPT: Crear Usuario SuperAdmin en BD
-- ================================================
-- IMPORTANTE: Reemplaza 'TU-EMAIL@ejemplo.com' con tu correo real de Azure

-- 1. Verificar si el usuario ya existe
SELECT * FROM Usuarios WHERE Correo = 'TU-EMAIL@ejemplo.com';

-- 2. Si NO existe, insertarlo
INSERT INTO Usuarios (
    IdUsuario,
    NombreUsuario,
    IdTipoDocumento,
    NumeroDocumento,
    Correo,
    Telefono,
    IdEstadoUsuario,
    Sede,
    FechaCreacion
)
VALUES (
    NEWID(),                                    -- IdUsuario (GUID)
    'SuperAdmin Chetango',                      -- NombreUsuario
    1,                                          -- IdTipoDocumento (1 = CC generalmente)
    '0000000000',                               -- NumeroDocumento
    'TU-EMAIL@ejemplo.com',                     -- Correo (CAMBIA ESTO)
    '+57 300 000 0000',                        -- Telefono
    1,                                          -- IdEstadoUsuario (1 = Activo)
    0,                                          -- Sede (0 = Medellín)
    GETDATE()                                   -- FechaCreacion
);

-- 3. Verificar que se creó correctamente
SELECT * FROM Usuarios WHERE Correo = 'TU-EMAIL@ejemplo.com';

-- ================================================
-- NOTAS:
-- - Este usuario tiene acceso por sus roles en Azure (Admin + SuperAdmin)
-- - No necesita registro en tablas Alumno o Profesor
-- - La autenticación es via Azure AD, no por password
-- ================================================

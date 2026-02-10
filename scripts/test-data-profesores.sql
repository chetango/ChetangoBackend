-- ============================================
-- TEST DATA: PROFESORES - CHETANGO
-- Datos de prueba para módulo de profesores
-- ============================================

-- Verificar profesores existentes
SELECT * FROM Profesores;

-- Insertar profesores adicionales para pruebas
-- Profesor 2: María González (Principal)
INSERT INTO Profesores (IdProfesor, IdUsuario, Nombre, Apellido, Correo, Telefono, IdTipoProfesor, Estado, FechaCreacion)
VALUES (
    NEWID(),
    NEWID(), -- IdUsuario temporal (no tiene cuenta Entra ID aún)
    N'María',
    N'González',
    N'maria.gonzalez@chetango.com',
    N'3001234567',
    'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF', -- Principal
    1, -- Activo
    GETDATE()
);

-- Profesor 3: Carlos Ramírez (Principal)
INSERT INTO Profesores (IdProfesor, IdUsuario, Nombre, Apellido, Correo, Telefono, IdTipoProfesor, Estado, FechaCreacion)
VALUES (
    NEWID(),
    NEWID(),
    N'Carlos',
    N'Ramírez',
    N'carlos.ramirez@chetango.com',
    N'3009876543',
    'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF', -- Principal
    1,
    GETDATE()
);

-- Profesor 4: Laura Martínez (Monitor)
INSERT INTO Profesores (IdProfesor, IdUsuario, Nombre, Apellido, Correo, Telefono, IdTipoProfesor, Estado, FechaCreacion)
VALUES (
    NEWID(),
    NEWID(),
    N'Laura',
    N'Martínez',
    N'laura.martinez@chetango.com',
    N'3157894561',
    '12121212-1212-1212-1212-121212121212', -- Monitor
    1,
    GETDATE()
);

-- Profesor 5: Diego Torres (Monitor)
INSERT INTO Profesores (IdProfesor, IdUsuario, Nombre, Apellido, Correo, Telefono, IdTipoProfesor, Estado, FechaCreacion)
VALUES (
    NEWID(),
    NEWID(),
    'Diego',
    'Torres',
    'diego.torres@chetango.com',
    '3201478523',
    '12121212-1212-1212-1212-121212121212', -- Monitor
    1,
    GETDATE()
);

-- Verificar inserción
SELECT 
    p.IdProfesor,
    p.Nombre + ' ' + p.Apellido AS NombreCompleto,
    p.Correo,
    p.Telefono,
    tp.Nombre AS TipoProfesor,
    CASE WHEN p.Estado = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
FROM Profesores p
LEFT JOIN TiposProfesor tp ON p.IdTipoProfesor = tp.IdTipoProfesor
ORDER BY p.FechaCreacion DESC;

-- Verificar tipos de profesor disponibles
SELECT * FROM TiposProfesor;

-- Verificar tarifas por tipo
SELECT 
    tp.Nombre AS TipoProfesor,
    t.ValorHora,
    r.Nombre AS RolEnClase
FROM TiposProfesor tp
LEFT JOIN TarifasProfesor t ON tp.IdTipoProfesor = t.IdTipoProfesor
LEFT JOIN RolesEnClase r ON t.IdRolEnClase = r.IdRolEnClase
ORDER BY tp.Nombre, r.Nombre;

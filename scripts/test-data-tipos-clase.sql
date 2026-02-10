-- ============================================
-- TEST DATA: TIPOS DE CLASE - CHETANGO
-- Datos de prueba para tipos de clase
-- ============================================

-- Verificar tipos de clase existentes
SELECT * FROM TiposClase;

-- Si no hay tipos de clase, crearlos
-- Tipo 1: Principiante
IF NOT EXISTS (SELECT 1 FROM TiposClase WHERE Nombre = 'Principiante')
BEGIN
    INSERT INTO TiposClase (IdTipoClase, Nombre, Descripcion, Duracion, CupoMaximo, Estado)
    VALUES (
        NEWID(),
        'Principiante',
        'Clase para alumnos que están iniciando en el tango',
        60, -- minutos
        20,
        1
    );
END

-- Tipo 2: Intermedio
IF NOT EXISTS (SELECT 1 FROM TiposClase WHERE Nombre = 'Intermedio')
BEGIN
    INSERT INTO TiposClase (IdTipoClase, Nombre, Descripcion, Duracion, CupoMaximo, Estado)
    VALUES (
        NEWID(),
        'Intermedio',
        'Clase para alumnos con conocimientos básicos',
        90,
        15,
        1
    );
END

-- Tipo 3: Avanzado
IF NOT EXISTS (SELECT 1 FROM TiposClase WHERE Nombre = 'Avanzado')
BEGIN
    INSERT INTO TiposClase (IdTipoClase, Nombre, Descripcion, Duracion, CupoMaximo, Estado)
    VALUES (
        NEWID(),
        'Avanzado',
        'Clase para alumnos con técnica avanzada',
        90,
        12,
        1
    );
END

-- Tipo 4: Técnica
IF NOT EXISTS (SELECT 1 FROM TiposClase WHERE Nombre = 'Técnica')
BEGIN
    INSERT INTO TiposClase (IdTipoClase, Nombre, Descripcion, Duracion, CupoMaximo, Estado)
    VALUES (
        NEWID(),
        'Técnica',
        'Clase enfocada en técnica y postura',
        60,
        10,
        1
    );
END

-- Tipo 5: Práctica Libre
IF NOT EXISTS (SELECT 1 FROM TiposClase WHERE Nombre = 'Práctica Libre')
BEGIN
    INSERT INTO TiposClase (IdTipoClase, Nombre, Descripcion, Duracion, CupoMaximo, Estado)
    VALUES (
        NEWID(),
        'Práctica Libre',
        'Sesión de práctica supervisada',
        120,
        25,
        1
    );
END

-- Verificar tipos creados
SELECT 
    IdTipoClase,
    Nombre,
    Descripcion,
    Duracion,
    CupoMaximo,
    CASE WHEN Estado = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado
FROM TiposClase
ORDER BY Nombre;

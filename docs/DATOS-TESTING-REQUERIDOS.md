# 🗄️ Análisis de Datos para Testing Automatizado

> **Fecha:** 05 de Febrero de 2026  
> **Objetivo:** Identificar datos existentes vs datos requeridos para ejecutar los 64 casos de prueba

---

## 📊 Estado Actual de Datos en BD

### ✅ Datos Existentes (seed_usuarios_prueba_ciam.sql)

#### Usuarios
| Rol | Nombre | Email | ID Usuario | ID Relacionado |
|-----|--------|-------|------------|----------------|
| Admin | Chetango Admin | Chetango@chetangoprueba.onmicrosoft.com | b91e51b9-4094-441e-a5b6-062a846b3868 | - |
| Profesor | Jorge Padilla | Jorgepadilla@chetangoprueba.onmicrosoft.com | 8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB | IdProfesor: 8f6e460d-328d-4a40-89e3-b8effa76829c |
| Alumno | Juan David | JuanDavid@chetangoprueba.onmicrosoft.com | 6f84b6a7-b0ae-456b-a455-eabae44d2930 | IdAlumno: 295093d5-b36f-4737-b68a-ab40ca871b2e |

#### Datos Relacionados Existentes
- ✅ 1 clase de Jorge (ID: 6a50c2cb-461e-4ee1-a50f-03f938bc5b4c)
- ✅ 1 paquete de Juan David (ID: aabbccdd-1234-5678-90ab-cdef12345678)
- ✅ 1 asistencia de ejemplo
- ✅ Catálogos de tipos (TipoClase, TipoPaquete, etc.)
- ✅ Métodos de pago (seed_metodos_pago.sql)

---

## 🎯 Datos Requeridos por Casos de Prueba

### MÓDULO: ASISTENCIAS (10 casos)

#### Datos Necesarios:

**✅ YA EXISTE:**
- Usuario Admin
- Usuario Profesor (Jorge)
- Alumno con paquete activo (Juan David)
- Clase completada
- TipoAsistencia: Normal, Cortesía, Recuperación, Clase de Prueba

**❌ FALTA:**
1. **Alumno SIN paquete activo** (para CP-ASI-003)
2. **Paquete VENCIDO** (FechaVencimiento < hoy) (para CP-ASI-004)
3. **Paquete CONGELADO** (Estado = 3) (para CP-ASI-005)
4. **Clase del día de AYER** (para pruebas de asistencia)
5. **Clase FUTURA** (para validar que no se puede registrar asistencia)

**📝 SE CONSTRUYE EN PRUEBAS:**
- Asistencias duplicadas (se crea la primera y luego se intenta crear otra)
- Paquete que se agota (se descuentan clases hasta agotar)

---

### MÓDULO: CLASES (13 casos)

#### Datos Necesarios:

**✅ YA EXISTE:**
- Admin
- Profesor Jorge con IdUsuario vinculado
- Tipos de clase (Tango, Vals, Milonga, etc.)

**❌ FALTA:**
1. **Segundo Profesor** (Ana) con tarifas configuradas (para CP-NOM-009, CP-NOM-010)
   - Ana.TarifaPrincipal = 40000
   - Ana.TarifaMonitor = 15000
   
2. **Tercer Profesor** (Santi) con tarifas diferentes (para CP-NOM-013)
   - Santi.TarifaPrincipal = 30000
   - Santi.TarifaMonitor = 12000

3. **Cuarto Profesor** (María) (para CP-NOM-011)
   - María.TarifaMonitor = 12000

4. **Tarifas en Profesores Existentes:**
   - Jorge.TarifaPrincipal = 40000
   - Jorge.TarifaMonitor = 15000

**📝 SE CONSTRUYE EN PRUEBAS:**
- Clases con conflictos de horario (se crea una y luego otra que solapa)
- Clases con múltiples profesores (se asignan al crear)
- Clases canceladas
- Clases con asistencias (para validar que no se puede cancelar)

---

### MÓDULO: PAQUETES (11 casos)

#### Datos Necesarios:

**✅ YA EXISTE:**
- Admin
- Alumno Juan David
- Tipos de paquete (4, 8, 12 clases)
- 1 paquete activo

**❌ FALTA:**
1. **Paquete con ClasesRestantes = 1** (para probar agotamiento en CP-PAQ-003)
2. **Paquete con Estado = Agotado** (ClasesUsadas >= ClasesDisponibles)
3. **Paquete con FechaVencimiento = hoy** (para ver cambio automático a Vencido)
4. **Segundo alumno** para pruebas de ownership (CP-PAQ-011)

**📝 SE CONSTRUYE EN PRUEBAS:**
- Paquetes congelados (se congela un activo)
- Paquetes con congelaciones solapadas (se intenta crear conflicto)
- Descuento de clases (se registra asistencia)

---

### MÓDULO: PAGOS (9 casos)

#### Datos Necesarios:

**✅ YA EXISTE:**
- Admin
- Alumno Juan David
- Métodos de pago (Efectivo, Transferencia, etc.)
- Tipos de paquete

**❌ FALTA:**
1. **Paquetes huérfanos** (sin IdPago) para vincular en CP-PAG-006
2. **Segundo alumno** con paquetes para validar ownership

**📝 SE CONSTRUYE EN PRUEBAS:**
- Pagos con múltiples paquetes (se crean al registrar pago)
- Validaciones de suma de valores
- Vinculación de paquetes existentes

---

### MÓDULO: NÓMINA (14 casos) ⭐ MÁS COMPLEJO

#### Datos Necesarios:

**✅ YA EXISTE:**
- Admin
- Profesor Jorge (necesita tarifas configuradas)

**❌ FALTA (CRÍTICO):**

1. **Tarifas Individuales en Tabla Profesores:**
   ```sql
   UPDATE Profesores 
   SET TarifaPrincipal = 40000, TarifaMonitor = 15000 
   WHERE IdProfesor = '8f6e460d-328d-4a40-89e3-b8effa76829c'; -- Jorge
   ```

2. **Profesores Adicionales con Tarifas:**
   - Ana Zoraida: TarifaPrincipal = 40000, TarifaMonitor = 15000
   - Santi: TarifaPrincipal = 30000, TarifaMonitor = 12000
   - María: TarifaPrincipal = 30000, TarifaMonitor = 12000

3. **Catálogo RolEnClase:**
   ```sql
   INSERT INTO RolesEnClase (Id, Nombre) VALUES 
     (NEWID(), 'Principal'),
     (NEWID(), 'Monitor');
   ```

4. **Clases Completadas sin ClaseProfesor:**
   - Para probar que se generan registros al completar

5. **Clases con múltiples profesores:**
   - 1 clase con Jorge (Principal) + Ana (Monitor)
   - 1 clase con Jorge + Ana ambos Principales
   - 1 clase con Jorge (Principal) + Santi (Monitor) + María (Monitor)

6. **ClasesProfesores en diferentes estados:**
   - Pendiente (recién completada)
   - Aprobado (para liquidar)
   - Liquidado (ya en liquidación)

**📝 SE CONSTRUYE EN PRUEBAS:**
- Completar clases genera ClaseProfesor
- Aprobar pagos (cambia estado)
- Liquidaciones mensuales
- Ajustes adicionales en pagos

---

### MÓDULO: REPORTES (6 casos)

#### Datos Necesarios:

**✅ YA EXISTE:**
- Script completo: seed_reportes_datos_prueba.sql

**❌ VALIDAR:**
- Que el script esté actualizado con estructura actual

---

## 📋 Script de Datos Faltantes para Testing

### Archivo: `seed_testing_data.sql`

```sql
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
DECLARE @IdUsuarioAna UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorAna UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioSanti UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorSanti UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioMaria UNIQUEIDENTIFIER = NEWID();
DECLARE @IdProfesorMaria UNIQUEIDENTIFIER = NEWID();
DECLARE @IdUsuarioAlumno2 UNIQUEIDENTIFIER = NEWID();
DECLARE @IdAlumno2 UNIQUEIDENTIFIER = NEWID();

-- Tipos y estados
DECLARE @IdTipoDocOID UNIQUEIDENTIFIER;
DECLARE @IdTipoProfesorPrincipal UNIQUEIDENTIFIER;
DECLARE @IdTipoClaseTango UNIQUEIDENTIFIER;
DECLARE @IdEstadoActivo INT = 1;
DECLARE @IdEstadoPaqueteVencido INT = 2;
DECLARE @IdEstadoPaqueteCongelado INT = 3;

SELECT @IdTipoDocOID = Id FROM TiposDocumento WHERE Nombre = 'OID';
SELECT @IdTipoProfesorPrincipal = Id FROM TiposProfesor WHERE Nombre = 'Principal';
SELECT @IdTipoClaseTango = Id FROM TiposClase WHERE Nombre = 'Tango';

BEGIN TRANSACTION;

-- ============================================
-- 1. ACTUALIZAR TARIFAS DE JORGE
-- ============================================
UPDATE Profesores
SET TarifaPrincipal = 40000,
    TarifaMonitor = 15000
WHERE IdProfesor = @IdProfesorJorge;

PRINT '✓ Tarifas de Jorge actualizadas';

-- ============================================
-- 2. CREAR PROFESORES ADICIONALES
-- ============================================

-- Ana Zoraida Gómez (Principal/Monitor)
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioAna)
BEGIN
    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
    VALUES (@IdUsuarioAna, 'Ana Zoraida Gómez', @IdTipoDocOID, 'test-ana-001', 'ana.zoraida@chetango.test', '3001111111', @IdEstadoActivo);
    
    INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, TarifaPrincipal, TarifaMonitor, Activo, FechaCreacion, UsuarioCreacion)
    VALUES (@IdProfesorAna, @IdUsuarioAna, @IdTipoProfesorPrincipal, 40000, 15000, 1, GETDATE(), 'TestingSetup');
    
    PRINT '✓ Profesora Ana creada';
END

-- Santiago (Santi)
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioSanti)
BEGIN
    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
    VALUES (@IdUsuarioSanti, 'Santiago Pérez', @IdTipoDocOID, 'test-santi-001', 'santiago.perez@chetango.test', '3002222222', @IdEstadoActivo);
    
    INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, TarifaPrincipal, TarifaMonitor, Activo, FechaCreacion, UsuarioCreacion)
    VALUES (@IdProfesorSanti, @IdUsuarioSanti, @IdTipoProfesorPrincipal, 30000, 12000, 1, GETDATE(), 'TestingSetup');
    
    PRINT '✓ Profesor Santi creado';
END

-- María Alejandra
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioMaria)
BEGIN
    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
    VALUES (@IdUsuarioMaria, 'María Alejandra López', @IdTipoDocOID, 'test-maria-001', 'maria.lopez@chetango.test', '3003333333', @IdEstadoActivo);
    
    INSERT INTO Profesores (IdProfesor, IdUsuario, IdTipoProfesor, TarifaPrincipal, TarifaMonitor, Activo, FechaCreacion, UsuarioCreacion)
    VALUES (@IdProfesorMaria, @IdUsuarioMaria, @IdTipoProfesorPrincipal, 30000, 12000, 1, GETDATE(), 'TestingSetup');
    
    PRINT '✓ Profesora María creada';
END

-- ============================================
-- 3. CREAR SEGUNDO ALUMNO (para pruebas de ownership)
-- ============================================
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE IdUsuario = @IdUsuarioAlumno2)
BEGIN
    INSERT INTO Usuarios (IdUsuario, NombreUsuario, IdTipoDocumento, NumeroDocumento, Correo, Telefono, IdEstadoUsuario)
    VALUES (@IdUsuarioAlumno2, 'María Rodríguez', @IdTipoDocOID, 'test-alumno2-001', 'maria.rodriguez@chetango.test', '3004444444', @IdEstadoActivo);
    
    INSERT INTO Alumnos (IdAlumno, IdUsuario, FechaInscripcion, IdEstadoAlumno)
    VALUES (@IdAlumno2, @IdUsuarioAlumno2, GETDATE(), @IdEstadoActivo);
    
    PRINT '✓ Alumna María (alumno2) creada';
END

-- ============================================
-- 4. CATÁLOGO: RolesEnClase
-- ============================================
DECLARE @IdRolPrincipal UNIQUEIDENTIFIER = NEWID();
DECLARE @IdRolMonitor UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT 1 FROM RolesEnClase WHERE Nombre = 'Principal')
BEGIN
    INSERT INTO RolesEnClase (Id, Nombre) VALUES (@IdRolPrincipal, 'Principal');
    PRINT '✓ Rol Principal creado';
END
ELSE
BEGIN
    SELECT @IdRolPrincipal = Id FROM RolesEnClase WHERE Nombre = 'Principal';
END

IF NOT EXISTS (SELECT 1 FROM RolesEnClase WHERE Nombre = 'Monitor')
BEGIN
    INSERT INTO RolesEnClase (Id, Nombre) VALUES (@IdRolMonitor, 'Monitor');
    PRINT '✓ Rol Monitor creado';
END
ELSE
BEGIN
    SELECT @IdRolMonitor = Id FROM RolesEnClase WHERE Nombre = 'Monitor';
END

-- ============================================
-- 5. PAQUETES ESPECIALES PARA PRUEBAS
-- ============================================

-- Paquete VENCIDO (para CP-ASI-004)
DECLARE @IdPaqueteVencido UNIQUEIDENTIFIER = NEWID();
INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, 
                      FechaActivacion, FechaVencimiento, IdEstado, ValorPaquete, 
                      FechaCreacion, UsuarioCreacion)
SELECT @IdPaqueteVencido, @IdAlumnoJuan, Id, 8, 2, 
       DATEADD(MONTH, -2, @Hoy), DATEADD(DAY, -5, @Hoy), @IdEstadoPaqueteVencido, 150000,
       GETDATE(), 'TestingSetup'
FROM TiposPaquete WHERE Nombre LIKE '%8%' AND Activo = 1;

PRINT '✓ Paquete vencido creado';

-- Paquete con 1 clase restante (para CP-PAQ-003)
DECLARE @IdPaqueteCasiAgotado UNIQUEIDENTIFIER = NEWID();
INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, 
                      FechaActivacion, FechaVencimiento, IdEstado, ValorPaquete, 
                      FechaCreacion, UsuarioCreacion)
SELECT @IdPaqueteCasiAgotado, @IdAlumnoJuan, Id, 8, 7, 
       @Hoy, DATEADD(DAY, 30, @Hoy), @IdEstadoActivo, 150000,
       GETDATE(), 'TestingSetup'
FROM TiposPaquete WHERE Nombre LIKE '%8%' AND Activo = 1;

PRINT '✓ Paquete casi agotado creado (7/8 usado)';

-- Paquete para alumno2 (ownership tests)
DECLARE @IdPaqueteAlumno2 UNIQUEIDENTIFIER = NEWID();
INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, 
                      FechaActivacion, FechaVencimiento, IdEstado, ValorPaquete, 
                      FechaCreacion, UsuarioCreacion)
SELECT @IdPaqueteAlumno2, @IdAlumno2, Id, 12, 0, 
       @Hoy, DATEADD(DAY, 45, @Hoy), @IdEstadoActivo, 200000,
       GETDATE(), 'TestingSetup'
FROM TiposPaquete WHERE Nombre LIKE '%12%' AND Activo = 1;

PRINT '✓ Paquete para alumno2 creado';

-- ============================================
-- 6. CLASES PARA PRUEBAS
-- ============================================

-- Clase de AYER (completada, para asistencias)
DECLARE @IdClaseAyer UNIQUEIDENTIFIER = NEWID();
INSERT INTO Clases (IdClase, Fecha, IdTipoClase, HoraInicio, HoraFin, CupoMaximo, 
                    IdProfesorPrincipal, Estado)
VALUES (@IdClaseAyer, @Ayer, @IdTipoClaseTango, '18:00:00', '19:30:00', 20,
        @IdProfesorJorge, 'Completada');

PRINT '✓ Clase de ayer creada';

-- Clase FUTURA (para validar que no se puede registrar asistencia)
DECLARE @IdClaseFutura UNIQUEIDENTIFIER = NEWID();
INSERT INTO Clases (IdClase, Fecha, IdTipoClase, HoraInicio, HoraFin, CupoMaximo, 
                    IdProfesorPrincipal, Estado)
VALUES (@IdClaseFutura, @Manana, @IdTipoClaseTango, '19:00:00', '20:00:00', 20,
        @IdProfesorJorge, 'Programada');

PRINT '✓ Clase futura creada';

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
SELECT NombreCompleto, TarifaPrincipal, TarifaMonitor FROM Profesores;
PRINT '';
PRINT 'Paquetes de prueba:';
SELECT p.IdPaquete, u.NombreUsuario, p.ClasesDisponibles, p.ClasesUsadas, 
       CASE p.IdEstado WHEN 1 THEN 'Activo' WHEN 2 THEN 'Vencido' ELSE 'Otro' END AS Estado
FROM Paquetes p
INNER JOIN Alumnos a ON p.IdAlumno = a.IdAlumno
INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario;
```

---

## 🎯 Plan de Ejecución

### Orden de Ejecución de Scripts:

1. ✅ **seed_usuarios_prueba_ciam.sql** (EJECUTADO)
   - Crea Admin, Jorge, Juan David
   - Crea 1 clase, 1 paquete, 1 asistencia

2. ✅ **seed_metodos_pago.sql** (EJECUTADO)
   - Crea catálogo de métodos de pago

3. ✅ **seed_paquetes_catalogos.sql** (EJECUTADO)
   - Crea tipos de paquete

4. ⚠️ **seed_testing_data.sql** (NUEVO - EJECUTAR)
   - Crea profesores adicionales con tarifas
   - Crea alumno adicional
   - Crea paquetes especiales (vencido, casi agotado)
   - Crea clases de ayer y futura
   - Actualiza tarifas de Jorge

5. 📝 **Datos dinámicos** (SE CREAN EN PRUEBAS)
   - Clases con conflictos
   - Asistencias duplicadas
   - Paquetes congelados
   - Pagos con múltiples paquetes
   - ClasesProfesores al completar clases

---

## ✅ Checklist Pre-Testing

Antes de ejecutar pruebas automatizadas, verificar:

- [ ] Script seed_usuarios_prueba_ciam.sql ejecutado
- [ ] Script seed_metodos_pago.sql ejecutado
- [ ] Script seed_paquetes_catalogos.sql ejecutado
- [ ] Script seed_testing_data.sql ejecutado (NUEVO)
- [ ] Backend corriendo en https://localhost:7194
- [ ] Frontend corriendo en http://localhost:5173
- [ ] Verificar que profesores tienen tarifas: `SELECT NombreCompleto, TarifaPrincipal, TarifaMonitor FROM Profesores`
- [ ] Verificar que existen roles: `SELECT * FROM RolesEnClase`

---

## 📊 Resumen

| Categoría | Existente | Faltante | Se Construye |
|-----------|-----------|----------|--------------|
| Usuarios base | 3 | 4 profesores | - |
| Alumnos | 1 | 1 adicional | - |
| Paquetes | 1 activo | Vencido, Casi agotado | Congelados, Agotados |
| Clases | 1 completada | Ayer, Futura | Con conflictos, Múltiples profes |
| Asistencias | 1 ejemplo | - | Todas las demás |
| Tarifas | - | Configurar todas | - |
| ClaseProfesor | - | - | Al completar clases |
| Pagos | - | - | Al registrar pagos |

**Total de scripts requeridos:** 1 nuevo (seed_testing_data.sql)

---

**Próximo paso:** Ejecutar `seed_testing_data.sql` y luego proceder con la configuración de Playwright.

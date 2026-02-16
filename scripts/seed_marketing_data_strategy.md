# ESTRATEGIA DE POBLAMIENTO - BASE DE DATOS MARKETING

## OBJETIVO
Poblar `ChetangoDB_Dev` con datos realistas para crear un video de marketing que muestre:
- Dashboard con KPIs visuales y gráficas
- Flujos completos de alumnos, profesores, pagos, clases, asistencias
- Reportes con información significativa
- Liquidaciones de profesores con transacciones
- Sistema de referidos activo
- Eventos y notificaciones

---

## DATOS A GENERAR

### 1. USUARIOS Y PERFILES
- **50 Alumnos** (distribuidos en el tiempo)
  - 35 activos, 10 inactivos, 5 suspendidos/retirados
  - Fechas de inscripción: últimos 18 meses
  - 10-15 con códigos de referidos activos
  
- **8 Profesores**
  - 6 principales
  - 2 asistentes
  - Biografías y especialidades realistas

- **1 Usuario Administrador**
  - Para auditorías y aprobaciones

### 2. PAQUETES Y PAGOS
- **Total: 80-100 paquetes** distribuidos así:
  - 40% paquetes 8 clases ($150,000)
  - 30% paquetes 12 clases ($210,000)
  - 20% paquetes 4 clases ($80,000)
  - 10% paquetes 20 clases ($320,000)
  
- **Estados de paquetes**:
  - 30 activos (con clases por usar)
  - 40 completados (todas las clases usadas)
  - 10 vencidos
  - 5 congelados (con fechas de congelación)

- **Pagos asociados**: 95% de paquetes con pago, 5% cortesía
  - Métodos: 40% Transferencia, 30% Efectivo, 20% Tarjeta, 10% Nequi
  - Estados: 90% Verificados, 8% Pendientes, 2% Rechazados

### 3. CLASES Y PROGRAMACIÓN
- **Total: 150-200 clases** en el rango de fechas:
  - Últimos 6 meses: 130 clases históricas
  - Próximos 14 días: 20 clases futuras
  
- **Distribución semanal**:
  - Lunes-Viernes: 3 clases/día (18:00, 19:30, 21:00)
  - Sábados: 4 clases (10:00, 15:00, 17:00, 19:00)
  - Domingos: 2 clases (15:00, 17:00)
  
- **Tipos de clase**:
  - 70% Tango Salón
  - 20% Tango Escenario
  - 10% Privadas y Workshops
  
- **Cupos**:
  - Salón: 25 cupos
  - Escenario: 18 cupos
  - Privadas: 2-4 cupos

### 4. ASIGNACIÓN DE PROFESORES A CLASES
- **ClaseProfesor**: 1-2 profesores por clase
  - Principal (80% de clases)
  - Co-Instructor (15% de clases)
  - Asistente (5% de clases)
  
- **Tarifas**:
  - Principal: $80,000/clase
  - Co-Instructor: $50,000/clase
  - Asistente: $30,000/clase

### 5. ASISTENCIAS
- **Total: 2,000-2,500 asistencias**
  - Tasa de ocupación: 60-80% del cupo
  - 90% con paquete (tipo Normal)
  - 8% cortesía
  - 2% prueba
  
- **Patrones de asistencia**:
  - Alumnos regulares: 2-3 clases/semana
  - Alumnos ocasionales: 1 clase/semana
  - Alumnos inactivos: 0 clases últimos 2 meses

### 6. LIQUIDACIONES DE PROFESORES
- **6 meses completos liquidados** (Enero-Junio 2026)
  - Estados: 4 meses Pagados, 1 mes Cerrado, 1 mes EnProceso
  
- **Datos por liquidación**:
  - 15-25 clases por profesor/mes
  - 20-40 horas totales
  - $1,200,000 - $3,000,000 por profesor/mes
  - 5-10% tienen ajustes adicionales (bonos/descuentos)

### 7. EVENTOS
- **12 eventos** en los últimos 12 meses
  - Milongas mensuales
  - Workshops especiales (trimestral)
  - 30% destacados
  - Audiencias: 50% Todos, 30% Alumnos, 20% Profesores

### 8. SISTEMA DE REFERIDOS
- **15 códigos de referidos activos**
  - 5 con 3+ usos
  - 5 con 1-2 usos
  - 5 sin usar todavía
  
- **Beneficios**:
  - Referidor: "10% descuento en próximo paquete"
  - Nuevo alumno: "Primera clase gratis"

### 9. SOLICITUDES
- **20 solicitudes de clases privadas**
  - Estados: 40% Aprobadas, 30% Pendientes, 20% Agendadas, 10% Rechazadas
  
- **15 solicitudes de renovación**
  - Estados: 50% Completadas, 30% Aprobadas, 20% Pendientes

---

## DISTRIBUCIÓN TEMPORAL

### Eje de Tiempo: Agosto 2024 - Febrero 2026 (18 meses)

#### Q3 2024 (Ago-Oct): Lanzamiento
- 10 alumnos iniciales
- 4 profesores
- 20 clases/mes
- 15 paquetes vendidos

#### Q4 2024 (Nov-Ene): Crecimiento
- +15 alumnos nuevos (total 25)
- +2 profesores (total 6)
- 30 clases/mes
- 25 paquetes vendidos

#### Q1 2025 (Feb-Abr): Expansión
- +15 alumnos (total 40)
- +2 profesores (total 8)
- 35 clases/mes
- 30 paquetes vendidos

#### Q2-Q4 2025 (May-Ene 2026): Consolidación
- +10 alumnos (total 50)
- 40 clases/mes estables
- 35-40 paquetes vendidos/trimestre
- Sistema de referidos activo

#### Febrero 2026: Estado Actual
- 50 alumnos (35 activos)
- 8 profesores
- 40-45 clases/mes
- Operación consolidada

---

## ORDEN DE EJECUCIÓN DE SCRIPTS

### FASE 1: Catálogos y Configuración Base
```sql
-- 1.1 Estados y Tipos Base
INSERT INTO TipoDocumento ...
INSERT INTO EstadoUsuario ...
INSERT INTO EstadoAlumno ...
INSERT INTO EstadoPago ...
INSERT INTO EstadoPaquete ...
INSERT INTO EstadoAsistencia ...
INSERT INTO EstadoNotificacion ...

-- 1.2 Catálogos de Negocio
INSERT INTO MetodoPago ...
INSERT INTO TipoClase ...
INSERT INTO TipoProfesor ...
INSERT INTO RolEnClase ...
INSERT INTO TipoAsistencia ...

-- 1.3 Configuración de Productos
INSERT INTO TipoPaquete ...

-- 1.4 Tarifas de Profesores
INSERT INTO TarifaProfesor ...
```

### FASE 2: Usuarios y Perfiles
```sql
-- 2.1 Usuario Administrador
INSERT INTO Usuario (administrador) ...

-- 2.2 Usuarios Alumnos (50)
INSERT INTO Usuario (alumnos) ...
INSERT INTO Alumno ...

-- 2.3 Usuarios Profesores (8)
INSERT INTO Usuario (profesores) ...
INSERT INTO Profesor ...
```

### FASE 3: Transacciones Financieras
```sql
-- 3.1 Pagos (95 pagos para 100 paquetes)
INSERT INTO Pago ...

-- 3.2 Paquetes (100 paquetes)
INSERT INTO Paquete ...

-- 3.3 Congelaciones (5 paquetes)
INSERT INTO CongelacionPaquete ...
```

### FASE 4: Programación y Ejecución
```sql
-- 4.1 Clases (150-200 clases en 18 meses)
INSERT INTO Clase ...

-- 4.2 Asignación de Profesores
INSERT INTO ClaseProfesor ...

-- 4.3 Monitores (10-15 monitores asignados)
INSERT INTO MonitorClase ...

-- 4.4 Asistencias (2000-2500 registros)
INSERT INTO Asistencia ...
```

### FASE 5: Gestión y Liquidaciones
```sql
-- 5.1 Liquidaciones Mensuales (6 meses x 8 profesores = 48)
INSERT INTO LiquidacionMensual ...

-- 5.2 Actualizar ClaseProfesor.EstadoPago según liquidaciones
UPDATE ClaseProfesor SET EstadoPago='Liquidado' ...
UPDATE ClaseProfesor SET EstadoPago='Pagado' ...
```

### FASE 6: Sistema de Referidos
```sql
-- 6.1 Códigos de Referidos (15 códigos)
INSERT INTO CodigoReferido ...

-- 6.2 Usos de Códigos (25-30 usos)
INSERT INTO UsoCodigoReferido ...
```

### FASE 7: Eventos y Comunicación
```sql
-- 7.1 Eventos (12 eventos)
INSERT INTO Evento ...

-- 7.2 Notificaciones (50-100 notificaciones)
INSERT INTO Notificacion ...
```

### FASE 8: Solicitudes
```sql
-- 8.1 Solicitudes de Clases Privadas (20)
INSERT INTO SolicitudClasePrivada ...

-- 8.2 Solicitudes de Renovación (15)
INSERT INTO SolicitudRenovacionPaquete ...
```

---

## ALGORITMOS DE GENERACIÓN

### Nombres y Datos Ficticios
```
Alumnos:
- Nombres: María Fernández, Carlos Rodríguez, Ana López, Juan Martínez, etc.
- Correos: maria.fernandez@example.com, carlos.rodriguez@example.com
- Teléfonos: +57 300 XXX XXXX
- Documentos: 1000000001 - 1000000050

Profesores:
- Nombres: Diego Velázquez, Lucía Morales, Fernando Castro, etc.
- Especialidades: ["Tango Salón", "Técnica Femenina"], ["Tango Escenario", "Coreografía"]
```

### Distribución de Clases
```python
# Pseudo-código
def generar_clases(fecha_inicio, fecha_fin):
    fecha_actual = fecha_inicio
    while fecha_actual <= fecha_fin:
        dia_semana = fecha_actual.weekday()
        
        if dia_semana < 5:  # Lunes-Viernes
            horarios = ["18:00", "19:30", "21:00"]
        elif dia_semana == 5:  # Sábado
            horarios = ["10:00", "15:00", "17:00", "19:00"]
        else:  # Domingo
            horarios = ["15:00", "17:00"]
        
        for hora in horarios:
            clase = crear_clase(fecha_actual, hora, tipo_aleatorio(), profesor_aleatorio())
            clases.append(clase)
        
        fecha_actual += 1 día
```

### Asignación de Asistencias
```python
# Pseudo-código
def generar_asistencias(clase):
    cupo = clase.cupo_maximo
    tasa_ocupacion = random(0.6, 0.8)
    num_asistencias = int(cupo * tasa_ocupacion)
    
    alumnos_activos = obtener_alumnos_activos_en_fecha(clase.fecha)
    alumnos_seleccionados = random_sample(alumnos_activos, num_asistencias)
    
    for alumno in alumnos_seleccionados:
        paquete = obtener_paquete_activo(alumno, clase.fecha)
        tipo_asistencia = determinar_tipo(paquete)
        
        asistencia = crear_asistencia(clase, alumno, paquete, tipo_asistencia)
        asistencias.append(asistencia)
        
        if tipo_asistencia.descontar_clase:
            paquete.clases_usadas += 1
```

### Cálculo de Liquidaciones
```python
# Pseudo-código
def calcular_liquidacion(profesor, mes, año):
    clases_profesor = obtener_clases_profesor(profesor, mes, año, estado="Completada")
    
    total_clases = len(clases_profesor)
    total_horas = sum([c.duracion for c in clases_profesor])
    total_base = sum([cp.tarifa_programada for cp in clases_profesor])
    total_adicionales = sum([cp.valor_adicional for cp in clases_profesor])
    total_pagar = total_base + total_adicionales
    
    liquidacion = crear_liquidacion(
        profesor, mes, año,
        total_clases, total_horas,
        total_base, total_adicionales, total_pagar
    )
    
    return liquidacion
```

---

## DATOS IMPORTANTES PARA GRÁFICAS

### Dashboard Admin - KPIs
- **Asistencias Hoy**: 15-25 (para mostrar actividad)
- **Ingresos del Mes**: $8,000,000 - $12,000,000
- **Egresos del Mes**: $4,000,000 - $6,000,000
- **Ganancia Neta**: $3,500,000 - $6,500,000
- **Clases Próximos 7 Días**: 20-25 clases
- **Paquetes por Vencer**: 8-12 paquetes

### Gráfica de Ingresos (últimos 6 meses)
```
Agosto 2025:   $6,500,000
Septiembre:    $7,200,000
Octubre:       $8,100,000
Noviembre:     $9,500,000
Diciembre:     $11,000,000
Enero 2026:    $10,200,000
Febrero 2026:  $9,800,000 (hasta el 13)
```

### Gráfica de Asistencias
```
Agosto:    480 asistencias
Septiembre: 520
Octubre:   580
Noviembre: 640
Diciembre: 720
Enero:     680
Febrero:   320 (hasta el 13)
```

### Métodos de Pago (distribución)
- Transferencia: 40%
- Efectivo: 30%
- Tarjeta: 20%
- Nequi/Daviplata: 10%

---

## VALIDACIONES POST-GENERACIÓN

### Verificar Integridad
```sql
-- 1. Todos los paquetes tienen alumno
SELECT COUNT(*) FROM Paquete WHERE IdAlumno IS NULL;  -- Debe ser 0

-- 2. Asistencias respetan cupo máximo
SELECT c.IdClase, COUNT(a.IdAsistencia) AS Total, c.CupoMaximo
FROM Clase c
LEFT JOIN Asistencia a ON c.IdClase = a.IdClase
GROUP BY c.IdClase, c.CupoMaximo
HAVING COUNT(a.IdAsistencia) > c.CupoMaximo;  -- Debe ser 0 filas

-- 3. ClasesUsadas <= ClasesDisponibles
SELECT * FROM Paquete WHERE ClasesUsadas > ClasesDisponibles;  -- Debe ser 0

-- 4. No hay asistencias duplicadas (mismo alumno, misma clase)
SELECT IdClase, IdAlumno, COUNT(*)
FROM Asistencia
GROUP BY IdClase, IdAlumno
HAVING COUNT(*) > 1;  -- Debe ser 0

-- 5. Liquidaciones tienen clases asociadas
SELECT l.IdLiquidacion, l.TotalClases, COUNT(cp.IdClaseProfesor) AS ClasesReales
FROM LiquidacionMensual l
LEFT JOIN ClaseProfesor cp ON cp.IdProfesor = l.IdProfesor 
    AND MONTH(cp.FechaCreacion) = l.Mes 
    AND YEAR(cp.FechaCreacion) = l.Año
GROUP BY l.IdLiquidacion, l.TotalClases
HAVING l.TotalClases != COUNT(cp.IdClaseProfesor);  -- Debe coincidir
```

### Verificar Distribución
```sql
-- Alumnos por estado
SELECT IdEstado, COUNT(*) FROM Alumno GROUP BY IdEstado;

-- Paquetes por estado
SELECT IdEstado, COUNT(*) FROM Paquete GROUP BY IdEstado;

-- Clases por mes
SELECT YEAR(Fecha) AS Año, MONTH(Fecha) AS Mes, COUNT(*) AS Total
FROM Clase
GROUP BY YEAR(Fecha), MONTH(Fecha)
ORDER BY Año, Mes;

-- Ingresos por mes
SELECT YEAR(FechaPago) AS Año, MONTH(FechaPago) AS Mes, SUM(MontoTotal) AS Total
FROM Pago
WHERE IdEstadoPago = (SELECT IdEstadoPago FROM EstadoPago WHERE Nombre = 'Verificado')
GROUP BY YEAR(FechaPago), MONTH(FechaPago)
ORDER BY Año, Mes;
```

---

## RESUMEN EJECUTIVO

### Total de Registros a Insertar
- Catálogos: ~60 registros
- Usuarios: 59 (1 admin + 50 alumnos + 8 profesores)
- Paquetes: 100
- Pagos: 95
- Clases: 180
- ClaseProfesor: 250
- Asistencias: 2,300
- Liquidaciones: 48
- Eventos: 12
- Códigos Referidos: 15
- Usos Referidos: 30
- Solicitudes: 35
- Notificaciones: 80

**TOTAL**: ~3,570 registros

### Tiempo Estimado de Ejecución
- Generación de scripts: 2-3 horas
- Ejecución en BD: 5-10 minutos
- Validación: 30 minutos

### Archivos a Generar
1. `01_catalogos_base.sql`
2. `02_usuarios_y_perfiles.sql`
3. `03_transacciones_financieras.sql`
4. `04_programacion_clases.sql`
5. `05_asistencias.sql`
6. `06_liquidaciones.sql`
7. `07_sistema_referidos.sql`
8. `08_eventos_y_comunicacion.sql`
9. `09_solicitudes.sql`
10. `00_ejecutar_todo.sql` (maestro que llama a los demás)
11. `99_validaciones.sql` (queries de verificación)

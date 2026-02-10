# 🧪 Plan de Pruebas Automatizadas - Sistema Chetango

> **Fecha de Creación:** 05 de Febrero de 2026  
> **Versión:** 1.0  
> **Stack de Testing Recomendado:** Playwright (E2E) + xUnit (Backend)

---

## 📋 Tabla de Contenidos

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Alcance y Objetivos](#alcance-y-objetivos)
3. [Módulos del Sistema](#módulos-del-sistema)
4. [Reglas de Negocio Identificadas](#reglas-de-negocio-identificadas)
5. [Casos de Prueba por Módulo](#casos-de-prueba-por-módulo)
6. [Priorización de Pruebas](#priorización-de-pruebas)
7. [Estrategia de Implementación](#estrategia-de-implementación)
8. [Datos de Prueba](#datos-de-prueba)

---

## 📊 Resumen Ejecutivo

> **⚠️ ACTUALIZACIÓN:** Documento actualizado para reflejar sistema de tarifas individuales por profesor y roles flexibles (Principal/Monitor configurables por clase)

### Estado Actual del Sistema
- ✅ **Autenticación:** Completo (OAuth 2.0 con Microsoft Entra CIAM)
- ✅ **Asistencias:** Completo con catálogo TipoAsistencia
- ✅ **Clases:** Completo con validación de conflictos y múltiples profesores
- ✅ **Paquetes:** Completo con congelación y descuento automático
- ✅ **Pagos:** Completo con verificación y paquetes múltiples
- ✅ **Reportes:** Completo con exportación Excel/PDF/CSV
- ✅ **Nómina:** Completo con tarifas individuales por profesor y roles flexibles
- ⚠️ **Alumnos/Profesores:** Básico (solo consultas)

### Módulos Funcionales a Probar
1. **Asistencias** (MVP - Alta prioridad)
2. **Clases** (MVP - Alta prioridad)
3. **Paquetes** (MVP - Alta prioridad)
4. **Pagos** (MVP - Alta prioridad)
5. **Nómina Profesores** (Alta prioridad - con tarifas configurables)
6. **Reportes** (Media prioridad)

**NOTA:** Autenticación/Seguridad ya está probada según documentación, por lo que nos enfocaremos en funcionalidades de negocio.

---

## 🎯 Alcance y Objetivos

### Objetivos de Testing

1. **Validar Reglas de Negocio Críticas**
   - Descuento correcto de paquetes
   - Cálculo de pagos a profesores
   - Validación de estados (Activo, Vencido, Congelado, etc.)
   - Ownership y permisos

2. **Validar Flujos End-to-End**
   - Flujo completo de alumno: Pago → Paquete → Asistencia
   - Flujo de profesor: Crear clase → Completar → Aprobar pago → Liquidar
   - Flujo de reportes: Generar y exportar

3. **Prevenir Regresiones**
   - Garantizar que cambios futuros no rompan funcionalidad existente
   - Detectar efectos secundarios en módulos integrados

### Fuera de Alcance
- ❌ Autenticación/OAuth (ya probado)
- ❌ Infraestructura de base de datos
- ❌ Performance/Load testing (fase posterior)

---

## 🧩 Módulos del Sistema

### 1. Módulo de Asistencias ✅
**Responsabilidad:** Registrar y gestionar asistencia de alumnos a clases

**Entidades:**
- `Asistencia`
- `TipoAsistencia` (catálogo)
- `EstadoAsistencia` (catálogo)

**Reglas de Negocio Críticas:**
- R1: No se puede registrar asistencia a clase futura
- R2: No puede haber asistencia duplicada (mismo alumno, misma clase)
- R3: Tipo "Normal" requiere paquete activo y descuenta clase
- R4: Tipo "Cortesía" NO requiere paquete y NO descuenta clase
- R5: Tipo "Clase de Prueba" NO requiere paquete y NO descuenta clase
- R6: Tipo "Recuperación" requiere paquete pero NO descuenta clase
- R7: Solo se descuenta si estado = Presente y TipoAsistencia.DescontarClase = true
- R8: Profesor solo puede registrar asistencias en SUS clases

---

### 2. Módulo de Clases ✅
**Responsabilidad:** Gestionar programación de clases

**Entidades:**
- `Clase`
- `TipoClase` (catálogo)
- `ClaseProfesor` (relación con profesores y pagos)

**Reglas de Negocio Críticas:**
- R9: No puede haber conflicto de horario (mismo profesor, horarios solapados)
- R10: Fecha y hora deben ser futuras al crear
- R11: HoraFin > HoraInicio
- R12: Profesor solo puede crear/editar SUS propias clases
- R13: Admin puede gestionar clases de cualquier profesor
- R14: No se puede cancelar clase pasada
- R15: No se puede cancelar clase con asistencias registradas
- R16: Al completar clase, se generan registros en ClaseProfesor con cálculo de pago

---

### 3. Módulo de Paquetes ✅
**Responsabilidad:** Gestionar paquetes de clases de alumnos

**Entidades:**
- `Paquete`
- `TipoPaquete` (catálogo)
- `EstadoPaquete` (catálogo)
- `CongelacionPaquete`

**Reglas de Negocio Críticas:**
- R17: Estado Activo: ClasesUsadas < ClasesDisponibles AND FechaVencimiento >= hoy
- R18: Estado Vencido: FechaVencimiento < hoy
- R19: Estado Agotado: ClasesUsadas >= ClasesDisponibles
- R20: Estado Congelado: Pausado manualmente
- R21: Al descontar clase: ClasesUsadas++ y recalcular estado
- R22: No se puede descontar clase de paquete Vencido/Congelado/Agotado
- R23: Congelación: FechaInicio < FechaFin
- R24: No puede haber congelaciones solapadas
- R25: Solo se pueden congelar paquetes Activos
- R26: FechaVencimiento se extiende según días congelados
- R27: Alumno solo ve SUS propios paquetes

---

### 4. Módulo de Pagos ✅
**Responsabilidad:** Registrar pagos y generar paquetes asociados

**Entidades:**
- `Pago`
- `MetodoPago` (catálogo)
- `EstadoPago` (catálogo)

**Reglas de Negocio Críticas:**
- R28: MontoTotal > 0
- R29: Debe haber al menos 1 paquete nuevo o vincular paquetes existentes
- R30: Suma de valorPaquete de paquetes <= MontoTotal
- R31: Si no se especifica valorPaquete: MontoTotal / cantidad de paquetes
- R32: Al crear pago, se crean paquetes con Estado = Activo
- R33: FechaActivacion = FechaPago
- R34: FechaVencimiento = FechaPago + diasVigencia
- R35: ClasesUsadas inicial = 0
- R36: Estado inicial del pago = "Pendiente Verificación"
- R37: Paquetes existentes vinculados deben pertenecer al alumno
- R38: Paquetes existentes no deben tener pago previo (IdPago = null)
- R39: Alumno solo ve SUS propios pagos

---

### 5. Módulo de Nómina ⚠️
**Responsabilidad:** Gestionar pagos a profesores por clases impartidas

**Entidades:**
- `ClaseProfesor` (vincula profesor, clase, rol y pago)
- `LiquidacionMensual`
- `Profesor` (con tarifas individuales configurables)
- `RolEnClase` (catálogo: Principal, Monitor)

**Reglas de Negocio Críticas:**
- R40: Al completar clase, se genera ClaseProfesor por CADA profesor asignado (principal o monitor)
- R41: TotalPago = TarifaProgramada + ValorAdicional
- R42: TarifaProgramada = TarifaProfesor × DuraciónHoras (tarifa según rol)
- R42a: **NUEVA:** Cada profesor tiene tarifas individuales configurables desde vista de usuarios
- R42b: **NUEVA:** Profesor.TarifaPrincipal se usa cuando IdRolEnClase = Principal
- R42c: **NUEVA:** Profesor.TarifaMonitor se usa cuando IdRolEnClase = Monitor
- R42d: **NUEVA:** Mismo profesor puede ser Principal en una clase y Monitor en otra
- R42e: **NUEVA:** Una clase puede tener múltiples profesores principales
- R42f: **NUEVA:** Una clase puede tener múltiples monitores
- R43: EstadoPago inicial = "Pendiente"
- R44: Solo se puede aprobar pago en estado "Pendiente"
- R45: Al aprobar, estado cambia a "Aprobado" y se registra FechaAprobacion
- R46: Solo se pueden liquidar pagos en estado "Aprobado"
- R47: Liquidación agrupa pagos aprobados de un mes específico
- R48: Al liquidar mes, estado cambia a "Liquidado"
- R49: Al registrar pago físico, estado cambia a "Pagado"
- R50: Profesor solo ve SUS propias clases y pagos

---

### 6. Módulo de Reportes ✅
**Responsabilidad:** Generar reportes y estadísticas

**Reglas de Negocio Críticas:**
- R51: Rangos de fechas máximo 1 año
- R52: Fechas futuras no permitidas
- R53: FechaDesde <= FechaHasta
- R54: Dashboard cacheado por 5 minutos
- R55: Profesor solo ve reportes de SUS clases
- R56: Alumno solo ve SUS propios reportes

---

## 🧪 Casos de Prueba por Módulo

### MÓDULO 1: ASISTENCIAS

#### CP-ASI-001: Registrar asistencia normal con paquete activo
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Happy Path  
**Precondiciones:**
- Alumno con paquete activo (Estado=Activo, ClasesRestantes > 0)
- Clase completada (fecha <= hoy)
- Sin asistencia previa del alumno en esa clase

**Pasos:**
1. Login como Admin o Profesor
2. Ir a módulo de Asistencias
3. Seleccionar clase
4. Seleccionar alumno
5. Seleccionar TipoAsistencia = "Normal"
6. Seleccionar paquete activo del alumno
7. Estado = "Presente"
8. Guardar

**Resultado Esperado:**
- ✅ Asistencia registrada exitosamente
- ✅ Paquete.ClasesUsadas incrementó en 1
- ✅ Paquete.ClasesRestantes decrementó en 1
- ✅ Si ClasesRestantes = 0, Estado cambió a "Agotado"
- ✅ Mensaje de éxito mostrado

**Reglas Validadas:** R3, R7, R21

---

#### CP-ASI-002: Registrar asistencia de cortesía sin paquete
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Caso especial  
**Precondiciones:**
- Alumno sin paquete activo o con paquete pero se desea cortesía
- Clase completada

**Pasos:**
1. Login como Admin o Profesor
2. Seleccionar clase y alumno
3. Seleccionar TipoAsistencia = "Cortesía"
4. NO seleccionar paquete (dejarlo vacío)
5. Estado = "Presente"
6. Guardar

**Resultado Esperado:**
- ✅ Asistencia registrada exitosamente
- ✅ NO se descontó clase de ningún paquete
- ✅ Observación incluye: "Cortesía - Sin descuento de paquete"
- ✅ Mensaje de éxito

**Reglas Validadas:** R4

---

#### CP-ASI-003: Intentar registrar asistencia normal sin paquete (debe fallar)
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Validación negativa  
**Precondiciones:**
- Alumno sin paquetes activos

**Pasos:**
1. Login como Admin
2. Seleccionar clase y alumno
3. Seleccionar TipoAsistencia = "Normal"
4. Intentar guardar sin paquete

**Resultado Esperado:**
- ❌ Error mostrado: "El tipo de asistencia 'Normal' requiere un paquete activo."
- ❌ Asistencia NO registrada

**Reglas Validadas:** R3

---

#### CP-ASI-004: Intentar registrar asistencia con paquete vencido (debe fallar)
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Validación negativa  
**Precondiciones:**
- Alumno con paquete vencido (FechaVencimiento < hoy)

**Pasos:**
1. Login como Admin
2. Seleccionar clase y alumno
3. Seleccionar TipoAsistencia = "Normal"
4. Seleccionar paquete vencido
5. Intentar guardar

**Resultado Esperado:**
- ❌ Error: "El paquete está vencido."
- ❌ Asistencia NO registrada

**Reglas Validadas:** R22

---

#### CP-ASI-005: Intentar registrar asistencia con paquete congelado (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  
**Precondiciones:**
- Alumno con paquete congelado (Estado=Congelado)

**Pasos:**
1. Login como Admin
2. Congelar paquete activo del alumno
3. Intentar registrar asistencia con ese paquete

**Resultado Esperado:**
- ❌ Error: "El paquete no está activo (estado: Congelado)."
- ❌ Asistencia NO registrada

**Reglas Validadas:** R22

---

#### CP-ASI-006: Intentar registrar asistencia duplicada (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  
**Precondiciones:**
- Ya existe asistencia del alumno en esa clase

**Pasos:**
1. Login como Admin
2. Intentar registrar asistencia del mismo alumno en la misma clase

**Resultado Esperado:**
- ❌ Error: "Ya existe un registro de asistencia para este alumno en esta clase."
- ❌ Asistencia NO registrada

**Reglas Validadas:** R2

---

#### CP-ASI-007: Intentar registrar asistencia a clase futura (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  
**Precondiciones:**
- Clase con fecha futura

**Pasos:**
1. Login como Admin
2. Seleccionar clase futura
3. Intentar registrar asistencia

**Resultado Esperado:**
- ❌ Error: "No se puede registrar asistencia a una clase futura."
- ❌ Asistencia NO registrada

**Reglas Validadas:** R1

---

#### CP-ASI-008: Registrar asistencia de recuperación (no descuenta)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional - Caso especial  
**Precondiciones:**
- Alumno con paquete activo

**Pasos:**
1. Login como Admin
2. Seleccionar TipoAsistencia = "Recuperación"
3. Seleccionar paquete activo
4. Estado = "Presente"
5. Guardar

**Resultado Esperado:**
- ✅ Asistencia registrada
- ✅ NO se descontó clase del paquete (ClasesUsadas sin cambios)
- ✅ Mensaje de éxito

**Reglas Validadas:** R6

---

#### CP-ASI-009: Registrar asistencia "Ausente" (no descuenta)
**Prioridad:** 🟢 BAJA  
**Tipo:** Funcional  
**Precondiciones:**
- Alumno con paquete activo

**Pasos:**
1. Login como Admin
2. Seleccionar alumno y clase
3. TipoAsistencia = "Normal"
4. Seleccionar paquete
5. Estado = "Ausente"
6. Guardar

**Resultado Esperado:**
- ✅ Asistencia registrada con estado "Ausente"
- ✅ NO se descontó clase (solo se descuenta si estado = Presente)
- ✅ Paquete sin cambios

**Reglas Validadas:** R7

---

#### CP-ASI-010: Profesor intenta registrar asistencia en clase de otro profesor (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación de ownership  
**Precondiciones:**
- Login como Profesor A
- Clase creada por Profesor B

**Pasos:**
1. Login como Profesor A
2. Intentar acceder a clase de Profesor B
3. Intentar registrar asistencia

**Resultado Esperado:**
- ❌ Error: "No tienes permiso para gestionar esta clase."
- ❌ Asistencia NO registrada

**Reglas Validadas:** R8

---

### MÓDULO 2: CLASES

#### CP-CLA-001: Crear clase válida (Admin)
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Happy Path  
**Precondiciones:**
- Login como Admin
- Tipo de clase existe
- Profesor existe

**Pasos:**
1. Login como Admin
2. Ir a módulo Clases
3. Click "Nueva Clase"
4. Llenar formulario:
   - Tipo: Tango
   - Fecha: Mañana
   - Hora inicio: 18:00
   - Hora fin: 19:30
   - Profesor: Jorge Padilla
   - Cupo: 20
5. Guardar

**Resultado Esperado:**
- ✅ Clase creada exitosamente
- ✅ Estado inicial = "Programada"
- ✅ Aparece en listado de clases
- ✅ Mensaje de éxito

**Reglas Validadas:** R10, R11

---

#### CP-CLA-002: Crear clase con conflicto de horario (debe fallar)
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Validación negativa  
**Precondiciones:**
- Ya existe clase del profesor el 10/02/2026 de 18:00 a 19:30

**Pasos:**
1. Login como Admin
2. Intentar crear clase del mismo profesor
3. Fecha: 10/02/2026
4. Hora inicio: 18:30 (solapa con clase existente)
5. Hora fin: 20:00
6. Guardar

**Resultado Esperado:**
- ❌ Error: "El profesor ya tiene una clase programada en ese horario."
- ❌ Clase NO creada

**Reglas Validadas:** R9

---

#### CP-CLA-003: Crear clase con fecha pasada (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Intentar crear clase con fecha de ayer
3. Guardar

**Resultado Esperado:**
- ❌ Error: "La fecha debe ser futura."
- ❌ Clase NO creada

**Reglas Validadas:** R10

---

#### CP-CLA-004: Crear clase con HoraFin < HoraInicio (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Hora inicio: 19:00
3. Hora fin: 18:00 (antes de inicio)
4. Intentar guardar

**Resultado Esperado:**
- ❌ Error: "La hora de fin debe ser posterior a la hora de inicio."
- ❌ Clase NO creada

**Reglas Validadas:** R11

---

#### CP-CLA-005: Profesor crea clase para sí mismo
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Ownership  
**Precondiciones:**
- Login como Profesor Jorge

**Pasos:**
1. Login como Profesor Jorge
2. Crear clase asignándose a sí mismo
3. Guardar

**Resultado Esperado:**
- ✅ Clase creada exitosamente
- ✅ Profesor asignado = Jorge

**Reglas Validadas:** R12

---

#### CP-CLA-006: Profesor intenta crear clase para otro profesor (debe fallar)
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Validación de ownership  
**Precondiciones:**
- Login como Profesor Jorge

**Pasos:**
1. Login como Profesor Jorge
2. Intentar crear clase para Profesor Ana
3. Guardar

**Resultado Esperado:**
- ❌ Error: "No tienes permiso para crear clases para otro profesor."
- ❌ Clase NO creada

**Reglas Validadas:** R12

---

#### CP-CLA-007: Editar clase (Admin)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional  

**Pasos:**
1. Login como Admin
2. Seleccionar clase existente
3. Cambiar tipo de clase
4. Cambiar horario
5. Guardar

**Resultado Esperado:**
- ✅ Clase actualizada con nuevos datos
- ✅ Validación de conflicto de horario se ejecuta (excluyendo la clase actual)

**Reglas Validadas:** R9, R13

---

#### CP-CLA-008: Profesor edita su propia clase
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional - Ownership  

**Pasos:**
1. Login como Profesor Jorge
2. Seleccionar SU clase
3. Editar horario
4. Guardar

**Resultado Esperado:**
- ✅ Clase actualizada
- ✅ Validación de conflicto ejecutada

**Reglas Validadas:** R12

---

#### CP-CLA-009: Profesor intenta editar clase de otro profesor (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación de ownership  

**Pasos:**
1. Login como Profesor Jorge
2. Intentar editar clase de Profesor Ana

**Resultado Esperado:**
- ❌ Error: "No tienes permiso para editar esta clase."
- ❌ Cambios NO guardados

**Reglas Validadas:** R12

---

#### CP-CLA-010: Cancelar clase sin asistencias
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional  

**Pasos:**
1. Login como Admin
2. Seleccionar clase sin asistencias
3. Click "Cancelar Clase"
4. Confirmar

**Resultado Esperado:**
- ✅ Estado cambió a "Cancelada"
- ✅ Aparece en listado como cancelada

**Reglas Validadas:** R15

---

#### CP-CLA-011: Intentar cancelar clase con asistencias (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  
**Precondiciones:**
- Clase con al menos 1 asistencia registrada

**Pasos:**
1. Login como Admin
2. Intentar cancelar clase con asistencias

**Resultado Esperado:**
- ❌ Error: "No se puede cancelar una clase con asistencias registradas."
- ❌ Estado NO cambiado

**Reglas Validadas:** R15

---

#### CP-CLA-012: Intentar cancelar clase pasada (debe fallar)
**Prioridad:** 🟢 BAJA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Intentar cancelar clase del día de ayer

**Resultado Esperado:**
- ❌ Error: "No se puede cancelar una clase pasada."
- ❌ Estado NO cambiado

**Reglas Validadas:** R14

---

#### CP-CLA-013: Completar clase genera pagos de profesores
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Integración con Nómina  
**Precondiciones:**
- Clase en estado "Programada"
- Fecha <= hoy

**Pasos:**
1. Login como Admin
2. Seleccionar clase
3. Click "Completar Clase"
4. Confirmar

**Resultado Esperado:**
- ✅ Estado cambió a "Completada"
- ✅ Se creó registro en ClaseProfesor
- ✅ TarifaProgramada calculada = TarifaProfesor × Duración
- ✅ TotalPago = TarifaProgramada
- ✅ EstadoPago = "Pendiente"

**Reglas Validadas:** R16, R40, R41, R42, R43

---

### MÓDULO 3: PAQUETES

#### CP-PAQ-001: Crear paquete válido
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Happy Path  

**Pasos:**
1. Login como Admin
2. Ir a Paquetes
3. Click "Crear Paquete"
4. Seleccionar alumno
5. Tipo paquete: "8 Clases"
6. ClasesDisponibles: 8
7. ValorPaquete: 150000
8. DiasVigencia: 30
9. Guardar

**Resultado Esperado:**
- ✅ Paquete creado con IdEstado = Activo
- ✅ FechaActivacion = hoy
- ✅ FechaVencimiento = hoy + 30 días
- ✅ ClasesUsadas = 0
- ✅ ClasesRestantes = 8

**Reglas Validadas:** R17, R33, R34, R35

---

#### CP-PAQ-002: Descontar clase de paquete activo
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional  
**Precondiciones:**
- Paquete activo con ClasesRestantes > 0

**Pasos:**
1. Login como Admin
2. Registrar asistencia Normal (Presente) usando ese paquete

**Resultado Esperado:**
- ✅ ClasesUsadas incrementó de 0 a 1
- ✅ ClasesRestantes decrementó de 8 a 7
- ✅ Estado sigue siendo Activo

**Reglas Validadas:** R21

---

#### CP-PAQ-003: Agotar paquete al usar última clase
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Estado  
**Precondiciones:**
- Paquete con ClasesRestantes = 1

**Pasos:**
1. Login como Admin
2. Registrar asistencia usando la última clase

**Resultado Esperado:**
- ✅ ClasesUsadas = ClasesDisponibles
- ✅ ClasesRestantes = 0
- ✅ Estado cambió a "Agotado"
- ✅ Ya no aparece como disponible para nuevas asistencias

**Reglas Validadas:** R19, R21

---

#### CP-PAQ-004: Intentar usar paquete agotado (debe fallar)
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Validación negativa  
**Precondiciones:**
- Paquete con Estado = Agotado

**Pasos:**
1. Login como Admin
2. Intentar registrar asistencia con paquete agotado

**Resultado Esperado:**
- ❌ Error: "El paquete no tiene clases disponibles."
- ❌ Asistencia NO registrada

**Reglas Validadas:** R22

---

#### CP-PAQ-005: Paquete se vence automáticamente por fecha
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Estado automático  
**Precondiciones:**
- Paquete con FechaVencimiento = hoy

**Pasos:**
1. Esperar a que pase medianoche (o simular cambio de fecha)
2. Consultar paquete

**Resultado Esperado:**
- ✅ Estado cambió automáticamente a "Vencido"
- ✅ estaVencido = true
- ✅ No se puede usar para asistencias

**Reglas Validadas:** R18

---

#### CP-PAQ-006: Congelar paquete activo
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional  
**Precondiciones:**
- Paquete activo

**Pasos:**
1. Login como Admin
2. Seleccionar paquete
3. Click "Congelar"
4. FechaInicio: Mañana
5. FechaFin: En 7 días
6. Motivo: "Viaje del alumno"
7. Guardar

**Resultado Esperado:**
- ✅ Estado cambió a "Congelado"
- ✅ Registro creado en CongelacionesPaquete
- ✅ FechaVencimiento NO se extendió aún (se calcula al descongelar)
- ✅ No se puede usar para asistencias

**Reglas Validadas:** R23, R24, R25

---

#### CP-PAQ-007: Intentar congelar paquete con fechas inválidas (debe fallar)
**Prioridad:** 🟢 BAJA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Intentar congelar con FechaFin < FechaInicio

**Resultado Esperado:**
- ❌ Error: "La fecha de fin debe ser posterior a la fecha de inicio."
- ❌ Congelación NO creada

**Reglas Validadas:** R23

---

#### CP-PAQ-008: Intentar congelar paquete ya congelado (debe fallar)
**Prioridad:** 🟢 BAJA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Paquete ya congelado
3. Intentar congelar nuevamente

**Resultado Esperado:**
- ❌ Error: "Solo se pueden congelar paquetes activos (estado actual: Congelado)."
- ❌ Congelación NO creada

**Reglas Validadas:** R25

---

#### CP-PAQ-009: Descongelar paquete extiende fecha de vencimiento
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional  
**Precondiciones:**
- Paquete congelado por 7 días

**Pasos:**
1. Login como Admin
2. Seleccionar paquete congelado
3. Click "Descongelar"
4. Confirmar

**Resultado Esperado:**
- ✅ Estado cambió a "Activo"
- ✅ FechaVencimiento se extendió +7 días
- ✅ Puede usarse para asistencias nuevamente

**Reglas Validadas:** R26

---

#### CP-PAQ-010: Alumno solo ve sus propios paquetes
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación de ownership  

**Pasos:**
1. Login como Alumno Juan
2. Ir a "Mis Paquetes"
3. Verificar listado

**Resultado Esperado:**
- ✅ Solo aparecen paquetes del alumno Juan
- ✅ No aparecen paquetes de otros alumnos

**Reglas Validadas:** R27

---

#### CP-PAQ-011: Intentar ver paquete de otro alumno (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación de ownership  

**Pasos:**
1. Login como Alumno Juan
2. Intentar acceder directamente a URL de paquete de otro alumno

**Resultado Esperado:**
- ❌ Error 403: "No tienes permiso para ver este paquete."

**Reglas Validadas:** R27

---

### MÓDULO 4: PAGOS

#### CP-PAG-001: Registrar pago con 2 paquetes iguales
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Happy Path  

**Pasos:**
1. Login como Admin
2. Ir a Pagos
3. Click "Registrar Pago"
4. Seleccionar alumno
5. Monto: 300000
6. Método: Efectivo
7. Agregar 2 paquetes de "8 Clases"
8. Guardar

**Resultado Esperado:**
- ✅ Pago creado con IdEstadoPago = "Pendiente Verificación"
- ✅ 2 paquetes creados y vinculados al pago
- ✅ ValorPaquete de cada uno = 150000 (300000 / 2)
- ✅ Ambos paquetes con Estado = Activo
- ✅ FechaActivacion = FechaPago
- ✅ IdPago vinculado en ambos paquetes

**Reglas Validadas:** R28, R29, R31, R32, R33, R36

---

#### CP-PAG-002: Registrar pago con valores específicos por paquete
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional  

**Pasos:**
1. Login como Admin
2. Registrar pago
3. Monto total: 300000
4. Paquete 1: 8 clases, valor 130000
5. Paquete 2: 12 clases, valor 170000
6. Guardar

**Resultado Esperado:**
- ✅ Pago creado
- ✅ Paquete 1 con ValorPaquete = 130000
- ✅ Paquete 2 con ValorPaquete = 170000
- ✅ Suma: 300000 = MontoTotal ✅

**Reglas Validadas:** R30

---

#### CP-PAG-003: Intentar pago con suma de paquetes > monto total (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Monto total: 300000
3. Paquete 1: valor 200000
4. Paquete 2: valor 150000 (suma = 350000)
5. Intentar guardar

**Resultado Esperado:**
- ❌ Error: "La suma de los valores de los paquetes no puede ser mayor al monto total del pago."
- ❌ Pago NO registrado

**Reglas Validadas:** R30

---

#### CP-PAG-004: Intentar pago con monto 0 o negativo (debe fallar)
**Prioridad:** 🟢 BAJA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Intentar registrar pago con monto = 0 o -100

**Resultado Esperado:**
- ❌ Error: "El monto total debe ser mayor a cero."
- ❌ Pago NO registrado

**Reglas Validadas:** R28

---

#### CP-PAG-005: Intentar pago sin paquetes (debe fallar)
**Prioridad:** 🟢 BAJA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Registrar pago sin agregar paquetes

**Resultado Esperado:**
- ❌ Error: "Debe especificar al menos un paquete nuevo o vincular paquetes existentes."
- ❌ Pago NO registrado

**Reglas Validadas:** R29

---

#### CP-PAG-006: Vincular paquetes existentes a pago
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional  
**Precondiciones:**
- Existen paquetes sin IdPago (creados manualmente)

**Pasos:**
1. Login como Admin
2. Crear paquete sin vincular a pago (ajuste administrativo)
3. Registrar pago y vincular ese paquete existente

**Resultado Esperado:**
- ✅ Pago creado
- ✅ Paquete existente ahora tiene IdPago vinculado
- ✅ FechaModificacion actualizada

**Reglas Validadas:** R37

---

#### CP-PAG-007: Intentar vincular paquete de otro alumno (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Registrar pago para Alumno A
3. Intentar vincular paquete que pertenece a Alumno B

**Resultado Esperado:**
- ❌ Error: "Uno o más paquetes especificados no pertenecen al alumno."
- ❌ Pago NO registrado

**Reglas Validadas:** R37

---

#### CP-PAG-008: Intentar vincular paquete que ya tiene pago (debe fallar)
**Prioridad:** 🟢 BAJA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Paquete ya vinculado a pago anterior (IdPago != null)
3. Intentar vincularlo a nuevo pago

**Resultado Esperado:**
- ❌ Error: "Uno o más paquetes ya tienen pago asociado."
- ❌ Pago NO registrado

**Reglas Validadas:** R38

---

#### CP-PAG-009: Alumno solo ve sus propios pagos
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación de ownership  

**Pasos:**
1. Login como Alumno Juan
2. Ir a "Mis Pagos"

**Resultado Esperado:**
- ✅ Solo aparecen pagos del Alumno Juan
- ✅ No aparecen pagos de otros alumnos

**Reglas Validadas:** R39

---

### MÓDULO 5: NÓMINA PROFESORES

#### CP-NOM-001: Completar clase con 1 profesor principal genera pago pendiente
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Happy Path  
**Precondiciones:**
- Clase programada con 1 profesor principal (Jorge)
- Jorge tiene TarifaPrincipal = 40000
- Clase duración: 1.5 horas
- Clase en fecha <= hoy

**Pasos:**
1. Login como Admin
2. Completar clase
3. Ir a módulo Nómina

**Resultado Esperado:**
- ✅ Aparece nueva entrada en "Clases Pendientes"
- ✅ TarifaProgramada = 40000 × 1.5 = 60000
- ✅ TotalPago = 60000 (sin ajustes aún)
- ✅ EstadoPago = "Pendiente"
- ✅ RolEnClase = "Principal"
- ✅ FechaCreacion = hoy

**Reglas Validadas:** R40, R41, R42, R42b, R43

---

#### CP-NOM-002: Aprobar pago sin ajustes
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional  
**Precondiciones:**
- Clase completada en estado "Pendiente"

**Pasos:**
1. Login como Admin
2. Ir a Nómina → Clases Pendientes
3. Seleccionar clase
4. Click "Aprobar Pago"
5. No agregar ajustes
6. Confirmar

**Resultado Esperado:**
- ✅ EstadoPago cambió a "Aprobado"
- ✅ FechaAprobacion = hoy
- ✅ AprobadoPorIdUsuario = ID del admin
- ✅ TotalPago sin cambios
- ✅ Movió de "Pendientes" a "Aprobadas"

**Reglas Validadas:** R44, R45

---

#### CP-NOM-003: Aprobar pago con ajuste adicional
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional  
**Precondiciones:**
- Clase completada en estado "Pendiente"
- TarifaProgramada = 30000

**Pasos:**
1. Login como Admin
2. Ir a Nómina → Pendientes
3. Seleccionar clase
4. Click "Aprobar con Ajuste"
5. ValorAdicional: 5000
6. Concepto: "Bono por clase especial"
7. Confirmar

**Resultado Esperado:**
- ✅ ValorAdicional = 5000
- ✅ ConceptoAdicional = "Bono por clase especial"
- ✅ TotalPago = 35000 (30000 + 5000)
- ✅ EstadoPago = "Aprobado"
- ✅ FechaAprobacion registrada

**Reglas Validadas:** R41, R45

---

#### CP-NOM-004: Intentar aprobar pago ya aprobado (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Pago ya en estado "Aprobado"
3. Intentar aprobar nuevamente

**Resultado Esperado:**
- ❌ Error: "El pago no está en estado Pendiente (Estado actual: Aprobado)."
- ❌ Sin cambios

**Reglas Validadas:** R44

---

#### CP-NOM-005: Liquidar mes con 3 clases aprobadas
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional  
**Precondiciones:**
- 3 clases aprobadas del mismo profesor en enero 2026

**Pasos:**
1. Login como Admin
2. Ir a Nómina → Liquidar Mes
3. Seleccionar mes: Enero 2026
4. Seleccionar profesor
5. Verificar resumen (3 clases, total $90000)
6. Click "Liquidar"
7. Confirmar

**Resultado Esperado:**
- ✅ LiquidacionMensual creada:
  - Mes = 1
  - Año = 2026
  - TotalClases = 3
  - TotalPagar = 90000
  - Estado = "Cerrada"
  - FechaCierre = hoy
- ✅ Las 3 ClaseProfesor cambiaron EstadoPago a "Liquidado"
- ✅ Ya no aparecen en "Aprobadas"

**Reglas Validadas:** R46, R47, R48

---

#### CP-NOM-006: Intentar liquidar pagos pendientes (debe fallar)
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. Intentar liquidar mes con pagos en estado "Pendiente" (no aprobados)

**Resultado Esperado:**
- ❌ Error: "Solo se pueden liquidar pagos en estado Aprobado."
- ❌ Liquidación NO creada

**Reglas Validadas:** R46

---

#### CP-NOM-007: Registrar pago físico a profesor
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional  
**Precondiciones:**
- Liquidación cerrada

**Pasos:**
1. Login como Admin
2. Ir a Liquidaciones
3. Seleccionar liquidación cerrada
4. Click "Registrar Pago"
5. Método pago: Transferencia
6. Comprobante: URL o referencia
7. Guardar

**Resultado Esperado:**
- ✅ Estado de liquidación cambió a "Pagada"
- ✅ FechaPago = hoy
- ✅ Todas las ClaseProfesor cambiaron a EstadoPago = "Pagado"
- ✅ Aparece en historial de pagos realizados

**Reglas Validadas:** R49

---

#### CP-NOM-008: Profesor solo ve sus propias clases en reporte
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación de ownership  

**Pasos:**
1. Login como Profesor Jorge
2. Ir a "Mis Clases Dictadas"

**Resultado Esperado:**
- ✅ Solo aparecen clases del Profesor Jorge
- ✅ No aparecen clases de otros profesores
- ✅ Puede ver estado de pago de sus clases

**Reglas Validadas:** R50

---

#### CP-NOM-009: Completar clase con múltiples profesores (Principal + Monitor)
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Múltiples profesores  
**Precondiciones:**
- Clase con 1 profesor principal (Jorge) y 1 monitor (Ana)
- Jorge.TarifaPrincipal = 40000
- Ana.TarifaMonitor = 15000
- Duración: 1.5 horas

**Pasos:**
1. Login como Admin
2. Crear clase asignando Jorge como Principal y Ana como Monitor
3. Completar clase
4. Ir a módulo Nómina

**Resultado Esperado:**
- ✅ Se crearon 2 registros en ClaseProfesor
- ✅ Jorge: TarifaProgramada = 40000 × 1.5 = 60000, Rol = Principal
- ✅ Ana: TarifaProgramada = 15000 × 1.5 = 22500, Rol = Monitor
- ✅ Ambos con EstadoPago = "Pendiente"
- ✅ Total a pagar en esa clase = 82500

**Reglas Validadas:** R40, R42, R42b, R42c

---

#### CP-NOM-010: Completar clase con múltiples profesores principales
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Múltiples principales  
**Precondiciones:**
- Clase con 2 profesores principales (Jorge + Ana)
- Jorge.TarifaPrincipal = 40000
- Ana.TarifaPrincipal = 40000
- Duración: 1.0 hora

**Pasos:**
1. Login como Admin
2. Crear clase asignando Jorge y Ana como Principales
3. Completar clase
4. Verificar módulo Nómina

**Resultado Esperado:**
- ✅ Se crearon 2 registros ClaseProfesor
- ✅ Jorge: TarifaProgramada = 40000 × 1.0 = 40000, Rol = Principal
- ✅ Ana: TarifaProgramada = 40000 × 1.0 = 40000, Rol = Principal
- ✅ Total clase = 80000

**Reglas Validadas:** R40, R42e

---

#### CP-NOM-011: Completar clase con múltiples monitores
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional - Múltiples monitores  
**Precondiciones:**
- Clase con 1 principal (Jorge) y 2 monitores (Santi + María)
- Jorge.TarifaPrincipal = 40000
- Santi.TarifaMonitor = 12000
- María.TarifaMonitor = 12000
- Duración: 2.0 horas

**Pasos:**
1. Login como Admin
2. Crear clase con configuración descrita
3. Completar clase
4. Verificar Nómina

**Resultado Esperado:**
- ✅ 3 registros ClaseProfesor creados
- ✅ Jorge: 40000 × 2.0 = 80000 (Principal)
- ✅ Santi: 12000 × 2.0 = 24000 (Monitor)
- ✅ María: 12000 × 2.0 = 24000 (Monitor)
- ✅ Total clase = 128000

**Reglas Validadas:** R40, R42f

---

#### CP-NOM-012: Mismo profesor es Principal en clase A y Monitor en clase B
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Roles flexibles  
**Precondiciones:**
- Jorge.TarifaPrincipal = 40000
- Jorge.TarifaMonitor = 15000
- Clase A: Jorge como Principal, duración 1 hora
- Clase B: Jorge como Monitor (Ana es principal), duración 1 hora

**Pasos:**
1. Login como Admin
2. Completar Clase A
3. Completar Clase B
4. Ir a Nómina y filtrar por Jorge

**Resultado Esperado:**
- ✅ Jorge tiene 2 pagos pendientes
- ✅ Clase A: TarifaProgramada = 40000 (usó TarifaPrincipal)
- ✅ Clase B: TarifaProgramada = 15000 (usó TarifaMonitor)
- ✅ Total a pagar a Jorge = 55000

**Reglas Validadas:** R42b, R42c, R42d

---

#### CP-NOM-013: Modificar tarifas de profesor desde vista usuarios
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Configuración  
**Precondiciones:**
- Login como Admin
- Profesor Santi existe con tarifas actuales

**Pasos:**
1. Login como Admin
2. Ir a Usuarios → Profesores
3. Seleccionar Santi
4. Editar perfil
5. Cambiar TarifaPrincipal de 30000 a 35000
6. Cambiar TarifaMonitor de 10000 a 12000
7. Guardar

**Resultado Esperado:**
- ✅ Tarifas actualizadas en BD
- ✅ Próximas clases de Santi usarán nuevas tarifas
- ✅ Clases anteriores mantienen TarifaProgramada original

**Reglas Validadas:** R42a

---

#### CP-NOM-014: Liquidar mes con clases de diferentes tarifas
**Prioridad:** 🔴 CRÍTICA  
**Tipo:** Funcional - Integración completa  
**Precondiciones:**
- Jorge trabajó en enero:
  - 2 clases como Principal (40k/h × 1.5h = 60k c/u)
  - 1 clase como Monitor (15k/h × 1h = 15k)
- Total: 135000

**Pasos:**
1. Completar las 3 clases
2. Aprobar los 3 pagos
3. Liquidar mes de enero

**Resultado Esperado:**
- ✅ LiquidacionMensual creada
- ✅ TotalClases = 3
- ✅ TotalPagar = 135000
- ✅ Desglose muestra 2 clases Principal + 1 Monitor
- ✅ Estado = "Cerrada"

**Reglas Validadas:** R40, R41, R42, R47, R48

---

### MÓDULO 6: REPORTES

#### CP-REP-001: Generar reporte de asistencias con rango válido
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional  

**Pasos:**
1. Login como Admin
2. Ir a Reportes → Asistencias
3. FechaDesde: 01/01/2026
4. FechaHasta: 31/01/2026
5. Generar

**Resultado Esperado:**
- ✅ Reporte generado con datos del período
- ✅ Métricas mostradas: total, presentes, ausentes, %
- ✅ Gráficas renderizadas

**Reglas Validadas:** R53

---

#### CP-REP-002: Intentar reporte con rango > 1 año (debe fallar)
**Prioridad:** 🟢 BAJA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. FechaDesde: 01/01/2025
3. FechaHasta: 31/12/2026 (2 años)
4. Intentar generar

**Resultado Esperado:**
- ❌ Error: "El rango de fechas no puede superar 1 año."
- ❌ Reporte NO generado

**Reglas Validadas:** R51

---

#### CP-REP-003: Intentar reporte con FechaDesde > FechaHasta (debe fallar)
**Prioridad:** 🟢 BAJA  
**Tipo:** Validación negativa  

**Pasos:**
1. Login como Admin
2. FechaDesde: 31/01/2026
3. FechaHasta: 01/01/2026 (invertido)
4. Intentar generar

**Resultado Esperado:**
- ❌ Error: "La fecha de inicio debe ser anterior a la fecha de fin."
- ❌ Reporte NO generado

**Reglas Validadas:** R53

---

#### CP-REP-004: Exportar reporte a Excel
**Prioridad:** 🟡 MEDIA  
**Tipo:** Funcional  

**Pasos:**
1. Login como Admin
2. Generar reporte de asistencias
3. Click "Exportar a Excel"

**Resultado Esperado:**
- ✅ Archivo .xlsx descargado
- ✅ Contiene datos del reporte en formato tabla
- ✅ Formato profesional con colores y headers

---

#### CP-REP-005: Profesor solo ve reportes de sus clases
**Prioridad:** 🟡 MEDIA  
**Tipo:** Validación de ownership  

**Pasos:**
1. Login como Profesor Jorge
2. Ir a Reportes → Mis Clases

**Resultado Esperado:**
- ✅ Solo aparecen datos de clases del Profesor Jorge
- ✅ No aparecen datos de otros profesores

**Reglas Validadas:** R55

---

#### CP-REP-006: Dashboard cacheado por 5 minutos
**Prioridad:** 🟢 BAJA  
**Tipo:** Performance  

**Pasos:**
1. Login como Admin
2. Acceder a Dashboard (primera vez)
3. Esperar 2 minutos
4. Refrescar Dashboard

**Resultado Esperado:**
- ✅ Segunda carga es instantánea (datos cacheados)
- ✅ Después de 5 minutos, cache expira y se recalcula

**Reglas Validadas:** R54

---

## 📊 Priorización de Pruebas

### Criterios de Priorización
1. **Impacto en Negocio:** ¿Afecta ingresos o operación crítica?
2. **Frecuencia de Uso:** ¿Cuántas veces al día se ejecuta?
3. **Complejidad:** ¿Tiene muchas dependencias?
4. **Riesgo de Regresión:** ¿Historial de errores?

### Matriz de Priorización

| Prioridad | Casos de Prueba | Total | % |
|-----------|----------------|-------|---|
| 🔴 **CRÍTICA** | CP-ASI-001, CP-ASI-002, CP-ASI-003, CP-ASI-004, CP-CLA-001, CP-CLA-002, CP-CLA-005, CP-CLA-006, CP-CLA-013, CP-PAQ-001, CP-PAQ-002, CP-PAQ-003, CP-PAQ-004, CP-PAQ-005, CP-PAG-001, CP-PAG-002, CP-NOM-001, CP-NOM-002, CP-NOM-003, CP-NOM-005, CP-NOM-009, CP-NOM-010, CP-NOM-012, CP-NOM-013, CP-NOM-014 | **25** | **39%** |
| 🟡 **MEDIA** | CP-ASI-005, CP-ASI-006, CP-ASI-007, CP-ASI-008, CP-ASI-010, CP-CLA-003, CP-CLA-004, CP-CLA-007, CP-CLA-008, CP-CLA-009, CP-CLA-010, CP-CLA-011, CP-PAQ-006, CP-PAQ-009, CP-PAQ-010, CP-PAQ-011, CP-PAG-003, CP-PAG-006, CP-PAG-007, CP-PAG-009, CP-NOM-004, CP-NOM-006, CP-NOM-008, CP-NOM-011, CP-REP-001, CP-REP-004, CP-REP-005 | **27** | **42%** |
| 🟢 **BAJA** | CP-ASI-009, CP-CLA-012, CP-PAQ-007, CP-PAQ-008, CP-PAG-004, CP-PAG-005, CP-PAG-008, CP-REP-002, CP-REP-003, CP-REP-006 | **10** | **16%** |

### Fases de Implementación

**FASE 1 - MVP (2-3 semanas):**
- ✅ Todos los casos 🔴 CRÍTICA
- ✅ Total: 25 casos (incluyendo tarifas individuales y roles flexibles)
- ✅ Cubre flujos principales de negocio

**FASE 2 - Consolidación (2 semanas):**
- ✅ Todos los casos 🟡 MEDIA
- ✅ Total: 27 casos
- ✅ Cubre validaciones y casos especiales

**FASE 3 - Cobertura Completa (1 semana):**
- ✅ Todos los casos 🟢 BAJA
- ✅ Total: 10 casos
- ✅ Edge cases y optimizaciones

**TOTAL GENERAL: 62 Casos de Prueba**

---

## 🏗️ Estrategia de Implementación

### Stack Tecnológico Recomendado

#### Backend Testing
- **xUnit**: Framework de testing para .NET
- **Moq**: Mocking de dependencias
- **FluentAssertions**: Assertions legibles
- **WebApplicationFactory**: Integration tests de API

#### Frontend Testing
- **Playwright**: E2E testing (ya recomendado)
- **React Testing Library**: Unit tests de componentes
- **MSW**: Mock Service Worker para APIs
- **Vitest**: Test runner (ya configurado)

#### CI/CD
- **GitHub Actions** o **Azure DevOps**
- Ejecutar tests en cada PR
- Reportes de cobertura

### Estructura de Proyecto de Testing

```
chetango-backend/
├── Chetango.Tests/                    ← NUEVO
│   ├── Unit/
│   │   ├── AsistenciasTests/
│   │   │   ├── RegistrarAsistenciaHandlerTests.cs
│   │   │   └── ValidacionTipoAsistenciaTests.cs
│   │   ├── ClasesTests/
│   │   ├── PaquetesTests/
│   │   ├── PagosTests/
│   │   └── NominaTests/
│   ├── Integration/
│   │   ├── AsistenciasIntegrationTests.cs
│   │   ├── ClasesIntegrationTests.cs
│   │   └── PagosIntegrationTests.cs
│   └── TestHelpers/
│       ├── TestData.cs
│       ├── DatabaseFixture.cs
│       └── AuthenticationHelper.cs

chetangoFrontend/
├── e2e/                                ← NUEVO (Playwright)
│   ├── asistencias/
│   │   ├── registrar-asistencia.spec.ts
│   │   ├── tipos-asistencia.spec.ts
│   │   └── validaciones.spec.ts
│   ├── clases/
│   │   ├── crear-clase.spec.ts
│   │   ├── editar-clase.spec.ts
│   │   └── conflictos.spec.ts
│   ├── paquetes/
│   │   ├── crear-paquete.spec.ts
│   │   ├── congelar-paquete.spec.ts
│   │   └── descontar-clase.spec.ts
│   ├── pagos/
│   │   └── registrar-pago.spec.ts
│   ├── nomina/
│   │   ├── aprobar-pago.spec.ts
│   │   └── liquidar-mes.spec.ts
│   └── helpers/
│       ├── auth.helper.ts
│       ├── test-data.ts
│       └── page-objects/
└── playwright.config.ts
```

### Ejemplo de Test Backend (xUnit)

```csharp
// Chetango.Tests/Unit/AsistenciasTests/RegistrarAsistenciaHandlerTests.cs
public class RegistrarAsistenciaHandlerTests
{
    [Fact]
    public async Task Handle_AsistenciaNormal_DescontaPaquete()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var handler = new RegistrarAsistenciaCommandHandler(dbContext, _mediator);
        
        var paquete = CreatePaqueteActivo(clasesDisponibles: 8, clasesUsadas: 0);
        var clase = CreateClase(fecha: DateTime.Today.AddDays(-1));
        var alumno = CreateAlumno();
        var tipoNormal = CreateTipoAsistencia(nombre: "Normal", descontarClase: true);
        
        dbContext.Add(paquete);
        dbContext.Add(clase);
        dbContext.Add(alumno);
        dbContext.Add(tipoNormal);
        await dbContext.SaveChangesAsync();
        
        var command = new RegistrarAsistenciaCommand
        {
            IdClase = clase.IdClase,
            IdAlumno = alumno.IdAlumno,
            IdTipoAsistencia = tipoNormal.IdTipoAsistencia,
            IdPaqueteUsado = paquete.IdPaquete,
            IdEstadoAsistencia = 1 // Presente
        };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Succeeded.Should().BeTrue();
        paquete.ClasesUsadas.Should().Be(1);
        paquete.ClasesRestantes.Should().Be(7);
    }
    
    [Fact]
    public async Task Handle_AsistenciaCortesia_NoDescontaPaquete()
    {
        // Arrange
        var handler = CreateHandler();
        var tipoCortesia = CreateTipoAsistencia(nombre: "Cortesía", descontarClase: false);
        
        var command = new RegistrarAsistenciaCommand
        {
            IdTipoAsistencia = tipoCortesia.IdTipoAsistencia,
            IdPaqueteUsado = null, // Sin paquete
            IdEstadoAsistencia = 1
        };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Succeeded.Should().BeTrue();
        // Verificar que NO se descontó ningún paquete
    }
}
```

### Ejemplo de Test E2E (Playwright)

```typescript
// e2e/asistencias/registrar-asistencia.spec.ts
import { test, expect } from '@playwright/test';
import { loginAsAdmin, loginAsProfesor } from '../helpers/auth.helper';
import { createPaqueteActivo, createClaseCompletada } from '../helpers/test-data';

test.describe('Registrar Asistencia', () => {
  test('CP-ASI-001: Registrar asistencia normal descuenta paquete', async ({ page }) => {
    // Arrange
    await loginAsAdmin(page);
    const paquete = await createPaqueteActivo({ clasesDisponibles: 8 });
    const clase = await createClaseCompletada();
    
    // Act - Navegar y registrar asistencia
    await page.goto('/admin/asistencias');
    await page.click('text=Registrar Asistencia');
    
    await page.selectOption('[name="idClase"]', clase.idClase);
    await page.selectOption('[name="idAlumno"]', paquete.idAlumno);
    await page.selectOption('[name="idTipoAsistencia"]', 'Normal');
    await page.selectOption('[name="idPaquete"]', paquete.idPaquete);
    await page.selectOption('[name="estado"]', 'Presente');
    
    await page.click('button:has-text("Guardar")');
    
    // Assert
    await expect(page.locator('text=Asistencia registrada exitosamente')).toBeVisible();
    
    // Verificar que el paquete se descontó
    await page.goto(`/admin/paquetes/${paquete.idPaquete}`);
    await expect(page.locator('text=Clases Usadas: 1')).toBeVisible();
    await expect(page.locator('text=Clases Restantes: 7')).toBeVisible();
  });
  
  test('CP-ASI-002: Asistencia de cortesía no requiere paquete', async ({ page }) => {
    await loginAsAdmin(page);
    
    await page.goto('/admin/asistencias');
    await page.click('text=Registrar Asistencia');
    
    // Seleccionar tipo Cortesía
    await page.selectOption('[name="idTipoAsistencia"]', 'Cortesía');
    
    // Campo de paquete debe estar deshabilitado o no requerido
    const paqueteField = page.locator('[name="idPaquete"]');
    await expect(paqueteField).toBeDisabled();
    
    // Guardar sin paquete
    await page.click('button:has-text("Guardar")');
    
    await expect(page.locator('text=Asistencia registrada exitosamente')).toBeVisible();
  });
  
  test('CP-ASI-003: Intentar asistencia normal sin paquete muestra error', async ({ page }) => {
    await loginAsAdmin(page);
    
    await page.goto('/admin/asistencias');
    await page.click('text=Registrar Asistencia');
    
    await page.selectOption('[name="idTipoAsistencia"]', 'Normal');
    // NO seleccionar paquete
    
    await page.click('button:has-text("Guardar")');
    
    // Assert - Error mostrado
    await expect(page.locator('text=requiere un paquete activo')).toBeVisible();
  });
});
```

---

## 🎲 Datos de Prueba

### Usuarios de Testing
```sql
-- Ya existen en seed_usuarios_prueba_ciam.sql
Admin: Chetango@chetangoprueba.onmicrosoft.com
Profesor: Jorgepadilla@chetangoprueba.onmicrosoft.com
Alumno: JuanDavid@chetangoprueba.onmicrosoft.com
```

### Datos Adicionales Requeridos

```sql
-- Crear datos de prueba para testing automatizado
-- Ejecutar DESPUÉS de seed_usuarios_prueba_ciam.sql

-- Paquetes de prueba
INSERT INTO Paquetes (IdPaquete, IdAlumno, IdTipoPaquete, ClasesDisponibles, ClasesUsadas, 
                      FechaActivacion, FechaVencimiento, IdEstado, ValorPaquete)
VALUES 
  (NEWID(), 'ID_ALUMNO_JUAN', 'ID_TIPO_8_CLASES', 8, 0, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1, 150000),
  (NEWID(), 'ID_ALUMNO_JUAN', 'ID_TIPO_8_CLASES', 8, 7, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1, 150000); -- Casi agotado

-- Clases de prueba
INSERT INTO Clases (IdClase, Fecha, IdTipoClase, HoraInicio, HoraFin, IdProfesorPrincipal, Estado)
VALUES
  (NEWID(), DATEADD(DAY, -1, GETDATE()), 'ID_TIPO_TANGO', '18:00:00', '19:30:00', 'ID_PROFESOR_JORGE', 'Completada'),
  (NEWID(), DATEADD(DAY, 1, GETDATE()), 'ID_TIPO_VALS', '19:00:00', '20:00:00', 'ID_PROFESOR_JORGE', 'Programada');
```

### Script de Limpieza (Reset Database para Tests)

```sql
-- scripts/reset_test_data.sql
-- Ejecutar para limpiar datos de prueba entre test runs

DELETE FROM Asistencias WHERE IdAsistencia IN (SELECT IdAsistencia FROM Asistencias WHERE UsuarioCreacion = 'TestAutomation');
DELETE FROM ClasesProfesores WHERE FechaCreacion > DATEADD(HOUR, -1, GETDATE());
DELETE FROM Clases WHERE Estado = 'TestData';
UPDATE Paquetes SET ClasesUsadas = 0, IdEstado = 1 WHERE IdAlumno IN (SELECT IdAlumno FROM Alumnos WHERE Nombre = 'TestAlumno');
```

---

## 📈 Métricas y Reportes de Testing

### KPIs de Testing
1. **Cobertura de Código:** Objetivo ≥ 80%
2. **Tasa de Éxito:** Objetivo ≥ 95%
3. **Tiempo de Ejecución:** < 10 minutos (todos los tests)
4. **Tests Flaky:** < 5% (tests que fallan intermitentemente)

### Reporte de Ejecución
```
=================================================
REPORTE DE PRUEBAS - Sistema Chetango
Fecha: 05/02/2026 10:30 AM
=================================================

RESUMEN GENERAL:
✅ Pasados: 59 / 64  (92.2%)
❌ Fallidos: 5       (7.8%)
⏭️ Omitidos: 0       (0%)

TIEMPO TOTAL: 9m 45s

DESGLOSE POR MÓDULO:
┌─────────────────┬────────┬─────────┬─────────┬──────────┐
│ Módulo          │ Total  │ Pasados │ Fallidos│ % Éxito  │
├─────────────────┼────────┼─────────┼─────────┼──────────┤
│ Asistencias     │   10   │    10   │    0    │  100%    │
│ Clases          │   13   │    12   │    1    │  92.3%   │
│ Paquetes        │   11   │    11   │    0    │  100%    │
│ Pagos           │    9   │     8   │    1    │  88.9%   │
│ Nómina          │   14   │    12   │    2    │  85.7%   │
│ Reportes        │    6   │     5   │    1    │  83.3%   │
└─────────────────┴────────┴─────────┴─────────┴──────────┘

TESTS FALLIDOS:
❌ CP-CLA-002: Crear clase con conflicto de horario
   Error: Validación no detectó conflicto
   
❌ CP-PAG-003: Intentar pago con suma > monto total
   Error: No mostró mensaje de error esperado
   
❌ CP-NOM-005: Liquidar mes con 3 clases
   Error: TotalPagar incorrecto (diferencia: $5000)
   
❌ CP-REP-002: Rango > 1 año
   Error: No bloqueó ejecución

RECOMENDACIONES:
1. Revisar validación de conflictos en ClasesCommandHandler
2. Verificar cálculo de liquidación mensual
3. Agregar validación de rango de fechas en ReportesQuery
```

---

## ✅ Próximos Pasos

### Semana 1-2: Setup Inicial
1. ✅ Instalar xUnit en backend
2. ✅ Instalar Playwright en frontend
3. ✅ Configurar CI/CD pipeline
4. ✅ Crear estructura de carpetas de testing

### Semana 3-4: FASE 1 - MVP
1. ✅ Implementar 20 casos 🔴 CRÍTICA
2. ✅ Setup de datos de prueba automatizados
3. ✅ Integrar con CI/CD

### Semana 5-6: FASE 2 - Consolidación
1. ✅ Implementar 26 casos 🟡 MEDIA
2. ✅ Refinar helpers y page objects
3. ✅ Documentar patrones de testing

### Semana 7: FASE 3 - Cobertura Completa
1. ✅ Implementar 10 casos 🟢 BAJA
2. ✅ Optimizar tiempos de ejecución
3. ✅ Reporte de cobertura final

---

## 📚 Referencias y Recursos

### Documentación del Sistema
- [API Contract - Asistencias](./API-CONTRACT-ASISTENCIAS.md)
- [API Contract - Clases](./API-CONTRACT-CLASES.md)
- [API Contract - Pagos](./API-CONTRACT-PAGOS.md)
- [API Contract - Paquetes](./API-CONTRACT-PAQUETES.md)
- [Proceso Nómina Profesores](./PROCESO-NOMINA-PROFESORES.md)
- [Test Módulo Paquetes](./test-modulo-paquetes.md)
- [Test Módulo Pagos](./test-modulo-pagos.md)

### Recursos Externos
- [Playwright Documentation](https://playwright.dev)
- [xUnit Documentation](https://xunit.net)
- [React Testing Library](https://testing-library.com/react)

---

**Documento generado por:** GitHub Copilot  
**Fecha:** 05 de Febrero de 2026  
**Versión:** 1.0

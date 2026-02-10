# 💼 Proceso de Nómina de Profesores - Sistema Chetango

## � Manual de Capacitación para Administradores

> **Versión:** 2.0 - Actualizado Febrero 2026  
> **Autor:** Sistema Chetango  
> **Última Actualización:** 02 de febrero de 2026

---

## 📋 Tabla de Contenidos

1. [Introducción y Resumen Ejecutivo](#introducción)
2. [Flujo Correcto y Mejores Prácticas](#flujo-correcto) ⭐ **NUEVO**
3. [Prerrequisitos y Configuración](#prerrequisitos)
4. [Proceso Completo: Paso a Paso](#proceso-completo)
5. [Estados del Sistema](#estados-del-sistema)
6. [Ejemplo Práctico Completo](#ejemplo-práctico)
7. [Vista del Profesor](#vista-del-profesor)
8. [Preguntas Frecuentes](#preguntas-frecuentes)
9. [Solución de Problemas](#solución-de-problemas)

---

## 🎯 Introducción y Resumen Ejecutivo {#introducción}

### ¿Qué es el Sistema de Nómina?

El **Sistema de Nómina de Profesores** es un módulo integrado que gestiona automáticamente:
- ✅ Cálculo de pagos según tarifas preconfiguradas
- ✅ Aprobación y ajustes de pagos por clase
- ✅ Liquidación mensual por profesor
- ✅ Registro de pagos efectuados
- ✅ Trazabilidad completa del proceso

### Flujo Resumido

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   CREAR     │ → │  COMPLETAR  │ → │   APROBAR   │ → │  LIQUIDAR   │ → │  REGISTRAR  │
│   CLASE     │   │    CLASE    │   │    PAGO     │   │     MES     │   │    PAGO     │
└─────────────┘   └─────────────┘   └─────────────┘   └─────────────┘   └─────────────┘
   Estado:           Estado:           Estado:           Estado:           Estado:
  Programada        Completada        Aprobado         Liquidado          Pagado
```

**Tiempo estimado del proceso completo:** 1-5 minutos por clase (dependiendo de ajustes)

---

## 🎓 Flujo Correcto y Mejores Prácticas {#flujo-correcto}

### 📅 Ciclo Mensual Recomendado

El sistema está diseñado para seguir un **ciclo mensual** con liquidación incremental y pago único al final:

```
┌─────────────────────────────────────────────────────────────────────┐
│  FASE 1: Durante el mes (Día 1-28)                                  │
├─────────────────────────────────────────────────────────────────────┤
│  • Profesores dictan clases                                         │
│  • Admin aprueba pagos → Estado: "Aprobado"                        │
│  • Las clases se acumulan esperando liquidación                    │
│  ⚠️  NO liquidar ni pagar todavía                                   │
└─────────────────────────────────────────────────────────────────────┘
        ↓
┌─────────────────────────────────────────────────────────────────────┐
│  FASE 2: Fin de mes (Día 28-31)                                     │
├─────────────────────────────────────────────────────────────────────┤
│  • Admin hace "Liquidar Mes" para cada profesor                    │
│  • Sistema agrupa TODAS las clases aprobadas del mes              │
│  • Liquidación creada → Estado: "Cerrada"                          │
│  • Clases cambian: "Aprobado" → "Liquidado"                        │
│  ⚠️  NO registrar pago todavía                                      │
└─────────────────────────────────────────────────────────────────────┘
        ↓
┌─────────────────────────────────────────────────────────────────────┐
│  FASE 3: Inicio mes siguiente (Día 1-5)                             │
├─────────────────────────────────────────────────────────────────────┤
│  • Admin registra el pago (transferencia bancaria)                 │
│  • Liquidación cambia: "Cerrada" → "Pagada"                        │
│  • Clases cambian: "Liquidado" → "Pagado"                          │
│  ✅ Ciclo completo, proceso finalizado                              │
└─────────────────────────────────────────────────────────────────────┘
```

---

### 🔄 Liquidaciones Incrementales (Característica Nueva)

El sistema ahora permite **agregar clases a liquidaciones existentes** siempre que NO estén pagadas:

#### ✅ Escenario Correcto: Liquidación + Agregar + Pagar

```
📅 5 de febrero
├─ Jorge dicta 1 clase ($70,000)
├─ Admin aprueba la clase
├─ Admin hace "Liquidar Mes" (anticipado)
└─ Liquidación creada: $70,000 → Estado: "Cerrada"

📅 7 de febrero  
├─ Jorge dicta otra clase ($90,000)
├─ Admin aprueba la clase
├─ Admin hace "Liquidar Mes" NUEVAMENTE
├─ Sistema DETECTA liquidación "Cerrada" existente
├─ Sistema AGREGA la nueva clase a la liquidación
└─ Liquidación actualizada: $160,000 → Estado: "Cerrada"

📅 28 de febrero
├─ Pueden seguir agregándose más clases
└─ Total acumulado en la liquidación crece

📅 1 de marzo
├─ Admin registra UN SOLO PAGO por el total
└─ Jorge recibe $160,000 (todas las clases juntas)
```

**💡 Ventaja:** Puedes llevar control progresivo durante el mes sin perder flexibilidad.

---

#### ❌ Escenario Incorrecto: Pagar Antes de Tiempo

```
📅 5 de febrero
├─ Jorge dicta 1 clase ($70,000)
├─ Admin aprueba y liquida
├─ Admin PAGA inmediatamente ❌
└─ Liquidación: $70,000 → Estado: "Pagada"

📅 7 de febrero
├─ Jorge dicta otra clase ($90,000)
├─ Admin aprueba la clase
├─ Admin intenta "Liquidar Mes"
└─ ❌ ERROR: "Ya existe una liquidación PAGADA para 2/2026"
    "No se pueden agregar más clases a una liquidación ya pagada"
```

**⚠️ Problema:** Una vez que pagas, el ciclo está cerrado. No se pueden agregar más clases.

**🔧 Solución:**
1. **Opción A:** Eliminar la liquidación pagada desde la base de datos (requiere soporte técnico)
2. **Opción B:** Registrar la nueva clase como adelanto del mes siguiente
3. **Opción C:** Hacer pago manual fuera del sistema

---

### 🎯 Reglas de Oro del Sistema

```
┌────────────────────────────────────────────────────────────────────┐
│  1️⃣  UNA LIQUIDACIÓN POR PROFESOR POR MES                          │
│     → Solo puede existir una liquidación activa                    │
│                                                                    │
│  2️⃣  ESTADO "CERRADA" = FLEXIBLE                                   │
│     → Puedes seguir agregando clases                               │
│     → Puedes liquidar múltiples veces en el mes                    │
│                                                                    │
│  3️⃣  ESTADO "PAGADA" = FINAL                                       │
│     → NO puedes modificar ni agregar clases                        │
│     → El ciclo está completamente cerrado                          │
│                                                                    │
│  4️⃣  LIQUIDAR ≠ PAGAR                                              │
│     → Son dos pasos separados por diseño                           │
│     → Liquida durante el mes, paga al final                        │
│                                                                    │
│  5️⃣  PAGA SOLO UNA VEZ AL FINAL DEL MES                           │
│     → Espera a tener todas las clases del mes                      │
│     → Registra un solo pago con el total acumulado                 │
└────────────────────────────────────────────────────────────────────┘
```

---

### 📊 Comparación de Estrategias

| Estrategia | Cuándo Liquidar | Cuándo Pagar | Ventajas | Desventajas |
|-----------|-----------------|--------------|----------|-------------|
| **Única al Final** 🟢 **RECOMENDADO** | Día 28-31 (una sola vez) | Inicio mes siguiente | Simple, un solo pago, menos riesgo de error | No tienes control progresivo |
| **Incremental Semanal** 🟡 | Cada semana (4-5 veces) | Inicio mes siguiente | Control progresivo, puedes revisar semana a semana | Más trabajo administrativo |
| **Inmediata** 🔴 **NO RECOMENDADO** | Después de cada clase | Inmediatamente | Pago rápido al profesor | No puedes agregar más clases, múltiples pagos |

---

### ✅ Mejores Prácticas Recomendadas

#### Para Administradores:

1. **Durante el mes (Día 1-27):**
   - ✅ Aprobar clases tan pronto como se completen
   - ✅ Revisar que todas las clases tengan la tarifa correcta
   - ✅ Agregar bonos/descuentos si aplican
   - ❌ NO liquidar todavía (excepto casos especiales)

2. **Fin de mes (Día 28-31):**
   - ✅ Revisar que todas las clases del mes estén aprobadas
   - ✅ Hacer "Liquidar Mes" una sola vez por profesor
   - ✅ Verificar el total calculado
   - ✅ Revisar las observaciones de la liquidación
   - ❌ NO registrar pago todavía

3. **Inicio mes siguiente (Día 1-5):**
   - ✅ Hacer transferencia bancaria
   - ✅ Registrar el pago en el sistema
   - ✅ Verificar que la liquidación cambie a "Pagada"
   - ✅ Informar al profesor que el pago está listo

#### Para Casos Especiales:

**Anticipos o Pagos Urgentes:**
```
Si un profesor necesita un adelanto antes de fin de mes:
1. Liquida solo las clases que vas a pagar
2. Registra el pago parcial
3. Crea una nueva liquidación el resto del mes
   ⚠️ Esto creará múltiples pagos en el mes
```

**Clases Posteriores a la Liquidación:**
```
Si llegan clases DESPUÉS de liquidar pero ANTES de pagar:
1. Vuelve a hacer "Liquidar Mes"
2. El sistema agregará las nuevas clases
3. El total se actualizará automáticamente
✅ Puedes hacerlo cuantas veces necesites mientras no hayas pagado
```

**Error en Liquidación Pagada:**
```
Si pagaste por error y necesitas agregar más clases:
1. Contacta a soporte técnico
2. Se debe eliminar la liquidación pagada desde la base de datos
3. Las clases volverán a estado "Aprobado"
4. Puedes generar una nueva liquidación completa
⚠️ Solo hacer en casos excepcionales
```

---

### 📝 Ejemplo Práctico: Febrero 2026

#### Escenario: Jorge Padilla - Mes Normal

```
📆 Febrero 2 - Clase 1
├─ Clase: Tango Avanzado
├─ Duración: 2 horas
├─ Tarifa: $35,000/hora × 2h = $70,000
└─ Admin aprueba → Estado: "Aprobado" ✅

📆 Febrero 7 - Clase 2
├─ Clase: Tango Principiantes  
├─ Duración: 2 horas
├─ Tarifa: $35,000/hora × 2h = $70,000
├─ Bono: +$20,000 (excelente asistencia)
├─ Total: $90,000
└─ Admin aprueba → Estado: "Aprobado" ✅

📆 Febrero 28 - Liquidación
├─ Admin hace "Liquidar Mes" para Jorge
├─ Sistema encuentra 2 clases aprobadas
├─ Total: $70,000 + $90,000 = $160,000
├─ Liquidación creada → Estado: "Cerrada"
└─ Clases cambian a: "Liquidado" ✅

📆 Marzo 1 - Pago
├─ Admin hace transferencia bancaria de $160,000
├─ Admin registra el pago en el sistema
├─ Liquidación cambia a: "Pagada"
├─ Clases cambian a: "Pagado"
└─ Jorge recibe notificación ✅
```

---

### 🚨 Errores Comunes a Evitar

| ❌ Error | ✅ Correcto | 💡 Por Qué |
|---------|-----------|------------|
| Liquidar y pagar inmediatamente después de cada clase | Liquidar al final del mes, pagar al inicio del siguiente | Permite agregar más clases sin problemas |
| Crear múltiples liquidaciones en el mismo mes | Una sola liquidación que va creciendo | El sistema solo permite una liquidación activa |
| Pagar antes de tener todas las clases | Esperar al final del mes para pagar | Evita tener que eliminar liquidaciones pagadas |
| No revisar el total antes de pagar | Verificar que el monto incluya todas las clases | Previene pagos incorrectos o incompletos |
| Liquidar sin aprobar las clases primero | Aprobar todas las clases antes de liquidar | Solo las clases "Aprobadas" se incluyen en la liquidación |

---

## ⚙️ Prerrequisitos y Configuración {#prerrequisitos}

### 1. Tarifas de Profesores

Las tarifas están preconfiguradas en la base de datos (`TarifasProfesor`):

| Tipo de Profesor | Rol en Clase | Tarifa |
|------------------|--------------|--------|
| **Principal (Titular)** | Principal | **$30,000/hora** |
| Principal (Titular) | Monitor | $10,000/clase |

### 2. Roles en Clases

Cada clase puede tener dos tipos de profesores:

- **🎓 Profesor Principal:** Dicta la clase, recibe tarifa completa por hora
- **👥 Monitor:** Asiste/apoya, recibe tarifa fija por clase

### 3. Acceso al Sistema

**Roles requeridos:**
- **Administrador:** Acceso completo a todos los módulos
- **Profesor:** Solo ve sus propias clases y pagos

---

## 🔄 Proceso Completo: Paso a Paso {#proceso-completo}

### **PASO 1: Crear Clase** 📝

**Módulo:** Clases (`/admin/classes`)  
**Responsable:** Administrador  
**Tiempo:** 2-3 minutos

#### Acciones:

1. Navega a **Clases** en el menú lateral
2. Click en **"+ Crear Clase"**
3. Completa el formulario:

```
📋 Información Básica
├─ Nombre: "Tango Salon"
├─ Tipo: "Tango Salon" (seleccionar del dropdown)
├─ Fecha: 01/02/2026
├─ Hora inicio: 19:00
├─ Hora fin: 20:00
├─ Duración: 1.0 horas (calculado automático)
└─ Cupo máximo: 20 personas

👥 Asignar Profesores
├─ Profesor Principal: Ana Zoraida Gomez (Rol: Principal)
└─ Monitor: (opcional)

💾 Guardar
```

#### ✨ Proceso Automático al Guardar

El sistema automáticamente:

1. **Crea la clase** con estado `"Programada"`
2. **NO crea registros de pago** todavía (esto ocurre al completar)

**⚠️ Nota Importante:** Los registros de pago se crean al marcar la clase como "Completada", no al crearla.

---

### **PASO 2: Completar Clase** ✅

**Módulo:** Clases (`/admin/classes`)  
**Responsable:** Administrador  
**Tiempo:** 30 segundos  
**Cuándo:** Después de que la clase se haya dictado

#### Acciones:

1. En el listado de clases, ubica la clase dictada
2. Click en el botón **"Completar"** (icono de check ✓)
3. Confirma la acción

#### ✨ Proceso Automático al Completar

El sistema ejecuta lo siguiente:

```sql
1. Actualiza la clase:
   └─ Estado: "Programada" → "Completada"

2. Busca la tarifa del profesor:
   └─ TarifasProfesor
      ├─ IdTipoProfesor: Principal
      ├─ IdRolEnClase: Principal
      └─ ValorPorClase: $30,000

3. Calcula el pago:
   └─ TotalPago = $30,000 × 1.0 hora = $30,000

4. Crea registro en ClasesProfesores:
   ├─ IdClase: [GUID de la clase]
   ├─ IdProfesor: A63C58BE-0DDE-4672-9431-119390A04E7E (Ana Zoraida)
   ├─ IdRolEnClase: Principal
   ├─ TarifaProgramada: $30,000
   ├─ ValorAdicional: $0
   ├─ TotalPago: $30,000
   ├─ EstadoPago: "Pendiente"
   └─ FechaCreacion: 2026-02-01
```

**Resultado:**
- ✅ Clase marcada como "Completada"
- ✅ Pago generado en estado "Pendiente"
- ✅ Aparece en el módulo de Nómina para aprobación

---

### **PASO 3: Aprobar Pago** 💰

**Módulo:** Nómina (`/admin/payroll`)  
**Responsable:** Administrador  
**Tiempo:** 1-2 minutos por clase

#### Vista del Dashboard

```
┌──────────────────────────────────────────────────────────────┐
│               📊 ESTADÍSTICAS GLOBALES                       │
│  ┌──────────┬──────────┬──────────┬──────────┐             │
│  │ Clases   │ Clases   │Liquidadas│  Total   │             │
│  │Pendientes│Aprobadas │          │Profesores│             │
│  │    1     │    0     │    0     │    2     │             │
│  │$30,000   │   $0     │   $0     │          │             │
│  └──────────┴──────────┴──────────┴──────────┘             │
└──────────────────────────────────────────────────────────────┘

┌────────────────┬────────────────┬──────────────────────────┐
│  🟡 PENDIENTES │  🟢 APROBADAS  │  👥 PROFESORES           │
│                │                │                          │
│  Tango Salon   │                │  Ana Zoraida Gomez       │
│  01 feb 2026   │                │  ├─ Pendiente: $30,000  │
│                │                │  ├─ Aprobado: $0         │
│  Ana Zoraida   │                │  └─ Liquidado: $0        │
│  Principal     │                │  [Ver detalle →]         │
│  $ 30,000      │                │                          │
│  [Aprobar]     │                │  Jorge Padilla           │
│                │                │  ├─ Pendiente: $0        │
└────────────────┴────────────────┴──────────────────────────┘
```

#### Acciones:

1. En la columna **"Clases Pendientes"** (amarilla), ubica la clase
2. Click en el botón **"Aprobar"**
3. Se abre el **Modal de Aprobación** con 3 opciones:

##### Opción A: Sin Ajuste

```
┌────────────────────────────────────────────────┐
│  🏦 Aprobar Pago                               │
├────────────────────────────────────────────────┤
│  Clase: Tango Salon                            │
│  Profesor: Ana Zoraida Gomez (Principal)       │
│  Fecha: 01 de feb de 2026                      │
│                                                │
│  💵 Tarifa Programada: $ 30,000                │
│                                                │
│  ¿Deseas agregar un ajuste?                    │
│  [✓ Sin Ajuste] [ Bono ] [ Descuento ]        │
│                                                │
│  TOTAL A PAGAR: $ 30,000                       │
│                                                │
│  [Cancelar]  [✓ Aprobar Pago]                 │
└────────────────────────────────────────────────┘
```

##### Opción B: Con Bono

```
┌────────────────────────────────────────────────┐
│  🏦 Aprobar Pago                               │
├────────────────────────────────────────────────┤
│  Clase: Tango Salon                            │
│  Profesor: Ana Zoraida Gomez (Principal)       │
│  Fecha: 01 de feb de 2026                      │
│                                                │
│  💵 Tarifa Programada: $ 30,000                │
│                                                │
│  ¿Deseas agregar un ajuste?                    │
│  [ Sin Ajuste] [✓ Bono ] [ Descuento ]        │
│                                                │
│  💰 Valor del Bono: [ $5,000    ]             │
│  📝 Concepto: [ Excelente asistencia ]         │
│                                                │
│  TOTAL A PAGAR: $ 35,000                       │
│                                                │
│  [Cancelar]  [✓ Aprobar Pago]                 │
└────────────────────────────────────────────────┘
```

##### Opción C: Con Descuento

```
┌────────────────────────────────────────────────┐
│  🏦 Aprobar Pago                               │
├────────────────────────────────────────────────┤
│  Clase: Tango Salon                            │
│  Profesor: Ana Zoraida Gomez (Principal)       │
│  Fecha: 01 de feb de 2026                      │
│                                                │
│  💵 Tarifa Programada: $ 30,000                │
│                                                │
│  ¿Deseas agregar un ajuste?                    │
│  [ Sin Ajuste] [ Bono ] [✓ Descuento ]        │
│                                                │
│  💸 Valor del Descuento: [ $5,000    ]        │
│  📝 Concepto: [ Clase terminó más temprano ]   │
│                                                │
│  TOTAL A PAGAR: $ 25,000                       │
│                                                │
│  [Cancelar]  [✓ Aprobar Pago]                 │
└────────────────────────────────────────────────┘
```

4. Selecciona la opción deseada
5. Click en **"Aprobar Pago"**

#### ✨ Proceso Automático al Aprobar

```sql
UPDATE ClasesProfesores
SET 
  EstadoPago = 'Aprobado',
  ValorAdicional = [valor ingresado], -- Si aplica
  ConceptoAdicional = '[concepto]',   -- Si aplica
  TotalPago = TarifaProgramada + ValorAdicional,
  FechaAprobacion = '2026-02-02',
  AprobadoPorIdUsuario = [GUID del admin]
WHERE IdClaseProfesor = [GUID]
```

**Resultado:**
- ✅ Estado cambia de "Pendiente" → "Aprobado"
- ✅ La clase aparece en la columna **"Clases Aprobadas"** (verde)
- ✅ En el resumen del profesor, el monto aparece en "Aprobado"

---

### **PASO 4: Liquidar Mes** 📅

**Módulo:** Nómina (`/admin/payroll`)  
**Responsable:** Administrador  
**Tiempo:** 1 minuto  
**Cuándo:** Al final del mes o cuando se desee procesar pagos

#### Acciones:

1. Click en el botón **"💵 Liquidar Mes"** (esquina superior derecha)
2. Se abre el **Modal de Liquidación:**

```
┌────────────────────────────────────────────────┐
│  💼 Liquidar Mes de Profesores                 │
├────────────────────────────────────────────────┤
│  Selecciona el periodo a liquidar:             │
│                                                │
│  📅 Mes: [ Febrero ▼ ]                        │
│  📅 Año: [ 2026    ▼ ]                        │
│                                                │
│  📝 Observaciones (opcional):                  │
│  ┌────────────────────────────────────┐       │
│  │ Liquidación mensual febrero 2026   │       │
│  └────────────────────────────────────┘       │
│                                                │
│  ℹ️ Profesores a liquidar: 1                   │
│     Total a liquidar: $ 30,000                 │
│                                                │
│  [Cancelar]  [✓ Liquidar Mes]                 │
└────────────────────────────────────────────────┘
```

3. Selecciona el **mes** y **año**
4. Agrega **observaciones** (opcional)
5. Click en **"Liquidar Mes"**

#### ✨ Proceso Automático al Liquidar

Para cada profesor con clases aprobadas:

```sql
1. Crea registro en LiquidacionesMensuales:
   ├─ IdProfesor: A63C58BE-0DDE-4672-9431-119390A04E7E
   ├─ Mes: 2
   ├─ Año: 2026
   ├─ TotalClases: 1
   ├─ TotalHoras: 1.0
   ├─ TotalPagar: $30,000
   ├─ Estado: "Cerrada"
   ├─ Observaciones: "Liquidación mensual febrero 2026"
   └─ FechaCreacion: 2026-02-02

2. Actualiza todas las clases aprobadas del profesor:
   UPDATE ClasesProfesores
   SET EstadoPago = 'Liquidado'
   WHERE IdProfesor = [GUID]
     AND EstadoPago = 'Aprobado'
     AND MONTH(FechaClase) = 2
     AND YEAR(FechaClase) = 2026
```

**Resultado:**
- ✅ Estado cambia de "Aprobado" → "Liquidado"
- ✅ Se crea una liquidación mensual agrupada
- ✅ Aparece en "Liquidaciones Pendientes de Pago"
- ✅ El profesor ve la liquidación en su perfil

---

### **PASO 5: Registrar Pago** 🏦

**Módulo:** Nómina (`/admin/payroll`)  
**Responsable:** Administrador  
**Tiempo:** 1 minuto  
**Cuándo:** Cuando se realiza el pago real al profesor

#### Vista de Liquidaciones Pendientes

Debajo del Kanban aparece una nueva sección:

```
┌──────────────────────────────────────────────────────────────┐
│  💼 Liquidaciones Pendientes de Pago            [1 pendiente]│
├──────────────────────────────────────────────────────────────┤
│  Profesor         Periodo      Clases  Horas  Monto          │
│  ───────────────────────────────────────────────────────────│
│  Ana Zoraida G.   Feb 2026     1       1.0h   $ 30,000      │
│                                         [🏦 Registrar Pago]  │
└──────────────────────────────────────────────────────────────┘
```

#### Acciones:

1. Ubica la liquidación en la lista
2. Click en **"🏦 Registrar Pago"**
3. Se abre el **Modal de Registro de Pago:**

```
┌────────────────────────────────────────────────┐
│  🏦 Registrar Pago                             │
├────────────────────────────────────────────────┤
│  Confirma que el pago fue realizado            │
│                                                │
│  👤 Profesor: Ana Zoraida Gomez                │
│  📅 Periodo: Febrero 2026                      │
│  💰 Monto a Pagar: $ 30,000                    │
│                                                │
│  📅 Fecha de Pago *                            │
│  [ 02/02/2026    ]                             │
│                                                │
│  📝 Observaciones (opcional)                   │
│  ┌────────────────────────────────────┐       │
│  │ Transferencia bancaria #12345      │       │
│  └────────────────────────────────────┘       │
│                                                │
│  ℹ️ Al registrar este pago, el profesor       │
│     verá $ 30,000 en "Total Pagado"           │
│                                                │
│  [Cancelar]  [💵 Confirmar Pago]              │
└────────────────────────────────────────────────┘
```

4. Confirma o ajusta la **fecha de pago**
5. Agrega **observaciones** (método de pago, referencia, etc.)
6. Click en **"Confirmar Pago"**

#### ✨ Proceso Automático al Registrar

```sql
UPDATE LiquidacionesMensuales
SET 
  Estado = 'Pagada',
  FechaPago = '2026-02-02',
  Observaciones = 'Transferencia bancaria #12345'
WHERE IdLiquidacion = [GUID]

-- Las clases ya tienen EstadoPago = 'Liquidado'
-- No se actualiza el estado individual de clases
```

**Resultado:**
- ✅ Estado de liquidación: "Cerrada" → "Pagada"
- ✅ Liquidación desaparece de "Pendientes de Pago"
- ✅ **El profesor ve en su perfil:**
  - Total Pagado: $30,000
  - Historial: Liquidación de Febrero 2026 - Estado: **Pagada**

---

## 📊 Estados del Sistema {#estados-del-sistema}

### Estados de Clase

| Estado | Descripción | Tabla |
|--------|-------------|-------|
| **Programada** | Clase creada, aún no dictada | `Clases.Estado` |
| **Completada** | Clase dictada, genera registros de pago | `Clases.Estado` |
| **Cancelada** | Clase cancelada (no genera pagos) | `Clases.Estado` |

### Estados de Pago (ClasesProfesores)

| Estado | Descripción | Vista en Nómina | Siguiente Paso |
|--------|-------------|-----------------|----------------|
| **Pendiente** | Clase completada, pago por aprobar | Columna Amarilla | Admin aprueba |
| **Aprobado** | Pago aprobado por admin, listo para liquidar | Columna Verde | Admin liquida mes |
| **Liquidado** | Incluido en liquidación mensual | No visible | Admin registra pago |
| **Pagado** | Pago efectuado al profesor | No visible | Proceso completo |

### Estados de Liquidación (LiquidacionesMensuales)

| Estado | Descripción | Vista en Admin | Vista en Profesor |
|--------|-------------|----------------|-------------------|
| **Cerrada** | Liquidación generada, pago pendiente | Pendientes de Pago | Historial (Cerrada) |
| **Pagada** | Pago registrado y efectuado | No visible | Historial (Pagada) |

### Diagrama de Flujo de Estados

```
CLASE
┌─────────────┐
│ Programada  │ ← Creación
└──────┬──────┘
       │ Completar
       ▼
┌─────────────┐
│ Completada  │ ← Genera ClasesProfesores
└──────┬──────┘
       │
       ▼
PAGO (ClasesProfesores)
┌─────────────┐
│  Pendiente  │ ← Columna Amarilla (Admin)
└──────┬──────┘
       │ Aprobar
       ▼
┌─────────────┐
│  Aprobado   │ ← Columna Verde (Admin)
└──────┬──────┘
       │ Liquidar Mes
       ▼
┌─────────────┐
│  Liquidado  │ ← Ya no aparece en Kanban
└──────┬──────┘
       │
       ▼
LIQUIDACIÓN (LiquidacionesMensuales)
┌─────────────┐
│   Cerrada   │ ← Pendientes de Pago (Admin)
└──────┬──────┘
       │ Registrar Pago
       ▼
┌─────────────┐
│   Pagada    │ ← Total Pagado (Profesor)
└─────────────┘
```

---

## 💡 Ejemplo Práctico Completo {#ejemplo-práctico}

### Caso Real: Clase de Ana Zoraida

Este es un ejemplo real ejecutado en el sistema:

#### 📝 Datos Iniciales

```
Clase: Tango Salon
Profesor: Ana Zoraida Gomez
Rol: Principal
Tipo Profesor: Principal
Tarifa: $30,000/hora
Fecha: 01 de febrero de 2026
Duración: 1.0 hora
```

#### 🔄 Ejecución Paso a Paso

**DÍA 1 - 31 de Enero (Creación)**

```
✅ PASO 1: Crear Clase
├─ Admin crea clase "Tango Salon"
├─ Asigna a Ana Zoraida como Principal
└─ Estado: Programada

BASE DE DATOS:
└─ Clases
   ├─ IdClase: 1BA69284-C3A7-4971-B51D-1532463922C4
   ├─ Nombre: "Tango Salon"
   ├─ Estado: "Programada"
   └─ IdProfesorPrincipal: A63C58BE-...
```

**DÍA 2 - 01 de Febrero (Después de la clase)**

```
✅ PASO 2: Completar Clase
├─ Admin click en "Completar"
└─ Sistema genera registro de pago

BASE DE DATOS:
├─ Clases
│  └─ Estado: "Programada" → "Completada"
│
└─ ClasesProfesores (NUEVO REGISTRO)
   ├─ IdClaseProfesor: 898A17FF-...
   ├─ IdClase: 1BA69284-...
   ├─ IdProfesor: A63C58BE-...
   ├─ IdRolEnClase: FFFFFFFF-...(Principal)
   ├─ TarifaProgramada: $30,000
   ├─ ValorAdicional: $0
   ├─ TotalPago: $30,000
   └─ EstadoPago: "Pendiente" ← APARECE EN NÓMINA
```

**DÍA 3 - 02 de Febrero (Aprobación)**

```
✅ PASO 3: Aprobar Pago
├─ Admin abre Nómina
├─ Ve en columna amarilla: "Tango Salon - $ 30,000"
├─ Click en "Aprobar"
├─ Selecciona "Sin Ajuste"
└─ Confirma

BASE DE DATOS:
└─ ClasesProfesores
   ├─ EstadoPago: "Pendiente" → "Aprobado"
   ├─ FechaAprobacion: 2026-02-02
   └─ AprobadoPorIdUsuario: [GUID admin]

VISTA ADMIN:
└─ La clase se mueve a columna verde "Clases Aprobadas"
```

**DÍA 4 - 02 de Febrero (Liquidación)**

```
✅ PASO 4: Liquidar Mes
├─ Admin click en "Liquidar Mes"
├─ Selecciona: Febrero 2026
├─ Observaciones: "Liquidación mensual febrero 2026"
└─ Confirma

BASE DE DATOS:
├─ LiquidacionesMensuales (NUEVO REGISTRO)
│  ├─ IdLiquidacion: [NUEVO GUID]
│  ├─ IdProfesor: A63C58BE-...
│  ├─ Mes: 2
│  ├─ Año: 2026
│  ├─ TotalClases: 1
│  ├─ TotalHoras: 1.0
│  ├─ TotalPagar: $30,000
│  ├─ Estado: "Cerrada"
│  └─ Observaciones: "Liquidación mensual febrero 2026"
│
└─ ClasesProfesores
   └─ EstadoPago: "Aprobado" → "Liquidado"

VISTA ADMIN:
├─ Columna verde ahora vacía (clase ya liquidada)
└─ Aparece nueva sección: "Liquidaciones Pendientes de Pago"
   └─ Ana Zoraida - Febrero 2026 - $ 30,000 [Registrar Pago]

VISTA PROFESOR:
└─ Ana Zoraida ve en "Mis Pagos":
   ├─ Total Liquidado: $ 30,000
   └─ Historial: Febrero 2026 - Estado: Cerrada
```

**DÍA 5 - 02 de Febrero (Pago)**

```
✅ PASO 5: Registrar Pago
├─ Admin click en "Registrar Pago"
├─ Fecha: 02/02/2026
├─ Observaciones: "Transferencia bancaria #12345"
└─ Confirma

BASE DE DATOS:
└─ LiquidacionesMensuales
   ├─ Estado: "Cerrada" → "Pagada"
   ├─ FechaPago: 2026-02-02
   └─ Observaciones: "Transferencia bancaria #12345"

VISTA ADMIN:
└─ Liquidación desaparece de "Pendientes de Pago"

VISTA PROFESOR:
└─ Ana Zoraida ve en "Mis Pagos":
   ├─ Total Pagado: $ 30,000  ← ACTUALIZADO
   ├─ Total Liquidado: $ 0     ← AHORA CERO
   └─ Historial: Febrero 2026 - Estado: Pagada ✓
      └─ Pagado el: 02 de febrero de 2026
```

---

## 👨‍🏫 Vista del Profesor {#vista-del-profesor}

### Acceso

El profesor accede a `/profesor/payments` (Mis Pagos)

### Dashboard de Pagos del Profesor

```
┌──────────────────────────────────────────────────────────────┐
│  💰 Historial de liquidaciones y pagos mensuales             │
├────────────────────────────────────────────────────────────────┤
│  ┌──────────┬──────────┬──────────┬──────────┐              │
│  │$ 30,000  │   $0     │   $0     │    1     │              │
│  │Total     │Total     │Total     │Clases    │              │
│  │Pagado    │Liquidado │Aprobado  │Pagadas   │              │
│  └──────────┴──────────┴──────────┴──────────┘              │
│                                                              │
│  📋 Historial de Liquidaciones             Año: [2026 ▼]    │
│  ┌────────────────────────────────────────────────────────┐ │
│  │ Periodo      Clases  Horas  Monto       Estado     │ │
│  ├────────────────────────────────────────────────────────┤ │
│  │ Feb 2026     1       1.0h   $ 30,000    ✓ Pagada   👁│ │
│  │ Pagado el 02 de febrero de 2026                     │ │
│  └────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────┘
```

### Estados que ve el Profesor

| Estado en Admin | Estado que ve Profesor | Card de Totales |
|----------------|------------------------|-----------------|
| Pendiente | No visible | - |
| Aprobado | Total Aprobado | Tarjeta Naranja |
| Liquidado | Total Liquidado | Tarjeta Azul |
| Pagado | Total Pagado + Historial | Tarjeta Verde |

---

## ❓ Preguntas Frecuentes {#preguntas-frecuentes}

### ¿Puedo aprobar múltiples clases a la vez?

No, cada clase debe aprobarse individualmente para permitir ajustes personalizados.

### ¿Puedo liquidar solo algunos profesores?

No, al liquidar un mes se procesan todos los profesores con clases aprobadas en ese período.

### ¿Qué pasa si me equivoco al aprobar un pago?

Actualmente no hay función de "desaprobar". Contacta al área de sistemas para revertir.

### ¿El profesor ve los ajustes (bonos/descuentos)?

Sí, en el detalle de la liquidación puede ver el concepto adicional.

### ¿Puedo registrar el pago antes de liquidar?

No, el flujo es secuencial: Aprobar → Liquidar → Registrar Pago.

### ¿Qué pasa si una clase tiene varios profesores?

Cada profesor genera su propio registro de pago según su rol (Principal o Monitor).

---

## 🔧 Solución de Problemas {#solución-de-problemas}

### Problema: No aparece la clase en Nómina

**Causa:** La clase no está marcada como "Completada"

**Solución:**
1. Ve a Clases
2. Busca la clase
3. Click en "Completar"

---

### Problema: No puedo liquidar el mes

**Causa:** No hay clases aprobadas para ese mes

**Solución:**
1. Ve a Nómina
2. Verifica la columna "Clases Aprobadas"
3. Si está vacía, aprueba las clases primero

---

### Problema: El profesor no ve sus pagos

**Causa:** El pago aún no fue registrado

**Solución:**
1. Verifica que completaste el PASO 5 (Registrar Pago)
2. El profesor solo ve pagos con estado "Pagada"

---

### Problema: Error al completar clase

**Causa:** No hay tarifa configurada para ese profesor/rol

**Solución:**
1. Verifica en la tabla `TarifasProfesor`
2. Asegúrate de que existe registro para:
   - IdTipoProfesor del profesor
   - IdRolEnClase asignado

---

## 📞 Soporte

Para problemas técnicos o dudas adicionales:
- **Email:** soporte@chetango.com
- **Teléfono:** +57 XXX XXX XXXX
- **Documentación Técnica:** `/docs`

---

**© 2026 Sistema Chetango - Todos los derechos reservados**

> **Última actualización:** 02 de febrero de 2026  
> **Versión del documento:** 2.0  
> **Próxima revisión:** Marzo 2026
──────────────────────────────
💰 Total a Pagar: $60,000
```
- Click en el botón "Sin Ajuste"
- Confirmar con "Aprobar Pago"
- El sistema guarda:
  ```
  TarifaProgramada = $60,000
  ValorAdicional = $0
  ConceptoAdicional = null
  TotalPago = $60,000
  EstadoPago = "Aprobado"
  ```

**B) Con Bono (+)** 📈
```
🎯 Clase: Tango Salón Avanzado
👤 Profesor: Jorge Padilla (Principal)
📅 Fecha: 5 de febrero 2026

💵 Tarifa Programada: $60,000
➕ Bono: $10,000
   Motivo: "Clase excepcional con 25 alumnos"
──────────────────────────────
💰 Total a Pagar: $70,000
```

Pasos:
1. Click en el botón "Bono" (botón verde con ícono ↗)
2. Ingresar valor del bono: `$10,000`
3. Ingresar motivo (requerido): `"Clase excepcional con 25 alumnos"`
4. El modal calcula automáticamente el total: `$60,000 + $10,000 = $70,000`
5. Click en "Aprobar Pago"

El sistema guarda:
```
TarifaProgramada = $60,000
ValorAdicional = +$10,000
ConceptoAdicional = "Clase excepcional con 25 alumnos"
TotalPago = $70,000
EstadoPago = "Aprobado"
```

**Casos comunes para bonos:**
- ✅ Clase con asistencia excepcional (más de X alumnos)
- ✅ Clase de reemplazo urgente
- ✅ Clase especial (masterclass, evento)
- ✅ Horas extra por extender la clase
- ✅ Bono por desempeño excepcional

**C) Con Descuento (-)** 📉
```
🎯 Clase: Tango Salón Avanzado
👤 Profesor: Jorge Padilla (Principal)
📅 Fecha: 5 de febrero 2026

💵 Tarifa Programada: $60,000
➖ Descuento: $5,000
   Motivo: "Llegó 15 minutos tarde"
──────────────────────────────
💰 Total a Pagar: $55,000
```

Pasos:
1. Click en el botón "Descuento" (botón rojo con ícono ↘)
2. Ingresar valor del descuento: `$5,000`
3. Ingresar motivo (requerido): `"Llegó 15 minutos tarde"`
4. El modal calcula automáticamente el total: `$60,000 - $5,000 = $55,000`
5. Click en "Aprobar Pago"

El sistema guarda:
```
TarifaProgramada = $60,000
ValorAdicional = -$5,000
ConceptoAdicional = "Llegó 15 minutos tarde"
TotalPago = $55,000
EstadoPago = "Aprobado"
```

**Casos comunes para descuentos:**
- ⚠️ Llegó tarde (proporcional a minutos de retraso)
- ⚠️ Se retiró antes de tiempo
- ⚠️ No cumplió con la preparación esperada
- ⚠️ Baja asistencia (menos de X alumnos)

**🎨 Características del Modal:**
- ✅ Interfaz clara con 3 botones visuales (Sin Ajuste, Bono, Descuento)
- ✅ Cálculo en tiempo real del total a pagar
- ✅ Validación de campos requeridos (valor y motivo obligatorios para ajustes)
- ✅ Diseño consistente con el sistema
- ✅ Confirmación clara antes de aprobar

#### **Opción 2: Rechazar/Cancelar**

Si la clase no se dictó realmente:
- El administrador puede eliminar el registro
- O marcar la clase como cancelada
- Esto evita que aparezca en nómina

**⚠️ Validaciones Importantes:**

Antes de aprobar, verificar:
- ✅ La clase realmente se dictó
- ✅ El profesor asistió
- ✅ La duración fue la programada
- ✅ No hubo inconvenientes mayores

#### **3.2 Clases Aprobadas (Columna Verde)**

**Qué se muestra:**
- Clases con `EstadoPago = "Aprobado"`
- Están validadas y esperando liquidación mensual
- Ya no se pueden modificar (salvo revertir aprobación)

**Estado:**
```
✅ Clase ya aprobada para pago
📊 Incluida en próxima liquidación mensual
💰 Monto confirmado: $70,000
```

#### **3.3 Resumen por Profesor (Columna Derecha)**

**Qué se muestra:**
```
👤 Jorge Padilla
📊 15 clases activas

💛 Pendiente: $240,000 (6 clases)
💚 Aprobado: $320,000 (8 clases)
💙 Liquidado: $80,000 (1 clase)

[Ver detalle →]
```

**Acciones:**
- Click en "Ver detalle" para ver todas las clases del profesor
- Filtrar por estado
- Ver historial de pagos

---

### **FASE 4: Liquidación Mensual** 📊

**Ubicación:** `/admin/payroll` (Módulo de Nómina)

**Responsable:** Administrador

**Cuándo:** A fin de mes (típicamente día 28-30)

**Descripción:** La liquidación mensual agrupa todas las clases APROBADAS del mes y genera el documento oficial que se usará para realizar el pago bancario.

**Pasos Detallados:**

#### **4.1 Preparación Pre-Liquidación**

Antes de liquidar, verificar:
1. ✅ **Todas las clases del mes están aprobadas**
   - Revisar la columna "Clases Pendientes"
   - Si quedan clases pendientes, aprobarlas primero
   - Las clases sin aprobar NO se incluirán en la liquidación

2. ✅ **Los ajustes (bonos/descuentos) están correctos**
   - Revisar la columna "Clases Aprobadas"
   - Verificar que los montos sean correctos

#### **4.2 Generar Liquidación con el Modal**

1. **Abrir el Modal:**
   - En la parte superior derecha de la pantalla de Nómina
   - Click en el botón **"Liquidar Mes"** (color azul, ícono 💰)

2. **Seleccionar Período:**
   ```
   📅 Período a Liquidar
   
   Mes: [Febrero ▼]
   Año: [2026]
   ```
   - Por defecto muestra el mes actual
   - Puedes cambiar a otro mes si necesitas liquidar uno anterior

3. **Revisar Preview de Profesores:**
   
   El modal muestra automáticamente:
   ```
   💰 Resumen de Liquidación
   
   ┌─────────────────────────────────────────┐
   │ Jorge Padilla                           │
   │ 12 clases                               │
   │                            $480,000 ✓   │
   ├─────────────────────────────────────────┤
   │ Ana Zoraida Gomez                       │
   │ 8 clases                                │
   │                            $320,000 ✓   │
   ├─────────────────────────────────────────┤
   │ Laura Martínez                          │
   │ 5 clases                                │
   │                            $150,000 ✓   │
   └─────────────────────────────────────────┘
   
   ╔═══════════════════════════════════════╗
   ║ Total Clases:    25                   ║
   ║ Total a Liquidar: $950,000            ║
   ╚═══════════════════════════════════════╝
   ```

   **⚠️ Nota Importante:**
   - Solo se muestran profesores con clases en estado "Aprobado"
   - Si un profesor no aparece, es porque no tiene clases aprobadas
   - Si el modal muestra "No hay clases aprobadas", debes aprobar clases primero

4. **Agregar Observaciones (Opcional):**
   ```
   📝 Observaciones
   
   [Ej: Liquidación de febrero 2026. Incluye bonos 
   por desempeño excepcional en clases especiales.]
   ```
   - Campo de texto libre
   - Aparecerá en el documento de liquidación
   - Útil para notas contables o aclaraciones

5. **Confirmar Liquidación:**
   - Click en el botón **"Generar Liquidación"** (azul)
   - El sistema procesa:
     ```
     ⏳ Generando liquidación para 3 profesores...
     ✓ Jorge Padilla liquidado
     ✓ Ana Zoraida Gomez liquidada
     ✓ Laura Martínez liquidada
     ✅ Liquidación mensual generada exitosamente
     ```

#### **4.3 Proceso Automático del Sistema**

Al confirmar, el sistema ejecuta automáticamente:

```javascript
1. Por cada profesor con clases aprobadas:
   
   Crear LiquidacionMensual {
     IdLiquidacion: [GUID generado]
     IdProfesor: [GUID del profesor]
     Mes: 2
     Año: 2026
     
     // Contadores
     TotalClases: 12          // Clases aprobadas del mes
     TotalHoras: 18           // Suma de horas trabajadas
     
     // Montos
     TotalBase: $480,000      // Suma de TarifaProgramada
     TotalAdicionales: $20,000 // Suma de ValorAdicional (bonos - descuentos)
     TotalPagar: $500,000     // TotalBase + TotalAdicionales
     
     // Estado
     Estado: "Cerrada"        // Liquidación confirmada, no editable
     FechaCierre: 2026-02-28  // Fecha de generación
     Observaciones: "Liquidación de febrero..."
     
     // Auditoría
     CreadoPorIdUsuario: [GUID del admin]
     FechaCreacion: 2026-02-28 10:30:00
   }

2. Actualizar todas las ClaseProfesor del mes:
   
   WHERE EstadoPago = "Aprobado" 
   AND Mes = 2 AND Año = 2026
   
   SET EstadoPago = "Liquidado"
       IdLiquidacion = [GUID de liquidación creada]

3. Refrescar la interfaz:
   - Las clases desaparecen de "Clases Aprobadas" (verde)
   - Los contadores se actualizan
   - El resumen por profesor refleja el cambio
```

#### **4.4 Después de Liquidar**

**Estado de las clases:**
- ✅ Pasan de "Aprobado" → "Liquidado"
- 🔒 Ya no se pueden modificar
- 📊 Están agrupadas en la liquidación mensual

**Documento generado:**
- Se crea un registro de LiquidacionMensual por profesor
- Este documento es el que se usa para hacer el pago bancario
- Contiene el desglose completo de clases y montos

**Siguiente paso:**
- Realizar el pago bancario/efectivo al profesor
- Registrar el pago en el sistema (Fase 5)

---

### **FASE 5: Registro de Pago** 💸

**Ubicación:** `/admin/payroll` → Detalle de Liquidación

**Responsable:** Administrador/Contador

**Pasos:**

1. **Realizar el pago físico:**
   - Transferencia bancaria al profesor
   - Pago en efectivo
   - Cualquier método acordado

2. **Registrar en el sistema:**
   - Abrir la liquidación correspondiente
   - Click en "Registrar Pago"
   - Completar información:
     ```
     Método de Pago: [Transferencia/Efectivo/Cheque]
     Fecha de Pago: 2026-03-05
     Referencia/Comprobante: TR-2026-02-005
     Observaciones: "Pago febrero 2026 - Transferencia Bancolombia"
     ```

3. **Confirmar:**
   - El sistema actualiza:
     ```
     LiquidacionMensual:
       Estado = "Pagada"
       FechaPago = 2026-03-05
     
     ClaseProfesor (todas las del mes):
       EstadoPago = "Pagado"
       FechaPago = 2026-03-05
     ```

4. **Comprobante:**
   - Generar comprobante de pago
   - Enviar al profesor por email
   - Archivar en contabilidad

---

## 🔄 Estados del Pago

### Diagrama de Estados

```
┌──────────────┐
│  PENDIENTE   │ ← Clase creada, no aprobada aún
└──────┬───────┘
       │ Admin aprueba
       ↓
┌──────────────┐
│  APROBADO    │ ← Validado, esperando liquidación mensual
└──────┬───────┘
       │ Liquidación mensual
       ↓
┌──────────────┐
│  LIQUIDADO   │ ← Incluido en liquidación, esperando pago
└──────┬───────┘
       │ Pago realizado
       ↓
┌──────────────┐
│   PAGADO     │ ← Completado, archivado
└──────────────┘
```

### Descripción de Estados

| Estado | Descripción | Puede Modificarse | Acción Siguiente |
|--------|-------------|-------------------|------------------|
| **Pendiente** | Clase realizada, esperando aprobación | ✅ Sí | Aprobar o Rechazar |
| **Aprobado** | Validado por admin, monto confirmado | ⚠️ Solo revertir | Incluir en liquidación |
| **Liquidado** | En liquidación mensual cerrada | ❌ No | Realizar pago físico |
| **Pagado** | Pago completado y registrado | ❌ No | Archivar |

---

## 👥 Roles y Permisos

### Rol: Administrador

**Permisos en Nómina:**
- ✅ Ver todas las clases y pagos
- ✅ Aprobar/rechazar pagos
- ✅ Agregar ajustes (bonos/descuentos)
- ✅ Generar liquidaciones mensuales
- ✅ Registrar pagos realizados
- ✅ Ver historial completo
- ✅ Exportar reportes

### Rol: Profesor

**Permisos en Nómina:**
- ✅ Ver sus propias clases y pagos
- ✅ Ver estado de aprobación
- ✅ Ver liquidaciones mensuales propias
- ❌ NO puede aprobar pagos
- ❌ NO puede modificar montos
- ❌ NO puede ver datos de otros profesores

### Rol: Contador (futuro)

**Permisos en Nómina:**
- ✅ Ver liquidaciones cerradas
- ✅ Registrar pagos
- ✅ Exportar reportes contables
- ❌ NO puede aprobar clases
- ❌ NO puede modificar montos

---

## 💡 Casos de Uso Comunes

### Caso 1: Clase Normal con Un Profesor

**Escenario:**
- Clase: "Tango Salón Nivel 1"
- Profesor: Jorge Padilla (Principal)
- Duración: 1 hora
- Tarifa: $40,000/hora

**Proceso:**
1. Crear clase → Sistema calcula: $40,000
2. Realizar clase → Marcar como realizada
3. Aprobar pago → Sin ajustes, aprobar directo
4. Fin de mes → Liquidar
5. Realizar transferencia → Registrar pago

**Resultado:** Profesor recibe $40,000

---

### Caso 2: Clase con Profesor Principal y Monitor

**Escenario:**
- Clase: "Tango Avanzado - Giros Complejos"
- Profesor Principal: Jorge Padilla
- Monitor: Laura Martínez
- Duración: 1.5 horas

**Proceso:**
1. Crear clase con 2 profesores:
   - Jorge (Principal) → $40,000 × 1.5h = $60,000
   - Laura (Monitor) → $10,000
2. Realizar clase
3. Aprobar ambos pagos por separado
4. Liquidar mes

**Resultado:**
- Jorge recibe $60,000
- Laura recibe $10,000

---

### Caso 3: Clase con Bono por Excelencia

**Escenario:**
- Clase excepcional con 28 alumnos (capacidad máxima 25)
- Profesor: Ana Zoraida Gomez
- Tarifa base: $40,000

**Proceso:**
1. Crear y realizar clase normalmente
2. Al aprobar, agregar ajuste:
   - ValorAdicional: +$15,000
   - Concepto: "Bono por clase con sobre-cupo (28 alumnos)"
3. Aprobar con ajuste

**Resultado:** Profesora recibe $55,000

---

### Caso 4: Clase Cancelada con Penalización

**Escenario:**
- Profesor canceló clase con menos de 24h de anticipación
- Tarifa: $30,000

**Proceso:**
1. Clase marcada como "Cancelada"
2. Admin decide aplicar penalización del 50%
3. Al aprobar:
   - ValorAdicional: -$15,000
   - Concepto: "Cancelación sin aviso (políticas internas)"
4. Aprobar con descuento

**Resultado:** Profesor recibe $15,000 (penalización)

---

### Caso 5: Profesor Ausente - No Aprobar

**Escenario:**
- Clase programada pero profesor no asistió
- Se consiguió reemplazo de emergencia

**Proceso:**
1. Clase aparece en pendientes para profesor original
2. Admin rechaza/elimina el pago del profesor original
3. Admin crea registro manual o ajusta para profesor reemplazo
4. Aprueba solo el pago del reemplazo

**Resultado:** Solo el reemplazo recibe pago

---

## ❓ Preguntas Frecuentes

### Sobre Tarifas

**P: ¿Cómo se calculan las tarifas?**
R: Las tarifas se definen en la tabla `TarifasProfesor` según dos criterios:
- Tipo de Profesor (Principal/Monitor en perfil)
- Rol en la Clase específica (Principal/Monitor asignado)

**P: ¿Puedo cambiar la tarifa de un profesor?**
R: Sí, pero requiere acceso a base de datos o módulo de configuración. Los cambios afectan solo clases futuras.

**P: ¿Las tarifas incluyen impuestos?**
R: No, son valores brutos. Los impuestos se manejan en nómina fiscal externa.

---

### Sobre Aprobaciones

**P: ¿Por qué debo aprobar manualmente cada clase?**
R: Para validar que:
- La clase realmente se dictó
- El profesor cumplió con calidad y puntualidad
- No hubo inconvenientes que ameriten ajustes

**P: ¿Puedo aprobar múltiples clases a la vez?**
R: Actualmente no, pero se puede implementar "Aprobar todas" con precaución.

**P: ¿Puedo revertir una aprobación?**
R: Sí, si aún no se ha liquidado. Una vez liquidado, requiere proceso contable especial.

---

### Sobre Liquidaciones

**P: ¿Cuándo debo generar la liquidación?**
R: Típicamente a fin de mes, después de aprobar todas las clases del período.

**P: ¿Puedo liquidar quincenalmente?**
R: Sí, el sistema permite liquidar por cualquier período. Solo ajusta las fechas.

**P: ¿Qué pasa si olvido incluir una clase en la liquidación?**
R: Esa clase quedará para la siguiente liquidación. Es importante revisar antes de cerrar.

---

### Sobre Pagos

**P: ¿El sistema realiza los pagos automáticamente?**
R: No, el sistema solo registra que el pago se realizó. El pago físico lo hace el administrador.

**P: ¿Puedo pagar antes de liquidar el mes completo?**
R: No recomendado, pero técnicamente sí se puede marcar clases individuales como pagadas.

**P: ¿Cómo demuestro que pagué?**
R: El sistema genera comprobantes con fecha, monto y referencia del pago registrado.

---

### Sobre Ajustes

**P: ¿Cuándo debo usar "ValorAdicional"?**
R: Para:
- Bonos por excelencia o sobre-cupo
- Compensaciones por horas extra
- Descuentos por llegadas tarde
- Penalizaciones por cancelaciones

**P: ¿Hay límite para los ajustes?**
R: No hay límite técnico, pero se recomienda mantener ajustes razonables (<50% tarifa).

**P: ¿Los ajustes se aplican automáticamente en otras clases?**
R: No, cada ajuste es específico para esa clase en particular.

---

## 📊 Reportes Disponibles

### 1. Reporte de Clases Pendientes
- Todas las clases sin aprobar
- Agrupado por profesor
- Ordenado por fecha

### 2. Reporte de Liquidación Mensual
- Resumen por profesor del mes
- Detalle clase por clase
- Total a pagar

### 3. Historial de Pagos
- Todas las liquidaciones cerradas
- Filtrado por profesor, mes, año
- Exportable a Excel/PDF

### 4. Análisis de Tarifas
- Comparativa de tarifas aplicadas
- Promedio por profesor
- Bonos y descuentos otorgados

---

## 🎯 Mejores Prácticas

### ✅ Recomendaciones Generales

1. **Aprobar clases semanalmente** - No acumular muchas pendientes
2. **Documentar ajustes siempre** - Usar campo "ConceptoAdicional"
3. **Revisar antes de liquidar** - Verificar que todo esté aprobado
4. **Comunicar a profesores** - Avisar fechas de pago anticipadamente
5. **Mantener respaldos** - Exportar liquidaciones en PDF/Excel
6. **Revisar estadísticas** - Usar columna de resumen para detectar anomalías

### 💰 Uso Efectivo del Modal de Ajustes

#### **Cuándo usar bonos (+):**
```
✅ USAR BONO cuando:
• Clase con sobre-cupo (más alumnos de lo normal)
• Clase de reemplazo urgente (< 24h aviso)
• Clase especial/evento (masterclass, showcase)
• Extendió la clase voluntariamente
• Desempeño excepcional documentado
• Preparación extra (material didáctico nuevo)
```

**Ejemplos de bonos bien documentados:**
```
✓ "+$15,000 - Clase con 28 alumnos (capacidad máxima 25)"
✓ "+$20,000 - Reemplazo urgente mismo día por emergencia"
✓ "+$10,000 - Preparó coreografía especial para evento"
✓ "+$8,000 - Extendió clase 30 min para práctica adicional"
```

#### **Cuándo usar descuentos (-):**
```
⚠️ USAR DESCUENTO cuando:
• Llegó tarde (proporcional a minutos)
• Salió antes de terminar la clase
• Baja asistencia significativa (< 50% cupo)
• No preparó material prometido
• Incumplimiento de estándares acordados
```

**Ejemplos de descuentos bien documentados:**
```
✓ "-$5,000 - Llegó 15 minutos tarde sin aviso previo"
✓ "-$10,000 - Clase con solo 3 alumnos (cupo mín. 8)"
✓ "-$3,000 - No llevó música preparada, improvisó"
✓ "-$8,000 - Salió 20 min antes por asunto personal"
```

#### **Principios para ajustes justos:**

1. **Transparencia:** El concepto debe ser claro y objetivo
2. **Proporcionalidad:** El ajuste debe ser razonable vs tarifa
3. **Consistencia:** Aplicar criterios similares a todos
4. **Comunicación:** Informar al profesor sobre el ajuste
5. **Documentación:** Guardar evidencia si es necesario

**Rangos sugeridos:**
```
Bonos:
• Pequeño:  $5,000 - $10,000   (10-25% tarifa)
• Medio:    $10,000 - $20,000  (25-50% tarifa)
• Grande:   $20,000+           (50%+ tarifa)

Descuentos:
• Menor:    $3,000 - $5,000    (< 15% tarifa)
• Moderado: $5,000 - $15,000   (15-35% tarifa)
• Mayor:    $15,000+           (35%+ tarifa)
```

### 📅 Flujo Mensual Recomendado

**Semana 1-3 del mes:**
- ✅ Aprobar clases conforme se dictan (diario o cada 2-3 días)
- ✅ Aplicar ajustes inmediatamente mientras está fresca la información
- ✅ Comunicar ajustes significativos al profesor

**Semana 4 (25-28 del mes):**
- ✅ Revisar columna de "Clases Pendientes"
- ✅ Aprobar todas las clases restantes
- ✅ Verificar que no haya errores en montos

**Último día del mes (28-30):**
- ✅ Abrir modal "Liquidar Mes"
- ✅ Revisar el preview de profesores y montos
- ✅ Agregar observaciones del mes si es necesario
- ✅ Generar liquidación oficial

**Primeros días del siguiente mes (1-5):**
- ✅ Realizar transferencias bancarias
- ✅ Registrar pagos en el sistema
- ✅ Archivar comprobantes

### ❌ Errores Comunes a Evitar

1. ❌ **Aprobar sin verificar asistencia** - Validar que la clase se dictó realmente
2. ❌ **Liquidar sin aprobar todo** - Revisar columna pendientes antes de liquidar
3. ❌ **Ajustes sin justificación** - Siempre agregar el concepto/motivo
4. ❌ **Descuentos arbitrarios** - Deben ser objetivos y documentados
5. ❌ **No comunicar ajustes** - Informar al profesor antes de liquidar
6. ❌ **Acumular aprobaciones** - Aprobar semanalmente, no al final del mes
7. ❌ **Cambiar tarifas sin aviso** - Comunicar cambios de tarifa anticipadamente
8. ❌ **Registrar pago sin transferencia** - Solo registrar cuando el dinero fue enviado
9. ❌ **No guardar comprobantes** - Exportar y archivar cada liquidación

---

## 📞 Soporte y Capacitación

Para dudas o capacitación adicional sobre el módulo de nómina:

1. **Documentación Técnica:** Ver `implementacion-modulo-nomina.md`
2. **Videos Tutoriales:** (Por crear)
3. **Soporte Técnico:** Contactar al administrador del sistema

---

**Última actualización:** 1 de Febrero 2026  
**Versión:** 2.0 - Incluye modales interactivos de ajustes y liquidación  
**Módulo:** Nómina de Profesores

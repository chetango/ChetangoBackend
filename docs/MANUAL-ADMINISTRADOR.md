# 📚 MANUAL DEL ADMINISTRADOR - Sistema Chetango

## Guía Completa de Operación Diaria

> **Versión:** 1.0  
> **Fecha:** Febrero 2026  
> **Audiencia:** Administradores del Sistema Chetango

---

## 📋 Tabla de Contenidos

1. [Introducción](#introducción)
2. [Conceptos Fundamentales](#conceptos-fundamentales)
3. [Flujo de Trabajo Completo](#flujo-de-trabajo-completo)
4. [Módulo 1: Gestión de Usuarios](#módulo-1-gestión-de-usuarios)
5. [Módulo 2: Gestión de Paquetes](#módulo-2-gestión-de-paquetes)
6. [Módulo 3: Gestión de Pagos](#módulo-3-gestión-de-pagos)
7. [Módulo 4: Gestión de Clases](#módulo-4-gestión-de-clases)
8. [Módulo 5: Gestión de Asistencias](#módulo-5-gestión-de-asistencias)
9. [Módulo 6: Nómina de Profesores](#módulo-6-nómina-de-profesores)
10. [Reportes y Consultas](#reportes-y-consultas)
11. [Casos Especiales](#casos-especiales)
12. [Preguntas Frecuentes](#preguntas-frecuentes)
13. [Glosario de Términos](#glosario-de-términos)

---

## 🎯 Introducción

### ¿Qué es el Sistema Chetango?

El Sistema Chetango es una plataforma integral para la gestión de una academia de baile (Tango), que permite administrar:

- **Usuarios:** Alumnos, profesores y administradores
- **Paquetes:** Planes de clases que compran los alumnos
- **Pagos:** Registro y seguimiento de transacciones
- **Clases:** Programación de sesiones grupales y privadas
- **Asistencias:** Control de asistencia de alumnos
- **Nómina:** Liquidación y pago a profesores

### Objetivo de este Manual

Este manual te guiará paso a paso en la operación diaria del sistema, desde la creación de un nuevo alumno hasta el pago mensual de profesores. Está diseñado para que entiendas **cómo se relacionan todos los elementos** del sistema y **cuál es el orden correcto** de las operaciones.

---

## 🧩 Conceptos Fundamentales

### Tipos de Usuarios

El sistema maneja **tres roles principales**:

#### 1. 👨‍💼 Administrador
- **Tú eres este rol**
- Acceso completo a todos los módulos
- Responsable de toda la operación diaria
- Puede crear, editar y eliminar cualquier información

#### 2. 👨‍🏫 Profesor
- Dicta las clases de tango
- Puede ver sus propias clases y pagos
- Puede registrar asistencias en sus clases
- Recibe pagos mensuales según tarifas configuradas

#### 3. 🧑‍🎓 Alumno
- Asiste a las clases
- Compra paquetes de clases
- Ve su historial de asistencias y pagos
- Puede tener múltiples paquetes simultáneamente

### Elementos Clave del Sistema

#### Paquete de Clases
- Es un **conjunto de clases prepagadas** que compra un alumno
- Ejemplo: "Paquete 8 Clases" = 8 sesiones que el alumno puede usar
- Tiene una **fecha de vencimiento** (vigencia)
- Se **descuenta automáticamente** cuando el alumno asiste a clase
- Puede estar en diferentes estados: Activo, Vencido, Agotado, Congelado

#### Tipos de Paquetes

##### Paquetes Grupales
- Para clases grupales (Tango Salón, Tango Escenario, etc.)
- Ejemplo: 4, 8, 12 clases
- Vigencia típica: 30-120 días
- Precio por clase: $17,500 - $20,000

##### Paquetes Privados (1 Persona)
- Para clases personalizadas individuales
- Precio por clase: $90,000
- Opciones: 1, 4, 8, 12 clases
- Mayor costo, atención 100% personalizada

##### Paquetes Privados (2 Personas/Parejas)
- Para parejas que quieren clases privadas juntos
- Precio total: $140,000 por clase
- **Importante:** Se crean **2 paquetes individuales** (uno por alumno)
- Ambos paquetes se vinculan al mismo pago

#### Estados de Paquetes

| Estado | Descripción | ¿Se puede usar? |
|--------|-------------|-----------------|
| **Activo (1)** | Tiene clases disponibles y no está vencido | ✅ Sí |
| **Vencido (2)** | Pasó su fecha de vencimiento | ❌ No |
| **Congelado (3)** | Pausado temporalmente por el admin | ❌ No |
| **Agotado (4)** | Usó todas sus clases disponibles | ❌ No |

### Relación entre Elementos

```
ALUMNO
  ↓
PAGO (Registra transacción)
  ↓
PAQUETE(S) (Se generan automáticamente)
  ↓
CLASE (Se programa con fecha/hora)
  ↓
ASISTENCIA (Se registra + se descuenta clase del paquete)
```

---

## 🔄 Flujo de Trabajo Completo

### Vista General del Proceso

Este es el **orden correcto** de operaciones que debes seguir:

```
1. CREAR USUARIO (ALUMNO)
   ↓
2. REGISTRAR PAGO
   ↓
3. ASIGNAR PAQUETE(S) [Automático al registrar pago]
   ↓
4. CREAR CLASE
   ↓
5. REGISTRAR ASISTENCIA [Descuenta 1 clase del paquete]
   ↓
6. COMPLETAR CLASE [Genera pago pendiente para profesor]
   ↓
7. APROBAR PAGO DEL PROFESOR
   ↓
8. LIQUIDAR MES [Fin de mes]
   ↓
9. REGISTRAR PAGO A PROFESOR [Inicio del siguiente mes]
```

### ¿Por Qué Este Orden?

- **No puedes registrar un pago** sin antes tener un alumno creado
- **No puedes registrar asistencia** sin antes tener una clase programada
- **No puedes descontar un paquete** sin antes tener un paquete activo
- **No puedes pagar a un profesor** sin antes aprobar y liquidar sus clases

Ahora veamos cada módulo en detalle.

---

## 👥 Módulo 1: Gestión de Usuarios

### 1.1 Crear un Nuevo Alumno

#### ¿Cuándo hacerlo?
- Cuando llega un nuevo cliente a la academia
- **ANTES** de registrar su primer pago

#### Pasos:

1. **Navega al módulo de Usuarios**
   - Menu lateral → `Usuarios`

2. **Click en "Crear Usuario"**

3. **Completa los datos básicos:**
   ```
   📋 DATOS PERSONALES
   ├─ Nombre completo: "María López"
   ├─ Tipo de documento: "Cédula de Ciudadanía"
   ├─ Número documento: "1234567890"
   ├─ Correo electrónico: "maria.lopez@correo.com"
   ├─ Teléfono: "+57 300 123 4567"
   └─ Rol: "Alumno" ⬅️ IMPORTANTE
   ```

4. **Datos específicos de alumno (opcional):**
   ```
   👨‍👩‍👧 CONTACTO DE EMERGENCIA
   ├─ Nombre: "Pedro López"
   ├─ Teléfono: "+57 300 987 6543"
   ├─ Relación: "Esposo"
   └─ Observaciones médicas: (si aplica)
   ```

5. **Credenciales de Azure (autogeneradas):**
   ```
   🔐 ACCESO AL SISTEMA
   ├─ Correo Azure: maria.lopez@chetango.com
   ├─ Contraseña temporal: [Se genera automática]
   ├─ ☑️ Enviar WhatsApp: Sí
   └─ ☑️ Enviar Email: Sí
   ```

6. **Click en "Guardar"**

#### ¿Qué pasa cuando lo guardas?

El sistema automáticamente:
- ✅ Crea el usuario en la base de datos
- ✅ Crea el perfil de Alumno vinculado
- ✅ Asigna el estado "Activo"
- ✅ Genera credenciales en Azure Active Directory
- ✅ Envía notificaciones por WhatsApp/Email (si activaste la opción)
- ✅ El alumno ya puede iniciar sesión en su portal

#### ⚠️ Importante
- El **correo electrónico debe ser único** en el sistema
- El **número de documento debe ser único**
- Si el sistema dice que ya existe, busca primero el usuario antes de crear uno duplicado

---

### 1.2 Crear un Nuevo Profesor

#### ¿Cuándo hacerlo?
- Cuando contratas un nuevo profesor
- **ANTES** de asignarle clases

#### Pasos:

1. **Menu lateral → Usuarios → Crear Usuario**

2. **Completa los datos básicos:**
   ```
   📋 DATOS PERSONALES
   ├─ Nombre: "Jorge Padilla"
   ├─ Documento: "CC - 1017141203"
   ├─ Correo: "jorge.padilla@chetango.com"
   ├─ Teléfono: "+57 310 171 4120"
   └─ Rol: "Profesor" ⬅️ IMPORTANTE
   ```

3. **Datos específicos del profesor:**
   ```
   👨‍🏫 INFORMACIÓN PROFESIONAL
   ├─ Tipo profesor: "Principal (Titular)"
   ├─ Fecha ingreso: "01/02/2026"
   ├─ Tarifa base: $30,000/hora
   ├─ Biografía: "Profesor con 10 años de experiencia..."
   └─ Especialidades: [Seleccionar]
      ├─ ☑️ Tango Salón
      ├─ ☑️ Tango Escenario
      └─ ☐ Clases Privadas
   ```

4. **Click en "Guardar"**

#### ¿Qué pasa cuando lo guardas?

- ✅ Crea el usuario base
- ✅ Crea el perfil de Profesor
- ✅ Configura sus tarifas automáticamente
- ✅ El profesor puede iniciar sesión y ver su portal

#### Tipos de Profesores

| Tipo | Rol en Clase | Tarifa | Descripción |
|------|--------------|--------|-------------|
| **Principal (Titular)** | Principal | $30,000/hora | Dicta la clase completa |
| Principal | Monitor | $10,000/clase | Asiste/apoya en otra clase |

---

### 1.3 Editar un Usuario

#### Pasos:
1. Menu → Usuarios
2. Busca el usuario (por nombre o documento)
3. Click en el botón de editar (ícono lápiz)
4. Modifica los campos necesarios
5. Guardar

#### ⚠️ Lo que NO puedes editar:
- El número de documento (es el identificador único)
- El rol (no puedes convertir un alumno en profesor directamente)

---

### 1.4 Desactivar un Usuario

#### ¿Cuándo hacerlo?
- Cuando un alumno se retira de la academia
- Cuando un profesor deja de trabajar
- **NO elimines usuarios**, mejor desactívalos

#### Pasos:
1. Menu → Usuarios
2. Busca el usuario
3. Click en el botón de acciones (⋮)
4. Seleccionar "Desactivar"
5. Confirmar

#### ¿Qué pasa?
- ❌ El usuario no puede iniciar sesión
- ✅ Se mantiene todo su historial (pagos, clases, asistencias)
- ✅ No aparece en los dropdowns de selección
- ✅ Puedes reactivarlo cuando quieras

---

## 📦 Módulo 2: Gestión de Paquetes

### 2.1 ¿Cómo se Crean los Paquetes?

#### ⚠️ Importante: Los Paquetes NO se Crean Manualmente

Los paquetes se crean **automáticamente** cuando registras un pago. 

**Flujo correcto:**
```
REGISTRAR PAGO → Seleccionar tipo(s) de paquete → SISTEMA CREA PAQUETE(S)
```

> Ver [Módulo 3: Gestión de Pagos](#módulo-3-gestión-de-pagos) para el proceso completo

### 2.2 Estados y Ciclo de Vida de un Paquete

#### Estado Inicial: Activo
```
Cuando se crea:
├─ Estado: Activo (1)
├─ Clases disponibles: 8 (ejemplo)
├─ Clases usadas: 0
├─ Fecha activación: HOY
└─ Fecha vencimiento: HOY + vigencia (ej: 30 días)
```

#### Transiciones Automáticas

```
ACTIVO (1)
   ↓ [Alumno asiste a clase]
   ├─ ClasesUsadas++
   ├─ Si ClasesUsadas >= ClasesDisponibles → AGOTADO (4)
   └─ Si FechaVencimiento < HOY → VENCIDO (2)
   
ACTIVO (1)
   ↓ [Admin congela el paquete]
CONGELADO (3)
   ↓ [Admin descongela]
ACTIVO (1)
```

### 2.3 Consultar Paquetes de un Alumno

#### Pasos:
1. Menu → Paquetes
2. **Filtrar por alumno:**
   - Buscar por nombre o documento
   - O seleccionar del dropdown
3. Ver lista de paquetes

#### Información que verás:

```
📦 Paquete #1
├─ Tipo: "Paquete 8 Clases - Tango Salón"
├─ Estado: 🟢 Activo
├─ Clases disponibles: 8
├─ Clases usadas: 3
├─ Clases restantes: 5
├─ Fecha activación: 01/02/2026
├─ Fecha vencimiento: 03/03/2026
├─ Días restantes: 15
└─ Valor: $140,000
```

### 2.4 Congelar un Paquete

#### ¿Cuándo hacerlo?
- El alumno viaja y no puede asistir
- Problemas de salud temporales
- Vacaciones prolongadas
- El alumno lo solicita

#### ⚠️ Condiciones:
- Solo se pueden congelar paquetes **Activos**
- No puedes congelar un paquete **Vencido**, **Agotado** o ya **Congelado**

#### Pasos:

1. **Menu → Paquetes**
2. **Buscar el paquete del alumno**
3. **Click en "Congelar"**
4. **Completar formulario:**
   ```
   ⏸️ CONGELAR PAQUETE
   ├─ Fecha inicio: "10/02/2026" (puede ser hoy o futura)
   ├─ Fecha fin: "20/02/2026"
   ├─ Días a congelar: 10 días (calculado)
   └─ Motivo: "Viaje de vacaciones" (opcional)
   ```
5. **Click en "Confirmar"**

#### ¿Qué pasa cuando congelas?

- ✅ Estado cambia a "Congelado" (3)
- ✅ Se crea un registro en `CongelacionesPaquete`
- ✅ El paquete **NO se puede usar** para registrar asistencias
- ⏳ La fecha de vencimiento **NO se modifica todavía** (se ajusta al descongelar)

#### ⚠️ Importante
- **No puedes tener congelaciones solapadas** (fechas que se cruzan)
- La fecha inicio debe ser >= HOY
- La fecha fin debe ser > fecha inicio

---

### 2.5 Descongelar un Paquete

#### ¿Cuándo hacerlo?
- El alumno regresó y puede volver a clases
- Terminó el período de congelación
- El alumno lo solicita

#### Pasos:

1. **Menu → Paquetes**
2. **Buscar el paquete congelado**
3. **Click en "Descongelar"**
4. **Seleccionar la congelación activa** (si hay múltiples)
5. **Click en "Confirmar"**

#### ¿Qué pasa cuando descongelas?

```
PROCESO AUTOMÁTICO:

1. Actualiza FechaFin de la congelación = HOY
2. Calcula días congelados = (FechaFin - FechaInicio).Days
3. Extiende vencimiento:
   └─ FechaVencimiento += días congelados
4. Recalcula el estado:
   ├─ Si ClasesUsadas >= ClasesDisponibles → Agotado (4)
   ├─ Si FechaVencimiento < HOY → Vencido (2)
   └─ Si tiene clases disponibles → Activo (1)
```

#### Ejemplo Real:

```
Paquete Original:
├─ Fecha vencimiento: 01/03/2026
└─ Clases restantes: 5

Congelación:
├─ Inicio: 10/02/2026
└─ Fin: 20/02/2026
└─ Total: 10 días

Después de descongelar:
├─ Nueva fecha vencimiento: 11/03/2026 (01/03 + 10 días)
├─ Estado: Activo
└─ Clases restantes: 5 (no cambian)
```

---

### 2.6 Reportes de Paquetes

#### Paquetes Activos
- Menu → Paquetes
- Filtro Estado: "Activo"
- Muestra todos los paquetes que se pueden usar

#### Paquetes Por Vencer
- Filtro: "Próximos a vencer"
- Muestra paquetes que vencen en los próximos 7 días
- **Recomendación:** Revisar diariamente y contactar alumnos

#### Paquetes Vencidos
- Filtro Estado: "Vencido"
- Paquetes que ya pasaron su fecha de vigencia
- **Acción:** Contactar alumno para renovación

#### Paquetes Agotados
- Filtro Estado: "Agotado"
- Paquetes que ya usaron todas sus clases
- **Acción:** Ofrecer nuevo paquete al alumno

---

## 💰 Módulo 3: Gestión de Pagos

### 3.1 Registrar un Pago (Flujo Completo)

#### ¿Cuándo hacerlo?
- Un alumno compra un paquete de clases
- Un alumno renueva su paquete
- Pagos de inscripción

#### Prerrequisitos:
- ✅ El alumno debe existir en el sistema
- ✅ Debes conocer el tipo de paquete que compra

---

### 3.2 CASO 1: Paquete Individual (1 Alumno - Clases Grupales)

#### Ejemplo: María López compra "Paquete 8 Clases"

#### Pasos:

1. **Menu → Pagos → Registrar Nuevo Pago**

2. **Seleccionar el alumno:**
   ```
   🧑 ALUMNO
   └─ Buscar: "María López"
      └─ Seleccionar: María López - CC 1234567890
   ```

3. **Información del pago:**
   ```
   💵 DATOS DEL PAGO
   ├─ Fecha de pago: "01/02/2026" (por defecto: HOY)
   ├─ Método de pago: [Seleccionar del dropdown]
   │  ├─ Efectivo
   │  ├─ Transferencia Bancaria
   │  ├─ Tarjeta Crédito
   │  ├─ Tarjeta Débito
   │  └─ Nequi/Daviplata
   ├─ Referencia: "TRF-12345" (si aplica)
   └─ Nota: "Pago paquete 8 clases" (opcional)
   ```

4. **Asignar paquete(s):**
   ```
   📦 PAQUETES A ASIGNAR
   
   Paquete #1:
   ├─ Tipo paquete: "Paquete 8 Clases - Tango Salón"
   ├─ Precio sugerido: $140,000
   ├─ Precio ajustado: $140,000 (editable)
   └─ Observaciones: (opcional)
   ```

5. **Resumen:**
   ```
   💳 RESUMEN
   ├─ Subtotal paquetes: $140,000
   ├─ Descuentos: $0
   └─ TOTAL A PAGAR: $140,000
   ```

6. **Click en "Registrar Pago"**

#### ¿Qué pasa cuando guardas?

```
PROCESO AUTOMÁTICO:

1. CREA PAGO:
   ├─ IdPago: [GUID generado]
   ├─ IdAlumno: María López
   ├─ FechaPago: 01/02/2026
   ├─ MontoTotal: $140,000
   ├─ MetodoPago: "Transferencia Bancaria"
   └─ Estado: "Completado"

2. CREA PAQUETE:
   ├─ IdPaquete: [GUID generado]
   ├─ IdAlumno: María López
   ├─ IdPago: [Vinculado al pago anterior]
   ├─ IdTipoPaquete: "Paquete 8 Clases"
   ├─ ClasesDisponibles: 8
   ├─ ClasesUsadas: 0
   ├─ FechaActivacion: 01/02/2026 (HOY)
   ├─ FechaVencimiento: 03/03/2026 (HOY + 30 días)
   ├─ Estado: Activo (1)
   └─ ValorPaquete: $140,000

3. NOTIFICA:
   └─ Email al alumno con confirmación de pago
```

---

### 3.3 CASO 2: Paquete para Pareja (2 Alumnos - Clase Privada)

#### Ejemplo: Juan y María (pareja) compran "Privada 2P - 4 Clases"

#### ⚠️ IMPORTANTE: Cómo Funciona

Las clases privadas para parejas funcionan así:
- Se hace **UN SOLO PAGO** (el total para ambos)
- Se crean **DOS PAQUETES** (uno por cada alumno)
- Cada alumno usa su paquete de forma independiente
- Ambos paquetes quedan vinculados al mismo `IdPago`

#### Pasos:

##### PASO 1: Registrar Pago para el PRIMER Alumno (Juan)

1. **Menu → Pagos → Registrar Nuevo Pago**

2. **Seleccionar alumno 1:**
   ```
   🧑 ALUMNO
   └─ Seleccionar: Juan Pérez
   ```

3. **Datos del pago:**
   ```
   💵 DATOS DEL PAGO
   ├─ Fecha: 01/02/2026
   ├─ Método: Transferencia Bancaria
   └─ Referencia: "TRF-PAREJA-001"
   ```

4. **Asignar paquete:**
   ```
   📦 PAQUETE
   ├─ Tipo: "Privada 2 Personas - 4 Clases"
   ├─ Precio: $560,000 ⬅️ TOTAL PARA AMBOS
   └─ Nota: "Pago completo - Pareja Juan y María"
   ```

5. **Click en "Registrar Pago"**

**Resultado:**
```
✅ Pago registrado: $560,000
✅ Paquete creado para Juan: 4 clases privadas
```

##### PASO 2: Registrar Pago para el SEGUNDO Alumno (María)

6. **Menu → Pagos → Registrar Nuevo Pago** (de nuevo)

7. **Seleccionar alumno 2:**
   ```
   🧑 ALUMNO
   └─ Seleccionar: María López
   ```

8. **Datos del pago:**
   ```
   💵 DATOS DEL PAGO
   ├─ Fecha: 01/02/2026 (MISMA FECHA)
   ├─ Método: Transferencia Bancaria (MISMO MÉTODO)
   └─ Referencia: "TRF-PAREJA-001" (MISMA REFERENCIA)
   ```

9. **Asignar paquete:**
   ```
   📦 PAQUETE
   ├─ Tipo: "Privada 2 Personas - 4 Clases"
   ├─ Precio sugerido: $560,000
   ├─ ⚠️ EDITAR PRECIO A: $0 ⬅️ IMPORTANTE
   └─ Nota: "Vinculado a pago de Juan Pérez - Pareja"
   ```

10. **Click en "Registrar Pago"**

**Resultado Final:**
```
✅ Pago 1: Juan - $560,000
   └─ Paquete Juan: 4 clases privadas

✅ Pago 2: María - $0 (vinculado)
   └─ Paquete María: 4 clases privadas

📊 TOTAL COBRADO: $560,000 (correcto)
📦 PAQUETES CREADOS: 2 (uno por alumno)
```

#### ¿Por Qué Este Proceso?

- ✅ Cada alumno tiene su propio historial de asistencias
- ✅ Flexibilidad si uno de los dos no puede asistir
- ✅ Reportes más claros por alumno
- ✅ Evita problemas de sincronización
- ✅ Puedes ver cuántas clases usó cada uno

#### ⚠️ Errores Comunes a Evitar

❌ **ERROR:** Crear dos pagos de $560,000 cada uno
```
Total cobrado: $1,120,000 ❌ (cobrarías el doble)
```

✅ **CORRECTO:** Un pago de $560,000 + un pago de $0 vinculado
```
Total cobrado: $560,000 ✅ (correcto)
```

---

### 3.4 Tarifas de Paquetes (Referencia)

#### Paquetes Grupales

| Paquete | Clases | Precio | Precio/Clase | Vigencia |
|---------|--------|--------|--------------|----------|
| Paquete 4 Clases | 4 | $80,000 | $20,000 | 30 días |
| Paquete 8 Clases | 8 | $140,000 | $17,500 | 60 días |
| Paquete 12 Clases | 12 | $200,000 | $16,667 | 90 días |

#### Paquetes Privados (1 Persona)

| Paquete | Clases | Precio | Precio/Clase | Vigencia |
|---------|--------|--------|--------------|----------|
| Privada 1P - 1 Clase | 1 | $90,000 | $90,000 | 30 días |
| Privada 1P - 4 Clases | 4 | $360,000 | $90,000 | 60 días |
| Privada 1P - 8 Clases | 8 | $720,000 | $90,000 | 90 días |
| Privada 1P - 12 Clases | 12 | $1,080,000 | $90,000 | 120 días |

#### Paquetes Privados (2 Personas/Parejas)

| Paquete | Clases | Precio TOTAL | Precio/Clase | Vigencia |
|---------|--------|--------------|--------------|----------|
| Privada 2P - 1 Clase | 1 | $140,000 | $140,000 | 30 días |
| Privada 2P - 4 Clases | 4 | $560,000 | $140,000 | 60 días |
| Privada 2P - 8 Clases | 8 | $1,120,000 | $140,000 | 90 días |
| Privada 2P - 12 Clases | 12 | $1,680,000 | $140,000 | 120 días |

> **Nota:** Los precios son sugeridos y **editables** al momento de registrar el pago

---

### 3.5 Aplicar Descuentos

#### ¿Cómo aplicar un descuento?

1. Al registrar el pago, después de seleccionar el tipo de paquete:
2. **Editar el precio sugerido:**
   ```
   Precio sugerido: $140,000
   Descuento: -$20,000
   Precio final: $120,000 ⬅️ Editar aquí
   ```
3. **Agregar observación:**
   ```
   Observaciones: "Descuento 15% - Promoción Febrero"
   ```

#### Tipos de Descuentos Comunes

- **Referidos:** 10-15% de descuento
- **Promociones temporales:** Descuentos por temporada
- **Inscripción múltiple:** Descuento por comprar varios paquetes
- **Alumnos antiguos:** Bonificaciones por fidelidad

---

### 3.6 Consultar Historial de Pagos

#### Ver pagos de un alumno:

1. **Menu → Pagos**
2. **Filtro: Por Alumno**
   - Buscar por nombre o documento
3. **Ver lista de pagos:**

```
📋 HISTORIAL DE PAGOS - María López

Pago #1 - 01/02/2026
├─ Monto: $140,000
├─ Método: Transferencia
├─ Estado: Completado
└─ Paquetes generados:
   └─ Paquete 8 Clases (ID: xxx)

Pago #2 - 15/03/2026
├─ Monto: $200,000
├─ Método: Efectivo
├─ Estado: Completado
└─ Paquetes generados:
   └─ Paquete 12 Clases (ID: yyy)
```

#### Ver detalle de un pago:

- Click en el pago
- Verás:
  - Fecha y hora exacta
  - Método de pago y referencia
  - Monto total
  - Paquetes generados (con su estado actual)
  - Usuario que registró el pago

---

### 3.7 Estadísticas de Pagos (Solo Admin)

#### Menu → Pagos → Estadísticas

```
📊 ESTADÍSTICAS DEL MES

💰 Ingresos
├─ Total recaudado: $4,500,000
├─ Cantidad de pagos: 25
└─ Promedio por pago: $180,000

📈 Por Método de Pago
├─ Transferencia: $2,800,000 (62%)
├─ Efectivo: $1,200,000 (27%)
└─ Tarjeta: $500,000 (11%)

📦 Por Tipo de Paquete
├─ Paquete 8 Clases: 15 vendidos
├─ Paquete 12 Clases: 8 vendidos
└─ Paquete 4 Clases: 2 vendidos
```

---

## 📅 Módulo 4: Gestión de Clases

### 4.1 Tipos de Clases

#### Clases Grupales
- **Tango Salón:** Nivel básico/intermedio
- **Tango Escenario:** Nivel avanzado, técnica de escenario
- **Elencos Formativos:** Grupos de práctica

#### Clases Privadas
- **Tango Salón Privado:** Sesión individual o pareja
- **Tango Escenario Privado:** Técnica personalizada

---

### 4.2 Crear una Clase Grupal

#### ¿Cuándo hacerlo?
- Para programar las sesiones semanales
- Con anticipación (recomendado: al menos 2-3 días)

#### Pasos:

1. **Menu → Clases → Crear Nueva Clase**

2. **Información básica:**
   ```
   📋 DATOS DE LA CLASE
   ├─ Tipo clase: "Tango Salon" [Dropdown]
   ├─ Fecha: "05/02/2026"
   ├─ Hora inicio: "19:00"
   ├─ Hora fin: "20:30"
   ├─ Duración: 1.5 horas (calculado automático)
   └─ Cupo máximo: 20 personas
   ```

3. **Asignar profesor(es):**
   ```
   👨‍🏫 PROFESORES
   
   Profesor Principal:
   ├─ Nombre: Jorge Padilla
   ├─ Rol: Principal
   └─ Tarifa: $30,000/hora × 1.5h = $45,000
   
   Monitor (opcional):
   ├─ Nombre: Ana Zoraida Gomez
   ├─ Rol: Monitor
   └─ Tarifa: $10,000/clase (fija)
   ```

4. **Observaciones (opcional):**
   ```
   📝 OBSERVACIONES
   └─ "Clase enfocada en técnica de abrazo"
   ```

5. **Click en "Crear Clase"**

#### ¿Qué pasa cuando la creas?

```
CLASE CREADA:
├─ IdClase: [GUID generado]
├─ Estado: "Programada"
├─ Profesores asignados con sus tarifas
├─ Visible en el calendario
├─ Alumnos pueden verla en su portal
└─ ⚠️ NO se genera pago al profesor todavía
   (se genera al completar la clase)
```

#### ⚠️ Validaciones Automáticas

El sistema valida:
- ❌ No puede haber otra clase del mismo profesor en el mismo horario
- ❌ La hora fin debe ser mayor que la hora inicio
- ❌ La fecha debe ser futura (no puedes crear clases en el pasado)
- ❌ El tipo de clase debe existir

---

### 4.3 Crear una Clase Privada

#### Diferencias con Clase Grupal:
- Cupo máximo: 1 o 2 (según sea individual o pareja)
- Solo alumnos con paquetes privados pueden asistir
- Usualmente se agenda con el alumno específico

#### Pasos:

1. **Menu → Clases → Crear Nueva Clase**

2. **Información básica:**
   ```
   📋 DATOS DE LA CLASE PRIVADA
   ├─ Tipo clase: "Tango Salon Privado" o "Tango Escenario Privado"
   ├─ Fecha: "06/02/2026"
   ├─ Hora inicio: "16:00"
   ├─ Hora fin: "17:00"
   ├─ Duración: 1.0 hora
   └─ Cupo máximo: 1 (individual) o 2 (pareja)
   ```

3. **Asignar profesor(es):**
   ```
   👨‍🏫 PROFESORES
   
   Profesor Principal:
   ├─ Nombre: Jorge Padilla
   ├─ Rol: Principal
   └─ Tarifa: $30,000/hora × 1.0h = $30,000
   
   Monitor (opcional):
   ├─ Puede agregar segundo profesor si es pareja
   └─ Tarifa: $10,000/clase
   ```

4. **Observaciones:**
   ```
   📝 OBSERVACIONES
   └─ "Clase privada para Juan y María - Enfoque en giros"
   ```

5. **Click en "Crear Clase"**

---

### 4.4 Editar una Clase

#### ¿Cuándo hacerlo?
- Cambio de horario
- Cambio de profesor
- Ajustar cupo máximo
- Solo antes de que inicie la clase

#### ⚠️ Restricciones:
- No puedes editar clases que ya están "Completadas"
- Solo admin o el profesor dueño pueden editar

#### Pasos:

1. Menu → Clases
2. Buscar la clase
3. Click en botón editar (ícono lápiz)
4. Modificar los campos necesarios
5. Guardar

---

### 4.5 Cancelar una Clase

#### ¿Cuándo hacerlo?
- El profesor no puede asistir
- Feriado o evento especial
- Problemas de infraestructura

#### Pasos:

1. Menu → Clases
2. Buscar la clase
3. Click en botón cancelar (ícono X)
4. **Escribir motivo:**
   ```
   Motivo de cancelación:
   "Profesor con incapacidad médica"
   ```
5. Confirmar

#### ¿Qué pasa cuando cancelas?

- ✅ Estado cambia a "Cancelada"
- ✅ Los alumnos son notificados
- ✅ NO se descuentan clases de paquetes
- ✅ NO se genera pago al profesor
- ✅ La clase sigue visible en el historial (para auditoría)

---

### 4.6 Completar una Clase

#### ¿Cuándo hacerlo?
- **Después de que la clase termine**
- Antes de registrar asistencias (recomendado)

#### Pasos:

1. Menu → Clases
2. Buscar la clase del día
3. Click en botón "Completar" (✓)
4. Confirmar

#### ¿Qué pasa cuando completas?

```
PROCESO AUTOMÁTICO:

1. ACTUALIZA LA CLASE:
   └─ Estado: "Programada" → "Completada"

2. GENERA REGISTROS DE PAGO PARA PROFESORES:
   
   Para cada profesor asignado:
   ├─ Busca su tarifa configurada
   ├─ Calcula el monto (tarifa × horas)
   ├─ Crea registro en ClasesProfesores:
   │  ├─ IdClase: [ID de la clase]
   │  ├─ IdProfesor: [ID del profesor]
   │  ├─ RolEnClase: "Principal" o "Monitor"
   │  ├─ TarifaProgramada: $30,000 (ejemplo)
   │  ├─ ValorAdicional: $0
   │  ├─ TotalPago: $45,000 (tarifa × 1.5h)
   │  ├─ EstadoPago: "Pendiente" ⬅️ IMPORTANTE
   │  └─ FechaCreacion: HOY
   
   ⚠️ Estado "Pendiente" significa que está listo para aprobarse
```

#### ⚠️ Importante
- **No puedes deshacer** la acción de completar (solo desde BD)
- Al completar, se genera el pago pendiente del profesor
- La clase completada ya está lista para registrar asistencias

---

### 4.7 Ver Calendario de Clases

#### Vista Semanal:
- Menu → Clases → Vista Calendario
- Muestra las clases de la semana

#### Vista Lista:
- Menu → Clases → Vista Lista
- Filtra por:
  - Fecha
  - Profesor
  - Tipo de clase
  - Estado (Programada, Completada, Cancelada)

---

## ✅ Módulo 5: Gestión de Asistencias

### 5.1 ¿Qué es una Asistencia?

Una asistencia es el **registro de que un alumno asistió a una clase específica** y, al mismo tiempo, **se descuenta una clase de su paquete**.

### 5.2 Flujo de Asistencias

```
1. La clase está "Completada" ✅
   ↓
2. Admin registra que Juan asistió
   ↓
3. Selecciona el paquete de Juan a usar
   ↓
4. SISTEMA:
   ├─ Crea registro de Asistencia
   ├─ Estado: "Presente"
   ├─ Paquete usado: Paquete X
   ├─ Descuenta: ClasesUsadas++ en el paquete
   └─ Recalcula estado del paquete
```

---

### 5.3 Registrar Asistencia Manual

#### ¿Cuándo hacerlo?
- Después de que la clase termine
- Cuando tienes la lista de asistentes

#### Prerrequisitos:
- ✅ La clase debe estar "Completada"
- ✅ El alumno debe tener un paquete Activo

#### Pasos:

1. **Menu → Asistencias → Registrar Asistencia**
   - O desde: Menu → Clases → [Seleccionar clase] → Registrar Asistencias

2. **Seleccionar la clase:**
   ```
   📅 CLASE
   ├─ Fecha: 05/02/2026
   ├─ Tipo: Tango Salon
   ├─ Profesor: Jorge Padilla
   └─ Hora: 19:00 - 20:30
   ```

3. **Agregar alumno que asistió:**
   ```
   🧑 ALUMNO #1: María López
   
   Seleccionar paquete a usar:
   ├─ Opción 1: Paquete 8 Clases
   │  ├─ Clases restantes: 6/8
   │  ├─ Vence: 05/03/2026 (28 días restantes)
   │  └─ Estado: ✅ Activo
   │
   ├─ Opción 2: Paquete 12 Clases
   │  ├─ Clases restantes: 10/12
   │  ├─ Vence: 15/03/2026 (38 días restantes)
   │  └─ Estado: ✅ Activo
   │
   └─ ⬇️ Seleccionar: Paquete 8 Clases
   
   Estado: ✅ Presente (por defecto)
   Observaciones: (opcional)
   ```

4. **Repetir para cada alumno que asistió**

5. **Click en "Guardar Asistencias"**

#### ¿Qué pasa cuando guardas?

```
PARA CADA ALUMNO:

1. CREA ASISTENCIA:
   ├─ IdAsistencia: [GUID]
   ├─ IdClase: [ID clase]
   ├─ IdAlumno: María López
   ├─ IdPaqueteUsado: [Paquete seleccionado]
   ├─ TipoAsistencia: "Normal"
   ├─ Estado: "Presente"
   └─ FechaRegistro: HOY

2. ACTUALIZA PAQUETE:
   ├─ ClasesUsadas: 2 → 3
   └─ ClasesRestantes: 6 → 5
   
3. RECALCULA ESTADO DEL PAQUETE:
   ├─ Si ClasesUsadas >= ClasesDisponibles:
   │  └─ Estado: "Agotado" (4)
   ├─ Si FechaVencimiento < HOY:
   │  └─ Estado: "Vencido" (2)
   └─ Si está OK:
      └─ Estado: "Activo" (1)
```

#### ⚠️ ¿Qué pasa si el paquete se agota?

```
Ejemplo:
María tiene un paquete con 1 clase restante (7/8 usadas)

Registras su asistencia:
├─ ClasesUsadas: 7 → 8
├─ ClasesRestantes: 1 → 0
└─ Estado: "Activo" → "Agotado"

Resultado:
✅ Asistencia registrada correctamente
⚠️ Paquete ahora está agotado
📢 Debes contactar a María para renovar
```

---

### 5.4 Tipos de Asistencia

| Tipo | Descripción | ¿Descuenta clase? | Cuándo usarlo |
|------|-------------|-------------------|---------------|
| **Normal** | Asistencia regular con paquete | ✅ Sí | Caso estándar |
| **Cortesía** | Clase gratis promocional | ❌ No | Clase de prueba, promoción |
| **Recuperación** | Reposición de clase perdida | ✅ Sí* | Alumno faltó a clase anterior |
| **Prueba** | Primera clase de prueba | ❌ No | Nuevo alumno probando |

> *Depende de la política de la academia

---

### 5.5 Marcar Ausencias

#### ¿Cuándo hacerlo?
- Un alumno avisó que no puede asistir
- Para llevar registro de inasistencias

#### Pasos:

1. Menu → Asistencias → Registrar Asistencia
2. Seleccionar la clase
3. Agregar el alumno
4. **Cambiar estado:**
   ```
   Estado: Ausente
   Motivo: "Enfermedad" (opcional)
   ```
5. **NO selecciones paquete** (porque no asistió)
6. Guardar

#### ¿Qué pasa?

- ✅ Se crea registro de asistencia con Estado: "Ausente"
- ✅ NO se descuenta clase del paquete
- ✅ Queda registrado para estadísticas

---

### 5.6 Editar una Asistencia

#### ¿Cuándo hacerlo?
- Marcaste al alumno incorrecto
- Usaste el paquete equivocado
- Cambiar estado (Presente → Ausente o viceversa)

#### Pasos:

1. Menu → Asistencias
2. Buscar la asistencia (por clase o alumno)
3. Click en editar
4. Modificar:
   - Estado
   - Paquete usado
   - Observaciones
5. Guardar

#### ⚠️ Al cambiar de paquete:
```
PROCESO:
1. Revierte el descuento del paquete anterior
   └─ ClasesUsadas--
2. Descuenta del nuevo paquete
   └─ ClasesUsadas++
```

---

### 5.7 Ver Historial de Asistencias

#### Por Alumno:
```
Menu → Alumnos → [Seleccionar alumno] → Ver Asistencias

📊 HISTORIAL - María López

Mes Febrero 2026:
├─ Clases asistidas: 6
├─ Clases ausentes: 1
└─ Porcentaje asistencia: 85%

Detalle:
├─ 01/02: Tango Salon ✅ (Paquete A)
├─ 03/02: Tango Escenario ✅ (Paquete A)
├─ 05/02: Tango Salon ❌ Ausente
├─ 08/02: Tango Salon ✅ (Paquete A)
└─ ...
```

#### Por Clase:
```
Menu → Clases → [Seleccionar clase] → Ver Asistencias

📋 ASISTENCIAS - Tango Salon 05/02/2026

Total: 15 alumnos

✅ Presentes (12):
├─ María López (Paquete 8 clases)
├─ Juan Pérez (Paquete 12 clases)
├─ Ana Gómez (Paquete 4 clases)
└─ ...

❌ Ausentes (3):
├─ Pedro Ruiz (avisó)
├─ Laura Torres
└─ Carlos Medina
```

---

### 5.8 Validaciones Importantes

#### ⚠️ No puedes registrar asistencia si:

- ❌ La clase NO está "Completada"
- ❌ El alumno NO tiene paquetes activos
- ❌ El paquete está Vencido/Congelado/Agotado
- ❌ Ya existe una asistencia del alumno en esa clase
- ❌ El tipo de paquete no corresponde (ej: paquete grupal en clase privada)

---

## 💼 Módulo 6: Nómina de Profesores

### 6.1 Conceptos Clave

#### Estados de Pago del Profesor

| Estado | Descripción | Siguiente paso |
|--------|-------------|----------------|
| **Pendiente** | Clase completada, pago calculado | Aprobar |
| **Aprobado** | Admin revisó y aprobó el monto | Liquidar mes |
| **Liquidado** | Incluido en liquidación mensual | Registrar pago |
| **Pagado** | Profesor recibió el dinero | Fin del ciclo |

---

### 6.2 Ciclo Mensual de Nómina (Flujo Recomendado)

```
📅 FASE 1: DURANTE EL MES (Día 1-28)

Día 2:
├─ Jorge dicta clase → Admin completa clase
├─ Sistema genera: Pago Pendiente ($45,000)
└─ Admin APRUEBA el pago
   └─ Estado: "Pendiente" → "Aprobado"

Día 5:
├─ Jorge dicta clase → Admin completa clase
├─ Sistema genera: Pago Pendiente ($45,000)
└─ Admin APRUEBA el pago
   └─ Estado: "Pendiente" → "Aprobado"

Día 10, 15, 20...:
└─ Se repite el proceso para cada clase

⚠️ NO LIQUIDAR NI PAGAR TODAVÍA

---

📅 FASE 2: FIN DE MES (Día 28-31)

Día 28:
├─ Admin hace "Liquidar Mes" para Jorge
├─ Sistema agrupa TODAS las clases "Aprobadas"
├─ Total: $180,000 (4 clases × $45,000)
├─ Crea Liquidación Mensual:
│  ├─ Mes: Febrero 2026
│  ├─ Estado: "Cerrada"
│  └─ Total: $180,000
└─ Todas las clases cambian:
   └─ Estado: "Aprobado" → "Liquidado"

⚠️ NO REGISTRAR PAGO TODAVÍA

---

📅 FASE 3: INICIO MES SIGUIENTE (Día 1-5)

Día 1 de Marzo:
├─ Admin hace transferencia bancaria: $180,000
├─ Admin registra pago en el sistema
├─ Liquidación cambia:
│  └─ Estado: "Cerrada" → "Pagada"
└─ Todas las clases cambian:
   └─ Estado: "Liquidado" → "Pagado"

✅ CICLO COMPLETO
```

---

### 6.3 Aprobar Pago de una Clase

#### ¿Cuándo hacerlo?
- Después de completar una clase
- Antes de fin de mes (para incluirla en la liquidación)

#### Pasos:

1. **Menu → Nómina → Pagos Pendientes**

2. **Ver lista de clases pendientes:**
   ```
   ⏳ PAGOS PENDIENTES
   
   Clase #1:
   ├─ Profesor: Jorge Padilla
   ├─ Fecha: 02/02/2026
   ├─ Tipo: Tango Salon
   ├─ Duración: 1.5 horas
   ├─ Tarifa: $30,000/hora
   ├─ Total calculado: $45,000
   └─ Estado: "Pendiente"
   ```

3. **Revisar el monto:**
   - ¿Es correcto?
   - ¿Necesita ajustes? (bonos, descuentos)

4. **Agregar ajustes (opcional):**
   ```
   💰 AJUSTES
   
   Valor base: $45,000
   
   Bonos/Adicionales:
   ├─ + Bono asistencia: $10,000
   └─ + Bono puntualidad: $5,000
   
   Total final: $60,000
   
   Observaciones:
   "Clase con 20 alumnos (máximo cupo) - Bono por gestión"
   ```

5. **Click en "Aprobar"**

#### ¿Qué pasa cuando apruebas?

```
ACTUALIZACIÓN:
├─ EstadoPago: "Pendiente" → "Aprobado"
├─ TotalPago actualizado (si hubo ajustes)
├─ Observaciones guardadas
└─ Clase lista para liquidación mensual
```

---

### 6.4 Liquidar Mes

#### ¿Cuándo hacerlo?
- **Fin de mes** (día 28-31)
- Cuando hayas aprobado todas las clases del mes

#### ⚠️ Condiciones:
- Solo puedes liquidar clases en estado "Aprobado"
- Solo puede haber UNA liquidación activa por profesor por mes
- Si ya existe liquidación "Cerrada", el sistema AGREGA las nuevas clases

#### Pasos:

1. **Menu → Nómina → Liquidar Mes**

2. **Seleccionar profesor:**
   ```
   👨‍🏫 PROFESOR
   └─ Jorge Padilla
   ```

3. **Seleccionar mes:**
   ```
   📅 PERÍODO
   └─ Febrero 2026
   ```

4. **El sistema muestra resumen:**
   ```
   📊 RESUMEN LIQUIDACIÓN
   
   Profesor: Jorge Padilla
   Período: Febrero 2026
   
   Clases aprobadas: 6
   ├─ 02/02: Tango Salon - $45,000
   ├─ 05/02: Tango Escenario - $45,000 + $10,000 (bono)
   ├─ 09/02: Tango Salon - $45,000
   ├─ 12/02: Tango Salon - $45,000
   ├─ 16/02: Tango Escenario - $45,000
   └─ 20/02: Tango Salon - $45,000
   
   TOTAL A LIQUIDAR: $280,000
   
   Observaciones: (opcional)
   "Liquidación febrero - Excelente mes"
   ```

5. **Click en "Liquidar"**

#### ¿Qué pasa cuando liquidas?

```
PROCESO AUTOMÁTICO:

1. BUSCA LIQUIDACIÓN EXISTENTE:
   ├─ Si NO existe → Crea nueva liquidación
   └─ Si existe y está "Cerrada" → AGREGA a la existente

2. CREA/ACTUALIZA LIQUIDACIÓN:
   ├─ IdLiquidacion: [GUID]
   ├─ IdProfesor: Jorge Padilla
   ├─ Mes: 2 (Febrero)
   ├─ Anio: 2026
   ├─ TotalPago: $280,000
   ├─ Estado: "Cerrada"
   └─ CantidadClases: 6

3. ACTUALIZA LAS CLASES:
   └─ Todas las clases incluidas:
      └─ EstadoPago: "Aprobado" → "Liquidado"

✅ Liquidación lista para pago
```

---

### 6.5 Liquidaciones Incrementales (Característica Avanzada)

#### ¿Qué significa?

Puedes liquidar **múltiples veces en el mismo mes**, y el sistema irá **agregando las clases a la misma liquidación** mientras NO esté pagada.

#### Ejemplo Real:

```
Semana 1 (5 de febrero):
├─ Jorge dicta 1 clase ($45,000)
├─ Admin aprueba
├─ Admin hace "Liquidar Mes" (anticipado)
└─ Liquidación creada: $45,000 (Estado: "Cerrada")

Semana 2 (12 de febrero):
├─ Jorge dicta 2 clases más ($90,000)
├─ Admin aprueba
├─ Admin hace "Liquidar Mes" NUEVAMENTE
├─ Sistema DETECTA liquidación existente
├─ Sistema AGREGA las 2 nuevas clases
└─ Liquidación actualizada: $135,000 (Estado: "Cerrada")

Semana 4 (28 de febrero):
├─ Jorge dicta 3 clases más ($135,000)
├─ Admin aprueba
├─ Admin hace "Liquidar Mes" por tercera vez
└─ Liquidación final: $270,000 (Estado: "Cerrada")

Inicio Marzo:
├─ Admin hace transferencia de $270,000
└─ Admin registra pago (UNA SOLA VEZ)
```

#### Ventaja:
- ✅ Control progresivo durante el mes
- ✅ Puedes revisar el acumulado en cualquier momento
- ✅ Flexibilidad sin perder trazabilidad

---

### 6.6 Registrar Pago a Profesor

#### ¿Cuándo hacerlo?
- **Inicio del mes siguiente** (día 1-5)
- Después de hacer la transferencia bancaria real
- Solo si la liquidación está "Cerrada"

#### ⚠️ Importante:
- Una vez registres el pago, **NO puedes agregar más clases** a esa liquidación
- El ciclo se cierra completamente

#### Pasos:

1. **Hacer la transferencia bancaria real:**
   ```
   Desde: Cuenta Chetango
   Hacia: Cuenta Jorge Padilla
   Monto: $280,000
   Referencia: "NOMINA-FEB-2026-JORGE"
   ```

2. **Menu → Nómina → Liquidaciones**

3. **Buscar la liquidación:**
   ```
   🔍 FILTROS
   ├─ Profesor: Jorge Padilla
   ├─ Mes: Febrero 2026
   └─ Estado: "Cerrada"
   
   📋 RESULTADO
   └─ Liquidación Feb-2026
      ├─ Total: $280,000
      ├─ Clases: 6
      └─ Estado: Cerrada
   ```

4. **Click en "Registrar Pago"**

5. **Completar formulario:**
   ```
   💳 REGISTRAR PAGO
   
   Liquidación: Feb-2026 - Jorge Padilla
   Monto: $280,000 (no editable)
   
   Fecha pago: 01/03/2026
   Método: Transferencia Bancaria
   Referencia: "NOMINA-FEB-2026-JORGE"
   Observaciones: "Pago mensual febrero"
   
   ☑️ Notificar al profesor por email
   ```

6. **Click en "Confirmar Pago"**

#### ¿Qué pasa cuando registras?

```
PROCESO FINAL:

1. ACTUALIZA LIQUIDACIÓN:
   ├─ Estado: "Cerrada" → "Pagada"
   ├─ FechaPago: 01/03/2026
   └─ 🔒 Liquidación BLOQUEADA (no se puede modificar)

2. ACTUALIZA TODAS LAS CLASES:
   └─ EstadoPago: "Liquidado" → "Pagado"

3. NOTIFICA AL PROFESOR:
   └─ Email: "Tu pago de $280,000 ha sido procesado"

✅ CICLO COMPLETADO
```

---

### 6.7 Reglas de Oro de la Nómina

```
┌──────────────────────────────────────────────────────┐
│  1️⃣  UNA LIQUIDACIÓN POR PROFESOR POR MES            │
│     Solo puede existir una liquidación activa        │
│                                                      │
│  2️⃣  ESTADO "CERRADA" = FLEXIBLE                     │
│     Puedes seguir agregando clases                   │
│     Liquida cuantas veces necesites                  │
│                                                      │
│  3️⃣  ESTADO "PAGADA" = FINAL                         │
│     NO puedes modificar ni agregar clases            │
│     El ciclo está completamente cerrado              │
│                                                      │
│  4️⃣  LIQUIDAR ≠ PAGAR                                │
│     Son dos pasos separados                          │
│     Liquida durante el mes, paga al final            │
│                                                      │
│  5️⃣  PAGA SOLO UNA VEZ AL FINAL DEL MES              │
│     Espera a tener todas las clases                  │
│     Registra un solo pago con el total               │
└──────────────────────────────────────────────────────┘
```

---

### 6.8 Errores Comunes y Soluciones

#### ❌ ERROR 1: Pagar antes de tiempo

```
Problema:
├─ Liquidaste y pagaste después de la primera clase
└─ Llegan más clases del mes

Resultado:
❌ No puedes agregar más clases a liquidación pagada
❌ Tendrías que crear pago adicional (incorrecto)

Solución:
✅ ESPERA al fin de mes para pagar
✅ Liquida cuantas veces quieras, pero paga UNA sola vez
```

#### ❌ ERROR 2: Olvidar aprobar clases

```
Problema:
└─ Llegas a fin de mes y algunas clases están "Pendientes"

Resultado:
⚠️ No se incluyen en la liquidación

Solución:
✅ Aprueba las clases durante el mes
✅ Revisa diariamente los pagos pendientes
```

#### ❌ ERROR 3: Múltiples pagos en el mismo mes

```
Problema:
├─ Pagaste semana 1: $45,000
├─ Pagaste semana 2: $90,000
└─ Pagaste semana 4: $135,000
   └─ Total: 3 pagos diferentes

Resultado:
❌ Dificulta contabilidad
❌ Múltiples transferencias
❌ Confusión en reportes

Solución:
✅ UN SOLO PAGO al final del mes
✅ Usar liquidaciones incrementales
```

---

### 6.9 Vista del Profesor

#### ¿Qué ve el profesor en su portal?

```
PORTAL PROFESOR - Jorge Padilla

💼 MIS PAGOS

Febrero 2026:
├─ Estado: 🟡 Liquidado
├─ Total: $280,000
├─ Clases: 6
└─ Fecha estimada pago: 01/03/2026

Detalle:
├─ 02/02: Tango Salon - $45,000 ✅ Pagado
├─ 05/02: Tango Escenario - $55,000 ✅ Pagado
├─ 09/02: Tango Salon - $45,000 ✅ Pagado
└─ ...

Enero 2026:
├─ Estado: 💚 Pagado
├─ Total: $320,000
├─ Fecha pago: 01/02/2026
└─ Método: Transferencia
```

---

## 📊 Reportes y Consultas

### 7.1 Reporte de Ingresos

#### Menu → Reportes → Ingresos

```
📈 INGRESOS MENSUALES

Período: Febrero 2026

💰 Resumen:
├─ Total recaudado: $4,500,000
├─ Cantidad de pagos: 25
├─ Promedio por pago: $180,000
└─ Variación vs mes anterior: +15%

📊 Por Método de Pago:
├─ Transferencia: $2,800,000 (62%)
├─ Efectivo: $1,200,000 (27%)
├─ Tarjeta: $400,000 (9%)
└─ Nequi: $100,000 (2%)

📦 Por Tipo de Paquete:
├─ Paquete 8 Clases: 15 × $140,000 = $2,100,000
├─ Paquete 12 Clases: 8 × $200,000 = $1,600,000
└─ Paquete 4 Clases: 2 × $80,000 = $160,000

📅 Tendencia Semanal:
├─ Semana 1: $900,000
├─ Semana 2: $1,200,000
├─ Semana 3: $1,100,000
└─ Semana 4: $1,300,000
```

---

### 7.2 Reporte de Asistencias

#### Menu → Reportes → Asistencias

```
✅ REPORTE DE ASISTENCIAS

Período: Febrero 2026

📊 Resumen General:
├─ Total clases dictadas: 40
├─ Total asistencias registradas: 480
├─ Promedio por clase: 12 alumnos
└─ Porcentaje ocupación: 60% (cupo 20)

👥 Por Alumno (Top 10):
├─ María López: 12 clases (100% asistencia)
├─ Juan Pérez: 11 clases (92%)
├─ Ana Gómez: 10 clases (83%)
└─ ...

📅 Por Tipo de Clase:
├─ Tango Salon: 280 asistencias (58%)
├─ Tango Escenario: 150 asistencias (31%)
└─ Elencos: 50 asistencias (11%)

⚠️ Alertas:
├─ 3 alumnos sin asistir en 2 semanas
└─ 5 alumnos con paquetes por vencer (< 7 días)
```

---

### 7.3 Reporte de Paquetes

#### Menu → Reportes → Paquetes

```
📦 REPORTE DE PAQUETES

Estado actual:

🟢 Activos: 45 paquetes
├─ Total clases disponibles: 360
├─ Valor total: $6,300,000
└─ Promedio días vigencia: 25 días

🔴 Por vencer (< 7 días): 8 paquetes
⚫ Vencidos: 5 paquetes
🔵 Congelados: 2 paquetes
⚪ Agotados: 12 paquetes

📊 Tendencia de compra:
├─ Paquete más vendido: 8 Clases (60%)
├─ Paquete menos vendido: 4 Clases (15%)
└─ Tasa renovación: 75%
```

---

### 7.4 Reporte de Nómina

#### Menu → Reportes → Nómina

```
💼 REPORTE DE NÓMINA

Período: Febrero 2026

👨‍🏫 Por Profesor:

Jorge Padilla:
├─ Clases dictadas: 12
├─ Horas totales: 18h
├─ Total pagado: $540,000
├─ Promedio/hora: $30,000
└─ Estado: ✅ Pagado (01/03/2026)

Ana Zoraida Gomez:
├─ Clases dictadas: 8 (6 Principal + 2 Monitor)
├─ Horas totales: 12h
├─ Total pagado: $380,000
├─ Estado: ✅ Pagado (01/03/2026)

📊 Totales:
├─ Total profesores: 5
├─ Total clases: 40
├─ Total pagado: $1,800,000
└─ Promedio por profesor: $360,000
```

---

### 7.5 Estadísticas del Día (Dashboard)

#### Menu → Dashboard / Inicio

```
📊 RESUMEN DEL DÍA - 05/02/2026

💰 Ingresos Hoy:
└─ $420,000 (3 pagos)

📅 Clases de Hoy:
├─ Programadas: 3
├─ Completadas: 1
└─ Pendientes: 2

✅ Asistencias:
└─ 15 alumnos registrados

⚠️ Alertas:
├─ 🔔 5 paquetes vencen en 3 días
├─ 🔔 2 pagos de profesores pendientes de aprobar
└─ 🔔 1 clase sin asistencias registradas

📈 Comparado con ayer:
├─ Ingresos: +25%
└─ Asistencias: +10%
```

---

## 🆘 Casos Especiales

### 8.1 Devolución de Dinero

#### Escenario: Alumno se retira y pide devolución

#### ¿Qué hacer?

1. **Verificar estado del paquete:**
   ```
   ¿Cuántas clases usó?
   ├─ 0 clases → Devolución 100%
   ├─ 1-3 clases → Devolución proporcional
   └─ >50% usado → Sin devolución (política)
   ```

2. **Calcular monto a devolver:**
   ```
   Paquete 8 clases: $140,000
   Clases usadas: 2
   Clases no usadas: 6
   
   Monto a devolver:
   └─ ($140,000 / 8) × 6 = $105,000
   ```

3. **Proceso en el sistema:**
   ```
   ❌ NO elimines el pago original
   
   ✅ Registra un nuevo pago NEGATIVO:
   ├─ Menu → Pagos → Registrar Pago
   ├─ Alumno: [Seleccionar]
   ├─ Monto: -$105,000 (negativo)
   ├─ Método: Devolución/Reembolso
   ├─ Observaciones: "Devolución paquete 8 clases - 6 clases no usadas"
   └─ Guardar
   ```

4. **Actualizar estado del paquete:**
   ```
   Menu → Paquetes → [Buscar paquete]
   └─ Cambiar estado a "Cancelado" (requiere permisos especiales)
   ```

5. **Hacer la transferencia bancaria:**
   ```
   Transferir $105,000 a la cuenta del alumno
   ```

---

### 8.2 Clase de Cortesía (Sin Paquete)

#### Escenario: Alumno nuevo quiere probar una clase gratis

#### Pasos:

1. **Asegúrate que el alumno existe en el sistema**
   - Si no, créalo primero

2. **El día de la clase:**
   ```
   Menu → Asistencias → Registrar Asistencia
   
   Alumno: [Seleccionar alumno nuevo]
   Clase: [Seleccionar la clase]
   
   ⚠️ IMPORTANTE:
   ├─ Tipo Asistencia: "Cortesía" o "Prueba"
   ├─ Paquete: NINGUNO (dejar en blanco)
   └─ Observaciones: "Clase de prueba - Primera vez"
   ```

3. **Guardar**

#### ¿Qué pasa?
- ✅ Se registra la asistencia
- ✅ NO se descuenta ningún paquete
- ✅ Queda el registro para seguimiento
- ✅ Puedes ver luego si el alumno compró paquete

---

### 8.3 Recuperación de Clase

#### Escenario: Alumno faltó a clase por fuerza mayor, le das oportunidad de reponer

#### Opción 1: Usar la misma clase del paquete
```
Menu → Asistencias
├─ Registrar asistencia en la clase de reposición
├─ Tipo: "Recuperación"
├─ Seleccionar su paquete normal
└─ ✅ Se descuenta del paquete
```

#### Opción 2: No descontar
```
Menu → Asistencias
├─ Registrar asistencia en la clase de reposición
├─ Tipo: "Cortesía"
├─ NO seleccionar paquete
└─ ❌ NO se descuenta del paquete
```

#### ⚠️ Política Recomendada:
- Primera recuperación: Cortesía (no descuentala)
- Siguientes: Usar paquete normal

---

### 8.4 Cambio de Horario de Clase

#### Escenario: Necesitas cambiar el horario de una clase ya programada

#### ⚠️ Importante:
- Solo puedes cambiar clases que NO estén completadas
- Notifica a los alumnos del cambio

#### Pasos:

1. **Menu → Clases → [Buscar la clase]**

2. **Click en Editar**

3. **Cambiar horario:**
   ```
   Hora inicio: 19:00 → 20:00
   Hora fin: 20:30 → 21:30
   
   Observaciones: "Cambio de horario por [motivo]"
   ```

4. **Guardar**

5. **Notificar manualmente:**
   - Email/WhatsApp a los alumnos
   - Mensaje en el portal (si está activo)

---

### 8.5 Profesor No Puede Dictar Clase

#### Escenario: Profesor avisa que no puede asistir

#### Opción 1: Cancelar la clase
```
Menu → Clases → [Buscar clase] → Cancelar
└─ Motivo: "Profesor con incapacidad médica"

Notificar a alumnos
```

#### Opción 2: Cambiar de profesor
```
Menu → Clases → [Buscar clase] → Editar
├─ Profesor Principal: [Cambiar por otro profesor]
└─ Observaciones: "Cambio de profesor por [motivo]"

Notificar a alumnos
```

---

### 8.6 Paquete Vencido con Clases Disponibles

#### Escenario: Alumno tiene clases que no usó y su paquete venció

#### Política Sugerida:

**Opción A: Extensión de vigencia**
```
Menu → Paquetes → [Buscar paquete vencido]

⚠️ Requiere permisos especiales o script SQL:
UPDATE Paquetes
SET FechaVencimiento = DATEADD(DAY, 15, FechaVencimiento)
WHERE IdPaquete = '[ID]'

Observaciones: "Extensión 15 días - Cortesía por pandemia"
```

**Opción B: Crear nuevo paquete cortesía**
```
El alumno debe pagar nuevo paquete
└─ Opcionalmente con descuento
```

---

## ❓ Preguntas Frecuentes

### ¿Qué hago si un alumno compra por error?
- Registra una devolución (pago negativo)
- Ver [8.1 Devolución de Dinero](#81-devolución-de-dinero)

### ¿Puedo eliminar un pago?
- ❌ NO se recomienda eliminar
- ✅ Mejor: registra devolución

### ¿Puedo editar un paquete después de creado?
- ✅ Sí, puedes editar:
  - Fecha de vencimiento
  - Estado (congelar/descongelar)
- ❌ No puedes editar:
  - Clases disponibles (se modifican solo con asistencias)
  - Tipo de paquete
  - Alumno propietario

### ¿Cómo sé qué paquetes están por vencer?
- Menu → Dashboard → Ver alertas
- Menu → Paquetes → Filtro: "Por vencer"

### ¿Puedo tener múltiples paquetes activos para un alumno?
- ✅ SÍ, un alumno puede tener varios paquetes simultáneamente
- Al registrar asistencia, eliges cuál usar

### ¿Qué pasa si marco asistencia con paquete equivocado?
- Puedes editar la asistencia
- El sistema revierte el descuento del paquete anterior
- Y descuenta del nuevo paquete seleccionado

### ¿Puedo pagar a un profesor fuera del ciclo mensual?
- ✅ Sí, puedes liquidar y pagar cuando quieras
- ⚠️ Recomendado: esperar a fin de mes para un solo pago

### ¿Qué hago si liquidé por error?
- Si NO has pagado todavía: Puedes agregar más clases
- Si YA pagaste: Requiere soporte técnico para revertir

### ¿Cómo manejo clases privadas vs grupales?
- Son tipos de clase diferentes
- Paquetes privados solo para clases privadas
- Paquetes grupales solo para clases grupales
- No se pueden mezclar

---

## 📖 Glosario de Términos

| Término | Definición |
|---------|------------|
| **Paquete** | Conjunto de clases prepagadas que compra un alumno |
| **Tipo de Paquete** | Plantilla que define: cantidad de clases, precio, vigencia |
| **Estado de Paquete** | Activo, Vencido, Congelado, Agotado |
| **Asistencia** | Registro de que un alumno asistió a una clase |
| **Liquidación** | Agrupación mensual de pagos a un profesor |
| **Nómina** | Proceso completo de pago a profesores |
| **Clase Grupal** | Clase con varios alumnos (cupo > 2) |
| **Clase Privada** | Clase personalizada (1-2 personas) |
| **Tarifa** | Monto que se paga al profesor por hora o clase |
| **Rol en Clase** | Principal (dicta) o Monitor (asiste) |
| **Congelación** | Pausa temporal de un paquete |
| **Cortesía** | Clase gratis sin descontar del paquete |
| **Estado Pendiente** | Pago de profesor calculado pero no aprobado |
| **Estado Aprobado** | Pago revisado y listo para liquidar |
| **Estado Liquidado** | Incluido en liquidación mensual |
| **Estado Pagado** | Profesor recibió el dinero |
| **Vigencia** | Días que dura un paquete desde su activación |

---

## 🎯 Resumen Final

### Flujo Diario Típico del Administrador

```
MAÑANA:
├─ Revisar Dashboard
├─ Ver alertas (paquetes por vencer, pagos pendientes)
└─ Planificar el día

DURANTE EL DÍA:
├─ Registrar pagos de alumnos que compran
├─ Crear clases para la próxima semana
├─ Responder consultas de alumnos/profesores
└─ Revisar asistencia de clases del día anterior

NOCHE (después de clases):
├─ Completar las clases del día
├─ Registrar asistencias
├─ Aprobar pagos de profesores
└─ Actualizar estadísticas

FIN DE MES:
├─ Aprobar todos los pagos pendientes
├─ Liquidar mes para cada profesor
└─ Generar reportes mensuales

INICIO DE MES:
├─ Hacer transferencias bancarias
├─ Registrar pagos en el sistema
└─ Verificar que todo esté pagado
```

### Checklist de Operación Mensual

```
☐ Semana 1:
   ├─ ☐ Registrar pagos de renovaciones
   ├─ ☐ Crear clases del mes
   └─ ☐ Aprobar pagos de profesores de semana anterior

☐ Semana 2:
   ├─ ☐ Contactar alumnos con paquetes por vencer
   ├─ ☐ Aprobar pagos de profesores
   └─ ☐ Revisar asistencias

☐ Semana 3:
   ├─ ☐ Seguimiento a renovaciones
   ├─ ☐ Aprobar pagos de profesores
   └─ ☐ Preparar cierre de mes

☐ Semana 4 (Fin de mes):
   ├─ ☐ Aprobar TODOS los pagos pendientes
   ├─ ☐ Liquidar mes para TODOS los profesores
   ├─ ☐ Generar reporte mensual
   └─ ☐ Revisar estadísticas

☐ Inicio mes siguiente:
   ├─ ☐ Hacer transferencias bancarias
   ├─ ☐ Registrar pagos en sistema
   └─ ☐ Enviar comprobantes a profesores
```

---

## 📞 Soporte y Contacto

### Soporte Técnico
- Email: soporte@chetango.com
- Teléfono: +57 XXX XXX XXXX
- Horario: Lunes a Viernes, 9am - 6pm

### Documentación Adicional
- [Manual Técnico de la API](./api/README.md)
- [Guía de Despliegue](./DEPLOYMENT-STRATEGY.md)
- [Proceso de Nómina Detallado](./business/payroll-process.md)
- [Gestión de Paquetes Privados](./business/package-management.md)

---

**Fin del Manual del Administrador**

*Versión 1.0 - Febrero 2026*  
*Sistema Chetango - Academia de Tango*

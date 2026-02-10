# 🗄️ ANÁLISIS DE LIMPIEZA DE BASE DE DATOS - CHETANGO

**Fecha:** 30 Enero 2026  
**Objetivo:** Limpiar la BD para pruebas desde cero, manteniendo solo catálogos y 3 usuarios con Entra ID

---

## 📊 CLASIFICACIÓN DE TABLAS

### ✅ CATEGORÍA 1: CATÁLOGOS Y MAESTROS (MANTENER)
Estas tablas contienen datos de configuración que NO deben borrarse:

#### 📌 Estados (Catálogos de Estados)
- ✅ **EstadosAlumno** - Estados de alumnos (Activo, Inactivo, etc.)
- ✅ **EstadosAsistencia** - Estados de asistencia (Presente, Ausente, etc.)
- ✅ **EstadosUsuario** - Estados de usuarios
- ✅ **EstadosPaquete** - Estados de paquetes (Activo, Vencido, Agotado)
- ✅ **EstadosPago** - Estados de pagos (Pendiente, Pagado, etc.)
- ✅ **EstadosNotificacion** - Estados de notificaciones

#### 📌 Tipos (Catálogos de Tipos)
- ✅ **TiposDocumento** - Tipos de documento (CC, TI, CE, etc.)
- ✅ **TiposClase** - Tipos de clase (Principiante, Intermedio, Avanzado, etc.)
- ✅ **TiposPaquete** - Tipos de paquetes disponibles
- ✅ **TiposProfesor** - Tipos de profesor (Principal, Monitor) - **CRÍTICO**
- ✅ **TiposAsistencia** - Tipos de asistencia (Normal, Cortesía, Prueba, Recuperación) - **CRÍTICO**
- ✅ **RolesEnClase** - Roles en clase (Principal, Monitor) - **CRÍTICO**
- ✅ **MetodosPago** - Métodos de pago (Efectivo, Transferencia, etc.)

#### 📌 Tarifas (Configuración de Pagos)
- ✅ **TarifasProfesor** - Tarifas por tipo de profesor y rol - **CRÍTICO**
  - Principal: $30,000/hora
  - Monitor: $10,000/hora

**Total Tablas a Mantener:** 15 tablas

---

### 🔴 CATEGORÍA 2: DATOS TRANSACCIONALES (BORRAR)
Estas tablas contienen datos de operaciones del día a día:

#### 🗑️ Operaciones de Clases
- ❌ **Clases** - Todas las clases programadas
- ❌ **ClasesProfesores** - Asignaciones profesor-clase
- ❌ **MonitoresClase** - Monitores asignados a clases
- ❌ **Asistencias** - Registros de asistencia

#### 🗑️ Operaciones Financieras
- ❌ **Paquetes** - Paquetes vendidos a alumnos
- ❌ **CongelacionesPaquete** - Congelaciones de paquetes
- ❌ **Pagos** - Pagos registrados (paquetes)
- ❌ **LiquidacionesMensuales** - Liquidaciones de nómina

#### 🗑️ Comunicaciones
- ❌ **Notificaciones** - Notificaciones enviadas
- ❌ **Eventos** - Eventos del sistema

#### 🗑️ Auditoría
- ❌ **Auditorias** - Logs de auditoría (opcional mantener para debugging)

**Total Tablas a Limpiar:** 11 tablas

---

### ⚠️ CATEGORÍA 3: USUARIOS Y PROFESORES (MANTENER SOLO 3 CON ENTRA ID)

#### 👤 Usuarios a MANTENER (con Entra ID)

| Rol | Email | ID Usuario | ID Relacionado |
|-----|-------|------------|----------------|
| **Admin** | Chetango@chetangoprueba.onmicrosoft.com | b91e51b9-4094-441e-a5b6-062a846b3868 | - |
| **Profesor** | Jorgepadilla@chetangoprueba.onmicrosoft.com | 8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB | IdProfesor: 8f6e460d-328d-4a40-89e3-b8effa76829c |
| **Alumno** | JuanDavid@chetangoprueba.onmicrosoft.com | 71462106-9863-4fd0-b13d-9878ed231aa6 | IdAlumno: 295093d5-b36f-4737-b68a-ab40ca871b2e |

#### 🗑️ Usuarios/Profesores/Alumnos a BORRAR
- ❌ **Usuarios** - Todos EXCEPTO los 3 con Entra ID
- ❌ **Profesores** - Todos EXCEPTO Jorge Padilla (8f6e460d-328d-4a40-89e3-b8effa76829c)
- ❌ **Alumnos** - Todos EXCEPTO Juan David (295093d5-b36f-4737-b68a-ab40ca871b2e)
- ❌ **ConfiguracionesNotificaciones** - Configuraciones de notificaciones de usuarios
- ❌ **UsuarioRol** - Si existe (roles asignados en BD, aunque ahora vienen de Entra)

---

## 📋 ORDEN DE EJECUCIÓN DEL BORRADO

### Fase 1: Borrar datos transaccionales (respetando FK)

```sql
-- 1. Borrar asistencias (dependen de clases y alumnos)
DELETE FROM Asistencias;

-- 2. Borrar monitores de clases (dependen de clases y profesores)
DELETE FROM MonitoresClase;

-- 3. Borrar clases-profesores (dependen de clases y profesores)
DELETE FROM ClasesProfesores;

-- 4. Borrar clases
DELETE FROM Clases;

-- 5. Borrar congelaciones de paquetes (dependen de paquetes)
DELETE FROM CongelacionesPaquete;

-- 6. Borrar paquetes (excepto los del alumno que mantenemos)
DELETE FROM Paquetes 
WHERE IdAlumno != '295093d5-b36f-4737-b68a-ab40ca871b2e';

-- 7. Borrar pagos (excepto del alumno que mantenemos, si aplica)
DELETE FROM Pagos 
WHERE IdAlumno != '295093d5-b36f-4737-b68a-ab40ca871b2e';

-- 8. Borrar liquidaciones mensuales
DELETE FROM LiquidacionesMensuales;

-- 9. Borrar notificaciones
DELETE FROM Notificaciones;

-- 10. Borrar eventos
DELETE FROM Eventos;

-- 11. Borrar configuraciones de notificaciones
DELETE FROM ConfiguracionesNotificaciones;

-- 12. Borrar auditorías (opcional - útil para debugging)
-- DELETE FROM Auditorias;
```

### Fase 2: Limpiar usuarios, profesores y alumnos

```sql
-- 1. Borrar alumnos EXCEPTO Juan David
DELETE FROM Alumnos 
WHERE IdAlumno != '295093d5-b36f-4737-b68a-ab40ca871b2e';

-- 2. Borrar profesores EXCEPTO Jorge Padilla
DELETE FROM Profesores 
WHERE IdProfesor != '8f6e460d-328d-4a40-89e3-b8effa76829c';

-- 3. Borrar usuarios EXCEPTO los 3 con Entra ID
DELETE FROM Usuarios 
WHERE IdUsuario NOT IN (
    'b91e51b9-4094-441e-a5b6-062a846b3868', -- Admin
    '8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB', -- Profesor
    '71462106-9863-4fd0-b13d-9878ed231aa6'  -- Alumno
);
```

---

## 📊 VERIFICACIÓN POST-LIMPIEZA

### Debe quedar así:

| Tabla | Registros Esperados |
|-------|---------------------|
| **Usuarios** | 3 (Admin, Profesor, Alumno) |
| **Profesores** | 1 (Jorge Padilla) |
| **Alumnos** | 1 (Juan David) |
| **TiposProfesor** | 2 (Principal, Monitor) |
| **TiposAsistencia** | 4 (Normal, Cortesía, Prueba, Recuperación) |
| **RolesEnClase** | 2 (Principal, Monitor) |
| **TarifasProfesor** | 4 (2 tipos × 2 roles) |
| **TiposClase** | Según configuración (0-5) |
| **TiposPaquete** | Según configuración (0-10) |
| **Clases** | 0 |
| **Asistencias** | 0 |
| **Paquetes** | 0 ó 1 (si Juan David tiene paquete activo) |
| **Pagos** | 0 |

---

## ⚠️ CONSIDERACIONES IMPORTANTES

### 🔴 CRÍTICO - NO BORRAR JAMÁS:
1. **TiposProfesor** (Principal, Monitor)
2. **TarifasProfesor** (Configuración de pagos)
3. **RolesEnClase** (Principal, Monitor)
4. **TiposAsistencia** (Normal, Cortesía, Prueba, Recuperación)

### ⚠️ PRECAUCIÓN - Evaluar según caso:
1. **Paquetes** del alumno Juan David - Decidir si mantener o borrar
2. **Auditorias** - Útil mantener para debugging
3. **TiposClase** - Si ya están configurados, mantener
4. **TiposPaquete** - Si ya están configurados, mantener

### 💡 RECOMENDACIÓN:
- Mantener **1 paquete activo** para Juan David (para pruebas de asistencias)
- Borrar el resto de paquetes
- Mantener tipos de clase si ya están creados (para no tener que configurar de nuevo)

---

## 🎯 RESULTADO ESPERADO

Después de la limpieza tendremos:

✅ **Base limpia** con solo catálogos y configuración
✅ **3 usuarios** funcionales con Entra ID
✅ **1 profesor** (Jorge Padilla) listo para asignar a clases
✅ **1 alumno** (Juan David) listo para registrar asistencias
✅ **Sistema listo** para crear nuevas clases, asistencias, pagos desde cero

---

## 📝 DECISIONES PENDIENTES

¿Qué deseas hacer con:

1. **Paquetes de Juan David** - ¿Mantener 1 activo o borrar todos?
2. **Auditorías** - ¿Mantener para debugging o borrar?
3. **TiposClase y TiposPaquete** - ¿Mantener si existen o empezar desde cero?

---

## ✅ SIGUIENTE PASO

Una vez confirmes qué mantener/borrar, generaré:
1. ✅ Script SQL completo de limpieza
2. ✅ Script de verificación post-limpieza
3. ✅ Backup recomendado antes de ejecutar

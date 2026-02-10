# 🧩 Módulos del Sistema - Chetango Backend

Este documento detalla los módulos principales de la aplicación Chetango, con descripción funcional, usuarios involucrados, estado de implementación y prioridad de desarrollo.

> **Última actualización:** 5 Febrero 2026  
> **Estado general:** MVP Fase 1 - Módulos Asistencias (con TipoAsistencia), Clases, Paquetes, Pagos y Reportes completados

---

## ⚠️ Consideraciones Importantes de Base de Datos

### Codificación de Caracteres (Unicode / UTF-8)

**IMPORTANTE:** SQL Server maneja Unicode automáticamente con tipos `NVARCHAR/NCHAR/NTEXT`, pero requiere el prefijo `N''` en strings literales para preservar caracteres especiales (acentos, ñ, etc.).

#### ✅ Correcto
```sql
INSERT INTO Usuarios (NombreUsuario) VALUES (N'María Gómez');
UPDATE Alumnos SET Nombre = N'José Pérez' WHERE Id = @id;
```

#### ❌ Incorrecto (causa corrupción)
```sql
INSERT INTO Usuarios (NombreUsuario) VALUES ('María Gómez');  -- Se guarda como "MarÃa GÃ³mez"
```

#### Síntomas de Corrupción
- Nombres como "GÃ³mez" en lugar de "Gómez"
- "MarÃa" en lugar de "María"
- "PÃ©rez" en lugar de "Pérez"

#### Solución
1. **Prevención:** Usar siempre `N''` para strings con caracteres Unicode en SQL
2. **Corrección:** Ejecutar script `scripts/fix_encoding_simple.sql` para corregir datos existentes
3. **Connection String:** NO usar `Charset=utf8` (no es compatible con SQL Server)

#### Scripts Disponibles
- `scripts/fix_encoding_simple.sql` - Corrige nombres corruptos en tabla Usuarios
- `scripts/fix_character_encoding.sql` - Versión completa para todas las tablas
- Todos los scripts seed actualizados con prefijo `N''`

---

## 📊 Estado Global de Implementación

| Módulo | Estado | Completitud | Prioridad |
|--------|--------|-------------|-----------|
| 1. Autenticación y Seguridad | ✅ Completo | 100% | ✅ MVP |
| 2. Asistencias + TipoAsistencia | ✅ Completo | 100% | ✅ MVP |
| 3. Clases | ✅ Completo | 100% | ✅ MVP |
| 4. Alumnos | ⚠️ Básico | 20% | Media |
| 5. Profesores | ⚠️ Básico | 20% | Media |
| 6. Paquetes | ✅ Completo | 100% | ✅ MVP |
| 7. Pagos | ✅ Completo | 100% | ✅ MVP |
| 8. Reportes | ✅ Completo | 100% | ✅ MVP |
| 9. Alertas y Notificaciones | ❌ Pendiente | 0% | Baja |

---

## 1. Módulo de Autenticación y Seguridad ✅

**Estado:** ✅ Implementado y funcional  
**Prioridad:** MVP - Crítico

### Descripción
Sistema de autenticación y autorización basado en **Microsoft Entra External ID (CIAM)** con OAuth 2.0.

### Usuarios Involucrados
Todos los usuarios (Admin, Profesor, Alumno)

### Implementación Actual
- ✅ OAuth 2.0 Authorization Code + PKCE
- ✅ Microsoft Entra External ID como proveedor de identidad
- ✅ Roles gestionados en Azure Entra (admin, profesor, alumno)
- ✅ Tokens JWT con claims de roles
- ✅ Políticas de autorización: `AdminOnly`, `AdminOrProfesor`, `ApiScope`
- ✅ Ownership validation por correo electrónico
- ✅ Usuarios de prueba configurados con credenciales

### Arquitectura
```
Usuario → Entra CIAM → Token JWT → API Backend
                                   ↓
                            Validación + Autorización
                                   ↓
                            Endpoint Protegido
```

### Cambios vs Diseño Original
❌ **Eliminado:** Gestión de usuarios en BD, contraseñas encriptadas, autenticación interna  
✅ **Implementado:** Sistema OAuth profesional con CIAM (más seguro y escalable)

### Documentación
Ver: `docs/API-CONTRACT-FRONTEND.md` - Sección Configuración de Autenticación

---

## 2. Módulo de Asistencias ✅

**Estado:** ✅ Implementado y funcional  
**Prioridad:** MVP - Crítico

### Descripción
Permite registrar y consultar la asistencia de alumnos a clases específicas con soporte para **múltiples tipos de asistencia** (Normal, Cortesía, Clase de Prueba, Recuperación).

### Usuarios Involucrados
- **Profesores:** Registran asistencias de sus clases
- **Administrador:** Consulta asistencias de todas las clases
- **Alumnos:** Consultan su propio historial de asistencias

### Funciones Implementadas
- ✅ Registrar asistencia (profesor/admin)
- ✅ **Catálogo TipoAsistencia con reglas de negocio centralizadas**
- ✅ **Soporte para clases de cortesía/prueba sin descuento de paquete**
- ✅ **Clases de recuperación sin descuento**
- ✅ Actualizar estado de asistencia
- ✅ Consultar asistencias por clase
- ✅ Consultar asistencias por alumno (con validación de ownership)
- ✅ Admin: Días con clases programadas
- ✅ Admin: Clases del día específico
- ✅ Admin: Resumen de asistencias por clase

### Catálogo TipoAsistencia 🆕

Catálogo de tipos de asistencia con reglas de negocio centralizadas:

| ID | Nombre | RequierePaquete | DescontarClase | Descripción |
|----|--------|----------------|----------------|-------------|
| 1 | Normal | ✅ Sí | ✅ Sí | Asistencia normal con paquete activo |
| 2 | Cortesía | ❌ No | ❌ No | Clase de cortesía sin descuento |
| 3 | Clase de Prueba | ❌ No | ❌ No | Clase de prueba para nuevos alumnos |
| 4 | Recuperación | ✅ Sí | ❌ No | Recuperación por inasistencia justificada |

**Ventajas:**
- ✅ Extensible: Agregar tipos nuevos (becado, intercambio) sin cambiar código
- ✅ Reportes potentes: Filtrar/agrupar por tipo de asistencia
- ✅ Validaciones automáticas según `RequierePaquete`
- ✅ Control de descuento según `DescontarClase`
- ✅ Auditoría clara del tipo de asistencia

### Endpoints Disponibles
```
POST   /api/asistencias                          [AdminOrProfesor]
       Body: { idClase, idAlumno, idTipoAsistencia, idPaqueteUsado?, observaciones? }
PUT    /api/asistencias/{id}/estado              [AdminOrProfesor]
GET    /api/clases/{id}/asistencias              [AdminOrProfesor]
GET    /api/alumnos/{idAlumno}/asistencias       [ApiScope + Ownership]
GET    /api/admin/asistencias/dias-con-clases    [AdminOnly]
GET    /api/admin/asistencias/clases-del-dia     [AdminOnly]
GET    /api/admin/asistencias/clase/{id}/resumen [AdminOnly]
```

### Arquitectura CQRS
```
Chetango.Application/Asistencias/
  Commands/
    - RegistrarAsistenciaCommand + Handler (validación por TipoAsistencia)
    - ActualizarEstadoAsistenciaCommand + Handler
  Queries/
    - GetAsistenciasPorClaseQuery + Handler
    - GetAsistenciasPorAlumnoQuery + Handler
  Admin/
    - GetDiasConClasesAdminQuery + Handler
    - GetClasesDelDiaAdminQuery + Handler
    - GetResumenAsistenciasClaseAdminQuery + Handler
  DTOs/
    - AsistenciaDto, ClaseConAsistenciasDto, etc.
```

### Validaciones por TipoAsistencia
```csharp
// Handler de RegistrarAsistencia
if (tipoAsistencia.RequierePaquete && !request.IdPaqueteUsado.HasValue)
    return Error("Este tipo requiere paquete activo");

if (!tipoAsistencia.RequierePaquete && request.IdPaqueteUsado.HasValue)
    return Error("Este tipo no debe incluir paquete");

// Solo descuenta si el tipo lo permite
if (estado == Presente && tipoAsistencia.DescontarClase && idPaquete.HasValue)
    await DescontarClaseDelPaquete(idPaquete);
```

### Relaciones
- **Clase:** Una asistencia pertenece a una clase específica
- **Alumno:** Una asistencia registra la presencia de un alumno
- **TipoAsistencia:** Define comportamiento de descuento (Normal, Cortesía, etc.)
- **Paquete:** Descuenta clase del paquete activo solo si TipoAsistencia.DescontarClase = true

### Cambios Recientes (Enero 2026)
- ✅ Agregado catálogo `TipoAsistencia` con seed data
- ✅ `IdPaqueteUsado` ahora es nullable (permite cortesía sin paquete)
- ✅ Validaciones centralizadas por tipo de asistencia
- ✅ Migración: `AgregarCatalogoTipoAsistencia`

### Pendiente (0%)
- ✅ Todo implementado y funcional

---

## 3. Módulo de Clases ✅

**Estado:** ✅ Implementado y funcional  
**Prioridad:** ✅ MVP - Crítico
### ⚠️ Cambio Importante: Múltiples Profesores por Clase

A partir de **Febrero 2026**, el sistema permite asignar **múltiples profesores principales y monitores** a una misma clase.

**Cambios implementados:**
- ✅ Tabla `ClasesProfesores` para relación muchos-a-muchos
- ✅ Campo `IdRolEnClase` (Principal/Monitor) por cada asignación
- ✅ Validación de ownership: cualquier profesor asignado puede ver/gestionar la clase
- ✅ UI muestra todos los profesores con sus roles
- ✅ Formato de tiempo estandarizado a 24h (HH:mm) en todo el sistema
### Descripción
Gestión del calendario de clases: creación, edición, cancelación y consulta con validación de conflictos de horario y ownership.

### Usuarios Involucrados
- **Profesores:** Crean y editan sus propias clases
- **Administrador:** Gestión completa del calendario
- **Alumnos:** Consultan clases disponibles

### Funciones Implementadas (100%)
- ✅ Crear clase (admin)
  - **Permite múltiples profesores principales** (cambio Feb 2026)
  - **Permite múltiples monitores** (cambio Feb 2026)
  - Al menos un profesor principal requerido
- ✅ Editar clase (admin + ownership)
  - **Ownership validado contra todos los profesores asignados**
- ✅ Consultar clases por profesor
  - **Usa tabla ClasesProfesores para incluir todas las asignaciones**
- ✅ Consultar clases por día
- ✅ Detalle de clase
  - **Incluye lista completa de profesores con roles**
- ✅ Validaciones de negocio
  - No clases duplicadas (mismo día/hora/tipo + **cualquier profesor asignado**)
  - Hora fin debe ser posterior a hora inicio
  - Cupo máximo > 0
  - Al menos un profesor principal obligatorio

### Endpoints Disponibles
```
POST   /api/clases                           [AdminOnly]
PUT    /api/clases/{id}                      [AdminOnly]
GET    /api/clases/{id}                      [ApiScope]
GET    /api/clases/profesor/{idProfesor}     [ApiScope] (incluye Principal + Monitor)
GET    /api/clases/dia/{fecha}               [ApiScope]
GET    /api/clases/{id}/asistencias          [Profesor] (ownership validado)
```

### Arquitectura CQRS
```
Chetango.Application/Clases/
  Commands/
    CrearClase/
      - CrearClaseCommand
      - CrearClaseCommandHandler (validación de conflictos y ownership)
      - CrearClaseCommandValidator
    EditarClase/
      - EditarClaseCommand
      - EditarClaseCommandHandler (validación de conflictos y ownership)
      - EditarClaseCommandValidator
    CancelarClase/
      - CancelarClaseCommand
      - CancelarClaseCommandHandler (validación de ownership)
      - CancelarClaseCommandValidator
  Queries/
    GetClaseById/
      - GetClaseByIdQuery
      - GetClaseByIdQueryHandler (validación de ownership)
    GetClasesDeProfesor/
      - GetClasesDeProfesorQuery
      - GetClasesDeProfesorQueryHandler (con paginación y filtros)
    GetClasesDeAlumno/ (ya existía)
      - GetClasesDeAlumnoQuery
      - GetClasesDeAlumnoQueryHandler
  DTOs/
    - CrearClaseDTO
    - EditarClaseDTO
    - ClaseDTO
    - ClaseDetalleDTO
    - MonitorClaseDTO
```

### Funcionalidades Clave
- **Crear Clase:** 
  - Profesor crea clase para sí mismo (debe ser uno de los principales)
  - Admin puede crear clase para cualquier profesor
  - Soporte para múltiples profesores principales y monitores
  - Validación de conflictos de horario para todos los profesores
  - Validación de que fecha/hora es futura
  - Validación de tipo de clase existente
  
- **Editar Clase:**
  - Profesor solo edita sus propias clases
  - Admin puede editar cualquier clase
  - Validación de conflictos de horario (excluyendo la clase actual)
  - Validación de fecha/hora futura
  
- **Cancelar Clase:**
  - Profesor solo cancela sus propias clases
  - Admin puede cancelar cualquier clase
  - No se puede cancelar clase pasada o con asistencias
  
- **Consultar Clases:**
  - Profesor solo ve sus propias clases
  - Admin ve todas las clases
  - Filtros por rango de fechas
  - Paginación para listados grandes

### Validaciones Implementadas
- ✅ Fecha y hora futura al crear/editar
- ✅ HoraFin posterior a HoraInicio
- ✅ Profesor existe y está activo
- ✅ Tipo de clase existe
- ✅ No hay conflicto de horario para el profesor
- ✅ Ownership: Profesor solo gestiona sus clases
- ✅ No se puede cancelar clase pasada
- ✅ No se puede cancelar clase con asistencias

### Relaciones
- **Asistencias:** Una clase tiene múltiples asistencias
- **Profesor:** Una clase tiene un profesor principal
- **TipoClase:** Una clase tiene un tipo (Tango, Vals, Milonga, etc.)
- **MonitorClase:** Tabla intermedia para profesores monitores

### Cambios vs Diseño Original
✅ **Implementado con Clean Architecture:** CQRS, MediatR, FluentValidation
✅ **Ownership Validation:** Profesores solo gestionan sus propias clases
✅ **Validación de conflictos:** No permite solapamiento de horarios
⚠️ **Separado de Asistencias:** Módulos independientes para mayor claridad

---

## 4. Módulo de Alumnos ⚠️

**Estado:** ⚠️ Básico - Solo consultas  
**Prioridad:** Media

### Descripción
Gestión del perfil del alumno, historial de clases, pagos y estado de paquetes.

### Usuarios Involucrados
- **Administrador:** Gestión completa
- **Alumno:** Consulta su información

### Funciones Implementadas (20%)
- ✅ Consultar clases de alumno
- ✅ Consultar asistencias de alumno
- ✅ Entidad `Alumno` con relaciones

### Funciones Pendientes (80%)
- ❌ Ver estado de paquetes activos
- ❌ Historial de pagos
- ❌ Solicitar edición de datos personales
- ❌ Ver próximo vencimiento de paquete
- ❌ Dashboard del alumno

### Endpoints Actuales
```
GET /api/alumnos/{id}/clases      [ApiScope + Ownership]
GET /api/alumnos/{id}/asistencias [ApiScope + Ownership]
```

### Endpoints Pendientes
```
GET    /api/alumnos/{id}                [ApiScope + Ownership]
GET    /api/alumnos/{id}/paquetes       [ApiScope + Ownership]
GET    /api/alumnos/{id}/pagos          [ApiScope + Ownership]
PUT    /api/alumnos/{id}/perfil         [ApiScope + Ownership]
POST   /api/alumnos/{id}/solicitud-edicion [ApiScope + Ownership]
```

### Relaciones
- **Usuario:** Tabla base con información de autenticación
- **Paquetes:** Múltiples paquetes activos posibles
- **Pagos:** Historial de pagos realizados
- **Asistencias:** Registro de clases tomadas

---

## 5. Módulo de Profesores ⚠️

**Estado:** ⚠️ Básico - Solo consultas  
**Prioridad:** Media

### Descripción
Permite a profesores gestionar sus clases y visualizar su historial de trabajo.

### Usuarios Involucrados
- **Profesores:** Consultan su información
- **Administrador:** Gestión completa

### Funciones Implementadas (20%)
- ✅ Entidad `Profesor` con relaciones
- ✅ Relación con clases impartidas

### Funciones Pendientes (80%)
- ❌ Consultar historial de clases impartidas
- ❌ Ver cálculo de pago mensual
- ❌ Dashboard del profesor
- ❌ Estadísticas de asistencia a sus clases

### Endpoints Pendientes
```
GET /api/profesores/{id}/clases            [ApiScope + Ownership]
GET /api/profesores/{id}/historial         [ApiScope + Ownership]
GET /api/profesores/{id}/pago-mensual      [ApiScope + Ownership]
GET /api/profesores/{id}/estadisticas      [ApiScope + Ownership]
```

### Funcionalidades Clave Futuras
- Ver historial completo de clases
- Cálculo automático de pago:
  - Por tipo de clase (grupal, privada, privada múltiple)
  - Por rol (principal o monitor)
  - Tarifa configurable por tipo

### Relaciones
- **Usuario:** Tabla base
- **Clases:** Clases como profesor principal
- **MonitorClase:** Clases como monitor
- **TarifaProfesor:** Tabla de tarifas configurables

---

## 6. Módulo de Paquetes ✅

**Estado:** ✅ Implementado y funcional  
**Prioridad:** MVP - Crítico

### Descripción
Gestión completa de paquetes de clases adquiridos por alumnos: creación, consumo, congelación, y consulta de estado.

### Usuarios Involucrados
- **Administrador:** Crea, edita y gestiona todos los paquetes
- **Alumno:** Consulta estado de sus propios paquetes

### Funciones Implementadas (100%)
- ✅ Crear paquete vinculado a pago
- ✅ Editar paquete (clases disponibles, fecha vencimiento)
- ✅ Congelar paquete con fechas inicio/fin
- ✅ Descongelar paquete activo
- ✅ Descontar clase automáticamente al registrar asistencia (integrado con Asistencias)
- ✅ Consultar paquetes de alumno con filtros y paginación
- ✅ Consultar detalle de paquete con historial de asistencias
- ✅ Consultar mis paquetes (alumno) con autenticación por email
- ✅ Obtener estadísticas de paquetes (admin)
- ✅ Catálogo de tipos de paquetes
- ✅ Validar disponibilidad de paquete antes de usar

### Endpoints Disponibles
```
GET    /api/paquetes/estadisticas              [AdminOnly]
GET    /api/paquetes                           [AdminOnly] - con filtros y paginación
GET    /api/paquetes/{id}                      [ApiScope + Ownership por email]
GET    /api/mis-paquetes                       [ApiScope] - autenticación por email
GET    /api/alumnos/{id}/paquetes              [ApiScope + Ownership por email]
GET    /api/paquetes/tipos                     [ApiScope]
POST   /api/paquetes                           [AdminOnly]
PUT    /api/paquetes/{id}                      [AdminOnly]
POST   /api/paquetes/{id}/congelar             [AdminOnly]
POST   /api/paquetes/{id}/descongelar          [AdminOnly]
```

### Arquitectura CQRS
```
Chetango.Application/Paquetes/
  Commands/
    CrearPaquete/
    EditarPaquete/
    CongelarPaquete/
    DescongelarPaquete/
    DescontarClase/
  Queries/
    GetPaqueteById/
    GetPaquetesDeAlumno/
    GetPaquetes/
    GetMisPaquetes/
    GetEstadisticasPaquetes/
    GetTiposPaquete/
    ValidarPaqueteDisponible/
  DTOs/
    PaqueteAlumnoDTO
    PaqueteDetalleDTO
    CrearPaqueteDTO
    EditarPaqueteDTO
    CongelarPaqueteDTO
    TipoPaqueteDTO
    CongelacionDTO
    CongelacionDetalleDTO
    AsistenciaHistorialDTO
```

### Validaciones Implementadas
- ✅ Alumno existe y está activo
- ✅ Tipo de paquete existe
- ✅ Pago existe (si se vincula)
- ✅ Paquete está en estado Activo para descontar clase
- ✅ Paquete tiene clases disponibles
- ✅ No está vencido
- ✅ No está congelado
- ✅ Congelación no se solapa con otras
- ✅ Ownership: Alumno solo ve sus propios paquetes

### Estados de Paquete
- **Activo (1):** Paquete disponible para usar
- **Vencido (2):** Fecha de vencimiento pasada
- **Congelado (3):** Temporalmente suspendido
- **Completado (calculado):** Clases usadas = clases disponibles

### Autenticación
**Patrón unificado por email:** Todos los endpoints usan `ClaimTypes.Email` o `preferred_username` del token JWT para validación de ownership (consistente con módulos Asistencias y otros).

### Integración con Otros Módulos
- **Asistencias:** Cuando se registra una asistencia presente, se descuenta automáticamente del paquete del alumno
- **Pagos:** Al crear un paquete se puede vincular a un pago existente
- **Catálogos:** Usa tabla `TipoPaquete` con tipos predefinidos (8 Clases, 12 Clases, Mensual Ilimitado, etc.)

### Relaciones
- **Alumno:** Un paquete pertenece a un alumno
- **TipoPaquete:** Define el tipo de paquete (8, 12, mensual, etc.)
- **Pago:** Paquete puede estar vinculado a un pago
- **Congelaciones:** Múltiples periodos de congelación por paquete
- **Asistencias:** Asistencias descontadas del paquete (via IdPaqueteUsado)

### Datos de Prueba
Script `seed_paquetes_catalogos.sql` crea:
- 4 tipos de paquete (8 Clases, 12 Clases, Mensual Ilimitado, Clase Individual)
- 5 paquetes de prueba para alumnos
- 6 alumnos vinculados con usuarios de Entra ID

### Tests Realizados
✅ 9/9 endpoints probados en Postman con tokens de admin, profesor y alumno
✅ Autenticación por email validada
✅ Ownership validation funcionando
✅ Integración con módulo Asistencias verificada

### Cambios vs Diseño Original
✅ **Implementado con Clean Architecture completa:** CQRS, MediatR, Result Pattern
✅ **Autenticación unificada por email:** En lugar de OID GUID (más consistente)
✅ **Historial de asistencias en detalle:** Incluido en GetPaqueteById y GetMisPaquetes
✅ **Validación de disponibilidad:** Query específica para validar antes de usar paquete
- ❌ Alertas por vencimiento próximo
- ❌ Alertas por clases agotadas

### Endpoints Pendientes
```
POST   /api/paquetes                      [AdminOnly]
GET    /api/paquetes/{id}                 [ApiScope + Ownership]
PUT    /api/paquetes/{id}                 [AdminOnly]
POST   /api/paquetes/{id}/congelar        [AdminOnly]
POST   /api/paquetes/{id}/descongelar     [AdminOnly]
GET    /api/alumnos/{id}/paquetes/activos [ApiScope + Ownership]
```

### Funcionalidades Clave
- Múltiples paquetes activos por alumno
- Vigencia inicia al tomar primera clase
- Sistema de congelaciones con períodos
- Validación de clases disponibles antes de asistencia
- Alertas automáticas (7 días antes de vencer, etc.)

### Relaciones
- **Alumno:** Un paquete pertenece a un alumno
- **Pago:** Origen del paquete
- **TipoPaquete:** Catálogo (Mensual, Bimestral, etc.)
- **CongelacionPaquete:** Períodos de pausa
- **Asistencias:** Descuento de clases

---

## 7. Módulo de Pagos ❌

**Estado:** ❌ No implementado  
**Prioridad:** Media

### Descripción
Registro y consulta de pagos realizados por alumnos, con asociación a paquetes.

### Usuarios Involucrados
- **Administrador:** Registra y consulta pagos
- **Alumno:** Consulta su historial

### Funciones Pendientes (100%)
- ❌ Registrar pago manual (efectivo/transferencia)
- ❌ Adjuntar comprobante (imagen)
- ❌ Asociar pago a uno o más paquetes
- ❌ Historial de pagos por alumno
- ❌ Reporte de ingresos por período

### Endpoints Pendientes
```
POST   /api/pagos                     [AdminOnly]
GET    /api/pagos/{id}                [AdminOnly]
GET    /api/alumnos/{id}/pagos        [ApiScope + Ownership]
GET    /api/pagos/reporte             [AdminOnly]
POST   /api/pagos/{id}/comprobante    [AdminOnly]
```

### Funcionalidades Clave
- Métodos de pago: efectivo, transferencia, etc.
- Adjuntar imagen de comprobante
- Un pago puede generar múltiples paquetes
- Notas administrativas en cada pago
- Reporte de ingresos con filtros

### Relaciones
- **Alumno:** Pagos realizados por alumno
- **Paquetes:** Paquetes generados por pago
- **MetodoPago:** Catálogo de métodos

---

## 8. Módulo de Reportes ❌

**Estado:** ❌ No implementado  
**Prioridad:** Baja - Fase 2

### Descripción
Generación de informes exportables con filtros, adaptados al rol del usuario.

### Usuarios Involucrados
Todos (con diferentes permisos)

### Funciones Pendientes (100%)
- ❌ Reporte de asistencias (día, alumno, clase)
- ❌ Reporte de pagos por período
- ❌ Reporte de clases impartidas por profesor
- ❌ Exportación a PDF con diseño Chetango
- ❌ Exportación a Excel
- ❌ Dashboard con métricas clave

### Endpoints Pendientes
```
GET /api/reportes/asistencias [AdminOnly]
GET /api/reportes/pagos       [AdminOnly]
GET /api/reportes/profesores  [AdminOnly]
GET /api/reportes/dashboard   [Según rol]
```

### Relaciones
Todos los módulos

---

## 9. Módulo de Alertas y Notificaciones ❌

**Estado:** ❌ No implementado  
**Prioridad:** Baja - Fase 2

### Descripción
Sistema de notificaciones sobre eventos importantes del sistema.

### Usuarios Involucrados
Todos

### Funciones Pendientes (100%)
- ❌ Alerta paquete próximo a vencer
- ❌ Alerta paquete agotado
- ❌ Alerta inasistencia prolongada
- ❌ Panel de notificaciones por usuario
- ❌ Configuración de preferencias de alertas
- ❌ (Futuro) Integración WhatsApp

### Endpoints Pendientes
```
GET    /api/notificaciones           [ApiScope]
PUT    /api/notificaciones/{id}/leer [ApiScope]
GET    /api/alertas/activas          [ApiScope]
PUT    /api/preferencias-alertas     [ApiScope]
```

### Relaciones
Paquetes, Asistencias, Alumnos

---

## 🎯 Roadmap de Desarrollo

### ✅ Fase 1: MVP Básico (Completado)
- ✅ Autenticación OAuth 2.0 con CIAM
- ✅ Módulo Asistencias completo
- ✅ Infraestructura base (CQRS, EF Core, Policies)

### 🔥 Fase 2: Completar MVP Funcional (En Curso)
**Prioridad Alta - Próximos pasos:**
1. **Módulo Clases** (CRUD completo)
   - Crear clase
   - Editar clase
   - Consultar clases por profesor
   - Validaciones

2. **Módulo Paquetes** (Gestión básica)
   - Crear paquete
   - Descontar clases
   - Estados y vencimientos

3. **Módulo Pagos** (Registro básico)
   - Registrar pago
   - Asociar a paquetes
   - Historial

### 📋 Fase 3: Features Avanzadas
- Módulo Reportes
- Módulo Alertas
- Congelaciones de paquetes
- Dashboard con métricas
- Exportación PDF/Excel

### 🚀 Fase 4: Optimizaciones
- Integración WhatsApp
- Reserva de clases online
- Pagos online
- App móvil

---

## 📚 Documentación Adicional

- **Contrato API para Frontend:** `docs/API-CONTRACT-FRONTEND.md`
- **Contrato API Clases:** `docs/API-CONTRACT-CLASES.md` ⚡ Actualizado Feb 2026
- **Implementación Clases:** `docs/implementacion-modulo-clases.md` ⚡ Actualizado Feb 2026
- **Matriz de pruebas AuthZ:** `docs/authz-postman-test-matrix.md`
- **Scripts de datos de prueba:** `scripts/seed_usuarios_prueba_ciam.sql`
- **Scripts de corrección:** `scripts/fix_encoding_simple.sql` 🆕
- **Documentación de scripts:** `scripts/README.md`

---

## 🔗 Relaciones Entre Módulos

```
┌─────────────────┐
│  Autenticación  │ → Todos los módulos
└─────────────────┘

┌─────────┐      ┌──────────────┐      ┌─────────────┐
│ Alumno  │─────→│  Paquetes    │─────→│   Pagos     │
└─────────┘      └──────────────┘      └─────────────┘
     │                   │
     │                   ↓
     │            ┌──────────────┐
     └───────────→│ Asistencias  │
                  └──────────────┘
                         ↑
                         │
                  ┌──────────────┐
                  │   Clases     │←─── Profesor
                  └──────────────┘

                  ┌──────────────┐
                  │   Reportes   │ → Consume todos
                  └──────────────┘

                  ┌──────────────┐
                  │   Alertas    │ → Consume Paquetes, Asistencias
                  └──────────────┘
```

---

## 🔧 Historial de Cambios Importantes

### Febrero 2026
- ✅ **Múltiples profesores por clase:** Implementación completa con tabla ClasesProfesores
- ✅ **Corrección de encoding:** Scripts para corregir caracteres Unicode corruptos
- ✅ **Formato tiempo 24h:** Estandarización HH:mm en todo el sistema
- ✅ **Ownership mejorado:** Validación contra todos los profesores asignados

### Enero 2026
- ✅ Catálogo TipoAsistencia con reglas de negocio centralizadas
- ✅ Módulos Paquetes, Pagos y Reportes completados
- ✅ Infraestructura OAuth 2.0 con Microsoft Entra CIAM

---

**Documento generado:** Febrero 2026  
**Versión:** 2.1  
**Mantenedor:** Equipo Backend Chetango

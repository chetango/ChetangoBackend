# 🧩 Módulos del Sistema - Chetango Backend

Este documento detalla los módulos principales de la aplicación Chetango, con descripción funcional, usuarios involucrados, estado de implementación y prioridad de desarrollo.

> **Última actualización:** Enero 2026  
> **Estado general:** MVP Fase 1 - Módulo Asistencias completado

---

## 📊 Estado Global de Implementación

| Módulo | Estado | Completitud | Prioridad |
|--------|--------|-------------|-----------|
| 1. Autenticación y Seguridad | ✅ Completo | 100% | ✅ MVP |
| 2. Asistencias | ✅ Completo | 95% | ✅ MVP |
| 3. Clases | ✅ Completo | 100% | ✅ MVP |
| 4. Alumnos | ⚠️ Básico | 20% | Media |
| 5. Profesores | ⚠️ Básico | 20% | Media |
| 6. Paquetes | ❌ Pendiente | 0% | Media |
| 7. Pagos | ❌ Pendiente | 0% | Media |
| 8. Reportes | ❌ Pendiente | 0% | Baja |
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
Permite registrar y consultar la asistencia de alumnos a clases específicas.

### Usuarios Involucrados
- **Profesores:** Registran asistencias de sus clases
- **Administrador:** Consulta asistencias de todas las clases
- **Alumnos:** Consultan su propio historial de asistencias

### Funciones Implementadas
- ✅ Registrar asistencia (profesor/admin)
- ✅ Actualizar estado de asistencia
- ✅ Consultar asistencias por clase
- ✅ Consultar asistencias por alumno (con validación de ownership)
- ✅ Admin: Días con clases programadas
- ✅ Admin: Clases del día específico
- ✅ Admin: Resumen de asistencias por clase

### Endpoints Disponibles
```
POST   /api/asistencias                          [AdminOrProfesor]
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
    - RegistrarAsistenciaCommand + Handler + Validator
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

### Relaciones
- **Clase:** Una asistencia pertenece a una clase específica
- **Alumno:** Una asistencia registra la presencia de un alumno
- **Paquete:** Descuenta clase del paquete activo (implementado en dominio)

### Pendiente (5%)
- ⚠️ Validación completa de paquetes disponibles al registrar
- ⚠️ Notificaciones al descontar clase de paquete

---

## 3. Módulo de Clases ✅

**Estado:** ✅ Implementado y funcional  
**Prioridad:** ✅ MVP - Crítico

### Descripción
Gestión del calendario de clases: creación, edición, cancelación y consulta con validación de conflictos de horario y ownership.

### Usuarios Involucrados
- **Profesores:** Crean y editan sus propias clases
- **Administrador:** Gestión completa del calendario
- **Alumnos:** Consultan clases disponibles

### Funciones Implementadas (100%)
- ✅ Crear clase (profesor para sí mismo o admin para cualquier profesor)
- ✅ Editar clase (profesor dueño o admin)
- ✅ Cancelar clase (profesor dueño o admin)
- ✅ Consultar detalle de clase por ID (con ownership validation)
- ✅ Consultar clases de un profesor con filtros y paginación
- ✅ Consultar clases de un alumno (ownership validation)
- ✅ Validación de conflictos de horario
- ✅ Validación de ownership (profesores solo gestionan sus clases)
- ✅ Entidad `Clase` con relaciones completas

### Endpoints Disponibles
```
POST   /api/clases                             [AdminOrProfesor + Ownership]
PUT    /api/clases/{id}                        [AdminOrProfesor + Ownership]
DELETE /api/clases/{id}                        [AdminOrProfesor + Ownership]
GET    /api/clases/{id}                        [AdminOrProfesor]
GET    /api/profesores/{idProfesor}/clases     [AdminOrProfesor + Ownership]
GET    /api/alumnos/{idAlumno}/clases          [ApiScope + Ownership]
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
  - Profesor crea clase para sí mismo
  - Admin puede crear clase para cualquier profesor
  - Validación de conflictos de horario
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

## 6. Módulo de Paquetes ❌

**Estado:** ❌ No implementado  
**Prioridad:** Media - Siguiente después de Clases

### Descripción
Gestión de paquetes de clases adquiridos por alumnos: creación, consumo, congelación.

### Usuarios Involucrados
- **Administrador:** Crea y gestiona paquetes
- **Alumno:** Consulta estado de sus paquetes

### Funciones Pendientes (100%)
- ❌ Crear paquete al registrar pago
- ❌ Descontar clase automáticamente al registrar asistencia
- ❌ Congelar/descongelar paquete
- ❌ Calcular vencimiento considerando congelaciones
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
- **Matriz de pruebas AuthZ:** `docs/authz-postman-test-matrix.md`
- **Scripts de datos de prueba:** `scripts/seed_usuarios_prueba_ciam.sql`
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

**Documento generado:** Enero 2026  
**Versión:** 2.0  
**Mantenedor:** Equipo Backend Chetango

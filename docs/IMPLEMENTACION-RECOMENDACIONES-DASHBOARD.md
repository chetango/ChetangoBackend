# 🚀 Implementación de Recomendaciones Dashboard Alumno

## 📋 RESUMEN

Se implementaron 3 funcionalidades nuevas en la sección "Recomendado para Ti" del dashboard del alumno:

### 1. **Renovar Paquete** 📦
- Alumno solicita renovación cuando su paquete está por agotarse
- Notificación al Admin con icono de paquete brillante
- Admin puede aprobar y crear el nuevo paquete

### 2. **Solicitar Clase Privada** ⭐
- Alumno solicita clase privada con preferencias de fecha/hora
- Notificación al Admin con icono de estrella
- Admin puede agendar la clase

### 3. **Invita un Amigo** 🤝
- Sistema de referidos con código único por alumno
- Beneficio: Alumno referidor = 1 clase gratis
- Beneficio: Alumno nuevo = 10% descuento en primer paquete

---

## 🗂️ ARCHIVOS CREADOS

### Backend - Domain (Entidades)
1. `SolicitudRenovacionPaquete.cs` - Entidad para solicitudes de renovación
2. `SolicitudClasePrivada.cs` - Entidad para solicitudes de clases privadas
3. `CodigoReferido.cs` - Entidad para códigos de referido
4. `UsoCodigoReferido.cs` - Entidad para registro de usos

### Backend - Application Layer

**Commands:**
- `SolicitarRenovacionPaqueteCommand.cs` + Handler
- `SolicitarClasePrivadaCommand.cs` + Handler
- `GenerarCodigoReferidoCommand.cs` + Handler

**Queries:**
- `GetSolicitudesRenovacionPendientesQuery.cs` + Handler
- `GetSolicitudesClasePrivadaPendientesQuery.cs` + Handler
- `GetMiCodigoReferidoQuery.cs` + Handler

**DTOs:**
- `SolicitudDTOs.cs` (SolicitudRenovacionPaqueteDTO, SolicitudClasePrivadaDTO)
- `ReferidoDTOs.cs` (CodigoReferidoDTO, UsoCodigoReferidoDTO)

### Backend - Infrastructure

**Configuraciones EF:**
- `SolicitudRenovacionPaqueteConfiguration.cs`
- `SolicitudClasePrivadaConfiguration.cs`
- `CodigoReferidoConfiguration.cs`
- `UsoCodigoReferidoConfiguration.cs`

---

## 🌐 ENDPOINTS IMPLEMENTADOS

### Solicitudes de Renovación de Paquete

```http
POST /api/solicitudes/renovar-paquete
Authorization: Bearer {token} (ApiScope)
Content-Type: application/json

{
  "idTipoPaqueteDeseado": "guid", // opcional
  "mensajeAlumno": "string"       // opcional
}

Response 201: { "idSolicitud": "guid" }
```

```http
GET /api/solicitudes/renovacion-paquete/pendientes
Authorization: Bearer {token} (AdminOnly)

Response 200: [
  {
    "idSolicitud": "guid",
    "idAlumno": "guid",
    "nombreAlumno": "string",
    "correoAlumno": "string",
    "idPaqueteActual": "guid",
    "tipoPaqueteActual": "string",
    "clasesRestantes": 2,
    "tipoPaqueteDeseado": "string",
    "mensajeAlumno": "string",
    "estado": "Pendiente",
    "fechaSolicitud": "datetime",
    "fechaRespuesta": null,
    "mensajeRespuesta": null
  }
]
```

### Solicitudes de Clase Privada

```http
POST /api/solicitudes/clase-privada
Authorization: Bearer {token} (ApiScope)
Content-Type: application/json

{
  "idTipoClaseDeseado": "guid",          // opcional
  "fechaPreferida": "2026-02-15",        // opcional
  "horaPreferida": "18:00:00",           // opcional
  "observacionesAlumno": "Quiero trabajar en giros"
}

Response 201: { "idSolicitud": "guid" }
```

```http
GET /api/solicitudes/clase-privada/pendientes
Authorization: Bearer {token} (AdminOnly)

Response 200: [
  {
    "idSolicitud": "guid",
    "idAlumno": "guid",
    "nombreAlumno": "string",
    "correoAlumno": "string",
    "tipoClaseDeseado": "Tango Salón Privado",
    "fechaPreferida": "2026-02-15",
    "horaPreferida": "18:00",
    "observacionesAlumno": "string",
    "estado": "Pendiente",
    "fechaSolicitud": "datetime",
    "fechaRespuesta": null,
    "mensajeRespuesta": null
  }
]
```

### Sistema de Referidos

```http
GET /api/referidos/mi-codigo
Authorization: Bearer {token} (ApiScope)

Response 200: {
  "idCodigo": "guid",
  "codigo": "JUAN2645",
  "activo": true,
  "vecesUsado": 3,
  "beneficioReferidor": "1 clase gratis",
  "beneficioNuevoAlumno": "10% descuento en primer paquete",
  "fechaCreacion": "datetime"
}
```

```http
POST /api/referidos/generar-codigo
Authorization: Bearer {token} (ApiScope)

Response 201: {
  "idCodigo": "guid",
  "codigo": "JUAN2645",
  "activo": true,
  "vecesUsado": 0,
  "beneficioReferidor": "1 clase gratis",
  "beneficioNuevoAlumno": "10% descuento en primer paquete",
  "fechaCreacion": "datetime"
}
```

---

## 📊 MODELO DE DATOS

### Tabla: SolicitudesRenovacionPaquete
```sql
- IdSolicitud (PK, Guid)
- IdAlumno (FK, Guid) → Alumnos
- IdPaqueteActual (FK, Guid, nullable) → Paquetes
- IdTipoPaqueteDeseado (Guid, nullable)
- TipoPaqueteDeseado (nvarchar(200))
- MensajeAlumno (nvarchar(1000), nullable)
- Estado (nvarchar(50)) DEFAULT 'Pendiente'
- FechaSolicitud (datetime)
- FechaRespuesta (datetime, nullable)
- IdUsuarioRespondio (FK, Guid, nullable) → Usuarios
- MensajeRespuesta (nvarchar(1000), nullable)
- IdPaqueteCreado (FK, Guid, nullable) → Paquetes

Índices:
- IdAlumno
- Estado
- FechaSolicitud
```

### Tabla: SolicitudesClasePrivada
```sql
- IdSolicitud (PK, Guid)
- IdAlumno (FK, Guid) → Alumnos
- IdTipoClaseDeseado (Guid, nullable)
- TipoClaseDeseado (nvarchar(200))
- FechaPreferida (datetime, nullable)
- HoraPreferida (time, nullable)
- ObservacionesAlumno (nvarchar(1000), nullable)
- Estado (nvarchar(50)) DEFAULT 'Pendiente'
- FechaSolicitud (datetime)
- FechaRespuesta (datetime, nullable)
- IdUsuarioRespondio (FK, Guid, nullable) → Usuarios
- MensajeRespuesta (nvarchar(1000), nullable)
- IdClaseCreada (FK, Guid, nullable) → Clases

Índices:
- IdAlumno
- Estado
- FechaSolicitud
```

### Tabla: CodigosReferido
```sql
- IdCodigo (PK, Guid)
- IdAlumno (FK, Guid) → Alumnos
- Codigo (nvarchar(20), UNIQUE)
- Activo (bit) DEFAULT 1
- VecesUsado (int) DEFAULT 0
- BeneficioReferidor (nvarchar(500), nullable)
- BeneficioNuevoAlumno (nvarchar(500), nullable)
- FechaCreacion (datetime)
- FechaModificacion (datetime, nullable)

Índices:
- Codigo (UNIQUE)
- IdAlumno
- Activo
```

### Tabla: UsosCodigoReferido
```sql
- IdUso (PK, Guid)
- IdCodigoReferido (FK, Guid) → CodigosReferido
- IdAlumnoReferidor (FK, Guid) → Alumnos
- IdAlumnoNuevo (FK, Guid) → Alumnos
- FechaUso (datetime)
- Estado (nvarchar(50)) DEFAULT 'Pendiente'
- BeneficioAplicadoReferidor (bit) DEFAULT 0
- FechaBeneficioReferidor (datetime, nullable)
- BeneficioAplicadoNuevo (bit) DEFAULT 0
- FechaBeneficioNuevo (datetime, nullable)
- Observaciones (nvarchar(1000), nullable)

Índices:
- IdCodigoReferido
- IdAlumnoReferidor
- IdAlumnoNuevo
- FechaUso
- Estado
```

---

## 🔄 PRÓXIMOS PASOS

### Migración de Base de Datos
```bash
cd chetango-backend
dotnet ef migrations add AgregarSolicitudesYReferidos --project Chetango.Infrastructure --startup-project Chetango.Api
dotnet ef database update --project Chetango.Infrastructure --startup-project Chetango.Api
```

### Frontend - Componentes a Crear
1. **SolicitudNotification.tsx** - Notificación estilo "zapato de tango" para Admin
2. **CodigoReferidoCard.tsx** - Card para compartir código de referido
3. Actualizar **RecomendadosSection.tsx** con las nuevas acciones

### Admin Dashboard
- Vista de solicitudes pendientes con badges de notificación
- Modal para aprobar/rechazar solicitudes
- Creación de paquete directa desde la solicitud

---

## ✅ VALIDACIONES IMPLEMENTADAS

1. **Renovación de Paquete:**
   - Solo 1 solicitud pendiente por alumno a la vez
   - Verifica que el alumno esté autenticado
   - Captura paquete actual automáticamente

2. **Clase Privada:**
   - Solo 1 solicitud pendiente por alumno en los últimos 7 días
   - Validación de fecha y hora preferidas opcionales
   - Tipo de clase opcional (por defecto "Clase Privada")

3. **Código de Referido:**
   - Código único de 8 caracteres (4 letras + 2 dígitos año + 2 números aleatorios)
   - Ej: JUAN2645, MARI2612
   - 1 código activo por alumno
   - Si ya existe, retorna el existente

---

## 🎨 DISEÑO VISUAL (Pendiente Frontend)

### Notificación Admin - Renovación Paquete
```
┌────────────────────────────────────┐
│  📦 ✨ (Paquete brillante)         │
│────────────────────────────────────│
│  María Rodríguez                   │
│  quiere renovar su paquete         │
│                                    │
│  Paquete actual: 8 Clases          │
│  Clases restantes: 2               │
│  Mensaje: "Me gustaría renovar"    │
│                                    │
│  [Ver Solicitud] [Ignorar]         │
└────────────────────────────────────┘
```

### Notificación Admin - Clase Privada
```
┌────────────────────────────────────┐
│  ⭐ 👑 (Estrella/Corona brillante) │
│────────────────────────────────────│
│  Juan Pérez                        │
│  solicita clase privada            │
│                                    │
│  Tipo: Tango Salón Privado         │
│  Fecha preferida: 15 Feb 2026      │
│  Hora: 18:00                       │
│  Observaciones: "Giros avanzados"  │
│                                    │
│  [Agendar] [Contactar] [Ignorar]   │
└────────────────────────────────────┘
```

---

## 🚀 ARQUITECTURA

**Patrón CQRS:**
- Commands: Modifican estado (solicitudes, generación de códigos)
- Queries: Solo lectura (consultar solicitudes pendientes, mi código)

**Clean Architecture:**
- Domain: Entidades puras sin dependencias
- Application: Lógica de negocio (Commands/Queries/DTOs)
- Infrastructure: EF Core Configurations, DbContext
- API: Endpoints mínimos, delegación a MediatR

**Ownership Validation:**
- Alumno solo puede solicitar para sí mismo (email del JWT)
- Admin puede ver todas las solicitudes pendientes
- Código de referido es personal (por email del alumno)

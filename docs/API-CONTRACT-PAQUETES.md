# 📋 API Contract - Módulo Paquetes

## Base URL
```
https://localhost:7194
```

## Autenticación
Todos los endpoints requieren autenticación mediante **Bearer Token** (OAuth 2.0).

```http
Authorization: Bearer {access_token}
```

---

## 📦 Entidades

### Paquete
```typescript
{
  IdPaquete: Guid,
  IdAlumno: Guid,
  IdTipoPaquete: Guid,
  IdPago?: Guid | null,
  ClasesDisponibles: int,
  ClasesUsadas: int,
  FechaActivacion: DateTime,
  FechaVencimiento: DateTime,
  IdEstado: int, // 1=Activo, 2=Vencido, 3=Congelado, 4=Agotado
  ValorPaquete: decimal,
  FechaCreacion: DateTime,
  FechaModificacion?: DateTime | null,
  UsuarioCreacion: string,
  UsuarioModificacion?: string | null
}
```

### CongelacionPaquete
```typescript
{
  IdCongelacion: Guid,
  IdPaquete: Guid,
  FechaInicio: DateTime,
  FechaFin: DateTime
}
```

### Estados de Paquete
```typescript
enum EstadoPaquete {
  Activo = 1,      // Tiene clases disponibles y no está vencido
  Vencido = 2,     // FechaVencimiento < DateTime.Today
  Congelado = 3,   // Pausado temporalmente
  Agotado = 4      // ClasesUsadas >= ClasesDisponibles
}
```

---

## 📍 Endpoints

### 1. POST /api/paquetes
Crear un nuevo paquete de clases para un alumno.

**Autorización:** `AdminOnly`

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "idTipoPaquete": "550e8400-e29b-41d4-a716-446655440001",
  "clasesDisponibles": 8,
  "valorPaquete": 150000,
  "diasVigencia": 30,
  "idPago": null // opcional
}
```

**Validaciones:**
- `idAlumno`: Requerido, debe existir en la BD y estar activo
- `idTipoPaquete`: Requerido, debe existir en la BD
- `clasesDisponibles`: Requerido, debe ser > 0
- `valorPaquete`: Requerido, debe ser >= 0
- `diasVigencia`: Requerido, debe ser > 0
- `idPago`: Opcional, si se proporciona debe existir en la BD

**Response 201 Created:**
```json
{
  "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678"
}
```

**Response 400 Bad Request:**
```json
{
  "error": "El alumno especificado no existe."
}
```

**Lógica de Negocio:**
- `FechaActivacion` = Fecha actual (hoy)
- `FechaVencimiento` = `FechaActivacion` + `diasVigencia`
- `IdEstado` = 1 (Activo)
- `ClasesUsadas` = 0

---

### 2. GET /api/paquetes/{id}
Obtener el detalle completo de un paquete específico.

**Autorización:** `ApiScope` (con validación de ownership)

**Path Parameters:**
- `id` (Guid): ID del paquete

**Ownership:**
- **Admin**: Puede ver cualquier paquete
- **Alumno**: Solo puede ver SUS propios paquetes (validado por IdAlumno vs IdUsuario del token)

**Response 200 OK:**
```json
{
  "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678",
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "nombreAlumno": "Juan David Perez",
  "idTipoPaquete": "550e8400-e29b-41d4-a716-446655440001",
  "nombreTipoPaquete": "Paquete 8 Clases",
  "clasesDisponibles": 8,
  "clasesUsadas": 2,
  "clasesRestantes": 6,
  "fechaActivacion": "2026-01-11T00:00:00",
  "fechaVencimiento": "2026-02-10T00:00:00",
  "valorPaquete": 150000,
  "idEstado": 1,
  "estado": "Activo",
  "estaVencido": false,
  "tieneClasesDisponibles": true,
  "congelaciones": [
    {
      "idCongelacion": "11223344-5566-7788-99aa-bbccddeeff00",
      "fechaInicio": "2026-01-15T00:00:00",
      "fechaFin": "2026-01-22T00:00:00",
      "diasCongelados": 7
    }
  ]
}
```

**Response 403 Forbidden:**
```json
{
  "error": "No tienes permiso para ver este paquete."
}
```

**Response 404 Not Found:**
```json
{
  "error": "El paquete especificado no existe."
}
```

---

### 3. PUT /api/paquetes/{id}
Editar un paquete existente (ajuste administrativo).

**Autorización:** `AdminOnly`

**Path Parameters:**
- `id` (Guid): ID del paquete

**Request Body:**
```json
{
  "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678",
  "clasesDisponibles": 10,
  "fechaVencimiento": "2026-02-20T00:00:00"
}
```

**Validaciones:**
- `idPaquete`: Debe coincidir con el ID en la ruta
- `clasesDisponibles`: Debe ser >= `clasesUsadas` (no se puede reducir por debajo de las ya usadas)
- `fechaVencimiento`: Requerido

**Response 204 No Content**

**Response 400 Bad Request:**
```json
{
  "error": "Las clases disponibles (5) no pueden ser menores a las clases ya usadas (7)."
}
```

**Lógica de Negocio:**
- Recalcula automáticamente el estado:
  - Si `ClasesUsadas >= ClasesDisponibles` → Estado = Agotado (4)
  - Si `FechaVencimiento < DateTime.Today` → Estado = Vencido (2)
  - Si no está congelado y tiene clases → Estado = Activo (1)

---

### 4. POST /api/paquetes/{id}/congelar
Congelar un paquete (pausa el vencimiento temporalmente).

**Autorización:** `AdminOnly`

**Path Parameters:**
- `id` (Guid): ID del paquete

**Request Body:**
```json
{
  "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678",
  "fechaInicio": "2026-01-15",
  "fechaFin": "2026-01-22",
  "motivo": "Viaje del alumno" // opcional
}
```

**Validaciones:**
- `idPaquete`: Debe coincidir con el ID en la ruta
- Paquete debe estar en estado **Activo** (no se puede congelar si ya está Vencido/Agotado/Congelado)
- `fechaInicio` < `fechaFin`
- `fechaInicio` >= hoy
- No debe haber solapamiento con otras congelaciones existentes

**Response 200 OK:**
```json
{
  "mensaje": "Paquete congelado exitosamente"
}
```

**Response 400 Bad Request:**
```json
{
  "error": "Solo se pueden congelar paquetes activos (estado actual: Vencido)."
}
```

**Lógica de Negocio:**
- Cambia `IdEstado` a 3 (Congelado)
- Crea registro en tabla `CongelacionesPaquete`

---

### 5. POST /api/paquetes/{id}/descongelar
Descongelar un paquete y extender su vencimiento.

**Autorización:** `AdminOnly`

**Path Parameters:**
- `id` (Guid): ID del paquete

**Query Parameters:**
- `idCongelacion` (Guid): ID de la congelación a cerrar

**Example:**
```http
POST /api/paquetes/aabbccdd-1234-5678-90ab-cdef12345678/descongelar?idCongelacion=11223344-5566-7788-99aa-bbccddeeff00
```

**Validaciones:**
- Paquete debe estar en estado **Congelado**
- La congelación debe existir y pertenecer al paquete

**Response 200 OK:**
```json
{
  "mensaje": "Paquete descongelado exitosamente"
}
```

**Response 400 Bad Request:**
```json
{
  "error": "El paquete no está congelado (estado actual: Activo)."
}
```

**Lógica de Negocio:**
1. Actualiza `FechaFin` de la congelación a hoy
2. Calcula días congelados: `diasCongelados = (FechaFin - FechaInicio).Days`
3. Extiende vencimiento: `FechaVencimiento += diasCongelados`
4. Recalcula estado:
   - Si `ClasesUsadas >= ClasesDisponibles` → Agotado (4)
   - Si `FechaVencimiento < hoy` → Vencido (2)
   - Si hay clases disponibles → Activo (1)

---

### 6. GET /api/alumnos/{idAlumno}/paquetes
Listar todos los paquetes de un alumno con filtros y paginación.

**Autorización:** `ApiScope` (con validación de ownership)

**Path Parameters:**
- `idAlumno` (Guid): ID del alumno

**Query Parameters:**
- `soloActivos` (bool, default: `true`) - Filtrar solo paquetes activos
- `estado` (int, opcional) - Filtrar por estado específico (1-4)
- `fechaVencimientoDesde` (DateTime, opcional) - Filtrar desde esta fecha
- `fechaVencimientoHasta` (DateTime, opcional) - Filtrar hasta esta fecha
- `pageNumber` (int, default: 1) - Número de página
- `pageSize` (int, default: 10) - Tamaño de página

**Ownership:**
- **Admin**: Puede ver paquetes de cualquier alumno
- **Alumno**: Solo puede ver SUS propios paquetes

**Example:**
```http
GET /api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/paquetes?soloActivos=true&pageSize=5
GET /api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/paquetes?estado=1&fechaVencimientoDesde=2026-01-01
```

**Response 200 OK:**
```json
{
  "items": [
    {
      "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678",
      "nombreTipoPaquete": "Paquete 8 Clases",
      "clasesDisponibles": 8,
      "clasesUsadas": 2,
      "clasesRestantes": 6,
      "fechaActivacion": "2026-01-11T00:00:00",
      "fechaVencimiento": "2026-02-10T00:00:00",
      "valorPaquete": 150000,
      "estado": "Activo",
      "estaVencido": false,
      "tieneClasesDisponibles": true
    }
  ],
  "pageNumber": 1,
  "totalPages": 1,
  "totalCount": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

**Response 403 Forbidden:**
```json
{
  "error": "No tienes permiso para ver los paquetes de otro alumno."
}
```

---

## 🔄 Integración con Asistencias

### Descuento Automático de Clases

Al registrar una asistencia con estado **Presente** (`idEstadoAsistencia = 1`), el sistema automáticamente:

1. **Valida** que el paquete esté disponible (`ValidarPaqueteDisponibleQuery`):
   - Estado = Activo (no Vencido/Congelado/Agotado)
   - ClasesRestantes > 0
   - FechaVencimiento > hoy

2. **Descuenta** una clase del paquete (`DescontarClaseCommand`):
   - `ClasesUsadas++`
   - Si `ClasesUsadas >= ClasesDisponibles` → Estado cambia a Agotado (4)

**Importante:** Los estados **Ausente** (2) y **Justificada** (3) **NO** descontarán clases del paquete.

### Endpoint de Asistencia Modificado

**POST /api/asistencias**

```json
{
  "idClase": "550e8400-e29b-41d4-a716-446655440010",
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "idPaqueteUsado": "aabbccdd-1234-5678-90ab-cdef12345678",
  "idEstadoAsistencia": 1, // 1=Presente (descuenta), 2=Ausente, 3=Justificada
  "observaciones": "Clase regular"
}
```

**Errores Relacionados con Paquetes:**
```json
// Paquete agotado
{
  "error": "El paquete no tiene clases disponibles."
}

// Paquete congelado
{
  "error": "El paquete no está activo (estado: Congelado)."
}

// Paquete vencido
{
  "error": "El paquete está vencido."
}
```

---

## 🔐 Políticas de Autorización

| Endpoint | Política | Descripción |
|----------|----------|-------------|
| POST /api/paquetes | `AdminOnly` | Solo administradores |
| GET /api/paquetes/{id} | `ApiScope` | Ownership: admin o dueño |
| PUT /api/paquetes/{id} | `AdminOnly` | Solo administradores |
| POST /api/paquetes/{id}/congelar | `AdminOnly` | Solo administradores |
| POST /api/paquetes/{id}/descongelar | `AdminOnly` | Solo administradores |
| GET /api/alumnos/{id}/paquetes | `ApiScope` | Ownership: admin o dueño |

**Definiciones de Políticas:**
- `AdminOnly`: Requiere rol "admin" o "Administrador"
- `ApiScope`: Requiere scope "chetango.api" (cualquier usuario autenticado)
  - Ownership validation se hace en el handler según el usuario del token

---

## 📊 Reglas de Negocio

### Estados de Paquete

1. **Activo (1):**
   - Tiene clases disponibles (`ClasesRestantes > 0`)
   - No está vencido (`FechaVencimiento >= hoy`)
   - Puede usarse para registrar asistencias

2. **Vencido (2):**
   - `FechaVencimiento < DateTime.Today`
   - No se puede usar para asistencias

3. **Congelado (3):**
   - Pausado temporalmente por admin
   - No se puede usar para asistencias
   - Al descongelar, se extiende el vencimiento

4. **Agotado (4):**
   - `ClasesUsadas >= ClasesDisponibles`
   - No se puede usar para asistencias

### Transiciones de Estado

```
Creación → Activo (1)
    ↓
Congelar → Congelado (3)
    ↓
Descongelar → Activo (1)
    ↓
Usar última clase → Agotado (4)
    ↓
Vence fecha → Vencido (2)
```

### Múltiples Paquetes

- Un alumno puede tener **varios paquetes activos simultáneamente**
- Al registrar asistencia, se especifica explícitamente cuál paquete usar (`idPaqueteUsado`)
- No hay orden automático de consumo (el profesor/admin elige cuál usar)

---

## 🧪 Testing

Ver documento completo de casos de prueba: [test-modulo-paquetes.md](./test-modulo-paquetes.md)

### Endpoints Base de Prueba

```bash
# Crear paquete
curl -X POST https://localhost:7194/api/paquetes \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
    "idTipoPaquete": "550e8400-e29b-41d4-a716-446655440001",
    "clasesDisponibles": 8,
    "valorPaquete": 150000,
    "diasVigencia": 30
  }'

# Listar paquetes
curl -X GET "https://localhost:7194/api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/paquetes?soloActivos=true" \
  -H "Authorization: Bearer {token}"
```

---

## 📝 Notas de Implementación

### DTOs

Ubicación: `Chetango.Application/Paquetes/DTOs/`

- `CrearPaqueteDTO`
- `EditarPaqueteDTO`
- `CongelarPaqueteDTO`
- `PaqueteAlumnoDTO` (lista)
- `PaqueteDetalleDTO` (detalle completo)
- `CongelacionDTO`

### Commands

Ubicación: `Chetango.Application/Paquetes/Commands/`

- `CrearPaqueteCommand`
- `EditarPaqueteCommand`
- `CongelarPaqueteCommand`
- `DescongelarPaqueteCommand`
- `DescontarClaseCommand` (interno, llamado desde RegistrarAsistenciaHandler)

### Queries

Ubicación: `Chetango.Application/Paquetes/Queries/`

- `GetPaqueteByIdQuery`
- `GetPaquetesDeAlumnoQuery`
- `ValidarPaqueteDisponibleQuery` (interno, llamado desde RegistrarAsistenciaHandler)

### Validators

Todos los commands incluyen su validator usando **FluentValidation**:
- `CrearPaqueteCommandValidator`
- `EditarPaqueteCommandValidator`
- `CongelarPaqueteCommandValidator`
- `DescongelarPaqueteCommandValidator`
- `DescontarClaseCommandValidator`

---

**Fecha de Creación:** 11 de Enero, 2026  
**Versión:** 1.0  
**Arquitectura:** Clean Architecture + CQRS + MediatR

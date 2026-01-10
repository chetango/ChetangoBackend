# Chetango API - Contrato Módulo Clases

**Fecha:** 2026-01-10  
**Versión API:** 1.0  
**Base URL:** `https://localhost:7194`  
**Autenticación:** Bearer Token (OAuth 2.0)

---

## 📋 Índice

1. [Resumen del Módulo](#resumen-del-módulo)
2. [Endpoints Disponibles](#endpoints-disponibles)
3. [DTOs y Modelos](#dtos-y-modelos)
4. [Matriz de Permisos](#matriz-de-permisos)
5. [Ejemplos de Uso](#ejemplos-de-uso)
6. [Códigos de Error](#códigos-de-error)

---

## 🎯 Resumen del Módulo

El módulo de **Clases** permite gestionar la programación de clases de baile, incluyendo:
- ✅ Crear clases (Admin o Profesor)
- ✅ Editar clases (Admin o Profesor dueño)
- ✅ Cancelar clases (Admin o Profesor dueño)
- ✅ Consultar clases individuales y listados
- ✅ Catálogos para dropdowns (tipos de clase, profesores, alumnos, paquetes)

**Características clave:**
- Validación de horarios (sin conflictos)
- Ownership validation (profesores solo editan sus clases)
- Paginación en listados
- Soporte de cupo máximo y observaciones

---

## 📡 Endpoints Disponibles

### 1. Catálogos / Lookups

#### 1.1 GET /api/tipos-clase
Obtiene todos los tipos de clase disponibles para dropdowns.

**Autenticación:** Requerida  
**Roles permitidos:** Todos  

**Response 200:**
```json
[
  {
    "id": "c423206d-c96a-471e-b65c-409901f9a25a",
    "nombre": "Tango Salon"
  },
  {
    "id": "70b40bbb-17e7-42dc-b376-59e0d462f9af",
    "nombre": "Tango Escenario"
  },
  {
    "id": "c343c7e2-febe-4a5f-8d75-9baa62c06af9",
    "nombre": "Elencos Formativos"
  },
  {
    "id": "81241cc2-c6a3-46bc-9b01-57d06607a3ad",
    "nombre": "Tango Salon Privado"
  },
  {
    "id": "e29aedbc-a206-4e19-9f97-91c517c2663e",
    "nombre": "Tango Escenario Privado"
  }
]
```

---

#### 1.2 GET /api/profesores
Obtiene todos los profesores activos para dropdowns.

**Autenticación:** Requerida  
**Roles permitidos:** Admin solamente  

**Response 200:**
```json
[
  {
    "idProfesor": "8f6e460d-328d-4a40-89e3-b8effa76829c",
    "nombreCompleto": "Jorge Padilla",
    "tipoProfesor": "Titular"
  }
]
```

---

#### 1.3 GET /api/alumnos
Obtiene todos los alumnos activos para dropdowns.

**Autenticación:** Requerida  
**Roles permitidos:** Todos  

**Response 200:**
```json
[
  {
    "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
    "nombreCompleto": "Juan David Ramirez",
    "correo": "juandavid@chetangoprueba.onmicrosoft.com"
  }
]
```

---

#### 1.4 GET /api/alumnos/{idAlumno}/paquetes
Obtiene los paquetes activos de un alumno específico.

**Autenticación:** Requerida  
**Roles permitidos:** Todos  

**Path Parameters:**
- `idAlumno` (Guid): ID del alumno

**Response 200:**
```json
[
  {
    "idPaquete": "abc123...",
    "nombreTipoPaquete": "Paquete 8 Clases",
    "clasesDisponibles": 5,
    "fechaVencimiento": "2026-02-28T00:00:00"
  }
]
```

---

### 2. Gestión de Clases (CRUD)

#### 2.1 POST /api/clases
Crear una nueva clase.

**Autenticación:** Requerida  
**Roles permitidos:** Admin, Profesor  

**Ownership:**
- Admin: puede crear clases para cualquier profesor
- Profesor: solo puede crear sus propias clases

**Request Body:**
```json
{
  "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "idTipoClase": "c423206d-c96a-471e-b65c-409901f9a25a",
  "fecha": "2026-01-20T00:00:00",
  "horaInicio": "18:00:00",
  "horaFin": "19:30:00",
  "cupoMaximo": 20,
  "observaciones": "Clase nivel principiante"
}
```

**Validaciones:**
- Fecha y hora deben ser futuras
- Hora fin > Hora inicio
- No puede haber conflicto de horario con otras clases del profesor
- Tipo de clase debe existir
- Profesor debe existir

**Response 201:**
```json
{
  "idClase": "e4ac8d43-e70f-4452-809e-d162df481155"
}
```

**Response 400:**
```json
{
  "error": "El profesor ya tiene una clase programada en ese horario."
}
```

---

#### 2.2 PUT /api/clases/{id}
Editar una clase existente.

**Autenticación:** Requerida  
**Roles permitidos:** Admin, Profesor (solo su clase)  

**Path Parameters:**
- `id` (Guid): ID de la clase a editar

**Request Body:**
```json
{
  "idTipoClase": "70b40bbb-17e7-42dc-b376-59e0d462f9af",
  "idProfesor": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "fechaHoraInicio": "2026-01-20T19:00:00",
  "duracionMinutos": 120,
  "cupoMaximo": 30,
  "observaciones": "Clase extendida - 2 horas"
}
```

**Campos editables:**
- ✅ Tipo de clase
- ✅ Profesor (solo admin)
- ✅ Fecha y hora
- ✅ Duración
- ✅ Cupo máximo
- ✅ Observaciones

**Validaciones:**
- Mismas validaciones que crear
- Ownership: profesor solo puede editar sus propias clases

**Response 204:** No Content (éxito)

**Response 400:**
```json
{
  "error": "No tienes permiso para editar esta clase."
}
```

---

#### 2.3 DELETE /api/clases/{id}
Cancelar/eliminar una clase.

**Autenticación:** Requerida  
**Roles permitidos:** Admin, Profesor (solo su clase)  

**Path Parameters:**
- `id` (Guid): ID de la clase a cancelar

**Response 204:** No Content (éxito)

**Response 400:**
```json
{
  "error": "No puedes cancelar una clase que ya tiene asistencias registradas."
}
```

---

#### 2.4 GET /api/clases/{id}
Obtener detalles de una clase específica.

**Autenticación:** Requerida  
**Roles permitidos:** Todos  

**Path Parameters:**
- `id` (Guid): ID de la clase

**Response 200:**
```json
{
  "idClase": "e4ac8d43-e70f-4452-809e-d162df481155",
  "fecha": "2026-01-15T00:00:00",
  "horaInicio": "18:00:00",
  "horaFin": "20:00:00",
  "tipoClase": "Tango Salon",
  "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "nombreProfesor": "Jorge Padilla",
  "cupoMaximo": 30,
  "observaciones": "Clase extendida - 2 horas",
  "totalAsistencias": 0,
  "monitores": []
}
```

**Response 404:**
```json
{
  "error": "La clase especificada no existe."
}
```

---

#### 2.5 GET /api/profesores/{idProfesor}/clases
Listar clases de un profesor (con paginación).

**Autenticación:** Requerida  
**Roles permitidos:** Admin, Profesor (solo sus clases)  

**Path Parameters:**
- `idProfesor` (Guid): ID del profesor

**Query Parameters:**
- `fechaDesde` (DateTime, opcional): Filtrar desde esta fecha
- `fechaHasta` (DateTime, opcional): Filtrar hasta esta fecha
- `pagina` (int, default: 1): Número de página
- `tamanoPagina` (int, default: 10): Registros por página

**Ejemplo:**
```
GET /api/profesores/8f6e460d-328d-4a40-89e3-b8effa76829c/clases?fechaDesde=2026-01-01&fechaHasta=2026-12-31&pagina=1&tamanoPagina=20
```

**Response 200:**
```json
{
  "items": [
    {
      "idClase": "e4ac8d43-e70f-4452-809e-d162df481155",
      "fecha": "2026-01-15T00:00:00",
      "horaInicio": "18:00:00",
      "horaFin": "20:00:00",
      "tipoClase": "Tango Salon",
      "cupoMaximo": 30,
      "totalAsistencias": 12
    }
  ],
  "paginaActual": 1,
  "totalPaginas": 3,
  "tamanoPagina": 20,
  "totalRegistros": 45,
  "tienePaginaAnterior": false,
  "tienePaginaSiguiente": true
}
```

---

## 📦 DTOs y Modelos

### CrearClaseDTO
```typescript
interface CrearClaseDTO {
  idProfesorPrincipal: string;  // Guid
  idTipoClase: string;          // Guid
  fecha: string;                // DateTime ISO 8601
  horaInicio: string;           // TimeSpan "HH:mm:ss"
  horaFin: string;              // TimeSpan "HH:mm:ss"
  cupoMaximo: number;           // int
  observaciones?: string;       // string nullable
}
```

### EditarClaseDTO
```typescript
interface EditarClaseDTO {
  idTipoClase: string;          // Guid
  idProfesor: string;           // Guid
  fechaHoraInicio: string;      // DateTime ISO 8601
  duracionMinutos: number;      // int
  cupoMaximo: number;           // int
  observaciones?: string;       // string nullable
}
```

### ClaseDetalleDTO
```typescript
interface ClaseDetalleDTO {
  idClase: string;              // Guid
  fecha: string;                // DateTime ISO 8601
  horaInicio: string;           // TimeSpan "HH:mm:ss"
  horaFin: string;              // TimeSpan "HH:mm:ss"
  tipoClase: string;            // Nombre del tipo
  idProfesorPrincipal: string;  // Guid
  nombreProfesor: string;
  cupoMaximo: number;
  observaciones?: string;
  totalAsistencias: number;
  monitores: MonitorClaseDTO[];
}

interface MonitorClaseDTO {
  idProfesor: string;           // Guid
  nombreProfesor: string;
}
```

### TipoClaseDTO
```typescript
interface TipoClaseDTO {
  id: string;                   // Guid
  nombre: string;
}
```

### ProfesorDTO
```typescript
interface ProfesorDTO {
  idProfesor: string;           // Guid
  nombreCompleto: string;
  tipoProfesor: string;         // "Titular" | "Monitor"
}
```

### AlumnoDTO
```typescript
interface AlumnoDTO {
  idAlumno: string;             // Guid
  nombreCompleto: string;
  correo: string;
}
```

### PaqueteAlumnoDTO
```typescript
interface PaqueteAlumnoDTO {
  idPaquete: string;            // Guid
  nombreTipoPaquete: string;
  clasesDisponibles: number;
  fechaVencimiento: string;     // DateTime ISO 8601
}
```

---

## 🔒 Matriz de Permisos

| Endpoint | Admin | Profesor | Alumno | Ownership |
|----------|-------|----------|--------|-----------|
| GET /api/tipos-clase | ✅ | ✅ | ✅ | N/A |
| GET /api/profesores | ✅ | ❌ | ❌ | N/A |
| GET /api/alumnos | ✅ | ✅ | ✅ | N/A |
| GET /api/alumnos/{id}/paquetes | ✅ | ✅ | ✅ | N/A |
| POST /api/clases | ✅ | ✅* | ❌ | Profesor solo crea sus clases |
| PUT /api/clases/{id} | ✅ | ✅* | ❌ | Profesor solo edita sus clases |
| DELETE /api/clases/{id} | ✅ | ✅* | ❌ | Profesor solo cancela sus clases |
| GET /api/clases/{id} | ✅ | ✅ | ✅ | N/A |
| GET /api/profesores/{id}/clases | ✅ | ✅* | ❌ | Profesor solo ve sus clases |

**\*** Profesor solo puede operar sobre sus propias clases

---

## 💡 Ejemplos de Uso

### Flujo: Crear una Clase (Profesor)

**Paso 1: Obtener Tipos de Clase**
```http
GET /api/tipos-clase
Authorization: Bearer {token}
```

**Paso 2: Crear Clase**
```http
POST /api/clases
Authorization: Bearer {token}
Content-Type: application/json

{
  "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "idTipoClase": "c423206d-c96a-471e-b65c-409901f9a25a",
  "fecha": "2026-01-20T00:00:00",
  "horaInicio": "18:00:00",
  "horaFin": "19:30:00",
  "cupoMaximo": 20,
  "observaciones": "Clase nivel principiante"
}
```

---

### Flujo: Editar una Clase

**Paso 1: Obtener Detalles**
```http
GET /api/clases/e4ac8d43-e70f-4452-809e-d162df481155
Authorization: Bearer {token}
```

**Paso 2: Editar Clase**
```http
PUT /api/clases/e4ac8d43-e70f-4452-809e-d162df481155
Authorization: Bearer {token}
Content-Type: application/json

{
  "idTipoClase": "70b40bbb-17e7-42dc-b376-59e0d462f9af",
  "idProfesor": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "fechaHoraInicio": "2026-01-20T19:00:00",
  "duracionMinutos": 120,
  "cupoMaximo": 30,
  "observaciones": "Clase extendida"
}
```

---

### Flujo: Listar Clases de Enero 2026

```http
GET /api/profesores/8f6e460d-328d-4a40-89e3-b8effa76829c/clases?fechaDesde=2026-01-01&fechaHasta=2026-01-31&pagina=1&tamanoPagina=50
Authorization: Bearer {token}
```

---

## ⚠️ Códigos de Error

### 400 Bad Request
```json
{
  "error": "La clase debe programarse para una fecha y hora futura."
}
```

**Causas comunes:**
- Fecha/hora pasada
- Conflicto de horario
- Tipo de clase no existe
- Profesor no existe
- Ownership violation

### 401 Unauthorized
Sin token o token inválido/expirado.

### 403 Forbidden
Token válido pero sin permisos para el endpoint.

### 404 Not Found
```json
{
  "error": "La clase especificada no existe."
}
```

### 204 No Content
Operación exitosa (PUT/DELETE) sin body en la respuesta.

---

## 📝 Notas Importantes

### TimeSpan vs DateTime
- **`fecha`**: Solo fecha sin hora (`"2026-01-20T00:00:00"`)
- **`horaInicio/horaFin`**: Solo hora (`"18:00:00"`)
- **`fechaHoraInicio`**: Fecha + hora combinadas (`"2026-01-20T18:00:00"`)

### Validaciones de Negocio
1. **Fecha futura**: Las clases solo se pueden crear/editar para fechas futuras
2. **Conflictos de horario**: No puede haber sobreposición de clases del mismo profesor
3. **Duración mínima**: 30 minutos
4. **Cupo mínimo**: 1 persona

### Ownership
- **Profesor**: Solo puede crear/editar/cancelar sus propias clases
- **Admin**: Puede operar sobre cualquier clase

### Paginación
- Por defecto: 10 registros por página
- Máximo recomendado: 100 registros por página
- La respuesta incluye metadatos para navegación

---

## 🚀 Testing Rápido

### Postman Collection
Importa la colección "QA Clases" que incluye:
- Variables de entorno (token, URLs)
- Todos los endpoints pre-configurados
- Herencia de autenticación

### cURL Examples

**Crear Clase:**
```bash
curl -X POST https://localhost:7194/api/clases \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
    "idTipoClase": "c423206d-c96a-471e-b65c-409901f9a25a",
    "fecha": "2026-01-20T00:00:00",
    "horaInicio": "18:00:00",
    "horaFin": "19:30:00",
    "cupoMaximo": 20
  }'
```

**Listar Clases:**
```bash
curl -X GET "https://localhost:7194/api/profesores/8f6e460d-328d-4a40-89e3-b8effa76829c/clases?pagina=1&tamanoPagina=10" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

**Última actualización:** 2026-01-10  
**Contacto Backend:** Equipo Chetango Backend

# 🧪 Casos de Prueba - Módulo Clases

## 📋 Información General

**Base URL:** `https://localhost:7194`  
**Autenticación:** Bearer Token (OAuth 2.0 - Microsoft Entra External ID)

### Usuarios de Prueba

| Usuario | Email | Rol | IdUsuario | IdProfesor |
|---------|-------|-----|-----------|------------|
| Admin | Chetango@chetangoprueba.onmicrosoft.com | admin | b91e51b9-4094-441e-a5b6-062a846b3868 | - |
| Profesor Jorge | Jorgepadilla@chetangoprueba.onmicrosoft.com | profesor | 8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB | 8f6e460d-328d-4a40-89e3-b8effa76829c |
| Alumno Juan David | JuanDavid@chetangoprueba.onmicrosoft.com | alumno | 71462106-9863-4fd0-b13d-9878ed231aa6 | - |

---

## 🔐 Autenticación

### 1. Obtener Token
**Método:** Usar OAuth 2.0 Authorization Code + PKCE con Microsoft Entra External ID

**Configuración en Postman:**
- Authorization Type: OAuth 2.0
- Grant Type: Authorization Code (With PKCE)
- Auth URL: `https://chetangoprueba.ciamlogin.com/chetangoprueba.onmicrosoft.com/oauth2/v2.0/authorize`
- Access Token URL: `https://chetangoprueba.ciamlogin.com/chetangoprueba.onmicrosoft.com/oauth2/v2.0/token`
- Client ID: (de appsettings.json)
- Scope: `openid profile email api://{ClientId}/chetango.api`

### 2. Verificar Token
```http
GET /auth/ping
Authorization: Bearer {{token}}
```

**Respuesta Esperada (200):**
```json
{
  "message": "pong",
  "oid": "8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB"
}
```

---

## 📚 Endpoints del Módulo Clases

### 0. Endpoints de Catálogos (Previos a Crear Clase)

#### GET /api/tipos-clase - Obtener Tipos de Clase Disponibles

**Endpoint:** `GET /api/tipos-clase`  
**Autorización:** ApiScope (Cualquier usuario autenticado)  
**Usuario:** Admin, Profesor, Alumno

**Respuesta Esperada (200 OK):**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "nombre": "Milonga"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "nombre": "Práctica"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440003",
    "nombre": "Tango"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440004",
    "nombre": "Técnica"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440005",
    "nombre": "Vals"
  }
]
```

**Uso:** Copiar el `id` del tipo de clase deseado para usarlo en el request de crear clase.

---

#### GET /api/profesores - Obtener Profesores (Solo Admin)

**Endpoint:** `GET /api/profesores`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Respuesta Esperada (200 OK):**
```json
[
  {
    "idProfesor": "8f6e460d-328d-4a40-89e3-b8effa76829c",
    "idUsuario": "8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB",
    "nombre": "Jorge Padilla",
    "correo": "Jorgepadilla@chetangoprueba.onmicrosoft.com"
  }
]
```

**Uso:** Admin puede ver todos los profesores disponibles para crear clases.

---

#### GET /api/alumnos - Obtener Alumnos

**Endpoint:** `GET /api/alumnos`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin, Profesor

**Respuesta Esperada (200 OK):**
```json
[
  {
    "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
    "idUsuario": "71462106-9863-4fd0-b13d-9878ed231aa6",
    "nombre": "Juan David",
    "correo": "JuanDavid@chetangoprueba.onmicrosoft.com"
  }
]
```

**Uso:** Necesario para registrar asistencias (seleccionar alumno).

---

#### GET /api/alumnos/{idAlumno}/paquetes - Obtener Paquetes de un Alumno

**Endpoint:** `GET /api/alumnos/{idAlumno}/paquetes?soloActivos=true`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin, Profesor

**Query Parameters:**
- `soloActivos` (opcional, default=true): Filtrar solo paquetes activos

**Respuesta Esperada (200 OK):**
```json
[
  {
    "idPaquete": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
    "clasesDisponibles": 10,
    "clasesUsadas": 3,
    "clasesRestantes": 7,
    "fechaVencimiento": "2026-06-30T00:00:00",
    "estado": "Activo",
    "estaVencido": false,
    "tieneClasesDisponibles": true
  }
]
```

**Uso:** Validar qué paquetes tiene disponibles el alumno antes de registrar asistencia.

---

### 1. Crear Clase (Admin)

**Endpoint:** `POST /api/clases`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin

**Request Body:**
```json
{
  "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "idTipoClase": "{{IdTipoClase-Tango}}",
  "fecha": "2026-01-15",
  "horaInicio": "18:00:00",
  "horaFin": "19:30:00"
}
```

**Respuesta Esperada (201 Created):**
```json
{
  "idClase": "a1b2c3d4-e5f6-7890-1234-567890abcdef"
}
```

**Casos de Error:**
- **400 Bad Request:** Fecha pasada, conflicto de horario, tipo de clase no existe
- **401 Unauthorized:** Token inválido o expirado
- **403 Forbidden:** Usuario no tiene rol AdminOrProfesor

---

### 2. Crear Clase (Profesor para sí mismo)

**Endpoint:** `POST /api/clases`  
**Autorización:** AdminOrProfesor  
**Usuario:** Profesor Jorge

**Request Body:**
```json
{
  "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "idTipoClase": "{{IdTipoClase-Vals}}",
  "fecha": "2026-01-16",
  "horaInicio": "19:00:00",
  "horaFin": "20:00:00"
}
```

**Respuesta Esperada (201 Created):**
```json
{
  "idClase": "b2c3d4e5-f6a7-8901-2345-67890abcdef1"
}
```

---

### 3. Crear Clase (Profesor para otro profesor - debe fallar)

**Endpoint:** `POST /api/clases`  
**Autorización:** AdminOrProfesor  
**Usuario:** Profesor Jorge

**Request Body:**
```json
{
  "idProfesorPrincipal": "otro-profesor-id",
  "idTipoClase": "{{IdTipoClase-Tango}}",
  "fecha": "2026-01-17",
  "horaInicio": "18:00:00",
  "horaFin": "19:00:00"
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "No tienes permiso para crear clases para otro profesor."
}
```

---

### 4. Crear Clase con Conflicto de Horario

**Endpoint:** `POST /api/clases`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin

**Prerequisito:** Ya existe una clase del Profesor Jorge el 2026-01-15 de 18:00 a 19:30

**Request Body:**
```json
{
  "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "idTipoClase": "{{IdTipoClase-Milonga}}",
  "fecha": "2026-01-15",
  "horaInicio": "18:30:00",
  "horaFin": "20:00:00"
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "El profesor ya tiene una clase programada en ese horario."
}
```

---

### 5. Editar Clase (Admin)

**Endpoint:** `PUT /api/clases/{idClase}`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin

**Request Body:**
```json
{
  "idTipoClase": "{{IdTipoClase-Tecnica}}",
  "fecha": "2026-01-15",
  "horaInicio": "20:00:00",
  "horaFin": "21:00:00"
}
```

**Respuesta Esperada (204 No Content)**

---

### 6. Editar Clase (Profesor dueño)

**Endpoint:** `PUT /api/clases/{idClase}`  
**Autorización:** AdminOrProfesor  
**Usuario:** Profesor Jorge

**Request Body:**
```json
{
  "idTipoClase": "{{IdTipoClase-Vals}}",
  "fecha": "2026-01-16",
  "horaInicio": "19:30:00",
  "horaFin": "20:30:00"
}
```

**Respuesta Esperada (204 No Content)**

---

### 7. Editar Clase (Profesor no dueño - debe fallar)

**Endpoint:** `PUT /api/clases/{idClase-de-otro-profesor}`  
**Autorización:** AdminOrProfesor  
**Usuario:** Profesor Jorge

**Request Body:**
```json
{
  "idTipoClase": "{{IdTipoClase-Tango}}",
  "fecha": "2026-01-18",
  "horaInicio": "18:00:00",
  "horaFin": "19:00:00"
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "No tienes permiso para editar esta clase."
}
```

---

### 8. Cancelar Clase (Admin)

**Endpoint:** `DELETE /api/clases/{idClase}`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin

**Respuesta Esperada (204 No Content)**

---

### 9. Cancelar Clase (Profesor dueño)

**Endpoint:** `DELETE /api/clases/{idClase}`  
**Autorización:** AdminOrProfesor  
**Usuario:** Profesor Jorge

**Respuesta Esperada (204 No Content)**

---

### 10. Cancelar Clase Pasada (debe fallar)

**Endpoint:** `DELETE /api/clases/{idClase-pasada}`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "No se puede cancelar una clase que ya ha comenzado o pasado."
}
```

---

### 11. Obtener Detalle de Clase (Admin)

**Endpoint:** `GET /api/clases/{idClase}`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin

**Respuesta Esperada (200 OK):**
```json
{
  "idClase": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  "fecha": "2026-01-15T00:00:00",
  "horaInicio": "18:00:00",
  "horaFin": "19:30:00",
  "tipoClase": "Tango",
  "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "nombreProfesor": "Jorge Padilla",
  "totalAsistencias": 12,
  "monitores": [
    {
      "idProfesor": "monitor-id",
      "nombreProfesor": "Monitor Name"
    }
  ]
}
```

---

### 12. Obtener Detalle de Clase (Profesor dueño)

**Endpoint:** `GET /api/clases/{idClase}`  
**Autorización:** AdminOrProfesor  
**Usuario:** Profesor Jorge (debe ser su clase)

**Respuesta Esperada (200 OK):**
```json
{
  "idClase": "b2c3d4e5-f6a7-8901-2345-67890abcdef1",
  "fecha": "2026-01-16T00:00:00",
  "horaInicio": "19:00:00",
  "horaFin": "20:00:00",
  "tipoClase": "Vals",
  "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
  "nombreProfesor": "Jorge Padilla",
  "totalAsistencias": 8,
  "monitores": []
}
```

---

### 13. Obtener Detalle de Clase (Profesor no dueño - debe fallar)

**Endpoint:** `GET /api/clases/{idClase-de-otro-profesor}`  
**Autorización:** AdminOrProfesor  
**Usuario:** Profesor Jorge

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "No tienes permiso para ver esta clase."
}
```

---

### 14. Listar Clases de Profesor (Admin)

**Endpoint:** `GET /api/profesores/{idProfesor}/clases?pageNumber=1&pageSize=10`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin

**Respuesta Esperada (200 OK):**
```json
{
  "items": [
    {
      "idClase": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
      "fecha": "2026-01-16T00:00:00",
      "horaInicio": "19:00:00",
      "horaFin": "20:00:00",
      "tipoClase": "Vals",
      "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
      "nombreProfesor": "Jorge Padilla",
      "totalAsistencias": 8
    },
    {
      "idClase": "b2c3d4e5-f6a7-8901-2345-67890abcdef1",
      "fecha": "2026-01-15T00:00:00",
      "horaInicio": "18:00:00",
      "horaFin": "19:30:00",
      "tipoClase": "Tango",
      "idProfesorPrincipal": "8f6e460d-328d-4a40-89e3-b8effa76829c",
      "nombreProfesor": "Jorge Padilla",
      "totalAsistencias": 12
    }
  ],
  "pageNumber": 1,
  "totalPages": 1,
  "totalCount": 2,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

---

### 15. Listar Clases de Profesor con Filtros de Fecha

**Endpoint:** `GET /api/profesores/{idProfesor}/clases?fechaDesde=2026-01-15&fechaHasta=2026-01-20&pageNumber=1&pageSize=10`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin

**Respuesta Esperada (200 OK):**
Lista filtrada de clases entre las fechas especificadas.

---

### 16. Listar Clases de Profesor (Profesor para sí mismo)

**Endpoint:** `GET /api/profesores/8f6e460d-328d-4a40-89e3-b8effa76829c/clases?pageNumber=1&pageSize=10`  
**Autorización:** AdminOrProfesor  
**Usuario:** Profesor Jorge

**Respuesta Esperada (200 OK):**
Lista de sus propias clases.

---

### 17. Listar Clases de Profesor (Profesor de otro - debe fallar)

**Endpoint:** `GET /api/profesores/{otro-profesor-id}/clases?pageNumber=1&pageSize=10`  
**Autorización:** AdminOrProfesor  
**Usuario:** Profesor Jorge

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "No tienes permiso para ver las clases de otro profesor."
}
```

---

### 18. Obtener Clases de Alumno

**Endpoint:** `GET /api/alumnos/{idAlumno}/clases?desde=2026-01-01&hasta=2026-01-31`  
**Autorización:** ApiScope + Ownership  
**Usuario:** Alumno Juan David (para sí mismo)

**Respuesta Esperada (200 OK):**
Lista de clases donde el alumno tiene asistencia registrada.

---

## 🧪 Matriz de Pruebas

| # | Escenario | Usuario | Endpoint | Resultado Esperado |
|---|-----------|---------|----------|-------------------|
| 1 | Crear clase como admin para profesor | Admin | POST /api/clases | 201 Created |
| 2 | Crear clase como profesor para sí mismo | Profesor Jorge | POST /api/clases | 201 Created |
| 3 | Crear clase como profesor para otro profesor | Profesor Jorge | POST /api/clases | 400 Bad Request |
| 4 | Crear clase con conflicto de horario | Admin | POST /api/clases | 400 Bad Request |
| 5 | Crear clase con fecha pasada | Admin | POST /api/clases | 400 Bad Request |
| 6 | Editar clase como admin | Admin | PUT /api/clases/{id} | 204 No Content |
| 7 | Editar clase como profesor dueño | Profesor Jorge | PUT /api/clases/{id} | 204 No Content |
| 8 | Editar clase como profesor no dueño | Profesor Jorge | PUT /api/clases/{id} | 400 Bad Request |
| 9 | Cancelar clase como admin | Admin | DELETE /api/clases/{id} | 204 No Content |
| 10 | Cancelar clase como profesor dueño | Profesor Jorge | DELETE /api/clases/{id} | 204 No Content |
| 11 | Cancelar clase pasada | Admin | DELETE /api/clases/{id} | 400 Bad Request |
| 12 | Cancelar clase con asistencias | Admin | DELETE /api/clases/{id} | 400 Bad Request |
| 13 | Obtener detalle de clase como admin | Admin | GET /api/clases/{id} | 200 OK |
| 14 | Obtener detalle de clase como profesor dueño | Profesor Jorge | GET /api/clases/{id} | 200 OK |
| 15 | Obtener detalle de clase como profesor no dueño | Profesor Jorge | GET /api/clases/{id} | 400 Bad Request |
| 16 | Listar clases de profesor como admin | Admin | GET /api/profesores/{id}/clases | 200 OK |
| 17 | Listar clases propias como profesor | Profesor Jorge | GET /api/profesores/{id}/clases | 200 OK |
| 18 | Listar clases de otro profesor | Profesor Jorge | GET /api/profesores/{id}/clases | 400 Bad Request |
| 19 | Listar clases con paginación | Admin | GET /api/profesores/{id}/clases?pageSize=5 | 200 OK |
| 20 | Listar clases con filtros de fecha | Admin | GET /api/profesores/{id}/clases?fechaDesde=... | 200 OK |

---

## 📝 Notas Importantes

1. **Tipos de Clase:** Antes de crear clases, asegúrate de tener IDs de tipos de clase en la BD (Tango, Vals, Milonga, Técnica, Práctica).

2. **Obtener IdProfesor:** 
   ```http
   GET /api/auth/me
   Authorization: Bearer {{token-profesor}}
   ```
   Para obtener los IDs necesarios para las pruebas.

3. **Validación de Conflictos:** La API valida automáticamente que no haya solapamiento de horarios para el mismo profesor en la misma fecha.

4. **Ownership:** Los profesores solo pueden:
   - Crear clases para sí mismos
   - Editar sus propias clases
   - Cancelar sus propias clases
   - Ver sus propias clases

5. **Paginación:** Por defecto pageSize=10, máximo recomendado 50.

---

## 🔧 Variables de Entorno Sugeridas (Postman)

```
BASE_URL = https://localhost:7194
TOKEN_ADMIN = {{obtener del OAuth flow}}
TOKEN_PROFESOR_JORGE = {{obtener del OAuth flow}}
TOKEN_ALUMNO_JUAN = {{obtener del OAuth flow}}
ID_PROFESOR_JORGE = 8f6e460d-328d-4a40-89e3-b8effa76829c
ID_ALUMNO_JUAN = 295093d5-b36f-4737-b68a-ab40ca871b2e
```

---

**Fecha de Creación:** 9 de enero de 2026  
**Módulo:** Clases  
**Estado:** ✅ Completo

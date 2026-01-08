# Matriz de pruebas AuthN/AuthZ (Entra ID + Postman) – Chetango Backend

Fecha: 2025-12-30

Objetivo: ejecutar y documentar evidencia de autenticación/autorización usando tokens de Microsoft Entra ID (App Roles) contra la API.

## 0) Precondiciones

- API corriendo (QA local):
  - `http://localhost:5129`
  - `https://localhost:7194`
- Postman configurado con OAuth 2.0 (Authorization Code + PKCE) para obtener **tokens de 3 usuarios**:
  - `token_admin` (role: `admin`)
  - `token_profesor` (role: `profesor` o equivalente asignado en Entra)
  - `token_alumno` (sin rol admin/profesor)

> Nota: Los roles deben venir solo desde Entra (claim `roles`).

## 1) Variables sugeridas en Postman

En el Environment / Collection:

- `baseUrl` = `http://localhost:5129`
- `token_admin` = (Bearer token admin)
- `token_profesor` = (Bearer token profesor)
- `token_alumno` = (Bearer token alumno)

Para facilitar, en cada request define el header:

- `Authorization: Bearer {{token_X}}`

## 2) Endpoints “Sanity” de autenticación

### 2.1 GET `/auth/ping`

Propósito: confirmar que el token es válido y que el pipeline auth corre.

Casos:
- Sin token → **401**
- Con token_admin/token_profesor/token_alumno → **200**

Evidencia:
- Captura del status + body.

### 2.2 GET `/api/auth/me`

Propósito: confirmar extracción de email y roles desde el token.

Casos:
- Sin token → **401**
- Con token_admin → **200** y `roles` contiene `admin`
- Con token_profesor → **200** y `roles` contiene `profesor` (o el rol asignado)
- Con token_alumno → **200** y `roles` vacío o sin `admin/profesor`

Evidencia:
- Captura del body (mostrar `correo` y `roles`).

## 3) Endpoints Admin Asistencias (solo admin)

Estos endpoints viven bajo `/api/admin/asistencias/*` y deben requerir rol admin (según configuración `Auth:RequiredRoles`).

### 3.1 GET `/api/admin/asistencias/dias-con-clases`

Casos:
- Sin token → **401**
- token_alumno → **403**
- token_profesor → **403**
- token_admin → **200**

Validación de contenido (200):
- JSON contiene: `hoy`, `desde`, `hasta`, `diasConClases` (array)

### 3.2 GET `/api/admin/asistencias/clases-del-dia?fecha=YYYY-MM-DD`

Casos:
- Sin token → **401**
- token_alumno → **403**
- token_profesor → **403**
- token_admin → **200**

Checks (200):
- `fecha` coincide con query param.
- `clases` es array.

### 3.3 GET `/api/admin/asistencias/clase/{idClase}/resumen`

Casos:
- Sin token → **401**
- token_alumno → **403**
- token_profesor → **403**
- token_admin → **200** (o **404** si idClase no existe)

Checks (200):
- JSON contiene `idClase`, `alumnos`, contadores.

## 4) Endpoints Asistencias (admin o profesor para escritura)

### 4.1 POST `/api/asistencias`

Casos esperados:
- Sin token → **401**
- token_alumno → **403**
- token_profesor → **201** o **400** (según validez del payload)
- token_admin → **201** o **400**

Evidencia:
- Para 201: guardar el `Id`/Location retornado.

### 4.2 PUT `/api/asistencias/{id}/estado`

Casos esperados:
- Sin token → **401**
- token_alumno → **403**
- token_profesor → **204** o **400/404**
- token_admin → **204** o **400/404**

## 5) Lecturas por recurso (owner/admin y profesor por clase)

> Regla negocio: admin puede ver todo; profesor solo sus clases y asistencias de sus clases; alumno (owner) solo sus datos.

### 5.1 GET `/api/alumnos/{idAlumno}/clases`

Casos:
- Sin token → **401**
- token_admin → **200** o **404**
- token_alumno (owner del alumno) → **200**
- token_alumno (NO owner) → **403**
- token_profesor → (según implementación actual) normalmente **403** (no es owner/admin)

### 5.2 GET `/api/alumnos/{idAlumno}/asistencias`

Casos:
- Sin token → **401**
- token_admin → **200** o **404**
- token_alumno (owner) → **200**
- token_alumno (NO owner) → **403**
- token_profesor → **403** (si no aplica owner/admin)

### 5.3 GET `/api/clases/{idClase}/asistencias`

Casos:
- Sin token → **401**
- token_admin → **200** o **404**
- token_profesor (dueño de la clase) → **200**
- token_profesor (NO dueño) → **403**
- token_alumno → **403**

## 6) Datos de prueba (IDs)

Recomendación:
1) Usa primero endpoints dev (sin auth) para encontrar IDs existentes:
   - `GET /api/dev/alumnos/seeded/clases`
   - `GET /api/dev/seed/status`
2) Con esos IDs, ejecuta los casos protegidos.

## 7) Evidencias mínimas a capturar

- Screenshot de `/api/auth/me` para cada token (admin/profesor/alumno).
- Para cada endpoint: al menos 1 evidencia de 401 (sin token), 1 de 403 (rol indebido), y 1 de 200/201/204 (rol correcto).

---

Si quieres, también puedo:
- Convertir esto en una colección Postman con tests automáticos (Scripts) y variables, o
- Agregar assertions específicas por endpoint (estructura JSON, tipos, etc.).

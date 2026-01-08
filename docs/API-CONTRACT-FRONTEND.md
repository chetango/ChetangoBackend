# Chetango API - Contrato para Frontend

**Fecha:** 2026-01-08  
**Versión API:** 1.0  
**Autenticación:** Microsoft Entra External ID (CIAM) + OAuth 2.0

---

## 📋 Índice

1. [Configuración de Autenticación](#configuración-de-autenticación)
2. [Usuarios de Prueba](#usuarios-de-prueba)
3. [Endpoints Disponibles](#endpoints-disponibles)
   - [Setup Inicial del Backend](#🔧-configuración-inicial-para-frontend)
4. [Matriz de Permisos](#matriz-de-permisos)
5. [Ejemplos de Requests/Responses](#ejemplos-de-requestsresponses)
6. [Códigos de Error](#códigos-de-error)

---

## 🔐 Configuración de Autenticación

> **📋 NOTA IMPORTANTE**: Esta sección es **INFORMATIVA**. La configuración en Entra ID ya está completa y funcionando. El frontend **NO necesita configurar nada nuevo en Azure/Entra ID**, solo debe usar estos valores en su implementación del flujo OAuth 2.0 en su código (React, Angular, Vue, etc.).

### Tipo de Autenticación
**OAuth 2.0 Authorization Code Flow + PKCE**

### Endpoints CIAM
```
Auth URL: https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251/oauth2/v2.0/authorize
Token URL: https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251/oauth2/v2.0/token
```

### Configuración del Cliente
```
Client ID: 3cfbc7f6-0df6-41dd-9216-213a3fbc618a
Redirect URI: (configurar según tu aplicación)
Scope: api://d35c1d4d-9ddc-4a8b-bb89-1964b37ff573/access_as_user openid profile offline_access
Response Type: code
Code Challenge Method: S256
```

### Headers Requeridos
```http
Authorization: Bearer {access_token}
Content-Type: application/json
```

### Formato de Token
El access token es un JWT que contiene:
```json
{
  "aud": "d35c1d4d-9ddc-4a8b-bb89-1964b37ff573",
  "iss": "https://8a57ec5a-e2e3-44ad-9494-77fbc7467251.ciamlogin.com/...",
  "preferred_username": "usuario@chetangoprueba.onmicrosoft.com",
  "roles": ["admin" | "profesor" | "alumno"],
  "exp": 1234567890
}
```

---

## 👥 Usuarios de Prueba

> **⚠️ IMPORTANTE - Paso Requerido**: Antes de usar estos usuarios, debes ejecutar el script SQL que crea sus datos en la base de datos:
> 
> ```bash
> # Desde la raíz del proyecto ChetangoBackend
> sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i scripts\seed_usuarios_prueba_ciam.sql
> ```
> 
> **Alternativa (PowerShell):**
> ```powershell
> Invoke-Sqlcmd -ServerInstance "(localdb)\MSSQLLocalDB" -Database "ChetangoDB_Dev" -InputFile "scripts\seed_usuarios_prueba_ciam.sql"
> ```
> 
> Este script es **idempotente** (puedes ejecutarlo múltiples veces sin problemas) y crea:
> - Los 3 usuarios vinculados a Entra ID
> - Relaciones Alumno/Profesor
> - 1 clase de Jorge para pruebas
> - 1 paquete activo para Juan David
> - 1 asistencia de ejemplo

### Usuario Admin
```
Email: Chetango@chetangoprueba.onmicrosoft.com
Contraseña: Chet4ngo20#
Rol: admin
Permisos: Acceso completo a todos los endpoints
```

### Usuario Profesor
```
Email: Jorgepadilla@chetangoprueba.onmicrosoft.com
Contraseña: Jorge2026
Rol: profesor
Permisos: Ver y editar sus propias clases y asistencias
```

### Usuario Alumno
```
Email: JuanDavid@chetangoprueba.onmicrosoft.com
Contraseña: Juaj0rge20#
Rol: alumno
Permisos: Ver solo sus propias clases y asistencias
```

---

## 🌐 Endpoints Disponibles

### Base URL
```
Development/QA: https://localhost:7194
Production: (configurar según deploy)
```

### 🔧 Configuración Inicial para Frontend

#### Paso 1: Levantar el Backend
Después de clonar el repositorio:
```bash
cd ChetangoBackend
dotnet run --project Chetango.Api/Chetango.Api.csproj --launch-profile https-qa
```

La API estará disponible en:
- `https://localhost:7194` (HTTPS - recomendado)
- `http://localhost:5129` (HTTP)

Verificar que está corriendo:
- Ir a `https://localhost:7194/swagger`
- O hacer `GET https://localhost:7194/api/auth/me` (debe responder `401 Unauthorized` sin token)

#### Paso 2: Crear Usuarios de Prueba en BD
**Este paso es OBLIGATORIO** para poder usar los usuarios documentados. Ejecutar el script SQL:

```bash
# Opción 1: sqlcmd (si está instalado)
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i scripts\seed_usuarios_prueba_ciam.sql

# Opción 2: PowerShell
Invoke-Sqlcmd -ServerInstance "(localdb)\MSSQLLocalDB" -Database "ChetangoDB_Dev" -InputFile "scripts\seed_usuarios_prueba_ciam.sql"

# Opción 3: SQL Server Management Studio o Azure Data Studio
# Abrir el archivo scripts/seed_usuarios_prueba_ciam.sql y ejecutarlo
```

El script crea:
- 3 usuarios (admin, profesor, alumno) vinculados a Entra ID
- 1 clase de prueba del profesor Jorge
- 1 paquete activo para el alumno Juan David
- 1 asistencia de ejemplo

**Nota**: El script es idempotente, puedes ejecutarlo múltiples veces sin errores.

#### Paso 3: CORS - Importante para Frontend
El backend tiene CORS configurado. Si tu frontend corre en un puerto diferente a los configurados, **debes solicitar que se agregue tu origen**.

**Orígenes configurados actualmente:**
```
Development: http://localhost:5129, https://localhost:7194
QA: https://qa.chetango.local
```

**Si tu frontend corre en otro puerto** (ej: `http://localhost:3000` para React):
1. Notifica al equipo de backend
2. Se agregará tu origen en `appsettings.Development.json` → `Cors:AllowedOrigins`

**Síntoma de CORS mal configurado:**
```
Error en navegador: "Access to fetch at '...' from origin 'http://localhost:3000' 
has been blocked by CORS policy"
```

#### Base de Datos
- Las migraciones se ejecutan **automáticamente** al iniciar la API
- BD: `ChetangoDB_Dev` en SQL Server local
- Usuarios de prueba ya están creados con los correos documentados abajo

### 1. Autenticación y Perfil

#### `GET /api/auth/me`
Obtiene el perfil del usuario autenticado.

**Autenticación:** Requerida  
**Roles permitidos:** Todos  

**Response 200 OK:**
```json
{
  "idUsuario": "uuid",
  "nombre": "string",
  "correo": "string",
  "telefono": "string",
  "roles": ["admin" | "profesor" | "alumno"]
}
```

**Errores posibles:**
- `401 Unauthorized`: Token inválido o expirado
- `404 Not Found`: Usuario no existe en base de datos

---

### 2. Admin - Gestión de Asistencias

Todos los endpoints bajo `/api/admin/asistencias/*` requieren rol **admin**.

#### `GET /api/admin/asistencias/dias-con-clases`
Obtiene el calendario de los últimos 7 días con marcas de días que tienen clases.

**Autenticación:** Requerida  
**Roles permitidos:** admin  

**Response 200 OK:**
```json
{
  "hoy": "2026-01-08",
  "desde": "2026-01-01",
  "hasta": "2026-01-08",
  "diasConClases": ["2026-01-05", "2026-01-08"]
}
```

**Errores posibles:**
- `401 Unauthorized`: Sin token
- `403 Forbidden`: Usuario sin rol admin

---

#### `GET /api/admin/asistencias/clases-del-dia?fecha={YYYY-MM-DD}`
Lista las clases disponibles para una fecha específica.

**Autenticación:** Requerida  
**Roles permitidos:** admin  

**Query Parameters:**
- `fecha` (requerido): Fecha en formato YYYY-MM-DD

**Response 200 OK:**
```json
{
  "fecha": "2025-12-22",
  "clases": [
    {
      "idClase": "uuid",
      "nombre": "Tango",
      "horaInicio": "17:00:00",
      "horaFin": "18:30:00",
      "profesorPrincipal": "Jorge Padilla"
    }
  ]
}
```

**Errores posibles:**
- `400 Bad Request`: Formato de fecha inválido
- `401 Unauthorized`: Sin token
- `403 Forbidden`: Usuario sin rol admin

---

#### `GET /api/admin/asistencias/clase/{idClase}/resumen`
Obtiene el resumen completo de una clase con alumnos, paquetes y asistencias.

**Autenticación:** Requerida  
**Roles permitidos:** admin  

**Path Parameters:**
- `idClase` (uuid): ID de la clase

**Response 200 OK:**
```json
{
  "idClase": "uuid",
  "fecha": "2025-12-22",
  "nombreClase": "Tango",
  "profesorPrincipal": "Jorge Padilla",
  "alumnos": [
    {
      "idAlumno": "uuid",
      "nombreCompleto": "Juan David",
      "documentoIdentidad": "12345678",
      "avatarIniciales": "JD",
      "paquete": {
        "estado": "Activo",
        "descripcion": "Paquete 8 Clases",
        "clasesTotales": 8,
        "clasesUsadas": 3,
        "clasesRestantes": 5
      },
      "asistencia": {
        "estado": "Presente",
        "observacion": "Llegó tarde"
      }
    }
  ],
  "presentes": 5,
  "ausentes": 3,
  "sinPaquete": 1
}
```

**Estados de Paquete:** `Activo`, `Agotado`, `Congelado`, `SinPaquete`  
**Estados de Asistencia:** `Presente`, `Ausente`

**Errores posibles:**
- `401 Unauthorized`: Sin token
- `403 Forbidden`: Usuario sin rol admin
- `404 Not Found`: Clase no encontrada

---

### 3. Asistencias - Registro y Edición

#### `POST /api/asistencias`
Registra una nueva asistencia.

**Autenticación:** Requerida  
**Roles permitidos:** admin, profesor  

**Request Body:**
```json
{
  "idClase": "uuid",
  "idAlumno": "uuid",
  "presente": true,
  "observacion": "string (opcional)"
}
```

**Response 201 Created:**
```json
"uuid-asistencia-id"
```

**Errores posibles:**
- `400 Bad Request`: Datos inválidos (alumno no existe, clase no existe)
- `401 Unauthorized`: Sin token
- `403 Forbidden`: Usuario sin permisos

---

#### `PUT /api/asistencias/{id}/estado`
Actualiza el estado de una asistencia existente.

**Autenticación:** Requerida  
**Roles permitidos:** admin, profesor  

**Path Parameters:**
- `id` (uuid): ID de la asistencia

**Request Body:**
```json
{
  "idAsistencia": "uuid",
  "presente": false,
  "observacion": "string (opcional)"
}
```

**Response:** `204 No Content`

**Errores posibles:**
- `400 Bad Request`: ID en ruta no coincide con body
- `401 Unauthorized`: Sin token
- `403 Forbidden`: Usuario sin permisos
- `404 Not Found`: Asistencia no encontrada

---

### 4. Clases - Consulta por Clase

#### `GET /api/clases/{idClase}/asistencias`
Obtiene las asistencias de una clase específica.

**Autenticación:** Requerida  
**Roles permitidos:**
- admin: Acceso a cualquier clase
- profesor: Solo sus propias clases

**Path Parameters:**
- `idClase` (uuid): ID de la clase

**Response 200 OK:**
```json
[
  {
    "idAsistencia": "uuid",
    "idAlumno": "uuid",
    "nombreAlumno": "string",
    "presente": true,
    "observacion": "string",
    "fechaRegistro": "datetime"
  }
]
```

**Errores posibles:**
- `401 Unauthorized`: Sin token
- `403 Forbidden`: Profesor intenta ver clase de otro profesor
- `404 Not Found`: Clase no encontrada

---

### 5. Alumnos - Consultas por Alumno

#### `GET /api/alumnos/{idAlumno}/clases`
Obtiene las clases de un alumno.

**Autenticación:** Requerida  
**Roles permitidos:**
- admin: Acceso a cualquier alumno
- alumno: Solo sus propias clases (ownership por correo)

**Path Parameters:**
- `idAlumno` (uuid): ID del alumno

**Query Parameters (opcionales):**
- `desde` (datetime): Fecha inicio
- `hasta` (datetime): Fecha fin

**Response 200 OK:**
```json
[
  {
    "idClase": "uuid",
    "fecha": "date",
    "horaInicio": "time",
    "horaFin": "time",
    "nombreClase": "string",
    "profesorPrincipal": "string"
  }
]
```

**Errores posibles:**
- `401 Unauthorized`: Sin token
- `403 Forbidden`: Alumno intenta ver clases de otro alumno
- `404 Not Found`: Alumno no encontrado

---

#### `GET /api/alumnos/{idAlumno}/asistencias`
Obtiene las asistencias de un alumno.

**Autenticación:** Requerida  
**Roles permitidos:**
- admin: Acceso a cualquier alumno
- alumno: Solo sus propias asistencias (ownership por correo)

**Path Parameters:**
- `idAlumno` (uuid): ID del alumno

**Query Parameters (opcionales):**
- `fechaDesde` (datetime): Fecha inicio
- `fechaHasta` (datetime): Fecha fin

**Response 200 OK:**
```json
[
  {
    "idAsistencia": "uuid",
    "idClase": "uuid",
    "nombreClase": "string",
    "fecha": "date",
    "presente": true,
    "observacion": "string",
    "fechaRegistro": "datetime"
  }
]
```

**Errores posibles:**
- `401 Unauthorized`: Sin token
- `403 Forbidden`: Alumno intenta ver asistencias de otro
- `404 Not Found`: Alumno no encontrado

---

## 🔒 Matriz de Permisos

| Endpoint | Admin | Profesor | Alumno | Sin Auth |
|----------|-------|----------|--------|----------|
| `GET /api/auth/me` | ✅ 200 | ✅ 200 | ✅ 200 | ❌ 401 |
| `GET /api/admin/asistencias/dias-con-clases` | ✅ 200 | ❌ 403 | ❌ 403 | ❌ 401 |
| `GET /api/admin/asistencias/clases-del-dia` | ✅ 200 | ❌ 403 | ❌ 403 | ❌ 401 |
| `GET /api/admin/asistencias/clase/{id}/resumen` | ✅ 200 | ❌ 403 | ❌ 403 | ❌ 401 |
| `POST /api/asistencias` | ✅ 201 | ✅ 201 | ❌ 403 | ❌ 401 |
| `PUT /api/asistencias/{id}/estado` | ✅ 204 | ✅ 204 | ❌ 403 | ❌ 401 |
| `GET /api/clases/{id}/asistencias` | ✅ 200 | ✅ 200* | ❌ 403 | ❌ 401 |
| `GET /api/alumnos/{id}/clases` | ✅ 200 | ❌ 403 | ✅ 200* | ❌ 401 |
| `GET /api/alumnos/{id}/asistencias` | ✅ 200 | ❌ 403 | ✅ 200* | ❌ 401 |

**\* Notas:**
- **Profesor**: Solo puede ver asistencias de **sus propias clases**. Si intenta acceder a una clase de otro profesor → `403`
- **Alumno**: Solo puede ver **sus propios datos**. Si intenta acceder a datos de otro alumno → `403` o `404`

---

## 📝 Ejemplos de Requests/Responses

### Ejemplo 1: Login y obtención de perfil

**Step 1: Obtener Access Token (OAuth)**
```http
POST https://chetangoprueba.ciamlogin.com/8a57ec5a-e2e3-44ad-9494-77fbc7467251/oauth2/v2.0/token
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code
&client_id=3cfbc7f6-0df6-41dd-9216-213a3fbc618a
&code={authorization_code}
&redirect_uri={your_redirect_uri}
&code_verifier={pkce_verifier}
```

**Response:**
```json
{
  "access_token": "eyJ0eXAiOiJKV1QiLCJhbGc...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "..."
}
```

**Step 2: Obtener perfil del usuario**
```http
GET https://localhost:7194/api/auth/me
Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGc...
```

**Response 200 OK:**
```json
{
  "idUsuario": "b91e51b9-4094-441e-a5b6-062a846b3868",
  "nombre": "Administrador Chetango",
  "correo": "Chetango@chetangoprueba.onmicrosoft.com",
  "telefono": "",
  "roles": ["admin"]
}
```

---

### Ejemplo 2: Consultar clases del día (Admin)

**Request:**
```http
GET https://localhost:7194/api/admin/asistencias/clases-del-dia?fecha=2025-12-22
Authorization: Bearer {admin_token}
```

**Response 200 OK:**
```json
{
  "fecha": "2025-12-22",
  "clases": [
    {
      "idClase": "6a50c2cb-461e-4ee1-a50f-03f938bc5b4c",
      "nombre": "Tango",
      "horaInicio": "17:00:00",
      "horaFin": "18:30:00",
      "profesorPrincipal": "Jorge Padilla"
    }
  ]
}
```

---

### Ejemplo 3: Registrar asistencia (Profesor)

**Request:**
```http
POST https://localhost:7194/api/asistencias
Authorization: Bearer {profesor_token}
Content-Type: application/json

{
  "idClase": "6a50c2cb-461e-4ee1-a50f-03f938bc5b4c",
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "presente": true,
  "observacion": "Llegó a tiempo"
}
```

**Response 201 Created:**
```json
"f123e456-7890-1234-5678-90abcdef1234"
```

---

### Ejemplo 4: Consultar mis clases (Alumno)

**Request:**
```http
GET https://localhost:7194/api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/clases
Authorization: Bearer {alumno_token_juandavid}
```

**Response 200 OK:**
```json
[]
```
*(vacío si no hay clases asignadas)*

---

## ⚠️ Códigos de Error

### Códigos HTTP estándar

| Código | Descripción | Causa típica |
|--------|-------------|--------------|
| `200 OK` | Solicitud exitosa | - |
| `201 Created` | Recurso creado exitosamente | POST exitoso |
| `204 No Content` | Solicitud exitosa sin contenido | PUT/DELETE exitoso |
| `400 Bad Request` | Datos inválidos en el request | Validación falló, datos faltantes |
| `401 Unauthorized` | Token inválido, expirado o ausente | No autenticado |
| `403 Forbidden` | Usuario sin permisos | Autenticado pero sin rol/ownership |
| `404 Not Found` | Recurso no encontrado | ID no existe en BD |
| `500 Internal Server Error` | Error del servidor | Error no controlado |

### Formato de error estándar

```json
{
  "message": "Descripción del error",
  "error": "Código de error (opcional)"
}
```

**Ejemplos:**

**401 Unauthorized:**
```json
{
  "message": "Unauthorized"
}
```

**403 Forbidden:**
```json
{
  "message": "Forbidden"
}
```

**404 Not Found:**
```json
{
  "message": "Usuario no existe en BD. Debe ser creado por seed/onboarding."
}
```

**400 Bad Request:**
```json
{
  "message": "El alumno especificado no existe."
}
```

---

## 🔄 Flujo de Trabajo Recomendado

### Para el Frontend

1. **Login:**
   - Redirigir al usuario a la URL de autorización CIAM
   - Capturar el `authorization_code` en el callback
   - Intercambiar el código por `access_token`
   - Guardar el token (localStorage/sessionStorage/secure cookie)

2. **Verificar perfil:**
   - Llamar a `GET /api/auth/me` con el token
   - Obtener roles del usuario
   - Habilitar/deshabilitar UI según roles

3. **Hacer requests protegidos:**
   - Incluir siempre `Authorization: Bearer {token}` en headers
   - Manejar errores 401 (re-login) y 403 (mostrar mensaje)

4. **Refresh token:**
   - Implementar lógica para renovar token antes de expiración
   - O manejar 401 y forzar re-login

---

## 📚 Recursos Adicionales

### Documentación relacionada
- [Admin Asistencias Endpoints](./admin-asistencias-endpoints.md)
- [Matriz de Pruebas AuthZ](./authz-postman-test-matrix.md)

### IDs de prueba útiles
```
Clase de Jorge: 6a50c2cb-461e-4ee1-a50f-03f938bc5b4c
Alumno Juan David: 295093d5-b36f-4737-b68a-ab40ca871b2e
Profesor Jorge: 8f6e460d-328d-4a40-89e3-b8effa76829c
Usuario Admin: b91e51b9-4094-441e-a5b6-062a846b3868
```

### Formatos de fecha/hora
- **Fechas:** `YYYY-MM-DD` (ISO 8601 date)
- **Horas:** `HH:mm:ss` (24 horas)
- **DateTime:** `YYYY-MM-DDTHH:mm:ss` (ISO 8601)

---

## 🛠️ Ambiente de Desarrollo

### Configuración Local
```
API URL: https://localhost:7194
Swagger UI: https://localhost:7194/swagger
ReDoc: https://localhost:7194/redoc
```

---

## ✅ Checklist de Integración

- [ ] Configurar flujo OAuth 2.0 con PKCE
- [ ] Implementar manejo de tokens (storage + refresh)
- [ ] Probar login con los 3 usuarios de prueba
- [ ] Verificar obtención de perfil y roles
- [ ] Implementar interceptor/middleware para agregar token a requests
- [ ] Manejar errores 401 (redirect a login)
- [ ] Manejar errores 403 (mostrar mensaje al usuario)
- [ ] Habilitar/deshabilitar UI según roles
- [ ] Probar todos los endpoints según matriz de permisos
- [ ] Implementar manejo de estados de carga y errores

---

**Fecha de actualización:** 2026-01-08  
**Mantenido por:** Backend Team  
**Contacto:** Para dudas o cambios en el contrato, contactar al equipo de backend.

# 🧪 Casos de Prueba - Módulo Paquetes

## 📋 Información General

**Base URL:** `https://localhost:7194`  
**Autenticación:** Bearer Token (OAuth 2.0 - Microsoft Entra External ID)

### Usuarios de Prueba

| Usuario | Email | Rol | IdUsuario | IdAlumno/IdProfesor |
|---------|-------|-----|-----------|---------------------|
| Admin | Chetango@chetangoprueba.onmicrosoft.com | admin | b91e51b9-4094-441e-a5b6-062a846b3868 | - |
| Profesor Jorge | Jorgepadilla@chetangoprueba.onmicrosoft.com | profesor | 8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB | IdProfesor: 8f6e460d-328d-4a40-89e3-b8effa76829c |
| Alumno Juan David | JuanDavid@chetangoprueba.onmicrosoft.com | alumno | 71462106-9863-4fd0-b13d-9878ed231aa6 | IdAlumno: 295093d5-b36f-4737-b68a-ab40ca871b2e |

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

---

## 📚 Estados de Paquete

| IdEstado | Nombre | Descripción |
|----------|--------|-------------|
| 1 | Activo | Tiene clases disponibles y no está vencido |
| 2 | Vencido | FechaVencimiento < hoy |
| 3 | Congelado | Pausado temporalmente |
| 4 | Agotado | ClasesUsadas >= ClasesDisponibles |

---

## 📦 FASE 1: Crear y Consultar Paquetes

### 1. POST /api/paquetes - Crear un Paquete

**Endpoint:** `POST /api/paquetes`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "idTipoPaquete": "GUID-DEL-TIPO-PAQUETE",
  "clasesDisponibles": 8,
  "valorPaquete": 150000,
  "diasVigencia": 30,
  "idPago": null
}
```

**Respuesta Esperada (201 Created):**
```json
{
  "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678"
}
```

**Validaciones:**
- ✅ Alumno debe existir y estar activo
- ✅ Tipo de paquete debe existir
- ✅ ClasesDisponibles > 0
- ✅ ValorPaquete >= 0
- ✅ DiasVigencia > 0
- ✅ Estado inicial = Activo (1)
- ✅ ClasesUsadas inicial = 0
- ✅ FechaVencimiento = Hoy + DiasVigencia

**Casos de Error:**
```json
// IdAlumno inválido
{
  "error": "El alumno especificado no existe."
}

// ClasesDisponibles = 0
{
  "error": "Las clases disponibles deben ser mayor a 0."
}
```

---

### 2. GET /api/paquetes/{id} - Obtener Detalle de Paquete

**Endpoint:** `GET /api/paquetes/{idPaquete}`  
**Autorización:** ApiScope (con ownership validation)  
**Usuario:** Admin (ve cualquier paquete) o Alumno dueño

**Respuesta Esperada (200 OK):**
```json
{
  "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678",
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "nombreAlumno": "Juan David Perez",
  "idTipoPaquete": "GUID-DEL-TIPO",
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
      "idCongelacion": "GUID",
      "fechaInicio": "2026-01-15T00:00:00",
      "fechaFin": "2026-01-22T00:00:00",
      "diasCongelados": 7
    }
  ]
}
```

**Ownership Validation:**
- ✅ Admin: puede ver cualquier paquete
- ✅ Alumno: solo puede ver SUS propios paquetes
- ❌ Otro alumno intentando ver paquete: `403 Forbidden`

---

### 3. GET /api/alumnos/{idAlumno}/paquetes - Listar Paquetes de Alumno

**Endpoint:** `GET /api/alumnos/{idAlumno}/paquetes`  
**Autorización:** ApiScope (con ownership validation)  
**Usuario:** Admin o Alumno dueño

**Query Parameters:**
- `soloActivos` (bool, default: true) - Filtrar solo paquetes activos
- `estado` (int, opcional) - Filtrar por estado específico (1-4)
- `fechaVencimientoDesde` (datetime, opcional)
- `fechaVencimientoHasta` (datetime, opcional)
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)

**Ejemplos:**
```
GET /api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/paquetes?soloActivos=true
GET /api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/paquetes?estado=1&pageSize=5
GET /api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/paquetes?fechaVencimientoDesde=2026-01-01
```

**Respuesta Esperada (200 OK):**
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

---

## 🎯 FASE 2: Descuento de Clases (Integración con Asistencias)

### 4. POST /api/asistencias - Registrar Asistencia con Descuento de Paquete

**Endpoint:** `POST /api/asistencias`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin o Profesor Jorge

**Request Body:**
```json
{
  "idClase": "GUID-CLASE-EXISTENTE",
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "idPaqueteUsado": "aabbccdd-1234-5678-90ab-cdef12345678",
  "idEstadoAsistencia": 1,
  "observaciones": "Presente - clase regular"
}
```

**Estados de Asistencia:**
- 1 = Presente (descuenta clase del paquete)
- 2 = Ausente (NO descuenta)
- 3 = Justificada (NO descuenta)

**Respuesta Esperada (201 Created):**
```json
"GUID-ASISTENCIA-CREADA"
```

**Validaciones Automáticas:**
1. ✅ Clase existe y no es futura
2. ✅ Alumno existe
3. ✅ Paquete pertenece al alumno
4. ✅ **NUEVA: Validar paquete disponible** (ValidarPaqueteDisponibleQuery)
   - Estado = Activo (no Vencido/Congelado/Agotado)
   - ClasesRestantes > 0
   - FechaVencimiento > hoy
5. ✅ **NUEVA: Descontar clase** (DescontarClaseCommand)
   - ClasesUsadas++
   - Si ClasesUsadas >= ClasesDisponibles → Estado = Agotado (4)

**Casos de Error:**
```json
// Paquete agotado
{
  "error": "El paquete no tiene clases disponibles."
}

// Paquete vencido
{
  "error": "El paquete está vencido."
}

// Paquete congelado
{
  "error": "El paquete no está activo (estado: Congelado)."
}
```

**Verificación Post-Registro:**
Consultar el paquete nuevamente para confirmar descuento:
```
GET /api/paquetes/{idPaquete}
```

Verificar que `clasesUsadas` incrementó en 1.

---

## ❄️ FASE 3: Congelación de Paquetes

### 5. POST /api/paquetes/{id}/congelar - Congelar un Paquete

**Endpoint:** `POST /api/paquetes/{idPaquete}/congelar`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request Body:**
```json
{
  "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678",
  "fechaInicio": "2026-01-15",
  "fechaFin": "2026-01-22",
  "motivo": "Viaje del alumno"
}
```

**Respuesta Esperada (200 OK):**
```json
{
  "mensaje": "Paquete congelado exitosamente"
}
```

**Validaciones:**
- ✅ Paquete debe estar en estado Activo
- ✅ FechaInicio < FechaFin
- ✅ FechaInicio >= hoy
- ✅ No debe haber solapamiento con otras congelaciones
- ✅ Estado cambia a Congelado (3)
- ✅ Se crea registro en CongelacionesPaquete

**Casos de Error:**
```json
// Paquete ya congelado
{
  "error": "Solo se pueden congelar paquetes activos (estado actual: Congelado)."
}

// Fechas solapadas
{
  "error": "Ya existe una congelación en el período especificado."
}
```

---

### 6. POST /api/paquetes/{id}/descongelar - Descongelar un Paquete

**Endpoint:** `POST /api/paquetes/{idPaquete}/descongelar`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Query Parameters:**
- `idCongelacion` (guid, requerido)

**Ejemplo:**
```
POST /api/paquetes/aabbccdd-1234-5678-90ab-cdef12345678/descongelar?idCongelacion=GUID-CONGELACION
```

**Respuesta Esperada (200 OK):**
```json
{
  "mensaje": "Paquete descongelado exitosamente"
}
```

**Lógica Automática:**
1. ✅ FechaFin de congelación = hoy
2. ✅ Calcular días congelados = FechaFin - FechaInicio
3. ✅ FechaVencimiento += días congelados (extiende vencimiento)
4. ✅ Estado vuelve a Activo (1) si hay clases disponibles
5. ✅ Si ClasesUsadas >= ClasesDisponibles → Estado = Agotado (4)
6. ✅ Si FechaVencimiento < hoy → Estado = Vencido (2)

**Casos de Error:**
```json
// Paquete no está congelado
{
  "error": "El paquete no está congelado (estado actual: Activo)."
}

// IdCongelacion inválido
{
  "error": "La congelación especificada no existe para este paquete."
}
```

---

## ✏️ FASE 4: Edición de Paquetes

### 7. PUT /api/paquetes/{id} - Editar un Paquete

**Endpoint:** `PUT /api/paquetes/{idPaquete}`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request Body:**
```json
{
  "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678",
  "clasesDisponibles": 10,
  "fechaVencimiento": "2026-02-20"
}
```

**Respuesta Esperada (204 No Content)**

**Validaciones:**
- ✅ ClasesDisponibles >= ClasesUsadas (no reducir por debajo de usadas)
- ✅ Recalcular estado automáticamente:
  - Si ClasesUsadas >= ClasesDisponibles → Agotado (4)
  - Si FechaVencimiento < hoy → Vencido (2)
  - Si no está congelado y hay clases → Activo (1)

**Casos de Error:**
```json
// ClasesDisponibles < ClasesUsadas
{
  "error": "Las clases disponibles (5) no pueden ser menores a las clases ya usadas (7)."
}
```

**Uso Típico:** Admin ajusta paquete por error en creación o cortesía al alumno.

---

## 🔄 Flujo Completo de Prueba

### Escenario 1: Crear Paquete y Usar Clases Hasta Agotarlo

**1. Crear Paquete (Admin)**
```
POST /api/paquetes
Body: { idAlumno, clasesDisponibles: 3, ... }
→ IdPaquete: aabbccdd-...
```

**2. Verificar Estado Inicial**
```
GET /api/paquetes/aabbccdd-...
→ clasesUsadas: 0, estado: "Activo"
```

**3. Registrar Asistencia #1 (Profesor Jorge)**
```
POST /api/asistencias
Body: { idPaqueteUsado: aabbccdd-..., idEstadoAsistencia: 1 }
→ clasesUsadas: 1
```

**4. Registrar Asistencia #2**
```
POST /api/asistencias
→ clasesUsadas: 2
```

**5. Registrar Asistencia #3 (última clase)**
```
POST /api/asistencias
→ clasesUsadas: 3, estado: "Agotado"
```

**6. Intentar Registrar Asistencia #4 (debe fallar)**
```
POST /api/asistencias
→ Error: "El paquete no tiene clases disponibles."
```

---

### Escenario 2: Congelar y Descongelar Paquete

**1. Crear Paquete con Vencimiento en 30 Días**
```
POST /api/paquetes
Body: { diasVigencia: 30 }
→ fechaVencimiento: 2026-02-10
```

**2. Congelar por 7 Días (Admin)**
```
POST /api/paquetes/{id}/congelar
Body: { fechaInicio: "2026-01-15", fechaFin: "2026-01-22" }
→ estado: "Congelado"
```

**3. Intentar Registrar Asistencia (debe fallar)**
```
POST /api/asistencias
→ Error: "El paquete no está activo (estado: Congelado)."
```

**4. Descongelar (Admin)**
```
POST /api/paquetes/{id}/descongelar?idCongelacion=GUID
→ estado: "Activo", fechaVencimiento: 2026-02-17 (extendido 7 días)
```

**5. Ahora Sí Registrar Asistencia**
```
POST /api/asistencias
→ Success
```

---

### Escenario 3: Ownership Validation

**1. Admin Crea Paquete para Juan David**
```
POST /api/paquetes (Admin)
Body: { idAlumno: 295093d5-... }
→ IdPaquete: aabbccdd-...
```

**2. Juan David Consulta SU Paquete (Success)**
```
GET /api/paquetes/aabbccdd-... (Juan David)
→ 200 OK (ownership válido)
```

**3. Otro Alumno Intenta Consultar (Fail)**
```
GET /api/paquetes/aabbccdd-... (Otro alumno)
→ Error: "No tienes permiso para ver este paquete."
```

**4. Admin Consulta Cualquier Paquete (Success)**
```
GET /api/paquetes/aabbccdd-... (Admin)
→ 200 OK (admin bypass ownership)
```

---

## 🐛 Casos de Error Comunes

### 1. Token Expirado
```json
{
  "status": 401,
  "title": "Unauthorized"
}
```
**Solución:** Renovar token en Postman (Get New Access Token)

### 2. Permisos Insuficientes
```json
{
  "status": 403,
  "title": "Forbidden"
}
```
**Solución:** Verificar que el usuario tiene el rol correcto

### 3. Paquete No Encontrado
```json
{
  "error": "El paquete especificado no existe."
}
```
**Solución:** Verificar que el IdPaquete es correcto

### 4. Alumno No Existe
```json
{
  "error": "El alumno especificado no existe."
}
```
**Solución:** Usar IdAlumno válido de la BD

---

## 📊 Endpoints por Rol

| Endpoint | Admin | Profesor | Alumno |
|----------|-------|----------|--------|
| POST /api/paquetes | ✅ | ❌ | ❌ |
| GET /api/paquetes/{id} | ✅ (todos) | ❌ | ✅ (suyos) |
| PUT /api/paquetes/{id} | ✅ | ❌ | ❌ |
| POST /api/paquetes/{id}/congelar | ✅ | ❌ | ❌ |
| POST /api/paquetes/{id}/descongelar | ✅ | ❌ | ❌ |
| GET /api/alumnos/{id}/paquetes | ✅ (todos) | ❌ | ✅ (suyos) |
| POST /api/asistencias (descuenta) | ✅ | ✅ | ❌ |

---

## ✅ Checklist de Pruebas Completas

### FASE 1 - Crear y Consultar
- [ ] Crear paquete como Admin (success)
- [ ] Crear paquete con clases = 0 (fail)
- [ ] Crear paquete con alumno inexistente (fail)
- [ ] Consultar detalle de paquete como Admin (success)
- [ ] Consultar detalle de paquete como dueño (success)
- [ ] Consultar detalle de paquete como otro usuario (fail 403)
- [ ] Listar paquetes de alumno con filtros
- [ ] Listar paquetes con paginación

### FASE 2 - Descuento de Clases
- [ ] Registrar asistencia "Presente" (descuenta clase)
- [ ] Registrar asistencia "Ausente" (NO descuenta)
- [ ] Intentar usar paquete agotado (fail)
- [ ] Intentar usar paquete vencido (fail)
- [ ] Verificar que paquete cambia a Agotado al llegar a última clase

### FASE 3 - Congelaciones
- [ ] Congelar paquete activo (success)
- [ ] Intentar congelar paquete ya congelado (fail)
- [ ] Intentar usar paquete congelado (fail)
- [ ] Descongelar paquete (success)
- [ ] Verificar extensión de fecha de vencimiento

### FASE 4 - Edición
- [ ] Editar clases disponibles (success)
- [ ] Editar fecha de vencimiento (success)
- [ ] Intentar reducir clases por debajo de usadas (fail)
- [ ] Verificar recálculo automático de estado

---

## 🎓 Notas Importantes

1. **Descuento Automático:** Solo asistencias con `idEstadoAsistencia = 1` (Presente) descontarán clases.

2. **Cambio Automático a Agotado:** Cuando `clasesUsadas >= clasesDisponibles`, el paquete cambia automáticamente a estado Agotado (4).

3. **Extensión de Vencimiento:** Al descongelar, la fecha de vencimiento se extiende por los días que estuvo congelado.

4. **Ownership Strict:** Alumnos solo pueden consultar SUS propios paquetes. Admin tiene acceso total.

5. **Orden de Operaciones:** Es recomendable:
   - Crear paquete primero
   - Luego consultar para obtener IdPaquete
   - Luego registrar asistencias usando ese IdPaquete

6. **Múltiples Paquetes:** Un alumno puede tener varios paquetes activos simultáneamente. Al registrar asistencia, se especifica cuál usar.

---

**Fecha de Creación:** 11 de Enero, 2026  
**Versión:** 1.0  
**Autor:** GitHub Copilot

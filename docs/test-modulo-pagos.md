# 🧪 Casos de Prueba - Módulo Pagos

## 📋 Información General

**Base URL:** `https://localhost:7194`  
**Autenticación:** Bearer Token (OAuth 2.0 - Microsoft Entra External ID)

### Usuarios de Prueba

| Usuario | Email | Rol | IdUsuario | IdAlumno |
|---------|-------|-----|-----------|----------|
| Admin | Chetango@chetangoprueba.onmicrosoft.com | admin | b91e51b9-4094-441e-a5b6-062a846b3868 | - |
| Profesor Jorge | Jorgepadilla@chetangoprueba.onmicrosoft.com | profesor | 8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB | - |
| Alumno Juan David | JuanDavid@chetangoprueba.onmicrosoft.com | alumno | 71462106-9863-4fd0-b13d-9878ed231aa6 | 295093d5-b36f-4737-b68a-ab40ca871b2e |

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

## 💳 Métodos de Pago Disponibles

| Nombre | Descripción |
|--------|-------------|
| Efectivo | Pago en efectivo en la academia |
| Transferencia Bancaria | Transferencia a cuenta bancaria |
| Tarjeta Débito | Pago con tarjeta débito |
| Tarjeta Crédito | Pago con tarjeta crédito |
| Nequi | Pago por Nequi |
| Daviplata | Pago por Daviplata |

---

## 📚 FASE 0: Configuración Inicial

### 0. Script SQL - Insertar Métodos de Pago

**Ejecutar PRIMERO antes de cualquier prueba:**

```powershell
# Opción 1: Con sqlcmd
sqlcmd -S localhost -d ChetangoDB_Dev -i scripts\seed_metodos_pago.sql

# Opción 2: Conectar manualmente a SQL Server y ejecutar:
```

```sql
USE ChetangoDB_Dev;

INSERT INTO MetodosPago (Id, Nombre) VALUES
(NEWID(), 'Efectivo'),
(NEWID(), 'Transferencia Bancaria'),
(NEWID(), 'Tarjeta Débito'),
(NEWID(), 'Tarjeta Crédito'),
(NEWID(), 'Nequi'),
(NEWID(), 'Daviplata');
```

---

## 📦 FASE 1: Catálogos

### 1. GET /api/pagos/metodos-pago - Obtener Métodos de Pago

**Endpoint:** `GET /api/pagos/metodos-pago`  
**Autorización:** ApiScope (Cualquier usuario autenticado)  
**Usuario:** Admin, Profesor, Alumno

**Respuesta Esperada (200 OK):**
```json
[
  {
    "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
    "nombre": "Efectivo",
    "descripcion": null
  },
  {
    "idMetodoPago": "a2b3c4d5-e6f7-4890-9123-456789abcdef",
    "nombre": "Transferencia Bancaria",
    "descripcion": null
  },
  {
    "idMetodoPago": "b3c4d5e6-f7a8-4901-2345-6789abcdef01",
    "nombre": "Tarjeta Débito",
    "descripcion": null
  }
]
```

**Uso:** Copiar el `idMetodoPago` deseado para usarlo al registrar pagos.

---

### 2. GET /api/paquetes/tipos - Obtener Tipos de Paquete

**Endpoint:** `GET /api/paquetes/tipos`  
**Autorización:** ApiScope  
**Usuario:** Admin, Profesor, Alumno

**Respuesta Esperada (200 OK):**
```json
[
  {
    "id": "tipo-8-clases-guid",
    "nombre": "Paquete 8 Clases"
  },
  {
    "id": "tipo-12-clases-guid",
    "nombre": "Paquete 12 Clases"
  }
]
```

**Uso:** Necesario para especificar qué tipo de paquetes se crean al registrar un pago.

---

## 💰 FASE 2: Registrar Pagos

### 3. POST /api/pagos - Registrar Pago (2 Paquetes Iguales)

**Endpoint:** `POST /api/pagos`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000,
  "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
  "nota": "Pago inicial alumno Juan David - 2 paquetes de 8 clases",
  "paquetes": [
    {
      "idTipoPaquete": "tipo-8-clases-guid",
      "clasesDisponibles": 8,
      "diasVigencia": 30
    },
    {
      "idTipoPaquete": "tipo-8-clases-guid",
      "clasesDisponibles": 8,
      "diasVigencia": 30
    }
  ]
}
```

**Respuesta Esperada (200 OK):**
```json
{
  "idPago": "aabbccdd-1234-5678-90ab-cdef12345678",
  "idPaquetesCreados": [
    "paquete1-guid-1234-5678-90ab-cdef12345678",
    "paquete2-guid-2345-6789-01bc-def234567890"
  ],
  "montoTotal": 300000
}
```

**Validaciones:**
- ✅ Alumno debe existir
- ✅ Método de pago debe existir
- ✅ MontoTotal > 0
- ✅ Al menos un paquete en la lista
- ✅ Todos los tipos de paquete deben existir
- ✅ Si se especifican valores, suma <= MontoTotal
- ✅ Paquetes se crean con Estado = Activo (1)
- ✅ FechaActivacion = FechaPago
- ✅ FechaVencimiento = FechaPago + diasVigencia
- ✅ ClasesUsadas = 0
- ✅ IdPago vinculado a cada paquete

**Comportamiento del ValorPaquete:**
- Si NO se especifica `valorPaquete` en cada paquete: `MontoTotal / cantidad de paquetes`
- En este caso: 300000 / 2 = 150000 por paquete

**Casos de Error:**
```json
// MontoTotal <= 0
{
  "error": "El monto total debe ser mayor a cero."
}

// Lista de paquetes vacía
{
  "error": "Debe especificar al menos un paquete a crear."
}

// Alumno no existe
{
  "error": "El alumno especificado no existe."
}

// Método de pago no existe
{
  "error": "El método de pago especificado no existe."
}

// Tipo de paquete no existe
{
  "error": "Uno o más tipos de paquete especificados no existen."
}
```

---

### 4. POST /api/pagos - Registrar Pago (Valores Específicos)

**Endpoint:** `POST /api/pagos`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T14:00:00",
  "montoTotal": 300000,
  "idMetodoPago": "a2b3c4d5-e6f7-4890-9123-456789abcdef",
  "nota": "Pago con valores específicos - Paquete 8 + Paquete 12",
  "paquetes": [
    {
      "idTipoPaquete": "tipo-8-clases-guid",
      "clasesDisponibles": 8,
      "diasVigencia": 30,
      "valorPaquete": 130000
    },
    {
      "idTipoPaquete": "tipo-12-clases-guid",
      "clasesDisponibles": 12,
      "diasVigencia": 45,
      "valorPaquete": 170000
    }
  ]
}
```

**Respuesta Esperada (200 OK):**
```json
{
  "idPago": "bbccddee-2345-6789-01bc-def234567890",
  "idPaquetesCreados": [
    "paquete3-guid-3456-7890-12cd-ef3456789012",
    "paquete4-guid-4567-8901-23de-f45678901234"
  ],
  "montoTotal": 300000
}
```

**Validación adicional:**
- ✅ Suma de valorPaquete (130000 + 170000 = 300000) <= MontoTotal ✅
- Si suma > MontoTotal: `"La suma de los valores de los paquetes no puede ser mayor al monto total del pago."`

---

### 5. POST /api/pagos - Registrar Pago (Paquete Único)

**Endpoint:** `POST /api/pagos`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T16:00:00",
  "montoTotal": 200000,
  "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
  "nota": "Renovación - Paquete 12 clases",
  "paquetes": [
    {
      "idTipoPaquete": "tipo-12-clases-guid",
      "clasesDisponibles": 12,
      "diasVigencia": 45
    }
  ]
}
```

**Respuesta Esperada (200 OK):**
```json
{
  "idPago": "ccddee ff-3456-7890-12cd-ef3456789012",
  "idPaquetesCreados": [
    "paquete5-guid-5678-9012-34ef-567890123456"
  ],
  "montoTotal": 200000
}
```

**ValorPaquete:** 200000 (MontoTotal completo asignado al único paquete)

---

## 🔍 FASE 3: Consultar Pagos

### 6. GET /api/pagos/{id} - Ver Detalle de Pago (Admin)

**Endpoint:** `GET /api/pagos/{idPago}`  
**Autorización:** ApiScope (con ownership validation)  
**Usuario:** Admin (ve cualquier pago) o Alumno dueño

**Respuesta Esperada (200 OK):**
```json
{
  "idPago": "aabbccdd-1234-5678-90ab-cdef12345678",
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "nombreAlumno": "Juan David Perez",
  "correoAlumno": "JuanDavid@chetangoprueba.onmicrosoft.com",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000,
  "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
  "nombreMetodoPago": "Efectivo",
  "nota": "Pago inicial alumno Juan David - 2 paquetes de 8 clases",
  "fechaCreacion": "2026-01-11T10:05:23",
  "paquetes": [
    {
      "idPaquete": "paquete1-guid-1234-5678-90ab-cdef12345678",
      "nombreTipoPaquete": "Paquete 8 Clases",
      "clasesDisponibles": 8,
      "clasesUsadas": 0,
      "clasesRestantes": 8,
      "fechaVencimiento": "2026-02-10T10:00:00",
      "estado": "Activo",
      "valorPaquete": 150000
    },
    {
      "idPaquete": "paquete2-guid-2345-6789-01bc-def234567890",
      "nombreTipoPaquete": "Paquete 8 Clases",
      "clasesDisponibles": 8,
      "clasesUsadas": 0,
      "clasesRestantes": 8,
      "fechaVencimiento": "2026-02-10T10:00:00",
      "estado": "Activo",
      "valorPaquete": 150000
    }
  ]
}
```

**Ownership Validation:**
- ✅ Admin: puede ver cualquier pago
- ✅ Alumno: solo puede ver SUS propios pagos
- ❌ Otro alumno intentando ver pago: `400 Bad Request - "No tienes permiso para ver este pago."`

---

### 7. GET /api/pagos/{id} - Ver Detalle de Pago (Alumno Dueño)

**Endpoint:** `GET /api/pagos/{idPago}`  
**Autorización:** ApiScope  
**Usuario:** Alumno Juan David (token del alumno dueño del pago)

**Respuesta Esperada (200 OK):**
Misma estructura que el caso anterior

**Validación:**
- ✅ El alumno solo puede ver pagos donde `idAlumno` del pago = IdAlumno asociado a su email en el token

---

### 8. GET /api/mis-pagos - Mis Pagos (Alumno)

**Endpoint:** `GET /api/mis-pagos?pageNumber=1&pageSize=10`  
**Autorización:** ApiScope  
**Usuario:** Alumno Juan David

**Query Parameters:**
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)
- `fechaDesde` (datetime, opcional)
- `fechaHasta` (datetime, opcional)
- `idMetodoPago` (guid, opcional)

**Respuesta Esperada (200 OK):**
```json
{
  "items": [
    {
      "idPago": "aabbccdd-1234-5678-90ab-cdef12345678",
      "fechaPago": "2026-01-11T10:00:00",
      "montoTotal": 300000,
      "nombreMetodoPago": "Efectivo",
      "nombreAlumno": "Juan David Perez",
      "cantidadPaquetes": 2
    },
    {
      "idPago": "bbccddee-2345-6789-01bc-def234567890",
      "fechaPago": "2026-01-11T14:00:00",
      "montoTotal": 300000,
      "nombreMetodoPago": "Transferencia Bancaria",
      "nombreAlumno": "Juan David Perez",
      "cantidadPaquetes": 2
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 2,
  "totalPages": 1
}
```

**Comportamiento:**
- ✅ Busca automáticamente el alumno por el email del token JWT
- ✅ Retorna solo los pagos de ese alumno
- ✅ Si el usuario no tiene alumno asociado, retorna lista vacía

---

### 9. GET /api/mis-pagos - Mis Pagos con Filtros (Alumno)

**Endpoint:** `GET /api/mis-pagos?pageNumber=1&pageSize=10&fechaDesde=2026-01-01&fechaHasta=2026-01-31&idMetodoPago=f1e2d3c4-b5a6-4798-8102-3456789abcde`  
**Autorización:** ApiScope  
**Usuario:** Alumno Juan David

**Respuesta Esperada (200 OK):**
Lista filtrada por fechas y método de pago (misma estructura que caso anterior)

---

### 10. GET /api/alumnos/{idAlumno}/pagos - Pagos de Alumno (Admin)

**Endpoint:** `GET /api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/pagos?pageNumber=1&pageSize=10`  
**Autorización:** ApiScope (con ownership validation)  
**Usuario:** Admin

**Query Parameters:**
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)
- `fechaDesde` (datetime, opcional)
- `fechaHasta` (datetime, opcional)
- `idMetodoPago` (guid, opcional)

**Respuesta Esperada (200 OK):**
```json
{
  "items": [
    {
      "idPago": "aabbccdd-1234-5678-90ab-cdef12345678",
      "fechaPago": "2026-01-11T10:00:00",
      "montoTotal": 300000,
      "nombreMetodoPago": "Efectivo",
      "nombreAlumno": "Juan David Perez",
      "cantidadPaquetes": 2
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 3,
  "totalPages": 1
}
```

**Validación:**
- ✅ Admin: puede ver pagos de cualquier alumno
- ✅ Alumno: solo puede ver sus propios pagos (ownership por email)

---

### 11. GET /api/alumnos/{idAlumno}/pagos - Pagos de Alumno (Mismo Alumno)

**Endpoint:** `GET /api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/pagos`  
**Autorización:** ApiScope  
**Usuario:** Alumno Juan David (token del alumno con ese idAlumno)

**Respuesta Esperada (200 OK):**
Misma estructura que el caso anterior

**Validación:**
- ✅ Ownership validation pasa porque el email del token corresponde al IdAlumno de la URL

---

### 12. GET /api/alumnos/{idAlumno}/pagos - Pagos de Otro Alumno (DEBE FALLAR)

**Endpoint:** `GET /api/alumnos/{otroAlumnoId}/pagos`  
**Autorización:** ApiScope  
**Usuario:** Alumno Juan David (intentando ver pagos de otro alumno)

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "No tienes permiso para ver los pagos de este alumno."
}
```

**Validación:**
- ❌ Ownership validation falla porque el email del token NO corresponde al IdAlumno de la URL
- ✅ Solo admin o el alumno dueño pueden ver los pagos

---

## 📊 FASE 4: Estadísticas

### 13. GET /api/pagos/estadisticas - Estadísticas de Pagos (Admin)

**Endpoint:** `GET /api/pagos/estadisticas?fechaDesde=2026-01-01&fechaHasta=2026-01-31`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Query Parameters:**
- `fechaDesde` (datetime, opcional)
- `fechaHasta` (datetime, opcional)

**Respuesta Esperada (200 OK):**
```json
{
  "totalRecaudado": 800000,
  "cantidadPagos": 3,
  "promedioMonto": 266666.67,
  "desgloseMetodosPago": [
    {
      "nombreMetodo": "Efectivo",
      "totalRecaudado": 500000,
      "cantidadPagos": 2
    },
    {
      "nombreMetodo": "Transferencia Bancaria",
      "totalRecaudado": 300000,
      "cantidadPagos": 1
    }
  ]
}
```

**Validaciones:**
- ✅ Solo admin puede acceder
- ✅ Filtros de fecha son opcionales
- ✅ DesgloseMetodosPago ordenado por TotalRecaudado descendente

---

### 14. GET /api/pagos/estadisticas - Sin Filtros (Admin)

**Endpoint:** `GET /api/pagos/estadisticas`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Respuesta Esperada (200 OK):**
Estadísticas de TODOS los pagos del sistema (sin filtro de fecha)

---

## ✏️ FASE 5: Editar Pagos

### 15. PUT /api/pagos/{id} - Editar Pago (Admin)

**Endpoint:** `PUT /api/pagos/{idPago}`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request Body:**
```json
{
  "montoTotal": 320000,
  "idMetodoPago": "a2b3c4d5-e6f7-4890-9123-456789abcdef",
  "nota": "Monto ajustado por descuento aplicado retrospectivamente"
}
```

**Respuesta Esperada (204 No Content)**

**Validaciones:**
- ✅ Pago debe existir
- ✅ MontoTotal > 0
- ✅ Método de pago debe existir
- ❌ NO se permite cambiar `idAlumno`
- ❌ NO se permite cambiar `fechaPago`
- ❌ NO se permite cambiar los paquetes asociados

**Casos de Error:**
```json
// MontoTotal <= 0
{
  "error": "El monto total debe ser mayor a cero."
}

// Pago no existe
{
  "error": "El pago especificado no existe."
}

// Método de pago no existe
{
  "error": "El método de pago especificado no existe."
}
```

---

## 🔗 FASE 6: Integración con Paquetes

### 16. GET /api/alumnos/{idAlumno}/paquetes - Verificar Paquetes Creados por Pago

**Endpoint:** `GET /api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/paquetes`  
**Autorización:** ApiScope  
**Usuario:** Admin o Alumno Juan David

**Respuesta Esperada (200 OK):**
```json
{
  "items": [
    {
      "idPaquete": "paquete1-guid-1234-5678-90ab-cdef12345678",
      "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
      "nombreAlumno": "Juan David Perez",
      "documentoAlumno": "1674588987",
      "nombreTipoPaquete": "Paquete 8 Clases",
      "clasesDisponibles": 8,
      "clasesUsadas": 0,
      "clasesRestantes": 8,
      "fechaActivacion": "2026-01-11T10:00:00",
      "fechaVencimiento": "2026-02-10T10:00:00",
      "estado": "Activo",
      "estaVencido": false,
      "tieneClasesDisponibles": true,
      "valorPaquete": 150000
    },
    {
      "idPaquete": "paquete2-guid-2345-6789-01bc-def234567890",
      "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
      "nombreAlumno": "Juan David Perez",
      "documentoAlumno": "1674588987",
      "nombreTipoPaquete": "Paquete 8 Clases",
      "clasesDisponibles": 8,
      "clasesUsadas": 0,
      "clasesRestantes": 8,
      "fechaActivacion": "2026-01-11T10:00:00",
      "fechaVencimiento": "2026-02-10T10:00:00",
      "estado": "Activo",
      "estaVencido": false,
      "tieneClasesDisponibles": true,
      "valorPaquete": 150000
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 2,
  "totalPages": 1
}
```

**Validación:**
- ✅ Los paquetes nuevos aparecen en la lista
- ✅ Cada paquete tiene `IdPago` vinculado (no visible en respuesta pero existe en BD)
- ✅ FechaActivacion = FechaPago del pago
- ✅ Estado = Activo
- ✅ ClasesUsadas = 0

---

### 17. GET /api/paquetes/{id} - Ver Detalle de Paquete Creado por Pago

**Endpoint:** `GET /api/paquetes/{idPaquete}`  
**Autorización:** ApiScope  
**Usuario:** Admin o Alumno Juan David

**Respuesta Esperada (200 OK):**
```json
{
  "idPaquete": "paquete1-guid-1234-5678-90ab-cdef12345678",
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "nombreAlumno": "Juan David Perez",
  "idTipoPaquete": "tipo-8-clases-guid",
  "nombreTipoPaquete": "Paquete 8 Clases",
  "clasesDisponibles": 8,
  "clasesUsadas": 0,
  "clasesRestantes": 8,
  "fechaActivacion": "2026-01-11T10:00:00",
  "fechaVencimiento": "2026-02-10T10:00:00",
  "valorPaquete": 150000,
  "idEstado": 1,
  "estado": "Activo",
  "estaVencido": false,
  "tieneClasesDisponibles": true,
  "congelaciones": []
}
```

**Nota:** El `IdPago` no se muestra en la respuesta actual pero existe en la base de datos vinculando el paquete al pago que lo creó.

---

## ⚠️ FASE 7: Casos de Error

### 18. POST /api/pagos - Error: MontoTotal = 0

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 0,
  "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
  "nota": "Test error",
  "paquetes": [
    {
      "idTipoPaquete": "tipo-8-clases-guid",
      "clasesDisponibles": 8,
      "diasVigencia": 30
    }
  ]
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "El monto total debe ser mayor a cero."
}
```

---

### 19. POST /api/pagos - Error: Lista de Paquetes Vacía

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000,
  "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
  "nota": "Test error",
  "paquetes": []
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "Debe especificar al menos un paquete a crear."
}
```

---

### 20. POST /api/pagos - Error: Alumno No Existe

**Request Body:**
```json
{
  "idAlumno": "00000000-0000-0000-0000-000000000000",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000,
  "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
  "nota": "Test error",
  "paquetes": [
    {
      "idTipoPaquete": "tipo-8-clases-guid",
      "clasesDisponibles": 8,
      "diasVigencia": 30
    }
  ]
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "El alumno especificado no existe."
}
```

---

### 21. POST /api/pagos - Error: Método de Pago No Existe

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000,
  "idMetodoPago": "00000000-0000-0000-0000-000000000000",
  "nota": "Test error",
  "paquetes": [
    {
      "idTipoPaquete": "tipo-8-clases-guid",
      "clasesDisponibles": 8,
      "diasVigencia": 30
    }
  ]
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "El método de pago especificado no existe."
}
```

---

### 22. POST /api/pagos - Error: Tipo de Paquete No Existe

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000,
  "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
  "nota": "Test error",
  "paquetes": [
    {
      "idTipoPaquete": "00000000-0000-0000-0000-000000000000",
      "clasesDisponibles": 8,
      "diasVigencia": 30
    }
  ]
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "Uno o más tipos de paquete especificados no existen."
}
```

---

### 23. POST /api/pagos - Error: Suma de Valores > MontoTotal

**Request Body:**
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000,
  "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
  "nota": "Test error",
  "paquetes": [
    {
      "idTipoPaquete": "tipo-8-clases-guid",
      "clasesDisponibles": 8,
      "diasVigencia": 30,
      "valorPaquete": 200000
    },
    {
      "idTipoPaquete": "tipo-12-clases-guid",
      "clasesDisponibles": 12,
      "diasVigencia": 45,
      "valorPaquete": 200000
    }
  ]
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "La suma de los valores de los paquetes no puede ser mayor al monto total del pago."
}
```

---

### 24. GET /api/pagos/{id} - Error: Pago No Existe

**Endpoint:** `GET /api/pagos/00000000-0000-0000-0000-000000000000`  
**Usuario:** Admin

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "El pago especificado no existe."
}
```

---

### 25. PUT /api/pagos/{id} - Error: Pago No Existe

**Endpoint:** `PUT /api/pagos/00000000-0000-0000-0000-000000000000`  
**Usuario:** Admin

**Request Body:**
```json
{
  "montoTotal": 320000,
  "idMetodoPago": "f1e2d3c4-b5a6-4798-8102-3456789abcde",
  "nota": "Test"
}
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "El pago especificado no existe."
}
```

---

## 📋 Resumen de Tests

### Tests Exitosos (200/201/204)
- ✅ GET Métodos de Pago
- ✅ POST Registrar Pago (2 paquetes iguales)
- ✅ POST Registrar Pago (valores específicos)
- ✅ POST Registrar Pago (paquete único)
- ✅ GET Detalle de Pago (admin)
- ✅ GET Detalle de Pago (alumno dueño)
- ✅ GET Mis Pagos (alumno)
- ✅ GET Mis Pagos con filtros (alumno)
- ✅ GET Pagos de alumno (admin)
- ✅ GET Pagos de alumno (mismo alumno)
- ✅ GET Estadísticas (admin)
- ✅ GET Estadísticas sin filtros (admin)
- ✅ PUT Editar Pago (admin)
- ✅ GET Verificar paquetes creados
- ✅ GET Detalle de paquete creado por pago

### Tests de Error (400/403)
- ❌ POST Pago con MontoTotal = 0
- ❌ POST Pago con lista de paquetes vacía
- ❌ POST Pago con alumno inexistente
- ❌ POST Pago con método de pago inexistente
- ❌ POST Pago con tipo de paquete inexistente
- ❌ POST Pago con suma de valores > MontoTotal
- ❌ GET Pago inexistente
- ❌ GET Alumno intenta ver pagos de otro alumno
- ❌ PUT Pago inexistente

---

## 🔗 Integración con Otros Módulos

### Módulo Paquetes
- ✅ Al registrar un pago se crean paquetes automáticamente
- ✅ Los paquetes tienen `IdPago` vinculado
- ✅ FechaActivacion = FechaPago
- ✅ Estado inicial = Activo (1)
- ✅ ClasesUsadas = 0

### Módulo Asistencias
- ℹ️ Indirectamente: Los paquetes creados por pagos se usan para registrar asistencias
- ℹ️ Las asistencias descuentan clases de paquetes generados por pagos

---

## 📝 Notas Importantes

1. **Transacciones:** RegistrarPago es atómico (pago + paquetes en una sola transacción)
2. **Ownership:** Siempre por EMAIL del token JWT, NO por OID
3. **Validaciones:** Inline en Handlers (sin FluentValidation)
4. **Result Pattern:** Todos los handlers retornan `Result<T>`
5. **ValorPaquete:** Se divide automáticamente si no se especifica
6. **Paquetes:** Vinculados al pago mediante `IdPago`
7. **Estados:** Paquetes siempre se crean con Estado = Activo (1)

---

**Última actualización:** 11 Enero 2026  
**Estado:** Módulo Pagos 100% implementado y listo para pruebas

# Documentación API - Módulo Pagos

## Descripción General
El módulo de Pagos gestiona el registro y consulta de pagos realizados por alumnos en la academia de tango Chetango. Un pago puede generar múltiples paquetes de clases automáticamente.

## URL Base
```
https://localhost:7194
```

## Autenticación
Todos los endpoints requieren autenticación mediante Bearer Token (JWT) de Microsoft Entra External ID.

```
Authorization: Bearer {token}
```

## Roles
- **admin / Administrador**: Acceso completo a todos los endpoints
- **alumno**: Acceso limitado a sus propios datos mediante ownership validation por email

---

## Endpoints

### 1. GET /api/pagos/metodos-pago
**Descripción**: Obtener catálogo de métodos de pago disponibles  
**Autorización**: ApiScope (cualquier usuario autenticado)  
**Response**: 200 OK

```json
[
  {
    "idMetodoPago": "guid",
    "nombre": "Efectivo",
    "descripcion": "Pago en efectivo en la academia"
  },
  {
    "idMetodoPago": "guid",
    "nombre": "Transferencia Bancaria",
    "descripcion": "Transferencia a cuenta bancaria"
  }
]
```

---

### 2. GET /api/pagos/estadisticas
**Descripción**: Obtener estadísticas de pagos por período  
**Autorización**: AdminOnly  
**Query Parameters**:
- `fechaDesde` (DateTime, opcional): Fecha inicial del período
- `fechaHasta` (DateTime, opcional): Fecha final del período

**Response**: 200 OK

```json
{
  "totalRecaudado": 1500000.00,
  "cantidadPagos": 10,
  "promedioMonto": 150000.00,
  "desgloseMetodosPago": [
    {
      "nombreMetodo": "Efectivo",
      "totalRecaudado": 800000.00,
      "cantidadPagos": 6
    },
    {
      "nombreMetodo": "Transferencia Bancaria",
      "totalRecaudado": 700000.00,
      "cantidadPagos": 4
    }
  ]
}
```

---

### 3. GET /api/pagos/{id}
**Descripción**: Obtener detalle de un pago específico  
**Autorización**: ApiScope + Ownership (admin ve todos, alumno solo los suyos)  
**Path Parameter**: `id` (guid) - ID del pago

**Response**: 200 OK

```json
{
  "idPago": "guid",
  "idAlumno": "guid",
  "nombreAlumno": "Juan David Pérez",
  "correoAlumno": "JuanDavid@chetangoprueba.onmicrosoft.com",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000.00,
  "idMetodoPago": "guid",
  "nombreMetodoPago": "Efectivo",
  "nota": "Pago inicial alumno Juan David",
  "fechaCreacion": "2026-01-11T10:05:00",
  "paquetes": [
    {
      "idPaquete": "guid",
      "nombreTipoPaquete": "8 Clases",
      "clasesDisponibles": 8,
      "clasesUsadas": 0,
      "clasesRestantes": 8,
      "fechaVencimiento": "2026-02-10T10:00:00",
      "estado": "Activo",
      "valorPaquete": 150000.00
    },
    {
      "idPaquete": "guid",
      "nombreTipoPaquete": "8 Clases",
      "clasesDisponibles": 8,
      "clasesUsadas": 0,
      "clasesRestantes": 8,
      "fechaVencimiento": "2026-02-10T10:00:00",
      "estado": "Activo",
      "valorPaquete": 150000.00
    }
  ]
}
```

**Errores**:
- 401 Unauthorized: Token inválido o no proporcionado
- 400 Bad Request: El pago no existe
- 400 Bad Request: No tienes permiso para ver este pago

---

### 4. GET /api/mis-pagos
**Descripción**: Obtener lista paginada de pagos del alumno autenticado  
**Autorización**: ApiScope  
**Query Parameters**:
- `fechaDesde` (DateTime, opcional): Filtrar desde fecha
- `fechaHasta` (DateTime, opcional): Filtrar hasta fecha
- `idMetodoPago` (guid, opcional): Filtrar por método de pago
- `pageNumber` (int, default: 1): Número de página
- `pageSize` (int, default: 10): Tamaño de página

**Response**: 200 OK

```json
{
  "items": [
    {
      "idPago": "guid",
      "fechaPago": "2026-01-11T10:00:00",
      "montoTotal": 300000.00,
      "nombreMetodoPago": "Efectivo",
      "nombreAlumno": "Juan David Pérez",
      "cantidadPaquetes": 2
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1
}
```

---

### 5. GET /api/alumnos/{idAlumno}/pagos
**Descripción**: Obtener lista paginada de pagos de un alumno específico  
**Autorización**: ApiScope + Ownership (admin ve cualquier alumno, alumno solo los suyos)  
**Path Parameter**: `idAlumno` (guid) - ID del alumno  
**Query Parameters**:
- `fechaDesde` (DateTime, opcional): Filtrar desde fecha
- `fechaHasta` (DateTime, opcional): Filtrar hasta fecha
- `idMetodoPago` (guid, opcional): Filtrar por método de pago
- `pageNumber` (int, default: 1): Número de página
- `pageSize` (int, default: 10): Tamaño de página

**Response**: 200 OK (mismo formato que /api/mis-pagos)

**Errores**:
- 400 Bad Request: El alumno especificado no existe
- 400 Bad Request: No tienes permiso para ver los pagos de este alumno

---

### 6. POST /api/pagos
**Descripción**: Registrar un nuevo pago y crear paquetes asociados  
**Autorización**: AdminOnly  
**Request Body**:

```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000.00,
  "idMetodoPago": "guid-del-metodo-pago",
  "nota": "Pago inicial alumno Juan David",
  "paquetes": [
    {
      "idTipoPaquete": "guid-8-clases"
    },
    {
      "idTipoPaquete": "guid-8-clases"
    }
  ]
}
```

**Nota**: Si no se especifica `valorPaquete` en cada paquete, el `montoTotal` se divide equitativamente entre todos los paquetes.

**Opción con valores específicos**:
```json
{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000.00,
  "idMetodoPago": "guid-del-metodo-pago",
  "nota": "Pago con descuento",
  "paquetes": [
    {
      "idTipoPaquete": "guid-8-clases",
      "valorPaquete": 130000.00
    },
    {
      "idTipoPaquete": "guid-12-clases",
      "valorPaquete": 170000.00
    }
  ]
}
```

**Response**: 200 OK

```json
{
  "idPago": "guid",
  "idPaquetesCreados": [
    "guid-paquete-1",
    "guid-paquete-2"
  ],
  "montoTotal": 300000.00
}
```

**Errores**:
- 400 Bad Request: El monto total debe ser mayor a cero
- 400 Bad Request: Debe especificar al menos un paquete a crear
- 400 Bad Request: El alumno especificado no existe
- 400 Bad Request: El método de pago especificado no existe
- 400 Bad Request: Uno o más tipos de paquete especificados no existen
- 400 Bad Request: La suma de los valores de los paquetes no puede ser mayor al monto total del pago

---

### 7. PUT /api/pagos/{id}
**Descripción**: Editar un pago existente (solo monto, método y nota)  
**Autorización**: AdminOnly  
**Path Parameter**: `id` (guid) - ID del pago  
**Request Body**:

```json
{
  "montoTotal": 320000.00,
  "idMetodoPago": "guid-del-metodo-pago",
  "nota": "Nota actualizada"
}
```

**Response**: 204 No Content

**Errores**:
- 400 Bad Request: El monto total debe ser mayor a cero
- 400 Bad Request: El pago especificado no existe
- 400 Bad Request: El método de pago especificado no existe

**Importante**: NO se permite cambiar `idAlumno` ni `fechaPago`.

---

## Reglas de Negocio

### Registro de Pagos
1. Un pago puede generar **múltiples paquetes** de clases
2. Los paquetes se crean automáticamente con:
   - Estado = Activo (IdEstado = 1)
   - ClasesUsadas = 0
   - FechaActivacion = FechaPago
   - FechaVencimiento = FechaActivacion + días del tipo de paquete
   - IdPago vinculado al pago registrado
3. El `montoTotal` se divide entre los paquetes si no se especifican valores individuales
4. Si se especifican valores, la suma NO puede exceder el `montoTotal`

### Ownership Validation
- Se valida por **EMAIL** del usuario (no por OID)
- Admin puede ver y gestionar todos los pagos
- Alumno solo puede ver sus propios pagos
- La validación se hace en los Handlers, no en los endpoints

### Transacciones
- `RegistrarPagoCommand` usa transacción para crear pago + paquetes
- Si falla la creación de algún paquete, se hace rollback completo

---

## Casos de Prueba - Postman

### Setup
1. Obtener token Bearer de autenticación CIAM
2. Configurar variable de entorno: `baseUrl = https://localhost:7194`
3. Agregar header en todas las requests: `Authorization: Bearer {{token}}`

### Test 1: Obtener Métodos de Pago (Admin)
```
GET {{baseUrl}}/api/pagos/metodos-pago
Authorization: Bearer {{token_admin}}
```
**Expected**: 200 OK con lista de 6 métodos de pago

### Test 2: Registrar Pago con 2 Paquetes (Admin)
```
POST {{baseUrl}}/api/pagos
Authorization: Bearer {{token_admin}}
Content-Type: application/json

{
  "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
  "fechaPago": "2026-01-11T10:00:00",
  "montoTotal": 300000,
  "idMetodoPago": "{{guid_efectivo}}",
  "nota": "Pago inicial alumno Juan David",
  "paquetes": [
    {
      "idTipoPaquete": "{{guid_8_clases}}"
    },
    {
      "idTipoPaquete": "{{guid_8_clases}}"
    }
  ]
}
```
**Expected**: 200 OK con idPago y lista de 2 idPaquetesCreados

### Test 3: Ver Detalle del Pago (Admin)
```
GET {{baseUrl}}/api/pagos/{{idPago}}
Authorization: Bearer {{token_admin}}
```
**Expected**: 200 OK con detalle del pago + 2 paquetes

### Test 4: Mis Pagos (Alumno Juan David)
```
GET {{baseUrl}}/api/mis-pagos?pageNumber=1&pageSize=10
Authorization: Bearer {{token_juandavid}}
```
**Expected**: 200 OK con lista paginada de pagos del alumno

### Test 5: Pagos de Alumno (Admin)
```
GET {{baseUrl}}/api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/pagos
Authorization: Bearer {{token_admin}}
```
**Expected**: 200 OK con lista de pagos del alumno

### Test 6: Pagos de Alumno (Alumno mismo)
```
GET {{baseUrl}}/api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/pagos
Authorization: Bearer {{token_juandavid}}
```
**Expected**: 200 OK (ownership validation pasa)

### Test 7: Pagos de Alumno (Alumno diferente) - DEBE FALLAR
```
GET {{baseUrl}}/api/alumnos/{{otro_alumno_id}}/pagos
Authorization: Bearer {{token_juandavid}}
```
**Expected**: 400 Bad Request "No tienes permiso para ver los pagos de este alumno"

### Test 8: Estadísticas (Admin)
```
GET {{baseUrl}}/api/pagos/estadisticas?fechaDesde=2026-01-01&fechaHasta=2026-01-31
Authorization: Bearer {{token_admin}}
```
**Expected**: 200 OK con totalRecaudado, cantidadPagos, promedioMonto, desglose

### Test 9: Verificar Paquetes Creados
```
GET {{baseUrl}}/api/alumnos/295093d5-b36f-4737-b68a-ab40ca871b2e/paquetes
Authorization: Bearer {{token_admin}}
```
**Expected**: 200 OK - Debe incluir los 2 paquetes nuevos con IdPago vinculado

### Test 10: Editar Pago (Admin)
```
PUT {{baseUrl}}/api/pagos/{{idPago}}
Authorization: Bearer {{token_admin}}
Content-Type: application/json

{
  "montoTotal": 320000,
  "idMetodoPago": "{{guid_transferencia}}",
  "nota": "Monto ajustado"
}
```
**Expected**: 204 No Content

---

## Datos de Prueba

### Usuarios

**Admin**:
- Email: `Chetango@chetangoprueba.onmicrosoft.com`
- Contraseña: `Chet4ngo20#`
- IdUsuario: `b91e51b9-4094-441e-a5b6-062a846b3868`

**Alumno Juan David**:
- Email: `JuanDavid@chetangoprueba.onmicrosoft.com`
- Contraseña: `Juaj0rge20#`
- IdUsuario: `71462106-9863-4fd0-b13d-9878ed231aa6`
- IdAlumno: `295093d5-b36f-4737-b68a-ab40ca871b2e`

### Métodos de Pago (obtener IDs con GET /api/pagos/metodos-pago)
- Efectivo
- Transferencia Bancaria
- Tarjeta Débito
- Tarjeta Crédito
- Nequi
- Daviplata

### Tipos de Paquete (obtener IDs con GET /api/paquetes/tipos)
- 8 Clases (30 días)
- 12 Clases (45 días)
- 20 Clases (60 días)
- Clase Individual

---

## Notas Importantes

1. **Script SQL**: Ejecutar `scripts/seed_metodos_pago.sql` antes de usar el módulo
2. **Transacciones**: Registrar pago es atómico (pago + paquetes)
3. **Ownership**: Siempre por EMAIL, no OID
4. **Validaciones**: Inline en Handlers (sin FluentValidation)
5. **Paquetes**: Se crean con Estado=Activo y vinculados al pago
6. **FechaActivacion**: Igual a FechaPago del pago
7. **ValorPaquete**: Si no se especifica, se calcula automáticamente (montoTotal / cantidad)

---

## Integración con Otros Módulos

### Módulo Paquetes
- Al registrar un pago se crean paquetes automáticamente
- Los paquetes tienen `IdPago` vinculado
- Se puede consultar paquetes por alumno y ver su pago asociado

### Módulo Asistencias
- Indirectamente: Paquetes creados por pagos se usan para registrar asistencias
- Las asistencias descuentan clases de paquetes generados por pagos

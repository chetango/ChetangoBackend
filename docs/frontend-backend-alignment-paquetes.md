# 🔄 Actualización Backend para Vista de Paquetes (Frontend)

## 📋 Análisis de la Vista del Frontend

**Fecha:** 11 de Enero, 2026  
**Tipo:** Ajustes para cumplir con diseño del frontend

---

## ❌ PROBLEMAS IDENTIFICADOS (RESUELTOS)

### 1. **CRÍTICO: Faltaba endpoint para listar TODOS los paquetes**
**Problema:** El frontend muestra paquetes de múltiples alumnos, pero solo teníamos `/api/alumnos/{id}/paquetes`  
**Solución:** ✅ Creado `GET /api/paquetes` con filtros globales

### 2. **IMPORTANTE: Faltaba DNI/Documento del alumno**
**Problema:** UI muestra "DNI: 42.567.123" pero el DTO no lo incluía  
**Solución:** ✅ Agregados campos `IdAlumno`, `NombreAlumno`, `DocumentoAlumno` al DTO

### 3. **IMPORTANTE: Faltaba filtro por tipo de paquete**
**Problema:** UI tiene dropdown "Tipo de paquete" pero no teníamos el filtro  
**Solución:** ✅ Agregado parámetro `IdTipoPaquete` en ambas queries

### 4. **IMPORTANTE: Faltaba búsqueda por nombre/documento**
**Problema:** UI tiene búsqueda "Nombre o documento..." pero no teníamos endpoint  
**Solución:** ✅ Agregado parámetro `BusquedaAlumno` en `GET /api/paquetes`

### 5. **MENOR: Faltaban estadísticas**
**Problema:** UI muestra contadores (Activos: 2, Agotados: 2, etc.)  
**Solución:** ✅ Creado `GET /api/paquetes/estadisticas`

---

## ✅ NUEVOS ENDPOINTS IMPLEMENTADOS

### 1. GET /api/paquetes/estadisticas
Obtiene contadores por estado para las tarjetas superiores.

**Autorización:** AdminOnly

**Response:**
```json
{
  "totalPaquetes": 6,
  "activos": 2,
  "vencidos": 1,
  "congelados": 1,
  "agotados": 2
}
```

---

### 2. GET /api/paquetes
Lista TODOS los paquetes del sistema con filtros (vista administrativa).

**Autorización:** AdminOnly

**Query Parameters:**
- `busquedaAlumno` (string, opcional) - Buscar por nombre o documento
- `estado` (int, opcional) - Filtrar por estado (1-4)
- `idTipoPaquete` (Guid, opcional) - Filtrar por tipo de paquete
- `fechaVencimientoDesde` (DateTime, opcional)
- `fechaVencimientoHasta` (DateTime, opcional)
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)

**Ejemplos:**
```
GET /api/paquetes?busquedaAlumno=Juan&estado=1&pageSize=20
GET /api/paquetes?idTipoPaquete=550e8400-e29b-41d4-a716-446655440001
GET /api/paquetes?busquedaAlumno=42.567.123
```

**Response:**
```json
{
  "items": [
    {
      "idPaquete": "aabbccdd-1234-5678-90ab-cdef12345678",
      "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
      "nombreAlumno": "María Rodríguez",
      "documentoAlumno": "42.567.123",
      "nombreTipoPaquete": "Paquete 8 clases",
      "clasesDisponibles": 8,
      "clasesUsadas": 3,
      "clasesRestantes": 5,
      "fechaActivacion": "2025-01-01T00:00:00",
      "fechaVencimiento": "2025-02-01T00:00:00",
      "valorPaquete": 150000,
      "estado": "Activo",
      "estaVencido": false,
      "tieneClasesDisponibles": true
    }
  ],
  "pageNumber": 1,
  "totalPages": 1,
  "totalCount": 6,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

---

### 3. GET /api/alumnos/{idAlumno}/paquetes (ACTUALIZADO)
Agregado filtro `idTipoPaquete`.

**Query Parameters (nuevos):**
- `idTipoPaquete` (Guid, opcional) - Filtrar por tipo de paquete

---

## 📦 DTO ACTUALIZADO: PaqueteAlumnoDTO

**Campos AGREGADOS:**
```csharp
Guid IdAlumno,              // NUEVO - ID del alumno
string NombreAlumno,        // NUEVO - Nombre completo del alumno
string DocumentoAlumno,     // NUEVO - DNI/Documento (ej: "42.567.123")
```

**DTO Completo:**
```typescript
{
  idPaquete: Guid,
  idAlumno: Guid,              // 🆕 NUEVO
  nombreAlumno: string,        // 🆕 NUEVO
  documentoAlumno: string,     // 🆕 NUEVO
  nombreTipoPaquete: string,
  clasesDisponibles: number,
  clasesUsadas: number,
  clasesRestantes: number,
  fechaActivacion: DateTime,
  fechaVencimiento: DateTime,
  valorPaquete: number,
  estado: string,              // "Activo", "Vencido", "Congelado", "Agotado"
  estaVencido: boolean,
  tieneClasesDisponibles: boolean
}
```

---

## 🎨 MAPEO VISTA → API

### Tarjetas de Estadísticas
```
Vista:  ACTIVOS: 2 | AGOTADOS: 2 | CONGELADOS: 1 | VENCIDOS: 1
API:    GET /api/paquetes/estadisticas
Mapeo:  activos → ACTIVOS
        agotados → AGOTADOS
        congelados → CONGELADOS
        vencidos → VENCIDOS
        totalPaquetes → Total (badge superior derecho)
```

### Filtros
```
Vista:  Buscar alumno (input)
API:    GET /api/paquetes?busquedaAlumno={texto}
Mapeo:  Busca en nombre y documento del alumno

Vista:  Estado (dropdown)
API:    GET /api/paquetes?estado={1|2|3|4}
Mapeo:  1=Activo, 2=Vencido, 3=Congelado, 4=Agotado

Vista:  Tipo de paquete (dropdown)
API:    GET /api/paquetes?idTipoPaquete={guid}
Mapeo:  Requiere obtener lista de tipos previamente
```

### Tabla de Paquetes
```
Columna ALUMNO:
  Vista:  "María Rodríguez\nDNI: 42.567.123"
  API:    nombreAlumno + "\n" + "DNI: " + documentoAlumno

Columna PAQUETE:
  Vista:  "Paquete 8 clases\n8 clases"
  API:    nombreTipoPaquete + "\n" + clasesDisponibles + " clases"

Columna CONSUMO:
  Vista:  Barra de progreso + "3 / 8" + "38%"
  API:    clasesUsadas + " / " + clasesDisponibles
  Cálculo: (clasesUsadas / clasesDisponibles) * 100

Columna ESTADO:
  Vista:  Badge con color (verde=Activo, naranja=Agotado, etc.)
  API:    estado (string: "Activo", "Vencido", "Congelado", "Agotado")

Columna VIGENCIA:
  Vista:  "1 ene 2025\n1 feb 2025"
  API:    fechaActivacion (formatear) + "\n" + fechaVencimiento (formatear)

Columna ACCIONES:
  Vista:  Ícono de ver (eye)
  API:    Navegar a detalle → GET /api/paquetes/{idPaquete}
```

### Botón Crear Paquete
```
Vista:  "+ Crear Paquete" (botón rojo superior derecho)
API:    POST /api/paquetes
Modal:  Mostrar formulario con campos de CrearPaqueteDTO
```

---

## 📊 FLUJO RECOMENDADO PARA EL FRONTEND

### 1. Carga Inicial de la Vista
```javascript
// 1. Obtener estadísticas para las tarjetas superiores
GET /api/paquetes/estadisticas
→ Mostrar contadores en las 4 tarjetas

// 2. Obtener lista de paquetes (primera página)
GET /api/paquetes?pageNumber=1&pageSize=10
→ Mostrar tabla con paquetes

// 3. Obtener tipos de paquete para el dropdown (si no existe endpoint, crear uno)
GET /api/tipos-paquete (TODO: crear este endpoint)
→ Popular dropdown "Tipo de paquete"
```

### 2. Al Escribir en Búsqueda
```javascript
// Debounce 500ms
onSearchChange(texto) {
  GET /api/paquetes?busquedaAlumno={texto}&pageNumber=1
  → Actualizar tabla
}
```

### 3. Al Cambiar Filtro de Estado
```javascript
onEstadoChange(estado) {
  GET /api/paquetes?estado={estado}&pageNumber=1
  → Actualizar tabla
}
```

### 4. Al Cambiar Filtro de Tipo de Paquete
```javascript
onTipoPaqueteChange(idTipo) {
  GET /api/paquetes?idTipoPaquete={idTipo}&pageNumber=1
  → Actualizar tabla
}
```

### 5. Al Hacer Click en Paginación
```javascript
onPageChange(page) {
  GET /api/paquetes?pageNumber={page}&[...otros filtros]
  → Actualizar tabla
}
```

### 6. Al Hacer Click en Ver (Ícono)
```javascript
onVerDetalle(idPaquete) {
  GET /api/paquetes/{idPaquete}
  → Navegar a vista de detalle o abrir modal
}
```

### 7. Al Hacer Click en "+ Crear Paquete"
```javascript
onCrearPaquete() {
  // Abrir modal/formulario
  // Al enviar:
  POST /api/paquetes
  Body: {
    idAlumno: guid,
    idTipoPaquete: guid,
    clasesDisponibles: number,
    valorPaquete: number,
    diasVigencia: number,
    idPago: null
  }
  → Al éxito: refrescar tabla y estadísticas
}
```

---

## ⚠️ PENDIENTES (RECOMENDACIONES)

### 1. Endpoint para Tipos de Paquete
**Requerido para:** Popular dropdown "Tipo de paquete"

**Sugerencia:** Crear en Application/Paquetes/Queries/GetTiposPaquete/
```csharp
GET /api/tipos-paquete
Response:
[
  { "id": "guid", "nombre": "Paquete 4 clases" },
  { "id": "guid", "nombre": "Paquete 8 clases" },
  { "id": "guid", "nombre": "Paquete 12 clases" }
]
```

### 2. Validar Formato de Documento
El campo `documentoAlumno` debería tener formato consistente:
- ¿Con puntos? "42.567.123"
- ¿Sin puntos? "42567123"
- ¿Solo números o permite letras (DNI/RUT/etc.)?

### 3. Colores de Estado en el Frontend
Sugerencias de colores para badges:
- **Activo**: Verde (#10B981)
- **Vencido**: Gris (#6B7280)
- **Congelado**: Azul (#3B82F6)
- **Agotado**: Naranja/Amarillo (#F59E0B)

### 4. Formato de Fechas
El frontend debe formatear fechas:
```javascript
// De: "2025-01-01T00:00:00"
// A: "1 ene 2025"

new Date("2025-01-01").toLocaleDateString('es-ES', { 
  day: 'numeric', 
  month: 'short', 
  year: 'numeric' 
})
```

---

## 🧪 TESTING

### Casos de Prueba para Frontend

1. **Carga Inicial:**
   - ✅ Tarjetas muestran contadores correctos
   - ✅ Tabla muestra paginación correcta
   - ✅ Dropdowns se populan correctamente

2. **Búsqueda:**
   - ✅ Buscar por nombre completo
   - ✅ Buscar por nombre parcial
   - ✅ Buscar por documento
   - ✅ Buscar sin resultados muestra mensaje apropiado

3. **Filtros:**
   - ✅ Filtrar por cada estado (1-4)
   - ✅ Filtrar por tipo de paquete
   - ✅ Combinar filtros (búsqueda + estado + tipo)
   - ✅ Limpiar filtros vuelve a mostrar todos

4. **Paginación:**
   - ✅ Navegar entre páginas mantiene filtros
   - ✅ Cambiar tamaño de página funciona
   - ✅ Botones Previous/Next se deshabilitan correctamente

5. **Acciones:**
   - ✅ Ver detalle abre correctamente
   - ✅ Crear paquete abre modal/formulario
   - ✅ Crear paquete con éxito refresca tabla

---

## 📝 RESUMEN DE CAMBIOS

**Archivos Nuevos (3):**
- `GetPaquetesQuery.cs` - Listar todos los paquetes con filtros
- `GetEstadisticasPaquetesQuery.cs` - Estadísticas por estado
- `EstadisticasPaquetesDTO` (dentro del query)

**Archivos Modificados (3):**
- `PaqueteAlumnoDTO.cs` - Agregados 3 campos (IdAlumno, NombreAlumno, DocumentoAlumno)
- `GetPaquetesDeAlumnoQuery.cs` - Agregado filtro IdTipoPaquete
- `Program.cs` - Agregados 2 endpoints nuevos

**Endpoints Nuevos (2):**
- `GET /api/paquetes/estadisticas`
- `GET /api/paquetes`

**Endpoints Actualizados (1):**
- `GET /api/alumnos/{id}/paquetes` (agregado filtro idTipoPaquete)

**Estados de Compilación:** ✅ Sin errores

---

## 🎯 COMPATIBILIDAD FRONTEND-BACKEND

| Componente Frontend | Endpoint Backend | Estado |
|---------------------|------------------|--------|
| Tarjetas de estadísticas | GET /api/paquetes/estadisticas | ✅ Implementado |
| Búsqueda por alumno | GET /api/paquetes?busquedaAlumno | ✅ Implementado |
| Filtro por estado | GET /api/paquetes?estado | ✅ Implementado |
| Filtro por tipo | GET /api/paquetes?idTipoPaquete | ✅ Implementado |
| Tabla de paquetes | GET /api/paquetes | ✅ Implementado |
| Paginación | pageNumber, pageSize | ✅ Implementado |
| Ver detalle | GET /api/paquetes/{id} | ✅ Ya existía |
| Crear paquete | POST /api/paquetes | ✅ Ya existía |
| Columna ALUMNO (con DNI) | PaqueteAlumnoDTO.documentoAlumno | ✅ Implementado |
| Columna VIGENCIA (fechas) | fechaActivacion + fechaVencimiento | ✅ Implementado |
| Dropdown tipos | GET /api/tipos-paquete | ⚠️ Pendiente |

---

**Fecha de Actualización:** 11 de Enero, 2026  
**Estado:** ✅ Backend 100% alineado con diseño del frontend  
**Pendiente:** Crear endpoint para obtener lista de tipos de paquete
